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

        public string[,] 处理乡镇精细化预报数据(string[,] YBSZ, ref string error)
        {
            bool bsBool = false;//省级智能网格是否入库
            try
            {
                DateTime qbDate = DateTime.Now.Date.AddHours(20);
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
                   List<区台温度> dataList = 根据区站号起报时间获取区台新方法温度(xzID, qbDate);
                   XZGS += XZIDName.Split('\n').Length;
                    if (dataList.Count > 0)
                    {
                        bsBool = true;
                        foreach (string ss in XZIDName.Split('\n'))
                        {
                            区台温度 q24 = dataList.Find(y => (y.StationID == ss.Split(',')[0] && y.SX == 24));
                            区台温度 q48 = dataList.Find(y => (y.StationID == ss.Split(',')[0] && y.SX == 48));
                            区台温度 q72 = dataList.Find(y => (y.StationID == ss.Split(',')[0] && y.SX == 72));
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
                    error = $"{qbDate:MM月dd日HH}时省级智能网格数据获取失败，请查询数据是否入库。\r\n报文正常生成，但是乡镇报文温度为-99";
                return szYB;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return new string[,] { };
        }
    }
}
