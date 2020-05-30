using System.ComponentModel;

namespace sjzd
{
    public class 进度条ViewModel : INotifyPropertyChanged
    {
        private double _value;

        public double myValue
        {
            get => _value;
            set
            {
                if (_value == value)
                {
                    return;
                }

                _value = value;
                OnPropertyChanged("myValue");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}