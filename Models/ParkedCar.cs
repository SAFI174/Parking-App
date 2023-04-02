using System;
using System.ComponentModel;
using System.Windows;

namespace ParkingApp.Models
{
    public class ParkedCar : INotifyPropertyChanged
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

        public int ID { get; set; }
        public int Number { get; set; }

        private string barcode;
        public string Barcode
        {
            get { return barcode; }
            set { barcode = value; OnPropertyChanged("Barcode"); }
        }
        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { startDate = value; OnPropertyChanged("StartDate"); }
        }
        private DateTime? endDate;
        public DateTime? EndDate
        {
            get { return endDate; }
            set { endDate = value; OnPropertyChanged("EndDate"); }
        }
        private string totalTime;
        public string TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; OnPropertyChanged("TotalTime"); }
        }
        private double cost;
        public double Cost
        {
            get { return cost; }
            set { cost = value; OnPropertyChanged("Cost"); }
        }
        private string customerName;
        public string CustomerName
        {
            get { return customerName; }
            set { customerName = value; OnPropertyChanged("CustomerName"); }
        }
        private double customerHourPrice;
        public double CustomerHourPrice
        {
            get { return customerHourPrice; }
            set { customerHourPrice = value; OnPropertyChanged("CustomerHourPrice"); }
        }
        private double customerHalfHourPrice;
        public double CustomerHalfHourPrice
        {
            get { return customerHalfHourPrice; }
            set { customerHalfHourPrice = value; OnPropertyChanged("CustomerHalfHourPrice"); }
        }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { customer = value; OnPropertyChanged("Customer"); }
        }
        private Users user;
        public Users User
        {
            get { return user; }
            set { user = value; OnPropertyChanged("User"); }
        }
        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }
        private Visibility isactive;
        public Visibility isActive
        {
            get
            {
                if (Status == "نشط")
                {
                    isactive = Visibility.Visible;
                }
                else
                {
                    isactive = Visibility.Collapsed;
                }
                return isactive;
            }
            set
            {
                isactive = value; OnPropertyChanged("isActive");
            }
        }
    }
}
