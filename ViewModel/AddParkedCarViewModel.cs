using FastReport;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Input;


namespace ParkingApp.ViewModel
{
    public class AddParkedCarViewModel : INotifyPropertyChanged
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

        public AddParkedCarViewModel()
        {
            _parkedCarsDataHandler = new ParkedCarsDataHandler();
            _customerDataHandler = new CustomerDataHandler();
            // fill Customer List  => Customers ComboBox
            _customerDataHandler.GetCustomers(CustomersList);

            // Set new data to the new parked car
           
            Number = _parkedCarsDataHandler.GetCarNumber() + 1;
            Barcode = Guid.NewGuid().ToString("N").Remove(10);
            StartDate = DateTime.Now.ToString();
            isPrintChecked = true;

            // set the car number for new car
            if (App.FirstCarNumber == 1)
            {
                Number = _parkedCarsDataHandler.GetCarNumber() + 1;
                return;
            }
            App.FirstCarNumber = _parkedCarsDataHandler.GetCarNumber();
            if (App.FirstCarNumber == 1)
            {
                Number = 1;
            }
        }

        #region Lists
        private ObservableCollection<Customer> _customersList = new ObservableCollection<Customer>();

        public ObservableCollection<Customer> CustomersList
        {
            get
            {
                return _customersList;
            }
            set
            {
                CustomersList = value;
            }
        }


        #endregion

        #region Properties
        private ParkedCarsDataHandler _parkedCarsDataHandler;
        private CustomerDataHandler _customerDataHandler;
        private int _number;
        public int Number
        {
            get
            {
                return _number;
            }
            set
            {
                _number = value;
                OnPropertyChanged("Number");
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
                _barcode = value;
                OnPropertyChanged("Barcode");
            }
        }
        private string _startDate;
        public string StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                OnPropertyChanged("StartDate");
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
                _isPrintChecked = value;
                OnPropertyChanged("isPrintChecked");
            }
        }
        private Customer customer;
        public Customer Customer
        {
            get { return customer; }
            set { customer = value; OnPropertyChanged("Customer"); }
        }
        #endregion

        #region Commands
        private DelegateCommand addParkedCarCommand;
        public ICommand AddParkedCarCommand
        {
            get
            {
                if (addParkedCarCommand == null)
                {
                    addParkedCarCommand = new DelegateCommand(AddParkedCar);
                }

                return addParkedCarCommand;
            }
        }
        private void AddParkedCar(object commandParameter)
        {
            try
            {
                // add parked car to the database
                _parkedCarsDataHandler.AddParkedCar(Barcode, Customer.Id);
                // print bill
                PrintBill();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
          

        }

        private void PrintBill()
        {
            if (_isPrintChecked)
            {
                Report report = new Report();
                report.Load(App.LoadEmbeddedReport("ParkingApp.Reports.ParkingBill.frx"));
                report.SetParameterValue("CarNumber", Number);
                report.SetParameterValue("StartDate", DateTime.Now.ToString(new CultureInfo("en-us")));
                report.SetParameterValue("Barcode", Barcode);
                report.Prepare();
                report.PrintSettings.ShowDialog = false;
                report.Print();
                report.Dispose();
            }

        }
        #endregion

    }
}
