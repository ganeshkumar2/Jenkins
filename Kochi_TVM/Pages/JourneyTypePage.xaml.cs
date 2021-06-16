using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Utils;
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
        public JourneyTypePage()
        {
            InitializeComponent();
            Message();
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(2, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(2, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(2, null, null, "IN");
            }
        }
        private void gridSJT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utility.PlayClick();
            Ticket.journeyType = JourneyType.SJT;
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void gridRJT_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Utility.PlayClick();
            Ticket.journeyType = JourneyType.RJT;
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
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
            Utility.PlayClick();
            Ticket.journeyType = JourneyType.Group_Ticket;
            GoToNextStep();
            NavigationService.Navigate(new Pages.StationPage());
        }
    }
}
