using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TapTrack.Demo
{
    public class Row : INotifyPropertyChanged
    {
        List<string> options = new List<string>();
        int index;
        public event PropertyChangedEventHandler PropertyChanged;


        public Row(int rowIndex)
        {
            options.Add("Text");
            options.Add("URI");
            index = rowIndex;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<string> Options
        {
            get { return options; }
            set { options = value; }
        }

        public string Content
        {
            get;
            set;
        }

        public string Selected
        {
            get;
            set;
        }

        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                index = value;
                NotifyPropertyChanged();
            }
        }
    }
}
