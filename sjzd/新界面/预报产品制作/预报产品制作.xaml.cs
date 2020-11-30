using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Map;
using Telerik.Windows.Controls.Navigation;

namespace sjzd
{
    /// <summary>
    /// mapUsercontrol.xaml 的交互逻辑
    /// </summary>
    public partial class 预报产品制作 : UserControl
    {
        string strTheme = "";
        string _path = "";
        public 预报产品制作()
        {
            InitializeComponent();
            生成窗体();
        }

        private void 生成窗体()
        {
            RadTreeViewItem 根目录 =new RadTreeViewItem() { Header = "预报产品制作" };
            根目录.IsExpanded = true;
            生成日常产品目录(根目录);
            生成乡镇精细化预报目录(根目录);
            生成非日常预报目录(根目录);
            生成其他预报目录(根目录);
            radTree.Items.Add(根目录);
        }

        private void 生成日常产品目录(RadTreeViewItem 根目录)
        {
            RadTreeViewItem 日常产品 = new RadTreeViewItem() { Header = "日常产品制作", IsExpanded = true };
            日常产品.Items.Add(new RadTreeViewItem() { Header = "制作社区预报",Name="社区预报"});
            日常产品.Items.Add(new RadTreeViewItem() { Header = "制作市四区预报", Name = "制作市四区预报" });
            RadTreeViewItem 短时预报= new RadTreeViewItem() { Header = "短时预报", IsExpanded = false };
            短时预报.Items.Add(new RadTreeViewItem() { Header = "短时08时", Name = "短时08时" });
            短时预报.Items.Add(new RadTreeViewItem() { Header = "短时14时", Name = "短时14时" });
            短时预报.Items.Add(new RadTreeViewItem() { Header = "短时20时", Name = "短时20时" });
            日常产品.Items.Add(短时预报);
            RadTreeViewItem 逐3小时预报 = new RadTreeViewItem() { Header = "逐3小时预报", IsExpanded = false };
            逐3小时预报.Items.Add(new RadTreeViewItem() { Header = "逐3小时08时", Name = "逐3小时08时" });
            逐3小时预报.Items.Add(new RadTreeViewItem() { Header = "逐3小时20时", Name = "逐3小时20时" });
            日常产品.Items.Add(逐3小时预报);
            RadTreeViewItem 短期预报 = new RadTreeViewItem() { Header = "短期预报", IsExpanded = false };
            短期预报.Items.Add(new RadTreeViewItem() { Header = "短期预报08时", Name = "短期预报08时" });
            短期预报.Items.Add(new RadTreeViewItem() { Header = "短期预报20时", Name = "短期预报20时" });
            日常产品.Items.Add(短期预报);
            日常产品.Items.Add(new RadTreeViewItem() { Header = "制作中期逐日预报", Name = "制作中期逐日预报" });
            日常产品.Items.Add(new RadTreeViewItem() { Header = "制作防凌预报", Name = "制作防凌预报" });
            日常产品.Items.Add(new RadTreeViewItem() { Header = "制作花粉预报", Name = "制作花粉预报" });
            根目录.Items.Add(日常产品);
        }

