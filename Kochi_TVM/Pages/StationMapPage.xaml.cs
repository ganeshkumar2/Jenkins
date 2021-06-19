using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.PID;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for StationMapPage.xaml
    /// </summary>
    public partial class StationMapPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(StationMapPage).Name);
        public StationMapPage()
        {
            InitializeComponent();
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(3, null, null, "IN");
            }
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void btnStationList_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.StationPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnStation_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            int selectedStationId = 0;
            SetDefaultStatus();
            selectedStationId = Convert.ToInt32(((Button)sender).Tag);
            if (Stations.stationList.ContainsKey(selectedStationId))
                SetStation(selectedStationId);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LedOperations.GreenText("Select Destination");
                Message();
                btnBack.Content = MultiLanguage.GetText("back");
                btnFinish.Content = MultiLanguage.GetText("cancel");
                btnStationList.Content = MultiLanguage.GetText("showStationList");
                SetHeaderText();
                SetDefaultStatus();
                ListStationsInGrid();

                btnStation1.Content = Stations.GetStation(1).description;
                btnStation2.Content = Stations.GetStation(2).description;
                btnStation3.Content = Stations.GetStation(3).description;
                btnStation4.Content = Stations.GetStation(4).description;
                btnStation5.Content = Stations.GetStation(5).description;
                btnStation6.Content = Stations.GetStation(6).description;
                btnStation7.Content = Stations.GetStation(7).description;
                btnStation8.Content = Stations.GetStation(8).description;
                btnStation9.Content = Stations.GetStation(9).description;
                btnStation10.Content = Stations.GetStation(10).description;
                btnStation11.Content = Stations.GetStation(11).description;
                btnStation12.Content = Stations.GetStation(12).description;
                btnStation13.Content = Stations.GetStation(13).description;
                btnStation14.Content = Stations.GetStation(14).description;
                btnStation15.Content = Stations.GetStation(15).description;
                btnStation16.Content = Stations.GetStation(16).description;
                btnStation17.Content = Stations.GetStation(17).description;
                btnStation18.Content = Stations.GetStation(18).description;
                btnStation19.Content = Stations.GetStation(19).description;
                btnStation20.Content = Stations.GetStation(20).description;
                btnStation21.Content = Stations.GetStation(21).description;
                btnStation22.Content = Stations.GetStation(22).description;
            }
            catch (Exception ex)
            {
                log.Error("Error StationMapPage -> Page_Loaded() : " + ex.ToString());
            }
        }
        private void SetHeaderText()
        {
            switch (Ticket.mapHeaderType)
            {
                case MapHeaderType.EndStation:
                    lblHeader.Content = MultiLanguage.GetText("selectDest");
                    break;
                case MapHeaderType.Station1:
                    lblHeader.Content = "Select First Station";
                    break;
                case MapHeaderType.Station2:
                    lblHeader.Content = "Select Second Station";
                    break;
                default:
                    break;
            }
        }
        private void SetDefaultStatus()
        {
            lblWarning.Visibility = Visibility.Hidden;
            lblWarning.Content = "";
        }
        private void SetStation(int selectedStationId)
        {
            switch (Ticket.mapHeaderType)
            {
                case MapHeaderType.EndStation:
                    SetEndStation(selectedStationId);
                    break;
                case MapHeaderType.Station1:
                    SetFirstStation(selectedStationId);
                    break;
                case MapHeaderType.Station2:
                    SetSecondStation(selectedStationId);
                    break;
                default:
                    break;
            }
        }
        private void SetSecondStation(int selectedStationId)
        {
            if (selectedStationId != Ticket.startStation.id)
            {
                Ticket.endStation = Stations.GetStation(selectedStationId);
                //PageControl.ShowPage(Pages.cardOperationPage);
            }
        }

        private void SetFirstStation(int selectedStationId)
        {
            Ticket.mapHeaderType = MapHeaderType.Station2;
            Ticket.startStation = Stations.GetStation(selectedStationId);
            SetHeaderText();
        }

        private void SetEndStation(int selectedStationId)
        {
            if (selectedStationId != Stations.currentStation.id)
            {
                Ticket.endStation = Stations.GetStation(selectedStationId);
                Ticket.startStation = Stations.currentStation;                
                NavigationService.Navigate(new Pages.TicketCountPage());
                //PageControl.ShowPage(Pages.journeyPage);
            }
            else
            {
                lblWarning.Visibility = Visibility.Visible;
                lblWarning.Content = "This station can not be selected!";
            }
        }
        private void ListStationsInGrid()
        {
            try
            {
                Grid.SetRow(imgHere, Stations.currentStation.mapHereRow);
                Grid.SetColumn(imgHere, Stations.currentStation.mapHereColumn);
                operationGrid.Children.Add(imgHere);
            }
            catch (Exception ex)
            {
                log.Error("Error StationMapPage -> ListStationsInGrid() : " + ex.ToString());
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
