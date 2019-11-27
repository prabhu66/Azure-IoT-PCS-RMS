// Copyright (c) Microsoft. All rights reserved.

using Microsoft.Azure.IoTSolutions.Auth.Services.Models;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.Auth.WebService.v1.Models
{
    public class UserApiModel
    {
        [JsonProperty(PropertyName = "Id", Order = 10)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "Email", Order = 20)]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Name", Order = 30)]
        public string Name { get; set; }

        public UserApiModel(User user)
        {
            this.Id = user.Id;
            this.Email = user.Email;
            this.Name = user.Name;
        }
    }
}
