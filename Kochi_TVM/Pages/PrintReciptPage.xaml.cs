using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
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

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for PrintReciptPage.xaml
    /// </summary>
    public partial class PrintReciptPage : Page
    {
        string RecAmt;
        string ChaAmt;
        public PrintReciptPage(string NumberOfTicket, string ReceivedAmt, string ChangeAmt)
        {
            InitializeComponent();
            RecAmt = ReceivedAmt;
            ChaAmt = ChangeAmt;
            returnCashImageGif.Source = new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Images\giving_money.gif");
            lblTicketCount.Content = NumberOfTicket;
            lblChange.Content = ChangeAmt;
            Message();
        }
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(9, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(9, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(9, null, null, "IN");
            }
        }
        async void PrintReceipt()
        {
            //foreach (var selectedTickets in Ticket.listTickets)
            //{
            //    var qr = Utility.PrepareQRImage(selectedTickets.TicketGUID);
            //    CustomTL60Printer.Instance.PrintQRTicket(selectedTickets, qr);
            //}
            if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            {
                CustomTL60Printer.Instance.TicketReceipt(RecAmt, ChaAmt);
            }
            await Task.Delay(1000);
            PrintQR();
            //NavigationService.Navigate(new Pages.MainPage());
        }
        async void PrintQR()
        {
            LastMessage();
            //foreach (var selectedTickets in Ticket.listTickets)
            //{
            //    var qr = Utility.PrepareQRImage(selectedTickets.TicketGUID);
            //    CustomTL60Printer.Instance.PrintQRTicket(selectedTickets, qr);
            //}
            //if (CustomTL60Printer.Instance.getStatusWithUsb() == Enums.PRINTER_STATE.OK)
            //{
            //    var qr = Utility.PrepareQRImage(Ticket.startStation.description);
            //    //CustomTL60Printer.Instance.(qr);
            //}
            await Task.Delay(4000);
            NavigationService.Navigate(new Pages.MainPage());
        }

        void LastMessage()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(10, null, null, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(10, null, null, "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(10, null, null, "IN");
            }
        }
        private void returnCashImageGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            returnCashImageGif.Position = new TimeSpan(0, 0, 1);
            returnCashImageGif.Play();
        }

        private void btnPrintReciptSkip_Click(object sender, RoutedEventArgs e)
        {
            PrintQR();
        }

        private void btnPrintRecipt_Click(object sender, RoutedEventArgs e)
        {
            PrintReceipt();            
        }
    }
}
