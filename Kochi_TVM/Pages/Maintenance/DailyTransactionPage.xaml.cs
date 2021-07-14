﻿using Kochi_TVM.Business;
using Kochi_TVM.Models;
using Kochi_TVM.Printers;
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
    /// Interaction logic for DailyTransactionPage.xaml
    /// </summary>
    public partial class DailyTransactionPage : Page
    {
        int QRSJTCashCount = 0;
        int QRSJTCashAmount = 0;
        int QRSJTNonCashCount = 0;
        int QRSJTNonCashAmount = 0;
        int QRRJTCashCount = 0;
        int QRRJTCashAmount = 0;
        int QRRJTNonCashCount = 0;
        int QRRJTNonCashAmount = 0;
        int RPTSJTCashCount = 0;
        int RPTSJTCashAmount = 0;
        int RPTSJTNonCashCount = 0;
        int RPTSJTNonCashAmount = 0;
        public DailyTransactionPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
                lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("descCode");
                btnFinish.Content = "Cancel";

                using (var context = new TVM_Entities())
                {
                    DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                    DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                    var trxData = context.sp_SelShiftPaymentReport(Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), Stations.currentStation.id, 0, 0, startDate, endDate).ToList();
                    foreach (var data in trxData)
                    {
                        if (Convert.ToString(data.Transaction) == "QR SJT-CASH")
                        {
                            QRSJTCashCount = Convert.ToInt32(data.Count);
                            QRSJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "QR SJT-NonCASH")
                        {
                            QRSJTNonCashCount = Convert.ToInt32(data.Count);
                            QRSJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        if (Convert.ToString(data.Transaction) == "QR RJT-CASH")
                        {
                            QRRJTCashCount = Convert.ToInt32(data.Count);
                            QRRJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "QR RJT-NonCASH")
                        {
                            QRRJTNonCashCount = Convert.ToInt32(data.Count);
                            QRRJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT SJT-CASH")
                        {
                            RPTSJTCashCount = Convert.ToInt32(data.Count);
                            RPTSJTCashAmount = Convert.ToInt32(data.Amount);
                        }
                        else if (Convert.ToString(data.Transaction) == "RPT SJT-NonCASH")
                        {
                            RPTSJTNonCashCount = Convert.ToInt32(data.Count);
                            RPTSJTNonCashAmount = Convert.ToInt32(data.Amount);
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }            

            lblQRSJTCashCount.Content = QRSJTCashCount;
            lblQRSJTCashAmount.Content = Conversion.MoneyFormat(QRSJTCashAmount);
            lblQRSJTNonCashCount.Content = QRSJTNonCashCount;
            lblQRSJTNonCashAmount.Content = Conversion.MoneyFormat(QRSJTNonCashAmount);
            lblQRRJTCashCount.Content = QRRJTCashCount;
            lblQRRJTCashAmount.Content = Conversion.MoneyFormat(QRRJTCashAmount);
            lblQRRJTNonCashCount.Content = QRRJTNonCashCount;
            lblQRRJTNonCashAmount.Content = Conversion.MoneyFormat(QRRJTNonCashAmount);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            CustomTL60Printer.Instance.StandAloneReceipt();
        }
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminInfoPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
    }
}
