using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 主页.xaml 的交互逻辑
    /// </summary>
    public partial class 主页 : RadWindow
    {
        public 主页()
        {
            LocalizationManager.Manager = new LocalizationManager()
            {
                ResourceManager = GridViewResources.ResourceManager
            };
            InitializeComponent();
            this.DataContext = new MainViewModel();
            XmlConfig xmlConfig = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            string theme = xmlConfig.Read("Theme");
            Settheme settheme1 = new Settheme();
            StyleManager.SetTheme(this, Gettheme.GetMyTheme(theme));
            settheme1.setTheme(settheme1.setLightOrDark(theme));
            StyleManager.ApplicationTheme = Gettheme.GetMyTheme(theme);
            this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/sjzd;component/新界面/Resources.xaml", UriKind.RelativeOrAbsolute) });
        }

        private void navigationView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
