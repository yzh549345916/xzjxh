using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace sjzd
{
    class 区局智能网格
    {
        private string con = "";
        public 区局智能网格()
        {
            CSH();
        }

        public void CSH()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            con = util.Read("OtherConfig", "DB");

        }

        public string[,] CLZNData(string[,] YBSZ, ref string error)
        {
            bool bsBool = false;//省级智能网格是否入库
            try
            {
                string strTime = DateTime.Now.ToString("yyyy-MM-dd");
                Int16 sc = 20;
                ConfigClass1 configClass1 = new ConfigClass1();
                string QXIDName = configClass1.IDName(-1);
                string strData = "";
                int XZGS = YBSZ.GetLength(0);
                for (int i = 0; i < YBSZ.GetLength(0); i++)
                {
                    string QXName = "武川";
                    string QXID = YBSZ[i, 0], QXTQ24 = YBSZ[i, 1], QXF24 = YBSZ[i, 2], QXTQ48 = YBSZ[i, 5], QXF48 = YBSZ[i, 6], QXTQ72 = YBSZ[i, 9], QXF72 = YBSZ[i, 10];

                    foreach (string ss in QXIDName.Split('\n'))
                    {
                        if (ss.Split(',')[0].Trim() == QXID.Trim())
                        {
                            QXName = ss.Split(',')[1];
                            break;
                        }
                    }
                    strData += QXName + ',';
                    for (int j = 0; j < YBSZ.GetLength(1); j++)
                    {
                        strData += YBSZ[i, j] + ',';
                    }

                    strData = strData.Substring(0, strData.Length - 1);
                    strData += '\n';
                    string XZIDName = configClass1.IDName(Convert.ToInt32(QXID));
                    string xzID = "";
                    foreach (string ss in XZIDName.Split('\n'))
                    {
                        xzID += '\'' + ss.Split(',')[0] + "\',";
                    }

                    xzID = xzID.Substring(0, xzID.Length - 1);
                    List<YSList> dataList = HQYS(strTime, sc, xzID);


                    XZGS += XZIDName.Split('\n').Length;
                    if (dataList.Count > 0)
                    {
                        bsBool = true;
                        foreach (string ss in XZIDName.Split('\n'))
                        {
                            YSList q24 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 24));
                            YSList q48 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 48));
                            YSList q72 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 72));
                            strData += ss.Split(',')[1] + ',' + ss.Split(',')[0] + ',' + QXTQ24 + ',' + QXF24 + ','
                                       + (q24 == null ? "-99" : q24.TMIN.ToString()) + ',' + (q24 == null ? "-99" : q24.TMAX.ToString()) + ','
                                       + QXTQ48 + ',' + QXF48 + ','
                                       + (q48 == null ? "-99" : q48.TMIN.ToString()) + ',' + (q48 == null ? "-99" : q48.TMAX.ToString()) + ','
                                       + QXTQ72 + ',' + QXF72 + ','
                                       + (q72 == null ? "-99" : q72.TMIN.ToString()) + ',' + (q72 == null ? "-99" : q72.TMAX.ToString()) + ',' + YBSZ[i, 13] + ',' + YBSZ[i, 14] + ",-99,-99," + YBSZ[i, 17] + ',' + YBSZ[i, 18] + ",-99,-99," + YBSZ[i, 21] + ',' + YBSZ[i, 22] + ",-99,-99," + YBSZ[i, 25] + ',' + YBSZ[i, 26] + ",-99,-99\n";

                        }
                    }
                    else
                    {
                        foreach (string ss in XZIDName.Split('\n'))
                        {

                            strData += ss.Split(',')[1] + ',' + ss.Split(',')[0] + ',' + QXTQ24 + ',' + QXF24 + ','
                                       + -99 + ',' + -99 + ','
                                       + QXTQ48 + ',' + QXF48 + ','
                                       + -99 + ',' + -99 + ','
                                       + QXTQ72 + ',' + QXF72 + ','
                                       + -99 + ',' + -99 + ',' + YBSZ[i, 13] + ',' + YBSZ[i, 14] + ",-99,-99," + YBSZ[i, 17] + ',' + YBSZ[i, 18] + ",-99,-99," + YBSZ[i, 21] + ',' + YBSZ[i, 22] + ",-99,-99," + YBSZ[i, 25] + ',' + YBSZ[i, 26] + ",-99,-99\n";

                        }
                    }


                }
                strData = strData.Substring(0, strData.Length - 1);
                string[,] szYB = new string[XZGS, 30];
                string[] sz1 = strData.Split('\n');
                for (int i = 0; i < sz1.Length; i++)
                {
                    string[] sz2 = sz1[i].Split(',');
                    for (int j = 0; j < szYB.GetLength(1); j++)
                    {
                        szYB[i, j] = sz2[j];
                    }
                }
                if (!bsBool)
                    error = strTime + sc + "时省级智能网格数据获取失败，请查询数据是否入库。\r\n报文正常生成，但是乡镇报文温度为-99";
                return szYB;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return new string[,] { };
        }

        public string HQStationByID(string stationID)
        {
            string Data = "";
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from Station where StatioID='{0}'", stationID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        Data = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")) + ' ' + sqlreader.GetString(sqlreader.GetOrdinal("Name")) + ' ' + sqlreader.GetDouble(sqlreader.GetOrdinal("JD")) + ' ' + sqlreader.GetDouble(sqlreader.GetOrdinal("WD")) + ' ' + sqlreader.GetDouble(sqlreader.GetOrdinal("High"));
                    }
                }
                catch
                {
                }
            }
            return Data;
        }
        public void SaveStation(string stationID, string name, Int16 stationlevel, double Lon, double Lat, double High)
        {
            Stopwatch sw = new Stopwatch();
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    string sql = "insert into Station(StatioID,Name,Station_levl,WD,JD,High) VALUES(@id,@name,@stationlev,@wd,@jd,@high)";
                    mycon.Open();//打开
                    int jlCount = 0;
                    sql = "insert into Station(StatioID,Name,Station_levl,WD,JD,High) VALUES(@id,@name,@stationlev,@wd,@jd,@high)";
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        sqlman.Parameters.AddWithValue("@id", stationID);
                        sqlman.Parameters.AddWithValue("@name", name);
                        sqlman.Parameters.AddWithValue("@stationlev", stationlevel);
                        sqlman.Parameters.AddWithValue("@wd", Lat);
                        sqlman.Parameters.AddWithValue("@jd", Lon);
                        sqlman.Parameters.AddWithValue("@high", High);
                        sw.Start();

                        try
                        {
                            jlCount = sqlman.ExecuteNonQuery();
                        }
                        catch
                        {

                        }

                    }
                    if (jlCount == 0)//如果插入失败，则说明已经存在，进行更新字段操作
                    {
                        try
                        {
                            sql = @"update Station set name=@name ,Station_levl=@stationlev,wd=@wd,jd=@jd,high=@high where StatioID=@id";
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                            {
                                sqlman.Parameters.AddWithValue("@id", stationID);
                                sqlman.Parameters.AddWithValue("@name", name);
                                sqlman.Parameters.AddWithValue("@stationlev", stationlevel);
                                sqlman.Parameters.AddWithValue("@wd", Lat);
                                sqlman.Parameters.AddWithValue("@jd", Lon);
                                sqlman.Parameters.AddWithValue("@high", High);
                                sw.Start();
                                try
                                {
                                    jlCount = sqlman.ExecuteNonQuery();
                                }
                                catch
                                {

                                }

                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        if (jlCount == 0)//如果更新字段失败，说明连接数据库失败，则保存至登陆信息至发报文件夹
                        {
                            MessageBox.Show("连接数据库失败");
                        }

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }
        public string CIMISS_ZDbyID(string StationID, ref string error)
        {

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getStaInfoByStaId";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "STA_INFO_SURF_CHN"); // 资料代码
                                                           //检索时间段
            paramsqx.Add("staIds", StationID);//选择区站号
            //此处增加风要素
            paramsqx.Add("elements", "Station_Id_C,Station_Name,Station_levl,Lat,Lon,Alti");// 检索要素：站号、站名、过去24h最高、最低气温、24小时降水量
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
            string strLS = strData.Split('"')[1];
            rst = Convert.ToInt32(strLS);

            try
            {
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
                }
                else
                {
                    error += strData + "\r\n";
                    strData = "";
                }
            }
            catch
            {
                strData = "";
            }

            return strData;

        }

        public double HQYS(string strTime, Int16 sc, Int16 sx, string ysName, string stationID)
        {
            double f1 = -99;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from 省级格点预报订正产品 where StatioID='{0}' and Date='{1}' and SC='{2}' and SX='{3}'", stationID, strTime, sc, sx);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        f1 = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal(ysName)), 4);
                    }
                }
                catch
                {
                }
            }
            return f1;
        }
        /// <summary>
        /// 根据时间、时次、站点号返回最高最低温度
        /// </summary>
        /// <param name="strTime">yyyy-MM-dd</param>
        /// <param name="sc">待查询的时次08或者20</param>
        /// <param name="stationID">区站号字符串，以需要加单引号，以逗号分隔。例如：'53464','C4531'</param>
        /// <returns>返回List YSList，ID、时效、最高、最低气温</returns>
        public List<YSList> HQYS(string strTime, Int16 sc, string stationID)
        {
            List<YSList> ySLists = new List<YSList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {
                        mycon.Open();//打开
                        string sql = String.Format("select * from 省级格点预报订正产品 where StatioID in ({0}) and Date='{1}' and SC='{2}' and SX in (24,48,72)", stationID, strTime, sc);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            ySLists.Add(new YSList()
                            {
                                ID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                                sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX")),
                                TMAX = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TMAX")), 2),
                                TMIN = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TMIN")), 2),
                            });
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }



            return ySLists;
        }

        public class YSList
        {
            public string ID { get; set; }
            public Int16 sx { get; set; }
            public double TMAX { get; set; }
            public double TMIN { get; set; }
        }
        /// <summary>
        /// 返回指定区站号范围内省级智能网格三天的最高最低气温的准确率
        /// </summary>
        /// <param name="StartDate">开始时间yyyy-MM-dd</param>
        /// <param name="EndDate">结束时间yyyy-MM-dd</param>
        /// <param name="QXID">区站号，以半角逗号分隔</param>
        /// <returns></returns>
        public float[] SJZQL(string StartDate, string EndDate, String QXID)//返回指定区站号、指定时间段市局三天预报的最高、最低、晴雨准确率以及缺报率
        {
            string[] strLS = QXID.Split(',');
            QXID = "";
            foreach (string ss1 in strLS)
            {
                QXID = QXID + '\'' + ss1 + "\',";
            }

            QXID = QXID.Substring(0, QXID.Length - 1);
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[7];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from 统计 where StationID in ({0}) AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    try
                                    {
                                        sjzqlTJ1.Add(new ZQLTJ1()
                                        {

                                            SJ24TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax24")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax24")),
                                            SJ24TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin24")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin24")),
                                            SJ48TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax48")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax48")),
                                            SJ48TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin48")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin48")),
                                            SJ72TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax72")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax72")),
                                            SJ72TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin72")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin72")),

                                        });
                                    }
                                    catch
                                    {

                                    }
                                }

                            }
                        }

                    }
                    catch (Exception)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception)
            {

            }
            int ss = sjzqlTJ1.Count;

            try
            {
                int[] zs = new int[6];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
                foreach (ZQLTJ1 sjzql in sjzqlTJ1)
                {
                    if (sjzql.SJ24TmaxZQL < 999998 && sjzql.SJ24TmaxZQL > -999999)
                    {
                        zs[0]++;
                        if (Math.Abs(sjzql.SJ24TmaxZQL) <= 2)
                        {
                            tjsz[0]++;
                        }
                    }
                    else if (sjzql.SJ24TmaxZQL == -999999)
                    {
                        tjsz[6]++;
                    }
                    if (sjzql.SJ24TminZQL < 999998 && sjzql.SJ24TminZQL > -999999)
                    {
                        zs[1]++;
                        if (Math.Abs(sjzql.SJ24TminZQL) <= 2)
                        {
                            tjsz[1]++;
                        }
                    }

                    if (sjzql.SJ48TmaxZQL < 999998 && sjzql.SJ48TmaxZQL > -999999)
                    {
                        zs[2]++;
                        if (Math.Abs(sjzql.SJ48TmaxZQL) <= 2)
                        {
                            tjsz[2]++;
                        }
                    }
                    if (sjzql.SJ48TminZQL < 999998 && sjzql.SJ48TminZQL > -999999)
                    {
                        zs[3]++;
                        if (Math.Abs(sjzql.SJ48TminZQL) <= 2)
                        {
                            tjsz[3]++;
                        }
                    }
                    if (sjzql.SJ72TmaxZQL < 999998 && sjzql.SJ72TmaxZQL > -999999)
                    {
                        zs[4]++;
                        if (Math.Abs(sjzql.SJ72TmaxZQL) <= 2)
                        {
                            tjsz[4]++;
                        }
                    }
                    if (sjzql.SJ72TminZQL < 999998 && sjzql.SJ72TminZQL > -999999)
                    {
                        zs[5]++;
                        if (Math.Abs(sjzql.SJ72TminZQL) <= 2)
                        {
                            tjsz[5]++;
                        }
                    }

                }
                for (int i = 0; i < tjsz.Length; i++)
                {
                    if (i < tjsz.Length - 1)
                    {
                        tjsz[i] = tjsz[i] / zs[i];
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                    else
                    {
                        tjsz[i] = tjsz[i] / sjzqlTJ1.Count;
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return tjsz;
        }

        public float[] SJQSZQL(string StartDate, string EndDate)//返回指定区站号、指定时间段市局三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[7];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from 统计 where  Date>='{0}' AND Date<='{1}'", StartDate, EndDate);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    try
                                    {
                                        sjzqlTJ1.Add(new ZQLTJ1()
                                        {

                                            SJ24TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax24")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax24")),
                                            SJ24TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin24")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin24")),
                                            SJ48TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax48")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax48")),
                                            SJ48TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin48")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin48")),
                                            SJ72TmaxZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmax72")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmax72")),
                                            SJ72TminZQL = sqlreader.IsDBNull(sqlreader.GetOrdinal("QJZN_SKTmin72")) ? 999999 : sqlreader.GetFloat(sqlreader.GetOrdinal("QJZN_SKTmin72")),

                                        });
                                    }
                                    catch
                                    {

                                    }
                                }

                            }
                        }

                    }
                    catch (Exception)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception)
            {

            }
            int ss = sjzqlTJ1.Count;

            try
            {
                int[] zs = new int[6];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
                foreach (ZQLTJ1 sjzql in sjzqlTJ1)
                {
                    if (sjzql.SJ24TmaxZQL < 999998 && sjzql.SJ24TmaxZQL > -999999)
                    {
                        zs[0]++;
                        if (Math.Abs(sjzql.SJ24TmaxZQL) <= 2)
                        {
                            tjsz[0]++;
                        }
                    }
                    else if (sjzql.SJ24TmaxZQL == -999999)
                    {
                        tjsz[6]++;
                    }
                    if (sjzql.SJ24TminZQL < 999998 && sjzql.SJ24TminZQL > -999999)
                    {
                        zs[1]++;
                        if (Math.Abs(sjzql.SJ24TminZQL) <= 2)
                        {
                            tjsz[1]++;
                        }
                    }

                    if (sjzql.SJ48TmaxZQL < 999998 && sjzql.SJ48TmaxZQL > -999999)
                    {
                        zs[2]++;
                        if (Math.Abs(sjzql.SJ48TmaxZQL) <= 2)
                        {
                            tjsz[2]++;
                        }
                    }
                    if (sjzql.SJ48TminZQL < 999998 && sjzql.SJ48TminZQL > -999999)
                    {
                        zs[3]++;
                        if (Math.Abs(sjzql.SJ48TminZQL) <= 2)
                        {
                            tjsz[3]++;
                        }
                    }
                    if (sjzql.SJ72TmaxZQL < 999998 && sjzql.SJ72TmaxZQL > -999999)
                    {
                        zs[4]++;
                        if (Math.Abs(sjzql.SJ72TmaxZQL) <= 2)
                        {
                            tjsz[4]++;
                        }
                    }
                    if (sjzql.SJ72TminZQL < 999998 && sjzql.SJ72TminZQL > -999999)
                    {
                        zs[5]++;
                        if (Math.Abs(sjzql.SJ72TminZQL) <= 2)
                        {
                            tjsz[5]++;
                        }
                    }

                }
                for (int i = 0; i < tjsz.Length; i++)
                {
                    if (i < tjsz.Length - 1)
                    {
                        tjsz[i] = tjsz[i] / zs[i];
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                    else
                    {
                        tjsz[i] = tjsz[i] / sjzqlTJ1.Count;
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return tjsz;
        }
        public class ZQLTJ1//统计信息列表
        {
            public string Name { get; set; }
            public float SJ24TmaxZQL { get; set; }
            public float SJ24TminZQL { get; set; }
            public float SJ48TmaxZQL { get; set; }
            public float SJ48TminZQL { get; set; }
            public float SJ72TmaxZQL { get; set; }
            public float SJ72TminZQL { get; set; }

        }
    }
}
