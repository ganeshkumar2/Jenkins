using dk.CctalkLib.Devices;
using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for CoinOperationPage.xaml
    /// </summary>
    public partial class CoinOperationPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(CoinOperationPage).Name);
        string level = "";
        public CoinOperationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            lblAppVersion.Content = "App Version : " + Parameters.TVMStatic.GetParameter("appVersion");
            lblEquipmentID.Content = "Equipment ID : " + Parameters.TVMDynamic.GetParameter("sys_EquipmentId");
            btnFinish.Content = "Cancel";
            CoinHopperEV4000.ResponseReceivedInputEvent += new CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);
            loadValues();
        }
        void loadValues()
        {
            lblCoin1.Content = "₹ " + Constants.HopperAddress1Coin;
            lblCoin2.Content = "₹ " + Constants.HopperAddress2Coin;
            lblCoin5.Content = "₹ " + Constants.HopperAddress3Coin;

            if (StockOperations.SelStockStatus())
            {
                UpdValOnScr();                
            }  
        }
        private void UpdValOnScr()
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                lblTypeCoin1.Text = Convert.ToString(StockOperations.coin1);
                lblTypeCoin2.Text = Convert.ToString(StockOperations.coin2);
                lblTypeCoin5.Text = Convert.ToString(StockOperations.coin5);

                lblCoin1Info.Content = "₹" + Constants.HopperAddress1Coin + " Coin Count : " + StockOperations.coin1.ToString();
                lblCoin2Info.Content = "₹" + Constants.HopperAddress2Coin + " Coin Count : " + StockOperations.coin2.ToString();
                lblCoin5Info.Content = "₹" + Constants.HopperAddress3Coin + " Coin Count : " + StockOperations.coin5.ToString();
            }), DispatcherPriority.Background);
        }

        int coins_remaining = 0;
        int coins_paid = 0;
        int coins_unpaid = 0;
        private void HooperManager_HopperStateInputEvent(byte[] res, CoinHopperCommands sentCommand)
        {
            try
            {
                log.Debug("Debug  HooperManager_HopperStateInputEvent : " + sentCommand);
                if (sentCommand == CoinHopperCommands.REQUEST_PAYOUT_HIGH_LOW_STATUS)
                {
                    var data = res;
                    var comd = sentCommand;
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        var dataresult = Utils.Utility.Hex2Binary(data[4].ToString());
                        var highorlow = dataresult.Substring(dataresult.Length - 2);
                        var high = highorlow[0];
                        var low = highorlow[1];
                        if (Convert.ToInt16(high.ToString()) == 1)
                        {
                            level = "High Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 1)
                        {
                            level = "Low Level";
                        }
                        else if (Convert.ToInt16(low.ToString()) == 0)
                        {
                            level = "Normal Level";
                        }

                    }));
                }

                if (sentCommand == CoinHopperCommands.REQUEST_HOPPER_STATUS)
                {
                    var data = res;
                    var comd = sentCommand;
                    coins_remaining = Convert.ToInt16(data[5].ToString());
                    coins_paid = Convert.ToInt16(data[6].ToString());
                    coins_unpaid = Convert.ToInt16(data[7].ToString());
                }
            }
            catch (Exception ex)
            {
                log.Error("Error CoinDispenserTestPage -> HooperManager_HopperStateInputEvent() : " + ex.ToString());
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            CoinHopperEV4000.ResponseReceivedInputEvent -= new CoinHopperEV4000.ResponseReceivedEventHandler(HooperManager_HopperStateInputEvent);
        }

        private void lblTypeCoin1_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;
            e.Handled = Regex.IsMatch(e.Text, "[^0-9]+");
        }

        bool coin1 = false;
        private void btnAddCoin1_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();

            if (!coin1)
                return;

            int count = Convert.ToInt32(lblTypeCoin1.Text);
            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(count) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_COIN1);
            if (StockOperations.InsStock(Convert.ToInt64(trxId), (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Increase, Convert.ToInt32(lblTypeCoin1.Text)))
                if (MoneyOperations.InsMoney(Convert.ToInt64(trxId), (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Increase, Convert.ToDecimal(lblTypeCoin1.Text) * 1))
                    if (StockOperations.SelStockStatus())
                    {
                        CustomTL60Printer.Instance.CoinAddPrint(Convert.ToInt32(lblTypeCoin1.Text), Constants.HopperAddress1Coin, StockOperations.coin1);
                        UpdValOnScr();
                        MessageBoxOperations.ShowMessage("ADD COIN", "Added Type : ₹ "+ Constants.HopperAddress1Coin +"\n" +"Added Count : " + count +
                                                "\nAdded Amount : " + "₹" + StockOperations.coin1 * Constants.HopperAddress1Coin + "\n", MessageBoxButtonSet.OK);
                    }
            
            coin1 = false;
        }

        private void btnMinusCoin1_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();

            if (!coin1)
                return;

            int count = Convert.ToInt32(lblTypeCoin1.Text);

            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(lblTypeCoin1.Text) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (StockOperations.coin1 == 0)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "There is no coin in the hopper!", MessageBoxButtonSet.OKCancel);
            else if (count > StockOperations.coin1)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Less value cant be greater than total value!", MessageBoxButtonSet.OKCancel);
            else
            {
                CCTalkManager.Instance.coinHopperEV4000_1.DispenseCoins(count);

                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN1);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, count))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, Convert.ToDecimal(count) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinDispatchPrint(count, Constants.HopperAddress1Coin, StockOperations.coin1);
                            UpdValOnScr();
                            MessageBoxOperations.ShowMessage("LESS COIN", "Removed Type : ₹ " + Constants.HopperAddress1Coin + "\nRemoved Count : " + count +
                                            "\nRemoved Amount : " + "₹" + count * Constants.HopperAddress1Coin + "\n", MessageBoxButtonSet.OK);
                        }
            }
            coin1 = false;
        }

        private void btnEmptyCoin1_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (StockOperations.coin1 == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Stock is already empty.", MessageBoxButtonSet.OKCancel);
                return;
            }
            int stock = StockOperations.coin1;

            CCTalkManager.Instance.coinHopperEV4000_1.EmptyHopper();

            if (stock != StockOperations.coin1)
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN1);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Decrease, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            //PrintOperations.CoinDispatchPrint(stock, 1, StockOperations.coin1);
                            UpdValOnScr();
                        }
            }
            else
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_COIN1);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Empty, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin1, (int)DeviceType.Hopper1, (int)UpdateType.Empty, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinEmptyBoxPrint(stock, Constants.HopperAddress1Coin, StockOperations.coin1);
                            UpdValOnScr();
                        }

                MessageBoxOperations.ShowMessage("EMPTY COIN", "Removed Type :₹\nRemoved Count : " + stock +
                                            "\nRemoved Amount : " + "₹" + stock * Constants.HopperAddress1Coin + "\n", MessageBoxButtonSet.OK);
            }
        }

        bool coin2 = false;
        private void btnAddCoin2_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (!coin2)
                return;

            int count = Convert.ToInt32(lblTypeCoin2.Text);
            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(count) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_COIN2);
            if (StockOperations.InsStock(Convert.ToInt64(trxId), (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Increase, Convert.ToInt32(lblTypeCoin2.Text)))
                if (MoneyOperations.InsMoney(Convert.ToInt64(trxId), (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Increase, Convert.ToDecimal(lblTypeCoin2.Text) * 1))
                    if (StockOperations.SelStockStatus())
                    {
                        CustomTL60Printer.Instance.CoinAddPrint(Convert.ToInt32(lblTypeCoin2.Text), Constants.HopperAddress2Coin, StockOperations.coin2);
                        UpdValOnScr();
                        MessageBoxOperations.ShowMessage("ADD COIN", "Added Type : ₹ " + Constants.HopperAddress2Coin + "\n" + "Added Count : " + count +
                                                "\nAdded Amount : " + "₹" + StockOperations.coin2 * Constants.HopperAddress2Coin + "\n", MessageBoxButtonSet.OK);
                    }

            coin2 = false;
        }

        private void btnMinusCoin2_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (!coin2)
                return;
            int count = Convert.ToInt32(lblTypeCoin2.Text);

            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(lblTypeCoin2.Text) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (StockOperations.coin2 == 0)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "There is no coin in the hopper!", MessageBoxButtonSet.OKCancel);
            else if (count > StockOperations.coin2)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Less value cant be greater than total value!", MessageBoxButtonSet.OKCancel);
            else
            {
                CCTalkManager.Instance.coinHopperEV4000_2.DispenseCoins(count);

                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN2);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, count))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, Convert.ToDecimal(count) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinDispatchPrint(count, Constants.HopperAddress2Coin, StockOperations.coin2);
                            UpdValOnScr();
                            MessageBoxOperations.ShowMessage("LESS COIN", "Removed Type : ₹ " + Constants.HopperAddress2Coin + "\nRemoved Count : " + count +
                                            "\nRemoved Amount : " + "₹" + count * Constants.HopperAddress2Coin + "\n", MessageBoxButtonSet.OK);
                        }
            }
            coin2 = false;
        }

        private void btnEmptyCoin2_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (StockOperations.coin2 == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Stock is already empty.", MessageBoxButtonSet.OKCancel);
                return;
            }
            int stock = StockOperations.coin2;

            CCTalkManager.Instance.coinHopperEV4000_2.EmptyHopper();

            if (stock != StockOperations.coin2)
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN2);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Decrease, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            //PrintOperations.CoinDispatchPrint(stock, 1, StockOperations.coin1);
                            UpdValOnScr();
                        }
            }
            else
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_COIN2);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Empty, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin2, (int)DeviceType.Hopper2, (int)UpdateType.Empty, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinEmptyBoxPrint(stock, Constants.HopperAddress2Coin, StockOperations.coin2);
                            UpdValOnScr();
                        }

                MessageBoxOperations.ShowMessage("EMPTY COIN", "Removed Type :₹\nRemoved Count : " + stock +
                                            "\nRemoved Amount : " + "₹" + stock * Constants.HopperAddress2Coin + "\n", MessageBoxButtonSet.OK);
            }
        }
        bool coin3 = false;
        private void btnAddCoin5_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (!coin3)
                return;

            int count = Convert.ToInt32(lblTypeCoin5.Text);
            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(count) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_ADD_COIN5);
            if (StockOperations.InsStock(Convert.ToInt64(trxId), (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Increase, Convert.ToInt32(lblTypeCoin5.Text)))
                if (MoneyOperations.InsMoney(Convert.ToInt64(trxId), (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Increase, Convert.ToDecimal(lblTypeCoin5.Text) * 1))
                    if (StockOperations.SelStockStatus())
                    {
                        CustomTL60Printer.Instance.CoinAddPrint(Convert.ToInt32(lblTypeCoin5.Text), Constants.HopperAddress3Coin, StockOperations.coin5);
                        UpdValOnScr();
                        MessageBoxOperations.ShowMessage("ADD COIN", "Added Type : ₹ " + Constants.HopperAddress3Coin + "\n" + "Added Count : " + count +
                                                "\nAdded Amount : " + "₹" + StockOperations.coin5 * Constants.HopperAddress3Coin + "\n", MessageBoxButtonSet.OK);
                    }

            coin3 = false;
        }

        private void btnMinusCoin5_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (!coin3)
                return;
            int count = Convert.ToInt32(lblTypeCoin5.Text);

            if (count == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (Convert.ToInt32(lblTypeCoin5.Text) < 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Please enter valid count", MessageBoxButtonSet.OKCancel);
                return;
            }

            if (StockOperations.coin5 == 0)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "There is no coin in the hopper!", MessageBoxButtonSet.OKCancel);
            else if (count > StockOperations.coin5)
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Less value cant be greater than total value!", MessageBoxButtonSet.OKCancel);
            else
            {
                CCTalkManager.Instance.coinHopperEV4000_3.DispenseCoins(count);

                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN5);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, count))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, Convert.ToDecimal(count) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinDispatchPrint(count, Constants.HopperAddress3Coin, StockOperations.coin5);
                            UpdValOnScr();
                            MessageBoxOperations.ShowMessage("LESS COIN", "Removed Type : ₹ " + Constants.HopperAddress3Coin + "\nRemoved Count : " + count +
                                            "\nRemoved Amount : " + "₹" + count * Constants.HopperAddress3Coin + "\n", MessageBoxButtonSet.OK);
                        }
            }
            coin3 = false;
        }

        private void btnEmptyCoin5_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            if (StockOperations.coin5 == 0)
            {
                MessageBoxOperations.ShowMessage("ATTENTION!!", "Stock is already empty.", MessageBoxButtonSet.OKCancel);
                return;
            }
            int stock = StockOperations.coin5;

            CCTalkManager.Instance.coinHopperEV4000_3.EmptyHopper();

            if (stock != StockOperations.coin5)
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_REMOVE_COIN5);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Decrease, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            //PrintOperations.CoinDispatchPrint(stock, 1, StockOperations.coin1);
                            UpdValOnScr();
                        }
            }
            else
            {
                decimal? trxId = TransactionInfo.SelTrxId((long)TransactionType.TT_EMPTY_COIN5);
                if (StockOperations.InsStock((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Empty, stock))
                    if (MoneyOperations.InsMoney((Int64)trxId, (int)StockType.Coin5, (int)DeviceType.Hopper5, (int)UpdateType.Empty, Convert.ToDecimal(stock) * 1))
                        if (StockOperations.SelStockStatus())
                        {
                            CustomTL60Printer.Instance.CoinEmptyBoxPrint(stock, Constants.HopperAddress3Coin, StockOperations.coin5);
                            UpdValOnScr();
                        }

                MessageBoxOperations.ShowMessage("EMPTY COIN", "Removed Type :₹" + "\nRemoved Count : " + stock +
                                            "\nRemoved Amount : " + "₹" + stock * Constants.HopperAddress3Coin + "\n", MessageBoxButtonSet.OK);
            }
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

        private void lblTypeCoin1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int i = NumberPadOperations.GetNumber();
            if (i != 0)
            {
                lblTypeCoin1.Text = i.ToString();
                coin1 = true;
            }
        }

        private void lblTypeCoin2_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int i = NumberPadOperations.GetNumber();
            if (i != 0)
            {
                lblTypeCoin2.Text = i.ToString();
                coin2 = true;
            }
        }

        private void lblTypeCoin5_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            int i = NumberPadOperations.GetNumber();
            if (i != 0)
            {
                lblTypeCoin5.Text = i.ToString();
                coin3 = true;
            }
        }
    }
}
