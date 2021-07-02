﻿using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.Logs;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.PID;
using Kochi_TVM.Printers;
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

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for PayByCashPage.xaml
    /// </summary>
    public partial class OrderPreviewPage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(OrderPreviewPage).Name);
        public OrderPreviewPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LedOperations.GreenText("PAY USING CASH");
                lblInfo.Content = MultiLanguage.GetText("startPaymentProc");
                btnBack.Content = MultiLanguage.GetText("back");
                btnFinish.Content = MultiLanguage.GetText("cancel");
                btnCash.Content = MultiLanguage.GetText("cash");
                PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
                if (ReceiptPrinter == PRINTER_STATE.OK)
                {
                    Constants.NoReceiptMode = false;
                }
                else
                {
                    Constants.NoReceiptMode = true;
                }

                var result = Parameters.TVMDynamic.GetAfcConnStatus();
                if (!result)
                {
                    ElectronicJournal.OrderCancelled();
                    Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("Communication", "LAN communication Error.", MessageBoxButtonSet.OK);
                    if (messageBoxResult == Custom.MessageBoxResult.OK)
                        NavigationService.Navigate(new Pages.MainPage());
                    return;
                }

                PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();//CustomKPM150HPrinter.Instance.getStatusWithUsb();
                if (QRStatus != PRINTER_STATE.OK)
                {
                    ElectronicJournal.OrderCancelled();
                    Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("QR Printer", "QR Printer Error.", MessageBoxButtonSet.OK);
                    if (messageBoxResult == Custom.MessageBoxResult.OK)
                        NavigationService.Navigate(new Pages.MainPage());
                    return;
                }
                else
                {
                    if (StockOperations.qrSlip > Ticket.ticketCount || StockOperations.qrSlip > Ticket.peopleCount)
                    {
                        Dispatcher.BeginInvoke(new Action(() =>
                        {
                            try
                            {
                                if (Ticket.PrepareTicket())
                                {
                                    DefaultTicketInfo();
                                    ArrangTicketInfo();
                                    btnCash.Visibility = Visibility.Visible;
                                    Message();
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }), DispatcherPriority.Background);
                    }
                    else
                    {
                        ElectronicJournal.OrderCancelled();
                        Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("QR Printer", "QR Printer Paper Less.", MessageBoxButtonSet.OK);
                        if (messageBoxResult == Custom.MessageBoxResult.OK)
                            NavigationService.Navigate(new Pages.MainPage());
                        return;
                    }
                }
                BNRManager.Instance.PollingAction();
            }
            catch (Exception ex)
            {
                log.Error("Error OrderPreviewPage -> Page_Loaded() : " + ex.ToString());
            }
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(6, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(6, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                TVMUtility.PlayVoice(6, null, Convert.ToString(Convert.ToInt16(Ticket.totalPrice)), "IN");
            }
        }
        private bool ArrangTicketInfo()
        {
            bool result = false;
            try
            {
                switch (Ticket.journeyType)
                {
                    case JourneyType.Topup:
                        lblLine1Key.Visibility = Visibility.Visible;
                        lblLine2Key.Visibility = Visibility.Visible; lblLine2Value.Visibility = Visibility.Visible;

                        lblLine1Key.Content = Ticket.journeyTypeText;
                        lblLine2Key.Content = MultiLanguage.GetText("amount"); lblLine2Value.Content = Conversion.MoneyFormat(TransactionInfo.amount);

                        break;
                    case JourneyType.Day_Pass:
                    case JourneyType.Weekend_Pass:
                        lblLine1Key.Visibility = Visibility.Visible;
                        lblLine2Key.Visibility = Visibility.Visible; lblLine2Value.Visibility = Visibility.Visible;
                        lblLine3Key.Visibility = Visibility.Visible; lblLine3Value.Visibility = Visibility.Visible;
                        lblLine4Key.Visibility = Visibility.Visible; lblLine4Value.Visibility = Visibility.Visible;

                        lblLine1Key.Content = Ticket.journeyTypeText;
                        lblLine2Key.Content = MultiLanguage.GetText("ticketType"); lblLine2Value.Content = Ticket.ticketTypeText;
                        lblLine3Key.Content = MultiLanguage.GetText("numberOfTickets"); lblLine3Value.Content = Ticket.ticketCount;
                        lblLine4Key.Content = MultiLanguage.GetText("amount"); lblLine4Value.Content = Conversion.MoneyFormat(Ticket.totalPrice);

                        break;
                    case JourneyType.Period_Pass:
                    case JourneyType.Trip_Pass:
                        lblLine1Key.Visibility = Visibility.Visible;
                        lblLine2Key.Visibility = Visibility.Visible; lblLine2Value.Visibility = Visibility.Visible;
                        lblLine3Key.Visibility = Visibility.Visible; lblLine3Value.Visibility = Visibility.Visible;
                        lblLine4Key.Visibility = Visibility.Visible; lblLine4Value.Visibility = Visibility.Visible;
                        lblLine5Key.Visibility = Visibility.Visible; lblLine5Value.Visibility = Visibility.Visible;
                        lblLine6Key.Visibility = Visibility.Visible; lblLine6Value.Visibility = Visibility.Visible;

                        lblLine1Key.Content = Ticket.journeyTypeText;
                        lblLine2Key.Content = "Origin Station :"; lblLine2Value.Content = MultiLanguage.GetText(Ticket.startStation.name);
                        lblLine3Key.Content = MultiLanguage.GetText("destination"); lblLine3Value.Content = Ticket.endStation.name;
                        lblLine4Key.Content = "Number Of Pass : "; lblLine4Value.Content = Ticket.passCount;
                        lblLine5Key.Content = "Validity : "; lblLine5Value.Content = Ticket.ticketExpiryDts.ToString("g");
                        lblLine6Key.Content = MultiLanguage.GetText("amount"); lblLine6Value.Content = Conversion.MoneyFormat(Ticket.totalPrice);
                        break;
                    case JourneyType.Group_Ticket:
                        lblLine1Key.Visibility = Visibility.Visible;
                        lblLine2Key.Visibility = Visibility.Visible; lblLine2Value.Visibility = Visibility.Visible;
                        lblLine3Key.Visibility = Visibility.Visible; lblLine3Value.Visibility = Visibility.Visible;
                        lblLine4Key.Visibility = Visibility.Visible; lblLine4Value.Visibility = Visibility.Visible;
                        lblLine5Key.Visibility = Visibility.Visible; lblLine5Value.Visibility = Visibility.Visible;

                        lblLine1Key.Content = Ticket.journeyTypeText;
                        lblLine2Key.Content = MultiLanguage.GetText("destination"); lblLine2Value.Content = MultiLanguage.GetText(Ticket.endStation.name);
                        lblLine3Key.Content = MultiLanguage.GetText("ticketType"); lblLine3Value.Content = Ticket.ticketTypeText;
                        lblLine4Key.Content = "Number Of Passenger : "; lblLine4Value.Content = Ticket.peopleCount;
                        lblLine5Key.Content = MultiLanguage.GetText("amount"); lblLine5Value.Content = Conversion.MoneyFormat(Ticket.totalPrice);

                        break;
                    case JourneyType.RJT:
                    case JourneyType.SJT:                       
                        lblLine1Key.Content = Ticket.journeyTypeText;
                        lblLine2Key.Content = MultiLanguage.GetText("destination"); lblLine2Value.Content = MultiLanguage.GetText(Stations.GetStation(Ticket.endStation.id).name);
                        //lblLine3Key.Content = "Ticket Type :"; lblLine3Value.Content = Ticket.ticketTypeText;
                        lblLine3Key.Content = MultiLanguage.GetText("numberOfTickets"); lblLine3Value.Content = Ticket.ticketCount;
                        lblLine4Key.Content = MultiLanguage.GetText("amount"); lblLine4Value.Content = Conversion.MoneyFormat(Ticket.totalPrice);
                        Constants.EnableBillNotes = TVMUtility.EnableBill(Ticket.totalPrice);
                        break;

                    default:
                        break;
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }
        private void DefaultTicketInfo()
        {
            try
            {
                lblLine5Key.Visibility = Visibility.Hidden; lblLine5Value.Visibility = Visibility.Hidden;
                lblLine6Key.Visibility = Visibility.Hidden; lblLine6Value.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                log.Error("Error DefaultTicketInfo -> Page_Loaded() : " + ex.ToString());
            }
        }
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnCash_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            PRINTER_STATE ReceiptPrinter = CustomTL60Printer.Instance.getStatusWithUsb();
            if (ReceiptPrinter == PRINTER_STATE.OK)
            {
                Constants.NoReceiptMode = false;
            }
            else
            {
                Constants.NoReceiptMode = true;
            }
            PRINTER_STATE QRStatus = QRPrinter.Instance.CheckQrPrinterStatus();//CustomKPM150HPrinter.Instance.getStatusWithUsb();
            if (QRStatus != PRINTER_STATE.OK)
            {
                Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("QR Printer", "QR Printer Error.", MessageBoxButtonSet.OK);
                if (messageBoxResult == Custom.MessageBoxResult.OK)
                {
                    ElectronicJournal.OrderCancelled();
                    NavigationService.Navigate(new Pages.MainPage());
                    return;
                }
            }
            else
            {
                if (StockOperations.qrSlip >= Ticket.ticketCount)
                {
                    ElectronicJournal.MediaSelected("CASH");
                    NavigationService.Navigate(new Pages.PayByCashPage());
                }
                else
                {
                    Custom.MessageBoxResult messageBoxResult = MessageBoxOperations.ShowMessage("QR Printer", "QR Printer Low Paper.", MessageBoxButtonSet.OK);
                    if (messageBoxResult == Custom.MessageBoxResult.OK)
                    {
                        ElectronicJournal.OrderCancelled();
                        NavigationService.Navigate(new Pages.MainPage());
                        return;
                    }
                }
            }
            
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            ElectronicJournal.OrderCancelled();
            NavigationService.Navigate(new Pages.MainPage());
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            TVMUtility.PlayClick();
            NavigationService.Navigate(new Pages.TicketCountPage());
        }
    }
}
