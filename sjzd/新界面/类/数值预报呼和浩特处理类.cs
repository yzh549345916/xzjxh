using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace sjzd
{
    public class 数值预报呼和浩特处理类
    {
        string _con = "";
        public 数值预报呼和浩特处理类()
        {
            CSH();
        }
        private void CSH()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            _con = util.Read("OtherConfig", "SZDB");
        }
        /// <summary>
        /// 获取指定预报要素、指定公式的数值预报的最新起报时次
        /// </summary>
        /// <param name="YBYS"></param>
        /// <param name="GS"></param>
        /// <returns></returns>
        public DateTime 获取最新时间(string YBYS, string GS)
        {
            DateTime latestDate = DateTime.MinValue;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select TOP 1 * from {YBYS} Where gs='{GS}'  ORDER BY Date DESC,SC DESC";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        Int16 mysc = sqlreader.GetInt16(sqlreader.GetOrdinal("SC"));
                        latestDate = sqlreader.GetDateTime(sqlreader.GetOrdinal("Date"));

                        latestDate = latestDate.AddHours(mysc);
                    }

                }
            }
            catch (Exception)
            {
            }

            return latestDate;
        }

        public List<IDName> 获取站点信息()
        {
            List<IDName> iDNames = new List<IDName>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = @"select * from Station  ORDER BY fatherStationID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();

                    while (sqlreader.Read())
                    {
                        Int16 level = sqlreader.GetInt16(sqlreader.GetOrdinal("Station_levl"));
                        string levelStr = "区域站";
                        if (level == 12 || level == 13)
                        {
                            levelStr = "国家站";
                        }
                        iDNames.Add(new IDName()
                        {
                            区站号 = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            站点名称 = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            所属旗县 = sqlreader.GetString(sqlreader.GetOrdinal("fatherStationID")),
                            测站级别 = level,
                            测站级别名称 = levelStr
                        });

                    }

                }
            }
            catch (Exception)
            {
            }

            return iDNames;
        }
        /// <summary>
        /// 根据时间范围、公式、时次、最小和最大时效获取温度信息
        /// </summary>
        /// <param name="sDateTime">开始时间</param>
        /// <param name="eDateTime">结束时间</param>
        /// <param name="GS">计算公式</param>
        /// <param name="scint">时次</param>
        /// <param name="minsx">最小时效</param>
        /// <param name="maxsx">最大时效</param>
        /// <returns></returns>
        public List<YBList> GetTemListsbyGSandDate(DateTime sDateTime, DateTime eDateTime, string GS, Int16 scint, Int16 minsx, Int16 maxsx)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TEM where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TEM is not null and TEM<2000 and TEM>-2000 and sx>={minsx} and sx<= {maxsx} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TEM"))
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }
        public List<YBList> GetTaxListsbyGSandDate(DateTime sDateTime, DateTime eDateTime, string GS, Int16 scint, Int16 sxint)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TMAX where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TMAX is not null and TMAX<2000 and TMAX>-2000 and sx={sxint} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TMAX"))
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }
        /// <summary>
        /// 根据时间、公式、时次、获取呼和浩特数值预报信息
        /// </summary>
        /// <param name="sDateTime">开始时间</param>
        /// <param name="GS">计算公式</param>
        /// <param name="scint">时次</param>
        /// <returns></returns>
        public List<YBList> GetTemListsbyGSandDate(DateTime sDateTime, int scint, string GS)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TEM where date>='{sDateTime:yyyy-MM-dd}' and gs='{GS}' and sc={scint} and TEM is not null and TEM<2000 and TEM>-2000 order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TEM"))
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }
        /// <summary>
        /// 根据时间范围、公式、时次、最小和最大时效获取风信息
        /// </summary>
        /// <param name="sDateTime">开始时间</param>
        /// <param name="scint">时次</param>
        /// <param name="GS">计算公式</param>
        /// <returns></returns>
        public List<YBList> GetWindListsbyGSandDate(DateTime sDateTime, int scint, string GS)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_WIN10 where date>='{sDateTime:yyyy-MM-dd}' and gs='{GS}' and sc={scint} and WIV10 is not null and WIV10<2000 and WIV10>-2000 order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")),
                            ys2 = sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")),
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }

        /// <summary>
        /// 根据时间范围、公式、时次、最小和最大时效获取极大风信息
        /// </summary>
        /// <param name="sDateTime">开始时间</param>
        /// <param name="scint">时次</param>
        /// <param name="GS">计算公式</param>
        /// <returns></returns>
        public List<YBList> GetJDWindListsbyGSandDate(DateTime sDateTime, int scint, string GS)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_WIN10JD where date>='{sDateTime:yyyy-MM-dd}' and gs='{GS}' and sc={scint} and WIV10 is not null and WIV10<2000 and WIV10>-2000 order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")),
                            ys2 = sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")),
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }
        public List<YBList> GetTminListsbyGSandDate(DateTime sDateTime, DateTime eDateTime, string GS, Int16 scint, Int16 sxint)
        {
            List<YBList> yBLists = new List<YBList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from YB_TMIN where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}' and gs='{GS}' and sc={scint} and TMIN is not null and TMIN<2000 and TMIN>-2000 and sx={sxint} order by StatioID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        Int16 sc = sqlreader.GetInt16(sqlreader.GetOrdinal("sc")),
                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("sx"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc + sx);
                        yBLists.Add(new YBList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            date = dtStr,
                            sc = sc,
                            sx = sx,
                            dateTime = dt,
                            ys = sqlreader.GetFloat(sqlreader.GetOrdinal("TMIN"))
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return yBLists;
        }
        public List<SKList> GetSKListsbyGSandDate(DateTime sDateTime, DateTime eDateTime)
        {
            List<SKList> skLists = new List<SKList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = $@"select * from SK where date>='{sDateTime.ToString("yyyy-MM-dd")}' and date<='{eDateTime.ToString("yyyy-MM-dd")}'  order by date,sc";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        DateTime dt = sqlreader.GetDateTime(sqlreader.GetOrdinal("date"));
                        string dtStr = dt.ToString("yyyy-MM-dd");

                        int sc = sqlreader.GetInt32(sqlreader.GetOrdinal("sc"));
                        dt = Convert.ToDateTime(dtStr + " 00:00:00").AddHours(sc);
                        skLists.Add(new SKList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                            dateTime = dt,
                            TEM = sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")),
                            Tmin = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin")),
                            Tmax = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax")),
                            PRE = sqlreader.GetFloat(sqlreader.GetOrdinal("PRE")),
                            FX = sqlreader.GetFloat(sqlreader.GetOrdinal("FX10")),
                            FS = sqlreader.GetFloat(sqlreader.GetOrdinal("FS10")),
                            FXJD = sqlreader.GetFloat(sqlreader.GetOrdinal("FXJD")),
                            FSJD = sqlreader.GetFloat(sqlreader.GetOrdinal("FSJD")),
                        });
                    }
                }



            }
            catch (Exception)
            {

            }
            return skLists;
        }

        public class YBList
        {
            public string id { get; set; }
            public string date { get; set; }
            public Int16 sc { get; set; }
            public Int16 sx { get; set; }
            public DateTime dateTime { get; set; }
            public float ys { get; set; }
            public float ys2 { get; set; }
        }
        public class SKList
        {
            public string id { get; set; }
            public DateTime dateTime { get; set; }
            public float TEM { get; set; }
            public float Tmax { get; set; }
            public float Tmin { get; set; }
            public float PRE { get; set; }
            public float FS { get; set; }
            public float FX { get; set; }
            public float FXJD { get; set; }
            public float FSJD { get; set; }
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
        public class IDName
        {
            public string 区站号 { get; set; }
            public string 站点名称 { get; set; }
            public string 所属旗县 { get; set; }
            public Int16 测站级别 { get; set; }
            public string 测站级别名称 { get; set; }
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
