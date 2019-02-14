using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 旗县端
{
    class TJ
    {
        public float[] SJZQL(string StartDate, string EndDate, String QXID)//返回指定区站号、指定时间段市局三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[10];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

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
                                            SJ24TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            SJ24TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            SJ24QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24")),
                                            SJ48TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            SJ48TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            SJ48QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48")),
                                            SJ72TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            SJ72TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                            SJ72QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72")),
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = sjzqlTJ1.Count;
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
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
                    tjsz[9]++;
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
                    zs[2]++;
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
                    zs[5]++;
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
                    zs[8]++;
                    tjsz[8] += s1[i].SJ72QYZQL;
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

        public float[] SJQSZQL(string StartDate, string EndDate)//返回指定时间段市局所有旗县三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[10];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {

            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where Date>='{0}' AND Date<='{1}'", StartDate, EndDate);

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
                                            SJ24TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            SJ24TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            SJ24QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24")),
                                            SJ48TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            SJ48TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            SJ48QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48")),
                                            SJ72TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            SJ72TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                            SJ72QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72")),
                                        });
                                    }
                                    catch

                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = sjzqlTJ1.Count;
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
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
                    tjsz[9]++;
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
                    zs[2]++;
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
                    zs[5]++;
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
                    zs[8]++;
                    tjsz[8] += s1[i].SJ72QYZQL;
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

        public float[] QXZQL(string StartDate, string EndDate, String QXID)//返回指定区站号、指定时间段旗县三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<PFZR> pfzrtj1 = new ObservableCollection<PFZR>();
            float[] tjsz = new float[10];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PFZR()
                                        {
                                            QX24TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax24")),
                                            QX24TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin24")),
                                            QX24QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain24")),
                                            QX48TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax48")),
                                            QX48TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin48")),
                                            QX48QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain48")),
                                            QX72TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax72")),
                                            QX72TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin72")),
                                            QX72QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain72")),
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PFZR[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
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
                    tjsz[9]++;
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
                    zs[2]++;
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
                    zs[5]++;
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
                    zs[8]++;
                    tjsz[8] += s1[i].QX72QYZQL;
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

        public float[] QXJDWC(string StartDate, string EndDate, String QXID)//返回指定区站号、指定时间段旗县三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin72")),
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
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

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public float[] SJJDWC(string StartDate, string EndDate, String QXID)//返回指定区站号、指定时间段市局三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
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

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public float[] SJJDWCALL(string StartDate, string EndDate)//返回所有乡镇指定时间段市局三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where Date>='{0}' AND Date<='{1}'", StartDate, EndDate);

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
                                        QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                        QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                        QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                        QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                        QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                        QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                    });
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
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

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public string[,] ZBXXTJ(string StartDate, string EndDate, String QXID, ref Int16 ZBJS)//返回指定旗县、指定时间段的人员名单及值班次数，引用参数ZBJS为该旗县值班基数
        {
            ObservableCollection<ZBXX> zbxx = new ObservableCollection<ZBXX>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from USERJL where QXID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID, StartDate, EndDate);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    zbxx.Add(new ZBXX()
                                    {
                                        userID = sqlreader.GetString(sqlreader.GetOrdinal("userID")),
                                    });
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = zbxx.Count;
            ZBXX[] s1 = zbxx.ToArray();
            string zbxxStr = "";
            for (int i = 0; i < s1.Length; i++)
            {
                zbxxStr += s1[i].userID + ',';

            }
            zbxxStr = zbxxStr.Substring(0, zbxxStr.Length - 1);
            string[] IDJLSz = zbxxStr.Split(',');//保存指定时间段所有值班人员ID

            string IDList = "";//保存指定时间段值班人员名单
            for (int i = 0; i < IDJLSz.Length; i++)
            {
                if (!IDList.Contains(IDJLSz[i]))
                {
                    IDList += IDJLSz[i] + ',';
                }
            }
            IDList = IDList.Substring(0, IDList.Length - 1);
            string[] SZLS1 = IDList.Split(',');//保存值班人员数组，人员ID不重复
            double douLS1;
            if (SZLS1.Length == 0)
            {
                douLS1 = 0;//防止没有记录时候报错
            }
            else
                douLS1 = Convert.ToDouble((IDJLSz.Length * 2)) / (SZLS1.Length * 3);
            ZBJS = Convert.ToInt16(Math.Round(douLS1, 0));//计算值班基数，2/3*值班总次数/（起止时间天数差+1）
            int[] CountSZ = new int[SZLS1.Length];//保存每个人值班次数
            string[,] TJSZ = new string[SZLS1.Length, 2];//保存返回统计信息，第一列为ID，第二列为值班次数
            for (int i = 0; i < SZLS1.Length; i++)
            {
                for (int j = 0; j < IDJLSz.Length; j++)
                {
                    if (SZLS1[i] == IDJLSz[j])
                    {
                        CountSZ[i]++;
                    }
                }
            }
            for (int i = 0; i < SZLS1.Length; i++)
            {
                TJSZ[i, 0] = SZLS1[i];
                TJSZ[i, 1] = CountSZ[i].ToString();
            }
            return TJSZ;
        }

        public string[] PeoleListTJ(string StartDate, string EndDate)//返回指定时间段的人员名单
        {
            ObservableCollection<ZBXX> zbxx = new ObservableCollection<ZBXX>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from USERJL where date>='{0}' AND date<='{1}'", StartDate, EndDate);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {
                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {


                                    zbxx.Add(new ZBXX()
                                    {
                                        userID = sqlreader.GetString(sqlreader.GetOrdinal("userID")),
                                    });
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = zbxx.Count;
            ZBXX[] s1 = zbxx.ToArray();
            string zbxxStr = "";
            for (int i = 0; i < s1.Length; i++)
            {
                zbxxStr += s1[i].userID + ',';

            }
            zbxxStr = zbxxStr.Substring(0, zbxxStr.Length - 1);
            string[] IDJLSz = zbxxStr.Split(',');//保存指定时间段所有值班人员ID

            string IDList = "";//保存指定时间段值班人员名单
            for (int i = 0; i < IDJLSz.Length; i++)
            {
                if (!IDList.Contains(IDJLSz[i]))
                {
                    IDList += IDJLSz[i] + ',';
                }
            }
            IDList = IDList.Substring(0, IDList.Length - 1);
            string[] SZLS1 = IDList.Split(',');//保存值班人员数组，人员ID不重复
            return SZLS1;
        }

        public float[] GRZQL(string StartDate, string EndDate, String userID, ref string qbdate)//返回指定区站号、指定时间段个人三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<PFZR> pfzrtj1 = new ObservableCollection<PFZR>();
            float[] tjsz = new float[10];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1}' AND Date<='{2}'", userID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PFZR()
                                        {
                                            QX24TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax24")),
                                            QX24TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin24")),
                                            QX24QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain24")),
                                            QX48TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax48")),
                                            QX48TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin48")),
                                            QX48QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain48")),
                                            QX72TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax72")),
                                            QX72TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin72")),
                                            QX72QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_Rain72")),
                                            fID = sqlreader.GetString(sqlreader.GetOrdinal("fromQXID")),
                                            ID = sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                                            Date = sqlreader.GetDateTime(sqlreader.GetOrdinal("Date")).ToString("yyyy-MM-dd")
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PFZR[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
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
                    tjsz[9]++;
                    if (s1[i].fID == s1[i].ID)
                    {
                        qbdate += " AND date!=" + '\'' + s1[i].Date + '\'';
                    }
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
                    zs[2]++;
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
                    zs[5]++;
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
                    zs[8]++;
                    tjsz[8] += s1[i].QX72QYZQL;
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

        public float[] GRJDWC(string StartDate, string EndDate, String userID)//返回指定区站号、指定时间段个人三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1}' AND Date<='{2}'", userID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("QX_SKTmin72")),
                                        });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
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

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public float[] GRSJJDWC(string StartDate, string EndDate, String userID)//返回指定人员、指定时间段市局三天预报的最高、最低气温与实况的平均绝对误差
        {
            ObservableCollection<PJWCList> pfzrtj1 = new ObservableCollection<PJWCList>();
            float[] tjsz = new float[6];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1}' AND Date<='{2}'", userID, StartDate, EndDate);

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
                                        pfzrtj1.Add(new PJWCList()
                                        {
                                            QX24TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                            QX24TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                            QX48TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                            QX48TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                            QX72TmaxJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                            QX72TminJDWC = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                        });
                                    }
                                    catch

                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = pfzrtj1.Count;
            PJWCList[] s1 = pfzrtj1.ToArray();
            int[] zs = new int[6];//保存计算平均绝对误差时候每个要素的个数总数
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

            }
            for (int i = 0; i < tjsz.Length; i++)
            {
                tjsz[i] = tjsz[i] / zs[i];
                tjsz[i] = (float)Math.Round(tjsz[i], 3);
            }
            return tjsz;
        }

        public float[] GRSJZQL(string StartDate, string EndDate, String userID, string qbdate)//返回指定区站号、指定时间段市局三天预报的最高、最低、晴雨准确率以及缺报率
        {
            ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
            float[] tjsz = new float[10];
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1}' AND Date<='{2}'", userID, StartDate, EndDate);
                    if (qbdate != "")
                    {
                        sql = string.Format(@"select * from TJ where PeopleID='{0}' AND Date>='{1}' AND Date<='{2}'", userID, StartDate, EndDate) + qbdate;
                    }
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
                                        SJ24TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax24")),
                                        SJ24TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin24")),
                                        SJ24QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain24")),
                                        SJ48TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax48")),
                                        SJ48TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin48")),
                                        SJ48QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain48")),
                                        SJ72TmaxZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmax72")),
                                        SJ72TminZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_SKTmin72")),
                                        SJ72QYZQL = sqlreader.GetFloat(sqlreader.GetOrdinal("SJ_Rain72")),
                                    });
                                    }
                                    catch
                                    {
                                    }
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }
            int ss = sjzqlTJ1.Count;
            ZQLTJ1[] s1 = sjzqlTJ1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
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
                    tjsz[9]++;
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
                    zs[2]++;
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
                    zs[5]++;
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
                    zs[8]++;
                    tjsz[8] += s1[i].SJ72QYZQL;
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


        public string[,] ZRPF(string StartDate, string times, String QXID)//返回指定旗县区站号指定时间、时次各个乡镇的逐日评分详情
        {
            ObservableCollection<ZRPFList> zrpflist = new ObservableCollection<ZRPFList>();
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            int coutXZGS = 0;//保存该旗县乡镇个数
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开
                    string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date='{1}'", QXID, StartDate);

                    try
                    {
                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {

                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                while (sqlreader.Read())
                                {

                                    coutXZGS++;
                                }

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }
            }
            catch (Exception ex)
            {

            }
            string[,] strszData = new string[coutXZGS, 13];//保存返回数组
            if (coutXZGS > 0)
            {
                try
                {
                    using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                    {
                        mycon1.Open();//打开
                        string sql = string.Format(@"select * from TJ where fromQXID='{0}' AND Date='{1}'", QXID, StartDate);

                        using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                        {

                            using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                            {
                                int count = 0;
                                while (sqlreader.Read())
                                {
                                    string qxrain = "QX_Rain" + times, sjrain = "SJ_Rain" + times;
                                    strszData[count, 0] = sqlreader.GetString(sqlreader.GetOrdinal("StationID"));
                                    strszData[count, 8] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal(qxrain))).ToString();
                                    if (strszData[count, 8] == "1")
                                    {
                                        strszData[count, 8] = "✔";
                                    }
                                    else if (strszData[count, 8] == "0")
                                    {
                                        strszData[count, 8] = "×";
                                    }
                                    else if (strszData[count, 8] == "999999")
                                    {
                                        strszData[count, 8] = "缺测";
                                    }
                                    else if (strszData[count, 8] == "-999999")
                                    {
                                        strszData[count, 8] = "缺报";
                                    }
                                    else if (strszData[count, 8] == "999998")
                                    {
                                        strszData[count, 8] = "不观测";
                                    }
                                    strszData[count, 11] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal(sjrain))).ToString();
                                    if (strszData[count, 11] == "1")
                                    {
                                        strszData[count, 11] = "✔";
                                    }
                                    else if (strszData[count, 11] == "0")
                                    {
                                        strszData[count, 11] = "×";
                                    }
                                    else if (strszData[count, 11] == "999999")
                                    {
                                        strszData[count, 11] = "缺测";
                                    }
                                    else if (strszData[count, 11] == "-999999")
                                    {
                                        strszData[count, 11] = "缺报";
                                    }
                                    else if (strszData[count, 11] == "999998")
                                    {
                                        strszData[count, 11] = "不观测";
                                    }
                                    count++;

                                }
                            }
                        }


                        mycon1.Close();
                    }
                    using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                    {
                        mycon1.Open();//打开
                        int intDays = Convert.ToInt16(times) / 24;
                        DateTime dtLS = Convert.ToDateTime(StartDate);
                        dtLS = dtLS.AddDays(intDays);
                        string SKDate = dtLS.ToString("yyyy-MM-dd");//查询日期
                        for (int i = 0; i < coutXZGS; i++)
                        {
                            string sql = string.Format(@"select * from SK where StationID='{0}' AND Date='{1}'", strszData[i, 0], SKDate);
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                            {
                                using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                                {
                                    int coutLS = 0;
                                    while (sqlreader.Read())
                                    {
                                        coutLS++;
                                        strszData[i, 2] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmax"))).ToString();
                                        if (strszData[i, 2] == "999999")
                                        {
                                            strszData[i, 2] = "缺测";
                                        }
                                        else if (strszData[i, 2] == "-999999")
                                        {
                                            strszData[i, 2] = "缺报";
                                        }
                                        else if (strszData[i, 2] == "999998")
                                        {
                                            strszData[i, 2] = "不观测";
                                        }
                                        strszData[i, 5] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmin"))).ToString();
                                        if (strszData[i, 5] == "999999")
                                        {
                                            strszData[i, 5] = "缺测";
                                        }
                                        else if (strszData[i, 5] == "-999999")
                                        {
                                            strszData[i, 5] = "缺报";
                                        }
                                        else if (strszData[i, 5] == "999998")
                                        {
                                            strszData[i, 5] = "不观测";
                                        }
                                        strszData[i, 9] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Rain"))).ToString();
                                        if (strszData[i, 9] == "999999")
                                        {
                                            strszData[i, 9] = "缺测";
                                        }
                                        else if (strszData[i, 9] == "-999999")
                                        {
                                            strszData[i, 9] = "缺报";
                                        }
                                        else if (strszData[i, 9] == "999998")
                                        {
                                            strszData[i, 9] = "不观测";
                                        }
                                        strszData[i, 12] = sqlreader.GetString(sqlreader.GetOrdinal("Name"));
                                    }
                                    if (coutLS == 0)
                                    {
                                        strszData[i, 2] = "缺测";
                                        strszData[i, 5] = "缺测";
                                        strszData[i, 9] = "缺测";
                                    }
                                }
                            }
                        }

                        mycon1.Close();
                    }
                    using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                    {
                        mycon1.Open();//打开
                        for (int i = 0; i < coutXZGS; i++)
                        {
                            string sql = string.Format(@"select * from SJYB where StationID='{0}' AND Date='{1}'", strszData[i, 0], StartDate);
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                            {
                                using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                                {
                                    int coutLS = 0;
                                    while (sqlreader.Read())
                                    {
                                        coutLS++;
                                        strszData[i, 3] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmax" + times))).ToString();
                                        strszData[i, 6] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmin" + times))).ToString();
                                        strszData[i, 10] = sqlreader.GetString(sqlreader.GetOrdinal("Rain" + times));
                                    }
                                    if (coutLS == 0)
                                    {
                                        strszData[i, 3] = "缺报";
                                        strszData[i, 6] = "缺报";
                                        strszData[i, 9] = "缺报";
                                    }
                                }
                            }
                        }

                        mycon1.Close();
                    }

                    using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                    {
                        mycon1.Open();//打开
                        for (int i = 0; i < coutXZGS; i++)
                        {
                            string sql = string.Format(@"select * from QXYB where StationID='{0}' AND Date='{1}'", strszData[i, 0], StartDate);
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                            {
                                using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                                {
                                    int coutLS = 0;
                                    while (sqlreader.Read())
                                    {
                                        coutLS++;
                                        strszData[i, 1] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmax" + times))).ToString();
                                        strszData[i, 4] = (sqlreader.GetSqlSingle(sqlreader.GetOrdinal("Tmin" + times))).ToString();
                                        strszData[i, 7] = sqlreader.GetString(sqlreader.GetOrdinal("Rain" + times));

                                    }
                                    if (coutLS == 0)
                                    {
                                        strszData[i, 1] = "缺报";
                                        strszData[i, 4] = "缺报";
                                        strszData[i, 7] = "缺报";
                                    }
                                }
                            }
                        }

                        mycon1.Close();
                    }
                }



                catch (Exception ex)
                {

                }

            }
            return strszData;
        }

        public string[,] DBLTJ(string[] QXID, string[] QXName, string StartDate, string EndDate)//返回指定旗县ID列表指导起止时间的旗县名称、到报率、缺报率、缺报日期、逾期率、逾期日期
        {
            string con = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string[,] dataFH = new string[QXID.Length, 6];
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
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            try
            {

                using (SqlConnection mycon1 = new SqlConnection(con))//创建SQL连接对象)
                {
                    mycon1.Open();//打开

                    for (int i = 0; i < QXID.Length; i++)
                    {
                        string sql = string.Format(@"select * from TJ where StationID='{0}' AND Date>='{1}' AND Date<='{2}'", QXID[i], StartDate, EndDate);
                        dataFH[i, 0] = QXName[i];
                        try
                        {
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                            {
                                using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                                {
                                    int ZS = 0, QBGS = 0, YQGS = 0;
                                    string QBDate = "", YQDate = "";
                                    while (sqlreader.Read())
                                    {
                                        string strLS = sqlreader.GetSqlSingle(sqlreader.GetOrdinal("QX_SKTmax24")).ToString();
                                        ZS++;
                                        if (strLS == "-999999")
                                        {
                                            QBGS++;
                                            QBDate += sqlreader.GetDateTime(sqlreader.GetOrdinal("Date")).ToString("yyyy年MM月dd日") + ";";
                                        }
                                        else
                                        {
                                            bool boolLS = sqlreader.GetBoolean(sqlreader.GetOrdinal("SFzhundian"));
                                            if (!boolLS)//如果没有缺报并且逾期
                                            {
                                                YQGS++;
                                                YQDate += sqlreader.GetDateTime(sqlreader.GetOrdinal("Date")).ToString("yyyy年MM月dd日") + ";";
                                            }
                                        }
                                    }
                                    if (QBDate.Length == 0)
                                    {
                                        QBDate = "没有缺报";
                                    }
                                    if (YQDate.Length == 0)
                                    {
                                        YQDate = "没有逾期";
                                    }
                                    dataFH[i, 2] = Convert.ToString(Math.Round(((double)QBGS * 100 / ZS), 2));
                                    dataFH[i, 1] = Convert.ToString(100 - Convert.ToDouble(dataFH[i, 2]));
                                    dataFH[i, 3] = QBDate;
                                    dataFH[i, 4] = Convert.ToString(Math.Round(((double)YQGS * 100 / ZS), 2));
                                    dataFH[i, 5] = YQDate;
                                }
                            }

                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    mycon1.Close();
                }



            }
            catch (Exception ex)
            {

            }


            return dataFH;
        }
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
        }

        public class PFZR//集体评分逐日查询统计信息列表
        {
            public string fID { get; set; }
            public string ID { get; set; }
            public string Date { get; set; }
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
        }

        public class PJWCList//旗县平均绝对误差列表
        {
            public float QX24TmaxJDWC { get; set; }
            public float QX24TminJDWC { get; set; }
            public float QX48TmaxJDWC { get; set; }
            public float QX48TminJDWC { get; set; }
            public float QX72TmaxJDWC { get; set; }
            public float QX72TminJDWC { get; set; }
        }

        public class ZBXX//个人值班信息列表
        {
            public string userID { get; set; }
        }

        public class ZRPFList//逐日评分列表
        {

            public string StationID { get; set; }
            public string QXGW { get; set; }
            public string SKGW { get; set; }
            public string SJGW { get; set; }
            public string QXDW { get; set; }
            public string SKDW { get; set; }
            public string SJDW { get; set; }
            public string QXTQ { get; set; }
            public string QXQY { get; set; }
            public string SKJS { get; set; }
            public string SJRQ { get; set; }
            public string SJQY { get; set; }
        }
    }
}
