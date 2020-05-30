using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 城镇预报逐日评分ViewModel : INotifyPropertyChanged
    {


        private string qXName;
        private string iD;
        private float sTGW;
        private float sKGW;
        private float zDGW;
        private string gW1;
        private string gW2;
        private string gW3;
        private float sTDW;
        private float sKDW;
        private float zDDW;
        private string dW1;
        private string dW2;
        private string dW3;
        private string sTTQ;
        private string sTQY;
        private string sKJS;
        private string zDTQ;
        private string zDQY;
        private string sTTQ12;
        private string sTQY12;
        private string sKJS12;
        private string zDTQ12;
        private string zDQY12;
        private string sTTQ24;
        private string sTQY24;
        private string sKJS24;
        private string zDTQ24;
        private string zDQY24;


        public 城镇预报逐日评分ViewModel()
        {
        }

        public 城镇预报逐日评分ViewModel(string myname, string mystationID, float mysTGW, float mysTDW, float mysKGW, float mysKDW, float myzDGW,
            float myzDDW, string myqXTQ, string myqXQY, string mysKJS, string mysJTQ, string mysJQY)
        {
            
        }

        public string QXName
        {
            get => qXName;
            set
            {
                if (value != qXName)
                {
                    qXName = value;
                    OnPropertyChanged("QXName");
                }
            }
        }

        public string ID
        {
            get => iD;
            set
            {
                if (value != iD)
                {
                    iD = value;
                    OnPropertyChanged("ID");
                }
            }
        }

        public float STGW
        {
            get => sTGW;
            set
            {
                if (value != sTGW)
                {
                    sTGW = value;
                    OnPropertyChanged("STGW");
                }
            }
        }
        public string GW1
        {
            get => gW1;
            set
            {
                if (value != gW1)
                {
                    gW1 = value;
                    OnPropertyChanged("GW1");
                }
            }
        }
        public string GW2
        {
            get => gW2;
            set
            {
                if (value != gW2)
                {
                    gW2 = value;
                    OnPropertyChanged("GW2");
                }
            }
        }
        public string GW3
        {
            get => gW3;
            set
            {
                if (value != gW3)
                {
                    gW3 = value;
                    OnPropertyChanged("GW3");
                }
            }
        }
        public string DW1
        {
            get => dW1;
            set
            {
                if (value != dW1)
                {
                    dW1 = value;
                    OnPropertyChanged("DW1");
                }
            }
        }
        public string DW2
        {
            get => dW2;
            set
            {
                if (value != dW2)
                {
                    dW2 = value;
                    OnPropertyChanged("DW2");
                }
            }
        }
        public string DW3
        {
            get => dW3;
            set
            {
                if (value != dW3)
                {
                    dW3 = value;
                    OnPropertyChanged("DW3");
                }
            }
        }
        public float STDW
        {
            get => sTDW;
            set
            {
                if (value != sTDW)
                {
                    sTDW = value;
                    OnPropertyChanged("STDW");
                }
            }
        }
        public float SKGW
        {
            get => sKGW;
            set
            {
                if (value != sKGW)
                {
                    sKGW = value;
                    OnPropertyChanged("SKGW");
                }
            }
        }
        public float SKDW
        {
            get => sKDW;
            set
            {
                if (value != sKDW)
                {
                    sKDW = value;
                    OnPropertyChanged("SKDW");
                }
            }
        }
        public float ZDGW
        {
            get => zDGW;
            set
            {
                if (value != zDGW)
                {
                    zDGW = value;
                    OnPropertyChanged("ZDGW");
                }
            }
        }
        public float ZDDW
        {
            get => zDDW;
            set
            {
                if (value != zDDW)
                {
                    zDDW = value;
                    OnPropertyChanged("ZDDW");
                }
            }
        }
        public string SKJS
        {
            get => sKJS;
            set
            {
                if (value != sKJS)
                {
                    sKJS = value;
                    OnPropertyChanged("SKJS");
                }
            }
        }
        public string STTQ
        {
            get => sTTQ;
            set
            {
                if (value != sTTQ)
                {
                    sTTQ = value;
                    OnPropertyChanged("STTQ");
                }
            }
        }
        public string STQY
        {
            get => sTQY;
            set
            {
                if (value != sTQY)
                {
                    sTQY = value;
                    OnPropertyChanged("STQY");
                }
            }
        }
        public string ZDTQ
        {
            get => zDTQ;
            set
            {
                if (value != zDTQ)
                {
                    zDTQ = value;
                    OnPropertyChanged("ZDTQ");
                }
            }
        }
        public string ZDQY
        {
            get => zDQY;
            set
            {
                if (value != zDQY)
                {
                    zDQY = value;
                    OnPropertyChanged("ZDQY");
                }
            }
        }

        public string STTQ12
        {
            get => sTTQ12;
            set
            {
                if (value != sTTQ12)
                {
                    sTTQ12 = value;
                    OnPropertyChanged("STTQ12");
                }
            }
        }
        public string STQY12
        {
            get => sTQY12;
            set
            {
                if (value != sTQY12)
                {
                    sTQY12 = value;
                    OnPropertyChanged("STQY12");
                }
            }
        }
        public string SKJS12
        {
            get => sKJS12;
            set
            {
                if (value != sKJS12)
                {
                    sKJS12 = value;
                    OnPropertyChanged("SKJS12");
                }
            }
        }
        public string ZDTQ12
        {
            get => zDTQ12;
            set
            {
                if (value != zDTQ12)
                {
                    zDTQ12 = value;
                    OnPropertyChanged("ZDTQ12");
                }
            }
        }
        public string ZDQY12
        {
            get => zDQY12;
            set
            {
                if (value != zDQY12)
                {
                    zDQY12 = value;
                    OnPropertyChanged("ZDQY12");
                }
            }
        }
        public string STTQ24
        {
            get => sTTQ24;
            set
            {
                if (value != sTTQ24)
                {
                    sTTQ24 = value;
                    OnPropertyChanged("STTQ24");
                }
            }
        }
        public string STQY24
        {
            get => sTQY24;
            set
            {
                if (value != sTQY24)
                {
                    sTQY24 = value;
                    OnPropertyChanged("STQY24");
                }
            }
        }
        public string SKJS24
        {
            get => sKJS24;
            set
            {
                if (value != sKJS24)
                {
                    sKJS24 = value;
                    OnPropertyChanged("SKJS24");
                }
            }
        }
        public string ZDTQ24
        {
            get => zDTQ24;
            set
            {
                if (value != zDTQ24)
                {
                    zDTQ24 = value;
                    OnPropertyChanged("ZDTQ24");
                }
            }
        }
        public string ZDQY24
        {
            get => zDQY24;
            set
            {
                if (value != zDQY24)
                {
                    zDQY24 = value;
                    OnPropertyChanged("ZDQY24");
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