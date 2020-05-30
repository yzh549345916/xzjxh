using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 乡镇精细化预报个人评分ViewModel : INotifyPropertyChanged
    {
        private float allJQ;
        private float dWJQ;
        private float dWPF;
        private float gWJQ;
        private float gWPF;
        private string name;
        private string peopleID;
        private string peopleName;
        private short pM;
        private float qYJQ;
        private float qYPF;
        private short zBCS;
        private short zBJS;
        private float zHPF;


        public 乡镇精细化预报个人评分ViewModel(string myname, string mypeopleID, string mypeopleName, short mypM, short myzBCS,
            short myzBJS, float myqYPF, float mygWPF, float mydWPF, float myzHPF, float myqYJQ,
            float mygWJQ, float mydWJQ, float myallJQ)
        {
            name = myname;
            peopleID = mypeopleID;
            peopleName = mypeopleName;
            pM = mypM;
            zBCS = myzBCS;
            ZBJS = myzBJS;
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

        public string PeopleID
        {
            get => peopleID;
            set
            {
                if (value != peopleID)
                {
                    peopleID = value;
                    OnPropertyChanged("PeopleID");
                }
            }
        }

        public string PeopleName
        {
            get => peopleName;
            set
            {
                if (value != peopleName)
                {
                    peopleName = value;
                    OnPropertyChanged("PeopleName");
                }
            }
        }

        public short PM
        {
            get => pM;
            set
            {
                if (value != pM)
                {
                    pM = value;
                    OnPropertyChanged("PM");
                }
            }
        }

        public short ZBCS
        {
            get => zBCS;
            set
            {
                if (value != zBCS)
                {
                    zBCS = value;
                    OnPropertyChanged("ZBCS");
                }
            }
        }

        public short ZBJS
        {
            get => zBJS;
            set
            {
                if (value != zBJS)
                {
                    zBJS = value;
                    OnPropertyChanged("ZBJS");
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