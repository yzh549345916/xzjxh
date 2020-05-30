using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RadialMenu;

namespace sjzd
{
    /// <summary>
    /// 设置菜单.xaml 的交互逻辑
    /// </summary>
    public partial class 设置菜单 : UserControl
    {
        string strTheme = "";
        public 设置菜单()
        {
            InitializeComponent();
        }
        private void OnRadRadialMenuNavigated(object sender, RadRoutedEventArgs e)
        {
            this.CloseAllRadWindows();

        }

        private void OnRadRadialMenuItemClick(object sender, RadRoutedEventArgs e)
        {

            this.CloseAllRadWindows();
        }

        private void OnRadialMenuClosed(object sender, RadRoutedEventArgs e)
        {
            this.CloseAllRadWindows();
        }

        private void CloseAllRadWindows()
        {
            // Skip MainWindow and close all other open RadWindows.
            RadWindowManager.Current.GetWindows().Skip(1).ToList().ForEach(w => w.Close());
        }


        private void Theme_Click(object sender, RadRoutedEventArgs e)
        {
            RadRadialMenuItem r1 = sender as RadRadialMenuItem;
            string name = r1.Name;
            strTheme = name;
            Settheme settheme1 = new Settheme();
            RadWindow rw = GetParentObject<RadWindow>(this, "");
            主页 mw = rw.Owner.Content as 主页;
            StyleManager.SetTheme(mw, GetMyTheme(name));
            settheme1.setTheme(settheme1.setLightOrDark(name));
            StyleManager.ApplicationTheme = GetMyTheme(name);

        }
        private void SaveBtu_Click(object sender, RoutedEventArgs e)
        {
            XmlConfig xmlConfig = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            xmlConfig.Write(strTheme, "Theme");
            RadWindow rw = GetParentObject<RadWindow>(this, "");
            rw.Close();
        }
        public Theme GetMyTheme(string name)
        {
            string myName = name.ToLower();
            if (myName.Contains("crystal"))
            {
                return new CrystalTheme();
            }
            else if (myName.Contains("fluent"))
            {
                return new FluentTheme();
            }
            else if (myName.Contains("material"))
            {
                return new MaterialTheme();
            }
            else if (myName.Contains("office2016touch"))
            {
                return new Office2016TouchTheme();
            }
            else if (myName.Contains("office2016"))
            {
                return new Office2016Theme();
            }
            else if (myName.Contains("green"))
            {
                return new GreenTheme();
            }
            else if (myName.Contains("office2013"))
            {
                return new Office2013Theme();
            }
            else if (myName.Contains("visualstudio2013"))
            {
                return new VisualStudio2013Theme();
            }
            else if (myName.Contains("windows8touch"))
            {
                return new Windows8TouchTheme();
            }
            else if (myName.Contains("windows8"))
            {
                return new Windows8Theme();
            }
            else if (myName.Contains("office_black"))
            {
                return new Office_BlackTheme();
            }
            else if (myName.Contains("office_blue"))
            {
                return new Office_BlueTheme();
            }
            else if (myName.Contains("office_silver"))
            {
                return new Office_SilverTheme();
            }
            else if (myName.Contains("summer"))
            {
                return new SummerTheme();
            }
            else if (myName.Contains("vista"))
            {
                return new VistaTheme();
            }
            else if (myName.Contains("transparent"))
            {
                return new TransparentTheme();
            }
            else if (myName.Contains("windows7"))
            {
                return new Windows7Theme();
            }
            else if (myName.Contains("expression_dark"))
            {
                return new Expression_DarkTheme();
            }
            return new CrystalTheme();
        }


        /// <summary>
        /// 查找父控件
        /// </summary>
        /// <typeparam name="T">父控件的类型</typeparam>
        /// <param name="obj">要找的是obj的父控件</param>
        /// <param name="name">想找的父控件的Name属性</param>
        /// <returns>目标父控件</returns>
        public static T GetParentObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            while (parent != null)
            {
                if (parent is T)
                {
                    return (T)parent;
                }

                // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        private void RadRadialMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            RadRadialMenuItem rad = sender as RadRadialMenuItem;
            NavigateContext context = new NavigateContext(rad);
            rrm.CommandService.ExecuteCommand(Telerik.Windows.Controls.RadialMenu.Commands.CommandId.NavigateToView, context);
        }

        private void CancelBtu_Click(object sender, RoutedEventArgs e)
        {

            XmlConfig xmlConfig = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            string theme = xmlConfig.Read("Theme");
            Settheme settheme1 = new Settheme();
            RadWindow rw = GetParentObject<RadWindow>(this, "");
            主页 mw = rw.Owner.Content as 主页;
            StyleManager.SetTheme(mw, GetMyTheme(theme));
            settheme1.setTheme(settheme1.setLightOrDark(theme));

            rw.Close();
        }
    }
}
