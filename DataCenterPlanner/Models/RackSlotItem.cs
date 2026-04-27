namespace DataCenterPlanner.Models
{
    public class RackSlotItem
    {
        private const double PixelsPerUnit = 10.0;

        public int StartU { get; set; }
        public int HeightU { get; set; }
        public string Name { get; set; } = "";
        public string ColorHex { get; set; } = "#2FA8FF";
        public bool IsFree { get; set; }

        public string ULabel => HeightU == 1
            ? $"U{StartU}"
            : $"U{StartU}–{StartU + HeightU - 1}";

        public double DisplayHeight => HeightU * PixelsPerUnit;
        public string HeightLabel => $"{HeightU}U";
    }
}
