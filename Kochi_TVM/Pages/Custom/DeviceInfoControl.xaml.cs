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

namespace Kochi_TVM.Pages.Custom
{
    /// <summary>
    /// Interaction logic for DeviceInfoControl.xaml
    /// </summary>
    public partial class DeviceInfoControl : UserControl
    {
        string cap = "";
        string val = "";

        public DeviceInfoControl(string caption, string value)
        {
            InitializeComponent();
            cap = caption;
            val = value;            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(val == "OK" || val == "Close" || val == "Connect")
                deviceValue.Foreground = Brushes.Green;
            else if (val == "ERROR" || val == "Open" || val == "Disconnect")
                deviceValue.Foreground = Brushes.Red;
            else if (val != "OK" && val != "Close" && val != "Connect" && val != "ERROR" && val != "Open" && val != "Disconnect")
                deviceValue.Foreground = Brushes.Black;

            deviceCaption.Content = cap;
            deviceValue.Content = val;
        }
    }
}
