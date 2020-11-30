using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    ///     主页.xaml 的交互逻辑
    /// </summary>
    public partial class 主页 : RadWindow
    {
        public 主页()
        {
            LocalizationManager.Manager = new LocalizationManager
            {
                ResourceManager = GridViewResources.ResourceManager
            };
            InitializeComponent();

            #region

            //.net core3.1中不支持GB2312编码,解决办法：
            // 1、下载安装System.Text.Encoding.CodePages。
            //2、 使用“Encoding.RegisterProvider”方法进行注册。

            #endregion

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DataContext = new MainViewModel();
            var xmlConfig = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            var theme = xmlConfig.Read("Theme");
            var settheme1 = new Settheme();
            StyleManager.SetTheme(this, Gettheme.GetMyTheme(theme));
            settheme1.setTheme(settheme1.setLightOrDark(theme));
            StyleManager.ApplicationTheme = Gettheme.GetMyTheme(theme);
            Resources.MergedDictionaries.Add(new ResourceDictionary
                {Source = new Uri("/sjzd;component/新界面/Resources.xaml", UriKind.RelativeOrAbsolute)});
        }

        private void navigationView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}