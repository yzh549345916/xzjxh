using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 蒙草预报产品ViewModel : INotifyPropertyChanged
    {
        private DateTime _时间;
        private string _名称;
        private double _降水量;
        private double _气温;
        private double _相对湿度;
        private string _天气;
        private string _风向;
        private string _风力;
        private string _ID;
        private bool _IsExpanded;
        

       
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
        public DateTime 时间
        {
            get => _时间;
            set
            {
                if (value != _时间)
                {
                    _时间 = value;
                    OnPropertyChanged("时间");
                }
            }
        }
        public string 名称
        {
            get => _名称;
            set
            {
                if (value != _名称)
                {
                    _名称 = value;
                    OnPropertyChanged("名称");
                }
            }
        }
        public string 天气
        {
            get => _天气;
            set
            {
                if (value != _天气)
                {
                    _天气 = value;
                    OnPropertyChanged("天气");
                }
            }
        }
        public string 风向
        {
            get => _风向;
            set
            {
                if (value != _风向)
                {
                    _风向 = value;
                    OnPropertyChanged("风向");
                }
            }
        }
        public string 风力
        {
            get => _风力;
            set
            {
                if (value != _风力)
                {
                    _风力 = value;
                    OnPropertyChanged("风力");
                }
            }
        }
        public string ID
        {
            get => _ID;
            set
            {
                if (value != _ID)
                {
                    _ID = value;
                    OnPropertyChanged("ID");
                }
            }
        }
        public double 降水量
        {
            get => _降水量;
            set
            {
                if (value != _降水量)
                {
                    _降水量 = value;
                    OnPropertyChanged("降水量");
                }
            }
        }
        public double 气温
        {
            get => _气温;
            set
            {
                if (value != _气温)
                {
                    _气温 = value;
                    OnPropertyChanged("气温");
                }
            }
        }
        public double 相对湿度
        {
            get => _相对湿度;
            set
            {
                if (value != _相对湿度)
                {
                    _相对湿度 = value;
                    OnPropertyChanged("相对湿度");
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