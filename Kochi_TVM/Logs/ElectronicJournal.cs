using Kochi_TVM.Business;
using Kochi_TVM.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Logs
{
    public static class ElectronicJournal
    {
        private static object ejsLock = new object();

        private static string folderName = AppDomain.CurrentDomain.BaseDirectory + @"TVM_LOG\Device_Log\";

        private static string fileName = Parameters.TVMDynamic.GetParameter("unitId") + DateTime.Now.ToString("ddMMyyyy") + ".ej";

        private static string path = folderName + fileName;

        private static string categoryval = "";

        private static string transactionNumber = "";

        private static string commondata = "DateTime: " + DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + " Stations Id: " + Stations.currentStation.id +" TVM Id: " + String.Format("{0:D8}", Convert.ToInt64(Parameters.TVMDynamic.GetParameter("unitId"))) + " ";

        private static readonly Dictionary<ElectronicJournalRecordType, string> EJItemList = new Dictionary<ElectronicJournalRecordType, string>()
        {
            {ElectronicJournalRecordType.OrderStarted , " OrderStarted "},
            {ElectronicJournalRecordType.ItemSelected , " ItemSelected "},
            {ElectronicJournalRecordType.Destinationselected , " Destinationselected "},
            {ElectronicJournalRecordType.NumberOfTickets , " NumberOfTickets "},
            {ElectronicJournalRecordType.AmountPayable , " AmountPayable "},
            {ElectronicJournalRecordType.MediaSelected , " MediaSelected "},
            {ElectronicJournalRecordType.MediaPaid , " MediaPaid "},
            {ElectronicJournalRecordType.QRPrintStarted , " QRPrintStarted"},
            {ElectronicJournalRecordType.QRPrintOver , " QRPrintOver"},
            {ElectronicJournalRecordType.ReceiptPrintStarted , " ReceiptPrintStarted"},
            {ElectronicJournalRecordType.ReceiptPrintOver , " ReceiptPrintOver"},
            {ElectronicJournalRecordType.OrderFinalised , " OrderFinalised"},
            {ElectronicJournalRecordType.TopupStarted , " TopupStarted"},
            {ElectronicJournalRecordType.TopupAmount , " TopupAmount "},
            {ElectronicJournalRecordType.TopupCompleted , " TopupCompleted "},
            {ElectronicJournalRecordType.BalanceCheckStarted , " BalanceCheckStarted"},
            {ElectronicJournalRecordType.BalanceCheckCompleted , " BalanceCheckCompleted"},
            {ElectronicJournalRecordType.BalanceSyncStarted , " BalanceSyncStarted"},
            {ElectronicJournalRecordType.BalanceSyncCompleted , " BalanceSyncCompleted"},
            {ElectronicJournalRecordType.NoteInserted , " NoteInserted "},
            {ElectronicJournalRecordType.CoinInserted , " CoinInserted "},
             {ElectronicJournalRecordType.NoteReturned , " NoteReturned "},
            {ElectronicJournalRecordType.CoinReturned , " CoinReturned "},
            {ElectronicJournalRecordType.BalanceNoteReturned , " BalanceNoteReturned "},
            {ElectronicJournalRecordType.BalanceCoinReturned , " BalanceCoinReturned "},
             {ElectronicJournalRecordType.OrderCancelled , " OrderCancelled "}
        };

        private static void CheckFolders()
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
        }

        public static void CreateFile()
        {
            lock (ejsLock)
            {
                try
                {
                    //create doc content into file
                    CheckFolders();
                    if (!File.Exists(path))
                    {
                        FileStream fs = new FileStream(path, FileMode.Create);
                        TVMUtility.ResetTransactionFileSequenceID(0);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        public static string GetEJCode(ElectronicJournalRecordType electronicJournalRecordType)
        {
            string ejCode = "";
            try
            {
                EJItemList.TryGetValue(electronicJournalRecordType, out ejCode);
            }
            catch (Exception ex)
            {

            }
            return ejCode;
        }

        public static void OrderStarted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    transactionNumber = TVMUtility.GenerateTransactionNumber();
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.OrderStarted) + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)));
                    tw.Close();
                }
            }
        }

        public static void ItemSelected(string category)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    categoryval = category;
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.ItemSelected) + category);
                    tw.Close();
                }
            }
        }

        public static void DestinationSelected(string destination)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.Destinationselected) + categoryval + ":" + destination);
                    tw.Close();
                }
            }
        }

        public static void NumberOfTicket(string tickets)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.NumberOfTickets) + ":" + tickets);
                    tw.Close();
                }
            }
        }

        public static void AmountPayable(string amount)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.AmountPayable) + amount);
                    tw.Close();
                }
            }
        }

        static string datamediacode = "";
        public static void MediaSelected(string mediacode)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    datamediacode = mediacode;
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.MediaSelected) + mediacode);
                    tw.Close();
                }
            }
        }

        public static void MediaPaid(string total, string paid, string balance)
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.MediaPaid) + datamediacode + ":" + total + ":" + paid + ":" + balance);
                    tw.Close();
                }
            }
        }

        public static void QRPrintStarted()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.QRPrintStarted));
                    tw.Close();
                }
            }
        }

        public static void QRPrintOver()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.QRPrintOver));
                    tw.Close();
                }
            }
        }

        public static void ReceiptPrintStarted()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.ReceiptPrintStarted));
                    tw.Close();
                }
            }
        }

        public static void ReceiptPrintOver()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.ReceiptPrintOver));
                    tw.Close();
                }
            }
        }

        public static void OrderFinalised()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.OrderFinalised));
                    tw.Close();
                }
            }
        }

        public static void OrderCancelled()
        {
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.OrderCancelled));
                    tw.Close();
                }
            }
        }
        public static void TopupStarted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    transactionNumber = TVMUtility.GenerateTransactionNumber();
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.TopupStarted));
                    tw.Close();
                }
            }
        }

        public static void TopupAmount(string amount)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.TopupAmount) + amount);
                    tw.Close();
                }
            }
        }

        public static void TopupCompleted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.TopupCompleted));
                    tw.Close();
                }
            }
        }

        public static void BalanceCheckStarted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    transactionNumber = TVMUtility.GenerateTransactionNumber();
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceCheckStarted));
                    tw.Close();
                }
            }
        }

        public static void BalanceCheckCompleted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceCheckCompleted));
                    tw.Close();
                }
            }
        }

        public static void BalanceSyncStarted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    transactionNumber = TVMUtility.GenerateTransactionNumber();
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceSyncStarted));
                    tw.Close();
                }
            }
        }

        public static void BalanceSyncCompleted()
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceSyncCompleted));
                    tw.Close();
                }
            }
        }

        public static void NoteInserted(int bill)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.NoteInserted) + " " + bill);
                    tw.Close();
                }
            }
        }

        public static void CoinInserted(int coin)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.CoinInserted) + " " + coin);
                    tw.Close();
                }
            }
        }

        public static void NoteReturned(int bill)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.NoteReturned) + " " + bill);
                    tw.Close();
                }
            }
        }

        public static void CoinReturned(int coin)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.CoinReturned) + " " + coin);
                    tw.Close();
                }
            }
        }

        public static void BalanceNoteReturned(int bill)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceNoteReturned) + " " + bill);
                    tw.Close();
                }
            }
        }

        public static void BalanceCoinReturned(int coin)
        {
            CreateFile();
            if (File.Exists(path))
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                using (StreamWriter tw = new StreamWriter(fs))
                {
                    tw.WriteLine(commondata + String.Format("{0:D8}", Convert.ToInt64(transactionNumber)) + GetEJCode(ElectronicJournalRecordType.BalanceCoinReturned) + " " + coin);
                    tw.Close();
                }
            }
        }
    }
}
