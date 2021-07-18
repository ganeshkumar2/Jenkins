﻿using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.PID;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for StationPage.xaml
    /// </summary>
    public partial class StationPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(StationPage).Name);
        private static Timer idleTimer;
        private static TimerCallback idleTimerDelegate;
        Grid GridStations = null;
        public StationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                initialTimer();
                LedOperations.GreenText("Select Destination");
                Message();
                btnBack.Content = MultiLanguage.GetText("back");
                btnFinish.Content = MultiLanguage.GetText("cancel");
                btnStationMap.Content = MultiLanguage.GetText("showStationMap");
                Dictionary<int, Station> stations = Stations.stationList;
                SetHeaderText();
                bool isOk = CreateGridStations();
                if (isOk)
                    ListStationsInGrid();
            }
            catch (Exception ex)
            {
                log.Error("Error StationPage -> Page_Loaded() : " + ex.ToString());
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

        private bool CreateGridStations()
        {
            bool result = false;
            try
            {
                GridStations = new Grid();
                svToRoots.Content = GridStations;
                var toCount = Stations.stationList.Count;
                var valueOfRow = (double)toCount / 5;
                var rowCount = Math.Ceiling(valueOfRow);
                GridLengthConverter gridLengthConverter = new GridLengthConverter();
                ColumnDefinition gridCol = null;
                RowDefinition gridRow = null;
                for (var i = 0; i < 4; i++)
                {
                    gridCol = new ColumnDefinition();
                    gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("5*");
                    GridStations.ColumnDefinitions.Add(gridCol);
                    if (i != 3)
                    {
                        gridCol = new ColumnDefinition();
                        gridCol.Width = (GridLength)gridLengthConverter.ConvertFrom("*");
                        GridStations.ColumnDefinitions.Add(gridCol);
                    }
                }
                for (var i = 0; i <= rowCount; i++)
                {
                    gridRow = new RowDefinition();
                    gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("5*");
                    GridStations.RowDefinitions.Add(gridRow);
                    if (i != rowCount)
                    {
                        gridRow = new RowDefinition();
                        gridRow.Height = (GridLength)gridLengthConverter.ConvertFrom("*");
                        GridStations.RowDefinitions.Add(gridRow);
                    }
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                log.Error("Error StationPage -> CreateGridStations() : " + ex.ToString());
            }
            return result;
        }

        private void ListStationsInGrid()
        {
            var a = 0;
            var b = 0;
            try
            {
                var style = Application.Current.FindResource("styleSelectionBtn") as Style;
                for (var i = 1; i <= Stations.stationList.Count; i++)
                {

                    if (Stations.stationList[i].id == Convert.ToInt32(Parameters.TVMDynamic.GetParameter("stationId")) /*|| Stations.stationList[i].id == 22*/) continue;
                    var buttonTo = new Button
                    {
                        Content = MultiLanguage.GetText(Stations.stationList[i].name),
                        Name = "btnStation" + i,
                        Tag = Stations.stationList[i].id,
                        Style = style,
                        FontSize = Ticket.language == Languages.English ? 20 : 13,
                        VerticalAlignment = VerticalAlignment.Stretch,
                        HorizontalAlignment = HorizontalAlignment.Stretch
                    };
                    buttonTo.Click += btnStation_Click;
                    Grid.SetRow(buttonTo, b);
                    Grid.SetColumn(buttonTo, a);
                    GridStations.Children.Add(buttonTo);
                    a = a + 2;
                    if (a != 8) continue;
                    b = b + 2;
                    a = 0;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error StationPage -> ListStationsInGrid() : " + ex.ToString());
            }
        }

        private void btnStation_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            int selectedStationId = 0;
            selectedStationId = Convert.ToInt32(((Button)sender).Tag.ToString());//Stations.GetStation().id;
            //if (Stations.stationList.ContainsKey(selectedStationId))
            //    SetStation(selectedStationId);
            Ticket.endStation = Stations.GetStation(selectedStationId);
            Ticket.startStation = Stations.currentStation;
            ElectronicJournal.DestinationSelected(Ticket.endStation.name.ToString());
            NavigationService.Navigate(new Pages.TicketCountPage());
            //PageControl.ShowPage(Pages.journeyPage);
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
            Ticket.endStation = Stations.GetStation(selectedStationId);
            //PageControl.ShowPage(Pages.cardOperationPage);
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
                string tmp = MultiLanguage.GetCurrentLanguage();
                MultiLanguage.ChangeLanguage("EN");
                Stations.FillStationList();
                Ticket.endStation = Stations.GetStation(selectedStationId);
                Ticket.startStation = Stations.currentStation;
                MultiLanguage.ChangeLanguage(tmp);
                Stations.FillStationList();
                //PageControl.ShowPage(Pages.journeyPage);
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (idleTimer != null)
                idleTimer.Dispose();
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnStationMap_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.StationMapPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.JourneyTypePage());
        }
    }
}
