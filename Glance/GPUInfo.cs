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
        }
        public void Update()
        {
            using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
            {
                foreach (var obj in searcher.Get())
                {
                    Name = obj["Name"] != null ? obj["Name"].ToString() : "Unknown";
                    DriverVersion = obj["DriverVersion"] != null ? obj["DriverVersion"].ToString() : "Unknown";
                }
            }
        }
    }
}