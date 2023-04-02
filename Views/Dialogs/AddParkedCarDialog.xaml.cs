using System.Globalization;
using System.Threading;

namespace ParkingApp.Views.Dialogs
{
    public partial class AddParkedCarDialog
    {
        public AddParkedCarDialog()
        {
            InitializeComponent();
            CustomerCmb.SelectedIndex = 0;
        }
    }
}
