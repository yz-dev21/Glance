using System.Management;

namespace Glance
{
    internal class CPUInfo
    {
        public string Name { get; private set; }

        public CPUInfo()
        {
            Name = string.Empty;
        }
        public void Update()
        {
            var cpuName = (from x in new ManagementObjectSearcher("SELECT Name FROM Win32_Processor").Get().Cast<ManagementObject>()
                           select x.GetPropertyValue("Name")).FirstOrDefault();
            Name = cpuName != null ? cpuName.ToString() : "Unknown";
        }
    }
}
