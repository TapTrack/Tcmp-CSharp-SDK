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

namespace TapTrack.Demo
{
    /// <summary>
    /// Interaction logic for BatchPasswordEntryForm.xaml
    /// </summary>
    public partial class BatchPasswordEntryForm : Window
    {
        public BatchPasswordEntryForm()
        {
            InitializeComponent();
        }

        public string enteredPassword = null;

        private void setBatchPassword_Click(object sender, RoutedEventArgs e)
        {
            if(batchTagPasswordEntry.Password == batchTagPasswordConfirmation.Password)
            {
                this.enteredPassword = batchTagPasswordEntry.Password;
                this.DialogResult = true;
                this.Close();
            }
            else
            {
                batchTagPasswordEntry.Clear();
                batchTagPasswordConfirmation.Clear();
            }
            
           
        }
    }
}
