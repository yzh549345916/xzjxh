using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;

namespace 旗县端
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Thread t1 = new Thread(CheckUpdate);
            t1.Start();
            Thread t2 = new Thread(UpdateSJ);
            t2.Start();
        }

        private void BUFB_Click(object sender, RoutedEventArgs e)
        {
            登录窗口 dlwindow = new 登录窗口();
            dlwindow.Show();

        }

        private void UpdataBu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("更新将关闭当前客户端，请确认是否继续", "请注意", MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {
                try
                {
                    string updatePath = Environment.CurrentDirectory + @"\QX-update.exe";
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
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void JYBu_Click(object sender, RoutedEventArgs e)
        {
            预报检验窗口 YBJYwindow = new 预报检验窗口();
            YBJYwindow.Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            旗县设置窗口 qxszWindow = new 旗县设置窗口();
            qxszWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                区局智能网格 znwg = new 区局智能网格();
                string error = "";
                string data = znwg.CIMISS_ZDbyID("53368", ref error);
            }
            catch (Exception)
            {

            }
        }

        public void CheckUpdate()
        {
            try
            {
                string updateText = "";
                string versionPath = System.Environment.CurrentDirectory + @"\设置文件\version.txt";//改
                int myVersion = 0;

                string GLVersion = "";
                using (StreamReader sr = new StreamReader(versionPath, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        try
                        {
                            if (line.Split('=')[0] == "myVersion")
                            {
                                myVersion = Convert.ToInt32(line.Trim().Split('=')[1]);
                            }
                            else if (line.Split('=')[0] == "GLVersion")
                            {
                                GLVersion = line.Trim().Split('=')[1];
                            }
                        }
                        catch
                        { }
                    }
                }
                string con = "";//这里是保存连接数据库的字符串
                string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";//改

                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
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
                int versionLast = 0;//保存数据库中最新的版本号
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from UpdateVersion where family='旗县' order by version desc");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    sqlreader.Read();
                    versionLast = sqlreader.GetInt32(sqlreader.GetOrdinal("version"));
                    updateText = sqlreader.GetString(sqlreader.GetOrdinal("versionAbout")) + "\r\n";
                    try
                    {
                        string[] szls = updateText.Split(';');
                        updateText = "";
                        foreach (string ss in szls)
                        {
                            updateText += ss + '\n';
                        }
                        updateText = updateText.Substring(0, updateText.Length - 1);
                    }
                    catch { }
                }
                //当前版本是否被忽略
                bool ISChek = true;
                foreach (string v in GLVersion.Split(','))
                {
                    try
                    {
                        int vint = Convert.ToInt32(v);
                        if (vint == versionLast)
                            ISChek = false;
                    }
                    catch { }
                }
                //版本比较
                if (ISChek && myVersion < versionLast)
                {

                    MessageBoxResult dr = System.Windows.MessageBox.Show("检查到有新的版本，更新内容有：\n" + updateText + "是否更新", "更新提示", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        //进行更新，此处修改更新程序，函数增加版本号变量，主程序检查是否包含新的更新程序
                        string data = "";
                        //保存最新版本信息，更新程序获取
                        try
                        {
                            using (StreamReader sr = new StreamReader(versionPath, Encoding.Default))
                            {
                                string line;
                                while ((line = sr.ReadLine()) != null)
                                {
                                    try
                                    {
                                        if (line.Split('=')[0] == "myLastVersion")
                                        {
                                        }
                                        else
                                        {
                                            data += line + "\r\n";
                                        }
                                    }
                                    catch
                                    { }
                                }
                            }
                            data += "myLastVersion=" + versionLast.ToString();
                            using (FileStream fs = new FileStream(versionPath, FileMode.Create))
                            {
                                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                                sw.Write(data);
                                sw.Flush();
                                sw.Close();
                            }
                            try
                            {
                                string updatePath = Environment.CurrentDirectory + @"\QX-update.exe";
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
                                MessageBox.Show(ex.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else if (dr == MessageBoxResult.No)
                    {
                        MessageBoxResult dr1 = System.Windows.MessageBox.Show("已经取消更新，是否以后忽略该版本的更新提示", "更新提示", MessageBoxButton.YesNo);
                        if (dr1 == MessageBoxResult.Yes)
                        {
                            if (GLVersion.Length == 0)
                            {
                                GLVersion = "GLVersion=" + versionLast.ToString();
                            }
                            else
                            {
                                GLVersion = "GLVersion=" + GLVersion + ',' + versionLast.ToString();
                                string data = "";
                                try
                                {
                                    using (StreamReader sr = new StreamReader(versionPath, Encoding.Default))
                                    {
                                        string line;
                                        while ((line = sr.ReadLine()) != null)
                                        {
                                            try
                                            {
                                                if (line.Split('=')[0] == "GLVersion")
                                                {

                                                }
                                                else
                                                {
                                                    data += line + "\r\n";
                                                }
                                            }
                                            catch
                                            { }
                                        }
                                    }
                                    data += GLVersion;
                                    using (FileStream fs = new FileStream(versionPath, FileMode.Create))
                                    {
                                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                                        sw.Write(data);
                                        sw.Flush();
                                        sw.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 更新升级文件，仅在需要升级更新程序时候需要
        /// </summary>
        public void UpdateSJ()
        {
            string path = @"\\172.18.142.167\sevp\更新程序升级\";
            DirectoryInfo dir = new DirectoryInfo(Environment.CurrentDirectory);
            FileSystemInfo[] fileinfo = dir.GetFiles("*QX-update.exe");  //获取目录下（不包含子目录）的文件
            if (fileinfo.Length > 0)
            {
                if (DateTime.Compare(fileinfo[0].LastWriteTime, Convert.ToDateTime("2019-02-20")) >= 0)
                {
                    return;
                }
            }
            CopyDirectory(path, Environment.CurrentDirectory);
        }

        public static bool CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (!Directory.Exists(destPath + "\\" + i.Name))
                        {
                            Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                        }
                        CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
