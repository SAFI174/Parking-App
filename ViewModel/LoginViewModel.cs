using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Models;
using ParkingApp.Views.Dialogs;
using ParkingApp.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
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


        #region Properties

        private bool isDataBaseConnected;
        private UsersDataHandler _usersDataHandler;

        private string _username;
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
                OnPropertyChanged("Username");
            }
        }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
                OnPropertyChanged("Password");
            }
        }
        private string _message;
        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                OnPropertyChanged("Message");
            }
        }
        #endregion


        #region Commands
        private DelegateCommand loginCommand;

        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new DelegateCommand(Login);
                }

                return loginCommand;
            }
        }
        private DelegateCommand openSettingsDialogCommand;

        public ICommand OpenSettingsDialogCommand
        {
            get
            {
                if (openSettingsDialogCommand == null)
                {
                    openSettingsDialogCommand = new DelegateCommand(OpenSettingsDialog);
                }

                return openSettingsDialogCommand;
            }
        }
        private void Login(object commandParameter)
        {
            // multi command parameter
            var parameters = (List<Object>)commandParameter;
            // get password plain text from commandparameter
            Password = ((PasswordBox)parameters[1]).Password;
            // check if database is connected
            try
            {
                _usersDataHandler = new UsersDataHandler();
                isDataBaseConnected = true;

            }
            catch (Exception)
            {
                MessageBox.Show("قاعدة البيانات غير متصلة");
                isDataBaseConnected = false;
            }
            if (isDataBaseConnected)
            {
                // check if username and password is valid
                Users user = new Users();
                if (_usersDataHandler.CheckLoginData(user, Username, Password))
                {
                    // set active user data to global variables
                    App.Users.ID = user.ID;
                    App.Users.Username = user.Username;
                    App.Users.FullName = user.FullName;
                    App.Users.isAdmin = user.isAdmin;
                    // open MainWindow
                    MainWindow mainWindow = new MainWindow();
                    Login loginWindow = null;
                    if (commandParameter != null)
                    {
                        // hide login windows until MainWindows open
                        loginWindow = (Login)parameters[0];
                        loginWindow.Hide();
                    }
                    // show MainWindows
                    mainWindow.Show();
                    // Close Login Windows
                    loginWindow.Close();
                }
                else
                {
                    // set message text to error no user found on database
                    Message = "اسم المستخدم او كلمة السر خاطئة!";
                }
            }

        }


        // open database settings dialog
        private async void OpenSettingsDialog(object commandParameter)
        {
            // create the dialog and set defualt data on it
            DataBaseSettingsDialog dialog = new DataBaseSettingsDialog();
            string connectionString = dialog.ConnectionStringTb.Text;
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // check if Connection String if changed
                if (connectionString == dialog.ConnectionStringTb.Text)
                {
                    // if not changed return and don't write new value to database.txt
                    return;
                }
                // else
                // write encrypted connection string to database.txt
                StreamWriter writer = new StreamWriter(DatabaseHelper.databaseFilePath);
                string secureText = Encryption.EncryptString(dialog.ConnectionStringTb.Text, "80X9q!Sq");
                writer.Write(secureText);
                writer.Close();
            }
        }

        #endregion

    }
}
