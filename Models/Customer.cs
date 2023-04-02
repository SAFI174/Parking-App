using System.ComponentModel;

namespace ParkingApp.Models
{
    public class Customer : INotifyPropertyChanged
    {
        #region PropertyChanegd
        // PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #endregion

        public int Id { get; set; }

        private string name;
        public string Name
        {
            get => name;
            set { name = value; OnPropertyChanged("Name"); }
        }

        private double hourPrice;
        public double HourPrice
        {
            get => hourPrice;
            set { hourPrice = value; OnPropertyChanged("HourPrice"); }
        }
        private double halfHourPrice;
        public double HalfHourPrice
        {
            get => halfHourPrice;
            set { halfHourPrice = value; OnPropertyChanged("HalfHourPrice"); }
        }
    }
}
