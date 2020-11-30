using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace sjzd.类
{
    public class mysql数据库类
    {
        String connetStr = "";
        public mysql数据库类()
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            connetStr = util.Read("DBcfig", "szybnmqxt");
        }
        public void 获取()
        {
            using (MySqlConnection conn = new MySqlConnection(connetStr))
            {
                try
                {
                    conn.Open();
                    string sql = "select * from Szyb_GD_ZD_20201119";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string id = reader.GetString("ID");
                        double TEM = reader.GetDouble("TEM");
                    }
                }
                catch
                {

                }
            }
        }
        public DateTime 获取区台新方法最新起报时间(int SC)
        {
            DateTime dateTime = DateTime.Now.Date.AddHours(SC);
            using (MySqlConnection conn = new MySqlConnection(connetStr))
            {
                try
                {
                    conn.Open();
                    string sql = $@"select * from Szyb_GD_ZD_{dateTime:yyyyMMdd} where MyDate='{dateTime:yyyy-MM-dd HH:mm:ss}'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        reader.Close();
                        dateTime = dateTime.AddDays(-1);
                        sql = $@"select * from Szyb_GD_ZD_{dateTime:yyyyMMdd} where MyDate='{dateTime:yyyy-MM-dd HH:mm:ss}'";
                        cmd = new MySqlCommand(sql, conn);
                        reader = cmd.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            dateTime = DateTime.MinValue;
                        }
                    }

                }
                catch (Exception ee)
                {

                }
            }
            return dateTime;
        }

        public List<区台温度> 根据区站号起报时间获取区台新方法温度(String ids, DateTime qbDatetime)
        {
            List<区台温度> myData = new List<区台温度>();
            try
            {
                //处理区站号
                String myIds = "";
                foreach (string id in ids.Split(','))
                {
                    myIds = myIds + "'" + id + "',";
                }
                myIds = myIds.Substring(0, myIds.Length - 1);
                using (MySqlConnection conn = new MySqlConnection(connetStr))
                {
                    try
                    {
                        conn.Open();
                        string sql = $"select * from Szyb_GD_ZD_{qbDatetime:yyyyMMdd} WHERE MyDate='{qbDatetime:yyyy-MM-dd HH:mm:ss}' AND ID IN ({myIds})";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            int sx = reader.GetInt32("SX");
                            myData.Add(new 区台温度
                            {
                                StationID = reader.GetString("ID"),
                                TEM = reader.GetDouble("TEM"),
                                TMAX = reader.GetDouble("TMAX"),
                                TMIN = reader.GetDouble("TMIN"),
                                SX = sx,
                                MyDate = reader.GetDateTime("MyDate").AddHours(sx),
                            });

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
        public class 区台温度
        {
            public string StationID { get; set; }
            public DateTime MyDate { get; set; }
            public double TEM { get; set; }
            public double TMAX { get; set; }
            public double TMIN { get; set; }
            public int SX { get; set; }
        }
    }
}
