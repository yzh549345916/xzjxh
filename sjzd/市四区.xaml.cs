using Aspose.Words;
using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace sjzd
{
    /// <summary>
    /// 市四区.xaml 的交互逻辑
    /// </summary>
    public partial class 市四区 : Window
    {
        string SJsaPath = "";
        string SSQconPath = System.Environment.CurrentDirectory + @"\设置文件\市四区\市四区配置.txt";
        string configZDPath = System.Environment.CurrentDirectory + @"\设置文件\市四区\市四区站点.txt";
        public 市四区()
        {
            InitializeComponent();
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            string PeopleConfig = System.Environment.CurrentDirectory + @"\设置文件\市四区\值班人员.txt";
            int intCount = 0;
            try
            {
                using (StreamReader sr = new StreamReader(PeopleConfig, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            string[] szls = line.Split('=');
                            mydic.Add(intCount++, szls[0]);
                        }
                    }
                }
                ZBCom.ItemsSource = mydic;
                ZBCom.SelectedValuePath = "Key";
                ZBCom.DisplayMemberPath = "Value";
                FBCom.ItemsSource = mydic;
                FBCom.SelectedValuePath = "Key";
                FBCom.DisplayMemberPath = "Value";
                QFCom.ItemsSource = mydic;
                QFCom.SelectedValuePath = "Key";
                QFCom.DisplayMemberPath = "Value";
                QFCom.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SSQ(DateTime dt, ref string YBData)
        {

            string DZTime = "15";
            try
            {
                using (StreamReader sr1 = new StreamReader(SSQconPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";

                    // 从文件读取数据库配置信息 
                    while ((line1 = sr1.ReadLine()) != null)
                    {
                        if (line1.Split('=')[0] == "订正市局指导实况时次")
                        {
                            DZTime = line1.Split('=')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            string strToday = dt.ToUniversalTime().ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            DateTime dtLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null);
            if (dtLS.CompareTo(DateTime.Now) > 0)
            {
                MessageBox.Show("预报时间太早了，是不是考虑晚点再做？");
                return;
            }
            strLS = dtLS.ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string XZID = "";
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
            paramsqx.Add("times", strToday);
            string StationID = "";
            string ZDXX = "";
            try
            {
                using (StreamReader sr1 = new StreamReader(configZDPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";
                    while ((line1 = sr1.ReadLine()) != null)
                    {
                        if (line1.Split('=').Length > 2)
                        {

                            ZDXX += line1 + '\n';
                            if (line1.Split('=')[1].Trim() == line1.Split('=')[2].Trim())
                            {
                                StationID += line1.Split('=')[1].Trim() + ',';
                            }
                            else
                            {
                                XZID += line1.Split('=')[1].Trim() + ',';
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            ZDXX = ZDXX.Substring(0, ZDXX.Length - 1);
            StationID = StationID.Substring(0, StationID.Length - 1);
            XZID = XZID.Substring(0, XZID.Length - 1); ;
            paramsqx.Add("staIds", StationID);//选择区站号
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h");// 检索要素：站名，乡镇，区站号，过去24小时最高、最低温度，降水量
            String dataFormat = "tabText";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
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

            /* 1. 定义client对象 */
            DataQueryClient client2 = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            //String userId = "BEHT_BFHT_2131";// 
            //String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId2 = "statSurfEleByStaID";
            Dictionary<String, String> params2 = new Dictionary<String, String>();
            params2.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            string strYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string timeRange1 = "(" + strYesterday + "," + strToday + "]";
            params2.Add("timeRange", timeRange1);
            params2.Add("staIds", XZID);//选择区站号，从乡镇名单中获取
            params2.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            params2.Add("statEles", "MAX_TEM_Max,MIN_TEM_MIN,SUM_PRE_1h");// 统计要素最高温度的最大值，与最低温度的最小值以及小时降水量
            /*   2.4 返回文件的格式 */
            StringBuilder retStrXZ = new StringBuilder();//返回字符串
            client2.initResources();
            // 调用接口
            int rst2 = client2.callAPI_to_serializedStr(userId, pwd, interfaceId2, params2, dataFormat, retStrXZ);
            client.destroyResources();
            string xzData = Convert.ToString(retStrXZ);

            string[] SZlinshi2 = xzData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            xzData = "";
            for (int i = 0; i < SZlinshi2.Length; i++)
            {
                if (i > 1)
                {
                    xzData += SZlinshi2[i] + '\n';
                }
            }
            xzData = xzData.Substring(0, xzData.Length - 1);
            //xzData = strData + '\n' + xzData;//将所有实况存入szData
            string[] xxSZ = ZDXX.Split('\n');
            string SKData = "";
            try
            {
                for (int i = 0; i < xxSZ.Length; i++)
                {
                    string zdLS = xxSZ[i].Split('=')[1];//该乡镇的区站号
                    string zdLS2 = xxSZ[i].Split('=')[2];//保存该乡镇订正依据的旗县区站号
                    string MainData = "";
                    string[] mainDaSZ = strData.Split('\n');
                    for (int j = 0; j < mainDaSZ.Length; j++)
                    {
                        if (mainDaSZ[j].Contains(zdLS2))
                        {
                            MainData = mainDaSZ[j].Split('\t')[3] + '\t' + mainDaSZ[j].Split('\t')[4] + '\t' + mainDaSZ[j].Split('\t')[5];
                        }
                    }

                    if (!xzData.Contains(zdLS))
                    {
                        SKData += xxSZ[i].Split('=')[0] + '\t' + xxSZ[i].Split('=')[0] + '\t' + xxSZ[i].Split('=')[1] + '\t' + MainData + '\n';
                    }
                    else
                    {
                        string[] xzSZ = xzData.Split('\n');
                        for (int j = 0; j < xzSZ.Length; j++)
                        {
                            if (xzSZ[j].Contains(zdLS))
                            {
                                string[] szLS = xzSZ[j].Split('\t');
                                if (Convert.ToDouble(szLS[3]) > 999 || Convert.ToDouble(szLS[3]) < -999 || Convert.ToDouble(szLS[4]) > 999 || Convert.ToDouble(szLS[4]) < -999)
                                {
                                    SKData += szLS[0] + '\t' + szLS[1] + '\t' + szLS[2] + '\t' + MainData + '\n';
                                }
                                else
                                {
                                    SKData += xzSZ[j] + '\n';
                                }

                            }
                        }
                    }
                }
            }
            //SKData保存实况资料
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            classSSQSJZD zdybCL = new classSSQSJZD();
            string error = "";
            string Ybdata = zdybCL.readXZYBtxt(ref error);
            string[,] YBSZ = zdybCL.ZDYBCL(Ybdata);

            string[,] ssqSZ = zdybCL.CZCL(YBSZ, strData, SKData, ref error);
            YBData = zdybCL.DCWord(ssqSZ, ZBCom.Text, FBCom.Text, QFCom.Text, ref SJsaPath);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string LS = "";
            SSQ(DateTime.Now, ref LS);
            YBtext.Text = LS;

        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                静态类.OpenBrowser(SJsaPath);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class classSSQSJZD
    {
        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\市四区\市四区配置.txt";
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\市四区\旗县乡镇.txt";
        public string readXZYBtxt(ref string error)//该方法读取城镇指导预报,返回指导预报整个内容
        {
            string YBPath = "";
            string YBdata = "";
            StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的路径配置文件中所有文本，寻找城镇指导预报路径
            while ((line = sr.ReadLine()) != null)
            {
                string[] linShi1 = line.Split('=');
                if (linShi1[0] == "城镇指导预报路径")
                {
                    YBPath = linShi1[1];
                }
            }
            sr.Close();
            string CZBWTime = "";
            using (StreamReader sr1 = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
            {

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr1.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市局读取城镇指导预报文件夹时次")
                    {
                        CZBWTime = line.Split('=')[1].Trim();
                        CZBWTime = '\\' + CZBWTime + '\\';
                    }

                }
            }
            YBPath = YBPath + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yy") + "." + DateTime.Now.ToString("MM") + CZBWTime + "呼市气象台指导预报" + DateTime.Now.ToString("MMdd") + ".txt";//文件路径为：基本路径+年后两位.月两位\06\呼市气象台指导预报+两位月两位日.txt
            //判断城镇指导预报是否存在，如果不存在，提示是否手动选择文件
            try
            {
                sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                YBdata = sr.ReadToEnd().ToString();
            }
            catch
            {
                error = YBPath + "\r\n路径错误，是否手动选择乡镇指导预报文件";
            }


            return YBdata;
        }
        public string readXZYBtxtNew(ref string error)//该方法读取城镇指导预报,返回指导预报整个内容
        {
            string YBPath = "";
            string YBdata = "";
            StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的路径配置文件中所有文本，寻找城镇指导预报路径
            while ((line = sr.ReadLine()) != null)
            {
                string[] linShi1 = line.Split('=');
                if (linShi1[0] == "城镇指导预报路径")
                {
                    YBPath = linShi1[1];
                }
            }
            sr.Close();
            string CZBWTime = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
            {

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr1.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市局读取城镇指导预报文件夹时次")
                    {
                        CZBWTime = line.Split('=')[1].Trim();
                        CZBWTime = '\\' + CZBWTime + '\\';
                    }

                }
            }
            YBPath = YBPath + DateTime.Now.ToString("yy") + "." + DateTime.Now.ToString("MM") + CZBWTime + "呼市气象台指导预报" + DateTime.Now.ToString("MMdd") + ".txt";//文件路径为：基本路径+年后两位.月两位\06\呼市气象台指导预报+两位月两位日.txt
            //判断城镇指导预报是否存在，如果不存在，提示是否手动选择文件
            try
            {
                sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                YBdata = sr.ReadToEnd().ToString();
            }
            catch
            {
                error = YBPath + "\r\n路径错误，是否手动选择乡镇指导预报文件";

            }


            return YBdata;
        }

        //返回数组每行内容为：旗县区站号+未来七天分别的天气、风向风速、最低气温、最高气温，
        public string[,] ZDYBCL(string YBData)
        {
            StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string[,] zDYBSZ = new string[intQXGS, 7 * 4 + 1];//数组行数为旗县个数，每行内容为：旗县名称+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为1+4*7

            //给每行第一列赋值，为旗县的名称
            int lineCount = 0, i = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                line = sr.ReadLine();
                if ((2 * i + 1) == lineCount)
                {
                    zDYBSZ[i++, 0] = line.Split(',')[0];
                }
                lineCount++;

            }
            sr.Close();
            string[] YBDataLines = YBData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            for (i = 0; i < intQXGS; i++)
            {
                int k = 1;
                for (int j = 0; j < YBDataLines.Length; j++)
                {
                    linShi1 = YBDataLines[j].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    //linShi1 = System.Text.RegularExpressions.Regex.Split(YBDataLines[j]，);
                    if (zDYBSZ[i, 0] == linShi1[0])
                    {
                        try
                        {
                            zDYBSZ[i, k++] = linShi1[1];
                            zDYBSZ[i, k++] = linShi1[2];
                            zDYBSZ[i, k++] = Convert.ToInt16(linShi1[3]).ToString();
                            zDYBSZ[i, k++] = Convert.ToInt16(linShi1[4]).ToString();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            lineCount = 0;
            i = 0;

            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                line = sr.ReadLine();
                if ((2 * i + 2) == lineCount)
                {
                    zDYBSZ[i++, 0] = line.Split(',')[0];
                }
                lineCount++;

            }
            sr.Close();

            return zDYBSZ;
        }//YBData为导出的指导预报内容

        public string[,] CZCL(string[,] zdybSZ, string QXSK, string XZSK, ref string strError)//输入为处理后的指导预报数组，旗县实况，各乡镇的实况.//输出数组行数为旗县乡镇个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
        {
            string QXID = "";
            int intQXGS;
            using (StreamReader sr1 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
            {
                String line1;
                line1 = sr1.ReadLine();
                string[] linShi2 = line1.Split(':');
                intQXGS = Convert.ToInt16(linShi2[1]);
            }

            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数

            //每两行第一列为旗县ID
            int lineCount = 0;
            using (StreamReader sr1 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
            {
                while (lineCount < intQXGS * 2 + 1)
                {
                    string line1 = sr1.ReadLine();
                    if ((lineCount > 1) && (lineCount % 2 == 0))
                    {
                        QXID += line1.Split(',')[0] + ',';
                    }
                    lineCount++;

                }
            }

            QXID = QXID.Substring(0, QXID.Length - 1);
            string strLS = QXSK;
            QXSK = "";
            lineCount = 0;
            using (StreamReader sr1 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
            {
                while (lineCount < intQXGS * 2 + 1)
                {
                    string line1 = sr1.ReadLine();
                    if ((lineCount > 1) && (lineCount % 2 == 0))
                    {
                        QXID = line1.Split(',')[0];
                        string[] SZlinshi = strLS.Split('\n');
                        for (int j = 0; j < SZlinshi.Length; j++)
                        {
                            if (SZlinshi[j].Contains(QXID))//判断该行是否存在该旗县区站号，如果包含，就把整行数据保存
                            {
                                QXSK += SZlinshi[j] + '\n';
                            }
                        }
                    }
                    lineCount++;

                }


            }
            QXSK = QXSK.Substring(0, QXSK.Length - 1);

            double d1 = 0, d2 = 0;
            //计算所有旗县与乡镇的个数

            string[,] szQXSK = new string[intQXGS, 5];//将旗县实况字符串转换为数组，每列分别为：站点名称、所属旗县、区站号、最高气温、最低气温(还有最后一列降水量，因为此处不用，故列数为6-1)
            int i = 0;
            for (i = 0; i < intQXGS; i++)
            {
                for (int j = 0; j < 5; j++)//注意该处，如果过去24h降水量没有时出现降水量这组为空时，列数就不是6而是5，此时强行赋值会报错
                {
                    szQXSK[i, j] = (QXSK.Split('\n')[i]).Split('\t')[j];
                }
            }
            int XZGS = 0;
            lineCount = 0;
            i = 0;
            StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                string line = sr.ReadLine();
                if ((2 * i + 1) == lineCount)
                {
                    XZGS += line.Split(',').GetLength(0);
                    i++;
                }
                lineCount++;

            }
            sr.Close();

            string[,] szYB = new string[XZGS, 30];//数组行数为旗县个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
            //给数组每行第一列赋值：旗县乡镇名称
            lineCount = 0;
            int intLS = 0;
            i = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                string line = sr.ReadLine();
                if ((2 * i + 1) == lineCount)
                {
                    for (int j = 0; j < line.Split(',').Length; j++)
                    {
                        szYB[intLS++, 0] = line.Split(',')[j];
                    }
                    i++;
                }
                lineCount++;

            }
            sr.Close();


            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            lineCount = 0;
            i = 0;
            int intHS = 0; intLS = 1;
            while (i < intQXGS)
            {
                string line = sr.ReadLine();
                if ((2 * i + 2) == lineCount)//寻找乡镇名单中的区站号行
                {
                    string strQXID = line.Split(',')[0];
                    double[] szMax = new double[7], szMin = new double[7];//7天旗县指导最高温度-旗县实况最高温度；7天旗县指导最低温度-旗县实况最低温度；
                    string[] strTQ = new string[7], strFXFS = new string[7];//指导预报7天天气与风向风速
                    int intCount1 = 0, intCount2 = 0, intCount3 = 0, intCount4 = 0;
                    for (int j = 1; j < zdybSZ.GetLength(1); j++)//因为指导预报数组和旗县实况已经按照乡镇名单排序，因此不用遍历寻找，只需与乡镇名单用一个序号i即可。遍历每列数据，保存每个时次指导实况差值
                    {
                        if ((j - 1) % 4 == 0)
                        {
                            strTQ[intCount1++] = zdybSZ[i, j];
                        }
                        else if ((j - 2) % 4 == 0)
                        {
                            strFXFS[intCount2++] = zdybSZ[i, j];
                        }
                        else if ((j - 3) % 4 == 0)
                        {
                            szMin[intCount3++] = Math.Round((Convert.ToDouble(zdybSZ[i, j]) - Convert.ToDouble(szQXSK[i, 4])), 1);//旗县实况数组编号为4的列是最低气温
                        }
                        else if ((j - 4) % 4 == 0)
                        {
                            szMax[intCount4++] = Math.Round((Convert.ToDouble(zdybSZ[i, j]) - Convert.ToDouble(szQXSK[i, 3])), 1);//旗县实况数组编号为3的列是最高气温
                        }
                    }
                    for (intLS = 1; intLS < szYB.GetLength(1); intLS++)
                    {
                        szYB[intHS, intLS] = zdybSZ[i, intLS - 1];//从旗县指导预报数组中保存该旗县的预报至整个乡镇精细化预报的数组
                    }
                    int intQXHS = intHS;//保存所属旗县的行数，为了后面做差比较温度差，防止乡镇与旗县温差过大
                    intHS++;
                    for (int j = 1; j < line.Split(',').Length; j++)//遍历该旗县每个乡镇
                    {
                        intCount1 = 0; intCount2 = 0; intCount3 = 0; intCount4 = 0;
                        double douMax = 0, douMin = 0;
                        //寻找该乡镇的最低最高温度
                        for (int k = 0; k < XZSK.Split('\n').Length; k++)
                        {
                            if (XZSK.Split('\n')[k].Contains(line.Split(',')[j]))
                            {
                                try
                                {
                                    douMin = Math.Round(Convert.ToDouble((XZSK.Split('\n')[k]).Split('\t')[4]), 1);//按换行符和制表符分割乡镇实况字符串，每行第5个为最低温，第4个为最高温度
                                }
                                catch (Exception)
                                {
                                    douMin = d1;
                                }
                                try
                                {
                                    douMax = Math.Round(Convert.ToDouble((XZSK.Split('\n')[k]).Split('\t')[3]), 1);
                                }
                                catch
                                {
                                    douMax = d2;
                                }
                                d2 = douMax;
                                d1 = douMin;
                                break;
                            }
                        }
                        for (intLS = 1; intLS < szYB.GetLength(1); intLS++)
                        {
                            if (intLS == 1)
                            {
                                szYB[intHS, intLS] = line.Split(',')[j];
                            }
                            else if ((intLS - 2) % 4 == 0)
                            {
                                szYB[intHS, intLS] = strTQ[intCount1++];
                            }
                            else if ((intLS - 3) % 4 == 0)
                            {
                                szYB[intHS, intLS] = strFXFS[intCount2++];
                            }
                            else if ((intLS - 4) % 4 == 0)
                            {
                                string QXName = "";
                                if (Math.Abs((douMin + szMin[intCount3]) - Convert.ToDouble(szYB[intQXHS, intLS])) >= 5)
                                {
                                    using (StreamReader sr3 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                                    {
                                        string line1;
                                        while ((line1 = sr3.ReadLine()) != null)
                                        {
                                            if (line1.Contains(szYB[intHS, 1]))
                                            {
                                                QXName = line1.Split(',')[0];
                                                break;
                                            }
                                        }
                                    }
                                    strError += szYB[intHS, 0] + '(' + szYB[intHS, 1] + ')' + ((intCount3 + 1) * 24).ToString() + "小时的最低温度与所属旗县" + QXName + "的最低温度相差5℃以上\r\n";//如果乡镇与旗县温度绝对值相差5度以上警告
                                }
                                szYB[intHS, intLS] = (Math.Round(douMin + szMin[intCount3++])).ToString("f0");
                            }
                            else if (((intLS - 5) % 4 == 0) && intLS != 1)
                            {
                                string QXName = "";
                                if (Math.Abs((douMax + szMax[intCount4]) - Convert.ToDouble(szYB[intQXHS, intLS])) >= 5)
                                {
                                    using (StreamReader sr3 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                                    {
                                        string line1;
                                        while ((line1 = sr3.ReadLine()) != null)
                                        {
                                            if (line1.Contains(szYB[intHS, 1]))
                                            {
                                                QXName = line1.Split(',')[0];
                                                break;
                                            }
                                        }
                                    }
                                    strError += szYB[intHS, 0] + '(' + szYB[intHS, 1] + ')' + ((intCount4 + 1) * 24).ToString() + "小时的最高温度与所属旗县" + QXName + "的最高温度相差5℃以上\r\n";//如果乡镇与旗县温度绝对值相差5度以上警告
                                }
                                szYB[intHS, intLS] = (Math.Round(douMax + szMax[intCount4++])).ToString("f0");
                            }
                        }
                        intHS++;
                    }
                    i++;
                }
                lineCount++;

            }
            sr.Close();
            string QXNameDZ = System.Environment.CurrentDirectory + @"\设置文件\市四区\指导预报与产品旗县名称对照.txt";
            using (StreamReader sr1 = new StreamReader(QXNameDZ, Encoding.GetEncoding("GB2312")))
            {
                string strLs = "";
                while ((strLs = sr1.ReadLine()) != null)
                {
                    for (int j = 0; j < szYB.GetLength(0); j++)
                    {
                        if (strLs.Contains(szYB[j, 0]))
                        {
                            szYB[j, 0] = strLs.Split('=')[1];
                        }

                    }
                }
            }
            return szYB;
        }

        public string DCWord(string[,] szYB, string ZBName, string FBName, string QFName, ref string SJsaPath)
        {
            string returnData = "";
            try
            {
                string SJMBPath = Environment.CurrentDirectory + @"\模版\市四区模板.doc";

                using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "产品发布路径")
                        {
                            SJsaPath = line.Split('=')[1];
                        }
                    }
                }
                SJsaPath += DateTime.Now.ToString("yyyy-MM") + "\\";
                if (!File.Exists(SJsaPath))
                {
                    Directory.CreateDirectory(SJsaPath);
                }
                SJsaPath += DateTime.Now.ToString("yyyyMMdd") + ".doc";
                Document doc = new Document(SJMBPath);
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.MoveToBookmark("日期");
                builder.Font.Size = 12;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                string data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 2] + "，" + szYB[i, 3] + "，" + szYB[i, 4] + "～" + szYB[i, 5] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报24");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                returnData += "24小时\r\b" + data + "\r\n";
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 6] + "，" + szYB[i, 7] + "，" + szYB[i, 8] + "～" + szYB[i, 9] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报48");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                returnData += "48小时\r\b" + data + "\r\n";
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 10] + "，" + szYB[i, 11] + "，" + szYB[i, 12] + "～" + szYB[i, 13] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报72");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                returnData += "72小时\r\b" + data + "\r\n";
                builder.MoveToBookmark("日期241");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("dd"));
                builder.MoveToBookmark("日期242");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期481");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期482");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期721");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期722");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(3).ToString("dd"));
                builder.MoveToBookmark("主班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(ZBName);
                builder.MoveToBookmark("副班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(FBName);
                builder.MoveToBookmark("签发");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(QFName);
                doc.Save(SJsaPath);
                MessageBox.Show("产品发布完成,保存路径为：\r\n" + SJsaPath);
                return returnData;


            }

            catch (Exception)
            {
                return returnData;
            }
        }

        public string DCWordNew(string[,] szYB, string ZBName, string FBName, string QFName, ref string error)
        {
            string SJsaPath = "";
            try
            {
                string SJMBPath = Environment.CurrentDirectory + @"\模版\市四区模板.doc";

                using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "产品发布路径")
                        {
                            SJsaPath = line.Split('=')[1];
                        }
                    }
                }
                SJsaPath += DateTime.Now.ToString("yyyy-MM") + "\\";
                if (!File.Exists(SJsaPath))
                {
                    Directory.CreateDirectory(SJsaPath);
                }
                SJsaPath += DateTime.Now.ToString("yyyyMMdd") + ".doc";
                Document doc = new Document(SJMBPath);
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.MoveToBookmark("日期");
                builder.Font.Size = 12;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                string data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 2] + "，" + szYB[i, 3] + "，" + szYB[i, 4] + "～" + szYB[i, 5] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报24");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 6] + "，" + szYB[i, 7] + "，" + szYB[i, 8] + "～" + szYB[i, 9] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报48");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 10] + "，" + szYB[i, 11] + "，" + szYB[i, 12] + "～" + szYB[i, 13] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报72");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                builder.MoveToBookmark("日期241");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("dd"));
                builder.MoveToBookmark("日期242");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期481");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期482");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期721");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期722");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(3).ToString("dd"));
                builder.MoveToBookmark("主班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(ZBName);
                builder.MoveToBookmark("副班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(FBName);
                builder.MoveToBookmark("签发");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(QFName);
                doc.Save(SJsaPath);
                return SJsaPath;


            }

            catch (Exception ex)
            {
                error = ex.Message;
            }

            return SJsaPath;
        }
    }

}
