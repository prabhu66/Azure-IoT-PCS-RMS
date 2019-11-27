// Copyright (c) Microsoft. All rights reserved.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.IoTSolutions.Auth.Services;
using Microsoft.Azure.IoTSolutions.Auth.WebService.Auth;
using Microsoft.Azure.IoTSolutions.Auth.WebService.v1.Filters;
using Microsoft.Azure.IoTSolutions.Auth.WebService.v1.Models;

namespace Microsoft.Azure.IoTSolutions.Auth.WebService.v1.Controllers
{
    [Route(Version.PATH + "/[controller]"), TypeFilter(typeof(ExceptionsFilterAttribute))]
    public class UsersController : Controller
    {
        private readonly IUsers users;

        public UsersController(IUsers users)
        {
            this.users = users;
        }

        [HttpGet("{id}")]
        public UserApiModel Get(string id)
        {
            var user = this.users.GetUserInfo(this.Request.GetCurrentUserClaims());

            if (id != "current" && id != user.Id) return null;

            return new UserApiModel(user);
        }
    }
}
