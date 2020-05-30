using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 预报人员选择 : RadWindow
    {
        public 预报人员选择()
        {
            InitializeComponent();
        }

        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Z308Btu_Copy_Click(object sender, RoutedEventArgs e)
        {
            ZBCom.Text = "";
            this.DialogResult = false;
            this.Close();
        }
    }
}
