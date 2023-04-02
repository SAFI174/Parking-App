using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using ParkingApp.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class ParkingViewModel : INotifyPropertyChanged
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
        public ParkingViewModel()
        {
            _parkedCarsDataHandler = new ParkedCarsDataHandler();
            // get active parked cars only from database to ParkedCarsList => ItemsRepeater
            _parkedCarsDataHandler.GetActiveParkingCars(ParkedCarsList);
        }

        #region Properties
        private ParkedCarsDataHandler _parkedCarsDataHandler;


        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged("SearchText"); OnPropertyChanged("FilterParkedCarsList"); AutoDisActiveCar(); }
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
        // filterd ParkedCarsList with Search Text => barcode
        private ObservableCollection<ParkedCar> _filterparkedCars = new ObservableCollection<ParkedCar>();
        public ObservableCollection<ParkedCar> FilterParkedCarsList
        {
            get
            {
                _filterparkedCars.Clear();
                if (searchText == null)
                {
                    searchText = string.Empty;
                }

                var querySplit = searchText.Split(' ');
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
                foreach (var item in matchingItems.Reverse())
                {
                    _filterparkedCars.Add(item);
                }

                return _filterparkedCars;
            }
            set
            {

                _filterparkedCars = value;
                OnPropertyChanged("FilterParkedCarsList");

            }
        }
        #endregion

        #region Commands
        // Dis Active Car if barcode hit and there is 1 item on the filterd list and search text is not empty
        private void AutoDisActiveCar()
        {
            if (FilterParkedCarsList.Count == 1 && !string.IsNullOrEmpty(SearchText))
            {
                DisActiveParkedCar(null);
            }
        }

        private DelegateCommand disActiveParkedCarCommand;
        public ICommand DisActiveParkedCarCommand
        {
            get
            {
                if (disActiveParkedCarCommand == null)
                {
                    disActiveParkedCarCommand = new DelegateCommand(DisActiveParkedCar);
                }

                return disActiveParkedCarCommand;
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
        private async void AddParkedCar(object commandParameter)
        {
            try
            {
                // show add parked car dialog to add new car to parking
                AddParkedCarDialog dialog = new AddParkedCarDialog();
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {

                    try
                    {
                        // add the car to list if result was add
                        ParkedCarsList.Add(new ParkedCar()
                        {
                            ID = _parkedCarsDataHandler.GetLastestParkedCarID(),
                            Barcode = dialog.BarcodeTb.Text,
                            StartDate = DateTime.Now,
                            Customer = (Customer)dialog.CustomerCmb.SelectedItem,
                            CustomerHourPrice = ((Customer)dialog.CustomerCmb.SelectedItem).HourPrice,
                            CustomerHalfHourPrice = ((Customer)dialog.CustomerCmb.SelectedItem).HalfHourPrice,

                            Number = _parkedCarsDataHandler.GetCarNumber()
                        });
                        OnPropertyChanged("FilterParkedCarsList");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
                else
                {
                    if (App.FirstCarNumber == 1)
                    {
                        App.FirstCarNumber = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                
            }
           
        }
        private async void DisActiveParkedCar(object commandParameter)
        {
            try
            {
                ParkedCar selectedItem = null;
                // if this command was called with null command parameter then command parameter = first item of the filterd list
                if (commandParameter == null && FilterParkedCarsList.Count > 0)
                {
                    selectedItem = FilterParkedCarsList[0];
                }

                if (commandParameter != null)
                {

                    selectedItem = (ParkedCar)commandParameter;

                }
                // show Dis Active Car Dialog
                DisActiveParkedCarDialog dialog = new DisActiveParkedCarDialog();
                dialog.DataContext = new DisActiveCarViewModel(selectedItem);
                //dialog.DataContext = new DisActiveCarViewModel(selectedItem);
                var result = await dialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // remove parked car from ParkedCarList => ItemsRepeater
                    ParkedCarsList.Remove(selectedItem);
                    OnPropertyChanged("FilterParkedCarsList");
                    SearchText = string.Empty;
                }

                dialog.DataContext = null;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
               
            }
      
        }


        #endregion

    }
}
