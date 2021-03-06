﻿// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;

namespace Microsoft.Azure.IoTSolutions.UIConfig.Services.Models
{
    public class CacheValue
    {
        public bool Rebuilding { get; set; } = false;

        public HashSet<string> Tags { get; set; }

        public HashSet<string> Reported { get; set; }

        public bool IsNullOrEmpty() => (Tags == null || Tags.Count == 0) && (Reported == null || Reported.Count == 0);
    }
}
