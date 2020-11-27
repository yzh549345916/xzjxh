using Aspose.Words;
using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace xzjxhyb_DBmain
{
    public class SJZDCXRK
    {
        string BWBCPath = System.Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";
        string RKTime = "20";
        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
        public string readXZYBtxt(DateTime dt)//该方法读取指定日期的城镇指导预报,返回指导预报整个内容
        {
            string YBPath = "";
            string YBdata = "";
            StreamReader sr = new StreamReader(configpathPath, Encoding.Default);
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
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.Default))
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
            YBPath = YBPath + $"{dt:yyyy}\\" + dt.ToString("yy") + "." + dt.ToString("MM") + CZBWTime + "呼市气象台指导预报" + dt.ToString("MMdd") + ".txt";//文件路径为：基本路径+年后两位.月两位\06\呼市气象台指导预报+两位月两位日.txt
            //判断城镇指导预报是否存在，如果不存在，提示是否手动选择文件
            try
            {
                sr = new StreamReader(YBPath, Encoding.Default);
                YBdata = sr.ReadToEnd().ToString();
            }
            catch
            {
                MessageBoxResult result1 = System.Windows.MessageBox.Show(dt.ToString("yyyyMMdd") + "的指导预报路径错误，是否手动选择乡镇指导预报文件", "错误", MessageBoxButton.YesNo);
                if (result1 == System.Windows.MessageBoxResult.Yes)
                {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
                    {
                        Filter = "文本 (*.txt)|*.txt"
                    };
                    bool? result = openFileDialog.ShowDialog();
                    if (result == true)
                    {
                        YBPath = openFileDialog.FileName;
                        sr = new StreamReader(YBPath, Encoding.Default);
                        YBdata = sr.ReadToEnd().ToString();
                    }
                }
            }


            return YBdata;
        }


        //返回数组每行内容为：旗县区站号+未来七天分别的天气、风向风速、最低气温、最高气温，
        public string[,] ZDYBCL(string YBData)
        {
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string[,] zDYBSZ = new string[intQXGS, 7 * 4 + 1];//数组行数为旗县个数，每行内容为：旗县名称+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为1+4*7

            //给每行第一列赋值，为旗县的名称
            int lineCount = 0, i = 0;
            sr = new StreamReader(configXZPath, Encoding.Default);
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
                        zDYBSZ[i, k++] = linShi1[1];
                        zDYBSZ[i, k++] = linShi1[2];
                        zDYBSZ[i, k++] = linShi1[3];
                        zDYBSZ[i, k++] = linShi1[4];
                    }
                }
            }
            lineCount = 0;
            i = 0;

            sr = new StreamReader(configXZPath, Encoding.Default);
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

        public string CIMISSHQQXSK(DateTime dt)
        {
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string DZTime = "15";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.Default))
            {
                string line1 = "";

                // 从文件读取数据库配置信息 
                while ((line1 = sr1.ReadLine()) != null)
                {


                    if (line1.Contains("订正市局指导实况时次="))
                    {
                        DZTime = line1.Substring("订正市局指导实况时次=".Length);
                    }
                }
            }
            string strToday = dt.ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
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
            //string strYesterday = DateTime.UtcNow.AddDays(-1).ToString("yyyyMMdd000000");
            //string timeRange1 = "(" + strYesterday + "," + strToday + "]";
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

            paramsqx.Add("staIds", QXID);//选择区站号，该处后面调整，从乡镇名单中获取
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h");// 检索要素：站号、站名、最高温度、盟市、旗县
            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大，最高温降序
            /*   2.4 返回文件的格式 */
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
                            strData += SZlinshi[j] + '\n';
                        }
                    }
                }
                lineCount++;

            }
            sr.Close();
            strData = strData.Substring(0, strData.Length - 1);
            return strData;

        }

        public string CIMISSHQXZSK(DateTime dt, string strQXData, ref string strError)//函数输入为旗县CIMISS实况数据，为了当乡镇实况数据缺失时候用旗县数据暂时替换处理；输出为错误提示，提醒缺少的站点；返回为各乡镇的实况，包括数据缺失站点的临时替换数据；
        {
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string strData = "";
            string DZTime = "15";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.Default))
            {
                string line1 = "";

                // 从文件读取数据库配置信息 
                while ((line1 = sr1.ReadLine()) != null)
                {


                    if (line1.Contains("订正市局指导实况时次="))
                    {
                        DZTime = line1.Substring("订正市局指导实况时次=".Length);
                    }
                }
            }
            string strToday = dt.ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC

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
            string strYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC
            string timeRange1 = "(" + strYesterday + "," + strToday + "]";
            paramsqx.Add("timeRange", timeRange1);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
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
            paramsqx.Add("statEles", "MAX_TEM_Max,MIN_TEM_MIN,SUM_PRE_1h");// 统计要素最高温度的最大值，与最低温度的最小值以及小时降水量
            // 可选参数
            paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "tabText";
            StringBuilder retStrXZ = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, retStrXZ);
            // 释放接口服务连接资源
            client.destroyResources();
            strData = Convert.ToString(retStrXZ);

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
            /*以下程序检查导出的实况数据的站点是否完整*/
            sr = new StreamReader(configXZPath, Encoding.Default);
            lineCount = 0;
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))
                {
                    for (int i = 1; i < line.Split(',').Length; i++)
                    {
                        if (!strData.Contains(line.Split(',')[i]))//如果导出的实况数据中没有区站号为line.Split(',')[i]的站点
                        {
                            StreamReader sr1 = new StreamReader(configXZPath, Encoding.Default);//新建一个流，重新遍历乡镇名单文件，找到该乡镇对应的旗县
                            strLS = sr1.ReadToEnd();//整个乡镇名单文本
                            string[] szLS = strLS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//名单按行分组
                            strLS = szLS[lineCount - 1];//区站号前一行为站名，因此Linecount-1确认站名，列数与区站号一致，
                            szLS = strLS.Split(',');//该旗县及其乡镇的名称数组
                            strError += dt.ToString("yyyyMMdd") + szLS[i] + "(" + line.Split(',')[i] + ")的实况数据不存在，实况暂时用其旗县站点" + line.Split(',')[0] + "的实况代替，请及时确认站点信息，设置旗县站点\r\n";
                            string[] szQXData = strQXData.Split('\n');
                            for (int j = 0; j < szQXData.Length; j++)//确认站名后用乡镇的站名代替第一组，后面的内容用其所在旗县的实况代替
                            {
                                if (szQXData[j].Contains(line.Split(',')[0]))
                                {
                                    string[] szLS2 = szQXData[j].Split('\t');//保存对应旗县的实况数组
                                    for (int l = 0; l < szLS2.Length; l++)
                                    {
                                        if (l == 0)
                                            strData += '\n' + szLS[i];
                                        else if (l == 2)
                                            strData += '\t' + line.Split(',')[i];
                                        else
                                            strData += '\t' + szLS2[l];

                                    }


                                    break;
                                }
                            }
                            sr1.Close();
                        }
                    }

                }
                lineCount++;

            }
            sr.Close();

            return strData;

        }

        public string[,] CZCL(string[,] zdybSZ, string QXSK, string XZSK, ref string strError)//输入为处理后的指导预报数组，旗县实况，各乡镇的实况.//输出数组行数为旗县乡镇个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
        {

            double d1 = 0, d2 = 0;
            //计算所有旗县与乡镇的个数
            StreamReader sr = new StreamReader(configXZPath, Encoding.Default);
            String line;

            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);

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
            int lineCount = 0;
            i = 0;
            sr = new StreamReader(configXZPath, Encoding.Default);
            while (i < intQXGS)
            {
                line = sr.ReadLine();
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
            sr = new StreamReader(configXZPath, Encoding.Default);
            while (i < intQXGS)
            {
                line = sr.ReadLine();
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


            sr = new StreamReader(configXZPath, Encoding.Default);
            lineCount = 0;
            i = 0;
            int intHS = 0; intLS = 1;
            while (i < intQXGS)
            {
                line = sr.ReadLine();
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
                                szYB[intHS, intLS] = (Math.Round(douMin + szMin[intCount3++])).ToString("f1");
                            }
                            else if (((intLS - 5) % 4 == 0) && intLS != 1)
                            {
                                szYB[intHS, intLS] = (Math.Round(douMax + szMax[intCount4++])).ToString("f1");
                            }
                        }
                        intHS++;
                    }
                    i++;
                }
                lineCount++;

            }
            sr.Close();
            string QXNameDZ = System.Environment.CurrentDirectory + @"\设置文件\指导预报与产品旗县名称对照.txt";
            using (StreamReader sr1 = new StreamReader(QXNameDZ, Encoding.Default))
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

        public void ZDSZ2BW(DateTime dt, string[,] szYB)
        {
            string line = "";
            using (StreamReader sr = new StreamReader(BWBCPath, Encoding.Default))
            {


                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("市局预报读取指导预报时次"))
                    {
                        RKTime = line.Substring("市局预报读取指导预报时次=".Length);
                    }

                }
            }

            string strDate = dt.ToString("yyyy-MM-dd") + " " + RKTime + ":00:00";
            DateTime dtLS = Convert.ToDateTime(strDate);
            dtLS = dtLS.ToUniversalTime();
            string BWName = "Z_SEVP_C_BABJ_" + dt.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SCMOC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
            string CSBWPathCon = Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";

            string CSBWPath = "";
            using (StreamReader sr3 = new StreamReader(CSBWPathCon, Encoding.Default))
            {
                while ((line = sr3.ReadLine()) != null)
                {
                    if (line.Contains("市局初始报文保存地址"))
                    {
                        CSBWPath = line.Split('=')[1];
                    }
                }
            }
            CSBWPath += BWName;//存的未订正的报文的全路径
            string ZDXQConf = Environment.CurrentDirectory + @"\设置文件\旗县乡镇具体.txt";//保存站点的经纬度信息,报文中需要
            string BWData = "ZCZC\r\nFSC150 BABJ " + dtLS.ToString("ddhhmm") + "\r\n" + dtLS.ToString("yyyyMMddhh") + "时中央台指导产品\r\nSCMOC  " + dtLS.ToString("yyyyMMddhh") + "\r\n" + szYB.GetLength(0).ToString() + "\r\n";//保存报文内容
            string WeatherDZ = System.Environment.CurrentDirectory + @"\设置文件\天气对照.txt";
            string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
            string FSDZ = System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
            for (int i = 0; i < szYB.GetLength(0); i++)
            {
                string strLS = BWData;
                string ID = szYB[i, 1];
                using (StreamReader sr1 = new StreamReader(ZDXQConf, Encoding.Default))
                {
                    while ((line = sr1.ReadLine()) != null)
                    {
                        if (line.Contains('='))
                        {
                            string[] szls = (line.Split('=')[1]).Split();
                            if (szls[0] == ID)
                            {
                                try
                                {
                                    float jd = (Convert.ToSingle(szls[2])) / 100, wd = (Convert.ToSingle(szls[1])) / 100, gd = (Convert.ToSingle(szls[6])) / 10;

                                    BWData += ID + jd.ToString().PadLeft(8) + wd.ToString().PadLeft(8) + gd.ToString().PadLeft(8) + " 14 21\r\n";
                                }
                                catch (Exception)
                                {

                                }
                            }

                        }

                    }
                }
                if (BWData.Length == strLS.Length)
                {
                    BWData += ID + 0.ToString().PadLeft(8) + 0.ToString().PadLeft(8) + 0.ToString().PadLeft(8) + " 14 21\r\n";
                }

                string[] weather = new string[7], FXFS = new string[7];
                float[] Tmax = new float[7], Tmin = new float[7];
                int intLS1 = 0, intLS2 = 0, intLS3 = 0, intLS4 = 0;
                for (int j = 2; j < szYB.GetLength(1); j++)
                {
                    if ((j - 2) % 4 == 0)
                    {
                        weather[intLS1++] = szYB[i, j];
                    }
                    else if ((j - 3) % 4 == 0)
                    {
                        FXFS[intLS2++] = szYB[i, j];
                    }
                    else if ((j - 4) % 4 == 0)
                    {
                        Tmin[intLS3++] = Convert.ToSingle(szYB[i, j]);
                    }
                    else if ((j - 5) % 4 == 0)
                    {
                        Tmax[intLS4++] = Convert.ToSingle(szYB[i, j]);
                    }

                }
                intLS1 = 0; intLS2 = 0; intLS3 = 0; intLS4 = 0;
                string weatherLS1 = "", FXLS1 = "", FSLS1 = "", weatherLS2 = "", FXLS2 = "", FSLS2 = "";
                for (int j = 0; j < 14; j++)
                {
                    string line1 = "";
                    int intls = 12 * (j + 1);
                    BWData += intls.ToString().PadLeft(3);

                    if (j % 2 == 0)
                    {
                        for (int k = 0; k < 18; k++)
                        {
                            BWData += 999.9.ToString().PadLeft(6);
                        }
                        string[] WeaSz = weather[intLS1++].Split('转');
                        if (WeaSz.Length > 1)
                        {
                            weatherLS1 = WeaSz[0];
                            weatherLS2 = WeaSz[1];

                            using (StreamReader sr2 = new StreamReader(WeatherDZ, Encoding.Default))
                            {

                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Split('=')[0] == (weatherLS1))
                                    {
                                        weatherLS1 = line1.Split('=')[1];
                                        weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Split('=')[0] == (weatherLS2))
                                    {
                                        weatherLS2 = line1.Split('=')[1];
                                        weatherLS2 = (Convert.ToSingle(weatherLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                        }
                        else if (WeaSz.Length <= 1)
                        {
                            weatherLS1 = WeaSz[0];
                            using (StreamReader sr2 = new StreamReader(WeatherDZ, Encoding.Default))
                            {

                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Split('=')[0] == (weatherLS1))
                                    {
                                        weatherLS1 = line1.Split('=')[1];
                                        weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            weatherLS2 = weatherLS1;
                        }
                        BWData += weatherLS1;
                        FXFS[intLS2] = FXFS[intLS2].Replace("旋转/不定", "不定风");//因为按照“转”字拆分风向风俗，当指导预报风向出现“旋转/不定”的风时候，会导致数组拆分失败，故将其替换为“不定风”
                        if (!(FXFS[intLS2].Contains('风')) && !(FXFS[intLS2].Contains('级')))
                        {
                            FXLS1 = ""; FSLS1 = ""; FXLS2 = ""; FSLS2 = "";
                            FXLS1 = 999.9.ToString("f1").PadLeft(6);
                            FXLS2 = FXLS1;
                            FSLS1 = 999.9.ToString("f1").PadLeft(6);
                            FSLS2 = FXLS1;
                            intLS2++;
                        }//判断该时次风的内容是否为空
                        else if ((FXFS[intLS2].Split('风').Length == 3) && (FXFS[intLS2].Split('级').Length == 3))
                        {
                            string[] fxfsStr = FXFS[intLS2++].Split('转');
                            FXLS1 = fxfsStr[0].Split('风')[0] + '风';
                            FSLS1 = fxfsStr[0].Split('风')[1];
                            FXLS2 = fxfsStr[1].Split('风')[0] + '风';
                            FSLS2 = fxfsStr[1].Split('风')[1];
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FXLS2))
                                    {
                                        FXLS2 = line1.Split('=')[1];
                                        FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FSLS2))
                                    {
                                        FSLS2 = line1.Split('=')[1];
                                        FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }

                        }
                        else if (!(FXFS[intLS2].Contains('级')) && (FXFS[intLS2].Contains('风')))
                        {
                            string[] fxfsStr = FXFS[intLS2++].Split('转');
                            FXLS1 = fxfsStr[0]; FXLS2 = "";
                            FSLS1 = 999.9.ToString("f1").PadLeft(6);
                            FSLS2 = FSLS1;
                            if (fxfsStr.Length == 1)
                            {
                                using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    while ((line1 = sr2.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS1))
                                        {
                                            FXLS1 = line1.Split('=')[1];
                                            FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                        }

                                    }
                                }
                                FXLS2 = FXLS1;

                            }
                            else
                            {
                                FXLS2 = fxfsStr[1];
                                using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                                {
                                    while ((line1 = sr2.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FXLS1))
                                        {
                                            FXLS1 = line1.Split('=')[1];
                                            FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                        }
                                        else if (line1.Contains(FXLS2))
                                        {
                                            FXLS2 = line1.Split('=')[1];
                                            FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                        }

                                    }
                                }

                            }
                        }
                        else if ((FXFS[intLS2].Contains('级')) && !(FXFS[intLS2].Contains('风')))
                        {
                            string[] fxfsStr = FXFS[intLS2++].Split('转');
                            FSLS1 = fxfsStr[0]; FSLS2 = "";
                            FXLS1 = 999.9.ToString("f1").PadLeft(6);
                            FXLS2 = FXLS1;
                            if (fxfsStr.Length == 1)
                            {
                                using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    while ((line1 = sr2.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FSLS1))
                                        {
                                            FSLS1 = line1.Split('=')[1];
                                            FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                        }

                                    }
                                }
                                FSLS2 = FSLS1;

                            }
                            else
                            {
                                FSLS2 = fxfsStr[1];
                                using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                                {
                                    while ((line1 = sr2.ReadLine()) != null)
                                    {
                                        if (line1.Contains(FSLS1))
                                        {
                                            FSLS1 = line1.Split('=')[1];
                                            FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                        }
                                        else if (line1.Contains(FSLS2))
                                        {
                                            FSLS2 = line1.Split('=')[1];
                                            FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                        }

                                    }
                                }
                            }

                        }
                        else if ((FXFS[intLS2].Split('风').Length == 3) && (FXFS[intLS2].Split('级').Length == 2))
                        {
                            FSLS1 = FXFS[intLS2].Split('风')[2];
                            string[] fxfsStr = FXFS[intLS2++].Split('转');
                            FXLS1 = fxfsStr[0];
                            FXLS2 = fxfsStr[1].Split('风')[0] + '风';
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FXLS2))
                                    {
                                        FXLS2 = line1.Split('=')[1];
                                        FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            FSLS2 = FSLS1;


                        }
                        else if ((FXFS[intLS2].Split('风').Length == 2) && (FXFS[intLS2].Split('级').Length == 3))
                        {
                            FXLS1 = FXFS[intLS2].Split('风')[0] + '风';
                            string[] fxfsStr = (FXFS[intLS2++].Split('风')[1]).Split('转');
                            FSLS1 = fxfsStr[0];
                            FSLS2 = fxfsStr[1];
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FSLS2))
                                    {
                                        FSLS2 = line1.Split('=')[1];
                                        FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }
                                }
                            }
                            FXLS2 = FXLS1;
                        }
                        else if ((FXFS[intLS2].Split('风').Length == 2) && (FXFS[intLS2].Split('级').Length == 2))
                        {
                            FXLS1 = FXFS[intLS2].Split('风')[0] + '风';
                            FSLS1 = FXFS[intLS2++].Split('风')[1];
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }
                                }
                            }
                            FXLS2 = FXLS1;
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            FSLS2 = FSLS1;
                        }
                        BWData += FXLS1 + FSLS1 + "\r\n";
                    }
                    else
                    {
                        for (int k = 0; k < 10; k++)
                        {
                            BWData += 999.9.ToString().PadLeft(6);
                        }
                        BWData += Tmax[intLS3++].ToString("f1").PadLeft(6) + Tmin[intLS4++].ToString("f1").PadLeft(6);
                        for (int k = 0; k < 6; k++)
                        {
                            BWData += 999.9.ToString().PadLeft(6);
                        }
                        BWData += weatherLS2 + FXLS2 + FSLS2 + "\r\n";
                        weatherLS1 = ""; FXLS1 = ""; FSLS1 = ""; weatherLS2 = ""; FXLS2 = ""; FSLS2 = "";

                    }
                }
            }

            try
            {
                using (FileStream fsr = new FileStream(CSBWPath, FileMode.Create))
                {
                    StreamWriter sw = new StreamWriter(fsr, Encoding.Default);
                    sw.Write(BWData);
                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception)
            {

            }

        }//该方法将指导预报数组转换为报文与产品，输入数组行数为旗县乡镇个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7

        public void centerBW2ZDBW(DateTime dt)
        {
            string line = "";
            using (StreamReader sr = new StreamReader(BWBCPath, Encoding.Default))
            {


                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("市局预报读取指导预报时次"))
                    {
                        RKTime = line.Substring("市局预报读取指导预报时次=".Length);
                    }

                }
            }
            string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
            string sjBWID = "";
            using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == ("市本级"))
                    {
                        sjBWID = line.Split('=')[1];
                        break;
                    }
                }
            }
            string strDate = dt.ToString("yyyy-MM-dd") + " " + RKTime + ":00:00";
            DateTime dtLS = Convert.ToDateTime(strDate);
            dtLS = dtLS.ToUniversalTime();
            string centerBWName = "Z_SEVP_C_BABJ_" + dt.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SCMOC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
            string ZDBWName = "Z_SEVP_C_" + sjBWID + '_' + dt.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SPCC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
            string CSBWPathCon = Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";

            string CSBWPath = "", ZDBWPath = "";
            using (StreamReader sr3 = new StreamReader(CSBWPathCon, Encoding.Default))
            {
                while ((line = sr3.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市局初始报文保存地址")
                    {
                        CSBWPath = line.Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "市局指导预报报文保存地址")
                    {
                        ZDBWPath = line.Split('=')[1];
                    }
                }
            }
            CSBWPath += centerBWName;//存的未订正的报文的全路径

            string strParPath = "*" + sjBWID + "*" + dt.ToString("yyyyMMdd") + "*";
            string[] fileNameList = Directory.GetFiles(ZDBWPath, strParPath);
            for (int i = 0; i < fileNameList.Length; i++)
            {
                File.Delete(fileNameList[i]);
            }
            ZDBWPath += ZDBWName;
            string YBData = "";
            using (StreamReader sr = new StreamReader(CSBWPath, Encoding.Default))
            {
                YBData = sr.ReadToEnd();
            }
            YBData = YBData.Replace("BABJ", sjBWID);
            YBData = YBData.Replace("SCMOC", "SPCC");
            FileStream fs = new FileStream(ZDBWPath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(YBData);
            sw.Flush();
            sw.Close();
            fs.Close();
        }

        public void ZDYBBWtoSZ(DateTime dt, string YBDate)
        {
            string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
            string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string QXCPNamepath = System.Environment.CurrentDirectory + @"\设置文件\指导预报与产品旗县名称对照.txt";
            string YBpath = "";
            string line;
            using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
            {

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
            string sjBWID = "";
            using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == ("市本级"))
                    {
                        sjBWID = line.Split('=')[1];
                        break;
                    }
                }
            }
            int intCount = 0;//记录该报文中的站点数
            string strParPath = "*" + sjBWID + "*" + YBDate + "*";
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
                using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                {
                    int lineCount = 0;
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
                        }

                        lineCount++;
                    }
                }
                //该旗县所有乡镇的预报信息已经保存，开始转换为数组
                string[,] zdybSZ = new string[intCount, 14];//每行内容为：旗县名称+区站号+未来三天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*3
                using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))
                {
                    line = sr.ReadToEnd();
                }
                using (StreamReader sr = new StreamReader(QXCPNamepath, Encoding.Default))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        string[] szLS = line1.Split('=');
                        if (szLS.Length > 1)
                        {
                            line = line.Replace(szLS[0], szLS[1]);
                        }
                    }
                }
                string[] XZSZ = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                Int16 QXGS = Convert.ToInt16((XZSZ[0].Split(':'))[1]);
                int szcount = 0;//保存报文导出数组的当前序号
                for (int i = 1; i < QXGS * 2 + 1; i++)
                {
                    if (i % 2 == 0)//如果是保存ID的行
                    {
                        string[] IDSZLS = XZSZ[i].Split(',');
                        for (int j = 0; j < IDSZLS.Length; j++)
                        {
                            for (int k = 0; k < intCount; k++)
                            {
                                if (StationID[k] == IDSZLS[j])//如果该站点与乡镇文本中索引的当前ID匹配
                                {
                                    zdybSZ[szcount, 0] = XZSZ[i - 1].Split(',')[j];//名称为ID行前一行对应的位置
                                    zdybSZ[szcount, 1] = StationID[k];
                                    zdybSZ[szcount, 2] = Rain24[k];
                                    zdybSZ[szcount, 3] = FX24[k] + FS24[k];
                                    zdybSZ[szcount, 4] = Tmin24[k].ToString();
                                    zdybSZ[szcount, 5] = Tmax24[k].ToString();
                                    zdybSZ[szcount, 6] = Rain48[k];
                                    zdybSZ[szcount, 7] = FX48[k] + FS48[k];
                                    zdybSZ[szcount, 8] = Tmin48[k].ToString();
                                    zdybSZ[szcount, 9] = Tmax48[k].ToString();
                                    zdybSZ[szcount, 10] = Rain72[k];
                                    zdybSZ[szcount, 11] = FX72[k] + FS24[k];
                                    zdybSZ[szcount, 12] = Tmin72[k].ToString();
                                    zdybSZ[szcount, 13] = Tmax72[k].ToString();
                                    szcount++;
                                    break;
                                }
                            }
                        }
                    }
                }
                DCWord(dt, zdybSZ);

            }

        }//

        public void DCWord(DateTime dt, string[,] szYB)
        {
            try
            {
                string SJMBPath = Environment.CurrentDirectory + @"\模版\市局乡镇精细化预报模板.docx";
                string SJsaPath = "";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
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
                SJsaPath += dt.ToString("yyyy") + "\\" + dt.ToString("MM") + "月\\";
                if (!File.Exists(SJsaPath))
                {
                    Directory.CreateDirectory(SJsaPath);
                }
                SJsaPath += dt.ToString("MM.dd") + "发布单.docx";
                Document doc = new Document(SJMBPath);
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.MoveToBookmark("标题日期");
                builder.Font.Size = 16;
                builder.Font.Name = "宋体";
                builder.Write(dt.ToString("yyyy年MM月dd日"));

                builder.MoveToBookmark("预报24");
                builder.InsertCell();
                builder.Font.Name = "宋体";
                builder.Font.Size = 11;
                builder.Write("名称");
                builder.InsertCell();
                builder.Write("天气现象");
                builder.InsertCell();
                builder.Write("最低温度");
                builder.InsertCell();
                builder.Write("最高温度");
                builder.EndRow();
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if ((j != 1) && (j % 4 != 3))
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }


                    }
                    builder.EndRow();
                }
                builder.EndTable();
                builder.MoveToBookmark("预报48");
                builder.InsertCell();
                builder.Font.Name = "宋体";
                builder.Font.Size = 11;
                builder.Write("名称");
                builder.InsertCell();
                builder.Write("天气现象");
                builder.InsertCell();
                builder.Write("最低温度");
                builder.InsertCell();
                builder.Write("最高温度");
                builder.EndRow();
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (j == 0)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 6)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 8)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 9)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }

                    }
                    builder.EndRow();
                }
                builder.EndTable();
                builder.MoveToBookmark("预报72");
                builder.InsertCell();
                builder.Font.Name = "宋体";
                builder.Font.Size = 11;
                builder.Write("名称");
                builder.InsertCell();
                builder.Write("天气现象");
                builder.InsertCell();
                builder.Write("最低温度");
                builder.InsertCell();
                builder.Write("最高温度");
                builder.EndRow();
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    for (int j = 0; j < 14; j++)
                    {
                        if (j == 0)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 10)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 12)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }
                        else if (j == 13)
                        {
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write(szYB[i, j]);
                        }

                    }
                    builder.EndRow();
                }
                builder.EndTable();

                doc.Save(SJsaPath);

            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
