using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Cma.Music.Client;
using Cma.Music;
using System.Threading;
using Newtonsoft.Json;

namespace sjzd.天擎
{
    public class 天擎实况
    {
        String userId = "USR_YZHWQ";
        String pwd = "YZHHGDjm3";

        public void cs()
        {
            DateTime sdateTime = Convert.ToDateTime("2021-01-11 00:00:00");
            DateTime edateTime = Convert.ToDateTime("2021-08-01 00:00:00");
            差分(sdateTime,edateTime,TimeSpan.FromDays(5));
            // 实况数据("SURF_CHN_MUL_DAY",DateTime.Now.AddDays(-10), DateTime.Now, "50934","Station_Id_C,Datetime,Year,Mon,Day,PRE_Time_2020,TEM_Avg,TEM_Max,TEM_Min,GST_Avg,GST_Avg_40cm,GST_Avg_80cm,GST_Avg_160cm,GST_Avg_320cm,Snow_Depth,Snow_PRS,WIN_S_2mi_Avg,Thund,Lit,SaSt,FlSa,FlDu,Hail,Haze,Fog,Mist");
        }

        public void 差分(DateTime sdateTime, DateTime edateTime,TimeSpan timeSpan)
        {
            
            for (DateTime dateTimeLs = sdateTime; dateTimeLs <= edateTime; dateTimeLs += timeSpan)
            {
                string data=分钟降水量(dateTimeLs, dateTimeLs+ timeSpan.Add(TimeSpan.FromSeconds(-1)), "50934");
                if (data.Length > 0)
                {
                   int countLS= data.IndexOf("\n", data.IndexOf("\n")+1);
                    string sfds= data.Remove(0, countLS + 1);
                }
            }
        }
        public void 区域气候日数据()
        {
            DateTime sdateTime = Convert.ToDateTime("1950-01-01 00:00:00");
            DateTime edateTime = Convert.ToDateTime("2021-08-01 00:00:00");
        }
        public string 实况数据(string dataCode,DateTime sdate,DateTime edate,string stationIDs, string elements)
        {
            string dataStr = "";
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2. 调用方法的参数定义，并赋值 */
            /* 2.1 用户名&密码 */

            /* 2.2 接口ID */
            String interfaceId = "getSurfEleByTimeRangeAndStaID";
            /* 2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> params1 = new Dictionary<String, String>();
            // 必选参数
            params1.Add("dataCode", dataCode); // 资料代码
            params1.Add("elements", elements); //统计要素：总降水，平均降水，总气温，平均气温
            params1.Add("timeRange", $"[{sdate:yyyyMMddHHmmss},{edate:yyyyMMddHHmmss}]"); // 检索时间
            params1.Add("staIds", stationIDs); // 检索时间

            // 可选参数
            params1.Add("orderby", "Datetime:ASC"); // 排序：按照站号从小到大
            //params1.Add("limitCnt", "10"); //返回最多记录数：10

            /* 2.4 返回数据格式 tabText */
            string dataFormat = "tabText";
            /* 2.5 返回字符串 */
            StringBuilder retStr = new StringBuilder();
            /* 3. 调用接口 */
            try
            {
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId, params1, dataFormat, retStr);
                util.ClibUtil clibUtil = new util.ClibUtil();
                // 输出结果
                if (rst == 0)
                { // 正常返回
                    try
                    {
                        string strData = retStr.ToString();
                        strData = strData.Replace("\r\n", "\n");
                        if (strData.Contains("="))
                        {
                            string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                            rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                            if (rst == 0)
                            {
                                dataStr = strData;
                            }
                        }
                            
                    }
                    catch
                    {
                    }
                }
                else
                { // 异常返回
                    Console.WriteLine("[error] StaElemSearchAPI_CLIB_callAPI_to_saveAsFile_XML.");
                }
            }
            catch (Exception e)
            {
                // 异常输出
                Console.WriteLine(e.Message);
                //e.Message();
            }
            finally
            {
                // 释放接口服务连接资源
                client.destroyResources();
            }

            return dataStr;
        }


