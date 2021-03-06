using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.PID;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for JourneyTypePage.xaml
    /// </summary>
    public partial class JourneyTypePage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(JourneyTypePage).Name);

        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        public JourneyTypePage()
        {
            InitializeComponent();
            try
            {
                initialTimer();
                if (Ticket.language == Languages.English || Ticket.language == Languages.Hint)
                {
                    lblDestination.FontSize = 14;
                    lblNoOfTickets.FontSize = 14;
                    lblAmount.FontSize = 14;
                }
                else
                {
                    lblDestination.FontSize = 12;
                    lblNoOfTickets.FontSize = 12;
                    lblAmount.FontSize = 12;
                }
                btnBack.Content = MultiLanguage.GetText("back");
                btnFinish.Content = MultiLanguage.GetText("cancel");
                labelSJT.Content = MultiLanguage.GetText("sj");
                lblRJT.Content = MultiLanguage.GetText("rj");
                lblGroup.Content = MultiLanguage.GetText("gj");
                lblOneDayPass.Content = MultiLanguage.GetText("onedaypass");
                lblWeekendPass.Content = MultiLanguage.GetText("weekenddaypass");
                lblHeader.Content = MultiLanguage.GetText("selectTicketType");
                lblType.Content = MultiLanguage.GetText("DispTicketType");
                lblDestination.Content = MultiLanguage.GetText("DispDestination");
                lblNoOfTickets.Content = MultiLanguage.GetText("DispNoOfTickets");
                lblAmount.Content = MultiLanguage.GetText("DispAmount");
                //lblGroup.Content
                Message();
                LedOperations.GreenText("SELECT TICKET TYPE");
            }
            catch (Exception ex)
            {
                log.Error("Error JourneyTypePage -> JourneyTypePage() : " + ex.ToString());
            }
        }
        private void initialTimer()
        {
            try
            {
                idleTimerDelegate = new TimerCallback(NavigateAction);
                idleTimer = new Timer(idleTimerDelegate, null, 0, 1000);
            }
            catch (Exception ex)
            {
                log.Error("Error TicketTypePage -> initialTimer() : " + ex.ToString());
            }
        }
        private void NavigateAction(object obj)
        {
            try
            {
                var idleTime = IdleTimeDetector.GetIdleTimeInfo();

                if (idleTime.IdleTime.TotalMinutes >= Constants.SystemIdleTimeout)
                {
                    idleTimer.Dispose();
                    this.Dispatcher.Invoke(() =>
                    {
                        NavigationService.Navigate(new Pages.MainPage());
                    });
                }
            }
            catch (Exception ex)
            {
                log.Error("Error TicketTypePage -> DateTimeTimerAction() : " + ex.ToString());
            }
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(2, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(2, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(2, null, null, "IN");
            }
        }
        private void gridSJT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Ticket.journeyType = JourneyType.SJT;
            ElectronicJournal.ItemSelected("SJT");
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void gridRJT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Ticket.journeyType = JourneyType.RJT;
            ElectronicJournal.ItemSelected("RJT");
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }
        private void GoToNextStep()
        {
            Ticket.ManageDTS();
            Ticket.ManageTicketType();
            switch (Ticket.journeyType)
            {
                case JourneyType.Group_Ticket:
                case JourneyType.SJT:
                case JourneyType.RJT:
                    Ticket.mapHeaderType = MapHeaderType.EndStation;
                    //PageControl.ShowPage(Pages.stationListPage);
                    break;
                default:
                    break;
            }
        }

        private void gridGrp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TVMUtility.PlayClick();
            Ticket.journeyType = JourneyType.Group_Ticket;
            ElectronicJournal.ItemSelected("GROUP");
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void gridOnePass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ticket.journeyType = JourneyType.Day_Pass;
            GoToNextStep();
            NavigationService.Navigate(new Pages.TicketCountPage());
        }

        private void gridWeekendPass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Ticket.journeyType = JourneyType.Weekend_Pass;
            GoToNextStep();
            NavigationService.Navigate(new Pages.TicketCountPage());
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();
        }
    }
}
