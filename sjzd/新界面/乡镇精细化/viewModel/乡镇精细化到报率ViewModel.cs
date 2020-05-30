using System.ComponentModel;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    public class 乡镇精细化到报率ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _旗县;
        private string _缺报日期;
        private string _逾期日期;
        private float _到报率;
        private float _缺报率;
        private float _逾期率;

        public string 旗县
        {
            get { return this._旗县; }
            set
            {
                if (value != this._旗县)
                {
                    this._旗县 = value;
                    this.OnPropertyChanged("旗县");
                }
            }
        }
        public string 缺报日期
        {
            get { return this._缺报日期; }
            set
            {
                if (value != this._缺报日期)
                {
                    this._缺报日期 = value;
                    this.OnPropertyChanged("缺报日期");
                }
            }
        }
        public string 逾期日期
        {
            get { return this._逾期日期; }
            set
            {
                if (value != this._逾期日期)
                {
                    this._逾期日期 = value;
                    this.OnPropertyChanged("逾期日期");
                }
            }
        }
        public float 到报率
        {
            get { return this._到报率; }
            set
            {
                if (value != this._到报率)
                {
                    this._到报率 = value;
                    this.OnPropertyChanged("到报率");
                }
            }
        }
        public float 缺报率
        {
            get { return this._缺报率; }
            set
            {
                if (value != this._缺报率)
                {
                    this._缺报率 = value;
                    this.OnPropertyChanged("缺报率");
                }
            }
        }
        public float 逾期率
        {
            get { return this._逾期率; }
            set
            {
                if (value != this._逾期率)
                {
                    this._逾期率 = value;
                    this.OnPropertyChanged("逾期率");
                }
            }
        }

        public 乡镇精细化到报率ViewModel(string my旗县, float my到报率, float my缺报率, string my缺报日期, float my逾期率, string my逾期日期)
        {
            this._旗县 = my旗县;
            this._到报率 = my到报率;
            this._缺报率 = my缺报率;
            this._缺报日期 = my缺报日期;
            this._逾期率 = my逾期率;
            this._逾期日期 = my逾期日期;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }
}
