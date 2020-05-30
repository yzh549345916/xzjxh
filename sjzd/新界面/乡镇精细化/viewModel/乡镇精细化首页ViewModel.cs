using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace sjzd
{
    public class Stations : INotifyPropertyChanged
    {
        private List<PlotInfo> _plotInfos;
        private Station _selectedStation;
        private StationsCollection _StationsCollection;

        public StationsCollection StationsCollection

        {
            get
            {
                if (_StationsCollection == null)
                {
                    _StationsCollection = new StationsCollection();

                    Uri resourceUri = new Uri("/sjzd;component/新界面/乡镇精细化/viewModel/StationsSampleDataSource.xaml", UriKind.Relative);
                    if (Application.GetResourceStream(resourceUri) != null)
                    {
                        Application.LoadComponent(this, resourceUri);
                    }
                }

                return _StationsCollection;
            }
        }

        public Station SelectedStation
        {
            get => _selectedStation;
            set
            {
                if (_selectedStation == value)
                {
                    return;
                }

                _selectedStation = value;
                OnPropertyChanged("SelectedStation");
            }
        }

        public List<PlotInfo> PlotInfos
        {
            get => _plotInfos;
            set
            {
                if (_plotInfos == value)
                {
                    return;
                }

                _plotInfos = value;
                OnPropertyChanged("PlotInfos");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Station : INotifyPropertyChanged
    {
        private string _Name = string.Empty;


        private string _Photo = string.Empty;
        private BitmapImage _Picture;

        private string _StationID;

        public BitmapImage Picture
        {
            get
            {
                if (_Picture == null)
                {
                    if (!string.IsNullOrEmpty(Photo))
                    {
                        _Picture = new BitmapImage(new Uri(Photo, UriKind.Relative));
                    }
                }

                return _Picture;
            }
        }

        public string StationID
        {
            get => _StationID;

            set
            {
                if (_StationID != value)
                {
                    _StationID = value;
                    OnPropertyChanged("StationID");
                }
            }
        }

        public string Name
        {
            get => _Name;

            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public string Photo
        {
            get => _Photo;

            set
            {
                if (_Photo != value)
                {
                    _Photo = value;
                    OnPropertyChanged("Photo");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class PlotInfo
    {
        public string 类别 { get; set; }
        public double 值 { get; set; }
    }

    public class StationsCollection : ObservableCollection<Station>
    {
    }
}