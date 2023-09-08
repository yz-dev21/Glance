namespace Glance
{
    internal struct BatteryInfo
    {
        public int Percentage { get; private set; }
        public bool Charging { get; private set; }

        public BatteryInfo()
        {
            Percentage = 0;
            Charging = false;
        }
        public void Update()
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;

            Percentage = (int)(powerStatus.BatteryLifePercent * 100);
            Charging = powerStatus.PowerLineStatus == PowerLineStatus.Online;
        }
        public override readonly string ToString()
        {
            if (Charging)
                return $"{Percentage}%, Charging";
            else
                return $"{Percentage}%";
        }
    }
}