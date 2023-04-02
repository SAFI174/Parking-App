using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using ParkingApp.Views.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class CustomersViewModel : INotifyPropertyChanged
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

        public CustomersViewModel()
        {
            _customerDataHandler = new CustomerDataHandler();
            // fill Customers List => DataGrid
            _customerDataHandler.GetCustomers(customers:CustomersList);
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

        // Filterd Customer List help to filter by search text
        private ObservableCollection<Customer> filterCustomersList = new ObservableCollection<Customer>();

        public ObservableCollection<Customer> FilterCustomersList
        {
            get
            {
                filterCustomersList.Clear();
                if (searchText == null)
                {
                    searchText = string.Empty;
                }

                var querySplit = searchText.Split(' ');
                var matchingItems = CustomersList.Where(
                    item =>
                    {
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.Name.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    filterCustomersList.Add(item);
                }

                return filterCustomersList;
            }
            set
            {
                filterCustomersList = value;
            }
        }

        #endregion

        #region Properties

        private CustomerDataHandler _customerDataHandler;

        private Customer _selectedCustomer;
        public Customer SelectedCustomer
        {
            get
            {
                return _selectedCustomer;
            }
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged("SelectedCustomer");
            }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged("SearchText"); OnPropertyChanged("FilterCustomersList"); }
        }

        #endregion

        #region Commands

        private DelegateCommand editCustomerCommand;
        public ICommand EditCustomerCommand
        {
            get
            {
                if (editCustomerCommand == null)
                {
                    editCustomerCommand = new DelegateCommand(EditCustomer);
                }

                return editCustomerCommand;
            }
        }

        private DelegateCommand deleteCustomerCommand;
        public ICommand DeleteCustomerCommand
        {
            get
            {
                if (deleteCustomerCommand == null)
                {
                    deleteCustomerCommand = new DelegateCommand(DeleteCustomer);
                }

                return deleteCustomerCommand;
            }
        }

        private DelegateCommand addCustomerCommand;
        public ICommand AddCustomerCommand
        {
            get
            {
                if (addCustomerCommand == null)
                {
                    addCustomerCommand = new DelegateCommand(AddCustomer);
                }

                return addCustomerCommand;
            }
        }

        private async void EditCustomer(object commandParameter)
        {
            // show dialog to edit customer data
            CustomerAddEditDialog dialog = new CustomerAddEditDialog();
            // set dialog style and fill data to "edit"
            dialog.Title = "تعديل بيانات زبون";
            dialog.NameTb.Text = SelectedCustomer.Name;
            dialog.HourPriceTb.Value = SelectedCustomer.HourPrice;
            dialog.HalfHourPriceTb.Value = SelectedCustomer.HalfHourPrice;
            dialog.PrimaryButtonText = "تعديل";
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // edit customer data on database
                _customerDataHandler.EditCustomer(SelectedCustomer.Id, dialog.NameTb.Text, dialog.HourPriceTb.Value, dialog.HalfHourPriceTb.Value);
                // edit customer data on datagrid
                var index = CustomersList.IndexOf(SelectedCustomer);
                CustomersList[index].Name = dialog.NameTb.Text;
                CustomersList[index].HourPrice = dialog.HourPriceTb.Value;
                CustomersList[index].HalfHourPrice = dialog.HalfHourPriceTb.Value;
                // update UI
                OnPropertyChanged("FilterCustomersList");
            }
        }
        private async void AddCustomer(object commandParameter)
        {
            // show dialog to add customer data
            CustomerAddEditDialog dialog = new CustomerAddEditDialog();
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // add user data to database
                _customerDataHandler.AddCustomer(dialog.NameTb.Text, dialog.HourPriceTb.Value, dialog.HalfHourPriceTb.Value);
                // add user data to customer list => datagrid
                CustomersList.Add(new Customer()
                {
                    Id = _customerDataHandler.GetLastId(),
                    Name = dialog.NameTb.Text,
                    HourPrice = dialog.HourPriceTb.Value,
                    HalfHourPrice = dialog.HalfHourPriceTb.Value
                });
                // Update UI
                OnPropertyChanged("FilterCustomersList");
            }
        }
        private async void DeleteCustomer(object commandParameter)
        {
            // show dialog to confirm delete customer
            DefulatContentDialog dlg = new DefulatContentDialog();
            // set delete style on the dialog
            dlg.Title = "حذف";
            dlg.PrimaryButtonText = "حذف";
            dlg.MessageText.Text = "هل تريد حذف الزبون المحدد سيتم حذف كافة التسجيلات المعتمدة عليه!؟";
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary && SelectedCustomer != null)
            {
                // remove customer from database
                _customerDataHandler.DeleteCustomer(SelectedCustomer.Id);
                // remove customer from UserList
                CustomersList.Remove(SelectedCustomer);
                OnPropertyChanged("FilterCustomersList");
            }
        }

        #endregion

    }
}
