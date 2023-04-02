using FastReport;
using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using ParkingApp.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class ReportsViewModel : INotifyPropertyChanged
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

        public ReportsViewModel()
        {
            parkedCarsDataHandler = new ParkedCarsDataHandler();
            // set end date filer to end of day example '2022-2-2 7:30 AM'
            EndDateFilter = DateTime.Now;
            // Fill data from database to ParkedCarList
            FillDataGrid();

            // Fill Users in to combobox
            usersDataHandler = new UsersDataHandler();
            UserList.Add(new Users {ID = 0, Username = "غير محدد"});
            usersDataHandler.GetUsers(UserList);

        }

        #region  Properties
        private TimeSpan ts = new TimeSpan(7, 30, 0);
        private ParkedCarsDataHandler parkedCarsDataHandler;
        private UsersDataHandler usersDataHandler;

        private string _searchText;
        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;

                OnPropertyChanged("SearchText");
                // Update Datagrid every text chang
                OnPropertyChanged("FilterdParkedCars");
                // update total earning text every text change
                CalcTotalEarnings();
            }
        }
        private DateTime? _startDateFilter;
        public DateTime? StartDateFilter
        {
            get
            {
                return _startDateFilter;
            }
            set
            {
                _startDateFilter = value;
                OnPropertyChanged("StartDateFilter");
                // Update Datagrid every date time chang
                OnPropertyChanged("FilterdParkedCars");
                // update total earning date time text change
                CalcTotalEarnings();

            }
        }
        private DateTime _endDateFilter;
        public DateTime EndDateFilter
        {
            get
            {
                return _endDateFilter;
            }
            set
            {
                _endDateFilter = value.Date + ts;
                OnPropertyChanged("EndDateFilter");
                // Update Datagrid every date time chang
                OnPropertyChanged("FilterdParkedCars");
                // update total earning date time text change
                CalcTotalEarnings();

            }
        }
        private string _totalEarning;
        public string TotalEarning
        {
            get
            {
                return _totalEarning;
            }
            set
            {
                _totalEarning = value;
                OnPropertyChanged("TotalEarning");
            }
        }
        private ParkedCar _selectedParkedCar;
        public ParkedCar SelectedParkedCar
        {
            get
            {
                return _selectedParkedCar;
            }
            set
            {
                _selectedParkedCar = value;
                OnPropertyChanged("SelectedParkedCar");
            }
        }
        private int selectedUserID;
        public int SelectedUserID
        {
            get
            {
                return selectedUserID;
            }
            set
            {
                selectedUserID = value;
                OnPropertyChanged("SelectedUserID");
                OnPropertyChanged("FilterdParkedCars");
                CalcTotalEarnings();
            }
        }
        private bool isNotActiveChecked;
        public bool IsNotActiveChecked
        {
            get
            {
                return isNotActiveChecked;
            }
            set
            {
                isNotActiveChecked = value;
                OnPropertyChanged("IsNotActiveChecked");
                OnPropertyChanged("FilterdParkedCars");
                CalcTotalEarnings();
            }
        }
        #endregion

        #region Lists

        private ObservableCollection<ParkedCar> _parkedCars = new ObservableCollection<ParkedCar>();

        public ObservableCollection<ParkedCar> ParkedCarsList
        {
            get
            {
                return _parkedCars;
            }
            set
            {
                _parkedCars = value;
            }
        }

        public ObservableCollection<Users> UserList { get; set; } = new ObservableCollection<Users>();
        private ObservableCollection<ParkedCar> _filterdParkedCars = new ObservableCollection<ParkedCar>();

        public ObservableCollection<ParkedCar> FilterdParkedCars
        {
            get
            {
                _filterdParkedCars.Clear();
                if (SearchText == null)
                {
                    SearchText = string.Empty;
                }
                var querySplit = SearchText.Split(' ');
                var matchingItems = ParkedCarsList.Where(
                    item =>
                    {
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.Barcode.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    if (StartDateFilter != null)
                    {

                        if (item.StartDate <= Convert.ToDateTime(StartDateFilter).Date + ts)
                        {
                            continue;
                        }

                        if (item.StartDate >= EndDateFilter.AddDays(1).Date + ts)
                        {
                            continue;
                        }
                    }

                    if (SelectedUserID != 0)
                    {
                        if (item.User.ID != SelectedUserID)
                        {
                            continue;
                        }
                    }

                    if (IsNotActiveChecked)
                    {
                        if (item.Status == "نشط")
                        {
                            continue;
                        }
                    }
                    _filterdParkedCars.Add(item);
                }
                return _filterdParkedCars;
            }
            set
            {
                _filterdParkedCars = value;
            }
        }
        #endregion

        #region Commands And Func
        // Fill datagrid from database after 0.5 sec of load page
        private async void FillDataGrid()
        {
            await Task.Delay(500);
            parkedCarsDataHandler.GetAllParkedCars(ParkedCarsList, this);
        }

        public void CalcTotalEarnings()
        {
            TotalEarning = "0";
            double totalEarnings = 0;

            foreach (var parkedCar in FilterdParkedCars)
            {
                totalEarnings += parkedCar.Cost;
                TotalEarning = parkedCar.Cost.ToString("N0");

            }

            TotalEarning = totalEarnings.ToString("N0");
        }


        private DelegateCommand deleteParkedCarCommand;
        public ICommand DeleteParkedCarCommand
        {
            get
            {
                if (deleteParkedCarCommand == null)
                {
                    deleteParkedCarCommand = new DelegateCommand(DeleteParkedCar);
                }

                return deleteParkedCarCommand;
            }
        }

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

        private DelegateCommand disActiveCarCommand;
        public ICommand DisActiveCarCommand
        {
            get
            {
                if (disActiveCarCommand == null)
                {
                    disActiveCarCommand = new DelegateCommand(DisActiveCar);
                }

                return disActiveCarCommand;
            }
        }

        private DelegateCommand printReprotCommand;

        public ICommand PrintReprotCommand
        {
            get
            {
                if (printReprotCommand == null)
                {
                    printReprotCommand = new DelegateCommand(PrintReprot);
                }

                return printReprotCommand;
            }
        }
        private async void AddParkedCar(object commandParameter)
        {
            // show add parked car dialog to add new car to parking
            AddParkedCarDialog dialog = new AddParkedCarDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var selectedCustomer = (Customer)dialog.CustomerCmb.SelectedItem;
                // add the car to list if result was add
                ParkedCarsList.Add(new ParkedCar()
                {
                    ID = parkedCarsDataHandler.GetLastestParkedCarID(),
                    Barcode = dialog.BarcodeTb.Text,
                    StartDate = DateTime.Now,
                    Status = "نشط",
                    Customer = selectedCustomer,
                    CustomerName = selectedCustomer.Name,
                    CustomerHourPrice = selectedCustomer.HourPrice,
                    CustomerHalfHourPrice = selectedCustomer.HalfHourPrice,
                    Number = parkedCarsDataHandler.GetCarNumber()
                });
                OnPropertyChanged("FilterdParkedCars");

            }
            else
            {
                if (App.FirstCarNumber == 1)
                {
                    App.FirstCarNumber = 0;
                }
            }
        }
        private async void DeleteParkedCar(object commandParameter)
        {
            // Show confirmation dialog to delete selected Parked Car
            DefulatContentDialog dlg = new DefulatContentDialog();
            dlg.Title = "حذف";
            dlg.PrimaryButtonText = "حذف";
            dlg.MessageText.Text = "هل تريد حذف العنصر المحدد؟";
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary && SelectedParkedCar != null)
            {
                // remove Parked Car from database
                parkedCarsDataHandler.RemoveParkedCar(SelectedParkedCar.ID);
                // remove Parked Car from UserList
                ParkedCarsList.Remove(SelectedParkedCar);
                CalcTotalEarnings();
                OnPropertyChanged("FilterdParkedCars");

            }

        }
        private async void DisActiveCar(object commandParameter)
        {
            DisActiveParkedCarDialog dialog = new DisActiveParkedCarDialog();
            dialog.DataContext = new DisActiveCarViewModel(SelectedParkedCar);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var index = ParkedCarsList.IndexOf(SelectedParkedCar);
                ParkedCarsList[index].EndDate = Convert.ToDateTime(dialog.EndDateTb.Text);
                ParkedCarsList[index].TotalTime = (dialog.TotalTimeTb.Text.Split(':')[1]);
                ParkedCarsList[index].Cost = Convert.ToDouble(dialog.CostTb.Text);
                ParkedCarsList[index].Status = "غير نشط";
                ParkedCarsList[index].User.ID = App.Users.ID;
                ParkedCarsList[index].User.Username = App.Users.Username;
                CalcTotalEarnings();
                OnPropertyChanged("FilterdParkedCars");

            }

        }


        private void PrintReprot(object commandParameter)
        {
            try
            {
                Report report = new Report();

                report.Load(App.LoadEmbeddedReport("ParkingApp.Reports.ParkedCarsReport.frx"));
                report.RegisterData(FilterdParkedCars, "ParkedCars");
                if (StartDateFilter == null)
                {
                    report.SetParameterValue("StartDate", "---");
                }
                else
                {
                    report.SetParameterValue("StartDate", Convert.ToDateTime(StartDateFilter).Date + ts);

                }
                report.SetParameterValue("EndDate", EndDateFilter.AddDays(1));
                report.Prepare();
                report.ShowPrepared();
                report.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }
        #endregion

    }
}
