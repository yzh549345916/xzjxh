using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sjzd
{
    /// <summary>
    /// 预报检验窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 预报检验窗口 : Window
    {
        public 预报检验窗口()
        {
            InitializeComponent();
        }
        private void 省级智能网格高低温查询_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/省级智能网格高低温查询.xaml", UriKind.Relative);
        }
        private void 市局乡镇精细化预报准确率页_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/市局乡镇精细化预报准确率页.xaml", UriKind.Relative);
        }
        private void 集体评分逐日查询页_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/集体评分逐日查询页.xaml", UriKind.Relative);
        }
        private void 旗县乡镇精细化预报准确率页_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/旗县乡镇精细化预报准确率页.xaml", UriKind.Relative);
        }
        private void 个人评分逐日查询页_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/个人评分页.xaml", UriKind.Relative);
        }
        private void 省级智能网格准确率查询页_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/省级智能网格准确率查询页.xaml", UriKind.Relative);
        }
        private void 逐日评分详情_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/逐日评分详情.xaml", UriKind.Relative);
        }
        private void 到报率查询_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/到报率查询页.xaml", UriKind.Relative);
        }
    }
}
