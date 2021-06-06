using Kochi_TVM.Business;
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
using System.Windows.Threading;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for StockInfoPage.xaml
    /// </summary>
    public partial class StockInfoPage : Page
    {
        public StockInfoPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ShoWDevicesInfo();

            if (StockOperations.SelStockStatus())
                if (MoneyOperations.SelMoneyStatus())
                    UpdValOnScr();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Cancel";
        }

        private void UpdValOnScr()
        {
            #region dispenser
            //DispenserValue.Content = StockOperations.rpt.ToString();
            #endregion

            #region hopper
            hoppersTotalValue.Content = String.Format("{0}", Conversion.MoneyFormat(MoneyOperations.coin1 + MoneyOperations.coin2 + MoneyOperations.coin5));
            hopper5Value.Content = hopper2Value.Content = String.Format(StockOperations.coin5.ToString() + "x5 = {0}", Conversion.MoneyFormat(MoneyOperations.coin5));
            hopper2Value.Content = String.Format(StockOperations.coin2.ToString() + "x2 = {0}", Conversion.MoneyFormat(MoneyOperations.coin2));
            hopper1Value.Content = String.Format(StockOperations.coin1.ToString() + "x1 = {0}", Conversion.MoneyFormat(MoneyOperations.coin1));
            #endregion

            #region bna
            bnaTotalValue.Content = String.Format("{0}", Conversion.MoneyFormat(MoneyOperations.banknote20 + MoneyOperations.banknote10 + MoneyOperations.box));
            bnaCassette2Value.Content = String.Format(StockOperations.banknote20.ToString() + " x 20 = {0}", Conversion.MoneyFormat(MoneyOperations.banknote20));
            bnaCassette3Value.Content = String.Format(StockOperations.banknote10.ToString() + " x 10 = {0}", Conversion.MoneyFormat(MoneyOperations.banknote10));
            bnaBoxValue.Content = String.Format("{0}", Conversion.MoneyFormat(MoneyOperations.box));
            #endregion

            #region receipts
            QRPaperValue.Content = Convert.ToString(StockOperations.qrSlip);
            //receiptValue.Content = Convert.ToString(StockOperations.receiptSlip);
            #endregion

            #region grandtotal
            GrandTotalAmount.Content = Convert.ToString(Conversion.MoneyFormat(MoneyOperations.banknote10 + MoneyOperations.banknote20 +
                                                        MoneyOperations.box + MoneyOperations.coin1 + MoneyOperations.coin2 +
                                                        MoneyOperations.coin5));

            HopperGrandAmount.Content = Convert.ToString(Conversion.MoneyFormat(MoneyOperations.coin1 + MoneyOperations.coin2 +
                                                        MoneyOperations.coin5));

            BanknoteGrandAmount.Content = Convert.ToString(Conversion.MoneyFormat(MoneyOperations.banknote10 + MoneyOperations.banknote20 +
                                                        MoneyOperations.box));
            #endregion

        }

        private void ShoWDevicesInfo()
        {
            try
            {
                int qrcPrinterStatus = 0;
                int rptDispenserStatus = 0;
                int bnaStatus = 0;

                if (Parameters.TVMStatic.GetParameter("bnaStatus") != String.Empty)
                {
                    bnaStatus = Convert.ToInt32(Parameters.TVMStatic.GetParameter("bnaStatus"));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { btnCash.Foreground = (bnaStatus == 0) ? Brushes.Black : Brushes.WhiteSmoke; }));
                }
                if (Parameters.TVMStatic.GetParameter("qrcPrinterStatus") != String.Empty)
                {
                    qrcPrinterStatus = Convert.ToInt32(Parameters.TVMStatic.GetParameter("qrcPrinterStatus"));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { btnQRPrinter.Foreground = (qrcPrinterStatus == 0) ? Brushes.Black : Brushes.WhiteSmoke; }));
                }
                if (Parameters.TVMStatic.GetParameter("rptDispenserStatus") != String.Empty)
                {
                    rptDispenserStatus = Convert.ToInt32(Parameters.TVMStatic.GetParameter("rptDispenserStatus"));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => { btnRpt.Foreground = (rptDispenserStatus == 0) ? Brushes.Black : Brushes.WhiteSmoke; }));
                }

                if (Parameters.TVMStatic.GetParameter("qrcPaperStatus") != String.Empty)
                {
                    QRPaperValue.Content = Parameters.TVMStatic.GetParameter("qrcPaperStatus");
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
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

        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            CustomTL60Printer.Instance.StockStatusReport(StockOperations.coin1, StockOperations.coin2, StockOperations.coin5, StockOperations.qrSlip,
               StockOperations.rpt, StockOperations.receiptSlip, StockOperations.banknote10, StockOperations.banknote20,
               Convert.ToInt32(MoneyOperations.box));
        }
    }
}
