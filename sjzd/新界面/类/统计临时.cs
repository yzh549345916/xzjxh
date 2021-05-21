using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace sjzd.新界面.类
{
    class 统计临时
    {
        public 统计临时()
        {
            string DBconPath = Environment.CurrentDirectory + @"\设置文件\城镇预报\DBconfig.txt";
            
            try
            {

                using (StreamReader sr = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
                {
                    string line;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains("sql管理员"))
                        {
                            _con = line.Substring("sql管理员=".Length);
                            con = _con;
                            break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        string con = "";
        private string QXID = "53368,53463,53464,53466,53467,53469,53562";
        private string _con = "";
        public void 市局预报处理()
        {
            string sql = string.Format(@"select * from LS_SJYB where  Date>='{0}' AND SC='20'", "2020-01-01");
            try
            {
                DataSet dt = new DataSet();  //创建dataSet
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    using (SqlDataAdapter data = new SqlDataAdapter(sql, mycon))
                    {
                        data.Fill(dt, "table1");
                    }
                }
                DataTable dt1 = dt.Tables["table1"];
                DataTable dt2 = new DataTable("LS_SJYB");
                dt2= dt1.Clone();
                foreach (DataRow myrow in dt1.Rows)
                {
                    DataRow dr = dt2.NewRow();
                    dr["StationID"] = myrow["StationID"];
                    dr["Date"] = Convert.ToDateTime(myrow["Date"]).AddDays(1).ToString("yyyy-MM-dd");
                    dr["SC"] = "08";
                    dr["GW"] = myrow["GW"];
                    dr["Tmax24"] = myrow["Tmax24"];
                    dr["Tmin24"] = myrow["Tmin48"];
                    dr["Rain24"] = 处理晴雨(myrow["Rain24"].ToString(), myrow["Rain48"].ToString());
                    dr["FX24"] = 处理晴雨(myrow["FX24"].ToString(), myrow["FX48"].ToString());
                    dr["FS24"] = 处理晴雨(myrow["FS24"].ToString(), myrow["FS48"].ToString());
                    dr["Tmax48"] = myrow["Tmax48"];
                    dr["Tmin48"] = myrow["Tmin72"];
                    dr["Rain48"] = 处理晴雨(myrow["Rain48"].ToString(), myrow["Rain72"].ToString());
                    dr["FX48"] = 处理晴雨(myrow["FX48"].ToString(), myrow["FX72"].ToString());
                    dr["FS48"] = 处理晴雨(myrow["FS48"].ToString(), myrow["FS72"].ToString());
                    dr["Tmax72"] = myrow["Tmax72"];
                    dr["Tmin72"] = myrow["Tmin96"];
                    dr["Rain72"] = 处理晴雨(myrow["Rain72"].ToString(), myrow["Rain96"].ToString()); ;
                    dr["FX72"] = 处理晴雨(myrow["FX72"].ToString(), myrow["FX96"].ToString()); ;
                    dr["FS72"] = 处理晴雨(myrow["FS72"].ToString(), myrow["FS96"].ToString()); ;
                    dr["YBtime"] = myrow["YBtime"];
                    dr["Tmax96"] = myrow["Tmax96"];
                    dr["Tmin96"] = myrow["Tmin120"];
                    dr["Rain96"] = 处理晴雨(myrow["Rain96"].ToString(), myrow["Rain120"].ToString()); ;
                    dr["FX96"] = 处理晴雨(myrow["FX96"].ToString(), myrow["FX120"].ToString()); ;
                    dr["FS96"] = 处理晴雨(myrow["FS96"].ToString(), myrow["FS120"].ToString()); ;
                    dr["Tmax120"] = myrow["Tmax120"];
                    /*dr["Tmin120"] = myrow["Tmin120"];
                    dr["Rain120"] = myrow["Rain120"];*/
                    dr["FX120"] = myrow["FX120"];
                    dr["FS120"] = myrow["FS120"];
                    dt2.Rows.Add(dr);
                    
                }
                SqlBulkCopyByDatatable(_con, "LS_SJYB", dt2);
            }
            catch(Exception e)
            {
            }
        }
        string YQtime20 = "1700", YQtime08 = "0700";
        public string TJRK(DateTime dt, string SC, string GW)//统计数据库初步建立，时间、岗位、时次、人员信息入库
        {
            string strError = "";

            try
            {
                string sql = string.Format(@"select * from USERJL where GW='{0}' AND date='{1:yyyy-MM-dd}'AND SC='{2}'", GW, dt, SC);
                SqlConnection mycon1 = new SqlConnection(_con);//创建SQL连接对象
                mycon1.Open();//打开
                string PeopleID = "";
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                PeopleID = sqlreader.GetString(sqlreader.GetOrdinal("userID"));
                            }
                        }

                    }
                    mycon1.Close();


                }
                catch (Exception ex)
                {
                    strError += ex.Message + '\n';
                }
                string[] qxidSZ = QXID.Split(',');
                for (int i = 0; i < qxidSZ.Length; i++)
                {

                    SqlConnection mycon2 = new SqlConnection(_con);//创建SQL连接对象
                    mycon2.Open();//打开
                    sql = string.Format(@"insert into LS_TJ (StationID,GW,PeopleID,Date,SC) values('{0}','{1}','{2}','{3}','{4}')", qxidSZ[i], GW, PeopleID, dt.ToString("yyyy-MM-dd"), SC);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon2))
                        {
                            sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数） 
                        }
                        mycon2.Close();



                    }
                    catch (Exception ex)
                    {
                        // strError += "新建" + dt.ToString("yyyy-MM-dd") + "日" + qxidSZ[i] + "的数据库统计信息字段失败，原因为：" + ex.Message + "\r\n";
                    }

                    strError += TJCZRKnew(qxidSZ[i], dt, SC, GW);
                }
            }
            catch (Exception ex)
            {
                strError += ex.Message + '\n';
            }

            return strError;
        }
        public string TJCZRKnew(string QXID, DateTime dt, string SC, string GW) //指定时间的24小时预报与实况差值入库 dt为预报时间 返回错误信息
        {
            string strError = "";
            float SKTmax = 999999, SKTmin = 999999;
            float SKRain = 999999, SKRain12 = 999999, SKRain24 = 999999;
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                mycon.Open();//打开
                DateTime SKtime = dt.AddDays(1);//预报时间加一天的实况为24小时实况时间
                string sql = string.Format(@"select * from SK where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, SKtime.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                try
                                {
                                    SKTmax = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax"));
                                    SKTmin = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin"));
                                    SKRain = sqlreader.GetFloat(sqlreader.GetOrdinal("Rain"));
                                    SKRain12 = sqlreader.GetFloat(sqlreader.GetOrdinal("Rain0012"));
                                    SKRain24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Rain1224"));
                                    if (SKRain == 999990)//CIMISS 999990为微量降水，计算时按照无降水计算
                                    {
                                        SKRain = 0;
                                    }
                                    if (SKRain12 == 999990)//CIMISS 999990为微量降水，计算时按照无降水计算
                                    {
                                        SKRain12 = 0;
                                    }
                                    if (SKRain24 == 999990)//CIMISS 999990为微量降水，计算时按照无降水计算
                                    {
                                        SKRain24 = 0;
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += "获取" + QXID + "的" + SKtime.ToString("yyyy-MM-dd") + "实况失败：" + ex.Message + '\n';
                }
                float SJTmax24 = -999999, SJTmin24 = -999999;
                float SJRain24 = -999999, SJRain2412 = -999999, SJRain2424 = -999999;
                int SJBWTime = 2359;
                int intYQtime = 0;//设置的逾期时间转换为数字，便于后续比较
                if (SC == "08")
                {
                    intYQtime = Convert.ToInt32(YQtime08);
                }
                else if (SC == "20")
                    intYQtime = Convert.ToInt32(YQtime20);
                bool SFZD24 = false;
                sql = string.Format(@"select * from LS_SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24"));
                                SJTmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24"));
                                SJBWTime = Convert.ToInt32(sqlreader.GetDateTime(sqlreader.GetOrdinal("YBtime")).ToString("HHmm"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain24"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain24 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain24 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                SJRain2412 = 1;
                                            }
                                            else
                                                SJRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                SJRain2424 = 1;
                                            }
                                            else
                                                SJRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            SJRain2412 = 1;//如果有降水为1
                                            SJRain2424 = 1;
                                        }
                                        else
                                        {
                                            SJRain2412 = 0;//无降水为0
                                            SJRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                if (SJBWTime <= intYQtime)
                {
                    SFZD24 = true;
                }
                float SJSKRain24 = 0, SJSKRain2412 = 0, SJSKRain2424 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    SJSKRain24 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain24 == -999999)
                    {
                        SJSKRain24 = SJRain24;
                    }
                    else
                    {
                        if (SKRainLS == SJRain24)
                        {
                            SJSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    SJSKRain2412 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2412 == -999999)
                    {
                        SJSKRain2412 = SJRain2412;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2412)
                        {
                            SJSKRain2412 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2412 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    SJSKRain2424 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2424 == -999999)
                    {
                        SJSKRain2424 = SJRain2424;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2424)
                        {
                            SJSKRain2424 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2424 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin24 = 0, SJSKTmax24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax24 = SKTmax;
                }
                else
                {
                    if (SJTmax24 == -999999)
                    {
                        SJSKTmax24 = SJTmax24;
                    }
                    else
                    {
                        SJSKTmax24 = Math.Round(SJTmax24 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin24 = SKTmin;
                }
                else
                {
                    if (SJTmin24 == -999999)
                    {
                        SJSKTmin24 = SJTmin24;
                    }
                    else
                    {
                        SJSKTmin24 = Math.Round(SJTmin24 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set SJ_SKTmax24='{SJSKTmax24}',SJ_SKTmin24='{SJSKTmin24}',SJ_Rain24='{SJSKRain24}',SFzhundian='{SFZD24}',SJ_Rain0012='{SJSKRain2412}',SJ_Rain1224='{SJSKRain2424}' where StationID='{QXID}' and Date='{dt:yyyy-MM-dd}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "24小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax24 = -999999, ZYTmin24 = -999999;
                float ZYRain24 = -999999, ZYRain2412 = -999999, ZYRain2424 = -999999; ;

                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24"));
                                ZYTmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain24"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain24 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain24 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                ZYRain2412 = 1;
                                            }
                                            else
                                                ZYRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                ZYRain2424 = 1;
                                            }
                                            else
                                                ZYRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            ZYRain2412 = 1;//如果有降水为1
                                            ZYRain2424 = 1;
                                        }
                                        else
                                        {
                                            ZYRain2412 = 0;//无降水为0
                                            ZYRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float ZYSKRain = 0, ZYSKRain12 = 0, ZYSKRain24 = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain24 == -999999)
                    {
                        ZYSKRain = ZYRain24;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain24)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    ZYSKRain12 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2412 == -999999)
                    {
                        ZYSKRain12 = ZYRain2412;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2412)
                        {
                            ZYSKRain12 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain12 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    ZYSKRain24 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2424 == -999999)
                    {
                        ZYSKRain24 = ZYRain2424;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2424)
                        {
                            ZYSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin24 = 0, ZYSKTmax24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax24 = SKTmax;
                }
                else
                {
                    if (ZYTmax24 == -999999)
                    {
                        ZYSKTmax24 = ZYTmax24;
                    }
                    else
                    {
                        ZYSKTmax24 = Math.Round(ZYTmax24 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin24 = SKTmin;
                }
                else
                {
                    if (ZYTmin24 == -999999)
                    {
                        ZYSKTmin24 = ZYTmin24;
                    }
                    else
                    {
                        ZYSKTmin24 = Math.Round(ZYTmin24 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set ZY_SKTmax24='{ZYSKTmax24}',ZY_SKTmin24='{ZYSKTmin24}',ZY_Rain24='{ZYSKRain}',ZY_Rain0012='{ZYSKRain12}',ZY_Rain1224='{ZYSKRain24}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "24小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #region//前一天48小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax48 = -999999, SJTmin48 = -999999;
                float SJRain48 = -999999; SJRain2412 = -999999; SJRain2424 = -999999;
                sql = string.Format(@"select * from LS_SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48"));
                                SJTmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain48"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain48 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain48 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                SJRain2412 = 1;
                                            }
                                            else
                                                SJRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                SJRain2424 = 1;
                                            }
                                            else
                                                SJRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            SJRain2412 = 1;//如果有降水为1
                                            SJRain2424 = 1;
                                        }
                                        else
                                        {
                                            SJRain2412 = 0;//无降水为0
                                            SJRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "48小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                SJSKRain2412 = 0; SJSKRain2424 = 0;
                if (SKRain > 999997)
                {
                    SJSKRain48 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain48 == -999999)
                    {
                        SJSKRain48 = SJRain48;
                    }
                    else
                    {
                        if (SKRainLS == SJRain48)
                        {
                            SJSKRain48 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain48 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    SJSKRain2412 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2412 == -999999)
                    {
                        SJSKRain2412 = SJRain2412;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2412)
                        {
                            SJSKRain2412 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2412 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    SJSKRain2424 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2424 == -999999)
                    {
                        SJSKRain2424 = SJRain2424;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2424)
                        {
                            SJSKRain2424 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2424 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin48 = 0, SJSKTmax48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax48 = SKTmax;
                }
                else
                {
                    if (SJTmax48 == -999999)
                    {
                        SJSKTmax48 = SJTmax48;
                    }
                    else
                    {
                        SJSKTmax48 = Math.Round(SJTmax48 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin48 = SKTmin;
                }
                else
                {
                    if (SJTmin48 == -999999)
                    {
                        SJSKTmin48 = SJTmin48;
                    }
                    else
                    {
                        SJSKTmin48 = Math.Round(SJTmin48 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set SJ_SKTmax48='{SJSKTmax48}',SJ_SKTmin48='{SJSKTmin48}',SJ_Rain48='{SJSKRain48}',SJ_Rain2436='{SJSKRain2412}',SJ_Rain3648='{SJSKRain2424}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "48小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax48 = -999999, ZYTmin48 = -999999;
                float ZYRain48 = -999999;
                ZYRain2412 = -999999; ZYRain2424 = -999999;
                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48"));
                                ZYTmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain48"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain48 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain48 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                ZYRain2412 = 1;
                                            }
                                            else
                                                ZYRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                ZYRain2424 = 1;
                                            }
                                            else
                                                ZYRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            ZYRain2412 = 1;//如果有降水为1
                                            ZYRain2424 = 1;
                                        }
                                        else
                                        {
                                            ZYRain2412 = 0;//无降水为0
                                            ZYRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "48小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                ZYSKRain12 = 0; ZYSKRain24 = 0;
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain48 == -999999)
                    {
                        ZYSKRain = ZYRain48;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain48)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    ZYSKRain12 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2412 == -999999)
                    {
                        ZYSKRain12 = ZYRain2412;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2412)
                        {
                            ZYSKRain12 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain12 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    ZYSKRain24 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2424 == -999999)
                    {
                        ZYSKRain24 = ZYRain2424;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2424)
                        {
                            ZYSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin48 = 0, ZYSKTmax48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax48 = SKTmax;
                }
                else
                {
                    if (ZYTmax48 == -999999)
                    {
                        ZYSKTmax48 = ZYTmax48;
                    }
                    else
                    {
                        ZYSKTmax48 = Math.Round(ZYTmax48 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin48 = SKTmin;
                }
                else
                {
                    if (ZYTmin48 == -999999)
                    {
                        ZYSKTmin48 = ZYTmin48;
                    }
                    else
                    {
                        ZYSKTmin48 = Math.Round(ZYTmin48 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set ZY_SKTmax48='{ZYSKTmax48}',ZY_SKTmin48='{ZYSKTmin48}',ZY_Rain48='{ZYSKRain}',ZY_Rain2436='{ZYSKRain12}',ZY_Rain3648='{ZYSKRain24}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "48小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
                #region//前两天72小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax72 = -999999, SJTmin72 = -999999;
                float SJRain72 = -999999;
                SJRain2412 = -999999; SJRain2424 = -999999;
                sql = string.Format(@"select * from LS_SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72"));
                                SJTmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain72"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain72 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain72 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                SJRain2412 = 1;
                                            }
                                            else
                                                SJRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                SJRain2424 = 1;
                                            }
                                            else
                                                SJRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            SJRain2412 = 1;//如果有降水为1
                                            SJRain2424 = 1;
                                        }
                                        else
                                        {
                                            SJRain2412 = 0;//无降水为0
                                            SJRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "72小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                SJSKRain2412 = 0; SJSKRain2424 = 0;
                if (SKRain > 999997)
                {
                    SJSKRain72 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain72 == -999999)
                    {
                        SJSKRain72 = SJRain72;
                    }
                    else
                    {
                        if (SKRainLS == SJRain72)
                        {
                            SJSKRain72 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain72 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    SJSKRain2412 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2412 == -999999)
                    {
                        SJSKRain2412 = SJRain2412;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2412)
                        {
                            SJSKRain2412 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2412 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    SJSKRain2424 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2424 == -999999)
                    {
                        SJSKRain2424 = SJRain2424;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2424)
                        {
                            SJSKRain2424 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2424 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin72 = 0, SJSKTmax72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax72 = SKTmax;
                }
                else
                {
                    if (SJTmax72 == -999999)
                    {
                        SJSKTmax72 = SJTmax72;
                    }
                    else
                    {
                        SJSKTmax72 = Math.Round(SJTmax72 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin72 = SKTmin;
                }
                else
                {
                    if (SJTmin72 == -999999)
                    {
                        SJSKTmin72 = SJTmin72;
                    }
                    else
                    {
                        SJSKTmin72 = Math.Round(SJTmin72 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set SJ_SKTmax72='{SJSKTmax72}',SJ_SKTmin72='{SJSKTmin72}',SJ_Rain72='{SJSKRain72}',SJ_Rain4860='{SJSKRain2412}',SJ_Rain6072='{SJSKRain2424}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "72小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax72 = -999999, ZYTmin72 = -999999;
                float ZYRain72 = -999999;
                ZYRain2412 = -999999; ZYRain2424 = -999999;
                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72"));
                                ZYTmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain72"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain72 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain72 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                ZYRain2412 = 1;
                                            }
                                            else
                                                ZYRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                ZYRain2424 = 1;
                                            }
                                            else
                                                ZYRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            ZYRain2412 = 1;//如果有降水为1
                                            ZYRain2424 = 1;
                                        }
                                        else
                                        {
                                            ZYRain2412 = 0;//无降水为0
                                            ZYRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "72小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                ZYSKRain12 = 0; ZYSKRain24 = 0;
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain72 == -999999)
                    {
                        ZYSKRain = ZYRain72;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain72)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    ZYSKRain12 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2412 == -999999)
                    {
                        ZYSKRain12 = ZYRain2412;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2412)
                        {
                            ZYSKRain12 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain12 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    ZYSKRain24 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2424 == -999999)
                    {
                        ZYSKRain24 = ZYRain2424;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2424)
                        {
                            ZYSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin72 = 0, ZYSKTmax72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax72 = SKTmax;
                }
                else
                {
                    if (ZYTmax72 == -999999)
                    {
                        ZYSKTmax72 = ZYTmax72;
                    }
                    else
                    {
                        ZYSKTmax72 = Math.Round(ZYTmax72 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin72 = SKTmin;
                }
                else
                {
                    if (ZYTmin72 == -999999)
                    {
                        ZYSKTmin72 = ZYTmin72;
                    }
                    else
                    {
                        ZYSKTmin72 = Math.Round(ZYTmin72 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set ZY_SKTmax72='{ZYSKTmax72}',ZY_SKTmin72='{ZYSKTmin72}',ZY_Rain72='{ZYSKRain}',ZY_Rain4860='{ZYSKRain12}',ZY_Rain6072='{ZYSKRain24}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "72小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
                #region//前三天96小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax96 = -999999, SJTmin96 = -999999;
                float SJRain96 = -999999;
                SJRain2412 = -999999; SJRain2424 = -999999;
                sql = string.Format(@"select * from LS_SJYB where StationID='{0}' AND Date='{1}' AND SC='{2}' AND GW='{3}'", QXID, dt.ToString("yyyy-MM-dd"), SC, GW);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SJTmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96"));
                                SJTmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain96"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        SJRain96 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        SJRain96 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                SJRain2412 = 1;
                                            }
                                            else
                                                SJRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                SJRain2424 = 1;
                                            }
                                            else
                                                SJRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            SJRain2412 = 1;//如果有降水为1
                                            SJRain2424 = 1;
                                        }
                                        else
                                        {
                                            SJRain2412 = 0;//无降水为0
                                            SJRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += QXID + "市局" + dt.ToString("yyyy-MM-dd") + "96小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                SJSKRain2412 = 0; SJSKRain2424 = 0;
                if (SKRain > 999997)
                {
                    SJSKRain96 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain96 == -999999)
                    {
                        SJSKRain96 = SJRain96;
                    }
                    else
                    {
                        if (SKRainLS == SJRain96)
                        {
                            SJSKRain96 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain96 = 0;//如果不一致为0
                        }
                    }
                }

                if (SKRain12 > 999997)
                {
                    SJSKRain2412 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2412 == -999999)
                    {
                        SJSKRain2412 = SJRain2412;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2412)
                        {
                            SJSKRain2412 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2412 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    SJSKRain2424 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (SJRain2424 == -999999)
                    {
                        SJSKRain2424 = SJRain2424;
                    }
                    else
                    {
                        if (SKRainLS == SJRain2424)
                        {
                            SJSKRain2424 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            SJSKRain2424 = 0;//如果不一致为0
                        }
                    }
                }
                double SJSKTmin96 = 0, SJSKTmax96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    SJSKTmax96 = SKTmax;
                }
                else
                {
                    if (SJTmax96 == -999999)
                    {
                        SJSKTmax96 = SJTmax96;
                    }
                    else
                    {
                        SJSKTmax96 = Math.Round(SJTmax96 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    SJSKTmin96 = SKTmin;
                }
                else
                {
                    if (SJTmin96 == -999999)
                    {
                        SJSKTmin96 = SJTmin96;
                    }
                    else
                    {
                        SJSKTmin96 = Math.Round(SJTmin96 - SKTmin, 1);
                    }
                }
                sql = $"update LS_TJ set SJ_SKTmax96='{SJSKTmax96}',SJ_SKTmin96='{SJSKTmin96}',SJ_Rain96='{SJSKRain96}',SJ_Rain7284='{SJSKRain2412}',SJ_Rain8496='{SJSKRain2424}' where StationID='{QXID}' and Date='{dt:yyyy-MM-dd}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "市局" + dt.ToString("yyyy-MM-dd") + "96小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                float ZYTmax96 = -999999, ZYTmin96 = -999999;
                float ZYRain96 = -999999;
                ZYRain2412 = -999999; ZYRain2424 = -999999;
                sql = string.Format(@"select * from ZYZD where StationID='{0}' AND Date='{1}'AND SC='{2}'", QXID, dt.ToString("yyyy-MM-dd"), SC);
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                ZYTmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax96"));
                                ZYTmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin96"));


                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain96"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        ZYRain96 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        ZYRain96 = 0;//无降水为0
                                    }
                                    if (strRain.Contains("转"))//
                                    {
                                        try
                                        {
                                            string[] strszls = Regex.Split(strRain, "转");
                                            if (strszls[0].Contains("雨") || strszls[0].Contains("雪"))
                                            {
                                                ZYRain2412 = 1;
                                            }
                                            else
                                                ZYRain2412 = 0;
                                            if (strszls[1].Contains("雨") || strszls[1].Contains("雪"))
                                            {
                                                ZYRain2424 = 1;
                                            }
                                            else
                                                ZYRain2424 = 0;

                                        }
                                        catch
                                        {
                                        }
                                    }
                                    else
                                    {
                                        if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                        {
                                            ZYRain2412 = 1;//如果有降水为1
                                            ZYRain2424 = 1;
                                        }
                                        else
                                        {
                                            ZYRain2412 = 0;//无降水为0
                                            ZYRain2424 = 0;
                                        }
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += QXID + "中央" + dt.ToString("yyyy-MM-dd") + "96小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                ZYSKRain = 0;//标志指导预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                ZYSKRain12 = 0; ZYSKRain24 = 0;
                if (SKRain > 999997)
                {
                    ZYSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYSKRain == -999999)
                    {
                        ZYSKRain = ZYRain96;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain96)
                        {
                            ZYSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain12 > 999997)
                {
                    ZYSKRain12 = SKRain12;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain12 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2412 == -999999)
                    {
                        ZYSKRain12 = ZYRain2412;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2412)
                        {
                            ZYSKRain12 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain12 = 0;//如果不一致为0
                        }
                    }
                }
                if (SKRain24 > 999997)
                {
                    ZYSKRain24 = SKRain24;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain24 > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (ZYRain2424 == -999999)
                    {
                        ZYSKRain24 = ZYRain2424;
                    }
                    else
                    {
                        if (SKRainLS == ZYRain2424)
                        {
                            ZYSKRain24 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            ZYSKRain24 = 0;//如果不一致为0
                        }
                    }
                }
                double ZYSKTmin96 = 0, ZYSKTmax96 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    ZYSKTmax96 = SKTmax;
                }
                else
                {
                    if (ZYTmax96 == -999999)
                    {
                        ZYSKTmax96 = ZYTmax96;
                    }
                    else
                    {
                        ZYSKTmax96 = Math.Round(ZYTmax96 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    ZYSKTmin96 = SKTmin;
                }
                else
                {
                    if (ZYTmin96 == -999999)
                    {
                        ZYSKTmin96 = ZYTmin96;
                    }
                    else
                    {
                        ZYSKTmin96 = Math.Round(ZYTmin96 - SKTmin, 1);
                    }
                }
                sql = $@"update LS_TJ set ZY_SKTmax96='{ZYSKTmax96}',ZY_SKTmin96='{ZYSKTmin96}',ZY_Rain96='{ZYSKRain}',ZY_Rain7284='{ZYSKRain12}',ZY_Rain8496='{ZYSKRain24}' where StationID='{QXID}' and Date='{dt.ToString("yyyy-MM-dd")}'and SC='{SC}'and GW='{GW}'";
                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "中央" + dt.ToString("yyyy-MM-dd") + "96小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
               
            }



            return strError;
        }
        public string 处理晴雨(string s1,string s2)
        {
            s1 = s1.Trim();
            s2=s2.Trim();
            string myTQ = "";
            if (s1.Contains("转"))
            {
                s1 = Regex.Split(s1, "转")[1];
            }
            if (s2.Contains("转"))
            {
                s2 = Regex.Split(s2, "转")[0];
            }
            s1 = s1.Trim();
            s2 = s2.Trim();
            if (!s1.Equals(s2))
            {
                myTQ = s1 +"转"+ s2;
            }
            else
            {
                myTQ = s1;
            }

            return myTQ;
        }
        #region SqlBulkCopy批量快速入库

        /// <summary>
        /// SqlBulkCopy批量快速入库
        /// </summary>
        /// <param name="connectionString">目标连接字符</param>
        /// <param name="TableName">目标表</param>
        /// <param name="dt">数据源</param>
        private void SqlBulkCopyByDatatable(string connectionString, string TableName, DataTable dt)
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
                    dt = null;
                }
                catch (Exception e)
                {
                }
            }
        }

        #endregion
        
        #region 岗位统计
        public float[] GWQXZQL120(DateTime sdt, DateTime edt, String QXID, String GW, String sc) //返回指定指定时间段个人五天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<PFZR> pfzrtj1 = new ObservableCollection<PFZR>();
            float[] tjsz = new float[16];
            try
            {
                using (SqlConnection mycon1 = new SqlConnection(con)) //创建SQL连接对象)
                {
                    mycon1.Open(); //打开
                    string sql =
                        $@"select * from LS_TJ where StationID='{QXID}' AND Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";
                    SqlCommand sqlman = new SqlCommand(sql, mycon1);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {
                            try
                            {
                                float tmax24 = 999999,
                                    tmin24 = 999999,
                                    qy24 = 999999,
                                    tmax48 = 999999,
                                    tmin48 = 999999,
                                    qy48 = 999999,
                                    tmax72 = 999999,
                                    tmin72 = 999999,
                                    qy72 = 999999,
                                    rain240012 = 999999,
                                     rain241224 = 999999,
                                    rain480012 = 999999,
                                     rain481224 = 999999,
                                    rain720012 = 999999,
                                     rain721224 = 999999,
                                    rain960012 = 999999,
                                     rain961224 = 999999,
                                    rain1200012 = 999999,
                                     rain1201224 = 999999,
                                tmax96 = 999999,
                                    tmin96 = 999999,
                                    qy96 = 999999,
                                tmax120 = 999999,
                                    tmin120 = 999999,
                                    qy120 = 999999;
                                try
                                {
                                    tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24"));
                                    tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24"));
                                    qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24"));
                                    tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48"));
                                    tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48"));
                                    qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48"));
                                    tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72"));
                                    tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72"));
                                    qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72"));
                                    tmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax96"));
                                    tmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin96"));
                                    qy96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain96"));
                                    tmax120 = 0;
                                    tmin120 = 0;
                                    qy120 = 0;
                                    rain240012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain0012"));
                                    rain241224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain1224"));
                                    rain480012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain2436"));
                                    rain481224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain3648"));
                                    rain720012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain4860"));
                                    rain721224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain6072"));
                                    rain960012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain7284"));
                                    rain961224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain8496"));
                                    rain1200012 = 0;
                                    rain1201224 = 0;
                                }
                                catch (Exception)
                                {

                                }
                                pfzrtj1.Add(new PFZR()
                                {
                                    QX24TmaxZQL = tmax24,
                                    QX24TminZQL = tmin24,
                                    QX24QYZQL = Math.Abs(rain240012) + Math.Abs(rain241224),
                                    QX48TmaxZQL = tmax48,
                                    QX48TminZQL = tmin48,
                                    QX48QYZQL = Math.Abs(rain480012) + Math.Abs(rain481224),
                                    QX72TmaxZQL = tmax72,
                                    QX72TminZQL = tmin72,
                                    QX72QYZQL = Math.Abs(rain720012) + Math.Abs(rain721224),
                                    QX96TmaxZQL = tmax96,
                                    QX96TminZQL = tmin96,
                                    QX96QYZQL = Math.Abs(rain960012) + Math.Abs(rain961224),
                                    QX120TmaxZQL = 0,
                                    QX120TminZQL = 0,
                                    QX120QYZQL =0,
                                });
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    mycon1.Close();
                }
                int ss = pfzrtj1.Count;
                PFZR[] s1 = pfzrtj1.ToArray();
                int[] zs = new int[15]; //保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
                for (int i = 0; i < s1.Length; i++)
                {
                    if (s1[i].QX24TmaxZQL < 999998 && s1[i].QX24TmaxZQL > -999999)
                    {
                        zs[0]++;
                        if (Math.Abs(s1[i].QX24TmaxZQL) <= 2)
                        {
                            tjsz[0]++;
                        }
                    }
                    else if (s1[i].QX24TmaxZQL == -999999)
                    {
                        tjsz[15]++;
                    }
                    if (s1[i].QX24TminZQL < 999998 && s1[i].QX24TminZQL > -999999)
                    {
                        zs[1]++;
                        if (Math.Abs(s1[i].QX24TminZQL) <= 2)
                        {
                            tjsz[1]++;
                        }
                    }
                    if (s1[i].QX24QYZQL < 999998 && s1[i].QX24QYZQL > -999999)
                    {
                        zs[2] = zs[2] + 2;

                        tjsz[2] += s1[i].QX24QYZQL;
                    }

                    if (s1[i].QX48TmaxZQL < 999998 && s1[i].QX48TmaxZQL > -999999)
                    {
                        zs[3]++;
                        if (Math.Abs(s1[i].QX48TmaxZQL) <= 2)
                        {
                            tjsz[3]++;
                        }
                    }
                    if (s1[i].QX48TminZQL < 999998 && s1[i].QX48TminZQL > -999999)
                    {
                        zs[4]++;
                        if (Math.Abs(s1[i].QX48TminZQL) <= 2)
                        {
                            tjsz[4]++;
                        }
                    }
                    if (s1[i].QX48QYZQL < 999998 && s1[i].QX48QYZQL > -999999)
                    {
                        zs[5] = zs[5] + 2;
                        tjsz[5] += s1[i].QX48QYZQL;
                    }
                    if (s1[i].QX72TmaxZQL < 999998 && s1[i].QX72TmaxZQL > -999999)
                    {
                        zs[6]++;
                        if (Math.Abs(s1[i].QX72TmaxZQL) <= 2)
                        {
                            tjsz[6]++;
                        }
                    }
                    if (s1[i].QX72TminZQL < 999998 && s1[i].QX72TminZQL > -999999)
                    {
                        zs[7]++;
                        if (Math.Abs(s1[i].QX72TminZQL) <= 2)
                        {
                            tjsz[7]++;
                        }
                    }
                    if (s1[i].QX72QYZQL < 999998 && s1[i].QX72QYZQL > -999999)
                    {
                        zs[8] = zs[8] + 2;
                        tjsz[8] += s1[i].QX72QYZQL;
                    }

                    if (s1[i].QX96TmaxZQL < 999998 && s1[i].QX96TmaxZQL > -999999)
                    {
                        zs[9]++;
                        if (Math.Abs(s1[i].QX96TmaxZQL) <= 2)
                        {
                            tjsz[9]++;
                        }
                    }
                    if (s1[i].QX96TminZQL < 999998 && s1[i].QX96TminZQL > -999999)
                    {
                        zs[10]++;
                        if (Math.Abs(s1[i].QX96TminZQL) <= 2)
                        {
                            tjsz[10]++;
                        }
                    }
                    if (s1[i].QX96QYZQL < 999998 && s1[i].QX96QYZQL > -999999)
                    {
                        zs[11] = zs[11] + 2;
                        tjsz[11] += s1[i].QX96QYZQL;
                    }
                    if (s1[i].QX120TmaxZQL < 999998 && s1[i].QX120TmaxZQL > -999999)
                    {
                        zs[12]++;
                        /*if (Math.Abs(s1[i].QX120TmaxZQL) <= 2)
                        {
                            tjsz[12]++;
                        }*/
                    }
                    if (s1[i].QX120TminZQL < 999998 && s1[i].QX120TminZQL > -999999)
                    {
                        zs[13]++;
                        /*if (Math.Abs(s1[i].QX120TminZQL) <= 2)
                        {
                            tjsz[13]++;
                        }*/
                    }
                    if (s1[i].QX120QYZQL < 999998 && s1[i].QX120QYZQL > -999999)
                    {
                        zs[14] = zs[14] + 2;
                        tjsz[14] += 0;
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
                        tjsz[i] = tjsz[i] / s1.Length;
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message);
            }
            return tjsz;
        }
        public float[] GWQXJDWC120(DateTime sdt, DateTime edt, String QXID, String GW, String sc)//返回指定区站号、指定时间段个人五天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[10];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where StationID='{QXID}' AND Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {
                                    if (sqlreader.HasRows)
                                    {
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                            QX96TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax96")),
                                            QX96TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin96")),
                                            QX120TmaxJDWC = 0,
                                            QX120TminJDWC =0,
                                        });
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
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[10];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }
                if (s1[i].QX96TmaxJDWC < 999998 && s1[i].QX96TmaxJDWC > -999999)
                {
                    zs[6]++;
                    tjsz[6] += Math.Abs(s1[i].QX96TmaxJDWC);
                }
                if (s1[i].QX96TminJDWC < 999998 && s1[i].QX96TminJDWC > -999999)
                {
                    zs[7]++;
                    tjsz[7] += Math.Abs(s1[i].QX96TminJDWC);
                }
                if (s1[i].QX120TmaxJDWC < 999998 && s1[i].QX120TmaxJDWC > -999999)
                {
                    zs[8]++;
                    /*tjsz[8] += Math.Abs(s1[i].QX120TmaxJDWC);*/
                }
                if (s1[i].QX120TminJDWC < 999998 && s1[i].QX120TminJDWC > -999999)
                {
                    zs[9]++;
                    /*tjsz[9] += Math.Abs(s1[i].QX120TminJDWC);*/
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }
        public class PJWCList//旗县平均绝对误差列表
        {
            public float QX24TmaxJDWC { get; set; }
            public float QX24TminJDWC { get; set; }
            public float QX48TmaxJDWC { get; set; }
            public float QX48TminJDWC { get; set; }
            public float QX72TmaxJDWC { get; set; }
            public float QX72TminJDWC { get; set; }
            public float QX96TmaxJDWC { get; set; }
            public float QX96TminJDWC { get; set; }
            public float QX120TmaxJDWC { get; set; }
            public float QX120TminJDWC { get; set; }
        }
        public float[] GWQXZDJDWC120(DateTime sdt, DateTime edt, String QXID, String GW, String sc)//返回指定人员、指定时间段中央指导五天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[10];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where StationID='{QXID}' AND Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    pfzrtj1.Add(new PJWCList()
                                    {
                                        QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24")),
                                        QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24")),
                                        QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48")),
                                        QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48")),
                                        QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72")),
                                        QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72")),
                                        QX96TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax96")),
                                        QX96TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin96")),
                                        QX120TmaxJDWC = 0,
                                        QX120TminJDWC = 0,
                                    });
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
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[10];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }
                if (s1[i].QX96TmaxJDWC < 999998 && s1[i].QX96TmaxJDWC > -999999)
                {
                    zs[6]++;
                    tjsz[6] += Math.Abs(s1[i].QX96TmaxJDWC);
                }
                if (s1[i].QX96TminJDWC < 999998 && s1[i].QX96TminJDWC > -999999)
                {
                    zs[7]++;
                    tjsz[7] += Math.Abs(s1[i].QX96TminJDWC);
                }
                if (s1[i].QX120TmaxJDWC < 999998 && s1[i].QX120TmaxJDWC > -999999)
                {
                    zs[8]++;
                    /*tjsz[8] += Math.Abs(s1[i].QX120TmaxJDWC);*/
                }
                if (s1[i].QX120TminJDWC < 999998 && s1[i].QX120TminJDWC > -999999)
                {
                    zs[9]++;
                    /*tjsz[9] += Math.Abs(s1[i].QX120TminJDWC);*/
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }
        public float[] GWQXZYZQL120(DateTime sdt, DateTime edt, String QXID, String GW, String sc)//返回指定区站号、指定时间段中央指导五天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[16];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where StationID='{QXID}' AND Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {

                                    float tmax24 = 999999,
                                        tmin24 = 999999,
                                        qy24 = 999999,
                                        tmax48 = 999999,
                                        tmin48 = 999999,
                                        qy48 = 999999,
                                        tmax72 = 999999,
                                        tmin72 = 999999,
                                        qy72 = 999999,
                                         rain240012 = 999999,
                                     rain241224 = 999999,
                                    rain480012 = 999999,
                                     rain481224 = 999999,
                                    rain720012 = 999999,
                                     rain721224 = 999999,
                                    rain960012 = 999999,
                                     rain961224 = 999999,
                                    rain1200012 = 999999,
                                     rain1201224 = 999999,
                                         tmax96 = 999999,
                                    tmin96 = 999999,
                                    qy96 = 999999,
                                tmax120 = 999999,
                                    tmin120 = 999999,
                                    qy120 = 999999;
                                    try
                                    {
                                        tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24"));
                                        tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24"));
                                        qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain24"));
                                        tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48"));
                                        tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48"));
                                        qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain48"));
                                        tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72"));
                                        tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72"));
                                        qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain72"));
                                        tmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax96"));
                                        tmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin96"));
                                        qy96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain96"));
                                        tmax120 = 0;
                                        tmin120 = 0;
                                        qy120 = 0;
                                        rain240012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain0012"));
                                        rain241224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain1224"));
                                        rain480012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain2436"));
                                        rain481224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain3648"));
                                        rain720012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain4860"));
                                        rain721224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain6072"));
                                        rain960012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain7284"));
                                        rain961224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain8496"));
                                        rain1200012 = 0;
                                        rain1201224 = 0;
                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                    sjzqlTJ1.Add(new ZQLTJ1()
                                    {
                                        SJ24TmaxZQL = tmax24,
                                        SJ24TminZQL = tmin24,
                                        SJ24QYZQL = Math.Abs(rain240012) + Math.Abs(rain241224),
                                        SJ48TmaxZQL = tmax48,
                                        SJ48TminZQL = tmin48,
                                        SJ48QYZQL = Math.Abs(rain480012) + Math.Abs(rain481224),
                                        SJ72TmaxZQL = tmax72,
                                        SJ72TminZQL = tmin72,
                                        SJ72QYZQL = Math.Abs(rain720012) + Math.Abs(rain721224),
                                        SJ96TmaxZQL = tmax96,
                                        SJ96TminZQL = tmin96,
                                        SJ96QYZQL = Math.Abs(rain960012) + Math.Abs(rain961224),
                                        SJ120TmaxZQL = 0,
                                        SJ120TminZQL = 0,
                                        SJ120QYZQL = 0,
                                    });
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
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[15];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].SJ24TmaxZQL < 999998 && s1[i].SJ24TmaxZQL > -999999)
                {
                    zs[0]++;
                    if (Math.Abs(s1[i].SJ24TmaxZQL) <= 2)
                    {
                        tjsz[0]++;
                    }
                }
                else if (s1[i].SJ24TmaxZQL == -999999)
                {
                    tjsz[15]++;
                }
                if (s1[i].SJ24TminZQL < 999998 && s1[i].SJ24TminZQL > -999999)
                {
                    zs[1]++;
                    if (Math.Abs(s1[i].SJ24TminZQL) <= 2)
                    {
                        tjsz[1]++;
                    }
                }
                if (s1[i].SJ24QYZQL < 999998 && s1[i].SJ24QYZQL > -999999)
                {
                    zs[2] += 2;
                    tjsz[2] += s1[i].SJ24QYZQL;
                }

                if (s1[i].SJ48TmaxZQL < 999998 && s1[i].SJ48TmaxZQL > -999999)
                {
                    zs[3]++;
                    if (Math.Abs(s1[i].SJ48TmaxZQL) <= 2)
                    {
                        tjsz[3]++;
                    }
                }
                if (s1[i].SJ48TminZQL < 999998 && s1[i].SJ48TminZQL > -999999)
                {
                    zs[4]++;
                    if (Math.Abs(s1[i].SJ48TminZQL) <= 2)
                    {
                        tjsz[4]++;
                    }
                }
                if (s1[i].SJ48QYZQL < 999998 && s1[i].SJ48QYZQL > -999999)
                {
                    zs[5] += 2;
                    tjsz[5] += s1[i].SJ48QYZQL;
                }
                if (s1[i].SJ72TmaxZQL < 999998 && s1[i].SJ72TmaxZQL > -999999)
                {
                    zs[6]++;
                    if (Math.Abs(s1[i].SJ72TmaxZQL) <= 2)
                    {
                        tjsz[6]++;
                    }
                }
                if (s1[i].SJ72TminZQL < 999998 && s1[i].SJ72TminZQL > -999999)
                {
                    zs[7]++;
                    if (Math.Abs(s1[i].SJ72TminZQL) <= 2)
                    {
                        tjsz[7]++;
                    }
                }
                if (s1[i].SJ72QYZQL < 999998 && s1[i].SJ72QYZQL > -999999)
                {
                    zs[8] += 2;
                    tjsz[8] += s1[i].SJ72QYZQL;
                }
                if (s1[i].SJ96TmaxZQL < 999998 && s1[i].SJ96TmaxZQL > -999999)
                {
                    zs[9]++;
                    if (Math.Abs(s1[i].SJ96TmaxZQL) <= 2)
                    {
                        tjsz[9]++;
                    }
                }
                if (s1[i].SJ96TminZQL < 999998 && s1[i].SJ96TminZQL > -999999)
                {
                    zs[10]++;
                    if (Math.Abs(s1[i].SJ96TminZQL) <= 2)
                    {
                        tjsz[10]++;
                    }
                }
                if (s1[i].SJ96QYZQL < 999998 && s1[i].SJ96QYZQL > -999999)
                {
                    zs[11] += 2;
                    tjsz[11] += s1[i].SJ96QYZQL;
                }
                if (s1[i].SJ120TmaxZQL < 999998 && s1[i].SJ120TmaxZQL > -999999)
                {
                    zs[12]++;
                    /*if (Math.Abs(s1[i].SJ120TmaxZQL) <= 2)
                    {
                        tjsz[12]++;
                    }*/
                }
                if (s1[i].SJ120TminZQL < 999998 && s1[i].SJ120TminZQL > -999999)
                {
                    zs[13]++;
                    /*if (Math.Abs(s1[i].SJ120TminZQL) <= 2)
                    {
                        tjsz[13]++;
                    }*/
                }
                if (s1[i].SJ120QYZQL < 999998 && s1[i].SJ120QYZQL > -999999)
                {
                    zs[14] += 2;
                    //tjsz[14] += s1[i].SJ120QYZQL;
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
                    tjsz[i] = tjsz[i] / s1.Length;
                    tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                }
            }
            return tjsz;
        }

        public float[] GWZQL120(DateTime sdt, DateTime edt, String GW, String sc) //返回指定指定时间段个人五天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<PFZR> pfzrtj1 = new ObservableCollection<PFZR>();
            float[] tjsz = new float[16];
            try
            {
                using (SqlConnection mycon1 = new SqlConnection(con)) //创建SQL连接对象)
                {
                    mycon1.Open(); //打开
                    string sql =
                        $@"select * from LS_TJ where Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";
                    SqlCommand sqlman = new SqlCommand(sql, mycon1);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {
                            try
                            {
                                float tmax24 = 999999,
                                    tmin24 = 999999,
                                    qy24 = 999999,
                                    tmax48 = 999999,
                                    tmin48 = 999999,
                                    qy48 = 999999,
                                    tmax72 = 999999,
                                    tmin72 = 999999,
                                    qy72 = 999999,
                                    rain240012 = 999999,
                                     rain241224 = 999999,
                                    rain480012 = 999999,
                                     rain481224 = 999999,
                                    rain720012 = 999999,
                                     rain721224 = 999999,
                                    rain960012 = 999999,
                                     rain961224 = 999999,
                                    rain1200012 = 999999,
                                     rain1201224 = 999999,
                                tmax96 = 999999,
                                    tmin96 = 999999,
                                    qy96 = 999999,
                                tmax120 = 999999,
                                    tmin120 = 999999,
                                    qy120 = 999999;
                                try
                                {
                                    tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24"));
                                    tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24"));
                                    qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24"));
                                    tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48"));
                                    tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48"));
                                    qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48"));
                                    tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72"));
                                    tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72"));
                                    qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72"));
                                    tmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax96"));
                                    tmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin96"));
                                    qy96 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain96"));
                                    tmax120 = 0;
                                    tmin120 = 0;
                                    qy120 = 0;
                                    rain240012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain0012"));
                                    rain241224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain1224"));
                                    rain480012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain2436"));
                                    rain481224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain3648"));
                                    rain720012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain4860"));
                                    rain721224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain6072"));
                                    rain960012 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain7284"));
                                    rain961224 = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain8496"));
                                    rain1200012 = 0;
                                    rain1201224 = 0;
                                }
                                catch (Exception)
                                {

                                }
                                pfzrtj1.Add(new PFZR()
                                {
                                    QX24TmaxZQL = tmax24,
                                    QX24TminZQL = tmin24,
                                    QX24QYZQL = Math.Abs(rain240012) + Math.Abs(rain241224),
                                    QX48TmaxZQL = tmax48,
                                    QX48TminZQL = tmin48,
                                    QX48QYZQL = Math.Abs(rain480012) + Math.Abs(rain481224),
                                    QX72TmaxZQL = tmax72,
                                    QX72TminZQL = tmin72,
                                    QX72QYZQL = Math.Abs(rain720012) + Math.Abs(rain721224),
                                    QX96TmaxZQL = tmax96,
                                    QX96TminZQL = tmin96,
                                    QX96QYZQL = Math.Abs(rain960012) + Math.Abs(rain961224),
                                    QX120TmaxZQL = 0,
                                    QX120TminZQL = 0,
                                    QX120QYZQL = 0,
                                });
                            }
                            catch (Exception)
                            { }
                        }
                    }
                    mycon1.Close();
                }
                int ss = pfzrtj1.Count;
                PFZR[] s1 = pfzrtj1.ToArray();
                int[] zs = new int[15]; //保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
                for (int i = 0; i < s1.Length; i++)
                {
                    if (s1[i].QX24TmaxZQL < 999998 && s1[i].QX24TmaxZQL > -999999)
                    {
                        zs[0]++;
                        if (Math.Abs(s1[i].QX24TmaxZQL) <= 2)
                        {
                            tjsz[0]++;
                        }
                    }
                    else if (s1[i].QX24TmaxZQL == -999999)
                    {
                        tjsz[15]++;
                    }
                    if (s1[i].QX24TminZQL < 999998 && s1[i].QX24TminZQL > -999999)
                    {
                        zs[1]++;
                        if (Math.Abs(s1[i].QX24TminZQL) <= 2)
                        {
                            tjsz[1]++;
                        }
                    }
                    if (s1[i].QX24QYZQL < 999998 && s1[i].QX24QYZQL > -999999)
                    {
                        zs[2] = zs[2] + 2;

                        tjsz[2] += s1[i].QX24QYZQL;
                    }

                    if (s1[i].QX48TmaxZQL < 999998 && s1[i].QX48TmaxZQL > -999999)
                    {
                        zs[3]++;
                        if (Math.Abs(s1[i].QX48TmaxZQL) <= 2)
                        {
                            tjsz[3]++;
                        }
                    }
                    if (s1[i].QX48TminZQL < 999998 && s1[i].QX48TminZQL > -999999)
                    {
                        zs[4]++;
                        if (Math.Abs(s1[i].QX48TminZQL) <= 2)
                        {
                            tjsz[4]++;
                        }
                    }
                    if (s1[i].QX48QYZQL < 999998 && s1[i].QX48QYZQL > -999999)
                    {
                        zs[5] = zs[5] + 2;
                        tjsz[5] += s1[i].QX48QYZQL;
                    }
                    if (s1[i].QX72TmaxZQL < 999998 && s1[i].QX72TmaxZQL > -999999)
                    {
                        zs[6]++;
                        if (Math.Abs(s1[i].QX72TmaxZQL) <= 2)
                        {
                            tjsz[6]++;
                        }
                    }
                    if (s1[i].QX72TminZQL < 999998 && s1[i].QX72TminZQL > -999999)
                    {
                        zs[7]++;
                        if (Math.Abs(s1[i].QX72TminZQL) <= 2)
                        {
                            tjsz[7]++;
                        }
                    }
                    if (s1[i].QX72QYZQL < 999998 && s1[i].QX72QYZQL > -999999)
                    {
                        zs[8] = zs[8] + 2;
                        tjsz[8] += s1[i].QX72QYZQL;
                    }

                    if (s1[i].QX96TmaxZQL < 999998 && s1[i].QX96TmaxZQL > -999999)
                    {
                        zs[9]++;
                        if (Math.Abs(s1[i].QX96TmaxZQL) <= 2)
                        {
                            tjsz[9]++;
                        }
                    }
                    if (s1[i].QX96TminZQL < 999998 && s1[i].QX96TminZQL > -999999)
                    {
                        zs[10]++;
                        if (Math.Abs(s1[i].QX96TminZQL) <= 2)
                        {
                            tjsz[10]++;
                        }
                    }
                    if (s1[i].QX96QYZQL < 999998 && s1[i].QX96QYZQL > -999999)
                    {
                        zs[11] = zs[11] + 2;
                        tjsz[11] += s1[i].QX96QYZQL;
                    }
                    if (s1[i].QX120TmaxZQL < 999998 && s1[i].QX120TmaxZQL > -999999)
                    {
                        zs[12]++;
                        if (Math.Abs(s1[i].QX120TmaxZQL) <= 2)
                        {
                          //  tjsz[12]++;
                        }
                    }
                    if (s1[i].QX120TminZQL < 999998 && s1[i].QX120TminZQL > -999999)
                    {
                        zs[13]++;
                        if (Math.Abs(s1[i].QX120TminZQL) <= 2)
                        {
                           // tjsz[13]++;
                        }
                    }
                    if (s1[i].QX120QYZQL < 999998 && s1[i].QX120QYZQL > -999999)
                    {
                        zs[14] = zs[14] + 2;
                       // tjsz[14] += s1[i].QX120QYZQL;
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
                        tjsz[i] = tjsz[i] / s1.Length;
                        tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message);
            }
            return tjsz;
        }
        public float[] GWJDWC120(DateTime sdt, DateTime edt, String GW, String sc)//返回指定区站号、指定时间段个人五天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[10];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {
                                    if (sqlreader.HasRows)
                                    {
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                            QX96TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax96")),
                                            QX96TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin96")),
                                            QX120TmaxJDWC = 0,
                                            QX120TminJDWC = 0,
                                        });
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
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[10];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }
                if (s1[i].QX96TmaxJDWC < 999998 && s1[i].QX96TmaxJDWC > -999999)
                {
                    zs[6]++;
                    tjsz[6] += Math.Abs(s1[i].QX96TmaxJDWC);
                }
                if (s1[i].QX96TminJDWC < 999998 && s1[i].QX96TminJDWC > -999999)
                {
                    zs[7]++;
                    tjsz[7] += Math.Abs(s1[i].QX96TminJDWC);
                }
                if (s1[i].QX120TmaxJDWC < 999998 && s1[i].QX120TmaxJDWC > -999999)
                {
                    zs[8]++;
                   // tjsz[8] += Math.Abs(s1[i].QX120TmaxJDWC);
                }
                if (s1[i].QX120TminJDWC < 999998 && s1[i].QX120TminJDWC > -999999)
                {
                    zs[9]++;
                    //tjsz[9] += Math.Abs(s1[i].QX120TminJDWC);
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }
        public float[] GWZDJDWC120(DateTime sdt, DateTime edt, String GW, String sc)//返回指定人员、指定时间段中央指导五天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[10];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    pfzrtj1.Add(new PJWCList()
                                    {
                                        QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24")),
                                        QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24")),
                                        QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48")),
                                        QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48")),
                                        QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72")),
                                        QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72")),
                                        QX96TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax96")),
                                        QX96TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin96")),
                                        QX120TmaxJDWC = 0,
                                        QX120TminJDWC = 0,
                                    });
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
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[10];//保存计算平均绝对误差时候每个要素的个数总数
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].QX24TmaxJDWC < 999998 && s1[i].QX24TmaxJDWC > -999999)
                {
                    zs[0]++;
                    tjsz[0] += Math.Abs(s1[i].QX24TmaxJDWC);

                }
                if (s1[i].QX24TminJDWC < 999998 && s1[i].QX24TminJDWC > -999999)
                {
                    zs[1]++;
                    tjsz[1] += Math.Abs(s1[i].QX24TminJDWC);
                }

                if (s1[i].QX48TmaxJDWC < 999998 && s1[i].QX48TmaxJDWC > -999999)
                {
                    zs[2]++;
                    tjsz[2] += Math.Abs(s1[i].QX48TmaxJDWC);
                }
                if (s1[i].QX48TminJDWC < 999998 && s1[i].QX48TminJDWC > -999999)
                {
                    zs[3]++;
                    tjsz[3] += Math.Abs(s1[i].QX48TminJDWC);
                }
                if (s1[i].QX72TmaxJDWC < 999998 && s1[i].QX72TmaxJDWC > -999999)
                {
                    zs[4]++;
                    tjsz[4] += Math.Abs(s1[i].QX72TmaxJDWC);
                }
                if (s1[i].QX72TminJDWC < 999998 && s1[i].QX72TminJDWC > -999999)
                {
                    zs[5]++;
                    tjsz[5] += Math.Abs(s1[i].QX72TminJDWC);
                }
                if (s1[i].QX96TmaxJDWC < 999998 && s1[i].QX96TmaxJDWC > -999999)
                {
                    zs[6]++;
                    tjsz[6] += Math.Abs(s1[i].QX96TmaxJDWC);
                }
                if (s1[i].QX96TminJDWC < 999998 && s1[i].QX96TminJDWC > -999999)
                {
                    zs[7]++;
                    tjsz[7] += Math.Abs(s1[i].QX96TminJDWC);
                }
                if (s1[i].QX120TmaxJDWC < 999998 && s1[i].QX120TmaxJDWC > -999999)
                {
                    zs[8]++;
                   // tjsz[8] += Math.Abs(s1[i].QX120TmaxJDWC);
                }
                if (s1[i].QX120TminJDWC < 999998 && s1[i].QX120TminJDWC > -999999)
                {
                    zs[9]++;
                   // tjsz[9] += Math.Abs(s1[i].QX120TminJDWC);
                }

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }
        public float[] GWZYZQL120(DateTime sdt, DateTime edt, String GW, String sc)//返回指定区站号、指定时间段中央指导五天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[16];
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql =
                        $@"select * from LS_TJ where Date>='{sdt:yyyy-MM-dd}' AND Date<='{edt:yyyy-MM-dd}' AND GW='{GW}' AND SC='{sc}'";

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {

                                    float tmax24 = 999999,
                                        tmin24 = 999999,
                                        qy24 = 999999,
                                        tmax48 = 999999,
                                        tmin48 = 999999,
                                        qy48 = 999999,
                                        tmax72 = 999999,
                                        tmin72 = 999999,
                                        qy72 = 999999,
                                         rain240012 = 999999,
                                     rain241224 = 999999,
                                    rain480012 = 999999,
                                     rain481224 = 999999,
                                    rain720012 = 999999,
                                     rain721224 = 999999,
                                    rain960012 = 999999,
                                     rain961224 = 999999,
                                    rain1200012 = 999999,
                                     rain1201224 = 999999,
                                         tmax96 = 999999,
                                    tmin96 = 999999,
                                    qy96 = 999999,
                                tmax120 = 999999,
                                    tmin120 = 999999,
                                    qy120 = 999999;
                                    try
                                    {
                                        tmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax24"));
                                        tmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin24"));
                                        qy24 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain24"));
                                        tmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax48"));
                                        tmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin48"));
                                        qy48 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain48"));
                                        tmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax72"));
                                        tmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin72"));
                                        qy72 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain72"));
                                        tmax96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmax96"));
                                        tmin96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_SKTmin96"));
                                        qy96 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain96"));
                                        tmax120 = 0;
                                        tmin120 = 0;
                                        qy120 = 0;
                                        rain240012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain0012"));
                                        rain241224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain1224"));
                                        rain480012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain2436"));
                                        rain481224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain3648"));
                                        rain720012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain4860"));
                                        rain721224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain6072"));
                                        rain960012 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain7284"));
                                        rain961224 = sqlreader.GetFloat(sqlreader.GetOrdinal("ZY_Rain8496"));
                                        rain1200012 = 0;
                                        rain1201224 = 0;
                                    }
                                    catch (Exception)
                                    {
                                        break;
                                    }
                                    sjzqlTJ1.Add(new ZQLTJ1()
                                    {
                                        SJ24TmaxZQL = tmax24,
                                        SJ24TminZQL = tmin24,
                                        SJ24QYZQL = Math.Abs(rain240012) + Math.Abs(rain241224),
                                        SJ48TmaxZQL = tmax48,
                                        SJ48TminZQL = tmin48,
                                        SJ48QYZQL = Math.Abs(rain480012) + Math.Abs(rain481224),
                                        SJ72TmaxZQL = tmax72,
                                        SJ72TminZQL = tmin72,
                                        SJ72QYZQL = Math.Abs(rain720012) + Math.Abs(rain721224),
                                        SJ96TmaxZQL = tmax96,
                                        SJ96TminZQL = tmin96,
                                        SJ96QYZQL = Math.Abs(rain960012) + Math.Abs(rain961224),
                                        SJ120TmaxZQL = 0,
                                        SJ120TminZQL = 0,
                                        SJ120QYZQL = 0,
                                    });
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
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[15];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
            for (int i = 0; i < s1.Length; i++)
            {
                if (s1[i].SJ24TmaxZQL < 999998 && s1[i].SJ24TmaxZQL > -999999)
                {
                    zs[0]++;
                    if (Math.Abs(s1[i].SJ24TmaxZQL) <= 2)
                    {
                        tjsz[0]++;
                    }
                }
                else if (s1[i].SJ24TmaxZQL == -999999)
                {
                    tjsz[15]++;
                }
                if (s1[i].SJ24TminZQL < 999998 && s1[i].SJ24TminZQL > -999999)
                {
                    zs[1]++;
                    if (Math.Abs(s1[i].SJ24TminZQL) <= 2)
                    {
                        tjsz[1]++;
                    }
                }
                if (s1[i].SJ24QYZQL < 999998 && s1[i].SJ24QYZQL > -999999)
                {
                    zs[2] += 2;
                    tjsz[2] += s1[i].SJ24QYZQL;
                }

                if (s1[i].SJ48TmaxZQL < 999998 && s1[i].SJ48TmaxZQL > -999999)
                {
                    zs[3]++;
                    if (Math.Abs(s1[i].SJ48TmaxZQL) <= 2)
                    {
                        tjsz[3]++;
                    }
                }
                if (s1[i].SJ48TminZQL < 999998 && s1[i].SJ48TminZQL > -999999)
                {
                    zs[4]++;
                    if (Math.Abs(s1[i].SJ48TminZQL) <= 2)
                    {
                        tjsz[4]++;
                    }
                }
                if (s1[i].SJ48QYZQL < 999998 && s1[i].SJ48QYZQL > -999999)
                {
                    zs[5] += 2;
                    tjsz[5] += s1[i].SJ48QYZQL;
                }
                if (s1[i].SJ72TmaxZQL < 999998 && s1[i].SJ72TmaxZQL > -999999)
                {
                    zs[6]++;
                    if (Math.Abs(s1[i].SJ72TmaxZQL) <= 2)
                    {
                        tjsz[6]++;
                    }
                }
                if (s1[i].SJ72TminZQL < 999998 && s1[i].SJ72TminZQL > -999999)
                {
                    zs[7]++;
                    if (Math.Abs(s1[i].SJ72TminZQL) <= 2)
                    {
                        tjsz[7]++;
                    }
                }
                if (s1[i].SJ72QYZQL < 999998 && s1[i].SJ72QYZQL > -999999)
                {
                    zs[8] += 2;
                    tjsz[8] += s1[i].SJ72QYZQL;
                }
                if (s1[i].SJ96TmaxZQL < 999998 && s1[i].SJ96TmaxZQL > -999999)
                {
                    zs[9]++;
                    if (Math.Abs(s1[i].SJ96TmaxZQL) <= 2)
                    {
                        tjsz[9]++;
                    }
                }
                if (s1[i].SJ96TminZQL < 999998 && s1[i].SJ96TminZQL > -999999)
                {
                    zs[10]++;
                    if (Math.Abs(s1[i].SJ96TminZQL) <= 2)
                    {
                        tjsz[10]++;
                    }
                }
                if (s1[i].SJ96QYZQL < 999998 && s1[i].SJ96QYZQL > -999999)
                {
                    zs[11] += 2;
                    tjsz[11] += s1[i].SJ96QYZQL;
                }
                if (s1[i].SJ120TmaxZQL < 999998 && s1[i].SJ120TmaxZQL > -999999)
                {
                    zs[12]++;
                    if (Math.Abs(s1[i].SJ120TmaxZQL) <= 2)
                    {
                       // tjsz[12]++;
                    }
                }
                if (s1[i].SJ120TminZQL < 999998 && s1[i].SJ120TminZQL > -999999)
                {
                    zs[13]++;
                    if (Math.Abs(s1[i].SJ120TminZQL) <= 2)
                    {
                      //  tjsz[13]++;
                    }
                }
                if (s1[i].SJ120QYZQL < 999998 && s1[i].SJ120QYZQL > -999999)
                {
                    zs[14] += 2;
                   // tjsz[14] += s1[i].SJ120QYZQL;
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
                    tjsz[i] = tjsz[i] / s1.Length;
                    tjsz[i] = (float)Math.Round(tjsz[i] * 100, 2);
                }
            }
            return tjsz;
        }
        #endregion
        public class ZQLTJ1//统计信息列表
        {
            public string Name { get; set; }
            public float SJ24TmaxZQL { get; set; }
            public float SJ24TminZQL { get; set; }
            public float SJ24QYZQL { get; set; }
            public float SJ48TmaxZQL { get; set; }
            public float SJ48TminZQL { get; set; }
            public float SJ48QYZQL { get; set; }
            public float SJ72TmaxZQL { get; set; }
            public float SJ72TminZQL { get; set; }
            public float SJ72QYZQL { get; set; }
            public float SJ96TmaxZQL { get; set; }
            public float SJ96TminZQL { get; set; }
            public float SJ96QYZQL { get; set; }
            public float SJ120TmaxZQL { get; set; }
            public float SJ120TminZQL { get; set; }
            public float SJ120QYZQL { get; set; }
        }
        public class PFZR
        {
            public string Name { get; set; }
            public float QX24TmaxZQL { get; set; }
            public float QX24TminZQL { get; set; }
            public float QX24QYZQL { get; set; }
            public float QX48TmaxZQL { get; set; }
            public float QX48TminZQL { get; set; }
            public float QX48QYZQL { get; set; }
            public float QX72TmaxZQL { get; set; }
            public float QX72TminZQL { get; set; }
            public float QX72QYZQL { get; set; }
            public float QX96TmaxZQL { get; set; }
            public float QX96TminZQL { get; set; }
            public float QX96QYZQL { get; set; }
            public float QX120TmaxZQL { get; set; }
            public float QX120TminZQL { get; set; }
            public float QX120QYZQL { get; set; }
        }
    }
}
