using System.ComponentModel;

namespace DataCenterPlanner.Models
{
    public class ShoppingItem : INotifyPropertyChanged
    {
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => Quantity * UnitPrice;

        private bool _isBought;
        public bool IsBought
        {
            get => _isBought;
            set { _isBought = value; OnPropertyChanged(nameof(IsBought)); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}