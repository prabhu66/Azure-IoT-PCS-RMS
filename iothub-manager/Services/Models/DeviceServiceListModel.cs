// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.IotHubManager.Services.Models
{
    public class DeviceServiceListModel
    {
        public string ContinuationToken { get; set; }

        public List<DeviceServiceModel> Items { get; set; }

        public DeviceServiceListModel(IEnumerable<DeviceServiceModel> devices, string continuationToken = null)
        {
            this.ContinuationToken = continuationToken;
            this.Items = new List<DeviceServiceModel>(devices);
        }
    }
}
