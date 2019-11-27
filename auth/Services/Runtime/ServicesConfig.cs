// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.Auth.Services.Runtime
{
    public interface IServicesConfig
    {
        IEnumerable<string> JwtUserIdFrom { get; set; }
        IEnumerable<string> JwtNameFrom { get; set; }
        IEnumerable<string> JwtEmailFrom { get; set; }
    }

    public class ServicesConfig : IServicesConfig
    {
        public IEnumerable<string> JwtUserIdFrom { get; set; }
        public IEnumerable<string> JwtNameFrom { get; set; }
        public IEnumerable<string> JwtEmailFrom { get; set; }
    }
}
