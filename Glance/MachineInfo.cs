using System.Management;

namespace Glance
{
    internal class MachineInfo
    {
        public string OSVersion { get; private set; }
        public string DesktopName { get; private set; }
        public string UserName { get; private set; }
        public MachineInfo()
        {
            OSVersion = string.Empty;
            DesktopName = string.Empty;
            UserName = string.Empty;
        }
        public void Update()
        {
            var osVersion = (from x in new ManagementObjectSearcher("SELECT Caption FROM Win32_OperatingSystem").Get().Cast<ManagementObject>()
                             select x.GetPropertyValue("Caption")).FirstOrDefault();
            if (osVersion == null)
                OSVersion = "Unknown";
            else
            {
                OSVersion = osVersion.ToString() + (Environment.Is64BitOperatingSystem ? " x64" : " x32");
            }
            DesktopName = Environment.MachineName;
            UserName = Environment.UserName;
        }
    }
}
