namespace ParkingApp.Views.Dialogs
{
    public partial class DataBaseSettingsDialog
    {
        public DataBaseSettingsDialog()
        {
            InitializeComponent();
            ConnectionStringTb.SelectAll();

            ConnectionStringTb.Focus();
        }
    }
}
