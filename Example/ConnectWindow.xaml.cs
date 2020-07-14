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
using System.Windows.Shapes;
using TapTrack.Tcmp.Communication;

namespace TapTrack.Demo
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
        public ConnectWindow()
        {
            InitializeComponent();
        }

        public CommunicationProtocol Protocol { get; internal set; }

        private void bleButton_Click(object sender, RoutedEventArgs e)
        {
            this.Protocol = CommunicationProtocol.Bluetooth;
            this.DialogResult = true;
            this.Close();
        }

        private void usbButton_Click(object sender, RoutedEventArgs e)
        {
            this.Protocol = CommunicationProtocol.Usb;
            this.DialogResult = true;
            this.Close();
        }

        private void trueUsbButton_Click(object sender, RoutedEventArgs e)
        {
            this.Protocol = CommunicationProtocol.TrueUsb;
            this.DialogResult = true;
            this.Close();
        }
    }
}
