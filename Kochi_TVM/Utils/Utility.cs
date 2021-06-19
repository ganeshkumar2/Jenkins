using log4net;
using System;
using System.Speech.Synthesis;
using System.Windows.Media;
using ZXing.QrCode;

namespace Kochi_TVM.Utils
{
    public static class TVMUtility
    {
        private static ILog log = LogManager.GetLogger(typeof(TVMUtility).Name);

        private static MediaPlayer mplayer = new MediaPlayer();
        private static SpeechSynthesizer synthesizer = new SpeechSynthesizer();
        private static int voiceFile = 0;
        private static string langVal = "";
        private static string lastMessage = "";
        public static void PlayClick()
        {
            try
            {
                mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\SoundFiles\MouseClick.wav", UriKind.Relative));
                mplayer.Play();
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PlayClick() : " + ex.ToString());
            }
        }
        public static void SpeechLastMessage(string message) // defining the function which will accept a string parameter
        {
            CancelSpeach();
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult); // to change VoiceGender and VoiceAge check out those links below
            synthesizer.Volume = 100;  // (0 - 100)
            synthesizer.Rate = -1;     // (-10 - 10)
            // Asynchronous
            synthesizer.SpeakAsync(message); // here args = pran
        }
        public static void SpeechFirstMessage(int voice, string message, string lang) // defining the function which will accept a string parameter
        {
            CancelSpeach();
            voiceFile = voice;
            langVal = lang;
            synthesizer.SpeakCompleted -= VoieEndedLoop;
            synthesizer.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult); // to change VoiceGender and VoiceAge check out those links below
            synthesizer.Volume = 100;  // (0 - 100)
            synthesizer.Rate = -1;     // (-10 - 10)
            // Asynchronous
            synthesizer.SpeakAsync(message); // here args = pran
            synthesizer.SpeakCompleted += VoieEndedLoop;
        }

        private static void VoieEndedLoop(object sender, SpeakCompletedEventArgs e)
        {
            mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\" + langVal + @"\" + langVal + voiceFile + ".mp3", UriKind.Relative));
            mplayer.Play();
        }

        public static void PlayVoice(int voice, string firstmessage = null, string lastmessage = null, string lang = "EN")
        {
            try
            {
                StopAudio();

                if (firstmessage != null)
                {
                    SpeechFirstMessage(voice,firstmessage, lang);
                    return;
                }
                mplayer.MediaEnded -= new EventHandler(MediaEndedLoop);
                mplayer.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\Resources\"+ lang + @"\" + lang + voice + ".mp3", UriKind.Relative));
                mplayer.Play();
                mplayer.MediaEnded += new EventHandler(MediaEndedLoop);
                if (lastmessage != null)
                {
                    lastMessage = lastmessage;
                }

            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PlayClick() : " + ex.ToString());
            }
        }

        private static void MediaEndedLoop(object sender, EventArgs e)
        {
            if(lastMessage != "")
                SpeechLastMessage(lastMessage);

            lastMessage = "";
        }

        public static void StopAudio()
        {
            synthesizer.SpeakAsyncCancelAll();
            mplayer.Stop();
        }
        public static void CancelSpeach()
        {
            synthesizer.SpeakAsyncCancelAll();
        }
        public static string Hex2Binary(string value)
        {
            int num;
            num = int.Parse(value);
            string binaryval = "";
            string rem = "";
            int quot;
            while (num >= 1)
            {
                quot = num / 2;
                rem += (num % 2).ToString();
                num = quot;
            }
            for (int i = rem.Length - 1; i >= 0; i--)
            {
                binaryval = binaryval + rem[i];
            }
            return binaryval;
        }
        public static byte[] EnableBill(decimal Amount)
        {
            int ivar = 255;

            // for not accepting
            //1111 1110 -> FE 5/-
            //1111 1101 -> FD 10/-
            //1111 1011 -> FB 20/-
            //1111 0111 -> F7 50/-
            //1110 1111 -> EF 100/-
            //1101 1111 -> DF 200/-
            //1011 1111 -> BF 500/-
            //0111 1111 -> 7F 2000/-

            // for accepting all
            //1111 1111 -> FF

            if (Constants.NoChangeMode)
            {
                if (Amount <= 5)
                    ivar = ivar & 0x01;
                if (Amount > 5 && Amount <= 10)
                    ivar = ivar & 0x03;
                if (Amount > 10 && Amount <= 20)
                    ivar = ivar & 0x07;
                if (Amount > 20 && Amount <= 50)
                    ivar = ivar & 0x0F;
                if (Amount > 50 && Amount <= 100)
                    ivar = ivar & 0x1F;
                if (Amount > 100 && Amount <= 200)
                    ivar = ivar & 0x3F;
                if (Amount > 200 && Amount <= 500)
                    ivar = ivar & 0x7F;
                if (Amount > 500 && Amount < 2000)
                    ivar = ivar & 0xFF;
                if (Amount > 2000)
                    ivar = ivar & 0xFF;
            }
            else
            {
                if (Amount < 50)
                    ivar = ivar & 0x0F;
                if (Amount > 50 && Amount <= 100)
                    ivar = ivar & 0x1E;
                else if (Amount > 100 && Amount <= 200)
                    ivar = ivar & 0x3E;
                else if (Amount > 200 && Amount <= 1000)
                    ivar = ivar & 0x7E;
                else if (Amount > 1000 && Amount < 1500)
                    ivar = ivar & 0xFE;
                else if (Amount >= 1500)
                    ivar = ivar & 0xFF;

                //    bool Note5 = false, Note10 = false, Note20 = false, Note50 = false, Note100 = false, Note200 = false, Note500 = false, Note2000 = false;

                //    foreach (string val in Constants.BankNotesAllowed)
                //    {
                //        switch (Convert.ToInt32(val))
                //        {
                //            case 5:
                //                Note5 = true;
                //                break;
                //            case 10:
                //                Note10 = true;
                //                break;
                //            case 20:
                //                Note20 = true;
                //                break;
                //            case 50:
                //                Note50 = true;
                //                break;
                //            case 100:
                //                Note100 = true;
                //                break;
                //            case 200:
                //                Note200 = true;
                //                break;
                //            case 500:
                //                Note500 = true;
                //                break;
                //            case 2000:
                //                Note2000 = true;
                //                break;
                //            default:
                //                break;
                //        }
                //    }

                //    if (Amount <= 100)
                //    {
                //        if (Note5 && Note10 && Note20 && Note50 && Note100)
                //        {
                //            ivar = ivar & 0x1F;
                //        }
                //        else if (Note10 && Note20 && Note50 && Note100)
                //        {
                //            ivar = ivar & 0x1E;
                //        }
                //        else if (Note20 && Note50 && Note100)
                //        {
                //            ivar = ivar & 0x1C;
                //        }
                //        else if (Note50 && Note100)
                //        {
                //            ivar = ivar & 0x18;
                //        }
                //        else if (Note100)
                //        {
                //            ivar = ivar & 0x10;
                //        }
                //    }
                //    else if (Amount > 100 && Amount <= 200)
                //    {
                //        if (Note5 && Note10 && Note20 && Note50 && Note100 && Note200)
                //        {
                //            ivar = ivar & 0x3F;
                //        }
                //        else if (Note10 && Note20 && Note50 && Note100 && Note200)
                //        {
                //            ivar = ivar & 0x3E;
                //        }
                //        else if (Note20 && Note50 && Note100 && Note200)
                //        {
                //            ivar = ivar & 0x3C;
                //        }
                //        else if (Note50 && Note100 && Note200)
                //        {
                //            ivar = ivar & 0x38;
                //        }
                //        else if (Note100 && Note200)
                //        {
                //            ivar = ivar & 0x30;
                //        }
                //        else if (Note200)
                //        {
                //            ivar = ivar & 0x20;
                //        }
                //    }
                //    else if (Amount > 200 && Amount < 1500)
                //    {
                //        if (Note5 && Note10 && Note20 && Note50 && Note100 && Note200 && Note500)
                //        {
                //            ivar = ivar & 0x7F;
                //        }
                //        else if (Note10 && Note20 && Note50 && Note100 && Note200 && Note500)
                //        {
                //            ivar = ivar & 0x7E;
                //        }
                //        else if (Note20 && Note50 && Note100 && Note200 && Note500)
                //        {
                //            ivar = ivar & 0x7C;
                //        }
                //        else if (Note50 && Note100 && Note200 && Note500)
                //        {
                //            ivar = ivar & 0x78;
                //        }
                //        else if (Note100 && Note200 && Note500)
                //        {
                //            ivar = ivar & 0x70;
                //        }
                //        else if (Note200 && Note500)
                //        {
                //            ivar = ivar & 0x60;
                //        }
                //        else if (Note500)
                //        {
                //            ivar = ivar & 0x40;
                //        }
                //    }
                //    else if (Amount >= 1500)
                //    {
                //        if (Note5 && Note10 && Note20 && Note50 && Note100 && Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xFF;
                //        }
                //        else if (Note10 && Note20 && Note50 && Note100 && Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xFE;
                //        }
                //        else if (Note20 && Note50 && Note100 && Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xFC;
                //        }
                //        else if (Note50 && Note100 && Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xF8;
                //        }
                //        else if (Note100 && Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xF0;
                //        }
                //        else if (Note200 && Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xE0;
                //        }
                //        else if (Note500 && Note2000)
                //        {
                //            ivar = ivar & 0xC0;
                //        }
                //        else if (Note2000)
                //        {
                //            ivar = ivar & 0x80;
                //        }
                //    }
            }

            /// for not accepting 10/-
            /// byte[] byte2 = { 0xFF, 0xFF, 0xFD, 0x00, 0x00, 0x00 };

            byte byte1 = Convert.ToByte(ivar);
            byte[] byte2 = { 0xFF, 0xFF, byte1, 0x00, 0x00, 0x00 };
            return byte2;
        }
        public static System.Drawing.Bitmap PrepareQRImage(string content)
        {
            try
            {
                var qr = new ZXing.BarcodeWriter();
                qr.Format = ZXing.BarcodeFormat.QR_CODE;
                qr.Options = new QrCodeEncodingOptions
                {
                    DisableECI = true,
                    CharacterSet = "UTF-8",
                    Width = 200,//200,
                    Height = 200
                };
                var result = new System.Drawing.Bitmap(qr.Write(content.Trim()));
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error Utility -> PrepareQRImage() : " + ex.ToString());
                return null;
            }
        }
    }
}