        private void 生成乡镇精细化预报目录(RadTreeViewItem 根目录)
        {
            RadTreeViewItem 乡镇精细化 = new RadTreeViewItem() { Header = "乡镇精细化预报制作", IsExpanded = true };
            乡镇精细化.Items.Add(new RadTreeViewItem() { Header = "实况订正指导预报", Name = "实况订正指导预报制作乡镇精细化预报" });
            乡镇精细化.Items.Add(new RadTreeViewItem() { Header = "省级智能网格", Name = "省级智能网格制作乡镇精细化预报" });
            乡镇精细化.Items.Add(new RadTreeViewItem() { Header = "制作发布单", Name = "制作乡镇精细化发布单" });
            根目录.Items.Add(乡镇精细化);
        }
        private void 生成非日常预报目录(RadTreeViewItem 根目录)
        {
            RadTreeViewItem 非日常 = new RadTreeViewItem() { Header = "非日常预报制作", IsExpanded = false };
            RadTreeViewItem 环保局专报 = new RadTreeViewItem() { Header = "环保局专报", IsExpanded = false };
            环保局专报.Items.Add(new RadTreeViewItem() { Header = "环保局专报08时", Name = "环保局专报08时" });
            环保局专报.Items.Add(new RadTreeViewItem() { Header = "环保局专报20时", Name = "环保局专报20时" });
            非日常.Items.Add(环保局专报);
            非日常.Items.Add(new RadTreeViewItem() { Header = "蒙草预报服务", Name = "蒙草预报服务" });
            根目录.Items.Add(非日常);
        }
        private void 生成其他预报目录(RadTreeViewItem 根目录)
        {
            RadTreeViewItem 其他 = new RadTreeViewItem() { Header = "其他", IsExpanded = false };
            其他.Items.Add(new RadTreeViewItem() { Header = "生态与农业", Name = "生态与农业" });
            其他.Items.Add(new RadTreeViewItem() { Header = "科开预报导出", Name = "科开预报导出" });
            其他.Items.Add(new RadTreeViewItem() { Header = "赛罕智能网格", Name = "赛罕智能网格" });
            根目录.Items.Add(其他);
        }

        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            RadTreeViewItem item = radTree.SelectedItem as RadTreeViewItem;
            if (item != null && item.Items.Count == 0)
            {
                switch (item.Name)
                {
                    case "社区预报":
                        制作社区预报();
                        break; 
                    case "制作市四区预报":
                        制作市四区预报();
                        break;
                    case "短时08时":
                        短时08时();
                        break;
                    case "短时14时":
                        短时14时();
                        break;
                    case "短时20时":
                        短时20时();
                        break;
                    case "逐3小时08时":
                        逐3小时08时();
                        break;
                    case "逐3小时20时":
                        逐3小时20时();
                        break;
                    case "短期预报08时":
                        短期预报08时();
                        break;
                    case "短期预报20时":
                        短期预报20时();
                        break;
                    case "制作中期逐日预报":
                        制作中期逐日预报();
                        break;
                    case "制作防凌预报":
                        制作防凌预报();
                        break;
                    case "制作花粉预报":
                        制作花粉预报();
                        break;
                    case "实况订正指导预报制作乡镇精细化预报":
                        实况订正指导预报制作乡镇精细化预报();
                        break;
                    case "省级智能网格制作乡镇精细化预报":
                        省级智能网格制作乡镇精细化预报();
                        break;
                    case "制作乡镇精细化发布单":
                        制作乡镇精细化发布单();
                        break;
                    case "环保局专报08时":
                        环保局专报08时();
                        break;
                    case "环保局专报20时":
                        环保局专报20时();
                        break;
                    case "蒙草预报服务":
                        蒙草预报服务();
                        break;
                    case "生态与农业":
                        生态与农业();
                        break;
                    case "科开预报导出":
                        科开预报导出();
                        break;
                    case "赛罕智能网格":
                        赛罕智能网格();
                        break;
                    default:
                        break;
                }
            }
        }
        #region 日常
        private void 短时08时()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成短时预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputInt = 8 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 短时14时()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成短时预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputInt = 14 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 短时20时()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成短时预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputInt = 20 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 逐3小时08时()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成逐3小时预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputInt = 8 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 逐3小时20时()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成逐3小时预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputInt = 20 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 短期预报08时()
        {
            预报人员选择2 ryxz = new 预报人员选择2();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成短期预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.FBCom.Text, inputStr3 = ryxz.QFCom.Text, inputInt = 8 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 短期预报20时()
        {
            预报人员选择2 ryxz = new 预报人员选择2();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();
                        settingsDialog.Content = new 进度条_实况订正("生成短期预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.FBCom.Text, inputStr3 = ryxz.QFCom.Text, inputInt = 20 };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 制作防凌预报()
        {
            try
            {
                //后续新增人员选择窗口
                预报人员选择3 ryxz = new 预报人员选择3();
                if (ryxz.ShowDialog() == true)
                {
                    if (ryxz.ZBCom.Text.Trim().Length > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RadWindow settingsDialog = new RadWindow();
                            settingsDialog.Content = new 进度条_实况订正("生成防凌预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text, inputStr3 = ryxz.SHCom.Text, inputStr4 = ryxz.LXCom.Text };
                            settingsDialog.Closed += 产品生成窗口关闭;
                            settingsDialog.ResizeMode = ResizeMode.CanResize;
                            settingsDialog.Header = "正在处理数据";
                            settingsDialog.Owner = Application.Current.MainWindow;
                            settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            settingsDialog.HideMinimizeButton = true;
                            settingsDialog.HideMaximizeButton = true;
                            settingsDialog.CanClose = false;
                            settingsDialog.ShowDialog();
                        });
                    }
                }


            }
            catch (Exception)
            {
            }

        }
        private void 制作中期逐日预报()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();


                settingsDialog.Content = new 进度条_实况订正("生成中期逐日预报产品");
                settingsDialog.Closed += 产品生成窗口关闭;
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.ShowDialog();
            });
        }
        private void 制作花粉预报()
        {
            try
            {
                //后续新增人员选择窗口
                花粉预报 ryxz = new 花粉预报();
                ryxz.Show();


            }
            catch (Exception)
            {
            }
        }
        private void 制作市四区预报()
        {
            预报人员选择2 ryxz = new 预报人员选择2();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();


                        settingsDialog.Content = new 进度条_实况订正("生成市四区精细化预报产品") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.FBCom.Text, inputStr3 = ryxz.QFCom.Text };
                        settingsDialog.Closed += 产品生成窗口关闭错误仍旧打开;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 制作社区预报()
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == true)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow settingsDialog = new RadWindow();


                        settingsDialog.Content = new 进度条_实况订正("生成社区精细化预报") { inputStr1 = ryxz.ZBCom.Text, inputStr2 = ryxz.QFCom.Text };
                        settingsDialog.Closed += 产品生成窗口关闭;
                        settingsDialog.ResizeMode = ResizeMode.CanResize;
                        settingsDialog.Header = "正在处理数据";
                        settingsDialog.Owner = Application.Current.MainWindow;
                        settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        settingsDialog.HideMinimizeButton = true;
                        settingsDialog.HideMaximizeButton = true;
                        settingsDialog.CanClose = false;
                        settingsDialog.ShowDialog();
                    });
                }

            }
        }
        private void 产品生成窗口关闭(object sender, EventArgs e)
        {
            try
            {
                进度条_实况订正 mydata = ((sender as RadWindow).Content as 进度条_实况订正);
                _path = mydata.myPath;
                if (mydata.strError.Length > 0)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = mydata.strError,
                        Owner = Application.Current.MainWindow,
                        OkButtonContent = "是",
                        Header = "警告"
                    });
                    return;
                }
                if (_path.Length == 0)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = "预报产品生成异常",
                        Owner = Application.Current.MainWindow,
                        OkButtonContent = "是",
                        Header = "警告"
                    });
                    return;
                }
                RadWindow.Confirm(new DialogParameters
                {
                    Content = "产品发布完成, 保存路径为：\r\n" + _path + "\r\n是否需要打开产品？",
                    Closed = OnConfirmClosed_打开乡镇精细化产品,
                    Owner = Application.Current.MainWindow,
                    CancelButtonContent = "否",
                    OkButtonContent = "是",
                    Header = "提示"
                });


            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }
        }
        private void 产品生成窗口关闭错误仍旧打开(object sender, EventArgs e)
        {
            try
            {
                进度条_实况订正 mydata = ((sender as RadWindow).Content as 进度条_实况订正);
                _path = mydata.myPath;
                if (mydata.strError.Length > 0)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = mydata.strError,
                        Owner = Application.Current.MainWindow,
                        OkButtonContent = "是",
                        Header = "警告"
                    });
                }
                if (_path.Length == 0)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = "预报产品生成异常",
                        Owner = Application.Current.MainWindow,
                        OkButtonContent = "是",
                        Header = "警告"
                    });
                    return;
                }
                RadWindow.Confirm(new DialogParameters
                {
                    Content = "产品发布完成, 保存路径为：\r\n" + _path + "\r\n是否需要打开产品？",
                    Closed = OnConfirmClosed_打开乡镇精细化产品,
                    Owner = Application.Current.MainWindow,
                    CancelButtonContent = "否",
                    OkButtonContent = "是",
                    Header = "提示"
                });


            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }
        }
        #endregion
        #region 乡镇精细化
        private void 实况订正指导预报制作乡镇精细化预报()
        {
            Thread thread = new Thread(实况订正new);
            thread.Start();
        }
        private void 省级智能网格制作乡镇精细化预报()
        {
            Thread thread = new Thread(区局智能网格生成乡镇精细化);
            thread.Start();
        }
        private void 制作乡镇精细化发布单()
        {
            Thread thread = new Thread(生成乡镇精细化预报产品);
            thread.Start();
        }
        public void 区局智能网格生成乡镇精细化()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();


                settingsDialog.Content = new 进度条_实况订正("区局智能网格生成乡镇精细化");
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.Closed += 区局智能网格生成乡镇精细化窗口关闭;
                settingsDialog.ShowDialog();
            });
        }
        private void 区局智能网格生成乡镇精细化窗口关闭(object sender, EventArgs e)
        {
            try
            {
                进度条_实况订正 mydata = ((sender as RadWindow).Content as 进度条_实况订正);
                string[,] szYB = mydata.szYB;
                string strError = mydata.strError;
                if (strError.Length > 0)
                {
                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.Content = strError;
                    RadWindow radWindow = new RadWindow()
                    {
                        Content = scrollViewer,
                        ResizeMode = ResizeMode.CanResize,
                        MaxHeight = 500,
                        MinHeight = 200,
                        MinWidth = 400,
                        Header = "警告",
                        ShouldUpdateActiveState = false,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                    radWindow.ShowDialog();
                }
                else
                {
                    ZDSZCL zdszcl = new ZDSZCL();
                    zdszcl.ZDSZ2BW(szYB);
                    RadWindow.Confirm(new DialogParameters
                    {
                        Content = "已根据区局智能网格，是否需要修改。\r\n如果选是，则自动进入发报软件。\r\n如果选否，则直接生成最终的指导预报报文与产品清单。",
                        Closed = OnConfirmClosed_生成乡镇精细化产品,
                        Owner = Application.Current.MainWindow,
                        CancelButtonContent = "否",
                        Header = "提示",
                        OkButtonContent = "是"
                    });
                }



            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = "产品生成异常\r\n" + ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }
        }
        public void 实况订正new()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();


                settingsDialog.Content = new 进度条_实况订正("开始实况订正");
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.Closed += 实况订正进度条窗口关闭;
                settingsDialog.ShowDialog();
            });

        }
        private void 实况订正进度条窗口关闭(object sender, EventArgs e)
        {
            try
            {
                进度条_实况订正 mydata = ((sender as RadWindow).Content as 进度条_实况订正);
                string[,] szYB = mydata.szYB;
                string strError = mydata.strError;
                if (strError.Length > 0)
                {
                    ScrollViewer scrollViewer = new ScrollViewer();
                    scrollViewer.Content = strError;
                    RadWindow radWindow = new RadWindow()
                    {
                        Content = scrollViewer,
                        ResizeMode = ResizeMode.CanResize,
                        MaxHeight = 500,
                        MinWidth = 400,
                        Header = "警告",
                        ShouldUpdateActiveState = false
                    };
                    RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                    radWindow.Show();
                }
                ZDSZCL zdszcl = new ZDSZCL();
                zdszcl.ZDSZ2BW(szYB);
                RadWindow.Confirm(new DialogParameters
                {
                    Content = "已根据实况与指导预报生成乡镇精细化预报报文，是否需要修改。\r\n如果选是，则自动进入发报软件。\r\n如果选否，则直接生成最终的指导预报报文与产品清单。",
                    Closed = OnConfirmClosed_生成乡镇精细化产品,
                    Owner = Application.Current.MainWindow,
                    CancelButtonContent = "否",
                    Header = "提示",
                    OkButtonContent = "是"
                });


            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }
        }

        private void OnConfirmClosed_生成乡镇精细化产品(object sender, WindowClosedEventArgs e)
        {

            if (e.DialogResult == true)
            {
                string FBPath = "";
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "CITYFORECAST路径")
                        {
                            FBPath = line.Split('=')[1];
                        }
                    }
                }
                静态类.OpenBrowser(FBPath);
                //p.WaitForExit();//关键，等待外部程序退出后才能往下执行
            }
            else
            {
                Thread thread = new Thread(生成乡镇精细化预报产品);
                thread.Start();
            }
        }
        private void OnConfirmClosed_打开乡镇精细化产品(object sender, WindowClosedEventArgs e)
        {

            if (e.DialogResult == true)
            {
                静态类.OpenBrowser(_path);
            }

        }
        public void 生成乡镇精细化预报产品()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();


                settingsDialog.Content = new 进度条_实况订正("生成乡镇精细化预报产品");
                settingsDialog.Closed += 乡镇精细化预报产品生成窗口关闭;
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.ShowDialog();
            });
        }
        private void 乡镇精细化预报产品生成窗口关闭(object sender, EventArgs e)
        {
            try
            {
                进度条_实况订正 mydata = ((sender as RadWindow).Content as 进度条_实况订正);
                _path = mydata.myPath;
                if (_path.Length == 0)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = "乡镇精细化预报生成异常",
                        Owner = Application.Current.MainWindow,
                        OkButtonContent = "是",
                        Header = "警告"
                    });
                    return;
                }
                RadWindow.Confirm(new DialogParameters
                {
                    Content = "产品发布完成, 保存路径为：\r\n" + _path + "\r\n是否需要打开产品？",
                    Closed = OnConfirmClosed_打开乡镇精细化产品,
                    Owner = Application.Current.MainWindow,
                    CancelButtonContent = "否",
                    Header = "提示",
                    OkButtonContent = "是"
                });


            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }
        }

        private void 制作发布单_Click(object sender, RadRoutedEventArgs e)
        {
            Thread thread = new Thread(生成乡镇精细化预报产品);
            thread.Start();
        }
        #endregion
        #region 非日常产品
        private void 环保局专报08时()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();
                settingsDialog.Content = new 进度条_实况订正("生成环保局预报产品") { inputInt = 8 };
                settingsDialog.Closed += 产品生成窗口关闭;
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.ShowDialog();
            });
        }
        private void 环保局专报20时()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();


                settingsDialog.Content = new 进度条_实况订正("生成环保局预报产品") { inputInt = 20 };
                settingsDialog.Closed += 产品生成窗口关闭;
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在处理数据";
                settingsDialog.Owner = Application.Current.MainWindow;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = true;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.ShowDialog();
            });
        }
        private void 蒙草预报服务()
        {
            蒙草预报产品 mymc = new 蒙草预报产品();
            mymc.Show();
        }
        #endregion
        #region 其他产品
        private void 生态与农业()
        {
            生态选择1 stwin = new 生态选择1();
            stwin.Show();
        }
        private void 科开预报导出()
        {
            科开服务窗口 ZQ = new 科开服务窗口();
            ZQ.Show();
        }
        private void 赛罕智能网格()
        {
            try
            {
                //后续新增人员选择窗口
                赛罕智能网格选择 ryxz = new 赛罕智能网格选择();
                if (ryxz.ShowDialog() == true)
                {
                    if (ryxz.ZBCom.Text.Trim().Length > 0)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            RadWindow settingsDialog = new RadWindow();
                            settingsDialog.Content = new 进度条_实况订正("生成赛罕智能网格产品") { inputStr1 = ryxz.ZBCom.Text, inputInt = Convert.ToInt16(ryxz.LXCom.Text) };
                            settingsDialog.Closed += 产品生成窗口关闭;
                            settingsDialog.ResizeMode = ResizeMode.CanResize;
                            settingsDialog.Header = "正在处理数据";
                            settingsDialog.Owner = Application.Current.MainWindow;
                            settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                            settingsDialog.HideMinimizeButton = true;
                            settingsDialog.HideMaximizeButton = true;
                            settingsDialog.CanClose = false;
                            settingsDialog.ShowDialog();
                        });
                    }
                }


            }
            catch (Exception)
            {
            }
        }
        #endregion
    }
}

