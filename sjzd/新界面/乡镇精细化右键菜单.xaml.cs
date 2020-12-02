using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.RadialMenu;

namespace sjzd
{
    //图标颜色 橙色：27c542  蓝色 ：  1488D8
    /// <summary>
    /// 设置菜单.xaml 的交互逻辑
    /// </summary>
    public partial class 乡镇精细化右键菜单 : RadRadialMenu
    {
        string strTheme = "";
        string _path = "";
        public 乡镇精细化右键菜单()
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

        private void 实况订正指导预报_Click(object sender, RadRoutedEventArgs e)
        {
            Thread thread = new Thread(实况订正new);
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

        private void 省级智能网格计算_Click(object sender, RadRoutedEventArgs e)
        {
            Thread thread = new Thread(区局智能网格生成乡镇精细化);
            thread.Start();
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

        private void 制作社区预报_Click(object sender, RadRoutedEventArgs e)
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

        private void 制作市四区预报_Click(object sender, RadRoutedEventArgs e)
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

        private void 短时08_Click(object sender, RadRoutedEventArgs e)
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

        private void 短时14_Click(object sender, RadRoutedEventArgs e)
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

        private void 短时20_Click(object sender, RadRoutedEventArgs e)
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

        private void 短期预报08_Click(object sender, RadRoutedEventArgs e)
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

        private void 短期预报20_Click(object sender, RadRoutedEventArgs e)
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

        private void 逐3小时08_Click(object sender, RadRoutedEventArgs e)
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

        private void 逐3小时20_Click(object sender, RadRoutedEventArgs e)
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

        private void 制作中期逐日预报_Click(object sender, RadRoutedEventArgs e)
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

        private void 环保局08_Click(object sender, RadRoutedEventArgs e)
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

        private void 环保局20_Click(object sender, RadRoutedEventArgs e)
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

        private void 生态与农业_Click(object sender, RadRoutedEventArgs e)
        {
            生态选择1 stwin = new 生态选择1();
            stwin.Show();
        }

        private void 科开预报导出_Click(object sender, RadRoutedEventArgs e)
        {
            科开服务窗口 ZQ = new 科开服务窗口();
            ZQ.Show();
        }

        private void 增加旗县_Click(object sender, RadRoutedEventArgs e)
        {
            WPFAddQX windowAddQx = new WPFAddQX();
            windowAddQx.Show();
        }

        private void 修改旗县_Click(object sender, RadRoutedEventArgs e)
        {
            WPFChangeQX windowChangeQx = new WPFChangeQX();
            windowChangeQx.Show();
        }

        private void 增加乡镇_Click(object sender, RadRoutedEventArgs e)
        {
            WPFAddXZ windowAddXZ = new WPFAddXZ();
            windowAddXZ.Show();
        }

        private void 人员新增_Click(object sender, RadRoutedEventArgs e)
        {
            人员管理页 rygl = new 人员管理页();
            rygl.Show();
        }

        private void 人员修改_Click(object sender, RadRoutedEventArgs e)
        {
            人员修改 ryxg = new 人员修改();
            ryxg.Show();
        }

        private void 设置同步_Click(object sender, RadRoutedEventArgs e)
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
                    configClass1.TBBD();
                }

            }
            catch
            {

            }
        }
        private void 其他设置_Click(object sender, RadRoutedEventArgs e)
        {
            其他设置 qtWindow = new 其他设置();
            qtWindow.Show();
        }

        private void 修改乡镇_Click(object sender, RadRoutedEventArgs e)
        {
            WPFChangeXZ windowChangeXz = new WPFChangeXZ();
            windowChangeXz.Show();
        }

        private void 测试_Click(object sender, RadRoutedEventArgs e)
        {
            TabWindow1 tabWindow1 = new TabWindow1();
            tabWindow1.Show();
        }

        private void 制作防凌预报_Click(object sender, RadRoutedEventArgs e)
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

        private void 查询统计_Click(object sender, RadRoutedEventArgs e)
        {
            TabWindow查询统计 tabWindow1 = new TabWindow查询统计();
            tabWindow1.Show();
        }

        private void 赛罕智能网格_Click(object sender, RadRoutedEventArgs e)
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

        private void 制作花粉预报_Click(object sender, RadRoutedEventArgs e)
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

        private void 蒙草预报服务_OnClick(object sender, RadRoutedEventArgs e)
        {
            蒙草预报产品 mymc = new 蒙草预报产品();
            mymc.Show();


        }

        private void 区台新方法温度_Click(object sender, RadRoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();
                settingsDialog.Content = new 进度条_实况订正("区台新方法生成乡镇精细化");
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
    }
}
