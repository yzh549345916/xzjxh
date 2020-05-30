using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 乡镇精细化逐日评分ViewModel : INotifyPropertyChanged
    {

        private string name;
        private string stationID;
        private float qXGW, qXDW, sKGW, sKDW, sJGW, sJDW;
        private string qXTQ, qXQY, sKJS, sJTQ, sJQY;

        public 乡镇精细化逐日评分ViewModel()
        {
        }

        public 乡镇精细化逐日评分ViewModel(string myname, string mystationID, float myqXGW, float myqXDW, float mysKGW, float mysKDW, float mysJGW,
            float mysJDW, string myqXTQ, string myqXQY, string mysKJS, string mysJTQ, string mysJQY)
        {
            name = myname;
            stationID = mystationID;
            qXGW = myqXGW;
            qXDW = myqXDW;
            sKGW = mysKGW;
            sKDW = mysKDW;
            sJGW = mysJGW;
            sJDW = mysJDW;
            qXTQ = myqXTQ;
            qXQY = myqXQY;
            sKJS = mysKJS;
            sJTQ = mysJTQ;
            sJQY = mysJQY;
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

        public string StationID
        {
            get => stationID;
            set
            {
                if (value != stationID)
                {
                    stationID = value;
                    OnPropertyChanged("StationID");
                }
            }
        }

        public float QXGW
        {
            get => qXGW;
            set
            {
                if (value != qXGW)
                {
                    qXGW = value;
                    OnPropertyChanged("QXGW");
                }
            }
        }
        public float QXDW
        {
            get => qXDW;
            set
            {
                if (value != qXDW)
                {
                    qXDW = value;
                    OnPropertyChanged("QXDW");
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
        public float SJGW
        {
            get => sJGW;
            set
            {
                if (value != sJGW)
                {
                    sJGW = value;
                    OnPropertyChanged("SJGW");
                }
            }
        }
        public float SJDW
        {
            get => sJDW;
            set
            {
                if (value != sJDW)
                {
                    sJDW = value;
                    OnPropertyChanged("SJDW");
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
        public string QXTQ
        {
            get => qXTQ;
            set
            {
                if (value != qXTQ)
                {
                    qXTQ = value;
                    OnPropertyChanged("QXTQ");
                }
            }
        }
        public string QXQY
        {
            get => qXQY;
            set
            {
                if (value != qXQY)
                {
                    qXQY = value;
                    OnPropertyChanged("QXQY");
                }
            }
        }
        public string SJTQ
        {
            get => sJTQ;
            set
            {
                if (value != sJTQ)
                {
                    sJTQ = value;
                    OnPropertyChanged("SJTQ");
                }
            }
        }
        public string SJQY
        {
            get => sJQY;
            set
            {
                if (value != sJQY)
                {
                    sJQY = value;
                    OnPropertyChanged("SJQY");
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