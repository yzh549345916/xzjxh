using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace xzjxhyb_DBmain
{
    class 区局智能网格
    {
        private string con = "";
        public 区局智能网格()
        {
            CSH();

            

        }
        /// <summary>

        /// Convert a List{T} to a DataTable.

        /// </summary>

        private DataTable ToDataTable<T>(List<T> items)

        {

            var tb = new DataTable(typeof(T).Name);



            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);



            foreach (PropertyInfo prop in props)

            {

                Type t = GetCoreType(prop.PropertyType);

                tb.Columns.Add(prop.Name, t);

            }



            foreach (T item in items)

            {

                var values = new object[props.Length];



                for (int i = 0; i < props.Length; i++)

                {

                    values[i] = props[i].GetValue(item, null);

                }



                tb.Rows.Add(values);

            }



            return tb;

        }



        /// <summary>

        /// Determine of specified type is nullable

        /// </summary>

        public static bool IsNullable(Type t)

        {

            return !t.IsValueType || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>));

        }



        /// <summary>

        /// Return underlying type if type is Nullable otherwise return the type

        /// </summary>

        public static Type GetCoreType(Type t)

        {

            if (t != null && IsNullable(t))

            {

                if (!t.IsValueType)

                {

                    return t;

                }

                else

                {

                    return Nullable.GetUnderlyingType(t);

                }

            }

            else

            {

                return t;

            }

        }

        public void CSH()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            con = util.Read("OtherConfig", "DB");
            
        }

        /// <summary>
        /// 根据时间、时次、时效、站点号返回最高最低温度
        /// </summary>
        /// <param name="strTime">yyyy-MM-dd</param>
        /// <param name="sc">待查询的时次08或者20</param>
        /// <param name="sx">待查询的时效24、48、72</param>
        /// <param name="stationID">区站号字符串，以需要加单引号，以逗号分隔。例如：'53464','C4531'</param>
        /// <returns>返回List YSList，ID、时效、最高、最低气温</returns>
        public List<YSList> HQYS(string strTime, Int16 sc,Int16 sx, string stationID)
        {
            List<YSList> ySLists = new List<YSList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {
                        mycon.Open();//打开
                        string sql = String.Format("select * from 省级格点预报订正产品 where StatioID in ({0}) and Date='{1}' and SC='{2}' and SX in ({3})", stationID, strTime, sc,sx);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            ySLists.Add(new YSList()
                            {

                                ID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                                TMAX = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TMAX")), 2),
                                TMIN = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TMIN")), 2),
                            });
                        }
                    }
                    catch (Exception ex)
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
            public double TMAX { get; set; }
            public double TMIN { get; set; }
        }
        /// <summary>
        /// 将指定时间+1天的实况分别统计到当天24h、前一天48h、前两天72h数据库中
        /// </summary>
        /// <param name="dt">预报时间</param>
        /// <returns>返回错误信息</returns>
        public string QJZNTJ(DateTime dt)// 
        {
            List<TJList> tJLists = new List<TJList>();
            try
            {
                string con2 = "";
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\DBconfig.txt", Encoding.Default))
                {
                    string line1;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        if (line1.Contains("sql管理员"))
                        {
                            con2 = line1.Substring("sql管理员=".Length);
                            break;
                        }
                    }
                }

                string strError = "";
                using (SqlConnection mycon = new SqlConnection(con2))
                {
                    mycon.Open(); //打开
                    DateTime SKtime = dt.AddDays(1); //预报时间加一天的实况为24小时实况时间

                    string sql = "";
                    sql = string.Format(@"select * from QX ");
                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {
                                    tJLists.Add(new TJList()
                                    {
                                        ID = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                                        SKTMAX= 999999,
                                        SKTMIN= 999999,
                                        YBTMAX= -999999,
                                        YBTMIN= -999999,
                                    });

                                }
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        strError += "从数据库获取" + "旗县列表失败：" + ex.Message + '\n';
                        return strError;
                    }
                    sql = string.Format(@"select * from SK where  Date='{0}'", SKtime.ToString("yyyy-MM-dd"));
                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {

                                    TJList tJList = tJLists.Find((TJList y) => (y.ID == sqlreader.GetString(sqlreader.GetOrdinal("StationID"))));
                                    if (tJList != null)
                                    {
                                        int index = tJLists.IndexOf(tJList);
                                        if (index > -1)
                                        {
                                            tJLists[index].SKTMIN = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin")), 2);
                                            tJLists[index].SKTMAX = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax")), 2);
                                        }
                                    }

                                }
                            }

                        }


                    }
                    catch (Exception ex)
                    {
                        strError += "获取" + SKtime.ToString("yyyy-MM-dd") + "的实况失败：" + ex.Message + '\n';
                        return strError;
                    }

                }

                string IDStr = "";
                foreach (var l1 in tJLists)
                {
                    IDStr += '\'' + l1.ID + "\',";
                }

                IDStr = IDStr.Substring(0, IDStr.Length - 1);
                List<YSList> ySLists=HQYS(dt.ToString("yyyy-MM-dd"), 20,24, IDStr);
                for (int i = 0; i < tJLists.Count; i++)
                {
                    if (tJLists[i].SKTMAX == 999999 || tJLists[i].SKTMIN == 999999)
                    {
                        tJLists[i].YBTMAX = 999999;
                        tJLists[i].YBTMIN = 999999;
                    }
                    else
                    {
                        YSList ySList1 = ySLists.Find((YSList y) => y.ID == tJLists[i].ID);
                        if (ySList1 != null)
                        {
                            tJLists[i].YBTMAX = Math.Round(ySList1.TMAX - tJLists[i].SKTMAX,2);
                            tJLists[i].YBTMIN = Math.Round(ySList1.TMIN - tJLists[i].SKTMIN, 2);
                        }
                    }

                }

                string strTime = dt.ToString("yyyy-MM-dd");
                List<TJList24> tJList24s = new List<TJList24>();
                foreach (var ll in tJLists)
                {
                    tJList24s.Add(new TJList24()
                    {
                        StationID = ll.ID,
                        Date = strTime,
                        QJZN_SKTmax24 = ll.YBTMAX,
                        QJZN_SKTmin24 = ll.YBTMIN
                    });
                    ll.YBTMIN = -999999;
                    ll.YBTMAX = -999999;
                }
                DataTable dtb = ToDataTable(tJList24s);
                tJList24s.Clear();
                SqlBulkCopyByDatatable(con, "统计", dtb);
                dtb.Clear();
                ySLists.Clear();
                ySLists = HQYS(dt.AddDays(-1).ToString("yyyy-MM-dd"), 20, 48, IDStr);
                for (int i = 0; i < tJLists.Count; i++)
                {
                    if (tJLists[i].SKTMAX == 999999 || tJLists[i].SKTMIN == 999999)
                    {
                        tJLists[i].YBTMAX = 999999;
                        tJLists[i].YBTMIN = 999999;
                    }
                    else
                    {
                        YSList ySList1 = ySLists.Find((YSList y) => y.ID == tJLists[i].ID);
                        if (ySList1 != null)
                        {
                            tJLists[i].YBTMAX = Math.Round(ySList1.TMAX - tJLists[i].SKTMAX, 2);
                            tJLists[i].YBTMIN = Math.Round(ySList1.TMIN - tJLists[i].SKTMIN, 2);
                        }
                    }

                }

                strTime = dt.AddDays(-1).ToString("yyyy-MM-dd");
                List<TJList48> tJList48s = new List<TJList48>();
                foreach (var ll in tJLists)
                {
                    tJList48s.Add(new TJList48()
                    {
                        StationID = ll.ID,
                        Date = strTime,
                        QJZN_SKTmax48 = ll.YBTMAX,
                        QJZN_SKTmin48= ll.YBTMIN
                    });
                    ll.YBTMIN = -999999;
                    ll.YBTMAX = -999999;
                }
                dtb = ToDataTable(tJList48s);
                SqlBulkCopyByDatatable(con, "统计", dtb);
                dtb.Clear();
                ySLists.Clear();
                ySLists = HQYS(dt.AddDays(-2).ToString("yyyy-MM-dd"), 20, 72, IDStr);
                for (int i = 0; i < tJLists.Count; i++)
                {
                    if (tJLists[i].SKTMAX == 999999 || tJLists[i].SKTMIN == 999999)
                    {
                        tJLists[i].YBTMAX = 999999;
                        tJLists[i].YBTMIN = 999999;
                    }
                    else
                    {
                        YSList ySList1 = ySLists.Find((YSList y) => y.ID == tJLists[i].ID);
                        if (ySList1 != null)
                        {
                            tJLists[i].YBTMAX = Math.Round(ySList1.TMAX - tJLists[i].SKTMAX, 2);
                            tJLists[i].YBTMIN = Math.Round(ySList1.TMIN - tJLists[i].SKTMIN, 2);
                        }
                    }

                }

                strTime = dt.AddDays(-2).ToString("yyyy-MM-dd");
                List<TJList72> tJList72s = new List<TJList72>();
                foreach (var ll in tJLists)
                {
                    tJList72s.Add(new TJList72()
                    {
                        StationID = ll.ID,
                        Date = strTime,
                        QJZN_SKTmax72 = ll.YBTMAX,
                        QJZN_SKTmin72 = ll.YBTMIN
                    });
                    ll.YBTMIN = -999999;
                    ll.YBTMAX = -999999;
                }
                dtb = ToDataTable(tJList72s);
                SqlBulkCopyByDatatable(con, "统计", dtb);
                dtb.Clear();
                return strError;
            }
            catch(Exception ex)
            {

                return ex.Message;
            }
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
                        Data = sqlreader.GetString(sqlreader.GetOrdinal("StatioID"))+' '+ sqlreader.GetString(sqlreader.GetOrdinal("Name")) + ' '+ sqlreader.GetDouble(sqlreader.GetOrdinal("JD")) + ' '+ sqlreader.GetDouble(sqlreader.GetOrdinal("WD")) + ' '+ sqlreader.GetDouble(sqlreader.GetOrdinal("High")) ;
                    }
                }
                catch
                {
                }
            }
            return Data;
        }
        public void SaveStation(string stationID, string name,Int16 stationlevel,double Lon,double Lat,double High)
        {
            Stopwatch sw = new Stopwatch();
            using (SqlConnection mycon = new SqlConnection(con))
            {
               try
                {
                    string sql = "insert into Station(StatioID,Name,Station_levl,WD,JD,High) VALUES(@id,@name,@stationlev,@wd,@jd,@high)";
                    mycon.Open();//打开
                    int i = 0;
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
                catch(Exception ex)
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
       


        public class TJList
        {
            public string ID { get; set; }
            public double SKTMAX { get; set; }
            public double SKTMIN { get; set; }
            public double YBTMAX { get; set; }
            public double YBTMIN { get; set; }
            
        }
        public class TJList24
        {
            public string StationID { get; set; }
            public string Date { get; set; }
            public double QJZN_SKTmax24 { get; set; }
            public double QJZN_SKTmin24 { get; set; }


        }
        public class TJList48
        {
            public string StationID { get; set; }
            public string Date { get; set; }
            public double QJZN_SKTmax48 { get; set; }
            public double QJZN_SKTmin48 { get; set; }


        }
        public class TJList72
        {
            public string StationID { get; set; }
            public string Date { get; set; }
            public double QJZN_SKTmax72 { get; set; }
            public double QJZN_SKTmin72 { get; set; }


        }

     
        /// <summary>
        /// SqlBulkCopy
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">数据源</param>
        private void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
        {
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlBulkCopy sqlbulkcopy = new SqlBulkCopy(connectionString, SqlBulkCopyOptions.FireTriggers))
                {
                    try
                    {
                        sqlbulkcopy.DestinationTableName = TableName;
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sqlbulkcopy.ColumnMappings.Add(dt.Columns[i].ColumnName, dt.Columns[i].ColumnName);
                        }
                        sqlbulkcopy.WriteToServer(dt);

                    }
                    catch (System.Exception ex)
                    {
                        
                    }
                }
            }
        }

           /// <summary>
        /// SqlBulkCopy批量插入数据
        /// </summary>
        /// <param name="connectionStr">链接字符串</param>
        /// <param name="dataTableName">表名</param>
        /// <param name="sourceDataTable">数据源</param>
        /// <param name="batchSize">一次事务插入的行数</param>
        public static void SqlBulkCopyByDataTable(string connectionStr, string dataTableName, DataTable sourceDataTable, int batchSize = 100000)
        {
            using (SqlConnection connection = new SqlConnection(connectionStr))
            {
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(connectionStr, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    try
                    {
                        sqlBulkCopy.DestinationTableName = dataTableName;
                        sqlBulkCopy.BatchSize = batchSize;
                        for (int i = 0; i < sourceDataTable.Columns.Count; i++)
                        {
                            
                            sqlBulkCopy.ColumnMappings.Add(sourceDataTable.Columns[i].ColumnName, sourceDataTable.Columns[i].ColumnName);
                        }
                        sqlBulkCopy.WriteToServer(sourceDataTable);
                    }
                    catch (Exception ex)
                    {

                        throw ex;
                    }
                }
            }
        }


    }
}
