using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Pages.Custom;
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
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages.Maintenance
{
    /// <summary>
    /// Interaction logic for BNROperationPage.xaml
    /// </summary>
    public partial class BNROperationPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(BNROperationPage).Name);
        List<BillTable> billTable = new List<BillTable>();
        List<Cassette> cassettes = new List<Cassette>();
        int noteincasset1 = 0, noteincasset2 = 0, noteincasset3 = 0;
        int notevalincasset1 = 0, notevalincasset2 = 0, notevalincasset3 = 0;
        int Casette1Billtype = 0, Casette2Billtype = 0, Casette3Billtype = 0;
        public BNROperationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Cancel";
            BNRManager.Instance.PollingAction();
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent += new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.Instance.GetBillTableProcess();
            BNRManager.BNRCurrencyStateInputEvent += new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
            BNRManager.Instance.GetCassetteStatus();
            BNRManager.BNRCassetteStatusInputEvent += new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
            lblBNR.Content = Constants.BNRStatus;
        }

        private void BNRManager_BNRBillTableInputEvent(List<BillTable> billTables)
        {
            try
            {
                billTable = billTables;
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> BNRManager_BNRBillTableInputEvent() : " + ex.ToString());
            }
        }

        private void BNRManager_BNRCassetteStatusInputEvent(List<Cassette> cassette)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (cassette[0].billType == 0 && cassette[1].billType == 0 && cassette[2].billType == 0)
                    {
                        BNRManager.Instance.GetCassetteStatus();
                    }
                    cassettes = cassette;
                    foreach (Cassette cassetteset in cassettes)
                    {
                        int bill = billTable.Where(x => x.BillType == cassetteset.billType).Select(x => x.DigitBillType).FirstOrDefault();
                        string billval = Convert.ToString(bill);
                        if (cassetteset.cassetteId == 1)
                        {
                            noteincasset1 = cassetteset.billNumber;
                            notevalincasset1 = bill;
                            lblCassette1.Content = "₹ " + bill;
                            Casette1Billtype = cassetteset.billType;
                            lblCassette1Info.Content = "₹ " + bill + " ₹ " + (noteincasset1 * notevalincasset1);
                        }
                        if (cassetteset.cassetteId == 2)
                        {
                            noteincasset2 = cassetteset.billNumber;
                            notevalincasset2 = bill;
                            lblCassette2.Content = "₹ " + bill;
                            Casette2Billtype = cassetteset.billType;
                            lblCassette2Info.Content = "₹ " + bill + " ₹ " + (noteincasset2 * notevalincasset2);
                        }
                        if (cassetteset.cassetteId == 3)
                        {
                            noteincasset3 = cassetteset.billNumber;
                            notevalincasset3 = Constants.EscrowAmount;
                            lblCassette3.Content = "₹ " + Constants.EscrowAmount.ToString();
                            Casette3Billtype = cassetteset.billType;
                            lblCassette3Info.Content = "₹ " + notevalincasset3 + " ₹ " + (noteincasset3 * notevalincasset3);
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRCassetteStatusInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void BNRManager_BNRCurrencyStateInputEvent(List<StackedNotes> stackedNotesListBox)
        {
            try
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    int bill = 0;
                    int billCount = 0;
                    int total = 0;
                    foreach (var stackedNote in stackedNotesListBox)
                    {
                        if (stackedNote.BillNumber > 0)
                        {
                            bill = billTable.Where(x => x.BillType == stackedNote.BillType).Select(x => x.DigitBillType).FirstOrDefault();
                            billCount = stackedNote.BillNumber;
                            total += bill * billCount;
                        }
                    }
                    lblBNRAmount.Content = Convert.ToString(total);
                }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> BNRManager_BNRCurrencyStateInputEvent() : " + ex.ToString());
            }
        }

        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    lblBNR.Content = Constants.BNRStatus;
                }
                catch (Exception ex)
                {
                    log.Error("Error BNROperationPage -> BNRManager_BNRStateInputEvent() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
            BNRManager.BNRBillTableInputEvent -= new BNRManager.BNRBillTableEventHandler(BNRManager_BNRBillTableInputEvent);
            BNRManager.BNRCurrencyStateInputEvent -= new BNRManager.BNRCurrencyStateEventHandler(BNRManager_BNRCurrencyStateInputEvent);
            BNRManager.BNRCassetteStatusInputEvent -= new BNRManager.BNRCassetteStatusEventHandler(BNRManager_BNRCassetteStatusInputEvent);
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.OperationPage());
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.Maintenance.AdminMainPage());
        }
        bool isAccepting = false;
        private void btnAddMoney_Click(object sender, RoutedEventArgs e)
        {
            if (!isAccepting)
            {
                Utility.PlayClick();
                grdMoneyAccept.Visibility = Visibility.Visible;
                isAccepting = true;
                if (notevalincasset1 == 10 && notevalincasset2 == 20 && notevalincasset3 == 50)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x0E;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 10 && notevalincasset2 == 20 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x16;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 10 && notevalincasset2 == 50 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x1A;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 20 && notevalincasset2 == 50 && notevalincasset3 == 100)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x1C;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 20 && notevalincasset2 == 100 && notevalincasset3 == 200)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x34;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
                else if (notevalincasset1 == 50 && notevalincasset2 == 100 && notevalincasset3 == 200)
                {
                    byte[] arr_enable = new byte[6];
                    arr_enable[0] = 0xFF;
                    arr_enable[1] = 0xFF;
                    arr_enable[2] = 0x38;
                    arr_enable[3] = 0x00;
                    arr_enable[4] = 0x00;
                    arr_enable[5] = 0x00;
                    BNRManager.Instance.AcceptProcess(arr_enable);
                }
            }
        }

        private void btnSendBox1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Utility.PlayClick();
                if (Constants.BNRStatus == "DISABLED")
                {
                    if (noteincasset1 != 0)
                    {
                        UnloadCassette(1, noteincasset1);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox1_Click() : " + ex.ToString());
            }
        }

        private async void btnStopMoney_Click(object sender, RoutedEventArgs e)
        {
            if (isAccepting)
            {
                Utility.PlayClick();
                grdMoneyAccept.Visibility = Visibility.Hidden;
                BNRManager.Instance.StopProcess();
                await Task.Delay(300);
                BNRManager.Instance.GetCassetteStatus();
                await Task.Delay(1000);
                isAccepting = false;
            }
        }

        private void btnSendBox2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Utility.PlayClick();
                if (Constants.BNRStatus == "DISABLED")
                {
                    if (noteincasset2 != 0)
                    {
                        UnloadCassette(2, noteincasset2);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox2_Click() : " + ex.ToString());
            }            
        }

        private void btnSendBox3_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Utility.PlayClick();
                if (Constants.BNRStatus == "DISABLED")
                {
                    if (noteincasset3 != 0)
                    {
                        UnloadCassette(3, noteincasset3);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error BNROperationPage -> btnSendBox3_Click() : " + ex.ToString());
            }
        }

        private void btnClearBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void UnloadCassette(int cassetteNumber, int count)
        {
            try
            {
                BNRManager.Instance.UnloadCassette(cassetteNumber, count);
            }
            catch (Exception ex)
            {
                log.Error("Error BNRTestPage -> UnloadCassette() : " + ex.ToString());
            }
        }
    }
}
