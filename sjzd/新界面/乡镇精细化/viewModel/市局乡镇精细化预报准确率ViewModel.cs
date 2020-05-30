using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 市局乡镇精细化预报准确率ViewModel : INotifyPropertyChanged
    {
        private string _旗县;
        private float sJ24QYZQL;
        private float sJ24TmaxZQL;
        private float sJ24TminZQL;
        private float sJ48QYZQL;
        private float sJ48TmaxZQL;
        private float sJ48TminZQL;
        private float sJ72QYZQL;
        private float sJ72TmaxZQL;
        private float sJ72TminZQL;

        public 市局乡镇精细化预报准确率ViewModel(string my旗县, float mysJ24TmaxZQL, float mysJ24TminZQL, float mysJ24QYZQL,
            float mysJ48TmaxZQL, float mysJ48TminZQL, float mysJ48QYZQL, float mysJ72TmaxZQL, float mysJ72TminZQL,
            float mysJ72QYZQL)
        {
            _旗县 = my旗县;
            sJ24TmaxZQL = mysJ24TmaxZQL;
            sJ24TminZQL = mysJ24TminZQL;
            sJ24QYZQL = mysJ24QYZQL;
            sJ48TmaxZQL = mysJ48TmaxZQL;
            sJ48TminZQL = mysJ48TminZQL;
            sJ48QYZQL = mysJ48QYZQL;
            sJ72TmaxZQL = mysJ72TmaxZQL;
            sJ72TminZQL = mysJ72TminZQL;
            sJ72QYZQL = mysJ72QYZQL;
        }

        public string 旗县
        {
            get => _旗县;
            set
            {
                if (value != _旗县)
                {
                    _旗县 = value;
                    OnPropertyChanged("旗县");
                }
            }
        }

        public float SJ24TmaxZQL
        {
            get => sJ24TmaxZQL;
            set
            {
                if (value != sJ24TmaxZQL)
                {
                    sJ24TmaxZQL = value;
                    OnPropertyChanged("SJ24TmaxZQL");
                }
            }
        }

        public float SJ24TminZQL
        {
            get => sJ24TminZQL;
            set
            {
                if (value != sJ24TminZQL)
                {
                    sJ24TminZQL = value;
                    OnPropertyChanged("SJ24TminZQL");
                }
            }
        }

        public float SJ24QYZQL
        {
            get => sJ24QYZQL;
            set
            {
                if (value != sJ24QYZQL)
                {
                    sJ24QYZQL = value;
                    OnPropertyChanged("SJ24QYZQL");
                }
            }
        }

        public float SJ48TmaxZQL
        {
            get => sJ48TmaxZQL;
            set
            {
                if (value != sJ48TmaxZQL)
                {
                    sJ48TmaxZQL = value;
                    OnPropertyChanged("SJ48TmaxZQL");
                }
            }
        }

        public float SJ48TminZQL
        {
            get => sJ48TminZQL;
            set
            {
                if (value != sJ48TminZQL)
                {
                    sJ48TminZQL = value;
                    OnPropertyChanged("SJ48TminZQL");
                }
            }
        }

        public float SJ48QYZQL
        {
            get => sJ48QYZQL;
            set
            {
                if (value != sJ48QYZQL)
                {
                    sJ48QYZQL = value;
                    OnPropertyChanged("SJ48QYZQL");
                }
            }
        }

        public float SJ72TmaxZQL
        {
            get => sJ72TmaxZQL;
            set
            {
                if (value != sJ72TmaxZQL)
                {
                    sJ72TmaxZQL = value;
                    OnPropertyChanged("SJ72TmaxZQL");
                }
            }
        }

        public float SJ72TminZQL
        {
            get => sJ72TminZQL;
            set
            {
                if (value != sJ72TminZQL)
                {
                    sJ72TminZQL = value;
                    OnPropertyChanged("SJ72TminZQL");
                }
            }
        }

        public float SJ72QYZQL
        {
            get => sJ72QYZQL;
            set
            {
                if (value != sJ72QYZQL)
                {
                    sJ72QYZQL = value;
                    OnPropertyChanged("SJ72QYZQL");
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