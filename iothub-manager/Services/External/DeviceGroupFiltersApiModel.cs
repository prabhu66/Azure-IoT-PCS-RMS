// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.Services.External
{
    public class DeviceGroupFiltersApiModel
    {
        [JsonProperty("Tags")]
        public HashSet<string> Tags { get; set; }

        [JsonProperty("Reported")]
        public HashSet<string> Reported { get; set; }
    }
}
