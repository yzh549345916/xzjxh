using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 城镇预报集体评分ViewModel : INotifyPropertyChanged
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
        private float sJ24QYZQL;
        private float sJ24TmaxZQL;
        private float sJ24TminZQL;
        private float sJ48QYZQL;
        private float sJ48TmaxZQL;
        private float sJ48TminZQL;
        private float sJ72QYZQL;
        private float sJ72TmaxZQL;
        private float sJ72TminZQL;
        private float sJ96QYZQL;
        private float sJ96TmaxZQL;
        private float sJ96TminZQL;
        private float sJ120QYZQL;
        private float sJ120TmaxZQL;
        private float sJ120TminZQL;

        private float sJ24QYJQ;
        private float sJ24TmaxJQ;
        private float sJ24TminJQ;
        private float sJ48QYJQ;
        private float sJ48TmaxJQ;
        private float sJ48TminJQ;
        private float sJ72QYJQ;
        private float sJ72TmaxJQ;
        private float sJ72TminJQ;
        private float sJ96QYJQ;
        private float sJ96TmaxJQ;
        private float sJ96TminJQ;
        private float sJ120QYJQ;
        private float sJ120TmaxJQ;
        private float sJ120TminJQ;

        public float SJ24Qyzql
        {
            get => sJ24QYZQL;
            set
            {
                if (value != sJ24QYZQL)
                {
                    sJ24QYZQL = value;
                    OnPropertyChanged("SJ24Qyzql");
                }
            }
        }

        public float SJ24TmaxZql
        {
            get => sJ24TmaxZQL;
            set
            {
                if (value != sJ24TmaxZQL)
                {
                    sJ24TmaxZQL = value;
                    OnPropertyChanged("SJ24TmaxZql");
                }
            }
        }

        public float SJ24TminZql
        {
            get => sJ24TminZQL;
            set
            {
                if (value != sJ24TminZQL)
                {
                    sJ24TminZQL = value;
                    OnPropertyChanged("SJ24TminZql");
                }
            }
        }

        public float SJ48Qyzql
        {
            get => sJ48QYZQL;
            set
            {
                if (value != sJ48QYZQL)
                {
                    sJ48QYZQL = value;
                    OnPropertyChanged("SJ48Qyzql");
                }
            }
        }

        public float SJ48TmaxZql
        {
            get => sJ48TmaxZQL;
            set
            {
                if (value != sJ48TmaxZQL)
                {
                    sJ48TmaxZQL = value;
                    OnPropertyChanged("SJ48TmaxZql");
                }
            }
        }

        public float SJ48TminZql
        {
            get => sJ48TminZQL;
            set
            {
                if (value != sJ48TminZQL)
                {
                    sJ48TminZQL = value;
                    OnPropertyChanged("SJ48TminZql");
                }
            }
        }

        public float SJ72Qyzql
        {
            get => sJ72QYZQL;
            set
            {
                if (value != sJ72QYZQL)
                {
                    sJ72QYZQL = value;
                    OnPropertyChanged("SJ72Qyzql");
                }
            }
        }

        public float SJ72TmaxZql
        {
            get => sJ72TmaxZQL;
            set
            {
                if (value != sJ72TmaxZQL)
                {
                    sJ72TmaxZQL = value;
                    OnPropertyChanged("SJ72TmaxZql");
                }
            }
        }

        public float SJ72TminZql
        {
            get => sJ72TminZQL;
            set
            {
                if (value != sJ72TminZQL)
                {
                    sJ72TminZQL = value;
                    OnPropertyChanged("SJ72TminZql");
                }
            }
        }

        public float SJ96Qyzql
        {
            get => sJ96QYZQL;
            set
            {
                if (value != sJ96QYZQL)
                {
                    sJ96QYZQL = value;
                    OnPropertyChanged("SJ96Qyzql");
                }
            }
        }

        public float SJ96TmaxZql
        {
            get => sJ96TmaxZQL;
            set
            {
                if (value != sJ96TmaxZQL)
                {
                    sJ96TmaxZQL = value;
                    OnPropertyChanged("SJ96TmaxZql");
                }
            }
        }

        public float SJ96TminZql
        {
            get => sJ96TminZQL;
            set
            {
                if (value != sJ96TminZQL)
                {
                    sJ96TminZQL = value;
                    OnPropertyChanged("SJ96TminZql");
                }
            }
        }

        public float SJ120Qyzql
        {
            get => sJ120QYZQL;
            set
            {
                if (value != sJ120QYZQL)
                {
                    sJ120QYZQL = value;
                    OnPropertyChanged("SJ120Qyzql");
                }
            }
        }

        public float SJ120TmaxZql
        {
            get => sJ120TmaxZQL;
            set
            {
                if (value != sJ120TmaxZQL)
                {
                    sJ120TmaxZQL = value;
                    OnPropertyChanged("SJ120TmaxZql");
                }
            }
        }

        public float SJ120TminZql
        {
            get => sJ120TminZQL;
            set
            {
                if (value != sJ120TminZQL)
                {
                    sJ120TminZQL = value;
                    OnPropertyChanged("SJ120TminZql");
                }
            }
        }

        public float SJ24Qyjq
        {
            get => sJ24QYJQ;
            set
            {
                if (value != sJ24QYJQ)
                {
                    sJ24QYJQ = value;
                    OnPropertyChanged("SJ24Qyjq");
                }
            }
        }

        public float SJ24TmaxJq
        {
            get => sJ24TmaxJQ;
            set
            {
                if (value != sJ24TmaxJQ)
                {
                    sJ24TmaxJQ = value;
                    OnPropertyChanged("SJ24TmaxJq");
                }
            }
        }

        public float SJ24TminJq
        {
            get => sJ24TminJQ;
            set
            {
                if (value != sJ24TminJQ)
                {
                    sJ24TminJQ = value;
                    OnPropertyChanged("SJ24TminJq");
                }
            }
        }

        public float SJ48Qyjq
        {
            get => sJ48QYJQ;
            set
            {
                if (value != sJ48QYJQ)
                {
                    sJ48QYJQ = value;
                    OnPropertyChanged("SJ48Qyjq");
                }
            }
        }

        public float SJ48TmaxJq
        {
            get => sJ48TmaxJQ;
            set
            {
                if (value != sJ48TmaxJQ)
                {
                    sJ48TmaxJQ = value;
                    OnPropertyChanged("SJ48TmaxJq");
                }
            }
        }

        public float SJ48TminJq
        {
            get => sJ48TminJQ;
            set
            {
                if (value != sJ48TminJQ)
                {
                    sJ48TminJQ = value;
                    OnPropertyChanged("SJ48TminJq");
                }
            }
        }

        public float SJ72Qyjq
        {
            get => sJ72QYJQ;
            set
            {
                if (value != sJ72QYJQ)
                {
                    sJ72QYJQ = value;
                    OnPropertyChanged("SJ72Qyjq");
                }
            }
        }

        public float SJ72TmaxJq
        {
            get => sJ72TmaxJQ;
            set
            {
                if (value != sJ72TmaxJQ)
                {
                    sJ72TmaxJQ = value;
                    OnPropertyChanged("SJ72TmaxJq");
                }
            }
        }

        public float SJ72TminJq
        {
            get => sJ72TminJQ;
            set
            {
                if (value != sJ72TminJQ)
                {
                    sJ72TminJQ = value;
                    OnPropertyChanged("SJ72TminJq");
                }
            }
        }

        public float SJ96Qyjq
        {
            get => sJ96QYJQ;
            set
            {
                if (value != sJ96QYJQ)
                {
                    sJ96QYJQ = value;
                    OnPropertyChanged("SJ96Qyjq");
                }
            }
        }

        public float SJ96TmaxJq
        {
            get => sJ96TmaxJQ;
            set
            {
                if (value != sJ96TmaxJQ)
                {
                    sJ96TmaxJQ = value;
                    OnPropertyChanged("SJ96TmaxJq");
                }
            }
        }

        public float SJ96TminJq
        {
            get => sJ96TminJQ;
            set
            {
                if (value != sJ96TminJQ)
                {
                    sJ96TminJQ = value;
                    OnPropertyChanged("SJ96TminJq");
                }
            }
        }

        public float SJ120Qyjq
        {
            get => sJ120QYJQ;
            set
            {
                if (value != sJ120QYJQ)
                {
                    sJ120QYJQ = value;
                    OnPropertyChanged("SJ120Qyjq");
                }
            }
        }

        public float SJ120TmaxJq
        {
            get => sJ120TmaxJQ;
            set
            {
                if (value != sJ120TmaxJQ)
                {
                    sJ120TmaxJQ = value;
                    OnPropertyChanged("SJ120TmaxJq");
                }
            }
        }

        public float SJ120TminJq
        {
            get => sJ120TminJQ;
            set
            {
                if (value != sJ120TminJQ)
                {
                    sJ120TminJQ = value;
                    OnPropertyChanged("SJ120TminJq");
                }
            }
        }

        public void update(float sJ24Qyzql, float sJ24TmaxZql, float sJ24TminZql, float sJ48Qyzql, float sJ48TmaxZql, float sJ48TminZql, float sJ72Qyzql, float sJ72TmaxZql, float sJ72TminZql, float sJ96Qyzql, float sJ96TmaxZql, float sJ96TminZql, float sJ120Qyzql, float sJ120TmaxZql, float sJ120TminZql, float sJ24Qyjq, float sJ24TmaxJq, float sJ24TminJq, float sJ48Qyjq, float sJ48TmaxJq, float sJ48TminJq, float sJ72Qyjq, float sJ72TmaxJq, float sJ72TminJq, float sJ96Qyjq, float sJ96TmaxJq, float sJ96TminJq, float sJ120Qyjq, float sJ120TmaxJq, float sJ120TminJq)
        {
            sJ24QYZQL = sJ24Qyzql;
            sJ24TmaxZQL = sJ24TmaxZql;
            sJ24TminZQL = sJ24TminZql;
            sJ48QYZQL = sJ48Qyzql;
            sJ48TmaxZQL = sJ48TmaxZql;
            sJ48TminZQL = sJ48TminZql;
            sJ72QYZQL = sJ72Qyzql;
            sJ72TmaxZQL = sJ72TmaxZql;
            sJ72TminZQL = sJ72TminZql;
            sJ96QYZQL = sJ96Qyzql;
            sJ96TmaxZQL = sJ96TmaxZql;
            sJ96TminZQL = sJ96TminZql;
            sJ120QYZQL = sJ120Qyzql;
            sJ120TmaxZQL = sJ120TmaxZql;
            sJ120TminZQL = sJ120TminZql;
            sJ24QYJQ = sJ24Qyjq;
            sJ24TmaxJQ = sJ24TmaxJq;
            sJ24TminJQ = sJ24TminJq;
            sJ48QYJQ = sJ48Qyjq;
            sJ48TmaxJQ = sJ48TmaxJq;
            sJ48TminJQ = sJ48TminJq;
            sJ72QYJQ = sJ72Qyjq;
            sJ72TmaxJQ = sJ72TmaxJq;
            sJ72TminJQ = sJ72TminJq;
            sJ96QYJQ = sJ96Qyjq;
            sJ96TmaxJQ = sJ96TmaxJq;
            sJ96TminJQ = sJ96TminJq;
            sJ120QYJQ = sJ120Qyjq;
            sJ120TmaxJQ = sJ120TmaxJq;
            sJ120TminJQ = sJ120TminJq;
        }

        public 城镇预报集体评分ViewModel(string myname, float myqYPF, float mygWPF, float mydWPF, float myzHPF, float myqYJQ,
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