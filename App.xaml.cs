using ModernWpf;
using ParkingApp.Models;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using FastReport.AdvMatrix;

namespace ParkingApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            #region check if app already running

            string procName = Process.GetCurrentProcess().ProcessName;

            // get the list of all processes by the "procName"       
            Process[] processes = Process.GetProcessesByName(procName);

            if (processes.Length > 1)
            {
                MessageBox.Show(procName + " already running");
                this.Shutdown();

                return;
            }

            #endregion

            // Change Language to Arabic 
            var culture = new CultureInfo("ar-SA");
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
            //set app theme
            SetAppTheme();
        }
        // global var for theme.txt location
        public static string themetxtlocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\ParkingApp\\theme.txt";
        // global var for current active user
        public static Users Users = new Users();
        public static int FirstCarNumber;
        // Set App Theme from theme.txt
        private void SetAppTheme()
        {
            // set custom accent color
            ThemeManager.Current.AccentColor = Color.FromRgb(0, 99, 177);
            // get theme data from theme.txt
            if (File.Exists(themetxtlocation))
            {
                if (File.ReadAllLines(themetxtlocation)[0] == "داكن")
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                }
                if (File.ReadAllLines(themetxtlocation)[0] == "فاتح")
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                }
            }
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                          "\\ParkingApp");
                StreamWriter Swr = new StreamWriter(themetxtlocation);
                Swr.Write("فاتح");
                Swr.Close();
                Swr.Dispose();
            }

        }
        // load embedded reports files 
        public static Stream LoadEmbeddedReport(string reportName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(reportName);
            return stream;
        }
    }
}
