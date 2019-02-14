using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace LS
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

                            }
                        }

                    }
                    catch (Exception ex)
                    {

                    }
                    mycon1.Close();
                }
            


            }
            catch(Exception ex)
            {

            }
            int ss = sjzqlTJ1.Count;
            ZQLTJ1[] s1=sjzqlTJ1.ToArray();
            int[] zs = new int[9];//保存计算准确率时候每个要素的总数，只需统计三天的最高最低晴雨，不用统计缺报，因为缺报率直接除元素总数即可
            for(int i=0;i<s1.Length;i++)
            {
                if(s1[i].SJ24TmaxZQL < 999998&& s1[i].SJ24TmaxZQL > -999999)
                {
                    zs[0]++;
                    if (Math.Abs(s1[i].SJ24TmaxZQL) <= 2)
                    {
                        tjsz[0]++;
                    }
                }
                else if(s1[i].SJ24TmaxZQL == -999999)
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
                    tjsz[2]+= s1[i].SJ24QYZQL;   
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
            for(int i=0;i<tjsz.Length;i++)
            {
                if(i<tjsz.Length-1)
                {
                    tjsz[i] = tjsz[i] / zs[i];
                    tjsz[i] = (float)Math.Round(tjsz[i]*100,2);
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








        public class ZQLTJ1//统计信息列表
        {
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
    }
}
