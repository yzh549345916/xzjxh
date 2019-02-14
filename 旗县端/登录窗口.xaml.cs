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
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace 旗县端
{
    /// <summary>
    /// 登录窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 登录窗口 : Window
    {
        bool SQLSuc=true;
        ObservableCollection<people> peopleList = new ObservableCollection<people>();
        int intCount = 0;//记录当前旗县人员个数
        string con;//这里是保存连接数据库的字符串
        string configPath = System.Environment.CurrentDirectory + @"\config\QXList.txt";
        string DQID = "";
        string basePath = System.Environment.CurrentDirectory + @"\config\user";


        public 登录窗口()
        {
            InitializeComponent();

            using (StreamReader sr = new StreamReader(configPath, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "当前旗县ID")
                        DQID = line.Split('=')[1];
                }
            }

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
                    }
                }
            }
            HQUserID(ref SQLSuc);
            if(SQLSuc)
            {
                HQDL();
            }
            

        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            旗县设置窗口 qxszWindow = new 旗县设置窗口();
            qxszWindow.Show();
            this.Close();
        }

        private void HQUserID(ref bool SQLSuc)
        {
            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQID + ".txt";
            string ss = "";
            SQLSuc = true;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from USERID where QXID='{0}'", DQID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("admin")) + '\n';
                    }
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    using (FileStream fs = new FileStream(idPath, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                        sw.Write(ss);
                        sw.Flush();
                        sw.Close();
                    }

                }
                catch (Exception ex)
                {
                    SQLSuc = false;
                    MessageBox.Show(ex.Message + "\n如果错误为数据库连接失败将连接本地人员名单");
                }
            }
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                string line = "";
                intCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] szls = line.Split('=');
                        mydic.Add(intCount++, szls[0]);
                    }
                }
                userchoose.ItemsSource = mydic;
                userchoose.SelectedValuePath = "Key";
                userchoose.DisplayMemberPath = "Value";
            }
            userchoose.SelectedValue = 0;
        }

        private void DL_Click(object sender, RoutedEventArgs e)
        {
            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQID + ".txt";
            string[,] userSZ = new string[intCount, 3];
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                int i = 0;
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        if (line.Contains(userchoose.Text))
                        {
                            string passStr = line.Split('=')[1];
                            if (passStr == passWord.Password)
                            {
                                using (SqlConnection mycon = new SqlConnection(con))
                                {
                                    int jlCount = 0;
                                    try
                                    {

                                        mycon.Open();//打开
                                        string sql = string.Format(@"insert into USERJL values('{0}','{1}','{2}','{3}')", passStr, DateTime.Now.ToString("yyyy-MM-dd"), DQID, userchoose.Text);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                                        jlCount = sqlman.ExecuteNonQuery();

                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                    if (jlCount == 0)//如果插入失败，则说明已经存在，进行更新字段操作
                                    {
                                        try
                                        {
                                            string sql = string.Format(@"update USERJL set userID='{0}' ,Name='{1}' where date='{2}' and QXID='{3}'", passStr, userchoose.Text, DateTime.Now.ToString("yyyy-MM-dd"), DQID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                                            jlCount = sqlman.ExecuteNonQuery();
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show(ex.Message);
                                        }
                                        if (jlCount == 0)//如果更新字段失败，说明连接数据库失败，则保存至登陆信息至发报文件夹
                                        {
                                            string pathconfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
                                            using (StreamReader sr1 = new StreamReader(pathconfig, Encoding.Default))
                                            {
                                                string line1;
                                                string pathLS = "";

                                                // 从文件读取并显示行，直到文件的末尾 
                                                while ((line1 = sr1.ReadLine()) != null)
                                                {
                                                    if (line1.Split('=')[0] == "登陆信息保存路径")
                                                    {
                                                        pathLS = line1.Split('=')[1];
                                                    }
                                                }
                                                pathLS += DQID + DateTime.Now.ToString("yyyyMMdd") + "登陆.txt";
                                               
                                                    string ss = "";
                                                    ss += DQID + '=' + passStr + '\n';
                                                    using (FileStream fs = new FileStream(pathLS, FileMode.Create))
                                                    {
                                                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                                                        sw.Write(ss);
                                                        sw.Flush();
                                                        sw.Close();
                                                    }
                                                

                                            }
                                        }

                                    }
                                }

                                OpenFB();


                            }
                            else
                            {
                                MessageBox.Show("密码错误，请重新输入");
                            }
                        }
                    }
                }

            }


        }

        private void HQDL()
        {
            string ss = "";
            DateTime d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);//本月第一天
            string sd1 = d1.ToString("yyyy-MM-dd");
            string sd2 = DateTime.Now.ToString("yyyy-MM-dd");
            using (SqlConnection mycon = new SqlConnection(con))
            {

                mycon.Open();//打开
                try
                {
                    string sql = string.Format(@"select * from USERJL where QXID='{0}' AND date between '{1}' AND '{2}'", DQID, sd1, sd2);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        peopleList.Add(new people()
                        {
                            DLdate = sqlreader.GetDateTime(sqlreader.GetOrdinal("date")).ToString("yyyy年MM月dd日"),
                            userName = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            //ss += sqlreader.GetString(sqlreader.GetOrdinal("userID")) + '\t';
                        });
                    }
                    ((this.FindName("History")) as DataGrid).ItemsSource = peopleList;

                }

                catch (Exception ex)
                {

                }
            }

        }

        private void OpenFB()
        {
            string pathConfig = System.Environment.CurrentDirectory + @"\config\YBpath.txt";
            string YBpath = "";
            using (StreamReader sr1 = new StreamReader(pathConfig, Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == "发报软件路径")
                    {
                        YBpath = line1.Split('=')[1];
                    }
                }

            }
            if (File.Exists(YBpath))
            {
                string[] szLS = YBpath.Split('\\');
                string strML = YBpath.Substring(0, YBpath.Length - szLS[szLS.Length - 1].Length);
                Process pr = new Process();//声明一个进程类对象
                pr.StartInfo.WorkingDirectory = strML;
                pr.StartInfo.FileName = YBpath;//指定运行的程序
                pr.Start();//运行
            }
            else
            {
                MessageBox.Show("发报软件路径有误，请设置发报软件路径");
                var openFileDialog = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "可执行文件|*"
                };
                var result = openFileDialog.ShowDialog();
                if (result == true)
                {
                    using (FileStream fs = new FileStream(pathConfig, FileMode.Create))
                    {
                        using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                        {
                            sw.Write("发报软件路径=" + openFileDialog.FileName);
                            sw.Flush();
                        }
                    }
                    using (StreamReader sr1 = new StreamReader(pathConfig, Encoding.Default))
                    {
                        string line1 = "";
                        while ((line1 = sr1.ReadLine()) != null)
                        {
                            if (line1.Split('=')[0] == "发报软件路径")
                            {
                                YBpath = line1.Split('=')[1];
                            }
                        }

                    }
                    string[] szLS = YBpath.Split('\\');
                    string strML = YBpath.Substring(0, YBpath.Length - szLS[szLS.Length - 1].Length);
                    Process pr = new Process();//声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = YBpath;//指定运行的程序
                    pr.Start();//运行
                }
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
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
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }



        public class people
        {
            public string DLdate { get; set; }
            public string userName { get; set; }
        }

        private void ZTCXBtu_Click(object sender, RoutedEventArgs e)
        {
            BWZTTxt.Text = BWCX(DateTime.Now);
            RKZTTxt.Text = RKCX(DateTime.Now);
        }

        string BWCX(DateTime dt)
        {

            string fhStr = "";
            string bwid = "";
            using (StreamReader sr1 =
                new StreamReader(Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt", Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == DQID)
                    {
                        bwid = line1.Split('=')[1];
                        break;
                    }
                }
            }
            string bwPath = "", bwkrPath = "";
            using (StreamReader sr1 =
                new StreamReader(Environment.CurrentDirectory + @"\设置文件\路径设置.txt", Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == "乡镇精细化预报报文路径")
                    {
                        bwPath = line1.Split('=')[1];
                        bwkrPath = bwPath + "已入库\\";
                        break;
                    }
                }
            }
            string strParPath = "*" + bwid + "*" + dt.ToString("yyyyMMdd") + "*";
            string[] fileNameList1 = Directory.GetFiles(bwPath, strParPath);
            string[] fileNameList2 = Directory.GetFiles(bwkrPath, strParPath);
            if (fileNameList1.Length > 0 || fileNameList2.Length > 0)
                fhStr = DQID + dt.ToString("yyyy年MM月dd日") + "的报文已发送";
            else
                fhStr = DQID + dt.ToString("yyyy年MM月dd日") + "的报文不存在";

            return fhStr;
        }

        string RKCX(DateTime dt)
        {
            string fhStr = "";
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";//改
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line1;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line1 = sr.ReadLine()) != null)
                {
                    if (line1.Contains("sql管理员"))
                    {
                        con = line1.Substring("sql管理员=".Length);
                        break;
                    }
                }
            }
            using (SqlConnection mycon = new SqlConnection(con))
            {
                string YBDate = dt.ToString("yyyyMMdd");
                mycon.Open();
                string myDate = YBDate.Substring(0, 4) + '-' + YBDate.Substring(4, 2) + '-' + YBDate.Substring(6, 2);
                string sql = string.Format(@"select * from QXYB where StationID='{0}' and Date='{1}'", DQID,YBDate);
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                if (sqlreader.HasRows)
                    fhStr = DQID + dt.ToString("yyyy年MM月dd日")+"乡镇精细化预报已入库";
                else
                    fhStr= DQID + dt.ToString("yyyy年MM月dd日") + "旗县乡镇精细化预报没有入库";
            }
                return fhStr;
        }

        string BWRK(DateTime dt)
        {
            string bwid = "";
            using (StreamReader sr1 =
                new StreamReader(Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt", Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == DQID)
                    {
                        bwid = line1.Split('=')[1];
                        break;
                    }
                }
            }
            string YBpath = "";
            using (StreamReader sr1 =
                new StreamReader(Environment.CurrentDirectory + @"\设置文件\路径设置.txt", Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr1.ReadLine()) != null)
                {
                    if (line1.Split('=')[0] == "乡镇精细化预报报文路径")
                    {
                        YBpath = line1.Split('=')[1];
                        break;
                    }
                }
            }
            string fh = "";
            int JLGS = 0, SucGS = 0;//统计应该入库的记录总个数与成功入库的个数.
            string YBDate=dt.ToString("yyyyMMdd");
            int intCount = 0;//记录该旗县乡镇个数
            string strParPath = "*" + bwid + "*" + YBDate + "*";
            string[] fileNameList = Directory.GetFiles(YBpath, strParPath);
            if (fileNameList.Length > 0)
            {
                Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                                                   //寻找指定日期中该旗县的最晚和最早报文在fileNameList文件列表中的序号，最晚报文的全路径的为fileNameList[maxXH]，最早的为fileNameList[minXH]
                for (Int16 j = 0; j < fileNameList.Length; j++)
                {
                    string strLS = fileNameList[j].Split('_')[4];
                    intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                    if (j == 0)
                    {
                        maxLS = intLS;
                        minLS = intLS;
                    }
                    else
                    {
                        if (intLS > maxLS)
                        {
                            maxLS = intLS;
                            maxXH = j;
                        }
                        if (intLS < minLS)
                        {
                            minLS = intLS;
                            minXH = j;
                        }
                    }
                }

                string FQXID = DQID;
                DateTime YBtime = File.GetLastWriteTime(fileNameList[minXH]);
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    int lineCount = 0;
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (lineCount == 4)
                        {
                            intCount = Convert.ToInt32(line);
                            break;
                        }
                        lineCount++;

                    }
                }
                float[] Tmax24 = new float[intCount], Tmin24 = new float[intCount], Tmax48 = new float[intCount], Tmax72 = new float[intCount], Tmin48 = new float[intCount], Tmin72 = new float[intCount];
                string[] StationID = new string[intCount], Rain24 = new string[intCount], Rain48 = new string[intCount], Rain72 = new string[intCount];
                string[] FX24 = new string[intCount], FS24 = new string[intCount], FX48 = new string[intCount], FS48 = new string[intCount], FX72 = new string[intCount], FS72 = new string[intCount];
                string WeatherDZ = System.Environment.CurrentDirectory + @"\设置文件\天气对照.txt";
                float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
                string FSDZ = System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    string line = "";
                    int lineCount = 0;
                    int k = 0;
                    while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                    {
                        string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (lineCount == (15 * k + 5))
                        {
                            StationID[k] = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                        }
                        else if (lineCount == (15 * k + 6))
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (15 * k + 7))
                        {
                            Tmax24[k] = Convert.ToSingle(szLS[11]);
                            Tmin24[k] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain24[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain24[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX24[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX24[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS24[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS24[k] = LS1 + "转" + LS2;
                                }
                            }
                        }

                        else if (lineCount == (15 * k + 8))
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (15 * k + 9))
                        {
                            Tmax48[k] = Convert.ToSingle(szLS[11]);
                            Tmin48[k] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain48[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain48[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX48[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX48[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS48[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS48[k] = LS1 + "转" + LS2;
                                }
                            }
                        }
                        else if (lineCount == (15 * k + 10))
                        {
                            WeatherLS = Convert.ToSingle(szLS[19]);
                            FXLS = Convert.ToSingle(szLS[20]);
                            FSLS = Convert.ToSingle(szLS[21]);

                        }
                        else if (lineCount == (15 * k + 11))
                        {
                            Tmax72[k] = Convert.ToSingle(szLS[11]);
                            Tmin72[k] = Convert.ToSingle(szLS[12]);
                            float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                            if (WeatherLS == WeatherLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            Rain72[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    Rain72[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FXLS == FXLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            FX72[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS.ToString()))
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (line1.Contains(FXLS1.ToString()))
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FX72[k] = LS1 + "转" + LS2;
                                }
                            }
                            if (FSLS == FSLS1)
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            FS72[k] = line1.Split('=')[0];
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    string line1 = "";
                                    string LS1 = "", LS2 = "";
                                    while ((line1 = sr1.ReadLine()) != null)
                                    {
                                        if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                        {
                                            LS1 = line1.Split('=')[0];
                                        }
                                        else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                        {
                                            LS2 = line1.Split('=')[0];
                                        }

                                    }
                                    FS72[k] = LS1 + "转" + LS2;
                                }
                            }
                            k++;
                            JLGS++;
                        }

                        lineCount++;

                    }
                }
                string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";//改
                //该旗县所有乡镇的预报信息已经保存，开始保存至数据库
                string con = "";
                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                {
                    string line1;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        if (line1.Contains("sql管理员"))
                        {
                            con = line1.Substring("sql管理员=".Length);
                            break;
                        }
                    }
                }
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();
                    string myDate = YBDate.Substring(0, 4) + '-' + YBDate.Substring(4, 2) + '-' + YBDate.Substring(6, 2);
                    for (int j = 0; j < intCount; j++)
                    {
                        string sql = string.Format(@"insert into QXYB values('{0}','{1}','{2}','','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')", StationID[j], myDate, FQXID, Tmax24[j], Tmin24[j], Rain24[j], FX24[j], FS24[j], Tmax48[j], Tmin48[j], Rain48[j], FX48[j], FS48[j], Tmax72[j], Tmin72[j], Rain72[j], FX72[j], FS72[j], YBtime);
                        try
                        {
                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                            sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数）  
                            SucGS++;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                }

            }

            string[] cutToPath = new string[fileNameList.Length];
            for (int j = 0; j < fileNameList.Length; j++)
            {
                string[] szLS = fileNameList[j].Split('\\');
                if (!Directory.Exists(YBpath + @"已入库\"))
                {
                    Directory.CreateDirectory(YBpath + @"已入库\");
                }
                cutToPath[j] = YBpath + @"已入库\" + szLS[szLS.Length - 1];
                File.Move(fileNameList[j], cutToPath[j]);
            }
            fh= string.Format("共计{0}条记录，成功入库{1}条记录。", JLGS, SucGS);
            fh = DateTime.Now.ToString() + "保存" + YBDate + "旗县预报至数据库：" + fh;
            return fh;
        }

        private void BWRKBtu_Click(object sender, RoutedEventArgs e)
        {
           RKZTTxt.Text= BWRK(DateTime.Now);
        }
    }
}
