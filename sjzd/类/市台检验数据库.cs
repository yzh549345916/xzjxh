using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;

namespace sjzd.类
{
    class 市台检验数据库
    {
        public List<PeopelList> 根据岗位获取人员列表(string GW)
        {
            List<PeopelList> yBLists = new List<PeopelList>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            string con = util.Read("DBcfig", "hhhtjxh");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open(); //打开
                string sql = $"select * from USERID where GW = '{GW}'";
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    try
                    {
                        yBLists.Add(new PeopelList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            GW = sqlreader.GetString(sqlreader.GetOrdinal("GW")),
                            PassWord = sqlreader.GetString(sqlreader.GetOrdinal("Password")),
                            Admin = sqlreader.GetString(sqlreader.GetOrdinal("admin")),
                        });

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return yBLists;
        }

        public List<StationList> 获取赛罕智能网格站点列表(Int16 level)
        {
            List<StationList> yBLists = new List<StationList>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "xzjxhDB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open(); //打开
                string sql = $"select * from 赛罕精细化站点 where Station_levl = {level}";
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    try
                    {
                        yBLists.Add(new StationList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            GJID = sqlreader.GetString(sqlreader.GetOrdinal("GJStationID")),

                        });

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            return yBLists;
        }
        public class StationList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string GJID { get; set; }
        }
        public class PeopelList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string GW { get; set; }
            public string PassWord { get; set; }
            public string Admin { get; set; }

        }
    }
}
