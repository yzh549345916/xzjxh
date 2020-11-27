using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 数值预报呼和浩特ViewModel : INotifyPropertyChanged
    {
        private DateTime _预报时间;
        private string _站点名称;
        private string _区站号;
        private string _所属旗县;
        private double _值1;
        private double _值2;
        private double _值3;
        private string _字符串1;
        private string _字符串2;
        private string _测站级别;
        private bool _IsExpanded;
        private List<数值预报呼和浩特详情ViewModel> _详情;
        public 数值预报呼和浩特ViewModel()
        {

        }
        public 数值预报呼和浩特ViewModel(DateTime my预报时间, string my站点名称, string my区站号, string my所属旗县, double my值1, double my值2, string my字符串1, string my字符串2)
        {
            _预报时间 = my预报时间;
            _站点名称 = my站点名称;
            _区站号 = my区站号;
            _所属旗县 = my所属旗县;
            _值1 = my值1;
            _值2 = my值2;
            _字符串1 = my字符串1;
            _字符串2 = my字符串2;

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
        public DateTime 预报时间
        {
            get => _预报时间;
            set
            {
                if (value != _预报时间)
                {
                    _预报时间 = value;
                    OnPropertyChanged("_预报时间");
                }
            }
        }
        public string 站点名称
        {
            get => _站点名称;
            set
            {
                if (value != _站点名称)
                {
                    _站点名称 = value;
                    OnPropertyChanged("站点名称");
                }
            }
        }
        public string 区站号
        {
            get => _区站号;
            set
            {
                if (value != _区站号)
                {
                    _区站号 = value;
                    OnPropertyChanged("区站号");
                }
            }
        }
        public string 所属旗县
        {
            get => _所属旗县;
            set
            {
                if (value != _所属旗县)
                {
                    _所属旗县 = value;
                    OnPropertyChanged("所属旗县");
                }
            }
        }
        public string 字符串1
        {
            get => _字符串1;
            set
            {
                if (value != _字符串1)
                {
                    _字符串1 = value;
                    OnPropertyChanged("字符串1");
                }
            }
        }
        public string 字符串2
        {
            get => _字符串2;
            set
            {
                if (value != _字符串2)
                {
                    _字符串2 = value;
                    OnPropertyChanged("字符串2");
                }
            }
        }
        public string 测站级别
        {
            get => _测站级别;
            set
            {
                if (value != _测站级别)
                {
                    _测站级别 = value;
                    OnPropertyChanged("测站级别");
                }
            }
        }
        public double 值1
        {
            get => _值1;
            set
            {
                if (value != _值1)
                {
                    _值1 = value;
                    OnPropertyChanged("值1");
                }
            }
        }
        public double 值2
        {
            get => _值2;
            set
            {
                if (value != _值2)
                {
                    _值2 = value;
                    OnPropertyChanged("值2");
                }
            }
        }
        public double 值3
        {
            get => _值3;
            set
            {
                if (value != _值3)
                {
                    _值3 = value;
                    OnPropertyChanged("值3");
                }
            }
        }

        public List<数值预报呼和浩特详情ViewModel> 详情
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
    public class 数值预报呼和浩特详情ViewModel : INotifyPropertyChanged
    {
        private DateTime _预报时间;
        private double _要素值;
        public 数值预报呼和浩特详情ViewModel(DateTime my预报时间, double my要素值)
        {
            _预报时间 = my预报时间;

            _要素值 = my要素值;


        }

        public DateTime 预报时间
        {
            get => _预报时间;
            set
            {
                if (value != _预报时间)
                {
                    _预报时间 = value;
                    OnPropertyChanged("预报时间");
                }
            }
        }

        public double 要素值
        {
            get => _要素值;
            set
            {
                if (value != _要素值)
                {
                    _要素值 = value;
                    OnPropertyChanged("要素值");
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