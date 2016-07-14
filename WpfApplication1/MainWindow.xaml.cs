using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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
using TapTrack.TappyBLE;
using TapTrack.TappyBLE.Communication;

namespace WpfApplication1
{



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Driver tappyDriver;

        public MainWindow()
        {
            InitializeComponent();
            tappyDriver = new Driver();
            tappyDriver.AutoDetect();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CustomCommand custom = new CustomCommand(new byte[] { 5, 4, 0xEA, 0xA1, 0x22, 0x6B });
            CommandFrame cmd = new CommandFrame(custom); 

            foreach (byte b in cmd.ToArray())
            {
                Debug.Write("0x"+string.Format($"{b:X}").PadLeft(2,'0')+" ");
            }
        }
    }

    public class CustomCommand : BasicNfcCommand
    {
        public CustomCommand(byte[] data) : base()
        {
            parameters.AddRange(data);
        }

        public override byte CommandCode
        {
            get
            {
                return 0x08;
            }
        }
    }
}
