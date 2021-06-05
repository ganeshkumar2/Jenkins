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
    /// Interaction logic for AdminMainPage.xaml
    /// </summary>
    public partial class AdminMainPage : Page
    {
        public AdminMainPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (Parameters.menuItems.Contains(Parameters.MenuStrings.QrRep) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.RptRep) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashEscCheck) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashDump) ||
             Parameters.menuItems.Contains(Parameters.MenuStrings.CashBoxRem)
             )
            {
                btnCollection.Visibility = Visibility.Visible;
            }
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Log Out";
            btnBack.Visibility = Visibility.Hidden;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminLoginPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Log Out", "Do you want to Log Out?", MessageBoxButtonSet.OKCancel);

            if (messageBoxResult == Custom.MessageBoxResult.OK)
            {
                NavigationService.Navigate(new Pages.Maintenance.AdminLoginPage());
            }
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnCollection_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.OperationPage());
        }

        private void btnMaintance_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.MaintancePage());
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminSettingPage());
        }
    }
}