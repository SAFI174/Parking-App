using System.ComponentModel;

namespace ParkingApp.Models
{
    public class Users : INotifyPropertyChanged
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
        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set { fullName = value; OnPropertyChanged("FullName"); }
        }
        private string userName;
        public string Username
        {
            get { return userName; }
            set { userName = value; OnPropertyChanged("Username"); }
        }
        private string password;
        public string Password
        {
            get { return password; }
            set { password = value; OnPropertyChanged("Password"); }
        }
        private bool isadmin;
        public bool isAdmin
        {
            get { return isadmin; }
            set { isadmin = value; OnPropertyChanged("isAdmin"); }
        }
    }
}
