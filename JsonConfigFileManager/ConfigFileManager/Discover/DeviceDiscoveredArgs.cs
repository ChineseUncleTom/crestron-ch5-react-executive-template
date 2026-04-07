using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigFileManager.Discover
{
    public class DeviceDiscoveredArgs
    {
        public DiscoveredDevice Device { get; private set; }

        public DeviceDiscoveredArgs(DiscoveredDevice dev)
        {
            Device = dev;
        }
    }
}
