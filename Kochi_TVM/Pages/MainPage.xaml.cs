using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Printers;
using Kochi_TVM.Utils;
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
        private static Timer checkDeviceTimer;
        private static TimerCallback checkDeviceTimerDelegate;
        bool Check_Receiptprinter = false;
        public MainPage()
        {
            InitializeComponent();
            MultiLanguage.Init("EN");
            Message();
            checkDeviceTimerDelegate = new TimerCallback(CheckDeviceAction);
            checkDeviceTimer = new Timer(checkDeviceTimerDelegate, null, 1000, Constants.CheckDeviceTime);
        }
        
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1,null, Stations.currentStation.name, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1, Stations.currentStation.name,null , "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1, Stations.currentStation.name, null, "IN");
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
                    }
                    else
                    {
                        Check_Receiptprinter = false;
                    }
                }
                catch (Exception ex)
                {
                }
            }), DispatcherPriority.Background);
        }
        private void btnSelectTicket_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
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
                //log.Error("Error OptionPage -> BrdQRInfo_MouseLeftButtonDown() : " + ex.ToString());
            }
        }

        private void btnSelectCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLang1_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            SetLanguage(BtnLang.CurHintBtn1MlBtn2En);
        }

        private void btnLang2_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            SetLanguage(BtnLang.CurMlBtn1EnBtn2Hint);
        }

        public void SetLanguage(BtnLang btnLang)
        {
            //ÖNCEKİ HAL
            //current   btn1    btn2
            //en        hint    ml
            //hint      en      ml
            //ml        en      hint   


            //SON HAL
            //current   btn1    btn2
            //en        ml      hint
            //hint      ml       en
            //ml        en      hint  

            // fonksiyon önceki hale göre yapılmış, son hal kararlaştırılsa fonksiyon güncellenecek

            if (btnLang == BtnLang.CurEnBtn1MlBtn2Hint)
            {
                //set to default            
                Ticket.language = Languages.English;
            }
            else if (btnLang == BtnLang.CurHintBtn1MlBtn2En)
            {
                if (MultiLanguage.GetCurrentLanguage() == "EN")
                {
                    Ticket.language = Languages.Hint;
                }
                else
                {
                    SetLanguage(BtnLang.CurEnBtn1MlBtn2Hint);
                }
            }
            else if (btnLang == BtnLang.CurMlBtn1EnBtn2Hint)
            {
                if (MultiLanguage.GetCurrentLanguage() == "EN")
                {
                    Ticket.language = Languages.Malayalam;
                }
               
            }

            if (Ticket.language == Languages.English)
            {
                MultiLanguage.ChangeLanguage("EN");
            }
            else if (Ticket.language == Languages.Malayalam)
            {
                MultiLanguage.ChangeLanguage("ML");
            }
            else if (Ticket.language == Languages.Hint)
            {
                MultiLanguage.ChangeLanguage("IN");
            }


            lblHeader.Content = MultiLanguage.GetText("welcome");
            btnSelectTicket.Content = MultiLanguage.GetText("buyTicket");
            btnSelectCard.Content = MultiLanguage.GetText("k1card");
            lblComingSoon.Content = MultiLanguage.GetText("comingSoon");
            lblSelectLanguage.Content = MultiLanguage.GetText("selectLang");

            //btnLang1.Content = "മലയാളം";
            //btnLang2.Content = "हिन्दी";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BNRManager.Instance.PollingAction();
            new Thread(() => AsyncDBCal()).Start();
        }
        private void AsyncDBCal()
        {
            try
            {
                StockOperations.SelStockStatus();
            }
            catch (Exception)
            { }
        }
        private void btnReceiptOK_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.JourneyTypePage());
        }

        private void btnReceiptCancel_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            grdNoReceiptPrinterMode.Visibility = Visibility.Hidden;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (checkDeviceTimer != null)
                checkDeviceTimer.Dispose();
        }

        private void btnNoChangeExit_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            grdNoChangeMode.Visibility = Visibility.Hidden;
        }

        private void btnNoChangeYes_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
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
