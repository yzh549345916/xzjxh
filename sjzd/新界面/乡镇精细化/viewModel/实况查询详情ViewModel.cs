using System;
using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 实况查询详情ViewModel : INotifyPropertyChanged
    {
        private DateTime _日期;
        private double _值53368;
        private double _值53464;
        private double _值53466;
        private double _值53467;
        private double _值53469;
        private double _值53562;
        private double _值53463;


        public 实况查询详情ViewModel(DateTime my日期, double my53368, double my53464, double my53466, double my53467, double my53469, double my53562, double my53463)
        {
            _日期 = my日期;

            _值53368 = my53368;
            _值53464 = my53464;
            _值53466 = my53466;
            _值53467 = my53467;
            _值53469 = my53469;
            _值53562 = my53562;
            _值53463 = my53463;


        }

        public DateTime 日期
        {
            get => _日期;
            set
            {
                if (value != _日期)
                {
                    _日期 = value;
                    OnPropertyChanged("日期");
                }
            }
        }

        public double 值53368
        {
            get => _值53368;
            set
            {
                if (value != _值53368)
                {
                    _值53368 = value;
                    OnPropertyChanged("值53368");
                }
            }
        }
        public double 值53464
        {
            get => _值53464;
            set
            {
                if (value != _值53464)
                {
                    _值53464 = value;
                    OnPropertyChanged("值53464");
                }
            }
        }
        public double 值53466
        {
            get => _值53466;
            set
            {
                if (value != _值53466)
                {
                    _值53466 = value;
                    OnPropertyChanged("值53466");
                }
            }
        }
        public double 值53467
        {
            get => _值53467;
            set
            {
                if (value != _值53467)
                {
                    _值53467 = value;
                    OnPropertyChanged("值53467");
                }
            }
        }
        public double 值53469
        {
            get => _值53469;
            set
            {
                if (value != _值53469)
                {
                    _值53469 = value;
                    OnPropertyChanged("值53469");
                }
            }
        }
        public double 值53463
        {
            get => _值53463;
            set
            {
                if (value != _值53463)
                {
                    _值53463 = value;
                    OnPropertyChanged("值53463");
                }
            }
        }
        public double 值53562
        {
            get => _值53562;
            set
            {
                if (value != _值53562)
                {
                    _值53562 = value;
                    OnPropertyChanged("值53562");
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