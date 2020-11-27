using Aspose.Words;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace sjzd
{
    class 短期预报
    {
        public void DCWord(Int16 sc, string ybName, string fbName, string qfName)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb(strDate, sc.ToString().PadLeft(2, '0'));
            if (ybData.Trim().Length > 0)
            {
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = "";

                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "呼和浩特短期预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yyyy-MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    if (sc == 8)
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\短期预报模板08.doc";
                        SJsaPath += DateTime.Now.ToString("yyyyMMdd") + "11.doc";
                    }
                    else
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\短期预报模板20.doc";
                        SJsaPath += DateTime.Now.ToString("yyyyMMdd") + "17.doc";
                    }

                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                    builder.Font.Size = 12;
                    builder.Font.Name = "宋体";
                    builder.MoveToBookmark("主班");
                    builder.Write(ybName);
                    builder.MoveToBookmark("副班");
                    builder.Write(fbName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("天气24"); //开始添加值
                    string erh = "相对湿度" + ERH("53463", sc, 24) + "。\r\n";
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[4].Contains("转") && szls[5].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                erh = "全市" + szls[3] + "，" + Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[1] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[1] + "，" + erh;
                            }
                            else
                            {
                                erh = "全市" + szls[3] + "，" + szls[4] + szls[5] + "，" + erh;
                            }

                            break;

                        }
                    }
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.Write(erh);
                    List<YBList> yBLists = CLYB(ybData);
                    if (yBLists.Count > 0)
                    {
                        builder.MoveToBookmark("table"); //开始添加值

                        builder.RowFormat.HeadingFormat = true;
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                        builder.Font.Bold = true;

                        builder.InsertCell();

                        builder.CellFormat.Width = 10;
                        builder.Write("地区");
                        builder.InsertCell();

                        builder.CellFormat.Width = 15;
                        builder.Write("天气现象");
                        builder.InsertCell();
                        builder.CellFormat.Width = 20;
                        builder.Write("气温");
                        builder.EndRow();
                        builder.Font.Bold = false;
                        foreach (YBList yBList in yBLists)
                        {

                            builder.InsertCell();
                            builder.CellFormat.Width = 10;
                            builder.Write(yBList.Name);

                            builder.InsertCell();
                            builder.CellFormat.Width = 15;
                            builder.Write(yBList.TQ);

                            builder.InsertCell();
                            builder.CellFormat.Width = 20;
                            builder.Write(yBList.TEM);
                            builder.EndRow();
                        }
                        builder.EndTable();
                    }
                    builder.MoveToBookmark("天气48"); //开始添加值
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    erh = "相对湿度" + ERH("53463", sc, 48) + "。\r\n";
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[10].Contains("转") && szls[9].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                erh = "全市" + szls[8] + "，" + Regex.Split(szls[9], "转", RegexOptions.IgnoreCase)[0] + Regex.Split(szls[10], "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(szls[9], "转", RegexOptions.IgnoreCase)[1] + Regex.Split(szls[10], "转", RegexOptions.IgnoreCase)[1] + "，" + erh;
                            }
                            else
                            {
                                erh = "全市" + szls[8] + "，" + szls[9] + szls[10] + "，" + erh;
                            }

                            break;

                        }
                    }
                    builder.Write(erh);





                    doc.Save(SJsaPath);
                    MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + SJsaPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        try
                        {
                            静态类.OpenBrowser(SJsaPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }


                }

                catch (Exception)
                {

                }
            }
            else
            {
                MessageBox.Show("城镇预报数据获取失败，无法制作产品");
            }


        }
        public string DCWordNew(Int16 sc, string ybName, string fbName, string qfName, ref string error)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb(strDate, sc.ToString().PadLeft(2, '0'));
            if (ybData.Trim().Length > 0)
            {
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = "";

                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "呼和浩特短期预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yyyy-MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    if (sc == 8)
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\短期预报模板08.doc";
                        SJsaPath += DateTime.Now.ToString("yyyyMMdd") + "10.doc";
                    }
                    else
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\短期预报模板20.doc";
                        SJsaPath += DateTime.Now.ToString("yyyyMMdd") + "17.doc";
                    }

                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                    builder.Font.Size = 12;
                    builder.Font.Name = "宋体";
                    builder.MoveToBookmark("主班");
                    builder.Write(ybName);
                    builder.MoveToBookmark("副班");
                    builder.Write(fbName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("天气24"); //开始添加值
                    string erh = "相对湿度" + ERH("53463", sc, 24) + "。\r\n";
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[4].Contains("转") && szls[5].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                erh = "全市" + szls[3] + "，" + Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[1] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[1] + "，" + erh;
                            }
                            else
                            {
                                erh = "全市" + szls[3] + "，" + szls[4] + szls[5] + "，" + erh;
                            }

                            break;

                        }
                    }
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.Write(erh);
                    List<YBList> yBLists = CLYB(ybData);
                    if (yBLists.Count > 0)
                    {
                        builder.MoveToBookmark("table"); //开始添加值

                        builder.RowFormat.HeadingFormat = true;
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                        builder.Font.Bold = true;

                        builder.InsertCell();

                        builder.CellFormat.Width = 10;
                        builder.Write("地区");
                        builder.InsertCell();

                        builder.CellFormat.Width = 15;
                        builder.Write("天气现象");
                        builder.InsertCell();
                        builder.CellFormat.Width = 20;
                        builder.Write("气温");
                        builder.EndRow();
                        builder.Font.Bold = false;
                        foreach (YBList yBList in yBLists)
                        {

                            builder.InsertCell();
                            builder.CellFormat.Width = 10;
                            builder.Write(yBList.Name);

                            builder.InsertCell();
                            builder.CellFormat.Width = 15;
                            builder.Write(yBList.TQ);

                            builder.InsertCell();
                            builder.CellFormat.Width = 20;
                            builder.Write(yBList.TEM);
                            builder.EndRow();
                        }
                        builder.EndTable();
                    }
                    builder.MoveToBookmark("天气48"); //开始添加值
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    erh = "相对湿度" + ERH("53463", sc, 48) + "。\r\n";
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[10].Contains("转") && szls[9].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                erh = "全市" + szls[8] + "，" + Regex.Split(szls[9], "转", RegexOptions.IgnoreCase)[0] + Regex.Split(szls[10], "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(szls[9], "转", RegexOptions.IgnoreCase)[1] + Regex.Split(szls[10], "转", RegexOptions.IgnoreCase)[1] + "，" + erh;
                            }
                            else
                            {
                                erh = "全市" + szls[8] + "，" + szls[9] + szls[10] + "，" + erh;
                            }

                            break;

                        }
                    }
                    builder.Write(erh);





                    doc.Save(SJsaPath);
                    return SJsaPath;



                }

                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            else
            {
                error = "城镇预报数据获取失败，无法制作产品";
            }

            return "";
        }
        public string ERH(string ID, Int16 sc, int sx)
        {
            string erh = "";
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "DB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                //预报取当前时次的预报

                mycon.Open();//打开
                string sql = "";
                if (sc == 8)
                {
                    sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID = '{2}' and sc=8 and sx ={0} and date='{1}'", sx, DateTime.Now.ToString("yyyy-MM-dd"), ID);
                }
                else
                {
                    sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID =  '{2}' and sc=20 and sx ={0}  and date='{1}'", sx, DateTime.Now.ToString("yyyy-MM-dd"), ID);
                }
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                if (sqlreader.HasRows)
                {
                    while (sqlreader.Read())
                    {


                        try
                        {
                            erh = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHI")), 0) + "～" + Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHA")), 0) + "%";

                        }
                        catch (Exception)
                        {

                            sqlreader.Close();
                            if (sc == 8)
                            {
                                sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID = '{2}' and sc=8 and sx ={0} and date='{1}'", sx + 24, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                            }
                            else
                            {
                                sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID =  '{2}' and sc=20 and sx ={0}  and date='{1}'", sx + 24, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                            }
                            sqlman = new SqlCommand(sql, mycon);
                            sqlreader = sqlman.ExecuteReader();
                            if (sqlreader.HasRows)
                            {
                                while (sqlreader.Read())
                                {

                                    try
                                    {
                                        erh = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHI")), 0) + "～" + Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHA")), 0) + "%";

                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                            return erh;
                        }
                    }
                }
                else
                {
                    sqlreader.Close();
                    if (sc == 8)
                    {
                        sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID =  '{2}' and sc=8 and sx ={0} and date='{1}'", sx + 24, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                    }
                    else
                    {
                        sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID =  '{2}' and sc=20 and sx ={0}  and date='{1}'", sx + 24, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                    }
                    sqlman = new SqlCommand(sql, mycon);
                    sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {

                            try
                            {
                                erh = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHI")), 0) + "～" + Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERHA")), 0) + "%";

                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

            }
            return erh;
        }

        public List<YBList> CLYB(string ybData)
        {
            List<YBList> yBLists = new List<YBList>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "xzjxhDB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                string sql = String.Format("select * from 社区精细化预报站点 where Station_levl = 92 order by StatioID");
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    try
                    {


                        yBLists.Add(new YBList()
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
            }

            for (int i = 0; i < yBLists.Count; i++)
            {
                foreach (string sls in ybData.Split('\n'))
                {
                    string[] szls = sls.Split(',');
                    if (szls[0].Trim() == yBLists[i].ID)
                    {
                        yBLists[i].TQ = szls[3];
                        yBLists[i].TEM = szls[1] + "～" + szls[2] + "℃";

                        break;

                    }
                }
            }


            return yBLists;
        }

        public class YBList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string TQ { get; set; }
            public string TEM { get; set; }
        }
    }
}
