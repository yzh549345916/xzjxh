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
    public partial class 乡镇精细化检验右键菜单 : RadRadialMenu
    {
        string strTheme = "";
        string _path = "";
        public 乡镇精细化检验右键菜单()
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



        private void 检验_Click(object sender, RadRoutedEventArgs e)
        {

            try
            {
                RadRadialMenuItem myItem = sender as RadRadialMenuItem;
                TabWindow1 myTabWindow = this.TargetElement as TabWindow1;
                RadialMenuCommands.Hide.Execute(null, myTabWindow);
                RadTabItem radTabItem = new RadTabItem();
                radTabItem.PinButtonVisibility = Visibility.Visible;
                radTabItem.Header = myItem.ToolTipContent;
                //radTabItem.ToolTip= myItem.ToolTipContent;
                Viewbox viewbox = new Viewbox()
                {
                    Height = 850,
                    Width = 1250
                };
                switch (radTabItem.Header)
                {
                    case "市局乡镇精细化准确率查询":
                        viewbox.Child = new 市局乡镇精细化预报准确率页new();
                        radTabItem.Content = viewbox;
                        break;
                    case "旗县乡镇精细化准确率查询":
                        viewbox.Child = new 旗县乡镇精细化预报准确率页new();
                        radTabItem.Content = viewbox;
                        break;
                    case "乡镇精细化集体评分查询":
                        viewbox.Child = new 集体评分逐日查询页new();
                        radTabItem.Content = viewbox;
                        break;
                    case "乡镇精细化个人评分查询":
                        viewbox.Child = new 个人评分页new();
                        radTabItem.Content = viewbox;
                        break;
                    case "乡镇精细化逐日评分详情查询":
                        viewbox.Child = new 逐日评分详情new();
                        radTabItem.Content = viewbox;
                        break;
                    case "乡镇精细化到报率查询":
                        viewbox.Child = new 到报率查询页new();
                        radTabItem.Content = viewbox;
                        break;
                    default:
                        return;
                }

                myTabWindow.Items.Add(radTabItem);
                myTabWindow.SelectedIndex = myTabWindow.Items.Count - 1;
            }
            catch
            {
            }
        }
        private void 城镇预报检验_Click(object sender, RadRoutedEventArgs e)
        {

            try
            {
                RadRadialMenuItem myItem = sender as RadRadialMenuItem;
                TabWindow1 myTabWindow = this.TargetElement as TabWindow1;
                RadialMenuCommands.Hide.Execute(null, myTabWindow);
                RadTabItem radTabItem = new RadTabItem();
                radTabItem.PinButtonVisibility = Visibility.Visible;
                radTabItem.Header = myItem.ToolTipContent;
                radTabItem.Name = myItem.Name;
                //radTabItem.ToolTip= myItem.ToolTipContent;
                Viewbox viewbox = new Viewbox()
                {
                    Height = 700,
                    Width = 1300
                };
                switch (radTabItem.Name)
                {
                    case "城镇个人评分120小时":
                        viewbox.Child = new 城镇个人评分120小时();
                        radTabItem.Content = viewbox;
                        break;
                    case "城镇个人评分72小时":
                        viewbox.Child = new 城镇个人评分72小时();
                        radTabItem.Content = viewbox;
                        break;
                    case "城镇预报逐日评分":
                        viewbox.Child = new 城镇预报逐日评分详情();
                        radTabItem.Content = viewbox;
                        break;
                    case "城镇集体评分":
                        viewbox.Child = new 城镇集体评分();
                        radTabItem.Content = viewbox;
                        break;
                    case "城镇临时统计":
                        viewbox.Child = new 城镇统计临时();
                        radTabItem.Content = viewbox;
                        break;
                    default:
                        return;
                }

                myTabWindow.Items.Add(radTabItem);
                myTabWindow.SelectedIndex = myTabWindow.Items.Count - 1;
            }
            catch
            {
            }
        }

    }
}
