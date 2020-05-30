using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;

namespace xzjxhyb_DBmain
{
    public class 统计信息
    {
        string con = "";
        public 统计信息()
        {
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
            catch
            {

            }
        }
        public bool 月检验结果入库(DateTime endDateTime)
        {
            try
            {
                List<检验结果> dataLists = new List<检验结果>();
                try
                {
                    string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
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
                    string[] QXID = new string[intQXGS];
                    string[] QXName = new string[intQXGS];

                    using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//第二行开始每两行为旗县及乡镇区站号列表
                    {
                        string line1 = "";
                        Int16 lineCount = 0;
                        Int16 count = 0;
                        while (lineCount < intQXGS * 2 + 1)
                        {
                            line1 = sr.ReadLine();
                            if (lineCount > 0)
                            {
                                if (lineCount % 2 == 1)
                                {
                                    QXName[count] = line1.Split(',')[0];
                                }
                                else
                                {
                                    QXID[count++] = line1.Split(',')[0];

                                }
                            }
                            lineCount++;
                        }
                    }

                    TJ tj = new TJ();
                    DateTime mysDate = endDateTime.AddDays(1 - endDateTime.Day), myeDate = endDateTime.AddDays(1 - endDateTime.Day).Date.AddMonths(1).AddSeconds(-1);
                    try
                    {
                        string startDate = mysDate.ToString("yyyy-MM-dd");
                        string endDate = myeDate.ToString("yyyy-MM-dd");
                        for (int i = 0; i < intQXGS + 1; i++)
                        {
                            if (i < intQXGS)
                            {
                                float[] zqlFloat = tj.QXZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率
                                float[] XSList = new float[8];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                                                              //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                                XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2);
                                XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2);
                                XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2);
                                XSList[3] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[0] + 0.3 * XSList[1] + 0.3 * XSList[2]), 2);//总评分
                                float[] QXJDWCSZ = tj.QXJDWC(startDate, endDate, QXID[i]);
                                float[] SJJDWCSZ = tj.SJJDWC(startDate, endDate, QXID[i]);
                                float[] SJzqlFloat = tj.SJZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率，主要计算技巧用晴雨准确率
                                float[] WDJQ = new float[6];//保存三天的最高、最低温度技巧
                                try
                                {
                                    for (int j = 0; j < 6; j++)
                                    {
                                        if (SJJDWCSZ[j] == 0)
                                        {
                                            WDJQ[j] = 1.01F * 100;
                                        }
                                        else
                                        {
                                            WDJQ[j] = (SJJDWCSZ[j] - QXJDWCSZ[j]) / SJJDWCSZ[j];
                                            WDJQ[j] = (float)Math.Round(WDJQ[j] * 100, 2);
                                        }
                                    }

                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                float[] QYJQ = new float[3];
                                try
                                {
                                    for (int j = 0; j < 3; j++)
                                    {
                                        QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                        QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }

                                XSList[4] = (float)Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6) / 24, 2);//晴雨技巧
                                XSList[5] = (float)Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6) / 24, 2);//高温技巧
                                XSList[6] = (float)Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) / 24, 2);//低温技巧
                                XSList[7] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[4] + 0.3 * XSList[5] + 0.3 * XSList[6]), 2);//总技巧
                                dataLists.Add(new 检验结果()
                                {
                                    Name = QXName[i],
                                    QYPF = XSList[0],
                                    GWPF = XSList[1],
                                    DWPF = XSList[2],
                                    ZHPF = XSList[3],
                                    QYJQ = XSList[4],
                                    GWJQ = XSList[5],
                                    DWJQ = XSList[6],
                                    AllJQ = XSList[7],
                                    Date = mysDate,
                                    StationID = QXID[i],
                                    QYPF24 = zqlFloat[2],
                                    QYPF48 = zqlFloat[5],
                                    QYPF72 = zqlFloat[8],
                                    GWPF24 = zqlFloat[0],
                                    GWPF48 = zqlFloat[3],
                                    GWPF72 = zqlFloat[6],
                                    DWPF24 = zqlFloat[1],
                                    DWPF48 = zqlFloat[4],
                                    DWPF72 = zqlFloat[7],
                                    QYJQ24 = QYJQ[0],
                                    QYJQ48 = QYJQ[1],
                                    QYJQ72 = QYJQ[2],
                                    GWJQ24 = WDJQ[0],
                                    GWJQ48 = WDJQ[2],
                                    GWJQ72 = WDJQ[4],
                                    DWJQ24 = WDJQ[1],
                                    DWJQ48 = WDJQ[3],
                                    DWJQ72 = WDJQ[5],
                                });
                            }
                            else
                            {


                                float[] zqlFloat = tj.SJQSZQL(startDate, endDate);
                                float[] XSList = new float[4];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                                                              //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                                XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2);
                                XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2);
                                XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2);
                                XSList[3] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[0] + 0.3 * XSList[1] + 0.3 * XSList[2]), 2);//总评分
                                dataLists.Add(new 检验结果()
                                {
                                    Name = "市台",
                                    QYPF = XSList[0],
                                    GWPF = XSList[1],
                                    DWPF = XSList[2],
                                    ZHPF = XSList[3],
                                    QYJQ = 0,
                                    GWJQ = 0,
                                    DWJQ = 0,
                                    AllJQ = 0,
                                    Date = mysDate,
                                    StationID = "BFHT",
                                    QYPF24 = zqlFloat[2],
                                    QYPF48 = zqlFloat[5],
                                    QYPF72 = zqlFloat[8],
                                    GWPF24 = zqlFloat[0],
                                    GWPF48 = zqlFloat[3],
                                    GWPF72 = zqlFloat[6],
                                    DWPF24 = zqlFloat[1],
                                    DWPF48 = zqlFloat[4],
                                    DWPF72 = zqlFloat[7],
                                    QYJQ24 = 0,
                                    QYJQ48 = 0,
                                    QYJQ72 = 0,
                                    GWJQ24 = 0,
                                    GWJQ48 = 0,
                                    GWJQ72 = 0,
                                    DWJQ24 = 0,
                                    DWJQ48 = 0,
                                    DWJQ72 = 0,
                                });
                            }

                        }
                    }
                    catch
                    {

                    }

                }
                catch
                {
                }
                if (dataLists.Count > 0)
                {
                    DataTable dataTable = ToDataTable(dataLists);
                    SqlBulkCopyByDatatable(con, "检验结果_站点_月_LS", dataTable);
                    return true;
                }
            }
            catch
            {
            }
            return false;
        }
        #region  SqlBulkCopy批量快速入库

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
                catch (Exception)
                {
                }
            }
        }

        #endregion

        /// <summary>
        /// Convert a List{T} to a DataTable.
        /// </summary>

        #region LIST转换为Datatable

        private DataTable ToDataTable<T>(List<T> items)

        {
            DataTable tb = new DataTable(typeof(T).Name);


            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);


            foreach (PropertyInfo prop in props)

            {
                Type t = GetCoreType(prop.PropertyType);

                tb.Columns.Add(prop.Name, t);
            }


            foreach (T item in items)

            {
                object[] values = new object[props.Length];


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
            return !t.IsValueType || t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
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

                return Nullable.GetUnderlyingType(t);
            }

            return t;
        }

        #endregion

        public class 检验结果
        {
            public string Name { get; set; }
            public string StationID { get; set; }
            public DateTime Date { get; set; }
            public double QYPF { get; set; }
            public double GWPF { get; set; }
            public double DWPF { get; set; }
            public double ZHPF { get; set; }
            public double QYJQ { get; set; }
            public double GWJQ { get; set; }
            public double DWJQ { get; set; }
            public double AllJQ { get; set; }
            public double QYPF24 { get; set; }
            public double GWPF24 { get; set; }
            public double DWPF24 { get; set; }
            public double QYPF48 { get; set; }
            public double GWPF48 { get; set; }
            public double DWPF48 { get; set; }
            public double QYPF72 { get; set; }
            public double GWPF72 { get; set; }
            public double DWPF72 { get; set; }
            public double QYJQ24 { get; set; }
            public double GWJQ24 { get; set; }
            public double DWJQ24 { get; set; }
            public double QYJQ48 { get; set; }
            public double GWJQ48 { get; set; }
            public double DWJQ48 { get; set; }
            public double QYJQ72 { get; set; }
            public double GWJQ72 { get; set; }
            public double DWJQ72 { get; set; }
        }
    }
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
            catch (Exception)
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
                    catch (Exception)
                    {

                    }
                    mycon1.Close();
                }



            }
            catch (Exception)
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

        public string[,] PeoleListTJ(string StartDate, string EndDate)//返回指定时间段的人员名单
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
                                        userName = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
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
            int ss = zbxx.Count;
            ZBXX[] s1 = zbxx.ToArray();
            string zbxxStr = "";
            string zbxxStr2 = "";
            for (int i = 0; i < s1.Length; i++)
            {
                zbxxStr += s1[i].userID + ',';
                zbxxStr2 += s1[i].userName + ',';

            }
            zbxxStr = zbxxStr.Substring(0, zbxxStr.Length - 1);
            zbxxStr2 = zbxxStr2.Substring(0, zbxxStr2.Length - 1);
            string[] IDJLSz = zbxxStr.Split(',');//保存指定时间段所有值班人员ID
            string[] nameJLsz = zbxxStr2.Split(',');//保存指定时间段所有值班人员ID

            string IDList = "";//保存指定时间段值班人员名单
            string NameList = "";//保存指定时间段值班人员名单
            for (int i = 0; i < IDJLSz.Length; i++)
            {
                if (!IDList.Contains(IDJLSz[i]))
                {
                    IDList += IDJLSz[i] + ',';
                    NameList += nameJLsz[i] + ',';
                }
            }
            IDList = IDList.Substring(0, IDList.Length - 1);
            NameList = NameList.Substring(0, NameList.Length - 1);
            string[] SZLS1 = IDList.Split(',');//保存值班人员数组，人员ID不重复
            string[] SZLS2 = NameList.Split(',');//保存值班人员数组，人员ID不重复
            string[,] FHSZ = new string[SZLS1.Length, 2];
            for (int i = 0; i < SZLS1.Length; i++)
            {
                FHSZ[i, 0] = SZLS1[i];
                FHSZ[i, 1] = SZLS2[i];
            }
            return FHSZ;
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
                                            Date = sqlreader.GetDateTime(sqlreader.GetOrdinal("Date"))
                                                .ToString("yyyy-MM-dd")
                                        });
                                    }
                                    catch
                                    {

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
                    catch (Exception)
                    {

                    }
                    mycon1.Close();
                }
            }
            catch (Exception)
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



                catch (Exception)
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
                        catch (Exception)
                        {

                        }
                    }

                    mycon1.Close();
                }



            }
            catch (Exception)
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
            public string userName { get; set; }
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