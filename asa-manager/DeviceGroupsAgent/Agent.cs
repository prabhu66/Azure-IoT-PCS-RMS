// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.IoTSolutions.AsaManager.DeviceGroupsAgent.Models;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Concurrency;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Diagnostics;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.EventHub;
using Microsoft.Azure.IoTSolutions.AsaManager.Services.Runtime;

namespace Microsoft.Azure.IoTSolutions.AsaManager.DeviceGroupsAgent
{
    public interface IAgent
    {
        Task RunAsync(CancellationToken runState);
    }

    public class Agent : IAgent
    {
        // ASA cannot have new reference data more than once per minute
        private const int CHECK_INTERVAL_MSECS = 60000;

        private readonly ILogger log;
        private readonly IDeviceGroupsWriter deviceGroupsWriter;
        private readonly IDeviceGroupsClient deviceGroupsClient;
        private readonly IEventHubStatus eventHubStatus;
        private readonly IEventProcessorHostWrapper eventProcessorHostWrapper;
        private readonly IEventProcessorFactory deviceEventProcessorFactory;
        private readonly IServicesConfig servicesConfig;
        private readonly IBlobStorageConfig blobStorageConfig;
        private readonly Dictionary<string, string> deviceGroupDefinitionDictionary;
        private readonly IThreadWrapper thread;

        public Agent(
           IDeviceGroupsWriter deviceGroupsWriter,
           IDeviceGroupsClient deviceGroupsClient,
           IEventProcessorHostWrapper eventProcessorHostWrapper,
           IEventProcessorFactory deviceEventProcessorFactory,
           IEventHubStatus eventHubStatus,
           IServicesConfig servicesConfig,
           IBlobStorageConfig blobStorageConfig,
           ILogger logger,
           IThreadWrapper thread)
        {
            this.log = logger;
            this.deviceGroupsWriter = deviceGroupsWriter;
            this.deviceGroupsClient = deviceGroupsClient;
            this.eventProcessorHostWrapper = eventProcessorHostWrapper;
            this.deviceEventProcessorFactory = deviceEventProcessorFactory;
            this.eventHubStatus = eventHubStatus;
            this.servicesConfig = servicesConfig;
            this.blobStorageConfig = blobStorageConfig;
            this.deviceGroupDefinitionDictionary = new Dictionary<string, string>();
            this.thread = thread;
        }

        public async Task RunAsync(CancellationToken runState)
        {
            this.log.Info("Device Groups Agent running", () => { });

            // ensure will do initial write even if there are no device group definitions
            bool forceWrite = true;

            // IotHub has some latency between reporting a device is created/updated and when
            // the API returns the updates. This flag will tell the service to write
            // again a minute after changes have been seen,
            // to ensures if there are updates they are not missed.
            bool previousEventHubSeenChanges = this.eventHubStatus.SeenChanges;

            if (!runState.IsCancellationRequested)
            {
                try
                {
                    await this.SetupEventHub();
                }
                catch (Exception e)
                {
                    this.log.Error("Received error setting up event hub. Will not receive updates from devices", () => new { e });
                }
            }

            while (!runState.IsCancellationRequested)
            {
                try
                {
                    // check device groups
                    DeviceGroupListApiModel deviceGroupList = await this.deviceGroupsClient.GetDeviceGroupsAsync();
                    bool deviceGroupsChanged = this.DidDeviceGroupDefinitionsChange(deviceGroupList);
                    if (forceWrite || deviceGroupsChanged || this.eventHubStatus.SeenChanges || previousEventHubSeenChanges)
                    {
                        previousEventHubSeenChanges = this.eventHubStatus.SeenChanges;

                        // set status before update so if message is received during update, will update again in a minute
                        this.eventHubStatus.SeenChanges = false;

                        // update
                        if (deviceGroupsChanged)
                        {
                            this.UpdateDeviceGroupDefinitionDictionary(deviceGroupList);
                        }

                        var deviceGroupMapping = await this.deviceGroupsClient.GetGroupToDevicesMappingAsync(deviceGroupList);
                        await this.deviceGroupsWriter.ExportMapToReferenceDataAsync(deviceGroupMapping, DateTimeOffset.UtcNow);
                        forceWrite = false;
                    }
                }
                catch (Exception e)
                {
                    this.log.Error("Received error updating device to device group mapping", () => new { e });
                    forceWrite = true;
                }

                this.thread.Sleep(CHECK_INTERVAL_MSECS);
            }
        }

        private async Task SetupEventHub()
        {
            string storageConnectionString =
                $"DefaultEndpointsProtocol=https;AccountName={this.blobStorageConfig.AccountName};AccountKey={this.blobStorageConfig.AccountKey};EndpointSuffix={this.blobStorageConfig.EndpointSuffix}";
            var eventProcessorHost = this.eventProcessorHostWrapper.CreateEventProcessorHost(
                this.servicesConfig.EventHubName,
                PartitionReceiver.DefaultConsumerGroupName,
                this.servicesConfig.EventHubConnectionString,
                storageConnectionString,
                this.blobStorageConfig.EventHubContainer);
            await this.eventProcessorHostWrapper.RegisterEventProcessorFactoryAsync(eventProcessorHost, this.deviceEventProcessorFactory);
        }

        private void UpdateDeviceGroupDefinitionDictionary(DeviceGroupListApiModel deviceGroupList)
        {
            this.deviceGroupDefinitionDictionary.Clear();
            foreach (DeviceGroupApiModel deviceGroup in deviceGroupList.Items)
            {
                this.deviceGroupDefinitionDictionary[deviceGroup.Id] = deviceGroup.ETag;
            }
        }

        private bool DidDeviceGroupDefinitionsChange(DeviceGroupListApiModel newDeviceGroupList)
        {
            if (newDeviceGroupList.Items.Count() != this.deviceGroupDefinitionDictionary.Keys.Count)
            {
                return true;
            }

            foreach (DeviceGroupApiModel deviceGroup in newDeviceGroupList.Items)
            {
                if (!this.deviceGroupDefinitionDictionary.ContainsKey(deviceGroup.Id) ||
                    !this.deviceGroupDefinitionDictionary[deviceGroup.Id].Equals(deviceGroup.ETag))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
