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
using System.Windows.Shapes;

namespace LS
{
    /// <summary>
    /// pageCeshi.xaml 的交互逻辑
    /// </summary>
    public partial class pageCeshi : Window
    {
        public pageCeshi()
        {
            InitializeComponent();
        }

        private void 地方_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/地方页.xaml",UriKind.Relative);
        }
        private void 你猜_Selected(object sender, RoutedEventArgs e)
        {
            this.frame.Source = new Uri("/页/地方页.xaml", UriKind.Relative);
        }
    }
}
