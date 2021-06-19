using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
using Kochi_TVM.Utils;
using log4net;
using RPTIssueLib;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(MainPage).Name);

        private static Timer checkDeviceTimer;
        private static TimerCallback checkDeviceTimerDelegate;
        bool Check_Receiptprinter = false;
        public MainPage()
        {
            InitializeComponent();
            btnLang1.IsEnabled = false;
            btnLang1.Opacity = 0.5;
        }
        
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(1,null, Stations.currentStation.name, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(1, Stations.currentStation.name,null , "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(1, Stations.currentStation.name, null, "IN");
            }
        }
        private void CheckDeviceAction(object o)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    PRINTER_STATE QRPrinter = CustomKPM150HPrinter.Instance.getStatusWithUsb();
                    if (QRPrinter == PRINTER_STATE.OK)
                    {
                        btnSelectTicket.IsEnabled = true;
                        btnSelectTicket.Opacity = 1;
                    }
                    else
                    {
                        btnSelectTicket.IsEnabled = false;
                        btnSelectTicket.Opacity = 0.2;
                    }


                    PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
                    if (ReceiptPrinter == PRINTER_STATE.OK)
                    {
                        Check_Receiptprinter = true;
                        Constants.NoReceiptMode = false;
                    }
                    else
                    {
                        Check_Receiptprinter = false;
                        Constants.NoReceiptMode = true;
                    }

                    //DISP_STAT stat = DISP_STAT.STACKER_FULL;
                    //RPTOperations.GetStatus(ref stat);
                    //byte status = 1;

                    //if (stat == DISP_STAT.STACKER_UNKNOWN)
                    //{
                    //    status = 0;
                    //}
                    //else if ((stat == DISP_STAT.STACKER_NOCARD) && (!RPTOperations.IsCardInRFCardOperationPosition()))
                    //{
                    //    status = 0;
                    //}
                    //Parameters.TVMStatic.AddOrUpdateParameter("rptDispenserStatus", status.ToString());
                    //log.Debug("Debug MainPage -> rptDispenserStatus : " + stat.ToString());
                }
                catch (Exception ex)
                {
                    log.Error("Error MainPage -> CheckDeviceAction() : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }
        private void btnSelectTicket_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            try
            {
                if (StockOperations.coin1 <= Constants.NoChangeAvailable || StockOperations.coin2 <= Constants.NoChangeAvailable || StockOperations.coin1 <= Constants.NoChangeAvailable)
                {
                    if (StockOperations.coin5 <= Constants.NoChangeAvailable)
                        lbl5RS.Visibility = Visibility.Collapsed;

                    if (StockOperations.coin2 <= Constants.NoChangeAvailable)
                        lbl2RS.Visibility = Visibility.Collapsed;

                    if (StockOperations.coin1 <= Constants.NoChangeAvailable)
                        lbl1RS.Visibility = Visibility.Collapsed;

                    grdNoChangeMode.Visibility = Visibility.Visible;
                }
                else if (!Check_Receiptprinter)
                {
                    grdNoReceiptPrinterMode.Visibility = Visibility.Visible;
                }
                else
                {
                    NavigationService.Navigate(new Pages.JourneyTypePage());
                }
            }
            catch (Exception ex)
            {
                log.Error("Error MainPage -> btnSelectTicket_Click() : " + ex.ToString());
            }
        }

        private void btnSelectCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLang1_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            btnLang1.IsEnabled = false;
            btnLang1.Opacity = 0.5;

            btnLang2.IsEnabled = true;
            btnLang2.Opacity = 1;

            btnLang3.IsEnabled = true;
            btnLang3.Opacity = 1;

            Ticket.language = Languages.English;
            MultiLanguage.ChangeLanguage("EN");
            SetLanguage();
        }

        private void btnLang2_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            btnLang1.IsEnabled = true;
            btnLang1.Opacity = 1;

            btnLang2.IsEnabled = false;
            btnLang2.Opacity = 0.5;

            btnLang3.IsEnabled = true;
            btnLang3.Opacity = 1;

            Ticket.language = Languages.Malayalam;
            MultiLanguage.ChangeLanguage("ML");
            SetLanguage();
        }

        private void btnLang3_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();

            btnLang1.IsEnabled = true;
            btnLang1.Opacity = 1;

            btnLang2.IsEnabled = true;
            btnLang2.Opacity = 1;

            btnLang3.IsEnabled = false;
            btnLang3.Opacity = 0.5;

            Ticket.language = Languages.Hint;
            MultiLanguage.ChangeLanguage("IN");
            SetLanguage();
        }

        public void SetLanguage()
        {
            ////ÖNCEKİ HAL
            ////current   btn1    btn2
            ////en        hint    ml
            ////hint      en      ml
            ////ml        en      hint   


            ////SON HAL
            ////current   btn1    btn2
            ////en        ml      hint
            ////hint      ml       en
            ////ml        en      hint  

            //// fonksiyon önceki hale göre yapılmış, son hal kararlaştırılsa fonksiyon güncellenecek

            //if (btnLang == BtnLang.CurEnBtn1MlBtn2Hint)
            //{
            //    //set to default            
            //    Ticket.language = Languages.English;
            //}
            //else if (btnLang == BtnLang.CurHintBtn1MlBtn2En)
            //{
            //    if (MultiLanguage.GetCurrentLanguage() == "EN")
            //    {
            //        Ticket.language = Languages.Hint;
            //    }
            //    else
            //    {
            //        SetLanguage(BtnLang.CurEnBtn1MlBtn2Hint);
            //    }
            //}
            //else if (btnLang == BtnLang.CurMlBtn1EnBtn2Hint)
            //{
            //    if (MultiLanguage.GetCurrentLanguage() == "EN")
            //    {
            //        Ticket.language = Languages.Malayalam;
            //    }
               
            //}

            //if (Ticket.language == Languages.English)
            //{
            //    MultiLanguage.ChangeLanguage("EN");
            //}
            //else if (Ticket.language == Languages.Malayalam)
            //{
            //    MultiLanguage.ChangeLanguage("ML");
            //}
            //else if (Ticket.language == Languages.Hint)
            //{
            //    MultiLanguage.ChangeLanguage("IN");
            //}


            lblHeader.Content = MultiLanguage.GetText("welcome");
            btnSelectTicket.Content = MultiLanguage.GetText("buyTicket");
            btnSelectCard.Content = MultiLanguage.GetText("k1card");
            lblComingSoon.Content = MultiLanguage.GetText("comingSoon");
            lblSelectLanguage.Content = MultiLanguage.GetText("selectLang");
            lblNoChange.Content = MultiLanguage.GetText("NoChange");
            lblAvailable.Content = MultiLanguage.GetText("AvailableCoin");
            lblNoReceipt.Content = MultiLanguage.GetText("NoReceipt");
            btnNoChangeExit.Content = MultiLanguage.GetText("Exit");
            btnNoChangeYes.Content = MultiLanguage.GetText("Ok");
            btnReceiptCancel.Content = MultiLanguage.GetText("Exit");
            btnReceiptOK.Content = MultiLanguage.GetText("Ok");
            //btnLang1.Content = "മലയാളം";
            //btnLang2.Content = "हिन्दी";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MultiLanguage.Init("EN");

                Message();

                checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
                checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);

                BNRManager.Instance.PollingAction();
                StockOperations.SelStockStatus();

                LedOperations.GreenText("WELCOME TO " + Stations.currentStation.name);
            }
            catch (Exception ex)
            {
                log.Error("Error MainPage -> Page_Loaded() : " + ex.ToString());
            }
        }
        private void btnReceiptOK_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.JourneyTypePage());
        }

        private void btnReceiptCancel_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdNoReceiptPrinterMode.Visibility = Visibility.Hidden;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (checkDeviceTimer != null)
                    checkDeviceTimer.Dispose();
            }
            catch (Exception ex)
            {
                log.Error("Error MainPage -> Page_Loaded() : " + ex.ToString());
            }
        }
        private void btnNoChangeExit_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdNoChangeMode.Visibility = Visibility.Hidden;
        }

        private void btnNoChangeYes_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            grdNoChangeMode.Visibility = Visibility.Hidden;
            if (!Check_Receiptprinter)
            {
                grdNoReceiptPrinterMode.Visibility = Visibility.Visible;
            }
            else
            {
                NavigationService.Navigate(new Pages.JourneyTypePage());
            }
        }
    }
}
