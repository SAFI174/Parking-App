using ParkingApp.Models;
using ParkingApp.ViewModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ParkingApp.Views.Dialogs
{
    public partial class DisActiveParkedCarDialog
    {
        public DisActiveParkedCarDialog()
        {
            CultureInfo ci = CultureInfo.CreateSpecificCulture("en-us");
            ci.NumberFormat.DigitSubstitution = DigitShapes.NativeNational;
            Thread.CurrentThread.CurrentCulture = ci;
            InitializeComponent();
            
        }

        private static readonly Regex _regex = new Regex("^[0-9]*$"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void DiscountTb_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private void DiscountTb_OnGotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.SelectAll();
        }
    }
}
