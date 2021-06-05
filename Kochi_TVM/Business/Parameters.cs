using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Kochi_TVM.Business
{
    public static class Parameters
    {
        //public static PayPointClient sr = new PayPointClient();
        //public static OccClient occ = new OccClient();
        //public static Db db = new Db();
        //public static TVMConsts TVMConst = new TVMConsts();
        public static ParameterOperations TVMStatic = new StaticParameterOpr();
        public static DynamicParameterOpr TVMDynamic = new DynamicParameterOpr();
        //public static BackgroundWorker bwQrcPrinterStatus = null;
        //public static BackgroundWorker bwDispStatus = null;
        //public static BackgroundWorker bwBnaStatus = null;
        //public static BackgroundWorker bwAfcStatus = null;
        public static DateTime lastSync;
        public static string userId;
        public static List<string> menuItems = new List<string>();
        //public static TopupInfo tuData = new TopupInfo();

        public static class MenuStrings
        {
            public const string QrRep = "QR Replenishment";
            public const string RptRep = "RPT Replenishment";
            public const string RcptRep = "Receipt Paper Replenishment";
            public const string CoinRep = "Coin Replenishment";
            public const string CoinEmpt = "Coin Hopper Emptying";
            public const string CashDump = "Cash Recycler Dumping";
            public const string CashBoxRem = "Cash Box Removing";
            public const string QRTest = "QR Test Printing";
            public const string RptTest = "RPT Test Printing";
            public const string HopTest = "Hopper Test Dispensing";
            public const string CashEscCheck = "Cash Escrow Checking";
            public const string K1CardRdrWrk = "Kochi1 Card Reader Working";
            public const string CreditDebitCardRdWrk = "Credit/Debit Card Reader Working";
            public const string TouchScreenTest = "Touch Screen Test";
            public const string Keypad1Test = "Keypad1 Test";
            public const string Keypad2Test = "Keypad2 Test";
            public const string ScrlDispTest = "Scroll Display Test(PID)";
            public const string RcptPrntTest = "Receipt Printer Testing";
            public const string SpkrTest = "Speaker Test";
            public const string VibTest = "Vibration Test";
            public const string RFIDRdTest = "RFID Reader Test";
        };
    }
    public abstract class ParameterOperations
    {
        public Dictionary<string, string> parameterList = new Dictionary<string, string>();
        public abstract bool FillOrUpdateParameters();
        public abstract string GetParameter(string key);
        public abstract bool Init();
        public bool AddOrUpdateParameter(String descCode, String paramValue)
        {
            bool result = false;
            try
            {
                if (!parameterList.ContainsKey(descCode))
                {
                    if (!String.IsNullOrEmpty(paramValue))
                    {
                        parameterList.Add(descCode, paramValue);
                    }
                    else
                    {
                        //Log.log.Write(LogTypes.Info.ToString() + "Parameter.cs AddOrUpdateParameter line 164 " + ":" + descCode + " is Null");
                    }
                }
                else if (parameterList.ContainsKey(descCode))
                {
                    if (!String.IsNullOrEmpty(paramValue))
                    {
                        parameterList[descCode] = paramValue;
                    }
                    else
                    {
                        //Log.log.Write(LogTypes.Info.ToString() + "Parameter.cs AddOrUpdateParameter line 171 " + ":" + descCode + " is Null");
                    }
                }
                result = true;
            }
            catch (Exception e)
            { }

            return result;
        }
    }
    public class DynamicParameterOpr : ParameterOperations
    {
        bool isInit = false;
        public DynamicParameterOpr()
        {
            if (!isInit)
            {
                isInit = Init();
            }
        }
        public override bool Init()
        {
            bool result = false;
            try
            {
                FillOrUpdateParameters();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(ex.ToString());
            }
            return result;
        }
        public override bool FillOrUpdateParameters()
        {
            bool result = false;
            try
            {
                //parameterList.Clear();

                FillOrUpdateAfcConnStatus();
                FillOrUpdateScConnStatus();
                FillOrUpdateLocalParams();

                if (FillOrUpdateSysParams())
                {
                    if (GetParameter("AfcConn") == "1")
                    {
                        if (FillOrUpdateAfcConnParams())
                        {
                            result = true;
                        }
                    }
                    else if (GetParameter("SCConn") == "1")
                    {
                        if (FillOrUpdateScConnParams())
                        {
                            result = true;
                        }
                    }
                    else if (FillOrUpdateLocalParams())
                    {
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }
            return result;
        }
        public override string GetParameter(string key)
        {
            string value = String.Empty;
            try
            {
                if (parameterList.ContainsKey(key))
                {
                    value = parameterList[key];
                }
            }
            catch (Exception ex)
            {
                value = String.Empty;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }
            return value;
        }
        private bool FillOrUpdateSysParams()
        {
            bool result = false;
            try
            {
                using (var context = new Models.TVM_Entities())
                {
                    var rp = context.sp_SelSysParams().ToList();
                    //var rp = Parameters.db.ExecSP("def.sp_SelSysParams");
                    if (rp.Count > 0)
                    {
                        foreach (var item in rp)
                        {
                            AddOrUpdateParameter(item.descCode.ToString(), item.paramValue.ToString());
                        }
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }
            return result;
        }
        public bool FillOrUpdateAfcConnParams()
        {
            bool result = false;
            try
            {
                //var rpc = Parameters.sr.SelUnitByUID(Parameters.TVMStatic.GetParameter("macAddress"));
                ////Log.log.Write("MacID : " + Parameters.TVMStatic.GetParameter("macAddress"));
                //result = ((Validation.IsValidAFCRP(rpc)) && (rpc.Result == 1));
                //if (result)
                //{
                //    AddOrUpdateParameter("macAddress", rpc.Data.Tables[0].Rows[0]["macAddress"].ToString());
                //    AddOrUpdateParameter("explanation", rpc.Data.Tables[0].Rows[0]["salePointCode"].ToString());
                //    AddOrUpdateParameter("unitId", rpc.Data.Tables[0].Rows[0]["recId"].ToString());
                //    AddOrUpdateParameter("stationId", rpc.Data.Tables[0].Rows[0]["stationId"].ToString());
                //    AddOrUpdateParameter("unitType", rpc.Data.Tables[0].Rows[0]["salePointTypeId"].ToString());
                //    AddOrUpdateParameter("descCode", rpc.Data.Tables[0].Rows[0]["descCode"].ToString());
                //    AddOrUpdateParameter("localAuth", "0");
                //    AddOrUpdateParameter("localAuthTimesUp", "0");
                //    AddOrUpdateParameter("localAuthClosed", "0");
                //}
            }
            catch (Exception ex)
            {
                result = false;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }

            return result;
        }
        private bool FillOrUpdateScConnParams()
        {
            bool result = false;

            return result;
        }
        private bool FillOrUpdateAfcConnStatus()
        {
            bool result = false;
            try
            {
                if (GetAfcConnStatus())
                {
                    AddOrUpdateParameter("AfcConn", "1");
                    result = true;
                }
                else
                {
                    AddOrUpdateParameter("AfcConn", "0");
                    result = false;
                }
            }
            catch (Exception ex)
            {

                AddOrUpdateParameter("AfcConn", "0");
                result = false;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }

            //Parameters.lastSync = DateTime.Now;
            return result;
        }
        public bool GetAfcConnStatus()
        {
            bool result = false;
            try
            {
                //var rpAfc = Parameters.sr.ExecSP("def.sp_GetSrvDT");
                //var AfcConn = Validation.IsValidAFCRP(rpAfc);
                //if (AfcConn)
                //    result = true;
                //else
                //    result = false;
            }
            catch (Exception ex)
            {
                result = false;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }
            //Parameters.lastSync = DateTime.Now;
            return result;
        }
        private bool FillOrUpdateLocalParams()
        {
            bool result = false;

            try
            {
                using (var context = new Models.TVM_Entities())
                {
                    var rp = context.sp_SelSalePointByMac(Parameters.TVMStatic.GetParameter("macAddress")).ToList();
                    if (rp.Count > 0)
                    {
                        foreach (var item in rp)
                        {
                            AddOrUpdateParameter("macAddress", item.macAddress.ToString());
                            AddOrUpdateParameter("explanation", item.salePointCode.ToString());
                            AddOrUpdateParameter("unitId", item.recId.ToString());
                            //Log.log.Write("stationId");
                            //Log.log.Write("stationId :" + rpl.Data.Tables[0].Rows[0]["stationId"].ToString());
                            AddOrUpdateParameter("stationId", item.stationId.ToString());
                            AddOrUpdateParameter("descCode", item.descCode.ToString());
                            AddOrUpdateParameter("unitType", item.salePointTypeId.ToString());
                            AddOrUpdateParameter("localAuth", "1");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                //Log.log.Write(LogTypes.Error.ToString() + ": " + ex.Message);
            }

            //Parameters.lastSync = DateTime.Now;

            return result;
        }
        private bool FillOrUpdateScConnStatus()
        {
            bool result = false;
            try
            {
                //AddOrUpdateParameter("SCConn", "0");
                result = false;
            }
            catch (Exception ex)
            {
            }

            //Parameters.lastSync = DateTime.Now;
            return result;
        }
    }
    public class StaticParameterOpr : ParameterOperations
    {
        bool isInit = false;
        public StaticParameterOpr()
        {
            if (!isInit)
            {
                isInit = Init();
            }
        }
        public override bool Init()
        {
            bool result = false;
            try
            {
                FillOrUpdateParameters();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(ex.ToString());
            }
            return result;
        }
        public override bool FillOrUpdateParameters()
        {
            bool result = false;
            try
            {
                //parameterList.Clear();
                AddOrUpdateParameter("cashPaymentRemainingTime", "60");
                AddOrUpdateParameter("macAddress", GetMacAddress());
                AddOrUpdateParameter("appVersion", GetAppVersion());

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                //Logger.Log.log.Write(ex.ToString());
            }
            return result;
        }
        public override string GetParameter(string key)
        {
            string value = String.Empty;
            try
            {
                if (parameterList.ContainsKey(key))
                    value = parameterList[key];
            }
            catch (Exception ex)
            {
                //Logger.Log.log.Write("Error:" + ex.ToString());
                value = String.Empty;
            }
            return value;
        }
        public static string GetMacAddress()
        {
            //var Result = "";
            //var macAddress = string.Empty;
            //var firstPhisicalAdress = string.Empty;

            //foreach (
            //    var nic in
            //        NetworkInterface.GetAllNetworkInterfaces()
            //            .Where(
            //                nic =>
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet3Megabit) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.Wireless80211) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetT) ||
            //                    (nic.NetworkInterfaceType == NetworkInterfaceType.FastEthernetFx)))
            //{
            //    if (firstPhisicalAdress == string.Empty)
            //        firstPhisicalAdress = nic.GetPhysicalAddress().ToString();
            //    if (nic.OperationalStatus != OperationalStatus.Up) continue;
            //    macAddress = nic.GetPhysicalAddress().ToString();
            //    break;
            //}

            //if (macAddress == string.Empty)
            //    macAddress = firstPhisicalAdress;
            //if (macAddress == string.Empty) return Result;
            //var lenMacAddress = macAddress.Length;
            //for (var ii = 0; ii < lenMacAddress; ii++)
            //{
            //    if ((ii > 0) && ((ii % 2) == 0))
            //        Result += "-";
            //    Result += macAddress.Substring(ii, 1);
            //}
            //return Result;
            IniFileOperations iniReader = new IniFileOperations();
            return iniReader.ReadParamByKey("DEVICES_NAME", "MacAddress");//Parameters.TVMConst.macAddress;
        }
        public static string GetAppVersion()
        {
            var assem = Assembly.GetExecutingAssembly();
            var assemName = assem.GetName();
            var ver = assemName.Version;
            var Result = ver.ToString();
            return Result;
        }
    }

    public class IniFileOperations
    {
        string filePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + "Setup.ini";
        //[DllImport("kernel32")]
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        //public void Write(string section, string key, string value)
        //{
        //    WritePrivateProfileString(section, key, value.ToLower(), filePath);
        //}
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        public string ReadParamByKey(string section, string key)
        {
            StringBuilder SB = new StringBuilder(255);
            int i = GetPrivateProfileString(section, key, "", SB, 255, filePath);
            return SB.ToString();
        }
        public void Write(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value.ToLower(), filePath);
        }
    }
}
