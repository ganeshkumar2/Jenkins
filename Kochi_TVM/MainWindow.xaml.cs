using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Configuration;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Kochi_TVM
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ILog log = LogManager.GetLogger(typeof(MainWindow).Name);

        private static Timer dateTimeTimer;
        private static TimerCallback dateTimeTimerDelegate;
        public MainWindow()
        {
            InitializeComponent();           
            try
            {
                ConfigLog4net();
                log.Debug("***TVM Application Started***");
                log.Debug("TVM App Version: " + Constants.appVersion);
                MultiLanguage.Init("EN");
                InitialTimer();
                Stations.FillStationList();
                Stations.FillCurrentStation();
                //Parameters.TVMDynamic.FillOrUpdateParameters();
                DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));
                lStation.Content = "You are at : [" + Stations.currentStation.name + "] Station";

                if (ConfigurationManager.AppSettings["VoiceEnable"].ToString() == "True")
                    Constants.IsVoiceEnabled = true;
                else
                    Constants.IsVoiceEnabled = false;

                //frameHomeMain.Navigate(new Pages.MainPage());
                frameHomeMain.Navigate(new Pages.OutOfSevicePage());
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void ConfigLog4net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        private void InitialTimer()
        {
            try
            {
                dateTimeTimerDelegate = new TimerCallback(DateTimeTimerAction);
                dateTimeTimer = new Timer(dateTimeTimerDelegate, null, 0, 1000);
            }
            catch (Exception ex)
            {
                log.Error("Error  OptionPage -> InitialTimer() : " + ex.ToString());
            }
        }
        private void DateTimeTimerAction(object obj)
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    lDate.Content = String.Format("{0}", DateTime.Now.ToString("dddd, dd MMMM yyyy - HH:mm:ss", new CultureInfo("en-GB")));
                }));
            }
            catch (Exception ex)
            {
                log.Error("Error MainWindow -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
        int i = 0;
        private void gridLogo_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (i == 2)
                return;
            TVMUtility.PlayClick();
            i++;            
        }

        private void gridDT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            i++;
            if (i == 3)
            {
                i = 0;
                frameHomeMain.Navigate(new Pages.Maintenance.AdminLoginPage());
            }
        }
    }
}
