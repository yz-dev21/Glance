using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glance
{
    internal class DiskInfo
    {
        public DriveInfo[] Drives;
        public DiskInfo()
        {
            Drives = DriveInfo.GetDrives();
        }
        public static float ToGB(long bytes)
        {
            return bytes / (1024 * 1024 * 1024);
        }
    }
}
