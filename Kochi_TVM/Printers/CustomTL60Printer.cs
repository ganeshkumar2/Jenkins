﻿using Kochi_TVM.Business;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Models;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;
using TransactionType = Kochi_TVM.Utils.Enums.TransactionType;

namespace Kochi_TVM.Printers
{
    class CustomTL60Printer
    {
        private static ILog log = LogManager.GetLogger(typeof(CustomTL60Printer).Name);

        Image Qrimg;

        static int widthPoits = 144;

        static int lineGap = 18;
        static int space = 5;
        static int seperatorPoint = 120;
        static int lineY = 2;
        static string seperatorChar = ":";
        PrintDocument printDocument = null;
        List<PrintObject> printList = new List<PrintObject>();
        Graphics printGraphics = null;
        Font printFont = new Font("Arial", 8);
        public enum ContentType
        {
            Text = 0,
            Image = 1
        }
        private struct PrintObject
        {
            public StringAlignment align;
            public int startX;
            public int startY;
            public ContentType contentType;
            public string text;
            public Bitmap image;
        }
        private static CustomTL60Printer _instance = null;
        public static CustomTL60Printer Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new CustomTL60Printer();
                return _instance;
            }
        }
        public CustomTL60Printer()
        {

        }
        string PrinterName = "CUSTOM TL60";//"Microsoft Print to PDF";
        public Enums.PRINTER_STATE getStatusWithUsb()
        {
            try
            {
                string query = string.Format("SELECT * from Win32_Printer WHERE Name LIKE '%{0}'", PrinterName);
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                using (ManagementObjectCollection coll = searcher.Get())
                {
                    try
                    {
                        foreach (ManagementObject printer in coll)
                        {
                            foreach (PropertyData property in printer.Properties)
                            {
                                if (property.Name == "WorkOffline")
                                {
                                    if ((bool)property.Value)
                                    {
                                        return Enums.PRINTER_STATE.ERROR;
                                    }
                                    else
                                    {
                                        var server = new LocalPrintServer();

                                        PrintQueue queue = server.GetPrintQueue(PrinterName, new string[0] { });

                                        queue.Refresh();

                                        if (queue.IsOffline)
                                            return Enums.PRINTER_STATE.ERROR;

                                        if (queue.IsPaused)
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
                                }
                            }
                        }
                        return Enums.PRINTER_STATE.ERROR;
                    }
                    catch (ManagementException ex)
                    {
                        return Enums.PRINTER_STATE.ERROR;
                    }
                }
            }
            catch (Exception ex)
            {
                return Enums.PRINTER_STATE.ERROR;
            }
        }

        void AddPrintObject(int startX, int startY, ContentType contentType, string printText, Bitmap image, StringAlignment align)
        {
            PrintObject po = new PrintObject();
            po.startX = startX;
            po.startY = startY;
            po.contentType = contentType;
            po.text = printText;
            po.align = align;
            po.image = image;

            printList.Add(po);
        }
        public bool AddLine(int startX, int startY, string printText, StringAlignment align)
        {
            AddPrintObject(startX, startY, ContentType.Text, printText, null, align);
            return true;
        }
        public bool AddLine(int startX, int startY, string printText)
        {
            AddPrintObject(startX, startY, ContentType.Text, printText, null, StringAlignment.Far);
            return true;
        }
        public bool AddLine(int startX, int startY, Bitmap image)
        {
            AddPrintObject(startX, startY, ContentType.Image, String.Empty, image, StringAlignment.Far);
            return true;
        }
        public void AddText(string caption)
        {
            lineY += lineGap;
            AddLine(0, lineY, caption, System.Drawing.StringAlignment.Center);
        }
        public void AddText(string firstPart, string secondPart, int seperatorPoint)
        {
            lineY += lineGap;
            AddLine(space, lineY, firstPart);
            AddLine(seperatorPoint, lineY, seperatorChar);
            AddLine(space + seperatorPoint, lineY, secondPart);
        }
        public void AddImage(Bitmap image)
        {
            lineY += 10;
            AddLine(50, lineY, image);
            lineY += image.Height;
        }
        int AlignCenter(PrintObject po)
        {
            int startPosition = 0;
            int objectSize = 0;
            int pageSize = 150;

            if (po.contentType == ContentType.Text)
            {
                SizeF textSize = printGraphics.MeasureString(po.text, printFont);
                objectSize = (int)textSize.Width;
            }
            else if (po.contentType == ContentType.Image)
            {
                objectSize = po.image.Height;
            }

            startPosition = ((pageSize - objectSize) / 2);
            return startPosition;
        }      
        public void TestBNA()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);
            //logo.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);
            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);
            AddText("BNA Device Test Result");
            AddText("------------------------------------------------------------------------------");
            AddText(Constants.IsBNRAvalable == true ? "Succesfull" : "Failed. Check Connection");
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void TestHopper()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);
            //logo.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);
            AddText("Hopper Board Test Result");
            AddText("------------------------------------------------------------------------------");
            AddText(CCTalkManager.Instance.coinHopperEV4000_1.Manufacture != null ? "Succesfull" : "Failed. Check Connection");
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void TestReceiptPrinter()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);
            //logo.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);
            AddText("ReceiptPrinter Test Result");
            AddText("------------------------------------------------------------------------------");

            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void TicketReceipt(string ReceivedAmt, string ChangeAmt)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("KOCHI METRO RAIL LTD");
            AddText("GSTIN32AAECK5274H1ZL");
            AddText("SALE");
            AddText("------------------------------------------------------------------------------");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("Media Type", "QR", 80);
            AddText("PaymentType", "CASH", 80);

            //daypass and weekend pass is usable all station
            if (Ticket.journeyType == JourneyType.Day_Pass || Ticket.journeyType == JourneyType.Weekend_Pass)
            {
                AddText("Activate", Ticket.ticketActivateDts.ToString("yyyy-MM-dd HH:mm"), 80);
                AddText("Expiry", Ticket.ticketExpiryDts.ToString("yyyy-MM-dd HH:mm"), 80);
            }
            //else
            //{
            //    PrinterFunctions.AddText("From", Ticket.startStation.name);
            //    PrinterFunctions.AddText("To", Ticket.endStation.name);
            //}

            //if (Ticket.journeyType == JourneyType.Group_Ticket)
            //    PrinterFunctions.AddText("Count", Ticket.peopleCount.ToString());

            AddText("Amaount Due", "Rs. " + Ticket.totalPrice.ToString(), 80);
            AddText("Amount Paid", "Rs. " + ReceivedAmt, 80);
            AddText("Balance", "Rs. " + ChangeAmt, 80);
            AddText("GST Percentage", "0%", 80);
            AddText("GST Amount", "Rs. 0.00", 80);
            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void TVMInfoReceipt()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("TVM Info");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("------------------------------------------------------------------------------");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Equipment ID", Parameters.TVMDynamic.GetParameter("sys_EquipmentId"), 80);
            AddText("App Version", Parameters.TVMStatic.GetParameter("appVersion"), 80);
            AddText("Parameter Version", Parameters.TVMDynamic.GetParameter("sys_CCVersion"), 105);
            AddText("Last Sync Date", Parameters.lastSync.ToString(), 105);
            AddText("Central Computer", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect", 105);
            AddText("OCC", Parameters.TVMDynamic.GetParameter("AfcConn") == "1" ? "Connect" : "Disconnect", 105);
            AddText("Station Computer", Parameters.TVMDynamic.GetParameter("SCConn") == "1" ? "Connect" : "Disconnect", 105);
            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void TVMDeviceInfoReceipt()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);
            //logo.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipY);

            AddText("KOCHI METRO");
            AddText("TVM Devices Info");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("------------------------------------------------------------------------------");
            AddText("Receipt Printer", getStatusWithUsb() == PRINTER_STATE.OK ? "OK" : "ERROR(Not Connected)", 110);
            AddText(string.Format("QR Printer: {0}", Parameters.TVMStatic.GetParameter("qrcPrinterStatus") == "1" ? "OK" : "ERROR(Not Connected)"));
            //AddText("QR Printer", PrintOperations.isInitQRPrinter == true ? "OK" : "Not Connected", 110);
            AddText("Banknote Acceptor", Parameters.TVMStatic.GetParameter("bnaStatus") == "1" ? "OK" : "Not Connected", 110);
            AddText("RPT Dispenser", Parameters.TVMStatic.GetParameter("rptDispenserStatus") == "1" ? "OK" : "Not Connected", 110);
            AddText("EMV POS", "Not Connected", 110);
            AddText("Card POS", "Not Connected", 110);
            AddText("Hopper 5 Rs.", CCTalkManager.Instance.coinHopperEV4000_3.Manufacture != null ? "OK" : "Not Connected)", 110);
            AddText("Hopper 2 Rs.", CCTalkManager.Instance.coinHopperEV4000_2.Manufacture != null ? "OK" : "Not Connected)", 110);
            AddText("Hopper 1 Rs.", CCTalkManager.Instance.coinHopperEV4000_1.Manufacture != null ? "OK" : "Not Connected)", 110);
            //PrinterFunctions.AddText(string.Format("Hopper 5 Rs.: {0}", Devices.isInitHopper5 == true ? "OK" : "ERROR(Not Connected)"));
            //PrinterFunctions.AddText(string.Format("Hopper 2 Rs.: {0}", Devices.isInitHopper2 == true ? "OK" : "ERROR(Not Connected)"));
            //PrinterFunctions.AddText(string.Format("Hopper 1 Rs.: {0}", Devices.isInitHopper1 == true ? "OK" : "ERROR(Not Connected)"));

            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void CollectionReceipt()
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("TVM Collection Info");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Card No", "1234567898765432", 80);

            AddText("");
            AddText("------------------------------------------------------------------------------");
            AddText("Operations");

            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("Slip Operation");
            //PrinterFunctions.AddText("Receipt Slip", "Change");
            //PrinterFunctions.AddText("QR Slip", "Change");

            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("RPT Operation");
            //PrinterFunctions.AddText("RPT Add Count", "140");
            //PrinterFunctions.AddText("RPT Less Count", "0");

            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("Banknote Operation");
            //PrinterFunctions.AddText("Banknote Box", "Change");
            //PrinterFunctions.AddText("Box Count", "152");
            //PrinterFunctions.AddText("Box Amount", "4820 Rs.");
            //PrinterFunctions.AddText("20 Rs. Add Count", "15");
            //PrinterFunctions.AddText("50 Rs. Add Count", "3");

            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("Coin Operation");
            //PrinterFunctions.AddText("10 Rs. Add Count", "45");
            //PrinterFunctions.AddText("5 Rs. Add Count", "0");
            //PrinterFunctions.AddText("1 Rs. Add Count", "0");
            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("");

            //PrinterFunctions.AddText("------------------------------------------------------------------------------");
            //PrinterFunctions.AddText("Current Values");
            //PrinterFunctions.AddText("RPT Count", "250");
            //PrinterFunctions.AddText("Box Count", "0");
            //PrinterFunctions.AddText("20 Rs. Count", "15");
            //PrinterFunctions.AddText("100 Rs. Count", "3");
            //PrinterFunctions.AddText("5 Rs. Count", "200");
            //PrinterFunctions.AddText("2 Rs. Count", "0");
            //PrinterFunctions.AddText("1 Rs. Count", "0");
            //PrinterFunctions.AddText("");
            //PrinterFunctions.AddText("");

            //PrinterFunctions.AddText("------------------------------------------------------------------------------");
            //PrinterFunctions.AddText("Before Values");
            //PrinterFunctions.AddText("RPT Count", "110");
            //PrinterFunctions.AddText("Box Count", "152");
            //PrinterFunctions.AddText("20 Rs. Count", "9");
            //PrinterFunctions.AddText("50 Rs. Count", "5");
            //PrinterFunctions.AddText("10 Rs. Count", "155");
            //PrinterFunctions.AddText("5 Rs. Count", "0");
            //PrinterFunctions.AddText("1 Rs. Count", "0");
            //PrinterFunctions.AddText("");

            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void StandAloneReceipt()
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


            try
            {

                DateTime startDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"));
                DateTime endDate = DateTime.Parse(Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"));

                using (var context = new TVM_Entities())
                {
                   var trxData = context.sp_SelShiftPaymentReport(Convert.ToInt32(Parameters.TVMDynamic.GetParameter("unitId")), Stations.currentStation.id, 0,0, startDate, endDate).ToList();
                    foreach(var data in trxData)
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

                lineY = 2;
                printList = new List<PrintObject>();
                string headerAddress = "Images\\kmrl_icon.png";
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
                logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
                AddImage(logo);

                AddText("KOCHI METRO");

                AddText("Daily Transaction Report");
                AddText("Date/Time", DateTime.Now.ToString("yyyy-MM-dd HH:mm"), 80);
                AddText("Start Time", Parameters.TVMDynamic.GetParameter("sys_WorkHoursStart"), 80);
                AddText("End Time", Parameters.TVMDynamic.GetParameter("sys_WorkHoursEnd"), 80);
                AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
                AddText("Station", Stations.currentStation.name, 80);
                AddText("User", Parameters.userId, 80);

                AddText("------------------------------------------------------------------------------");
                AddText("--Transactions--");
                AddText("");
                AddText("QR Sale(SJT Cash)");
                AddText("Count", QRSJTCashCount.ToString(), 80);
                AddText("Amount", QRSJTCashAmount.ToString() + "Rs.", 80);
                AddText("QR Sale(RJT Cash)");
                AddText("Count", QRRJTCashCount.ToString(), 80);
                AddText("Amount", QRRJTCashAmount.ToString() + "Rs.", 80);
                //PrinterFunctions.AddText("QR Sale(SJT Non-Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", QRSJTNonCashCount));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", QRSJTNonCashAmount)); 
                //PrinterFunctions.AddText("QR Sale(RJT Non-Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", QRRJTNonCashCount));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", QRRJTNonCashAmount)); 

                //PrinterFunctions.AddText("RPT Sale(SJT Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", RPTSJTCashCount));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", RPTSJTCashAmount)); 
                //PrinterFunctions.AddText("RPT Sale(RJT Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", 0));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", 0));
                //PrinterFunctions.AddText("RPT Sale(SJT Non-Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", RPTSJTNonCashCount));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", RPTSJTNonCashAmount)); ;
                //PrinterFunctions.AddText("RPT Sale(RJT Non-Cash)");
                //PrinterFunctions.AddText(string.Format("Count: {0}", 0));
                //PrinterFunctions.AddText(string.Format("Amount: {0} Rs", 0));
                //PrinterFunctions.AddText(" ");

                AddText("Cash Amount", (QRRJTCashAmount + QRSJTCashAmount).ToString() + "Rs" /*+ RPTSJTCashAmount*/, 80);
                //PrinterFunctions.AddText(string.Format("Non-Cash Amount: {0} Rs.", QRRJTNonCashAmount + QRSJTNonCashAmount + RPTSJTNonCashAmount));
                AddText("Total Amount", (QRRJTCashAmount + QRSJTCashAmount).ToString() + "Rs" /*+ RPTSJTCashAmount +
                                                                                    + QRRJTNonCashAmount + QRSJTNonCashAmount + RPTSJTNonCashAmount*/, 80);

                //PrinterFunctions.AddText("--Transactions--");
                //PrinterFunctions.AddText("QR Count: 321");
                //PrinterFunctions.AddText("QR Amount: 4330 Rs.");
                //PrinterFunctions.AddText("RPT Count: 0");
                //PrinterFunctions.AddText("RPT Amount: 0 Rs.");
                //PrinterFunctions.AddText("Total Count: 321");
                //PrinterFunctions.AddText("Total Amount: 4330 Rs.");
                //PrinterFunctions.AddText("");

                //PrinterFunctions.AddText("--Slip Informations--");
                //PrinterFunctions.AddText("QR Slip Sold Count: 321");
                //PrinterFunctions.AddText("QR Slip Replenish: 420");

                //PrinterFunctions.AddText("");
                //PrinterFunctions.AddText("--RPT Informations--");
                //PrinterFunctions.AddText("RPT Sold Count: 0");
                //PrinterFunctions.AddText("RPT Replenish: 50");

                //PrinterFunctions.AddText("");

                //PrinterFunctions.AddText("--Banknote Informations--");
                //PrinterFunctions.AddText("20 Rs. Amount: 200 Rs.");
                //PrinterFunctions.AddText("20 Rs. Replenish: 19");
                //PrinterFunctions.AddText("10 Rs. Amount: 70 Rs.");
                //PrinterFunctions.AddText("10 Rs. Replenish: 19");

                //PrinterFunctions.AddText("");
                //PrinterFunctions.AddText("--Coin Informations--");
                //PrinterFunctions.AddText("5 Rs. Amount: 500 Rs.");
                //PrinterFunctions.AddText("5 Rs. Replenish: 100");
                //PrinterFunctions.AddText("2 Rs. Amount: 200 Rs.");
                //PrinterFunctions.AddText("2 Rs. Replenish: 100");
                //PrinterFunctions.AddText("1 Rs. Amount: 100 Rs.");
                //PrinterFunctions.AddText("1 Rs. Replenish: 100");
                //PrinterFunctions.AddText("");
                //PrinterFunctions.AddText("");


                PrintDocument Document1 = new PrintDocument();
                PrintController printController = new StandardPrintController();
                Document1.PrintController = printController;
                Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
                Document1.PrinterSettings.PrinterName = PrinterName;
                Document1.Print();

            }
            catch (Exception ex)
            {
                //Log.log.Write("Error : " + ex.ToString());
            }
        }
        public void CoinAddPrint(int count, int coin, int stock)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Coin Replenish--");
            AddText("------------------------------------------------------------------------------");
            AddText("Added Coin", coin.ToString() + "Rs.", 80);
            AddText("Added Count", count.ToString(), 80);
            AddText("Added Amount", (count * coin).ToString() + "Rs.", 80);
            AddText("Total Count", stock.ToString(), 80);
            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void CoinDispatchPrint(int count, int coin, int stock)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("sys_EquipmentId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Coin Removing--");
            AddText("------------------------------------------------------------------------------");
            AddText(string.Format("Hopper: Hopper{0}.", coin));
            AddText("Removed Coin", coin.ToString() + "Rs.", 110);
            AddText("Removed Count", count.ToString() + "Rs.", 110);
            AddText("Removed Amount", (count * coin).ToString() + "Rs.", 110);
            AddText("Total Count", stock.ToString(), 110);
            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void CoinEmptyBoxPrint(int count, int coin, int stock)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            logo = new System.Drawing.Bitmap(logo, new System.Drawing.Size(120, 49));
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Hopper Info--");
            AddText("------------------------------------------------------------------------------");
            AddText(string.Format("Hopper: {0} Rs.", coin));
            AddText("Removed Count", count.ToString(), 100);
            AddText("Removed Amount", (count * coin).ToString() + "Rs.", 100);
            //AddText(string.Format("Hopper: {0} Rs.", coin1));
            //AddText("Removed Count: {0}.", count1.ToString(), 100);
            //AddText("Removed Amount: {0} Rs.", (count1 * coin1).ToString() + "Rs.", 100);
            //AddText(string.Format("Hopper: {0} Rs.", coin2));
            //AddText("Removed Amount", (count2 * coin2).ToString() + "Rs.", 100);
            //AddText("Removed Count", count2.ToString(), 100);
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void StockStatusReport(int coin1, int coin2, int coin5, int qr, int rpt, int receipt,
            int banknote10, int banknote20, int bankescrow, int billval1, int billval2, int billval3, int box)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--STOCKS--");
            AddText("------------------------------------------------------------------------------");
            AddText("QR Slip Count", qr.ToString(), 120);
            //PrinterFunctions.AddText("Receipt Slip Count", receipt.ToString(), 120);
            //PrinterFunctions.AddText("RPT Count", rpt.ToString(), 120);

            AddText(Constants.HopperAddress1Coin + " Rs. Coin Count", coin1.ToString(), 120);
            AddText(Constants.HopperAddress2Coin+" Rs. Coin Count", coin2.ToString(), 120);
            AddText(Constants.HopperAddress3Coin+" Rs. Coin Count", coin5.ToString(), 120);
            AddText("Hoppers Amount", ((coin1 * Constants.HopperAddress1Coin) + (coin2 * Constants.HopperAddress2Coin) + (coin5 * Constants.HopperAddress3Coin)).ToString() + " Rs.", 120);

            AddText(billval1+" Rs. Banknote Count", banknote10.ToString(), 130);
            AddText(billval2+" Rs. Banknote Count", banknote20.ToString(), 130);
            AddText(billval3+" Rs. Banknote Count", bankescrow.ToString(), 130);
            AddText("Box Amount", box.ToString() + " Rs.", 130);
            AddText("Banknotes Amount", ((banknote10 * billval1) + (banknote20 * billval2) + (bankescrow * billval3) + box).ToString() + " Rs.", 130);


            AddText("Grand Total", ((coin1 * Constants.HopperAddress1Coin) + (coin2 * Constants.HopperAddress2Coin) + (coin5 * Constants.HopperAddress3Coin) + (banknote10 * billval1) + (banknote20 * billval2) + (bankescrow * billval3) + box).ToString() + " Rs.", 130);

            AddText("------------------------------------------------------------------------------");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void AddPrintQRRPT(int count, TransactionType type, int stock)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            switch (type)
            {
                case TransactionType.TT_QR:

                    AddText("--QR Slip Replenish--");
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    AddText("Added Type", "QR Slip", 100);
                    AddText("Added Count", count.ToString(), 100);
                    AddText("Total Count", stock.ToString(), 100);
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                case TransactionType.TT_RPT:

                    AddText("--RPT Replenish--");
                    AddText("------------------------------------------------------------------------------");
                    AddText("Added Type", "RPT", 100);
                    AddText("Added Count", count.ToString(), 100);
                    AddText("Total Count", stock.ToString(), 100);
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                default:
                    break;
            }

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void DispatchQRRPT(int count, TransactionType type, int stock)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            switch (type)
            {
                case TransactionType.TT_QR:

                    AddText("--QR Slip Removing--");
                    AddText("------------------------------------------------------------------------------");
                    AddText("Removed Type", "QR Slip", 100);
                    AddText("Removed Count", count.ToString(), 100);
                    AddText("Total Count", stock.ToString(), 100);
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                case TransactionType.TT_RPT:

                    AddText("--RPT Removing--");
                    AddText("------------------------------------------------------------------------------");
                    AddText("Removed Type", "RPT", 100);
                    AddText("Removed Count", count.ToString(), 100);
                    AddText("Total Count", stock.ToString(), 100);
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                default:
                    break;
            }

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void EmptyQRRPT(int count, TransactionType type)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            switch (type)
            {
                case TransactionType.TT_QR:
                    AddText("--QR Slip Empty--");
                    AddText("");
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    AddText("Removed Type", "QR Slip", 100);
                    AddText("Removed Count", count.ToString(), 100);
                    AddText("");
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                case TransactionType.TT_RPT:
                    AddText("--RPT Empty--");
                    AddText("------------------------------------------------------------------------------");
                    AddText("Removed Type", "RPT", 100);
                    AddText("Removed Count", count.ToString(), 100);
                    AddText("------------------------------------------------------------------------------");
                    AddText("");
                    break;
                default:
                    break;
            }

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void SendBoxNotes(int count, int billType)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Send Box--");
            AddText("------------------------------------------------------------------------------");
            AddText("Sent Bill", billType + " Rs.", 110);
            AddText("Sent Count", count.ToString(), 110);
            AddText("Sent Amount", count * billType + " Rs.", 110);
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void RemoveCashBoxNotes(int amount, int count)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Cash Box Clear--");
            AddText("------------------------------------------------------------------------------");
            AddText("Removed Count", count + " Rs.", 110);
            AddText("Removed Amount", amount + " Rs.", 110);
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        public void AddBanknotes(int count1, int notes1,int count2, int notes2, int count3, int notes3)
        {
            lineY = 2;
            printList = new List<PrintObject>();
            string headerAddress = "Images\\kmrl_icon.png";
            Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
            System.Drawing.Bitmap logo = new System.Drawing.Bitmap(img);
            AddImage(logo);

            AddText("KOCHI METRO");
            AddText("Date/Time", Ticket.transactionDts.ToString("yyyy-MM-dd HH:mm"), 80);
            AddText("TVM ID", Parameters.TVMDynamic.GetParameter("unitId"), 80);
            AddText("Station", Stations.currentStation.name, 80);
            AddText("User", Parameters.userId, 80);

            AddText("--Banknote Replenish--");
            AddText("------------------------------------------------------------------------------");
            AddText(string.Format("Added Notes: {0} Rs.", notes1));
            AddText("Added Count", count1.ToString(), 80);
            AddText("Added Amount", (count1 * notes1).ToString() + "Rs.", 80);
            AddText("------------------------------------------------------------------------------");
            AddText(string.Format("Added Notes: {0} Rs.", notes2));
            AddText("Added Count", count2.ToString(), 80);
            AddText("Added Amount", (count2 * notes2).ToString() + "Rs.", 80);
            AddText("------------------------------------------------------------------------------");
            AddText(string.Format("Added Notes: {0} Rs.", notes3));
            AddText("Added Count", count3.ToString(), 80);
            AddText("Added Amount", (count3 * notes3).ToString() + "Rs.", 80);
            AddText("------------------------------------------------------------------------------");
            AddText("Total Count", (count1 + count2 + notes3).ToString(), 80);
            AddText("------------------------------------------------------------------------------");
            AddText("");

            PrintDocument Document1 = new PrintDocument();
            PrintController printController = new StandardPrintController();
            Document1.PrintController = printController;
            Document1.PrintPage += new PrintPageEventHandler(printDocumentPrintPage);
            Document1.PrinterSettings.PrinterName = PrinterName;
            Document1.Print();
        }
        void printDocumentPrintPage(object sender, PrintPageEventArgs e)
        {
            printGraphics = e.Graphics;
            PrintObject po;

            for (int i = 0; i < printList.Count; i++)
            {
                po = printList[i];

                if (po.align == StringAlignment.Center)
                {
                    po.startX = AlignCenter(po);
                }

                if (po.contentType == ContentType.Text)
                {
                    printGraphics.DrawString(po.text, printFont, new SolidBrush(System.Drawing.Color.Black), po.startX, po.startY);
                }
                else if (po.contentType == ContentType.Image)
                {
                    printGraphics.DrawImage(po.image, po.startX, po.startY);
                }
            }
        }       
    }
}
