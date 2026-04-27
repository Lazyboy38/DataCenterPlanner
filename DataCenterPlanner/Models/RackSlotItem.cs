namespace DataCenterPlanner.Models
{
    public class RackSlotItem
    {
        private const double PixelsPerUnit = 14.0;

        public int StartU { get; set; }
        public int HeightU { get; set; }
        public string Name { get; set; } = "";
        public string ColorHex { get; set; } = "#2FA8FF";
        public string LabelColorHex { get; set; } = "White";
        public bool IsFree { get; set; }

        public string ULabel => HeightU == 1
            ? $"U{StartU}"
            : $"U{StartU}–{StartU + HeightU - 1}";

        public double DisplayHeight => HeightU * PixelsPerUnit;
        public string HeightLabel => $"{HeightU}U";

        public double LabelFontSize => HeightU switch
        {
            >= 7 => 13.0,
            >= 3 => 11.0,
            >= 2 => 9.5,
            _    => 8.5
        };
    }
}
