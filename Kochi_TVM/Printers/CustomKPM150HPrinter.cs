using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Printers
{
    class CustomKPM150HPrinter
    {
        private static ILog log = LogManager.GetLogger(typeof(CustomKPM150HPrinter).Name);

        Image Qrimg;

        private static CustomKPM150HPrinter _instance = null;
        public static CustomKPM150HPrinter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomKPM150HPrinter();
                return _instance;
            }
        }
        public CustomKPM150HPrinter()
        {

        }
        string PrinterName = "CUSTOM KPM150";
        public Enums.PRINTER_STATE getStatusWithUsb()
        {
            try
            {

                var server = new LocalPrintServer();

                PrintQueue queue = server.GetPrintQueue(PrinterName, new string[0] { });

                queue.Refresh();

                if (queue.IsOffline)
                    return Enums.PRINTER_STATE.ERROR;

                if (queue.IsOutOfPaper)
                    return Enums.PRINTER_STATE.NO_PAPER;

                if (queue.HasPaperProblem)
                    return Enums.PRINTER_STATE.LOW_PAPER;

                if (!queue.IsOffline)
                    return Enums.PRINTER_STATE.OK;

                if (!queue.IsOutOfPaper)
                    return Enums.PRINTER_STATE.OK;

                return Enums.PRINTER_STATE.OTHER;
            }
            catch (Exception ex)
            {
                return Enums.PRINTER_STATE.ERROR;
            }
        }
        public void PrintQRTicket(Bitmap imgPrint)//, TicketGrid ticketGrid)
        {
            try
            {
                //StartShiftPrint s = new StartShiftPrint();
                //s.AssetId = assetId;
                //s.StationName = stationName;
                //s.OperatorId = ticketNumber;
                //startShiftPrint = s;

                Qrimg = imgPrint;

                //ticketData = ticketGrid;

                PrintDocument Document = new PrintDocument();
                Document.PrintPage += new PrintPageEventHandler(printHandlerQRTicket);
                Document.PrinterSettings.PrinterName = PrinterName;
                Document.Print();


            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> PrintQRTicket() : " + ex.ToString());
            }
        }
        private void printHandlerQRTicket(object sender, PrintPageEventArgs e)
        {
            //StartShiftPrint s = startShiftPrint;
            try
            {
                string headerAddress = "Images\\kmrl_icon.png";

                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                Rectangle p = new Rectangle(70, 0, 150, 60);
                e.Graphics.DrawImage(img, p);
                Rectangle p1 = new Rectangle(70, 60, 150, 150);
                e.Graphics.DrawImage(Qrimg, p1);
            }
            catch
            {

            }

            try
            {

                e.Graphics.DrawLine(Pens.Black, 10, 200, 280, 200);

                Font headerFont = new Font("Calibri", 14);
                Font mediumFont = new Font("Calibri", 12);
                Font smallFont = new Font("Calibri", 10);

                StringFormat sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);


                sf.Alignment = StringAlignment.Center;
                //e.Graphics.DrawString("TICKET NUMBER : " + s.OperatorId, smallFont, Brushes.Black, new RectangleF(10, 210, 280, 15), sf);

                e.Graphics.DrawLine(Pens.Black, 10, 235, 280, 235);

                //string qrType = ticketData.QRType.ToString();
                //if (ticketData.QRType == Enums.QRType.GROUP)
                //    qrType = ticketData.QRType + " - " + ticketData.ticketsCount;

                //sf.Alignment = StringAlignment.Center;
                //e.Graphics.DrawString("TICKET TYPE: " + qrType, smallFont, Brushes.Black, new RectangleF(10, 240, 280, 15), sf);//10, 135, 210, 15

                //sf.Alignment = StringAlignment.Center;
                //string travelString = string.Format("{0} TO {1}", ticketData.From, ticketData.To);
                //e.Graphics.DrawString(travelString, smallFont, Brushes.Black, new RectangleF(10, 260, 280, 15), sf);

                //sf.Alignment = StringAlignment.Center;
                //e.Graphics.DrawString("DATE : " + DateTime.Now.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture), smallFont, Brushes.Black, new RectangleF(10, 280, 280, 15), sf);

                //sf.Alignment = StringAlignment.Center;
                //e.Graphics.DrawString("TIME : " + DateTime.Now.ToString("HH:mm", CultureInfo.InvariantCulture), smallFont, Brushes.Black, new RectangleF(10, 300, 280, 15), sf);

                //sf.Alignment = StringAlignment.Center;
                //e.Graphics.DrawString("ASSET ID : " + s.AssetId, smallFont, Brushes.Black, new RectangleF(10, 320, 280, 15), sf);

                //sf.Alignment = StringAlignment.Center;
                //if (ticketData.QRType == Enums.QRType.GROUP)
                //{
                //    decimal ticketFare = decimal.Round(Convert.ToDecimal(ticketData.ticketPrice), 2);
                //    e.Graphics.DrawString("FARE : INR " + ticketFare, smallFont, Brushes.Black, new RectangleF(10, 340, 280, 15), sf);
                //}
                //else
                //{
                //    decimal ticketFare = decimal.Round(ticketData.baseFare, 2);
                //    e.Graphics.DrawString("FARE : INR " + ticketFare, smallFont, Brushes.Black, new RectangleF(10, 340, 280, 15), sf);
                //}

                //sf.Alignment = StringAlignment.Center;
                //DateTime validDate = DateTime.Now.AddMinutes(ticketData.validityTime);
                //e.Graphics.DrawString("VALID UPTO : " + validDate.ToString("HH:mm/dd-MM-yyyy", CultureInfo.InvariantCulture), smallFont, Brushes.Black, new RectangleF(20, 360, 280, 15), sf);


                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("PLEASE DO NOT FOLD THE QR CODE", smallFont, Brushes.Black, new RectangleF(20, 380, 280, 15), sf);

                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("     ", smallFont, Brushes.Black, new RectangleF(20, 475, 280, 15), sf);

            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> printHandlerQRTicket() : " + ex.ToString());
            }
        }
        public void PrintTransactionReceipt()
        {
            try
            {
                PrintDocument Document = new PrintDocument();
                Document.PrintPage += new PrintPageEventHandler(printHandlerTransaction);
                Document.PrinterSettings.PrinterName = PrinterName;
                Document.Print();
            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> PrintTransactionReceipt() : " + ex.ToString());
            }
        }
        private void printHandlerTransaction(object sender, PrintPageEventArgs e)
        {
            try
            {
                string headerAddress = "Images\\kmrl_icon.png";

                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                Rectangle p = new Rectangle(70, 0, 150, 60);
                e.Graphics.DrawImage(img, p);
            }
            catch
            {

            }

            try
            {

                Font headerFont = new Font("Courier Prime", 14);
                Font mediumFont = new Font("Courier Prime", 12);
                Font smallFont = new Font("Courier Prime", 7);

                StringFormat sf = new StringFormat(StringFormatFlags.DirectionRightToLeft);

                e.Graphics.DrawLine(Pens.Black, 5, 70, 290, 70);
                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("TRANSACTION RECEIPT", headerFont, Brushes.Black, new RectangleF(5, 80, 280, 20), sf);

                sf.Alignment = StringAlignment.Center;
                e.Graphics.DrawString("TRANSACTION NUMBER \r\n" + 23111, smallFont, Brushes.Black, new RectangleF(1, 100, 280, 20), sf);

                e.Graphics.DrawLine(Pens.Black, 10, 133, 290, 133);

                smallFont = new Font("Calibri", 8);
                sf.Alignment = StringAlignment.Far;
                e.Graphics.DrawString("TIME : " + DateTime.Now.ToString("HH:mm:ss tt", CultureInfo.InvariantCulture), smallFont, Brushes.Black, new RectangleF(5, 140, 280, 15), sf);


                sf.Alignment = StringAlignment.Far;
                e.Graphics.DrawString("DATE : " + DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture), smallFont, Brushes.Black, new RectangleF(5, 160, 280, 15), sf);

                smallFont = new Font("Courier Prime", 7);

                Pen blackPen = new Pen(Color.Black);
                // Set the DashCap to round.
                blackPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;

                // Create a custom dash pattern.
                blackPen.DashPattern = new float[] { 4.0F, 1.0F, 1.0F, 4.0F };

                // Draw a line.
                e.Graphics.DrawLine(blackPen, 5, 180, 290, 180);

                smallFont = new Font("Courier Prime", 7, FontStyle.Bold);
                sf.Alignment = StringAlignment.Far;
                e.Graphics.DrawString("TYPE       FARE        TAX         C        QTY       SUB.TOT ", smallFont, Brushes.Black, new RectangleF(5, 200, 280, 15), sf);

                e.Graphics.DrawLine(blackPen, 5, 220, 290, 220);
                smallFont = new Font("Courier Prime", 7);

                int i = 1;

                string s = "";
                string type = "";
                string baseFare = "";
                string tax = "";
                string cf = "";
                string quantity = "";
                string subTotal = "";
                decimal total = 0;

                e.Graphics.DrawString(s, smallFont, Brushes.Black, new RectangleF(5, 225, 280, 25 * i), sf);

                e.Graphics.DrawLine(blackPen, 5, 260 + (15 * i), 290, 260 + (15 * i));


                sf.Alignment = StringAlignment.Near;
                e.Graphics.DrawString("TOTAL : " + "INR " + Math.Round(total, 2), headerFont, Brushes.Black, new RectangleF(5, 270 + (15 * i), 280, 25), sf);

                e.Graphics.DrawLine(blackPen, 5, 290 + (15 * i), 290, 290 + (15 * i));

            }
            catch (Exception ex)
            {
                log.Error("Error CustomKPM150HPrinter -> printHandlerTransaction() : " + ex.ToString());
            }
        }
    }
}
