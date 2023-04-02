using ModernWpf.Controls;
using ParkingApp.Classes;
using ParkingApp.Views.Pages;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ParkingApp.Views.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {

            InitializeComponent();
            // select first page to be displayed
            nvSample.SelectedItem = nvSample.MenuItems.OfType<NavigationViewItem>().First();
            // check if active user is admin
            CheckActiveUser();
            parkedCarsDataHandler = new ParkedCarsDataHandler();
            // Create timer to schedule reset car number at 7:30 AM
            resetCarNumberTimer = new DispatcherTimer();
            resetCarNumberTimer.Interval = GetIntervalToNextDay();
            resetCarNumberTimer.Tick += ResetCarNumberTimer_Tick;
            resetCarNumberTimer.IsEnabled = true;
        }

        #region Properties
        private DispatcherTimer resetCarNumberTimer;
        private ParkedCarsDataHandler parkedCarsDataHandler;


        #endregion

        #region Event And Func

        private void ResetCarNumberTimer_Tick(object sender, EventArgs e)
        {
            // reset car number 
            parkedCarsDataHandler.ResetCarNumber();
            // re calc time to next 7:30 AM
            resetCarNumberTimer.Interval = GetIntervalToNextDay();
        }
        // calc time to 7:30 AM 
        private TimeSpan GetIntervalToNextDay()
        {
            DateTime now = DateTime.Now;
            DateTime next730AM = now.Date.AddHours(7).AddMinutes(30);
            TimeSpan interval = next730AM - now;
            if (interval < TimeSpan.Zero)
            {
                interval = interval.Add(TimeSpan.FromDays(1));
            }
            return interval;
        }
        private void CheckActiveUser()
        {
            // check if active user is admin if not hide some pages
            if (!App.Users.isAdmin)
            {
                Reports.Visibility = Visibility.Collapsed;
                CustomerPage.Visibility = Visibility.Collapsed;
                UsersPage.Visibility = Visibility.Collapsed;
            }
        }
        private void nvSample_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            // if settings selected navigate to settings page
            if (args.IsSettingsSelected)
            {
                contentFrame.Navigate(typeof(Settings));
                nvSample.Header = "إعدادت";
            }
            else
            {
                // get selected item from args
                var selectedItem = (NavigationViewItem)args.SelectedItem;
                // get selected item tag from selecteditem
                string selectedItemTag = ((string)selectedItem.Tag);
                // navigate to page by its tag
                if (selectedItemTag == "Parking")
                {
                    contentFrame.Navigate(typeof(Parking));
                    nvSample.Header = "مواقف السيارات";
                }
                if (selectedItemTag == "Reports")
                {
                    contentFrame.Navigate(typeof(Reports));
                    nvSample.Header = "التقارير";
                }
                if (selectedItemTag == "Users")
                {
                    contentFrame.Navigate(typeof(Users));
                    nvSample.Header = "المستخدمين";
                }
                if (selectedItemTag == "Customers")
                {
                    contentFrame.Navigate(typeof(Customers));
                    nvSample.Header = "الزبائن";
                }

            }
        }

        #endregion
    }
}