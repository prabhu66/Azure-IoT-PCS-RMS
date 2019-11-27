// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.Azure.IoTSolutions.Auth.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.Auth.Services.Models;
using Microsoft.Azure.IoTSolutions.Auth.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.Auth.Services
{
    public interface IUsers
    {
        User GetUserInfo(IEnumerable<Claim> claims);
    }

    public class Users : IUsers
    {
        private readonly ILogger log;
        private readonly IServicesConfig config;

        public Users(
            IServicesConfig config,
            ILogger log)
        {
            this.config = config;
            this.log = log;
        }

        public User GetUserInfo(IEnumerable<Claim> claims)
        {
            // Map all the claims into a dictionary
            var data = new Dictionary<string,string>();
            foreach (var c in claims)
            {
                data[c.Type.ToLowerInvariant()] = c.Value;
            }

            // Extract user information from the claims
            var id = this.config.JwtUserIdFrom
                .Select(key => key.ToLowerInvariant())
                .Where(k => data.ContainsKey(k))
                .Aggregate("", (current, k) => current + (data[k] + ' '))
                .TrimEnd();
            var name = this.config.JwtNameFrom
                .Select(key => key.ToLowerInvariant())
                .Where(k => data.ContainsKey(k))
                .Aggregate("", (current, k) => current + (data[k] + ' '))
                .TrimEnd();
            var email = this.config.JwtEmailFrom
                .Select(key => key.ToLowerInvariant())
                .Where(k => data.ContainsKey(k))
                .Aggregate("", (current, k) => current + (data[k] + ' '))
                .TrimEnd();

            if (string.IsNullOrEmpty(id)) id = "-unknown-";
            if (string.IsNullOrEmpty(name)) name = "user name unknown";
            if (string.IsNullOrEmpty(email)) email = "email address unknown";

            return new User
            {
                Id = id,
                Name = name,
                Email = email
            };
        }
    }
}
