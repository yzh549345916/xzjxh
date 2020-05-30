using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// 实况恢复.xaml 的交互逻辑
    /// </summary>
    public partial class 实况恢复 : Window
    {
        string con;//= "Server=172.18.142.151;Database=xzjxhyb_DB;user id=sa;password=134679;"; //这里是保存连接数据库的字符串172.18.142.151 id=sa;password=134679;
        string RKTime = "20";//实况入库的时次,窗口初始化程序中会重新给该值从配置文件中赋值
        Int16 SFJG = 0;
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 实况恢复()
        {
            InitializeComponent();
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            eDate.BlackoutDates.Add(dr2);//结束时间不可选的范围
            progressBar1.Maximum = 100;//进度条最大值为100

            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";

            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取数据库配置信息 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                    }

                    else if (line.Split('=')[0] == ("实况时次"))
                    {
                        RKTime = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "区域站雨量筒是否加盖")
                    {
                        try
                        {
                            SFJG = Convert.ToInt16(line.Split('=')[1]);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                }
            }
        }

        //开始日期更改后，结束日期的范围随之变更，禁止结束日期选取开始日期之前的
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                double douLS;//赋值保存进度条的进度数
                UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
                UpdateProgressBarDelegate updateText = new UpdateProgressBarDelegate(txtboxJD.SetValue);

                DateTime dateStartDate = Convert.ToDateTime(sDate.SelectedDate), dateEndDate = Convert.ToDateTime(eDate.SelectedDate);//获取选择的起止时间
                DateTime dateLS = dateStartDate, dateLS2 = dateLS;
                int intLS = 0;
                for (int i = 0; DateTime.Compare(dateLS2, dateEndDate) <= 0; i++)//判断总共需要循环的次数，决定进度条的进度
                {
                    intLS++;
                    dateLS2 = dateLS2.AddDays(1);
                }
                int ZSLS = 0, SucGSLS = 0;
                string ss = "";
                for (int i = 0; DateTime.Compare(dateLS, dateEndDate) <= 0; i++)//临时日期初始值为开始日期，每个循环加1天，一直到大于截止日期
                {
                    douLS = (i + 1) * 100 / intLS;
                    string strDate = dateLS.ToString("yyyyMMdd");
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, Math.Ceiling(douLS) });//委托更新显示进度条
                    Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, strDate });//委托更新显示文本
                    dateLS = dateLS.AddDays(1);
                    string strfile = Save(strDate, RKTime);//
                    SKaddDB(strfile, ref ZSLS, ref SucGSLS);
                    ss += strDate + " 实况共计" + ZSLS.ToString() + "条，成功入库" + SucGSLS.ToString() + "条。\n";
                    File.Delete(strfile);
                    HQSK hqRainSk = new HQSK();
                    string error = "";
                    hqRainSk.CIMISSRain12(strDate, ref error, ref ss);

                }
                Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, "完成" });
                MessageBox.Show(ss);
            }
            else
            {
                MessageBox.Show("请选择起止时间");
            }



        }

        //查询strDate-1(yyyyMMdd)日strTime时至strDate日strTime时的实况，并保存，返回保存文件的全路径
        public string Save(string strDate, string strTime)
        {
            string strError = "";
            string strSK = "";
            int rst1 = 0, rst2 = 0;
            string strSKPath = System.Environment.CurrentDirectory + @"\";//实况临时保存路径

            string strFile = strSKPath + strDate + strTime + "实况.txt";
            if (!File.Exists(strFile))
            {
                HQSK classHQSK = new HQSK();
                string strQXSK = classHQSK.CIMISSHQQXSK(strDate, strTime, ref rst1, ref strError);
                string strSZSK = classHQSK.CIMISSHQXZSK(strDate, strTime, ref rst2, ref strError);
                if ((rst1 == 0) && (rst2 == 0))//如果CIMISS返回均为0
                {

                    strSK = strQXSK + '\n' + strSZSK;
                    FileStream myFs = new FileStream(strFile, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs, Encoding.Default);
                    mySw.Write(strSK);
                    mySw.Close();
                    myFs.Close();
                }
            }




            if (strError.Length == 0)
            {

            }
            else
            {
                MessageBox.Show("CIMISS出错，返回代码为：" + strError);
            }
            return strFile;
        }

        //将指定全路径下的文件中的实况信息保存至数据中
        public void SKaddDB(string filepath, ref int ZS, ref int SucGS)
        {
            ZS = 0; SucGS = 0;
            if ((File.Exists(filepath)))
            {
                SqlConnection mycon = new SqlConnection(con);//创建SQL连接对象
                mycon.Open();//打开

                StreamReader sr = new StreamReader(filepath, Encoding.Default); //实例化SqlDtatAdapter并执行SQL语句，至于什么是SQLDataAdapter，就是用来连接DataSet与数据库的，DataSet是C#中用来保存数据库数据的，然后再DataSet中创建列与行来填充，个人理解。
                string line;
                string[] szLS2 = filepath.Split('\\');
                while ((line = sr.ReadLine()) != null)
                {
                    ZS++;
                    string[] szLS1 = line.Split(' ');
                    //定义从文本文件得到的每行的实况各要素,此处增加风要素
                    string myName = szLS1[0], myStationID = szLS1[2];
                    float myTmax, myTmin, myRain;
                    string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                    /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取旗县台站号*/
                    StreamReader sr1 = new StreamReader(configXZPath, Encoding.Default);
                    String line1;
                    //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
                    line1 = sr1.ReadLine();
                    sr1.Close();
                    string[] linShi1 = line1.Split(':');
                    int intQXGS = Convert.ToInt16(linShi1[1]);
                    string QXID = "";

                    //每两行第一列为旗县ID
                    int lineCount = 0;
                    sr1 = new StreamReader(configXZPath, Encoding.Default);
                    while (lineCount < intQXGS * 2 + 1)
                    {
                        line1 = sr1.ReadLine();
                        if ((lineCount > 1) && (lineCount % 2 == 0))
                        {
                            QXID += line1.Split(',')[0] + ',';
                        }
                        lineCount++;

                    }
                    sr1.Close();
                    QXID = QXID.Substring(0, QXID.Length - 1);
                    if (SFJG != 0 && !QXID.Contains(myStationID))//判断是否加盖，如果加盖判断是否为区域站，如果是，则降水量按照缺测处理  SFJG是否加盖标识，如果为0则不加盖，如果为1则加盖
                    {
                        myRain = 999999;
                    }
                    else
                    {
                        try
                        {
                            myRain = Convert.ToSingle(szLS1[5]);
                        }
                        catch
                        {
                            myRain = 999999;
                        }
                    }
                    //给各要素实况赋值，防止出现数据为空的情况，故使用try
                    try
                    {
                        myTmax = Convert.ToSingle(szLS1[3]);
                    }
                    catch
                    {
                        myTmax = 999999;
                    }
                    try
                    {
                        myTmin = Convert.ToSingle(szLS1[4]);
                    }
                    catch
                    {
                        myTmin = 999999;
                    }
                    if (myTmin == myTmax)//如果最高最低温度相等，按照缺测处理
                    {
                        myTmin = 999999;
                        myTmax = 999999;
                    }

                    int intLS = szLS2.Length - 1;
                    //数据库中的日期保存格式为“yyyy-MM-DD”需加“-”
                    string myDate = szLS2[intLS].Substring(0, 4) + '-' + szLS2[intLS].Substring(4, 2) + '-' + szLS2[intLS].Substring(6, 2);
                    //此处增加风要素
                    string sql = string.Format(@"insert into SK (Name,StationID,Date,Tmax,Tmin,Rain) values('{0}','{1}','{2}','{3}','{4}','{5}')", myName, myStationID, myDate, myTmax, myTmin, myRain);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数） 
                        SucGS++;


                    }
                    catch (Exception)
                    {
                        // MessageBox.Show("数据库添加失败\n" + ex.Message);
                    }

                }
                sr.Close();
                mycon.Close();
            }

        }
    }


}
