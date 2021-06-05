using Kochi_TVM.BNR;
using Kochi_TVM.Business;
using Kochi_TVM.MultiLanguages;
using Kochi_TVM.Pages.Custom;
using Kochi_TVM.Utils;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            MultiLanguage.Init("EN");
            Message();
        }
        
        void Message()
        {
            if (MultiLanguage.GetCurrentLanguage() == "EN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1,null, Stations.currentStation.name, "EN");
            }
            if (MultiLanguage.GetCurrentLanguage() == "ML" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1, Stations.currentStation.name,null , "ML");
            }
            if (MultiLanguage.GetCurrentLanguage() == "IN" && Constants.IsVoiceEnabled)
            {
                Utility.PlayVoice(1, Stations.currentStation.name, null, "IN");
            }
        }

        private void btnSelectTicket_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            NavigationService.Navigate(new Pages.JourneyTypePage());
        }

        private void btnSelectCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnLang1_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            SetLanguage(BtnLang.CurHintBtn1MlBtn2En);
        }

        private void btnLang2_Click(object sender, RoutedEventArgs e)
        {
            Utility.PlayClick();
            SetLanguage(BtnLang.CurMlBtn1EnBtn2Hint);
        }

        public void SetLanguage(BtnLang btnLang)
        {
            //ÖNCEKİ HAL
            //current   btn1    btn2
            //en        hint    ml
            //hint      en      ml
            //ml        en      hint   


            //SON HAL
            //current   btn1    btn2
            //en        ml      hint
            //hint      ml       en
            //ml        en      hint  

            // fonksiyon önceki hale göre yapılmış, son hal kararlaştırılsa fonksiyon güncellenecek

            if (btnLang == BtnLang.CurEnBtn1MlBtn2Hint)
            {
                //set to default            
                Ticket.language = Languages.English;
            }
            else if (btnLang == BtnLang.CurHintBtn1MlBtn2En)
            {
                if (MultiLanguage.GetCurrentLanguage() == "EN")
                {
                    Ticket.language = Languages.Hint;
                }
                else
                {
                    SetLanguage(BtnLang.CurEnBtn1MlBtn2Hint);
                }
            }
            else if (btnLang == BtnLang.CurMlBtn1EnBtn2Hint)
            {
                if (MultiLanguage.GetCurrentLanguage() == "EN")
                {
                    Ticket.language = Languages.Malayalam;
                }
               
            }

            if (Ticket.language == Languages.English)
            {
                MultiLanguage.ChangeLanguage("EN");
            }
            else if (Ticket.language == Languages.Malayalam)
            {
                MultiLanguage.ChangeLanguage("ML");
            }
            else if (Ticket.language == Languages.Hint)
            {
                MultiLanguage.ChangeLanguage("IN");
            }


            lblHeader.Content = MultiLanguage.GetText("welcome");
            btnSelectTicket.Content = MultiLanguage.GetText("buyTicket");
            btnSelectCard.Content = MultiLanguage.GetText("k1card");
            lblComingSoon.Content = MultiLanguage.GetText("comingSoon");
            lblSelectLanguage.Content = MultiLanguage.GetText("selectLang");

            //btnLang1.Content = "മലയാളം";
            //btnLang2.Content = "हिन्दी";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BNRManager.Instance.PollingAction();
        }
    }
}
