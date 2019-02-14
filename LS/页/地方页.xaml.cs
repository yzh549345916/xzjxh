using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LS
{
    /// <summary>
    /// 地方页.xaml 的交互逻辑
    /// </summary>
    public partial class 地方页 : Page
    {
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 地方页()
        {
            InitializeComponent();
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围
        }
    }
}
