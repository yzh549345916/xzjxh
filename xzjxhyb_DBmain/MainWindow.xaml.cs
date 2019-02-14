using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using cma.cimiss.client;
using System.Data.SqlClient;
using MessageBox = System.Windows.MessageBox;
using System.Drawing;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon = null;
        string con;//= "Server=172.18.142.151;Database=xzjxhyb_DB;user id=sa;password=134679;"; //这里是保存连接数据库的字符串172.18.142.151 id=sa;password=134679;
        Int16 RKMM = 0, RKH = 0,QXRKH=0,QXRKM=0,SJRKH=0,SJRKM=0;//实况入库的分钟和小时,窗口初始化程序中会重新给该值从配置文件中赋值
        string RKTime = "20";//实况入库的时次,窗口初始化程序中会重新给该值从配置文件中赋值
        Int16 SFJG = 0;
        System.Timers.Timer t = new System.Timers.Timer(60000);
        public MainWindow()
        {
            InitializeComponent();
            t1.Text = DateTime.Now.ToString()+ "  启动";
            //实例化timer，使得间隔为1000ms  
            
            t.Elapsed += new System.Timers.ElapsedEventHandler(refreshTime);
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true)；  
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件；  
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";

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
                    else if (line.Contains("实况入库小时"))
                    {
                        RKH = Convert.ToInt16(line.Substring("实况入库小时=".Length));
                    }
                    else if (line.Contains("实况入库分钟"))
                    {
                        RKMM = Convert.ToInt16(line.Substring("实况入库分钟=".Length));
                    }
                    else if(line.Split('=')[0]=="实况时次")
                    {
                        RKTime = line.Split('=')[1];
                    }
                    else if (line.Contains("旗县预报入库小时"))
                    {
                        QXRKH = Convert.ToInt16(line.Substring("旗县预报入库小时=".Length));
                    }
                    else if (line.Contains("旗县预报入库分钟"))
                    {
                        QXRKM = Convert.ToInt16(line.Substring("旗县预报入库分钟=".Length));
                    }
                    else if(line.Split('=')[0]== "市局预报入库小时")
                    {
                        SJRKH = Convert.ToInt16(line.Split('=')[1]);
                    }
                    else if (line.Split('=')[0] == "市局预报入库分钟")
                    {
                        SJRKM = Convert.ToInt16(line.Split('=')[1]);
                    }

                    else if (line.Split('=')[0] == "区域站雨量筒是否加盖")
                    {
                        try
                        {
                            SFJG = Convert.ToInt16(line.Split('=')[1]);
                        }
                        catch(Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        
                    }
                }
            }
           // InitialTray(); //最小化至托盘
        }
        #region 最小化系统托盘
        public void InitialTray()
        {
            if (_notifyIcon == null)
            {
                _notifyIcon = new NotifyIcon();
                //隐藏主窗体
                this.Visibility = Visibility.Hidden;
                //设置托盘的各个属性
                
                _notifyIcon.BalloonTipText = "呼和浩特市乡镇精细化预报数据库服务运行中...";//托盘气泡显示内容
                _notifyIcon.Text = "呼和浩特市乡镇精细化预报数据库";
                _notifyIcon.Visible = true;//托盘按钮是否可见
                _notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);;//托盘中显示的图标
                _notifyIcon.ShowBalloonTip(2000);//托盘气泡显示时间
                _notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
                //窗体状态改变时触发
                this.StateChanged += MainWindow_StateChanged;
            }
            else
            {
                this.WindowState = WindowState.Minimized;
            }
            
        }
        #endregion

        #region 窗口状态改变
        private void MainWindow_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Visibility = Visibility.Hidden;
            }
        }
        #endregion

        #region 托盘图标鼠标单击事件
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (this.Visibility == Visibility.Visible)
                {
                    this.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.Visibility = Visibility.Visible;
                    this.Activate();
                }
            }
        }
        #endregion


        private void SJYBHFBu_Click(object sender, RoutedEventArgs e)
        {
            SJYBHFWindow SJHF = new SJYBHFWindow();
            SJHF.Show();
        }

        private void JLButton_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Visible;
            errorTBox.Visibility = Visibility.Hidden;
            t1.Focus();
            t1.CaretIndex = (t1.Text.Length);
        }

        private void errorJLBut_Click(object sender, RoutedEventArgs e)
        {
            t1.Visibility = Visibility.Hidden;
            errorTBox.Visibility = Visibility.Visible;
            errorTBox.Focus();
            errorTBox.CaretIndex = (errorTBox.Text.Length);
        }

        private void TJHFBu_Click(object sender, RoutedEventArgs e)
        {
            统计信息重新入库窗口 TJHF = new 统计信息重新入库窗口();
            TJHF.Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
          
            QXXZConfig qXXZConfig = new QXXZConfig();
            qXXZConfig.Show();

        }

        public void refreshTime(object source, System.Timers.ElapsedEventArgs e)
        {
           
            try
            {
                t.Enabled = false;
                string errorQJZN = "";
                if (DateTime.Now.Hour == RKH && DateTime.Now.Minute == RKMM) //如果当前时间是10点30分
                {
                    try
                    {
                        区局智能网格 qjzn = new 区局智能网格();
                        for (int i = -1; i > -8; i--)
                        {
                            try
                            {
                                string strDate = DateTime.Now.AddDays(i).ToString("yyyyMMdd");
                                string strfile = Save(strDate, RKTime);
                                int SKRKGS = SKaddDB(strfile);
                                File.Delete(strfile);
                                this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(DateTime.Now.ToString() + "保存" + strDate + "日" +
                                                          SKRKGS.ToString() + "条实况至数据库" + '\n');
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                                SaveJL(DateTime.Now.ToString() + "保存" + strDate + "日" + SKRKGS.ToString() + "条实况至数据库" +
                                       "\r\n");
                                HQSK HQRain = new HQSK();
                                string error = "";
                                string jltext = "";
                                HQRain.CIMISSRain12(DateTime.Now.AddDays(i).ToString("yyyyMMdd"), ref error,
                                    ref jltext);

                                TJAddDB tjAddDB = new TJAddDB();
                                error += tjAddDB.FirstTJ(DateTime.Now.AddDays(i - 1)); //实况入库后，统计预报实况检验情况

                                this.t1.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            t1.AppendText(jltext + '\n');
                                            //将光标移至文本框最后
                                            t1.Focus();
                                            t1.CaretIndex = (t1.Text.Length);
                                        }
                                    ));
                                this.errorTBox.Dispatcher.Invoke(
                                    new Action(
                                        delegate
                                        {
                                            errorTBox.AppendText('\n' + error + '\n');
                                            //将光标移至文本框最后
                                            errorTBox.Focus();
                                            errorTBox.CaretIndex = (errorTBox.Text.Length);
                                        }
                                    ));
                                errorQJZN += qjzn.QJZNTJ(DateTime.Now.AddDays(i - 1));
                                if (i == -1)
                                {
                                    if (errorQJZN.Trim().Length == 0)
                                        errorQJZN = DateTime.Now.ToString() + "  " +
                                                    DateTime.Now.AddDays(i - 1).ToString("yyyy年MM月dd日") +
                                                    "区局智能网格统计信息入库成功。\r\n";
                                    else
                                        errorQJZN = DateTime.Now.AddDays(i - 1).ToString("yyyy年MM月dd日") +
                                                    "区局智能网格统计信息入库失败：" + errorQJZN;

                                    this.errorTBox.Dispatcher.Invoke(
                                        new Action(
                                            delegate
                                            {
                                                errorTBox.AppendText(errorQJZN);
                                                //将光标移至文本框最后
                                                errorTBox.Focus();
                                                errorTBox.CaretIndex = (errorTBox.Text.Length);
                                            }
                                        ));
                                    SaveJL(errorQJZN);
                                    errorQJZN = "";
                                }
                            }
                            catch
                            {
                            }
                        }

                        if (errorQJZN.Trim().Length == 0)
                            errorQJZN = DateTime.Now.ToString() + "  " +
                                        DateTime.Now.AddDays(-2).ToString("yyyy年MM月dd日") + "区局智能网格过去7天统计信息入库成功。\r\n";
                        else
                            errorQJZN = DateTime.Now.AddDays(-2).ToString("yyyy年MM月dd日") + "区局智能网格统计过去7天信息入库失败：" +
                                        errorQJZN;

                        this.errorTBox.Dispatcher.Invoke(
                            new Action(
                                delegate
                                {
                                    errorTBox.AppendText(errorQJZN);
                                    //将光标移至文本框最后
                                    errorTBox.Focus();
                                    errorTBox.CaretIndex = (errorTBox.Text.Length);
                                }
                            ));


                        SaveJL(errorQJZN);
                        errorQJZN = "";
                    }
                    catch
                    {
                    }
                }

                if (DateTime.Now.Hour == QXRKH && DateTime.Now.Minute == QXRKM)
                {
                    for (int i = 0; i > -7; i--)
                    {
                        try
                        {
                            saveQXYB saveqxyb = new saveQXYB();
                            string ss = saveqxyb.saveQXCS(DateTime.Now.AddDays(i).ToString("yyyyMMdd"));
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }
                        catch
                        {
                        }
                    }



                }

                if (DateTime.Now.Hour == SJRKH && DateTime.Now.Minute == SJRKM)
                {
                    try
                    {
                        DeleteFile();
                        for (int i = -1; i > -7; i--)
                        {
                            saveSJYB savesjyb = new saveSJYB();
                            string ss = savesjyb.saveSJCS(DateTime.Now.AddDays(i).ToString("yyyyMMdd"));
                            this.t1.Dispatcher.Invoke(
                                new Action(
                                    delegate
                                    {
                                        t1.AppendText(ss);
                                        //将光标移至文本框最后
                                        t1.Focus();
                                        t1.CaretIndex = (t1.Text.Length);
                                    }
                                ));
                            SaveJL(ss);
                        }
                    }
                    catch
                    {
                    }
                }

            }
            catch
            {
            }
            finally
            {
                t.Enabled = true;
            }
        }

        //strDAte为实况前一天strTime至当天strTime的实况。 方法返回保存的实况txt全路径
        public string Save(string strDate, string strTime)
        {
            string strError = "";
            string strSK = "";
            int rst1 = 0, rst2 = 0;
            string strSKPath = System.Environment.CurrentDirectory+ @"\";//实况临时保存路径
            
            string strFile = strSKPath + strDate + strTime + "实况.txt";
            try
            {
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
            }
            catch
            {
            }

            return strFile;
        }

        private void HistoryBu_Click(object sender, RoutedEventArgs e)
        {

            实况恢复 WinHF = new 实况恢复();
            WinHF.Show();
        }
        private void QXYBHF_Click(object sender, RoutedEventArgs e)
        {
            QXYBHFWindow WinHF = new QXYBHFWindow();
            WinHF.Show();
        }
        public int SKaddDB(string filepath)
        {
            
            int SKRKGS = 0; 
            if ((File.Exists(filepath)))
            {
                SqlConnection mycon = new SqlConnection(con);//创建SQL连接对象
                mycon.Open();//打开

                StreamReader sr = new StreamReader(filepath, Encoding.Default); //实例化SqlDtatAdapter并执行SQL语句，至于什么是SQLDataAdapter，就是用来连接DataSet与数据库的，DataSet是C#中用来保存数据库数据的，然后再DataSet中创建列与行来填充，个人理解。
                string line;
                string[] szLS2 = filepath.Split('\\');
                while ((line = sr.ReadLine()) != null)
                {
                    string[] szLS1 = line.Split(' ');
                    //定义从文本文件得到的每行的实况各要素 此处增加风要素
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
                    string myDate = szLS2[intLS].Substring(0, 4) + '-' + szLS2[intLS].Substring(4, 2) + '-' + szLS2[intLS].Substring(6, 2);
                    //此处增加风要素
                    string sql = string.Format(@"insert into SK (Name,StationID,Date,Tmax,Tmin,Rain) values('{0}','{1}','{2}','{3}','{4}','{5}')", myName, myStationID, myDate, myTmax, myTmin, myRain);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SKRKGS += sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数）     
                        


                    }
                    catch (Exception ex)
                    {
                        // MessageBox.Show("数据库添加失败\n" + ex.Message);
                    }

                }
                sr.Close();
                mycon.Close();
            }
            return SKRKGS;
        }
        public void SaveJL(string jtText)
        {
            try
            {
                string DicPath = Environment.CurrentDirectory + @"\日志";
                string path = DicPath + '\\' + DateTime.Now.ToString("yyyy年MM月dd日") + "日志文件.txt";
                if (!Directory.Exists(DicPath))
                {
                    Directory.CreateDirectory(DicPath);
                }
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.Default))
                {
                    sw.Write(jtText);
                    sw.Flush();
                }
            }
            catch
            {
            }
        }

        public void DeleteFile()
        {
            try
            {
                string YBpath = "";
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\路径设置.txt", Encoding.Default))
                {
                    string line = "";

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("乡镇精细化预报报文路径"))
                        {
                            YBpath = line.Substring("乡镇精细化预报报文路径=".Length);
                            break;
                        }
                    }
                }

                string strParPath = "*.ENN";
                string[] fileNameList = Directory.GetFiles(YBpath, strParPath);
                foreach (string f in fileNameList)
                {
                    File.Delete(f);
                }
                strParPath = "*BABJ*.TXT";
                fileNameList = Directory.GetFiles(YBpath, strParPath);
                foreach (string f in fileNameList)
                {
                    File.Delete(f);
                }
            }
            catch
            {
            }
        }

    }

    public class HQSK
    {
        Int16 SFJG = 0;
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";


        public string CIMISSHQQXSK(string strDate, string strTime, ref int rst1, ref string strError)
        {

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
                                                          //检索时间段
            string strToday = strDate + strTime + "0000";
            string strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            paramsqx.Add("times", strToday);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string QXID = "";

            //每两行第一列为旗县ID
            int lineCount = 0;
            sr = new StreamReader(configXZPath, Encoding.Default);
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))
                {
                    QXID += line.Split(',')[0] + ',';
                }
                lineCount++;

            }
            sr.Close();
            QXID = QXID.Substring(0, QXID.Length - 1);

            paramsqx.Add("staIds", QXID);//选择区站号
            //此处增加风要素
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h");// 检索要素：站号、站名、过去24h最高、最低气温、24小时降水量
            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            strLS = strData.Split('"')[1];
            rst = Convert.ToInt32(strLS);
            rst1 = rst;

            if (rst == 0)
            {
                string[] SZlinshi = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData += SZlinshi[i] + '\n';
                    }
                }
                strData = strData.Substring(0, strData.Length - 1);
                //对旗县实况排序，使得旗县的顺序与旗县名单文件中的一致，便于程序后续处理
                lineCount = 0;
                strLS = strData;
                strData = "";
                sr = new StreamReader(configXZPath, Encoding.Default);
                while (lineCount < intQXGS * 2 + 1)
                {
                    line = sr.ReadLine();
                    if ((lineCount > 1) && (lineCount % 2 == 0))
                    {
                        QXID = line.Split(',')[0];
                        SZlinshi = strLS.Split('\n');
                        for (int j = 0; j < SZlinshi.Length; j++)
                        {
                            if (SZlinshi[j].Contains(QXID))//判断该行是否存在该旗县区站号，如果包含，就把整行数据保存
                            {
                                strData += SZlinshi[j] + "\n";
                            }
                        }
                    }
                    lineCount++;

                }
                sr.Close();
                strData = strData.Substring(0, strData.Length - 1);
            }
            else
            {
                strError += strData + '\n';
            }

            return strData;

        }
        public string CIMISSHQXZSK(string strDate, string strTime, ref int rst2, ref string strError)//函数输入为查询实况的日期与时次，输出为旗县实况与返回CIMISS错误代码；
        {
            string strData = "";

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "statSurfEleByStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            //检索时间段
            string strToday = strDate + strTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string stYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");
            string timeRange1 = "(" + stYesterday + "," + strToday + "]";
            paramsqx.Add("timeRange", timeRange1);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县个数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string XZID = "";
            //每两行第一列为旗县ID 
            int lineCount = 0;
            sr = new StreamReader(configXZPath, Encoding.Default);
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))//避免取到第一行的旗县个数，因此lineCount>1，站号为编号偶数行故对2取余
                {
                    for (int i = 1; i < line.Split(',').Length; i++)//提取乡镇区站号，编号为0是旗县站号，故从1开始
                    {
                        XZID += line.Split(',')[i] + ',';
                    }

                }
                lineCount++;

            }
            sr.Close();
            XZID = XZID.Substring(0, XZID.Length - 1);

            paramsqx.Add("staIds", XZID);//选择区站号，从乡镇名单中获取
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            //此处增加风要素
            paramsqx.Add("statEles", "MAX_TEM_Max,MIN_TEM_MIN,SUM_PRE_1h");// 统计要素最高温度的最大值，与最低温度的最小值以及小时降水量
            // 可选参数
            paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder retStrXZ = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, retStrXZ);
            // 释放接口服务连接资源
            client.destroyResources();
            strData = Convert.ToString(retStrXZ);
            strLS = strData.Split('"')[1];
            rst = Convert.ToInt32(strLS);
            rst2 = rst;
            if (rst == 0)
            {
                string[] SZlinshi = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData += SZlinshi[i] + "\n";
                    }
                }
                strData = strData.Substring(0, strData.Length - 1);
            }
            else
            {
                strError += strData;
            }




            return strData;

        }

        public void CIMISSRain12(string strDate, ref string strError, ref string jltext)
        {
            string strTime = "20";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string  con = "";
            string line = "";
            using (StreamReader sr2 = new StreamReader(DBconPath, Encoding.Default))
            {


                // 从文件读取数据库配置信息 
                while ((line = sr2.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                    }

                    else if (line.Split ('=')[0]=="实况时次")
                    {
                        strTime = line.Split('=')[1];
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

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "statSurfEleByStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx1 = new Dictionary<String, String>();//前半天
            Dictionary<String, String> paramsqx2 = new Dictionary<String, String>();//后半天
            // 必选参数
            paramsqx1.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            paramsqx2.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            //检索时间段
            string strToday = strDate + strTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string stYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");
            string halfDay = Convert.ToDateTime(strLS).AddHours(-12).ToUniversalTime().ToString("yyyyMMddHH0000");
            string timeRange1 = "(" + stYesterday + "," + halfDay + "]";
            string timeRange2 = "(" + halfDay + "," + strToday + "]";
            paramsqx1.Add("timeRange", timeRange1);
            paramsqx2.Add("timeRange", timeRange2);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            //读取设置文件的旗县乡镇文件中第一行，确认旗县个数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string XZID = "";
            //每两行第一列为旗县ID 
            int lineCount = 0;
            sr = new StreamReader(configXZPath, Encoding.Default);
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))//避免取到第一行的旗县个数，因此lineCount>1，站号为编号偶数行故对2取余
                {
                    for (int i = 0; i < line.Split(',').Length; i++)//提取乡镇区站号，编号为0是旗县站号，故从1开始
                    {
                        XZID += line.Split(',')[i] + ',';
                    }

                }
                lineCount++;

            }
            sr.Close();
            XZID = XZID.Substring(0, XZID.Length - 1);

            paramsqx1.Add("staIds", XZID);//选择区站号，从乡镇名单中获取
            paramsqx2.Add("staIds", XZID);//选择区站号，从乡镇名单中获取
            paramsqx1.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            paramsqx2.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            //此处增加风要素
            paramsqx1.Add("statEles", "SUM_PRE_1h");// 统计要素小时降水量
            paramsqx2.Add("statEles", "SUM_PRE_1h");// 统计要素小时降水量
            // 可选参数
            paramsqx1.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            paramsqx2.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "Text";
            StringBuilder retStrXZ1 = new StringBuilder();//返回字符串
            StringBuilder retStrXZ2 = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst1 = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx1, dataFormat, retStrXZ1);
            int rst2 = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx2, dataFormat, retStrXZ2);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData1 = Convert.ToString(retStrXZ1);
            string strData2 = Convert.ToString(retStrXZ2);
            strLS = strData1.Split('"')[1];
            rst1 = Convert.ToInt32(strLS);
            if (rst1 == 0)
            {
                string[] SZlinshi = strData1.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData1 = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData1 += SZlinshi[i] + "\n";
                    }
                }
                strData1 = strData1.Substring(0, strData1.Length - 1);
            }
            else
            {
                strError += strDate+ "前十二小时降水量CIMISS获取出错：\n"+strData1;
            }

            strLS = strData2.Split('"')[1];
            rst2 = Convert.ToInt32(strLS);
            if (rst2 == 0)
            {
                string[] SZlinshi = strData2.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                strData2 = "";
                /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
                for (int i = 0; i < SZlinshi.Length; i++)
                {
                    if (i > 1)
                    {
                        strData2 += SZlinshi[i] + "\n";
                    }
                }
                strData2 = strData2.Substring(0, strData2.Length - 1);
            }
            else
            {
                strError += strDate + "后十二小时降水量CIMISS获取出错：\n" + strData2;
            }
            int XZGS = XZID.Split(',').Length;
            string[,] Rain0012 = new string[XZGS, 2], Rain1224 = new string[XZGS, 2];
            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取旗县台站号*/
            StreamReader sr1 = new StreamReader(configXZPath, Encoding.Default);
            String line1;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line1 = sr1.ReadLine();
            sr1.Close();
            string[] linShi2 = line1.Split(':');
            int intQXGS1 = Convert.ToInt16(linShi2[1]);
            string QXID = "";

            //每两行第一列为旗县ID
            int lineCount1 = 0;
            sr1 = new StreamReader(configXZPath, Encoding.Default);
            while (lineCount1 < intQXGS1 * 2 + 1)
            {
                line1 = sr1.ReadLine();
                if ((lineCount1 > 1) && (lineCount1 % 2 == 0))
                {
                    QXID += line1.Split(',')[0] + ',';
                }
                lineCount1++;

            }
            sr1.Close();
            QXID = QXID.Substring(0, QXID.Length - 1);
            
            try
            {

                string[] szLS = strData1.Split('\n');
                for (int i = 0; i < XZGS; i++)
                {
                    string[] szLS2 = szLS[i].Split(' ');
                    Rain0012[i, 0] = szLS2[2];
                    if (SFJG != 0 && !QXID.Contains(Rain0012[i, 0]))//判断是否加盖，如果加盖判断是否为区域站，如果是，则降水量按照缺测处理  SFJG是否加盖标识，如果为0则不加盖，如果为1则加盖
                    {
                        Rain0012[i, 1] = "999999";
                    }
                    else
                    {
                        Rain0012[i, 1] = szLS2[3];
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
            try
            {
                string[] szLS = strData2.Split('\n');
                for (int i = 0; i < XZGS; i++)
                {
                    string[] szLS2 = szLS[i].Split(' ');
                    Rain1224[i, 0] = szLS2[2];
                    if (SFJG != 0 && !QXID.Contains(Rain1224[i, 0]))//判断是否加盖，如果加盖判断是否为区域站，如果是，则降水量按照缺测处理  SFJG是否加盖标识，如果为0则不加盖，如果为1则加盖
                    {
                        Rain1224[i, 1] = "999999";
                    }
                    else
                    {
                        Rain1224[i, 1] = szLS2[3];
                    }

                }
            }
            catch (Exception ex)
            {
               
            }
            //数据库中的日期保存格式为“yyyy-MM-DD”需加“-”
            string myDate = strDate.Substring(0, 4) + '-' + strDate.Substring(4, 2) + '-' + strDate.Substring(6, 2);
            int ZS = 0, Q12 = 0, H12 = 0;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//Rain1224='{1}'
                for (int i = 0; i < XZGS; i++)
                {
                    ZS++;
                    string sql = string.Format(@"update SK set Rain0012='{0}' where StationID='{1}' and Date='{2}'", Rain0012[i, 1], Rain0012[i, 0], myDate);
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                                                    //执行数据库语句并返回一个int值（受影响的行数）  
                        if(sqlman.ExecuteNonQuery()>0)
                        {
                            Q12++;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                    sql = string.Format(@"update SK set Rain1224='{0}' where StationID='{1}' and Date='{2}'", Rain1224[i, 1], Rain1224[i, 0], myDate);
                    try
                    {
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                       //执行数据库语句并返回一个int值（受影响的行数）  
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            H12++;
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            jltext += strDate + " 降水量共需入库" + ZS.ToString() + "条，成功入库" + Q12.ToString() + "条。";


        }


    }
}
