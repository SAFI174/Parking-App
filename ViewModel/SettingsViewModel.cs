using ModernWpf;
using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Commands;
using ParkingApp.Views.Dialogs;
using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class SettingsViewModel : INotifyPropertyChanged
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

        public SettingsViewModel()
        {
            // set theme combobox selected items from theme.txt
            try { SelectedTheme = File.ReadAllText(App.themetxtlocation); }
            catch (Exception) { }

            ActiveUsername = App.Users.FullName;

            parkedCarsDataHandler = new ParkedCarsDataHandler();
            databaseHelper = new DatabaseHelper();
            // set price value from database
            // set current car number from database
            CarNumber = parkedCarsDataHandler.GetCarNumber();
        }

        #region Properties

        private DatabaseHelper databaseHelper;
        private ParkedCarsDataHandler parkedCarsDataHandler;
        

        private string _selectedTheme;
        public string SelectedTheme
        {
            get
            {
                return _selectedTheme;
            }
            set
            {
                _selectedTheme = value;
                OnPropertyChanged("SelectedTheme");
                ChangeTheme();
            }
        }
        private string _activeUsername;
        public string ActiveUsername
        {
            get
            {
                return _activeUsername;
            }
            set
            {
                _activeUsername = value;
                OnPropertyChanged("ActiveUsername");
            }
        }
        private int _carNumber;
        public int CarNumber
        {
            get
            {
                return _carNumber;
            }
            set
            {
                _carNumber = value;
                OnPropertyChanged("CarNumber");
            }
        }
        #endregion

        #region Commands And Func

        // change theme and write new theme to theme.txt
        private void ChangeTheme()
        {
            StreamWriter Swr = new StreamWriter(App.themetxtlocation);
            if (SelectedTheme == "داكن")
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                Swr.Write("داكن");
                Swr.Close();

            }
            else
            {
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                Swr.Write("فاتح");
                Swr.Close();
            }
            Swr.Dispose();
        }



        private DelegateCommand makeBackupCommand;
        public ICommand MakeBackupCommand
        {
            get
            {
                if (makeBackupCommand == null)
                {
                    makeBackupCommand = new DelegateCommand(MakeBackup);
                }

                return makeBackupCommand;
            }
        }

        private DelegateCommand restoreBackupCommand;
        public ICommand RestoreBackupCommand
        {
            get
            {
                if (restoreBackupCommand == null)
                {
                    restoreBackupCommand = new DelegateCommand(RestoreBackup);
                }

                return restoreBackupCommand;
            }
        }

        private DelegateCommand resetCarNumberCommand;
        public ICommand ResetCarNumberCommand
        {
            get
            {
                if (resetCarNumberCommand == null)
                {
                    resetCarNumberCommand = new DelegateCommand(ResetCarNumber);
                }

                return resetCarNumberCommand;
            }
        }

        private async void MakeBackup(object commandParameter)
        {
            // show save file dialog to the user to save backup of database
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Database Backup Files (*.bak)|*.bak|All files (*.*)|*.*";
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.FileName = DateTime.Now.ToString("MM-dd-yyyy (hh_mm)", new CultureInfo("en-us"));
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // the below query get backup of database you specified in combobox
                databaseHelper.ExecuteQuery("Backup database ParkingApp to disk='" + saveFileDialog.FileName + "'");
                // show complete message to the user
                DefulatContentDialog dialog = new DefulatContentDialog();
                dialog.Title = "قاعدة البيانات";
                dialog.MessageText.Text = "تمت حفظ نسخة إحتياطية بنجاح!";
                await dialog.ShowAsync();
            }
        }
        private async void RestoreBackup(object commandParameter)
        {
            try
            {
                // Show dialog to the user to select exist backup
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = true;
                openFileDialog.Title = "Select Database Backup File";
                openFileDialog.Filter = "Database Backup Files (*.bak)|*.bak|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {

                    // Execute Query to restore database from .bak file 
                    databaseHelper.ExecuteQuery("use master " +
                                                $"RESTORE DATABASE ParkingApp FROM DISK ='{openFileDialog.FileName}' With Replace");

                    // show complete message to the user
                    DefulatContentDialog dialog = new DefulatContentDialog();
                    dialog.Title = "قاعدة البيانات";
                    dialog.MessageText.Text = "تمت استرجاع النسخة الإحتياطية بنجاح!";
                    await dialog.ShowAsync();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("يرجى المحاولة لاحقاً");
            }
           

        }
        private async void ResetCarNumber(object commandParameter)
        {
            // show dialog to confirm reset car number to 1 and set style
            DefulatContentDialog dialog = new DefulatContentDialog();
            dialog.Title = "إعادة تعيين";
            dialog.MessageText.Text = "هل تريد إعادة تعيين رقم السيارات الى (1)";
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // reset car number
                parkedCarsDataHandler.ResetCarNumber();
                CarNumber = parkedCarsDataHandler.GetCarNumber();
                App.FirstCarNumber = 0;
            }

        }
        #endregion
    }
}
