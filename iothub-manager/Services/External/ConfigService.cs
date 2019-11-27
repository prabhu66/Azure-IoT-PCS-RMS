// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Extensions;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Helpers;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Models;
using Microsoft.Azure.IoTSolutions.IotHubManager.Services.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.Services.External
{
    public interface IConfigService
    {
        Task UpdateDeviceGroupFiltersAsync(DeviceTwinServiceModel twin);
    }

    public class ConfigService : IConfigService
    {
        private readonly string url;
        private readonly IHttpClientWrapper httpClient;

        public ConfigService(
            IServicesConfig config,
            IHttpClientWrapper httpClient)
        {
            this.url = $"{config.ConfigServiceUri}/devicegroupfilters";
            this.httpClient = httpClient;
        }

        public async Task UpdateDeviceGroupFiltersAsync(DeviceTwinServiceModel twin)
        {
            var model = new DeviceGroupFiltersApiModel();

            var tagRoot = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(twin.Tags)) as JToken;
            if (tagRoot != null)
            {
                model.Tags = new HashSet<string>(tagRoot.GetAllLeavesPath());
            }

            var reportedRoot = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(twin.ReportedProperties)) as JToken;
            if (reportedRoot != null)
            {
                model.Reported = new HashSet<string>(reportedRoot.GetAllLeavesPath());
            }

            await this.httpClient.PostAsync(this.url, "DeviceGroupFilters", model);
        }
    }
}
