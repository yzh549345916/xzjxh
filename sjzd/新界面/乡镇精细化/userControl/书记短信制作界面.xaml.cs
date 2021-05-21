using sjzd.类;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 书记短信制作界面 : RadWindow
    {
        private string _path = "";
        public 书记短信制作界面()
        {
            InitializeComponent();
          
        }

        string 获取产品路径()
        {
            string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            string SJsaPath = "";
            using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "书记短信发布路径")
                    {
                        SJsaPath = line.Split('=')[1];
                    }
                }
            }
            SJsaPath +=  DateTime.Now.ToString("yyyy-MM") + "\\";
            SJsaPath += "书记短信" + DateTime.Now.ToString("yyyyMMdd") + ".docx";
            return SJsaPath;
        }
        private void ZhiZuo_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();
                settingsDialog.Content = new 进度条_实况订正("生成书记短信");
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
        private void OnConfirmClosed_打开乡镇精细化产品(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                静态类.OpenBrowser(_path);
            }
        }

        private void open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string myPath = 获取产品路径();
                if (File.Exists(myPath))
                {
                    静态类.OpenBrowser(myPath);
                }
                else
                {
                    Alert("产品不存在");
                }
            }
            catch
            {
            }
        }

        private void send_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string myPath = 获取产品路径();
                if (File.Exists(myPath))
                {
                    //同步至本地
                    string pathLS = Environment.CurrentDirectory + @"\临时\";
                    if (!File.Exists(pathLS))
                    {
                        Directory.CreateDirectory(pathLS);
                    }
                    pathLS+= "书记短信" + DateTime.Now.ToString("yyyyMMdd") + ".docx";
                    try
                    {
                        File.Copy(myPath, pathLS, true);
                    }
                    catch
                    {
                    }
                    if (File.Exists(pathLS))
                    {
                        邮件发送 emailSend = new 邮件发送();
                        string error = "";
                        emailSend.Email书记短信(pathLS, $"书记短信{DateTime.Now:yyyyMMdd}", "", ref error);
                        if (error.Contains("成功"))
                        {
                            RadWindow.Alert(new DialogParameters
                            {
                                Content = error,
                                Header = "成功",
                            }); ;
                            try
                            {
                                File.Delete(pathLS);
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            RadWindow.Alert(new DialogParameters
                            {
                                Content = error,
                                Header = "发送失败"
                            }); ;
                        }
                    }
                }
                else
                {
                    Alert("产品不存在");
                }
            }
            catch(Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "发送失败"
                }); ;
            }
        }

    }
}
