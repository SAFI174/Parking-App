using ParkingApp.Classes;
using ParkingApp.Commands;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace ParkingApp.ViewModel
{
    public class DataBaseSettingsDialogViewModel : INotifyPropertyChanged
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

        public DataBaseSettingsDialogViewModel()
        {
            connectionString = ReadConnectionStringFromFile();
        }

        #region Properites

        private DatabaseHelper databaseHelper;

        private string connectionString = "";
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; OnPropertyChanged("ConnectionString"); }
        }
        #endregion


        #region Commands
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



        private string ReadConnectionStringFromFile()
        {
            // check if file exist
            if (!File.Exists(DatabaseHelper.databaseFilePath))
            {
                StreamWriter writer = new StreamWriter(DatabaseHelper.databaseFilePath);
                writer.Close();
            }
            // otherwise read the first line of it
            StreamReader reader = new StreamReader(DatabaseHelper.databaseFilePath);
            string secureText = reader.ReadLine();
            reader.Close();
            // return decrypt connection string
            return secureText;
        }

        private async void RestoreBackup(object commandParameter)
        {
            // Show dialog to the user to select exist backup
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Title = "Select Database Backup File";
            openFileDialog.Filter = "Database Backup Files (*.bak)|*.bak|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        databaseHelper = new DatabaseHelper();
                        // Execute Query to restore database from .bak file 
                        databaseHelper.ExecuteQuery("use master " +
                                                    $"RESTORE DATABASE ParkingApp FROM DISK ='{openFileDialog.FileName}' With Replace");
                        // show complete message to the user
                        MessageBox.Show("تمت استرجاع النسخة الإحتياطية بنجاح!");
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                });

            }
        }


        #endregion

    }
}
