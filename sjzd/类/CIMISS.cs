using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace sjzd
{
    public class CIMISS
    {
        private string _pwd = "YZHHGDJM";
        private string _userId = "BEHT_BFHT_2131";

        public List<YS> 获取小时温度(DateTime sDate, DateTime eDate, string adminCodes)
        {
            List<YS> ySs = new List<YS>();
            string myData = CIMISS_SK_Hour_byTimeRangeAndRegion_SURF_CHN_MUL_HOR(sDate, eDate, adminCodes, "TEM,TEM_Max,TEM_Max_OTime,TEM_Min,TEM_Min_OTime");
            if (myData.Trim().Length == 0)
                return ySs;
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return ySs;

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls < 999900)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);

                        if (ySs.Exists(y => y.DateTime == dtls.ToLocalTime() && y.StationID == szls[1]))
                            break;
                        ySs.Add(new YS
                        {
                            StationID = szls[1],
                            DateTime = dtls.ToLocalTime(),
                            TEM = fls,
                            TEM_Max = Convert.ToSingle(szls[3]),
                            TEM_Min = Convert.ToSingle(szls[5]),
                            TEM_Max_OTime = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[4])).ToLocalTime(),
                            TEM_Min_OTime = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[6])).ToLocalTime()
                        });
                    }
                }
                catch
                {
                }
            }

            return ySs;
        }


        public List<ECTEF0> 获取EC温度(DateTime sDate, string ID)
        {
            List<ECTEF0> ySs = new List<ECTEF0>();
            string myData = CIMISS_EC_byStaID(sDate, ID, "TEF0");
            if (myData.Trim().Length == 0)
                return ySs;
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return ySs;

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    double fls = Convert.ToDouble(szls[4]);
                    if (fls < 999900)
                    {

                        DateTime dtls = Convert.ToDateTime($"{szls[3].Substring(0, 4)}-{szls[3].Substring(4, 2)}-{szls[3].Substring(6, 2)} {szls[3].Substring(8, 2)}:{szls[3].Substring(10, 2)}:{szls[3].Substring(12, 2)}");

                        if (ySs.Exists(y => y.DateTime == dtls.ToLocalTime() && y.StationID == szls[1]))
                            break;
                        ySs.Add(new ECTEF0
                        {
                            StationID = szls[2],
                            DateTime = dtls.ToLocalTime(),
                            TEM = Math.Round(fls - 273.15, 2),

                        });
                    }
                }
                catch
                {
                }
            }

            return ySs;
        }

        public DateTime 获取EC2米温度最新时间()
        {
            DateTime latestDate = DateTime.MinValue;
            string myData = CIMISS_EC文件("TEF0");
            if (myData.Trim().Length == 0)
                return DateTime.MinValue;
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return DateTime.MinValue;

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    DateTime dateTimeLS = Convert.ToDateTime(szData[i].Substring(szData[i].Length - 19, 19));
                    if (latestDate.CompareTo(dateTimeLS) < 0)
                        latestDate = dateTimeLS;


                }
                catch
                {
                }
            }

            return latestDate.ToLocalTime();
        }
        public List<PreYS> 获取小时降水量(DateTime sDate, DateTime eDate, string adminCodes)
        {
            List<PreYS> ySs = new List<PreYS>();
            string myData = CIMISS_SK_Hour_byTimeRangeAndRegion_SURF_CHN_MUL_HOR(sDate, eDate, adminCodes, "PRE_1h");
            if (myData.Trim().Length == 0)
                return ySs;
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return ySs;

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[2]);
                    if (fls >= 0 && fls < 999990)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);

                        if (ySs.Exists(y => y.DateTime == dtls.ToLocalTime() && y.StationID == szls[1]))
                            break;
                        ySs.Add(new PreYS
                        {
                            StationID = szls[1],
                            DateTime = dtls.ToLocalTime(),
                            Pre = fls,

                        });
                    }
                }
                catch
                {
                }
            }

            return ySs;
        }
        public List<FYS> 获取小时风(DateTime sDate, DateTime eDate, string adminCodes)
        {
            List<FYS> ySs = new List<FYS>();
            string myData = CIMISS_SK_Hour_byTimeRangeAndRegion_SURF_CHN_MUL_HOR(sDate, eDate, adminCodes, "WIN_D_Avg_2mi,WIN_S_Avg_2mi,WIN_D_Avg_10mi,WIN_S_Avg_10mi,WIN_D_S_Max,WIN_S_Max,WIN_S_Max_OTime,WIN_D_INST,WIN_S_INST,WIN_D_INST_Max,WIN_S_Inst_Max,WIN_S_INST_Max_OTime");
            if (myData.Trim().Length == 0)
                return ySs;
            string[] szData = myData.Split(new[]
            {
                '\n'
            }, StringSplitOptions.RemoveEmptyEntries);
            if (szData.Length <= 2)
                return ySs;

            for (int i = 2; i < szData.Length; i++)
            {
                try
                {
                    string[] szls = szData[i].Split('\t');
                    float fls = Convert.ToSingle(szls[3]);
                    if (fls >= 0 && fls < 999990)
                    {
                        DateTime dtls = Convert.ToDateTime(szls[0]);

                        if (ySs.Exists(y => y.DateTime == dtls.ToLocalTime() && y.StationID == szls[1]))
                            break;
                        ySs.Add(new FYS
                        {
                            StationID = szls[1],
                            DateTime = dtls.ToLocalTime(),
                            WIN_D_Avg_2mi = Convert.ToSingle(szls[2]),
                            WIN_S_Avg_2mi = fls,
                            WIN_D_Avg_10mi = Convert.ToSingle(szls[4]),
                            WIN_S_Avg_10mi = Convert.ToSingle(szls[5]),
                            WIN_D_S_Max = Convert.ToSingle(szls[6]),
                            WIN_S_Max = Convert.ToSingle(szls[7]),
                            WIN_S_Max_OTime = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[8])).ToLocalTime(),
                            WIN_D_INST = Convert.ToSingle(szls[9]),
                            WIN_S_INST = Convert.ToSingle(szls[10]),
                            WIN_D_INST_Max = Convert.ToSingle(szls[11]),
                            WIN_S_Inst_Max = Convert.ToSingle(szls[12]),
                            WIN_S_INST_Max_OTime = Convert.ToDateTime(CIMISSDateTimeMinuteConvert(dtls, szls[13])).ToLocalTime()
                        });
                    }
                }
                catch
                {
                }
            }

            return ySs;
        }

        public DateTime CIMISSDateTimeMinuteConvert(DateTime dateTime, string time)
        {
            DateTime myDate = dateTime;
            time = time.PadLeft(4, '0');
            try
            {
                myDate = dateTime.Date.AddHours(Convert.ToInt32(time.Substring(0, 2))).AddMinutes(Convert.ToInt32(time.Substring(2, 2)));
                if ((myDate - dateTime).TotalHours > 2)
                    myDate = myDate.AddDays(-1);
            }
            catch
            {
            }

            return myDate;
        }

        /// <summary>
        /// 根据区站号，起止时间从CIMISS通过SURF_CHN_MUL_HOR获取小时数据
        /// </summary>
        /// <param name="sDate">开始时间</param>
        /// <param name="eDate">结束时间</param>
        /// <param name="adminCodes">区站号</param>
        /// /// <param name="elements">要素代码</param>
        /// <returns>CIMISS返回的数据</returns>
        public string CIMISS_SK_Hour_byTimeRangeAndRegion_SURF_CHN_MUL_HOR(DateTime sDate, DateTime eDate, string adminCodes, string elements)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                string userId = _userId; // 
                string pwd = _pwd; // 
                /*   2.2 接口ID */
                string interfaceId1 = "getSurfEleByTimeRangeAndStaID";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<string, string> paramsqx = new Dictionary<string, string>();
                // 必选参数
                paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + sDate.ToUniversalTime().ToString("yyyyMMddHHmm00,") + eDate.ToUniversalTime().ToString("yyyyMMddHHmm00]"));
                paramsqx.Add("elements", "Datetime,Station_Id_C," + elements);
                paramsqx.Add("staIds", adminCodes);
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                string dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder(); //返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }

                    strData = "";
                }
                catch
                {
                    strData = "";
                }

                return strData;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string CIMISS_EC_byStaID(DateTime sDate, string ID, string elements)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                string userId = _userId; // 
                string pwd = _pwd; // 
                /*   2.2 接口ID */
                string interfaceId1 = "getNafpEleByTimeAndLevelAndValidtimeRangeAndStaID";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<string, string> paramsqx = new Dictionary<string, string>();
                // 必选参数
                paramsqx.Add("dataCode", "NAFP_FOR_FTM_HIGH_EC_GLB"); // 资料代码
                //检索时间段
                paramsqx.Add("time", sDate.ToUniversalTime().ToString("yyyyMMddHHmm00"));
                paramsqx.Add("fcstEle", elements);
                paramsqx.Add("staIds", ID);
                paramsqx.Add("fcstLevel", "0");
                paramsqx.Add("minVT", "0");
                paramsqx.Add("maxVT", "168");
                // 可选参数
                //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
                /*   2.4 返回文件的格式 */
                string dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder(); //返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }

                    strData = "";
                }
                catch
                {
                    strData = "";
                }

                return strData;
            }
            catch (Exception)
            {
                return "";
            }
        }
        /// <summary>
        /// 获取欧洲中心数值预报产品-高分辨率C1D-全球，指定预报要素过去24小时的产品文件信息
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public string CIMISS_EC文件(string elements)
        {
            try
            {
                /* 1. 定义client对象 */
                DataQueryClient client = new DataQueryClient();

                /* 2.   调用方法的参数定义，并赋值 */
                /*   2.1 用户名&密码 */
                string userId = _userId; // 
                string pwd = _pwd; // 
                /*   2.2 接口ID */
                string interfaceId1 = "getNafpFileByElementAndTimeRange";
                /*   2.3 接口参数，多个参数间无顺序 */
                Dictionary<string, string> paramsqx = new Dictionary<string, string>();
                // 必选参数
                paramsqx.Add("dataCode", "NAFP_FOR_FTM_HIGH_EC_GLB"); // 资料代码
                //检索时间段
                paramsqx.Add("timeRange", '[' + DateTime.Now.AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000,") + DateTime.Now.ToUniversalTime().ToString("yyyyMMddHH0000]"));
                paramsqx.Add("fcstEle", elements);
                paramsqx.Add("elements", "Datetime");
                /*   2.4 返回文件的格式 */
                string dataFormat = "tabText";
                StringBuilder QXSK = new StringBuilder(); //返回字符串
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
                // 释放接口服务连接资源
                client.destroyResources();
                paramsqx = null;
                string strData = Convert.ToString(QXSK);
                QXSK = null;
                try
                {
                    strData = strData.Replace("\r\n", "\n");
                    string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                    rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                    if (rst == 0)
                    {
                        return strData;
                    }

                    strData = "";
                }
                catch
                {
                    strData = "";
                }

                return strData;
            }
            catch (Exception)
            {
                return "";
            }
        }


        public class YS
        {
            public string StationID { get; set; }
            public DateTime DateTime { get; set; }
            public float TEM { get; set; }
            public float TEM_Max { get; set; }
            public float TEM_Min { get; set; }
            public DateTime TEM_Max_OTime { get; set; }
            public DateTime TEM_Min_OTime { get; set; }
        }
        public class PreYS
        {
            public string StationID { get; set; }
            public DateTime DateTime { get; set; }
            public float Pre { get; set; }

        }
        public class ECTEF0
        {
            public string StationID { get; set; }
            public DateTime DateTime { get; set; }
            public double TEM { get; set; }

        }

        public class FYS
        {
            public string StationID { get; set; }
            public DateTime DateTime { get; set; }
            public float WIN_D_Avg_2mi { get; set; }
            public float WIN_S_Avg_2mi { get; set; }
            public float WIN_D_Avg_10mi { get; set; }
            public float WIN_S_Avg_10mi { get; set; }
            public float WIN_D_S_Max { get; set; }
            public float WIN_S_Max { get; set; }
            public DateTime WIN_S_Max_OTime { get; set; }
            public float WIN_D_INST { get; set; }
            public float WIN_S_INST { get; set; }
            public float WIN_D_INST_Max { get; set; }
            public float WIN_S_Inst_Max { get; set; }
            public DateTime WIN_S_INST_Max_OTime { get; set; }
        }
    }
}
