using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace xzjxhyb_DBmain
{
    public class TJAddDB
    {
        string con = "";
        string YQH = "17", YQM = "0";//旗县报文逾期时间
        string YQtime = "1700";////设置的逾期时间，待读取完配置文件后为逾期的小时加上逾期的分钟
        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
        public string FirstTJ(DateTime dt)
        {
            string strError = "";
            string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";

            try
            {
                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                {
                    string line1;

                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        if (line1.Contains("sql管理员"))
                        {
                            con = line1.Substring("sql管理员=".Length);
                        }
                        else if (line1.Split('=')[0] == "旗县逾期小时")
                        {
                            YQH = line1.Split('=')[1];
                        }
                        else if (line1.Split('=')[0] == "旗县逾期分钟")
                        {
                            YQM = line1.Split('=')[1];
                        }
                    }
                }
                while (YQH.Length < 2)
                {
                    YQH = '0' + YQH;//如果时间设置不足两位用0补位
                }
                while (YQM.Length < 2)
                {
                    YQM = '0' + YQM;
                }
                YQtime = YQH + YQM;

                Int16 intQXGS = 0;
                using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//统计旗县个数
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        if (line1.Split(':')[0] == "旗县个数")
                        {
                            intQXGS = Convert.ToInt16(line1.Split(':')[1]);
                            break;
                        }
                    }
                }
                using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//第二行开始每两行为旗县及乡镇区站号列表
                {
                    string line1 = "";
                    Int16 lineCount = 0;

                    while (lineCount < intQXGS * 2 + 1)
                    {
                        line1 = sr.ReadLine();
                        if ((lineCount > 1) && (lineCount % 2 == 0))
                        {
                            SqlConnection mycon1 = new SqlConnection(con);//创建SQL连接对象
                            mycon1.Open();//打开

                            string PeopleID = "";

                            string[] IDList = line1.Split(',');
                            string FQXID = IDList[0];//第一个为所属旗县的ID
                            string sql = string.Format(@"select * from USERJL where QXID='{0}' AND date='{1}'", FQXID, dt.ToString("yyyy-MM-dd"));
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
                            for (int i = 0; i < IDList.Length; i++)
                            {
                                SqlConnection mycon2 = new SqlConnection(con);//创建SQL连接对象
                                mycon2.Open();//打开
                                sql = string.Format(@"insert into TJ (StationID,fromQXID,PeopleID,Date) values('{0}','{1}','{2}','{3}')", IDList[i], FQXID, PeopleID, dt.ToString("yyyy-MM-dd"));  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
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
                                    strError += "新建" + dt.ToString("yyyy-MM-dd") + "日" + IDList[i] + "的数据库统计信息字段失败，原因为：" + ex.Message + '\n';
                                }

                                strError += SJQXTJ(IDList[i], dt);
                            }


                        }
                        lineCount++;

                    }
                }
            }
            catch (Exception ex)
            {
                strError += ex.Message + '\n';
            }
            return strError;


        }

        public string SJQXTJ(string XZID, DateTime dt)//指定时间的24小时预报与实况差值入库 dt为预报时间 返回错误信息
        {
            string strError = "";
            float SKTmax = 999999, SKTmin = 999999;
            float SKRain = 999999;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                DateTime SKtime = dt.AddDays(1);//预报时间加一天的实况为24小时实况时间

                string sql = string.Format(@"select * from SK where StationID='{0}' AND Date='{1}'", XZID, SKtime.ToString("yyyy-MM-dd"));
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                SKTmax = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax"));
                                SKTmin = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin"));
                                SKRain = sqlreader.GetFloat(sqlreader.GetOrdinal("Rain"));
                                if (SKRain == 999990)//CIMISS 999990为微量降水，计算时按照无降水计算
                                {
                                    SKRain = 0;
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += "获取" + XZID + "的" + SKtime.ToString("yyyy-MM-dd") + "实况失败：" + ex.Message + '\n';
                }

                float SJTmax24 = -999999, SJTmin24 = -999999;
                float SJRain24 = -999999;

                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
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
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += XZID + "市局" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
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
                sql = string.Format(@"update TJ set SJ_SKTmax24='{0}',SJ_SKTmin24='{1}',SJ_Rain24='{2}' where StationID='{3}' and Date='{4}'", SJSKTmax24, SJSKTmin24, SJSKRain24, XZID, dt.ToString("yyyy-MM-dd"));

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

                //获取数据库旗县预报信息
                float QXTmax24 = -999999, QXTmin24 = -999999;
                float QXRain24 = -999999;
                int QXBWTime = 2359;//旗县第一份报文的时间的小时与分钟
                int intYQtime = Convert.ToInt32(YQtime);//设置的逾期时间转换为数字，便于后续比较
                bool SFZD24 = false;
                sql = string.Format(@"select * from QXYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                QXTmax24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax24"));
                                QXTmin24 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin24"));

                                QXBWTime = Convert.ToInt32(sqlreader.GetDateTime(sqlreader.GetOrdinal("YBtime")).ToString("HHmm"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain24"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        QXRain24 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        QXRain24 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += XZID + "旗县" + dt.ToString("yyyy-MM-dd") + "24小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                if (QXBWTime <= intYQtime)
                {
                    SFZD24 = true;
                }
                float QXSKRain = 0;//标志旗县预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    QXSKRain = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (QXRain24 == -999999)
                    {
                        QXSKRain = QXRain24;
                    }
                    else
                    {
                        if (SKRainLS == QXRain24)
                        {
                            QXSKRain = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            QXSKRain = 0;//如果不一致为0
                        }
                    }
                }
                double QXSKTmin24 = 0, QXSKTmax24 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    QXSKTmax24 = SKTmax;
                }
                else
                {
                    if (QXTmax24 == -999999)
                    {
                        QXSKTmax24 = QXTmax24;
                    }
                    else
                    {
                        QXSKTmax24 = Math.Round(QXTmax24 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    QXSKTmin24 = SKTmin;
                }
                else
                {
                    if (QXTmin24 == -999999)
                    {
                        QXSKTmin24 = QXTmin24;
                    }
                    else
                    {
                        QXSKTmin24 = Math.Round(QXTmin24 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set QX_SKTmax24='{0}',QX_SKTmin24='{1}',QX_Rain24='{2}',SFzhundian='{3}' where StationID='{4}' and Date='{5}'", QXSKTmax24, QXSKTmin24, QXSKRain, SFZD24, XZID, dt.ToString("yyyy-MM-dd"));

                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "旗县" + dt.ToString("yyyy-MM-dd") + "24小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }


                #region//前一天48小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax48 = -999999, SJTmin48 = -999999;
                float SJRain48 = -999999;

                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
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
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += XZID + "市局" + dt.ToString("yyyy-MM-dd") + "48小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
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
                sql = string.Format(@"update TJ set SJ_SKTmax48='{0}',SJ_SKTmin48='{1}',SJ_Rain48='{2}' where StationID='{3}' and Date='{4}'", SJSKTmax48, SJSKTmin48, SJSKRain48, XZID, dt.ToString("yyyy-MM-dd"));

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

                //获取数据库旗县前一天48小时预报信息

                float QXTmax48 = -999999, QXTmin48 = -999999;
                float QXRain48 = -999999;
                sql = string.Format(@"select * from QXYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                QXTmax48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax48"));
                                QXTmin48 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin48"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain48"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        QXRain48 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        QXRain48 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += XZID + "旗县" + dt.ToString("yyyy-MM-dd") + "预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float QXSKRain48 = 0;//标志旗县预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    QXSKRain48 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (QXRain48 == -999999)
                    {
                        QXSKRain48 = QXRain48;
                    }
                    else
                    {
                        if (SKRainLS == QXRain48)
                        {
                            QXSKRain48 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            QXSKRain48 = 0;//如果不一致为0
                        }
                    }
                }
                double QXSKTmin48 = 0, QXSKTmax48 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    QXSKTmax48 = SKTmax;
                }
                else
                {
                    if (QXTmax48 == -999999)
                    {
                        QXSKTmax48 = QXTmax48;
                    }
                    else
                    {
                        QXSKTmax48 = Math.Round(QXTmax48 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    QXSKTmin48 = SKTmin;
                }
                else
                {
                    if (QXTmin48 == -999999)
                    {
                        QXSKTmin48 = QXTmin48;
                    }
                    else
                    {
                        QXSKTmin48 = Math.Round(QXTmin48 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set QX_SKTmax48='{0}', QX_SKTmin48='{1}',QX_Rain48='{2}' where StationID='{3}' and Date='{4}'", QXSKTmax48, QXSKTmin48, QXSKRain48, XZID, dt.ToString("yyyy-MM-dd"));

                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "旗县" + dt.ToString("yyyy-MM-dd") + "48小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion

                #region//前两天72小时预报情况统计
                dt = dt.AddDays(-1);
                float SJTmax72 = -999999, SJTmin72 = -999999;
                float SJRain72 = -999999;

                sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
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
                                }
                            }
                        }

                    }


                }
                catch (Exception ex)
                {
                    strError += XZID + "市局" + dt.ToString("yyyy-MM-dd") + "72小时预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float SJSKRain72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
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
                sql = string.Format(@"update TJ set SJ_SKTmax72='{0}',SJ_SKTmin72='{1}',SJ_Rain72='{2}' where StationID='{3}' and Date='{4}'", SJSKTmax72, SJSKTmin72, SJSKRain72, XZID, dt.ToString("yyyy-MM-dd"));

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

                //获取数据库旗县前两天72小时预报信息

                float QXTmax72 = -999999, QXTmin72 = -999999;
                float QXRain72 = -999999;
                sql = string.Format(@"select * from QXYB where StationID='{0}' AND Date='{1}'", XZID, dt.ToString("yyyy-MM-dd"));
                try
                {
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                QXTmax72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmax72"));
                                QXTmin72 = sqlreader.GetFloat(sqlreader.GetOrdinal("Tmin72"));
                                string strRain = sqlreader.GetString(sqlreader.GetOrdinal("Rain72"));
                                if (strRain.Length > 0)//如果长度等于0说明缺报
                                {
                                    if (strRain.Contains("雨") || strRain.Contains("雪"))//
                                    {
                                        QXRain72 = 1;//如果有降水为1
                                    }
                                    else
                                    {
                                        QXRain72 = 0;//无降水为0
                                    }
                                }
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    strError += XZID + "旗县" + dt.ToString("yyyy-MM-dd") + "预报信息数据库读取失败,请确认是数据库连接问题还是缺报：" + ex.Message + '\n';
                }
                float QXSKRain72 = 0;//标志旗县预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKRain > 999997)
                {
                    QXSKRain72 = SKRain;
                }
                else
                {
                    float SKRainLS = 0;
                    if (SKRain > 0)
                        SKRainLS = 1;//实况是否出现降水标致，如果有降水为1，如果没有降水为0
                    if (QXRain72 == -999999)
                    {
                        QXSKRain72 = QXRain72;
                    }
                    else
                    {
                        if (SKRainLS == QXRain72)
                        {
                            QXSKRain72 = 1;//如果预报与实况降水一致，则为1
                        }
                        else
                        {
                            QXSKRain72 = 0;//如果不一致为0
                        }
                    }
                }
                double QXSKTmin72 = 0, QXSKTmax72 = 0;//标致市局预报与实况降水是否一致，一致为1，不一致为0，999999为缺测，-999999为缺报，999998为不观测
                if (SKTmax > 999997)
                {
                    QXSKTmax72 = SKTmax;
                }
                else
                {
                    if (QXTmax72 == -999999)
                    {
                        QXSKTmax72 = QXTmax72;
                    }
                    else
                    {
                        QXSKTmax72 = Math.Round(QXTmax72 - SKTmax, 1);
                    }
                }
                if (SKTmin > 999997)
                {
                    QXSKTmin72 = SKTmin;
                }
                else
                {
                    if (QXTmin72 == -999999)
                    {
                        QXSKTmin72 = QXTmin72;
                    }
                    else
                    {
                        QXSKTmin72 = Math.Round(QXTmin72 - SKTmin, 1);
                    }
                }
                sql = string.Format(@"update TJ set QX_SKTmax72='{0}', QX_SKTmin72='{1}',QX_Rain72='{2}' where StationID='{3}' and Date='{4}'", QXSKTmax72, QXSKTmin72, QXSKRain72, XZID, dt.ToString("yyyy-MM-dd"));

                try
                {
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    //执行数据库语句并返回一个int值（受影响的行数）  
                    sqlman.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    strError += "旗县" + dt.ToString("yyyy-MM-dd") + "72小时预报实况统计信息入库失败：" + ex.Message + '\n';
                }
                #endregion
            }
            return strError;

        }
    }
}