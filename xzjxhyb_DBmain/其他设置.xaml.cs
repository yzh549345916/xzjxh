using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// WPFAddQX.xaml 的交互逻辑
    /// </summary>
    public partial class 其他设置 : Window
    {
        string con = "";
        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
        string PathConfigPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        private string data = "";
        public 其他设置()
        {
            InitializeComponent();
            CSH();

        }

        public void CSH()
        {
            string sqlAdmin = "", sqlQX = "", FBRJPath = "";

            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                        try
                        {
                            sqlAdmin = line.Split(';')[0].Split('=')[2];
                        }
                        catch
                        {
                        }
                    }
                    else if (line.Contains("sql旗县"))
                    {
                        try
                        {
                            sqlQX = line.Split(';')[0].Split('=')[2];
                        }
                        catch
                        {
                        }
                    }
                }
            }
            using (StreamReader sr = new StreamReader(PathConfigPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "CITYFORECAST路径")
                    {
                        try
                        {
                            FBRJPath = line.Split('=')[1];
                        }
                        catch
                        {
                        }
                    }
                }
            }
            //本地设置
            string dataLS = String.Format("sql管理员={0}=DBconfig\nsql旗县={1}=DBconfig\nCITYFORECAST路径={2}=路径设置\n", sqlAdmin, sqlQX, FBRJPath);
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            int intCount = 0;
            for (; intCount < 3; intCount++)
            {
                mydic.Add(intCount, dataLS.Split('\n')[intCount].Split('=')[0]);

            }

            //数据库设置
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    string sql =
                        string.Format(
                            @"select * from OtherConfig order by filename,name"); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();



                    while (sqlreader.Read())
                    {
                        string strls = sqlreader.GetString(sqlreader.GetOrdinal("name"));
                        mydic.Add(intCount++, strls);
                        dataLS += strls + '=' +
                                  sqlreader.GetString(sqlreader.GetOrdinal("data")) + '=' + sqlreader.GetString(sqlreader.GetOrdinal("filename")) + '\n';
                    }



                }

            }
            catch
            {
            }
            //添加帮助信息
            try
            {
                data = dataLS.Substring(0, dataLS.Length - 1);
                dataLS = "";
                foreach (string s1 in data.Split('\n'))
                {
                    string help = "";
                    string[] szls = s1.Split('=');
                    switch (szls[0])
                    {
                        case "sql管理员":
                            help = "输入sql服务器地址，如果为本地则输入“.”";
                            break;
                        case "sql旗县":
                            help = "输入sql服务器地址，如果为本地则输入“.”";
                            break;
                        case "实况入库小时":
                            help = "设置每天实况入库时间的小时";
                            break;
                        case "实况入库分钟":
                            help = "设置每天实况入库时间的分钟";
                            break;
                        case "实况时次":
                            help = "设置每天实况入库的时次";
                            break;
                        case "旗县预报入库小时":
                            help = "设置每天旗县预报入库时间的小时";
                            break;
                        case "旗县预报入库分钟":
                            help = "设置每天旗县预报入库时间的分钟";
                            break;
                        case "订正市局指导实况时次":
                            help = "订正市局指导实况时次是指订正市局指导预报时用的实况的时次，应该早于每天发报的时间";
                            break;
                        case "市局预报入库小时":
                            help = "设置每天市局预报入库时间的小时";
                            break;
                        case "市局预报入库分钟":
                            help = "设置每天市局预报入库时间的分钟";
                            break;
                        case "旗县逾期小时":
                            help = "设置旗县逾期时间的小时";
                            break;
                        case "旗县逾期分钟":
                            help = "设置旗县逾期时间的分钟";
                            break;
                        case "市局初始报文保存地址":
                            help = "该地址名称为中央指导预报命名格式，为了发报软件读取自动生成但未订正的预报报文";
                            break;
                        case "市局指导预报报文保存地址":
                            help = "该地址为市局对旗县下发的订正后的指导预报报文路径";
                            break;
                        case "市局预报读取指导预报时次":
                            help = "该条主要是发报软件选择的时次，如果选择的时次为20，则该处相应的设置为20";
                            break;
                        case "市局读取城镇指导预报文件夹时次":
                            help = "该处为市局自己城镇预报的时次，为了自动计算乡镇精细化预报，一般只有06和20两个文件夹";
                            break;
                        default:
                            help = "";
                            break;


                    }
                    dataLS += szls[0] + '=' +
                              szls[1] + '=' + help + '=' + szls[2] + '\n';
                }
            }
            catch
            {
            }
            try
            {
                NameSelect.SelectionChanged -= QXList_SelectionChanged;
                data = dataLS.Substring(0, dataLS.Length - 1);
                NameSelect.ItemsSource = mydic;
                NameSelect.SelectedValuePath = "Key";
                NameSelect.DisplayMemberPath = "Value";
                NameSelect.SelectionChanged += QXList_SelectionChanged;
                NameSelect.SelectedValue = 0;
            }
            catch
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                string strName = NameSelect.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim();
                if (strName.Trim() == "sql管理员" || strName.Trim() == "sql旗县")
                {
                    XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
                    util.Write("Server=" + ContentText.Text.Trim() + ";Database=智能网格;user id=sa;password=134679;", "OtherConfig", "DB");
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
                                    strData += "sql管理员=Server=" + ContentText.Text.Trim() +
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
                                    strData += "sql旗县=Server=" + ContentText.Text.Trim() +
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
                    MessageBox.Show("配置信息修改成功");
                }
                else if (strName.Trim() == "CITYFORECAST路径")
                {

                    TBBD("CITYFORECAST路径", ContentText.Text, PathConfigPath);
                    MessageBox.Show("配置信息修改成功");
                }
                else
                {
                    foreach (string s1 in data.Split('\n'))
                    {
                        string[] szls = s1.Split('=');
                        if (szls[0].Trim() == strName)
                        {
                            ConfigClass1 configClass1 = new ConfigClass1();
                            if (configClass1.OtherCon(strName, ContentText.Text))
                            {
                                MessageBox.Show("配置信息修改成功");
                            }
                            TBBD(strName, ContentText.Text, System.Environment.CurrentDirectory + @"\设置文件\" + szls[3].Trim() + ".txt");
                            break;
                        }
                    }
                }

                CSH();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }

        private void QXList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                string strName = NameSelect.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim();
                foreach (string s1 in data.Split('\n'))
                {
                    string[] szls = s1.Split('=');
                    if (szls[0].Trim() == strName)
                    {
                        ContentText.Text = szls[1].Trim();
                        HelpText.Text = szls[2];
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 保存修改的设置到本地
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="sx">修改后的属性内容</param>
        /// <param name="path">文件路径</param>
        public void TBBD(string name, string sx, string path)
        {
            try
            {
                if (!File.Exists(path))
                    File.Create(path);
                string strData = "";
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == name)
                        {
                            try
                            {
                                strData += name + '=' + sx + "\r\n";
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

                strData = strData.Substring(0, strData.Length - 2);
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
                {
                    sw.Write(strData);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TBBtu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    string sql =
                        string.Format(
                            @"select * from OtherConfig order by filename,name"); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        string name = sqlreader.GetString(sqlreader.GetOrdinal("name"));
                        string data = sqlreader.GetString(sqlreader.GetOrdinal("data"));
                        string filename = sqlreader.GetString(sqlreader.GetOrdinal("filename"));
                        TBBD(name, data, Environment.CurrentDirectory + @"\设置文件\" + filename.Trim() + ".txt");
                        Thread.Sleep(500);
                    }



                }

                MessageBox.Show("保存成功");

            }
            catch (Exception ex)
            {
                MessageBox.Show("同步失败，原因为\n" + ex.Message);
            }
        }
    }
}
