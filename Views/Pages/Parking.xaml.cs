using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ParkingApp.Views.Pages
{
    /// <summary>
    /// Interaction logic for Parking.xaml
    /// </summary>
    public partial class Parking
    {
        public Parking()
        {
            InitializeComponent();
            SearchTb.Focus();
        }

        private void Parking_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-zA-Z]+");
            if (!regex.IsMatch(e.Text))
            {
                SearchTb.Focus();
            }
        }
    }
}
