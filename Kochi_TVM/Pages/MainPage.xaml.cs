using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Models;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
using Kochi_TVM.RptDispenser;
using Kochi_TVM.Utils;
using log4net;
using RPTIssueLib;
using System;
using System.ComponentModel;
using System.Linq;
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
        DispatcherTimer timerOccConnMsg;
        private readonly BackgroundWorker bwAfcStatus = null;
        private readonly BackgroundWorker bwSendSc = null;
        private readonly BackgroundWorker bwSendMonitoring = null;

        bool Check_Receiptprinter = false;
        bool Check_QRprinter = false;
        public MainPage()
        {
            InitializeComponent();
            try
            {
                timerOccConnMsg = new DispatcherTimer();
                //bwAfcStatus = new BackgroundWorker
                //{
                //    WorkerReportsProgress = true,
                //    WorkerSupportsCancellation = true
                //};

                bwSendSc = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                bwSendMonitoring = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };

                //bwAfcStatus.ProgressChanged += bwAfcStatus_ProgressChanged;
                //bwAfcStatus.DoWork += bwAfcStatus_DoWork;
                //bwAfcStatus.RunWorkerAsync();

                bwSendSc.DoWork += bwSendSc_DoWork;
                bwSendSc.RunWorkerAsync();

                bwSendMonitoring.DoWork += bwSendMonitoring_DoWork;
                bwSendMonitoring.RunWorkerAsync();

                btnLang1.IsEnabled = false;
                btnLang1.Opacity = 0.5;

                Ticket.Clear();
            }
            catch (Exception ex)
            {
                log.Error("MainPage() " + ex.ToString());
            }
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
        void timerOccConnMsg_Tick(object sender, EventArgs e)
        {
            try
            {

                DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                if ((startDate <= DateTime.Now) && (endDate >= DateTime.Now))
                {

                    if (Parameters.TVMDynamic.GetParameter("AfcConn") == "1")
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            lblNoConnection.Content = "";
                        });
                    }
                }
            }

            //if (!Devices.inUse)
            //{
            //    Devices.InitHopper1();
            //    Parameters.TVMStatic.AddOrUpdateParameter("hopper1Status", Devices.GetStatusHopper() == true ? "OK" : "ERROR");

            //    Devices.InitHopper2();
            //    Parameters.TVMStatic.AddOrUpdateParameter("hopper2Status", Devices.GetStatusHopper() == true ? "OK" : "ERROR");

            //    Devices.InitHopper5();
            //    Parameters.TVMStatic.AddOrUpdateParameter("hopper5Status", Devices.GetStatusHopper() == true ? "OK" : "ERROR");
            //}

            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void bwAfcStatus_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var worker = sender as BackgroundWorker;
                DateTime lastUpdate = DateTime.Now;
                do
                {

                    TimeSpan diff = DateTime.Now - lastUpdate;
                    if (diff.Seconds > 2)
                    {
                        var result = Parameters.TVMDynamic.GetAfcConnStatus();
                        lastUpdate = DateTime.Now;
                        worker?.ReportProgress(0, result);
                    }

                    Thread.Sleep(100);

                } while (worker != null && !worker.CancellationPending);

            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void bwAfcStatus_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {

                DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                if ((startDate <= DateTime.Now) && (endDate >= DateTime.Now))
                {
                    var stat = (bool)e.UserState;
                    byte status = 1;

                    if (stat)
                        status = 1;
                    else
                        status = 0;

                    Parameters.TVMDynamic.AddOrUpdateParameter("AfcConn", status.ToString());

                    if (Parameters.TVMDynamic.GetParameter("AfcConn") == "1")
                    {
                        lblNoConnection.Content = "";
                        btnSelectTicket.IsEnabled = true;
                        btnSelectTicket.Opacity = 1;
                        //PageControl.ShowPage(Pages.journeyPage);
                    }
                    else
                    {
                        lblNoConnection.Content = "No Connection!";
                        btnSelectTicket.IsEnabled = false;
                        btnSelectTicket.Opacity = 0.2;
                    }
                }
                else
                {
                    NavigationService.Navigate(new Pages.StationClosedPage());
                }
                bwAfcStatus.CancelAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
            }
        }
        private void bwSendSc_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                try
                {
                    StockOperations.SelStockStatus();

                    if (StockOperations.qrSlip <= 10)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                            string.Format("QR PAPER STOCK IS LESS! Please ADD PAPER"));

                        log.Debug("InsNStationAlarm qr --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.coin1 == 0)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                            string.Format("Rs "+ Constants.HopperAddress1Coin +" stock is empty! Please refill."));

                        log.Debug("InsNStationAlarm coin1 --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.coin2 == 0)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                            string.Format("Rs " + Constants.HopperAddress2Coin + " stock is empty! Please refill."));

                        log.Debug("InsNStationAlarm coin2 --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.coin5 == 0)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                            string.Format("Rs " + Constants.HopperAddress3Coin + " stock is empty! Please refill."));

                        log.Debug("InsNStationAlarm coin5 --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.banknote10 <= 5)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                        string.Format("Rs "+ TVMUtility.BillTypeToBillValue(Constants.Cassette1Note) + " currency (Cassette 1) stock is less! Please add notes."));

                        log.Debug("InsNStationAlarm banknote10 --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.banknote20 <= 5)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                        string.Format("Rs " + TVMUtility.BillTypeToBillValue(Constants.Cassette2Note) + " currency (Cassette 2) stock is less! Please add notes."));

                        log.Debug("InsNStationAlarm banknote20 --> result : " + (result == true ? "true" : "false"));
                    }

                    if (StockOperations.escrow <= 5)
                    {
                        bool result = Parameters.InsNStationAlarm(Stations.currentStation.id, Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), 1,
                        string.Format("Rs " + TVMUtility.BillTypeToBillValue(Constants.Cassette3Note)+ " currency (Cassette 3) stock is less! Please add notes."));

                        log.Debug("InsNStationAlarm banknote20 --> result : " + (result == true ? "true" : "false"));
                    }

                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                }

                Parameters.lastSync = DateTime.Now;
                Thread.Sleep(60000 * 5);
            }

        }
        private void bwSendMonitoring_DoWork(object sender, DoWorkEventArgs e)
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

            bool result = false;

            while (true)
            {
                try
                {
                    DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                    DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                    using (var context = new TVM_Entities())
                    {
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
                    
                    Parameters.TvmMonitoringData.appVersion = Parameters.TVMStatic.GetParameter("appVersion");
                    Parameters.TvmMonitoringData.banknote10 = StockOperations.banknote10.ToString();
                    Parameters.TvmMonitoringData.banknote20 = StockOperations.banknote20.ToString();
                    Parameters.TvmMonitoringData.bnrStatus = Parameters.TVMStatic.GetParameter("bnaStatus");
                    Parameters.TvmMonitoringData.doorSensorStatus = " ";
                    Parameters.TvmMonitoringData.hopperCoins1 = StockOperations.coin1.ToString();
                    Parameters.TvmMonitoringData.hopperCoins2 = StockOperations.coin2.ToString();
                    Parameters.TvmMonitoringData.hopperCoins5 = StockOperations.coin5.ToString();
                    Parameters.TvmMonitoringData.hopperStatus1 = Parameters.TVMStatic.GetParameter("hopper1Status");
                    Parameters.TvmMonitoringData.hopperStatus2 = Parameters.TVMStatic.GetParameter("hopper2Status");
                    Parameters.TvmMonitoringData.hopperStatus5 = Parameters.TVMStatic.GetParameter("hopper5Status");
                    Parameters.TvmMonitoringData.lastTransactionDate = DateTime.Now;
                    Parameters.TvmMonitoringData.ledPanelStatus = " ";
                    Parameters.TvmMonitoringData.numberOfQr = StockOperations.qrSlip;
                    Parameters.TvmMonitoringData.qrPrinterStatus = Check_QRprinter == true ? "OK" : "ERROR";
                    Parameters.TvmMonitoringData.QRRJT_Amount = QRRJTCashAmount;
                    Parameters.TvmMonitoringData.QRRJT_Count = QRRJTCashCount;
                    Parameters.TvmMonitoringData.QRSJT_Amount = QRSJTCashAmount;
                    Parameters.TvmMonitoringData.QRSJT_Count = QRSJTCashCount;
                    Parameters.TvmMonitoringData.receiptPrinterStatus = Check_Receiptprinter == true ? "OK" : "ERROR";
                    Parameters.TvmMonitoringData.speakerStatus = " ";
                    Parameters.TvmMonitoringData.stationId = Stations.currentStation.id;
                    Parameters.TvmMonitoringData.Total_Amount = QRRJTCashAmount + QRSJTCashAmount;
                    Parameters.TvmMonitoringData.Total_Count = QRRJTCashCount + QRSJTCashCount;
                    Parameters.TvmMonitoringData.tvmId = Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId"));

                    result = Parameters.insTVMMonitoring();
                    log.Debug("--SC Conn insTVMMonitoring--" + result);
                    if (result)
                    {
                        Parameters.TVMStatic.AddOrUpdateParameter("SCConn", "1");
                        Parameters.lastSync = DateTime.Now;
                    }
                    else
                    {
                        Parameters.TVMStatic.AddOrUpdateParameter("SCConn", "0");
                        log.Debug("--SC Conn Error--");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.ToString());
                    Parameters.TVMStatic.AddOrUpdateParameter("SCConn", "0");
                }

                Thread.Sleep(30000);
            }
        }
        private void CheckDeviceAction(object o)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Parameters.TVMDynamic.FillOrUpdateParameters();
                    DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                    DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                    if (!((startDate <= DateTime.Now) && (endDate >= DateTime.Now)))
                    {
                        NavigationService.Navigate(new Pages.StationClosedPage());
                    }
                    int i = 0;
                    if (Parameters.TVMDynamic.GetAfcConnStatus())
                    {
                        Parameters.TVMDynamic.AddOrUpdateParameter("AfcConn", "1");
                        Parameters.TVMStatic.AddOrUpdateParameter("SCConn", "1");
                        i = 1;
                        lblNoConnection.Content = "";
                        btnSelectTicket.IsEnabled = true;
                        btnSelectTicket.Opacity = 1;
                        if (i == 1)
                        {
                            i = 2;
                            LedOperations.GreenText("WELCOME TO " + Stations.currentStation.name + " " + PIDMessageLog.getMessage());
                        }
                    }
                    else
                    {
                        Parameters.TVMDynamic.AddOrUpdateParameter("AfcConn", "0");
                        Parameters.TVMStatic.AddOrUpdateParameter("SCConn", "0");
                        i = 0;
                        LedOperations.Close();
                        lblNoConnection.Content = "No Connection!";
                        btnSelectTicket.IsEnabled = false;
                        btnSelectTicket.Opacity = 0.2;
                        return;
                    }

                    int j = 0;
                    PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();//CustomKPM150HPrinter.Instance.getStatusWithUsb();
                    if (QRStatus == PRINTER_STATE.OK)
                    {
                        j = 1;
                        Check_QRprinter = true;
                        btnSelectTicket.IsEnabled = true;
                        lblNoConnection.Content = "";
                        btnSelectTicket.Opacity = 1;
                        if (j == 1)
                        {
                            j = 2;
                            LedOperations.GreenText("WELCOME TO " + Stations.currentStation.name + " " + PIDMessageLog.getMessage());
                        }
                    }
                    else
                    {
                        j = 0;
                        LedOperations.DeviceError("");
                        Check_QRprinter = false;
                        btnSelectTicket.IsEnabled = false;
                        lblNoConnection.Content = "Device Error";
                        btnSelectTicket.Opacity = 0.2;
                        return;
                    }


                    PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
                    if (ReceiptPrinter == PRINTER_STATE.OK)
                    {
                        Check_Receiptprinter = true;
                        Constants.NoReceiptMode = false;
                    }
                    else
                    {
                        LedOperations.DeviceError("RECEIPT Printer");
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
                    bool isVisible = true;
                    if (StockOperations.coin5 <= Constants.NoChangeAvailable)
                    {
                        lbl5RS.Visibility = Visibility.Collapsed;
                        isVisible = false;
                    }
                    else
                    {
                        isVisible = true;
                    }

                    if (StockOperations.coin2 <= Constants.NoChangeAvailable)
                    {
                        lbl2RS.Visibility = Visibility.Collapsed;
                        isVisible = false;
                    }
                    else
                    {
                        isVisible = true;
                    }

                    if (StockOperations.coin1 <= Constants.NoChangeAvailable)
                    {
                        lbl1RS.Visibility = Visibility.Collapsed;
                        isVisible = false;
                    }
                    else
                    {
                        isVisible = true;
                    }

                    if (isVisible)
                        stackCoin.Visibility = Visibility.Visible;
                    else
                        stackCoin.Visibility = Visibility.Collapsed;

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

                LedOperations.GreenText("WELCOME TO " + Stations.currentStation.name + " " + PIDMessageLog.getMessage());

                timerOccConnMsg.Tick += timerOccConnMsg_Tick;
                timerOccConnMsg.Interval = TimeSpan.FromSeconds(1);
                timerOccConnMsg.Start();
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
