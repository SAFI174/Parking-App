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
    public class UsersViewModel : INotifyPropertyChanged
    {
        public UsersViewModel()
        {
            usersDataHandler = new UsersDataHandler();
            // Fill Users datagrid from database
            usersDataHandler.GetUsers(UsersList);
        }

        #region Properites

        private UsersDataHandler usersDataHandler;

        private Users selectedUser;
        public Users SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value; OnPropertyChanged("SelectedUser"); }
        }
        private string searchText;
        public string SearchText
        {
            get { return searchText; }
            set { searchText = value; OnPropertyChanged("SearchText"); OnPropertyChanged("FilterUsersList"); }
        }
        #endregion

        #region Lists
        private ObservableCollection<Users> _users = new ObservableCollection<Users>();
        public ObservableCollection<Users> Users
        {
            get => _users;
            set { _users = value; }
        }



        private ObservableCollection<Users> _usersList = new ObservableCollection<Users>();

        public ObservableCollection<Users> UsersList
        {
            get
            {
                return _usersList;
            }
            set
            {
                _usersList = value;
                OnPropertyChanged("FilterUsersList");
            }
        }


        private ObservableCollection<Users> filterUsersList = new ObservableCollection<Users>();

        public ObservableCollection<Users> FilterUsersList
        {
            get
            {
                filterUsersList.Clear();
                if (searchText == null)
                {
                    searchText = string.Empty;
                }

                var querySplit = searchText.Split(' ');
                var matchingItems = UsersList.Where(
                    item =>
                    {
                        bool flag = true;
                        foreach (string queryToken in querySplit)
                        {
                            // Check if token is not in string 
                            if (item.FullName.IndexOf(queryToken, StringComparison.CurrentCultureIgnoreCase) < 0)
                            {
                                // Token is not in string, so we ignore this item. 
                                flag = false;
                            }
                        }
                        return flag;
                    });
                foreach (var item in matchingItems)
                {
                    filterUsersList.Add(item);
                }

                return filterUsersList;
            }
            set
            {
                filterUsersList = value;
            }
        }
        #endregion

        #region Commands

        private DelegateCommand addUserCommand;
        public ICommand AddUserCommand
        {
            get
            {
                if (addUserCommand == null)
                {
                    addUserCommand = new DelegateCommand(AddUser);
                }

                return addUserCommand;
            }
        }

        private DelegateCommand deleteUserCommand;
        public ICommand DeleteUserCommand
        {
            get
            {
                if (deleteUserCommand == null)
                {
                    deleteUserCommand = new DelegateCommand(DeleteUser);
                }

                return deleteUserCommand;
            }
        }

        private DelegateCommand editUserCommand;
        public ICommand EditUserCommand
        {
            get
            {
                if (editUserCommand == null)
                {
                    editUserCommand = new DelegateCommand(EditUser);
                }

                return editUserCommand;
            }
        }


        private async void DeleteUser(object commandParameter)
        {
            // Show confirmation dialog to delete selected user
            DefulatContentDialog dlg = new DefulatContentDialog();
            dlg.Title = "حذف";
            dlg.PrimaryButtonText = "حذف";
            dlg.MessageText.Text = "هل تريد حذف المستخدم المحدد؟";
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary && SelectedUser != null)
            {
                // remove user from database
                usersDataHandler.RemoveUser(SelectedUser.ID);
                // remove user from UserList
                UsersList.Remove(SelectedUser);
                OnPropertyChanged("FilterUsersList");
            }

        }

        private async void AddUser(object commandParameter)
        {
            UserAddEditDialog dlg = new UserAddEditDialog();
            var result = await dlg.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // Check if users is already exist
                if (usersDataHandler.CheckifUsersExist(dlg.UsernameTb.Text))
                {
                    DefulatContentDialog dialog = new DefulatContentDialog();
                    dialog.Title = "خطأ!";
                    dialog.MessageText.Text = " اسم المستخدم موجود مسبقاً!";
                    dialog.PrimaryButtonText = "موافق";
                    await dialog.ShowAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(dlg.PasswordTb.Text))
                {
                    dlg.PasswordTb.Text = "";
                }
                // Add user to database
                usersDataHandler.AddUser(dlg.FullNameTb.Text, dlg.UsernameTb.Text, dlg.PasswordTb.Text, (bool)dlg.isAdmin.IsChecked);
                // Add user to DataGrid
                UsersList.Add(new Users()
                {
                    ID = usersDataHandler.GetUsers(Users).Last().ID,
                    FullName = dlg.FullNameTb.Text,
                    Username = dlg.UsernameTb.Text,
                    Password = dlg.PasswordTb.Text,
                    isAdmin = (bool)dlg.isAdmin.IsChecked
                });
                // Notify to update filtered user list from UserList
                OnPropertyChanged("FilterUsersList");
            }
        }

        private async void EditUser(object commandParameter)
        {
            if (SelectedUser == null)
            {
                return;
            }
            // Show Dialog to edit user details
            UserAddEditDialog dlg = new UserAddEditDialog();
            dlg.Title = "تعديل بيانات مستخدم";
            dlg.PrimaryButtonText = "تعديل";
            // fill data from datagrid row
            dlg.FullNameTb.Text = SelectedUser.FullName;
            dlg.UsernameTb.Text = SelectedUser.Username;
            dlg.PasswordTb.Text = SelectedUser.Password;
            dlg.isAdmin.IsChecked = SelectedUser.isAdmin;
            var result = await dlg.ShowAsync();

            if (result == ContentDialogResult.Primary && SelectedUser != null)
            {
                // Check if users is already exist
                if (usersDataHandler.CheckifUsersExist(dlg.UsernameTb.Text) && SelectedUser.Username != dlg.UsernameTb.Text)
                {
                    DefulatContentDialog dialog = new DefulatContentDialog();
                    dialog.Title = "خطأ!";
                    dialog.MessageText.Text = " اسم المستخدم موجود مسبقاً!";
                    dialog.PrimaryButtonText = "موافق";
                    await dialog.ShowAsync();
                    return;
                }

                if (string.IsNullOrWhiteSpace(dlg.PasswordTb.Text))
                {
                    dlg.PasswordTb.Text = "";
                }
                // update data on UserList item
                var index = UsersList.IndexOf(SelectedUser);
                UsersList[index].FullName = dlg.FullNameTb.Text;
                UsersList[index].Username = dlg.UsernameTb.Text;
                UsersList[index].Password = dlg.PasswordTb.Text;
                UsersList[index].isAdmin = (bool)dlg.isAdmin.IsChecked;

                // update data on database
                usersDataHandler.EditUser(SelectedUser.ID, dlg.FullNameTb.Text, dlg.UsernameTb.Text, dlg.PasswordTb.Text, (bool)dlg.isAdmin.IsChecked);
                OnPropertyChanged("FilterUsersList");
            }

        }
        #endregion

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
    }
}
