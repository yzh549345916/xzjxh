using System.Windows;
using Telerik.Windows.Controls;


namespace sjzd
{
    /// <summary>
    /// QXXZConfig.xaml 的交互逻辑
    /// </summary>
    public partial class QXXZConfig : RadWindow
    {
        public QXXZConfig()
        {
            InitializeComponent();
        }

        private void AddQX_Click(object sender, RoutedEventArgs e)
        {
            WPFAddQX windowAddQx = new WPFAddQX();
            windowAddQx.Show();
        }

        private void ChangeQX_Click(object sender, RoutedEventArgs e)
        {
            WPFChangeQX windowChangeQx = new WPFChangeQX();
            windowChangeQx.Show();
        }

        private void AddXZ_Click(object sender, RoutedEventArgs e)
        {
            WPFAddXZ windowAddXZ = new WPFAddXZ();
            windowAddXZ.Show();
        }

        private void ChangeXZ_Click(object sender, RoutedEventArgs e)
        {
            WPFChangeXZ windowChangeXz = new WPFChangeXZ();
            windowChangeXz.Show();
        }

        private void People_Click(object sender, RoutedEventArgs e)
        {
            人员管理页 rygl = new 人员管理页();
            rygl.Show();

        }

        private void PeopleXG_Click(object sender, RoutedEventArgs e)
        {
            人员修改 ryxg = new 人员修改();
            ryxg.Show();
        }

        private void otherConfig_Click(object sender, RoutedEventArgs e)
        {
            其他设置 qtWindow = new 其他设置();
            qtWindow.Show();
        }

        private void TB_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow.Confirm(new DialogParameters
                {
                    Content = "是否从数据库同步本地设置文件",
                    Closed = OnConfirmClosed_同步设置,
                    Owner = Application.Current.MainWindow,
                    CancelButtonContent = "否",
                    OkButtonContent = "是",
                    Header = "注意"
                });
            });
        }
        private void OnConfirmClosed_同步设置(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    ConfigClass1 configClass1 = new ConfigClass1();
                    //同步数据库旗县到本地文件
                    configClass1.TBBD();
                }

            }
            catch
            {

            }
        }
    }
}
