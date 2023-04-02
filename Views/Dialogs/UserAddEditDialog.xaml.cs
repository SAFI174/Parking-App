using ParkingApp.Classes;
using System.Windows.Controls;

namespace ParkingApp.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for UserAddEditDialog.xaml
    /// </summary>
    public partial class UserAddEditDialog
    {
        public UserAddEditDialog()
        {
            InitializeComponent();
            databaseHelper = new DatabaseHelper();
        }

        private DatabaseHelper databaseHelper;
        private void UsernameTb_OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
