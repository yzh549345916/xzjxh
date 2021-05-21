using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using sjzd.新界面.类;
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

            try
            {
                Thread t1 = new Thread(CheckUpdate);
                t1.Start();
            }
            catch
            {
            }

            //统计临时 ls = new 统计临时();
            //ls.市局预报处理();
            /*DateTime sdate = Convert.ToDateTime("2021-03-02");
            DateTime edate = Convert.ToDateTime("2021-03-12");
            string ss = "";
            for (DateTime dtls = sdate; dtls.CompareTo(edate) <= 0; dtls = dtls.AddDays(1))
            {
                ss+= ls.TJRK(dtls, "08", "主班");
            }*/
           
        }

        private void navigationView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
       private long versionLast = 0;//保存数据库中最新的版本号
       string versionList = "";
        public void CheckUpdate()
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                util.Write("", "version", "versionList");
                string updateText = "";
                long myVersion = Convert.ToInt64(util.Read("version", "myVersion")); ;
                //string GLVersion = util.Read("version", "GLVersion"); 

                string con = "";//这里是保存连接数据库的字符串
                string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";//改

                using (StreamReader sr = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("sql管理员"))
                        {
                            con = line.Substring("sql管理员=".Length);
                            break;
                        }
                    }
                }

                versionList = "";
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@$"select * from UpdateVersion_SJ where family='市局'and version>{myVersion} order by version");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        versionLast = sqlreader.GetInt64(sqlreader.GetOrdinal("version"));
                        versionList += versionLast.ToString() + ',';
                        string updateTextLS = sqlreader.GetString(sqlreader.GetOrdinal("versionAbout")) + "\r\n";
                        try
                        {
                            string[] szls = updateTextLS.Split(';');
                            updateTextLS = "";
                            foreach (string ss in szls)
                            {
                                updateTextLS += ss + '\n';
                            }
                            updateTextLS = updateTextLS.Substring(0, updateTextLS.Length - 1);
                        }
                        catch { }
                        updateText += versionLast+":"+updateTextLS;
                    }

                    if (versionList.Length > 0)
                    {
                        versionList = versionList.Substring(0, versionList.Length - 1);
                    }
                   
                }
                //当前版本是否被忽略
                /*bool ISChek = true;
                foreach (string v in GLVersion.Split(','))
                {
                    try
                    {
                        int vint = Convert.ToInt32(v);
                        if (vint == versionLast)
                            ISChek = false;
                    }
                    catch { }
                }*/
                //版本比较
                if ( myVersion < versionLast)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "检查到有新的版本，更新内容有：\n" + updateText + "请更新",
                            Closed = 开始更新,
                            Owner = Application.Current.MainWindow,
                            Header = "更新提示",
                            OkButtonContent = "更新"
                        });
                    });
                   

                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Alert(ex.Message);
                });
               
            }
        }

        private void 开始更新(object sender, WindowClosedEventArgs e)
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                util.Write(versionLast.ToString(), "version", "myLastVersion");
                util.Write(versionList, "version", "versionList");
                try
                {
                    string updatePath = Environment.CurrentDirectory + @"\市台更新.exe";
                    string strML = Environment.CurrentDirectory;
                    Process pr = new Process();//声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = updatePath;//指定运行的程序
                    pr.Start();//运行
                    System.Environment.Exit(0);
                    this.Close();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Alert(ex.Message);
                    });
                }
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                {
                    Alert(ex.Message);
                });
            }
        }
    }
}