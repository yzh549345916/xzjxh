using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace 旗县端
{
    /// <summary>
    /// 旗县设置窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 旗县设置窗口 : Window
    {
        string configPath = System.Environment.CurrentDirectory + @"\config\QXList.txt";
        public 旗县设置窗口()
        {
            string DQID = "";
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            InitializeComponent();
            
            using (StreamReader sr = new StreamReader(configPath, Encoding.Default))
            {
                string line = "";
                while((line=sr.ReadLine())!=null)
                {
                    if (line.Split('=')[0] == "当前旗县ID")
                        DQID = line.Split('=')[1];
                    else if(line.Split('=')[0] == "旗县ID列表")
                    {
                        string[] strsz = (line.Split('=')[1]).Split(',');
                        for(int i=0;i<strsz.Length;i++)
                        {
                            mydic.Add(i, strsz[i]);
                        }
                    }
                }
            }
            textBoxQXID.Text = DQID;
            ComBox1.ItemsSource = mydic;
            ComBox1.SelectedValuePath = "Key";
            ComBox1.DisplayMemberPath = "Value";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        try
                        {
                            sqlText.Text = line.Split(';')[0].Split('=')[2];
                            break;
                        }
                        catch
                        {
                        }
                    }

                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string DQID = "";
            string strData = "";
            if(ComBox1.SelectedIndex==-1)
            {
                MessageBox.Show("请选择需要设置的站号");
            }
            else
            {
                using (StreamReader sr = new StreamReader(configPath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "当前旗县ID")
                        {
                            strData += "当前旗县ID" + '=' + ComBox1.Text + "\r\n";
                            DQID = ComBox1.Text;
                        }
                            
                         else
                         {
                             strData += line;
                         }
                    }
                }
                using (FileStream fs = new FileStream(configPath, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(strData);
                    sw.Flush();
                    sw.Close();
                }
                MessageBox.Show("已将当前旗县更改为" + DQID);
                ConfigClass1 configClass1 = new ConfigClass1();
                configClass1.FBRJTBID(DQID);
                MainWindow dlck = new MainWindow();
                dlck.Show();
                this.Close();
            }
        }

        private void FBPathConfig_Click(object sender, RoutedEventArgs e)
        {
            string YBpath = "";
            string FBConfigPath = System.Environment.CurrentDirectory + @"\config\YBpath.txt";
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "可执行文件|*"
                };
                var result = openFileDialog.ShowDialog();
                if(result==true)
                {
                    YBpath= "发报软件路径="+ openFileDialog.FileName;
                    using (FileStream fs = new FileStream(FBConfigPath, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                        sw.Write(YBpath);
                        sw.Flush();
                        sw.Close();
                    }
                    MessageBox.Show("已将发报系统的路径设置为" + openFileDialog.FileName);
                }

                    
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MainWindow dlck = new MainWindow();
            dlck.Show();
            this.Close();
        }

        private void ConBtu_Click(object sender, RoutedEventArgs e)
        {
            
            管理员登陆 gly = new 管理员登陆();
            gly.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string StrSql = "";
            if (sqlText.Text.Trim().Length == 0)
                StrSql = ".";
            else
                StrSql = sqlText.Text.Trim();
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string strData = "";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        try
                        {
                            strData += "sql管理员=Server=" + StrSql +
                                       ";Database=xzjxhyb_DB;user id=sa;password=134679;\r\n";
                        }
                        catch
                        {
                        }
                    }
                    else if (line.Contains("sql旗县"))
                    {
                        try
                        {
                            strData += "sql旗县=Server=" + StrSql +
                                       ";Database=xzjxhyb_DB;user id=sa;password=134679;\r\n";
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        strData += line + "\r\n";
                    }
                }
            }
            using (StreamWriter sw = new StreamWriter(DBconPath, false, Encoding.Default))
            {
                sw.Write(strData);
                sw.Flush();
            }

            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
                util.Write("Server=" + StrSql.Trim() + ";Database=智能网格;user id=sa;password=134679;", "OtherConfig", "DB");
            }
            catch
            {
            }

            MessageBox.Show("数据库服务器修改成功");
        }
    }
}
