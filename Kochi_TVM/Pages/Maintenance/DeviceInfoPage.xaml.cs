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
    /// Interaction logic for DeviceInfoPage.xaml
    /// </summary>
    public partial class DeviceInfoPage : Page
    {
        public DeviceInfoPage()
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
            //receipt printer
            var printerReceipt = string.Empty;

           

            DeviceInfoControl receiptPrinter = new DeviceInfoControl("Receipt Printer", printerReceipt);
            Grid.SetRow(receiptPrinter, 0);
            Grid.SetColumn(receiptPrinter, 0);
            operationGrid.Children.Add(receiptPrinter);
                     
            //BNA
            try
            {
                DeviceInfoControl BNA = new DeviceInfoControl("Banknote Acceptor", Constants.BNRStatus);
                Grid.SetRow(BNA, 4);
                Grid.SetColumn(BNA, 0);
                operationGrid.Children.Add(BNA);
            }
            catch (Exception ex)
            {
            }

            //Dispenser
            try
            {
                DeviceInfoControl Dispenser = new DeviceInfoControl("RPT Dispenser", Parameters.TVMStatic.GetParameter("rptDispenserStatus") == "1" ? "OK" : "ERROR");
                Grid.SetRow(Dispenser, 6);
                Grid.SetColumn(Dispenser, 0);
                operationGrid.Children.Add(Dispenser);
            }
            catch (Exception ex)
            {
            }

            //EMV POS
            try
            {
                DeviceInfoControl emvPOS = new DeviceInfoControl("EMV", "ERROR");
                Grid.SetRow(emvPOS, 8);
                Grid.SetColumn(emvPOS, 0);
                operationGrid.Children.Add(emvPOS);
            }
            catch (Exception ex)
            {
            }

            //Kochi-1 Card POS
            try
            {
                DeviceInfoControl cardPOS = new DeviceInfoControl("Card POS", "ERROR");
                Grid.SetRow(cardPOS, 0);
                Grid.SetColumn(cardPOS, 2);
                operationGrid.Children.Add(cardPOS);
            }
            catch (Exception ex)
            {
            }

            //Hopper 1
            try
            {
                DeviceInfoControl hopper1 = new DeviceInfoControl("Hopper 1 Rs.", Parameters.TVMStatic.GetParameter("hopper1Status"));
                Grid.SetRow(hopper1, 2);
                Grid.SetColumn(hopper1, 2);
                operationGrid.Children.Add(hopper1);
            }
            catch (Exception ex)
            {
            }

            //Hopper 2
            try
            {
                DeviceInfoControl hopper2 = new DeviceInfoControl("Hopper 2 Rs.", Parameters.TVMStatic.GetParameter("hopper2Status"));
                Grid.SetRow(hopper2, 4);
                Grid.SetColumn(hopper2, 2);
                operationGrid.Children.Add(hopper2);
            }
            catch (Exception ex)
            {
            }

            //Hopper 5
            try
            {
                DeviceInfoControl hopper5 = new DeviceInfoControl("Hopper 5 Rs.", Parameters.TVMStatic.GetParameter("hopper5Status"));
                Grid.SetRow(hopper5, 6);
                Grid.SetColumn(hopper5, 2);
                operationGrid.Children.Add(hopper5);
            }
            catch (Exception ex)
            {
            }

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