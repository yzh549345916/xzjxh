using System.Windows;


namespace sjzd
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            new 主页().Show();
            base.OnStartup(e);
        }
    }
}
