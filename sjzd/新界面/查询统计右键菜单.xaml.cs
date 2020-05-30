using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.RadialMenu;

namespace sjzd
{
    //图标颜色 橙色：27c542  蓝色 ：  1488D8
    /// <summary>
    /// 设置菜单.xaml 的交互逻辑
    /// </summary>
    public partial class 查询统计右键菜单 : RadRadialMenu
    {
        string strTheme = "";
        string _path = "";
        public 查询统计右键菜单()
        {
            InitializeComponent();

        }
        private void OnRadRadialMenuNavigated(object sender, RadRoutedEventArgs e)
        {


        }

        private void OnRadRadialMenuItemClick(object sender, RadRoutedEventArgs e)
        {


        }

        private void OnRadialMenuClosed(object sender, RadRoutedEventArgs e)
        {

        }

        private void CloseAllRadWindows()
        {
            // Skip MainWindow and close all other open RadWindows.
            //RadWindowManager.Current.GetWindows().Skip(1).ToList().ForEach(w => w.Close());
        }


        private void Theme_Click(object sender, RadRoutedEventArgs e)
        {
            RadRadialMenuItem r1 = sender as RadRadialMenuItem;
            string name = r1.Name;
            strTheme = name;



        }


        private void RadRadialMenuItem_Click(object sender, RadRoutedEventArgs e)
        {
            RadRadialMenuItem rad = sender as RadRadialMenuItem;
            NavigateContext context = new NavigateContext(rad);
            rrm.CommandService.ExecuteCommand(Telerik.Windows.Controls.RadialMenu.Commands.CommandId.NavigateToView, context);
        }





        private void 查询实况_Click(object sender, RadRoutedEventArgs e)
        {
            try
            {
                RadRadialMenuItem myItem = sender as RadRadialMenuItem;
                TabWindow查询统计 myTabWindow = this.TargetElement as TabWindow查询统计;
                RadialMenuCommands.Hide.Execute(null, myTabWindow);
                RadTabItem radTabItem = new RadTabItem();
                radTabItem.PinButtonVisibility = Visibility.Visible;
                radTabItem.Header = myItem.ToolTipContent;
                //radTabItem.ToolTip= myItem.ToolTipContent;
                Viewbox viewbox = new Viewbox()
                {
                    Height = 850,
                    Width = 1550,
                    Stretch=System.Windows.Media.Stretch.Uniform
                };
                switch (radTabItem.Header)
                {
                    case "实况查询":
                        viewbox.Child = new 实况查询();
                        radTabItem.Content = viewbox;
                        break;
                    case "实况统计":
                        viewbox.Child = new 实况统计();
                        radTabItem.Content = viewbox;
                        break;
                    case "EC温度查询":
                        viewbox.Child = new EC温度08表格();
                        radTabItem.Content = viewbox;
                        break;
                    
                    default:
                        return;
                }

                myTabWindow.Items.Add(radTabItem);
                myTabWindow.SelectedIndex = myTabWindow.Items.Count - 1;
            }
            catch(Exception ex)
            {

            }


        }

        private void EC表格08_Click(object sender, RadRoutedEventArgs e)
        {

        }

        private void EC表格20_Click(object sender, RadRoutedEventArgs e)
        {

        }
    }
}
