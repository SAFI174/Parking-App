using System.Windows;

namespace ParkingApp.Views.Windows
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            UserNameTb.Focus();
        }

        private void UserNameTb_GotFocues(object sender, RoutedEventArgs e)
        {
            UserNameTb.SelectAll();
        }
        private void PasswordTb_OnGotFocus(object sender, RoutedEventArgs e)
        {
            PasswordTb.SelectAll();
        }
    }
}
