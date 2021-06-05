using Kochi_TVM.BNR;
using Kochi_TVM.CCTalk;
using Kochi_TVM.Utils;
using log4net;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using static Kochi_TVM.Utils.Enums;

namespace Kochi_TVM.Pages
{
    /// <summary>
    /// Interaction logic for OutOfSevicePage.xaml
    /// </summary>
    public partial class OutOfSevicePage : Page
    {
        private static ILog log = LogManager.GetLogger(typeof(OutOfSevicePage).Name);
        public OutOfSevicePage()
        {
            InitializeComponent();
            new Thread(() => AsyncIntFunc()).Start();
        }

        private void AsyncIntFunc()
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
                {
                  await Task.Delay(1000);

                    InitializeBNR();

                }));
            }
            catch (Exception ex)
            {
                log.Error("Error OutOfServicePage -> asyncFunc() : " + ex.ToString());
            }
        }

        private void InitializeBNR()
        {

            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    outOfServiceLbl.Content = "Initializing BNR";
                    BNRManager.Instance.PollingAction();
                }));

            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                {
                    outOfServiceLbl.Content = ex.Message;
                }));
            }

        }


        private void CoinHopper1()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_1.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_1.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper One OK";                      
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = ex.Message;
                }
            }));
        }

        private void CoinHopper2()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_2.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_2.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper Two OK";                    
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = ex.Message;
                }
            }));
        }

        private void CoinHopper3()
        {
            Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
            {
                try
                {
                    if (CCTalkManager.Instance.coinHopperEV4000_3.Manufacture != null)
                    {
                        CCTalkManager.Instance.coinHopperEV4000_3.EnableHopper();
                        outOfServiceLbl.Content = "Coin Hopper Three OK";                        
                    }
                }
                catch (Exception ex)
                {
                    outOfServiceLbl.Content = ex.Message;
                }
            }));
        }

        private void Page_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent += new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
        }

        private void Page_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BNRManager.BNRStateInputEvent -= new BNRManager.BNRStateEventHandler(BNRManager_BNRStateInputEvent);
        }

        private void BNRManager_BNRStateInputEvent(Utils.Enums.BNRState state)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    Constants.BNRStatus = Enum.GetName(typeof(BNRState), state);
                    Dispatcher.Invoke(DispatcherPriority.Background, new Action(() =>
                    {
                        outOfServiceLbl.Content = "BNR Status : " + Constants.BNRStatus;
                    }));
                    log.Debug("BNR Status : " + Constants.BNRStatus);
                    if (state == BNRState.DISABLED)
                    {
                        new Thread(() => AsyncIntHopperFunc()).Start();
                    }
                }
                catch (Exception ex)
                {
                    log.Error("Error PayByCashOrCoinPage -> BNRManager_BNRStateInputEvent : " + ex.ToString());
                }
            }), DispatcherPriority.Background);
        }

        private void AsyncIntHopperFunc()
        {
            try
            {
                Dispatcher.Invoke(DispatcherPriority.Background, new Action(async () =>
                {
                    await Task.Delay(1000);                                       

                    CoinHopper1();

                    await Task.Delay(1000);

                    CoinHopper2();

                    await Task.Delay(1000);

                    CoinHopper3();
                    await Task.Delay(1000);

                    NavigationService.Navigate(new Pages.MainPage());
                }));
            }
            catch (Exception ex)
            {
                log.Error("Error OutOfServicePage -> asyncFunc() : " + ex.ToString());
            }
        }
    }
}