        //没弄完呢
        public string 分钟降水量( DateTime sdate, DateTime edate, string stationIDs)
        {
            string dataReturn = "";
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2. 调用方法的参数定义，并赋值 */
            /* 2.1 用户名&密码 */

            /* 2.2 接口ID */
            String interfaceId = "getSurfEleByTimeRangeAndStaID";
            /* 2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> params1 = new Dictionary<String, String>();
            // 必选参数
            params1.Add("dataCode", "SURF_CHN_MUL_MIN"); // 资料代码
            params1.Add("elements", "Station_Id_C,Datetime,PRE"); //统计要素：总降水，平均降水，总气温，平均气温
            params1.Add("timeRange", $"[{sdate:yyyyMMddHHmmss},{edate:yyyyMMddHHmmss}]"); // 检索时间
            params1.Add("staIds", stationIDs); // 检索时间

            // 可选参数
            params1.Add("orderby", "Datetime:ASC"); // 排序：按照站号从小到大
            params1.Add("eleValueRanges", "PRE:(0,999)"); // 筛选降水量大于0
            //params1.Add("limitCnt", "10"); //返回最多记录数：10

            /* 2.4 返回数据格式 tabText */
            string dataFormat = "tabText";
            /* 2.5 返回字符串 */
            StringBuilder retStr = new StringBuilder();
            /* 3. 调用接口 */
            try
            {
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId, params1, dataFormat, retStr);
                util.ClibUtil clibUtil = new util.ClibUtil();
                // 输出结果
                if (rst == 0)
                { // 正常返回
                    try
                    {
                        string strData = retStr.ToString();
                        strData = strData.Replace("\r\n", "\n");
                        if (strData.Contains("="))
                        {
                            string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                            rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                            if (rst == 0)
                            {
                                dataReturn = strData;
                            }
                        }
                        
                    }
                    catch
                    {
                    }
                }
                else
                { // 异常返回
                    Console.WriteLine("[error] StaElemSearchAPI_CLIB_callAPI_to_saveAsFile_XML.");
                }
            }
            catch (Exception e)
            {
                // 异常输出
                Console.WriteLine(e.Message);
                //e.Message();
            }
            finally
            {
                // 释放接口服务连接资源
                client.destroyResources();
                
            }
            return dataReturn;
        }
        public void Json格式测试()
        {
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2. 调用方法的参数定义，并赋值 */
            /* 2.1 用户名&密码 */
            
            /* 2.2 接口ID */
            String interfaceId = "getSurfEleByTimeRangeAndStaID";
            /* 2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> params1 = new Dictionary<String, String>();
            // 必选参数
            params1.Add("dataCode", "SURF_CHN_MUL_DAY"); // 资料代码
            params1.Add("elements", "Station_Name,Datetime,TEM_Avg,TEM_Max,TEM_Min,PRE_Time_2020,Snow_Depth,Snow_PRS,WIN_S_2mi_Avg,GST_Avg,GST_Avg_40cm,GST_Avg_80cm,GST_Avg_160cm,GST_Avg_320cm,WEP_Sumary,WEP_Record"); //统计要素：总降水，平均降水，总气温，平均气温
            params1.Add("timeRange", "(20210601000000,20210612060000]"); // 检索时间
            params1.Add("staIds", "50934"); // 检索时间

            // 可选参数
            //params1.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            //params1.Add("limitCnt", "10"); //返回最多记录数：10

            /* 2.4 返回数据格式 tabText */
            string dataFormat = "json";
            /* 2.5 返回字符串 */
            StringBuilder retStr = new StringBuilder();
            /* 3. 调用接口 */
            try
            {
                // 初始化接口服务连接资源
                client.initResources();
                // 调用接口
                int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId, params1, dataFormat, retStr);
                util.ClibUtil clibUtil = new util.ClibUtil();
                // 输出结果
                if (rst == 0)
                { // 正常返回
                    try
                    {
                        string strData = retStr.ToString();
                        var cs=JsonConvert.DeserializeObject<日数据ReturnJson>(strData);
                        if (cs.returnCode == 0)
                        {

                        }
                    }
                    catch
                    {
                    }
                }
                else
                { // 异常返回
                    Console.WriteLine("[error] StaElemSearchAPI_CLIB_callAPI_to_saveAsFile_XML.");
                }
            }
            catch (Exception e)
            {
                // 异常输出
                Console.WriteLine(e.Message);
                //e.Message();
            }
            finally
            {
                // 释放接口服务连接资源
                client.destroyResources();
            }
        }
        public class 日数据ReturnJson
        {
            public int returnCode { get; set; }

            public string fieldNames { get; set; }

            public int rowCount { get; set; }
            public List<日数据Json> DS { get; set; }

        }

        public class 日数据Json
        {
            public string Station_Name { get; set; }
            public DateTime Datetime { get; set; }
            public double TEM_Avg { get; set; }
        }
    }
}
