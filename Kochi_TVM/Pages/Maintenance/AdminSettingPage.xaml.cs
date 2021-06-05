﻿using Kochi_TVM.Utils;
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
    /// Interaction logic for AdminSettingPage.xaml
    /// </summary>
    public partial class AdminSettingPage : Page
    {
        public AdminSettingPage()
        {
            InitializeComponent();
        }

        private void btnBNRSetting_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.BNRSettingPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminSettingPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnCoinSetting_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.CoinSettingPage());
        }
    }
}