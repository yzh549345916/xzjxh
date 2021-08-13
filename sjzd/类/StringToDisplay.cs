using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace sjzd.类
{
    public class StringToDisplay : INotifyPropertyChanged

    {
        private string text = "";

        public string Text

        {
            get => text;

            set

            {
                text = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Text"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
