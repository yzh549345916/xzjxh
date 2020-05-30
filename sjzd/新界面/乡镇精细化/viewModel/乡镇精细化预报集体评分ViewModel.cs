using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 乡镇精细化预报集体评分ViewModel : INotifyPropertyChanged
    {
        private float allJQ;
        private float dWJQ;
        private float dWPF;
        private float gWJQ;
        private float gWPF;
        private string name;
        private float qYJQ;
        private float qYPF;
        private float zHPF;


        public 乡镇精细化预报集体评分ViewModel(string myname, float myqYPF, float mygWPF, float mydWPF, float myzHPF, float myqYJQ,
            float mygWJQ, float mydWJQ, float myallJQ)
        {
            name = myname;
            qYPF = myqYPF;
            gWPF = mygWPF;
            dWPF = mydWPF;
            zHPF = myzHPF;
            qYJQ = myqYJQ;
            gWJQ = mygWJQ;
            dWJQ = mydWJQ;
            allJQ = myallJQ;
        }

        public string Name
        {
            get => name;
            set
            {
                if (value != name)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public float QYPF
        {
            get => qYPF;
            set
            {
                if (value != qYPF)
                {
                    qYPF = value;
                    OnPropertyChanged("QYPF");
                }
            }
        }

        public float GWPF
        {
            get => gWPF;
            set
            {
                if (value != gWPF)
                {
                    gWPF = value;
                    OnPropertyChanged("GWPF");
                }
            }
        }

        public float DWPF
        {
            get => dWPF;
            set
            {
                if (value != dWPF)
                {
                    dWPF = value;
                    OnPropertyChanged("DWPF");
                }
            }
        }

        public float ZHPF
        {
            get => zHPF;
            set
            {
                if (value != zHPF)
                {
                    zHPF = value;
                    OnPropertyChanged("ZHPF");
                }
            }
        }

        public float QYJQ
        {
            get => qYJQ;
            set
            {
                if (value != qYJQ)
                {
                    qYJQ = value;
                    OnPropertyChanged("QYJQ");
                }
            }
        }

        public float GWJQ
        {
            get => gWJQ;
            set
            {
                if (value != gWJQ)
                {
                    gWJQ = value;
                    OnPropertyChanged("GWJQ");
                }
            }
        }

        public float DWJQ
        {
            get => dWJQ;
            set
            {
                if (value != dWJQ)
                {
                    dWJQ = value;
                    OnPropertyChanged("DWJQ");
                }
            }
        }

        public float AllJQ
        {
            get => allJQ;
            set
            {
                if (value != allJQ)
                {
                    allJQ = value;
                    OnPropertyChanged("AllJQ");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, args);
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}