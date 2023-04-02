using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;


namespace ParkingApp.ViewModel
{
    public class DisActiveCarViewModel : INotifyPropertyChanged
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

        public DisActiveCarViewModel(ParkedCar parkedCar)
        {
            _parkedCarsDataHandler = new ParkedCarsDataHandler();

            // fill data from selected row
            ID = parkedCar.ID;
            Number = parkedCar.Number;
            StartDate = parkedCar.StartDate;
            Barcode = parkedCar.Barcode;
            Customer = parkedCar.Customer;
            isPrintChecked = false;
            CustomerHourPrice = parkedCar.CustomerHourPrice.ToString("N0");

            //set price
            hourprice = parkedCar.CustomerHourPrice;
            halfhourprice = parkedCar.CustomerHalfHourPrice;
            
            // calc total time and prices
            CalculateTotalTimeAndPrice();

            // Timer for calculate total time
            refreshTime = new DispatcherTimer();
            refreshTime.Tick += new EventHandler(RefreshTotalTime_Tick);
            refreshTime.Interval = new TimeSpan(0, 0, 1);
            refreshTime.Start();
        }

        #region Properties

        // Data provider for parked cars
        private ParkedCarsDataHandler _parkedCarsDataHandler;
        // time span to extract days and hours and minutes
        private TimeSpan totalTimeSpan;
        //// create timer to refresh time and prices
        private DispatcherTimer refreshTime;
        // hour price
        private double hourprice;
        private double halfhourprice;

        // Prop
        private int ID;

        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value; OnPropertyChanged("Number");
            }
        }
        private string _barcode;
        public string Barcode
        {
            get
            {
                return _barcode;
            }
            set
            {
                _barcode = value; OnPropertyChanged("Barcode");
            }
        }
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value; OnPropertyChanged("StartDate");
            }
        }
        private DateTime _endDate;
        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value; OnPropertyChanged("EndDate");
            }
        }
        private string _totalTime;
        public string TotalTime
        {
            get
            {
                return _totalTime;
            }
            set
            {
                _totalTime = value; OnPropertyChanged("TotalTime");
            }
        }
        private string _customerHourPrice;
        public string CustomerHourPrice
        {
            get
            {
                return _customerHourPrice;
            }
            set
            {
                _customerHourPrice = value; OnPropertyChanged("CustomerHourPrice");
            }
        }
        private double _cost;
        public double Cost
        {
            get
            {
                return _cost;
            }
            set
            {
                _cost = value; OnPropertyChanged("Cost");
            }
        }
        private Customer _customer;
        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }
        private bool _isPrintChecked;
        public bool isPrintChecked
        {
            get
            {
                return _isPrintChecked;
            }
            set
            {
                _isPrintChecked = value; OnPropertyChanged("isPrintChecked");
            }
        }

        #endregion

        #region Functions

        // Refresh time and prices every 1 sec
        private void RefreshTotalTime_Tick(object sender, EventArgs e)
        {
            Cost = 0;

            CalculateTotalTimeAndPrice();
        }

        // calc total time and update prices
        private void CalculateTotalTimeAndPrice()
        {
            // Update End Date 
            EndDate = DateTime.Now;
            // extract diff between start date and end date
            totalTimeSpan = DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(StartDate.ToString()));
            // if days and hours < 0 then check if minutes < 15 => cost = 0 or cost = price
            if (totalTimeSpan.Days <= 0 && totalTimeSpan.Hours <= 0)
            {
                TotalTime = ($"{totalTimeSpan.Minutes} دقيقة");
                if (totalTimeSpan.TotalMinutes <= 15)
                {
                    Cost = 0;
                }
                else
                {
                    Cost = hourprice;
                }
            }
            // if hours > 0
            else if (totalTimeSpan.Days <= 0 && totalTimeSpan.Hours != 0)
            {

                // update text of the total time
                TotalTime = ($"{totalTimeSpan.Hours} ساعة {totalTimeSpan.Minutes} دقيقة");
                // reset cost value to recalculate the cost
                Cost = 0;
                // extract diff between start date and end date
                totalTimeSpan = DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(StartDate.ToString()));
                //  if minutes = 0 and hours > 0 then => cost = price * hours
                Cost = (halfhourprice * Convert.ToInt16(totalTimeSpan.TotalHours - 1) * 2) + hourprice;
   
                if (totalTimeSpan.Minutes <= 30 && totalTimeSpan.Minutes > 0)
                {
                    Cost += halfhourprice;


                }
                //// if minutes between 0, 30 then cost will increase by price / 2
                //if (totalTimeSpan.Minutes <= 30 && totalTimeSpan.Minutes > 0)
                //{
                //    Cost = hourprice * Convert.ToInt16(totalTimeSpan.TotalHours);

                //    Cost += hourprice / 2;
                //}
                //// if minutes between 0, 30 then cost will increase by price per hour
                //else if (totalTimeSpan.Minutes > 30)
                //{
                //    Cost = hourprice * Convert.ToInt16(totalTimeSpan.TotalHours);
                //}

            }
            // if days > 0 then cost 
            else if (totalTimeSpan.Days > 0)
            {
                TotalTime = ($"{totalTimeSpan.Days} يوم  {totalTimeSpan.Hours} ساعة {totalTimeSpan.Minutes} دقيقة");
                // if minutes between 0, 30 then cost will increase by price / 2
                Cost = (halfhourprice * Convert.ToInt16(totalTimeSpan.TotalHours - 1) * 2) + hourprice;
                Console.WriteLine(totalTimeSpan.TotalHours);
                if (totalTimeSpan.Minutes <= 30 && totalTimeSpan.Minutes > 0)
                {
                    Cost += halfhourprice;
                }
                //// if minutes between 30, 60 then cost will increase by price per hour
                //else
                //{
                //    Cost = hourprice * Convert.ToInt16(totalTimeSpan.TotalHours);
                //}
            }
        }
        #endregion

        #region Commands



        private DelegateCommand disActiveParkedCarCommand;
        public ICommand DisActiveParkedCarCommand
        {
            get
            {
                if (disActiveParkedCarCommand == null)
                {
                    disActiveParkedCarCommand = new DelegateCommand(disActiveParkedCar);
                }

                return disActiveParkedCarCommand;
            }
        }


        private void disActiveParkedCar(object commandParameter)
        {
            // extract datetime
            totalTimeSpan = DateTime.Parse(DateTime.Now.ToString()).Subtract(DateTime.Parse(StartDate.ToString()));
            // disActiveCar edit data on database
            _parkedCarsDataHandler.DisActiveCar(ID, totalTimeSpan.TotalMinutes.ToString(), Cost);
        }


        #endregion

    }
}
