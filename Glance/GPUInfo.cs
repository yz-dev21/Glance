using System;
using System.Management;

namespace Glance
{
    internal class GPUInfo
    {
        public string Name { get; private set; }
        public string DriverVersion { get; private set; }
        public GPUInfo()
        {
            Name = string.Empty;
            DriverVersion = string.Empty;

            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                var collection = searcher.Get();
                ManagementObject[] videoControllers = new ManagementObject[collection.Count];

                collection.CopyTo(videoControllers, 0);

                Name = videoControllers[0]["Name"] != null ? videoControllers[0]["Name"].ToString() : "Unknown";
                DriverVersion = videoControllers[0]["DriverVersion"] != null ? videoControllers[0]["DriverVersion"].ToString() : "Unknown";
            }
        }
        public void Update()
        {

        }
    }
}