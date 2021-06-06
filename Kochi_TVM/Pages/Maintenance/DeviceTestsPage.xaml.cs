using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Pages.Custom;
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
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for DeviceTestsPage.xaml
    /// </summary>
    public partial class DeviceTestsPage : Page
    {
        public DeviceTestsPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StockOperations.SelStockStatus();

            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Cancel";

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.RcptPrntTest))
            {
                btnTestReceiptPrinter.Visibility = Visibility.Visible;
            }

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.QRTest))
            {
                btnTestQrPrinter.Visibility = Visibility.Visible;
            }

            if (Parameters.menuItems.Contains(Parameters.MenuStrings.HopTest))
            {
                btnTestHopper.Visibility = Visibility.Visible;
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.MaintancePage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }

        private void btnRPTDispenserTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnControlCardTest_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTestReceiptPrinter_Click(object sender, RoutedEventArgs e)
        {
            PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
            if (ReceiptPrinter == PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.TestReceiptPrinter();
                MessageBoxOperations.ShowMessage("Receipt Printer Test", "Test is completed.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("ERROR", "Receipt Printer Error." + ReceiptPrinter, MessageBoxButtonSet.OK);
            }
        }

        private void btnTestHopper_Click(object sender, RoutedEventArgs e)
        {
            CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(1);
            CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(1);
            CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(1);
            MessageBoxOperations.ShowMessage("Hopper Test", "Test is completed.", MessageBoxButtonSet.OK);
        }

        private void btnTestBNA_Click(object sender, RoutedEventArgs e)
        {
            CustomTL60Printer.Instance.TestBNA();
            MessageBoxOperations.ShowMessage("BNA Test", "Test is completed.", MessageBoxButtonSet.OK);
        }

        private void btnTestQrPrinter_Click(object sender, RoutedEventArgs e)
        {
            PRINTER_STATE QRPrinter = CustomKPM150HPrinter.Instance.getStatusWithUsb();
            if (QRPrinter == PRINTER_STATE.OK)
            {
                var qr = Utility.PrepareQRImage(Ticket.startStation.description);
                CustomKPM150HPrinter.Instance.PrintTestQRTicket(qr);
                MessageBoxOperations.ShowMessage("QR Printer Test", "Test is completed.", MessageBoxButtonSet.OK);
            }
            else
            {
                MessageBoxOperations.ShowMessage("ERROR", "Qr Printer Error." + QRPrinter, MessageBoxButtonSet.OK);
            }
        }
    }
}
