﻿using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
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

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for TVMInfoPage.xaml
    /// </summary>
    public partial class TVMInfoPage : Page
    {
        public TVMInfoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            UpdDevStat();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Cancel";
        }
        private void UpdDevStat()
        {
            //add real value

            //TVMId
            DeviceInfoControl TVMId = new DeviceInfoControl("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"));
            Grid.SetRow(TVMId, 0);
            Grid.SetColumn(TVMId, 0);
            operationGrid.Children.Add(TVMId);


            //EquipmentId
            DeviceInfoControl EquipmentId = new DeviceInfoControl("Equipment ID", Parameters.TVMDynamic.GetParameter("descCode"));
            Grid.SetRow(EquipmentId, 2);
            Grid.SetColumn(EquipmentId, 0);
            operationGrid.Children.Add(EquipmentId);

            ////ParamVersion
            //DeviceInfoControl ParamVersion = new DeviceInfoControl("Parameter Version", Parameters.TVMDynamic.GetParameter("sys_CCVersion"));
            //Grid.SetRow(ParamVersion, 4);
            //Grid.SetColumn(ParamVersion, 0);
            //operationGrid.Children.Add(ParamVersion);

            //AppVersion
            DeviceInfoControl AppVersion = new DeviceInfoControl("App Version", Parameters.TVMStatic.GetParameter("appVersion"));
            Grid.SetRow(AppVersion, 4);
            Grid.SetColumn(AppVersion, 0);
            operationGrid.Children.Add(AppVersion);

            //LastSyncDts
            DeviceInfoControl LastSyncDts = new DeviceInfoControl("Last Sync Date", Parameters.lastSync.ToString("dd/MM/yyyy HH:mm"));
            Grid.SetRow(LastSyncDts, 6);
            Grid.SetColumn(LastSyncDts, 0);
            operationGrid.Children.Add(LastSyncDts);

            //CentralComputer
            DeviceInfoControl CentralComputer = new DeviceInfoControl("Central Computer", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(CentralComputer, 8);
            Grid.SetColumn(CentralComputer, 0);
            operationGrid.Children.Add(CentralComputer);

            //OCC
            DeviceInfoControl OCC = new DeviceInfoControl("OCC", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(OCC, 0);
            Grid.SetColumn(OCC, 2);
            operationGrid.Children.Add(OCC);

            //StationComputer
            var scCon = Parameters.TVMStatic.GetParameter("ScConn");
            DeviceInfoControl StationComputer = new DeviceInfoControl("Station Computer", scCon == "1" ? "Connect" : "Disconnect");
            Grid.SetRow(StationComputer, 2);
            Grid.SetColumn(StationComputer, 2);
            operationGrid.Children.Add(StationComputer);
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
    }
}
