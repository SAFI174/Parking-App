using ParkingApp.Views.Windows;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ParkingApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings
    {
        public Settings()
        {
            InitializeComponent();
        }
        private static readonly Regex _regex = new Regex("^[0-9]*$"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void priceTb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);

        }

        private void LogoutBTN_OnClick(object sender, RoutedEventArgs e)
        {
            var myWindow = Window.GetWindow(this);
            Login login = new Login();
            login.Show();
            myWindow.Close();
        }
    }
}
