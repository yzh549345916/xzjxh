using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace sjzd.类
{
    public class mysql环境气象
    {
        String connetStr = "";
        public mysql环境气象()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            connetStr = util.Read("DBcfig", "huanjingqixiang");
        }
        public DateTime 获取EC地面最新时次()
        {
            DateTime dateTime = DateTime.MinValue;
            using (MySqlConnection conn = new MySqlConnection(connetStr))
            {
                try
                {
                    conn.Open();
                    string sql = "SELECT * FROM `ECSurface` ORDER BY `Datetime` DESC LIMIT 0,1";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        dateTime = reader.GetDateTime("Datetime");
                    }
                }
                catch
                {

                }
            }

            return dateTime;
        }

        public List<CIMISS.ECTEF0> 根据区站号起报时间获取EC温度(String ids, DateTime qbDatetime)
        {
            List<CIMISS.ECTEF0> myData = new List<CIMISS.ECTEF0>();
            try
            {
                //处理区站号
                String myIds = "";
                if (ids.Contains('\''))
                {
                    myIds = ids;
                }
                else
                {
                    foreach (string id in ids.Split(','))
                    {
                        myIds = myIds + "'" + id + "',";
                    }
                    myIds = myIds.Substring(0, myIds.Length - 1);
                }
                using (MySqlConnection conn = new MySqlConnection(connetStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = $"select * FROM  `ECSurface` WHERE Datetime='{qbDatetime:yyyy-MM-dd HH:mm:ss}' AND stationID IN ({myIds})";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int sx = reader.GetInt32("validTime");
                            try
                            {
                                myData.Add(new CIMISS.ECTEF0
                                {
                                    StationID = reader.GetString("stationID"),
                                    TEM = reader.GetDouble("TEM"),
                                    DateTime = reader.GetDateTime("Datetime").AddHours(sx),
                                });
                            }
                            catch
                            {
                            }
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

            return myData;
        }

    }
}
