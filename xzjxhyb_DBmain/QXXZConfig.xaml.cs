using System.Windows;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// QXXZConfig.xaml 的交互逻辑
    /// </summary>
    public partial class QXXZConfig : Window
    {
        public QXXZConfig()
        {
            InitializeComponent();
            /*
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = @"E:\气象台软件\CITYFORECAST\MakeForeCast(2.0).exe";    // 指定路径
            info.Arguments = "";
            info.WindowStyle = ProcessWindowStyle.Normal;   // 设置窗体
            Process pro = Process.Start(info);  // 启动
            */
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

        private void QYBtu_Click(object sender, RoutedEventArgs e)
        {
            数据库设置窗口 DBConfigWind = new 数据库设置窗口();
            DBConfigWind.Show();
        }
    }
}
