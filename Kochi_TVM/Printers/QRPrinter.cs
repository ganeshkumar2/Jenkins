using MessagingToolkit.QRCode.Codec;
using QRCPrintLib;
using SveltaLib;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Printers
{
    public class QRPrinter
    {
        #region Defines
        string id = "QR";
        string name = "QR Printer";
        string description = "Custom KPM150H";
        private static Printer_ErrorCodes errCode = Printer_ErrorCodes.Success;
        private static string errDesc = String.Empty;
        public static bool qrPrinterInit = false;
        public static PrinterStatus status = PrinterStatus.Unknown;

        string printerName = String.Empty;

        QRCPrint qrPrinter = new QRCPrint();
        #endregion

        #region DeviceBase
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private static QRPrinter _instance = null;
        public static QRPrinter Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QRPrinter();
                return _instance;
            }
        }
        public QRPrinter()
        {
            Init();
        }

        public bool Init()
        {
            bool result = false;
            string port = String.Empty;
            string logo = String.Empty;
            Bitmap image = null;
            string error = String.Empty;
            try
            {
                string headerAddress = "Images\\kmrl_icon.png";
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + headerAddress);
                image = new System.Drawing.Bitmap(img);
                qrPrinter.printerPort = ConfigurationManager.AppSettings["QR_PORT_NAME"];
                if (qrPrinter.Init("", image, ref status, ref error))
                {
                    result = true;
                    qrPrinterInit = true;
                }
            }
            catch (Exception ex)
            {

                result = false;
                qrPrinterInit = false;
            }

            return result;
        }

        public bool CheckQrPrinterStatus()
        {
            return qrPrinterInit;
        }

        public bool Check()
        {
            var server = new LocalPrintServer();
            PrintQueue queue = server.DefaultPrintQueue;

            if (queue.HasPaperProblem || queue.IsPaperJammed)
                errCode = Printer_ErrorCodes.PaperError;
            else if (queue.IsNotAvailable || queue.IsOffline)
                errCode = Printer_ErrorCodes.NotAvailable;
            else if (queue.IsBusy)
                errCode = Printer_ErrorCodes.DeviceBussyError;
            else
                errCode = Printer_ErrorCodes.Success;

            return errCode == Printer_ErrorCodes.Success;
        }
        private void SetErrCode(PrinterStatus errStatus)
        {
            errCode = Printer_ErrorCodes.Success;

            if (errStatus == PrinterStatus.CutterError)
                errCode = Printer_ErrorCodes.CutterError;
            else if (errStatus == PrinterStatus.HeadingOverTemperatureError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.NotchError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.PaperEnd)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.PowerSupplyVoltageError)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.WrongCommand)
                errCode = Printer_ErrorCodes.UnknownError;
            else if (errStatus == PrinterStatus.Unknown)
                errCode = Printer_ErrorCodes.UnknownError;
        }
        public bool GetStatus(ref PrinterStatus status)
        {
            errCode = Printer_ErrorCodes.Success;
            errDesc = String.Empty;
            String error = String.Empty;
            PrinterStatus printerStatus = PrinterStatus.NoError;
            bool result = false;
            try
            {
                result = qrPrinter.GetStatus(ref printerStatus, ref error);
                if (printerStatus != PrinterStatus.NoError)
                {
                    result = false;
                    SetErrCode(printerStatus);
                    errDesc = "Exception Message : " + error;
                }
            }
            catch (Exception ex)
            {
                result = false;
                errCode = Printer_ErrorCodes.UnknownError;
                errDesc = "Exception Message : " + ex.Message;
                Console.WriteLine(errDesc);
            }
            status = printerStatus;
            return result;
        }

        #endregion

        #region PrinterBase
        public bool Print()
        {
            return true;
        }

        public List<string> PrintQR(string ticketGUID, string journeyType, string startStation, string endStation, int peopleCount, decimal totalPrice, string ticketID)
        {
            //bool result = false;
            List<string> result = new List<string>();

            try
            {
                string err = String.Empty;

                Bitmap imageOF = new Bitmap(new QRCodeEncoder().Encode(ticketGUID), new Size(180, 180));

                bool resultQr = qrPrinter.PrintG(imageOF, "KOCHI METRO", ticketGUID, DateTime.Now.ToString(), journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, string.Format("Kochi1 card holder saved Rs. {0:0.00}\non this trip. Get your card now!!\n- \nPlease retain till the end of \njourney!", totalPrice / 10), ref err);//TVM'deki halin TOM'a benzetilmesi
                //qrPrinter.PrintK("KOCHI METRO", ticketGUID, DateTime.Now.ToString(), journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, "Please retain till the end of journey!", ref err);//TVM'deki halin TOM'a benzetilmesi
                //bool resultQr = qrPrinter.Print("KOCHI METRO", ticketGUID, DateTime.Now.ToString(), journeyType, startStation, endStation, (peopleCount == 0) ? "" : peopleCount.ToString(), String.Format("Rs.{0}", totalPrice), ticketID, "Please retain till the end of journey!", ref err);//TVM orjinali
                result.Add(resultQr.ToString());                                                                                                                                                                                                                                                                   //return qrPrinter.Print("KOCHI METRO", t.TicketGUID, PayPointConst.QRCodeCreateDate.ToString(), t.explanation, t.From, t.To, (t.count == 0) ? "" : t.count.ToString(), $"Rs.{t.price}", $"{PayPointConst.qrDayCount}.{unitParams.stationId.ToString("D2")}.{ unitParams.unitId.ToString("D2")}.{t.alias}", "Please retain till the end of journey!", ref err);//TOM'daki şekli
                result.Add(err);                                                                                                                                                                                                                                                                   //return qrPrinter.Print("KOCHI METRO", t.TicketGUID, PayPointConst.QRCodeCreateDate.ToString(), t.explanation, t.From, t.To, (t.count == 0) ? "" : t.count.ToString(), $"Rs.{t.price}", $"{PayPointConst.qrDayCount}.{unitParams.stationId.ToString("D2")}.{ unitParams.unitId.ToString("D2")}.{t.alias}", "Please retain till the end of journey!", ref err);//TOM'daki şekli

            }
            catch (Exception ex)
            {
                //result= false;
                result.Add(false.ToString());                                                                                                                                                                                                                                                                   //return qrPrinter.Print("KOCHI METRO", t.TicketGUID, PayPointConst.QRCodeCreateDate.ToString(), t.explanation, t.From, t.To, (t.count == 0) ? "" : t.count.ToString(), $"Rs.{t.price}", $"{PayPointConst.qrDayCount}.{unitParams.stationId.ToString("D2")}.{ unitParams.unitId.ToString("D2")}.{t.alias}", "Please retain till the end of journey!", ref err);//TOM'daki şekli
                result.Add(ex.Message + Environment.NewLine + ex.StackTrace);

            }

            return result;
        }

        public bool GetStatus(ref PrinterStatus status, ref string error)
        {
            bool result = false;
            error = string.Empty;

            result = qrPrinter.GetStatus(ref status, ref error);

            return result;

        }

        public bool GetVersion(ref string info, ref string error)
        {
            return qrPrinter.GetVersion(ref info, ref error);
        }

        public bool GetFullStatus(ref List<FullStatus> fullStatus)
        {
            bool result = false;
            string err = string.Empty;
            List<UserStatus> userStatus = new List<UserStatus>();
            List<RecoverebleErrorStatus> recoverebleErrorStatus = new List<RecoverebleErrorStatus>();
            List<UnrecoverebleErrorStatus> unrecoverebleErrorStatus = new List<UnrecoverebleErrorStatus>();
            try
            {
                result = qrPrinter.GetFullStatus(ref err, ref fullStatus, ref userStatus, ref recoverebleErrorStatus, ref unrecoverebleErrorStatus);
                if (!result)
                {
                    errCode = Printer_ErrorCodes.UnknownError;
                    errDesc = "Exception Message : " + err;
                }
            }
            catch (Exception ex)
            {
                errCode = Printer_ErrorCodes.UnknownError;
                errDesc = "Exception Message : " + ex.Message;
            }

            return result;
        }
        #endregion
    }

    public enum Printer_ErrorCodes
    {
        //device err : 10, device Code : 05, err Code : 1 - 99 => ex : 100312
        Success = 0,
        ParameterError = 100501,
        GetStatusError = 100502,
        ResetError = 100503,
        DeviceBussyError = 100504,
        NotAvailable = 100505,
        OfflineError = 100506,
        PaperError = 100507,
        JammedError = 100508,
        PaperEnd = 100409,
        NoError = 100410,
        WrongCommand = 100411,
        NotchError = 100412,
        HeadingOverTemperatureError = 100413,
        PowerSupplyVoltageError = 100414,
        CutterError = 100415,
        UnknownError = 100599
    }
}
