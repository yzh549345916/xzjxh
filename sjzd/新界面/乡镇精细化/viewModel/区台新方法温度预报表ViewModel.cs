using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 区台新方法温度预报表ViewModel : INotifyPropertyChanged
    {
        private DateTime _日期;
        private string _类型;
        private double _值53368;
        private double _值53464;
        private double _值53466;
        private double _值53467;
        private double _值53469;
        private double _值53562;
        private double _值53463;
        private bool _IsExpanded;
        private List<实况查询详情ViewModel> _详情;

        public 区台新方法温度预报表ViewModel(DateTime my日期, string my类型, double my53368, double my53464, double my53466, double my53467, double my53469, double my53562, double my53463 )
        {
            _日期 = my日期;
            _类型 = my类型;
            _值53368 = my53368;
            _值53464 = my53464;
            _值53466 = my53466;
            _值53467 = my53467;
            _值53469 = my53469;
            _值53562 = my53562;
            _值53463 = my53463;

        }
        public bool IsExpanded
        {
            get => _IsExpanded;
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    OnPropertyChanged("IsExpanded");
                }
            }
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
        public string 类型
        {
            get => _类型;
            set
            {
                if (value != _类型)
                {
                    _类型 = value;
                    OnPropertyChanged("类型");
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
        

        public List<实况查询详情ViewModel> 详情
        {
            get => _详情;
            set
            {
                if (value != _详情)
                {
                    _详情 = value;
                    OnPropertyChanged("详情");
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