using System.Collections.Generic;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Utils
{
    public static class Constants
    {
        public static int CheckDeviceTime = 15 * 1000;

        public static bool IsVoiceEnabled = false;
        //BNR
        public static string BNRStatus = "";
        public static bool IsBNRAvalable = false;
        public static int Cassette1NoteCont = 0;
        public static int Cassette2NoteCont = 0;
        public static int Cassette3NoteCont = 0;
        public static int EscrowCassetteNo = 0;
        public static int EscrowAmount = 50;
        public static Cassette[] CassettesInfo = new Cassette[3];
        public static byte[] EnableBillNotes = new byte[6];
        public static bool NoChangeMode = false;

        public static string Hopper1Level = "";
        public static string Hopper2Level = "";
        public static string Hopper3Level = "";

        public static int HopperAddress3Coin = 5;
        public static int HopperAddress2Coin = 2;
        public static int HopperAddress1Coin = 1;
        public static int NoChangeAvailable = 5;

        public static string CurrencySymbol = "₹";
    }
}
