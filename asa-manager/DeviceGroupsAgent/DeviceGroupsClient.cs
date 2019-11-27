// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.IoTSolutions.AsaManager.DeviceGroupsAgent.Models;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Concurrency;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Exceptions;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Http;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.AsaManager.DeviceGroupsAgent
{
    public interface IDeviceGroupsClient
    {
        Task<Dictionary<string, IEnumerable<string>>> GetGroupToDevicesMappingAsync(DeviceGroupListApiModel deviceGroupList);
        Task<DeviceGroupListApiModel> GetDeviceGroupsAsync();
    }

    public class DeviceGroupsClient : IDeviceGroupsClient
    {
        private readonly ILogger logger;
        private readonly IHttpClient httpClient;
        private readonly IDevicesClient devices;
        private readonly IServicesConfig servicesConfig;
        private readonly string baseUrl;
        private readonly IThreadWrapper thread;

        public DeviceGroupsClient(
            IHttpClient httpClient,
            IDevicesClient devices,
            IServicesConfig servicesConfig,
            ILogger logger,
            IThreadWrapper thread)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            this.devices = devices;
            this.baseUrl = $"{servicesConfig.ConfigServiceUrl}/devicegroups";
            this.servicesConfig = servicesConfig;
            this.thread = thread;
        }

        /**
         * Given a list of device group definitions, queries for the list of devices
         * for each group and returns a dictionary of group id -> device ids
         */
        public async Task<Dictionary<string, IEnumerable<string>>> GetGroupToDevicesMappingAsync(DeviceGroupListApiModel deviceGroupList)
        {
            var groupToDeviceMapping = new Dictionary<string, IEnumerable<string>>();
            if (deviceGroupList?.Items != null)
            {
                // TODO: Add retry if this call fails
                // https://github.com/Azure/asa-manager-dotnet/issues/7
                foreach (DeviceGroupApiModel group in deviceGroupList.Items)
                {
                    groupToDeviceMapping.Add(group.Id, await this.GetDevicesAsync(group));
                }
            }
            return groupToDeviceMapping;
        }

        /**
         * Queries for the list of device group definitions and returns the list
         */
        public async Task<DeviceGroupListApiModel> GetDeviceGroupsAsync()
        {
            try
            {
                return await this.httpClient.GetJsonAsync<DeviceGroupListApiModel>($"{this.baseUrl}/", $"get device groups", true);
            }
            catch (Exception e)
            {
                this.logger.Error("Failed to get list of device groups", () => new { e });
                throw new ExternalDependencyException("Unable to get list of device groups", e);
            }
        }

        /**
         * Given a device group definition, returns the list of device ids in the group.
         * Will retry up to MAX_RETRY_COUNT if there is a failure, doubling the sleep time
         * on each retry.
         */
        private async Task<IEnumerable<string>> GetDevicesAsync(DeviceGroupApiModel group)
        {
            int retryCount = 0;
            Exception innerException = null;
            int retryPause = this.servicesConfig.InitialIotHubManagerRetryIntervalMs;

            while (retryCount < this.servicesConfig.IotHubManagerRetryCount)
            {
                if (retryCount > 0)
                {
                    this.thread.Sleep(retryPause);
                    retryPause *= this.servicesConfig.IotHubManagerRetryIntervalIncreaseFactor;
                }

                try
                {
                    if (@group?.Conditions == null)
                    {
                        this.logger.Error("Device group definitions or conditions were null", () => new { });
                        return new string[] { };
                    }

                    return await this.devices.GetListAsync(group.Conditions);
                }
                catch (Exception e)
                {
                    retryCount++;
                    string errorMessage = $"Failed to get list of devices. {this.servicesConfig.IotHubManagerRetryCount - retryCount} retries remaining.";
                    this.logger.Warn(errorMessage, () => new { e });
                    innerException = e;
                }
            }

            this.logger.Error("Failed to get list of devices after retrying", () => new { innerException, this.servicesConfig.IotHubManagerRetryCount });
            throw new ExternalDependencyException("Unable to get list of devices", innerException);
        }
    }
}
