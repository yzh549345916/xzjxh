using Aspose.Words;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace sjzd.类
{
    class 气象条件与空气质量预报
    {
        public string DCWord( string ybName, string shName, string qfName, ref string error)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb7days(strDate, 8.ToString().PadLeft(2, '0'));
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
                            if (line.Split('=')[0] == "气象条件与空气质量预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yyyy.MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJMBPath = Environment.CurrentDirectory + @"\模版\呼和浩特市气象条件与空气质量预报模板.docx";
                    SJsaPath += "呼和浩特市气象条件与空气质量预报"+DateTime.Now.ToString("MM.dd") + ".docx";

                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("期数");
                    builder.Font.Size = 10.5;
                    builder.Font.Name = "黑体";
                    builder.Write(DateTime.Now.ToString("yyyy年第")+DateTime.Now.DayOfYear.ToString().PadLeft(3, '0'));
                    builder.MoveToBookmark("日期");
                    builder.Write(DateTime.Now.ToString("yyyy年M月d日")); 
                    builder.Font.Size = 12;
                    builder.Font.Name = "宋体";
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("审核");
                    builder.Write(shName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("预报表格");

                    List<myerh> myERH = ERH("53463", 168);
                    List<YBList> myYB=CLYB(ybData, myERH);
                    myERH.Clear();
                    if (myYB.Count > 0)
                    {
                        builder.MoveToBookmark("table"); //开始添加表格
                        builder.RowFormat.HeadingFormat = true;
                        builder.ParagraphFormat.LineSpacing = 12.0;
                        builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                        builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center; //垂直居中对齐
                        builder.RowFormat.Height = 33;
                        builder.InsertCell();
                        builder.Font.Size = 11;
                        builder.Font.Bold = true;
                        builder.CellFormat.Width = 100;
                        builder.Write("日期");
                        builder.InsertCell();
                        builder.CellFormat.Width = 180;
                        builder.Write("风速");
                        builder.InsertCell();
                        builder.CellFormat.Width = 90;
                        builder.Write("风向");
                        builder.InsertCell();
                        builder.CellFormat.Width = 90;
                        builder.Write("温度℃");
                        builder.InsertCell();
                        builder.CellFormat.Width = 90;
                        builder.Write("湿度");
                        builder.InsertCell();
                        builder.CellFormat.Width = 100;
                        builder.Write("天气现象");
                        builder.InsertCell();
                        builder.CellFormat.Width = 170;
                        builder.Write("等级");
                        builder.InsertCell();
                        builder.CellFormat.Width = 100;
                        builder.Write("AQI范围");
                        builder.InsertCell();
                        builder.CellFormat.Width = 140;
                        builder.Write("首要污染物");
                        builder.EndRow();
                        builder.Font.Size = 12;
                        builder.Font.Bold = false;
                        for (int i = 0; i < 7; i++)
                        {
                           
                            try
                            {
                                if (myYB.Exists(y => y.sx == i))
                                {
                                    YBList data = myYB.First(y => y.sx == i);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 100;
                                    builder.Write(DateTime.Now.AddDays(i).ToString("M月d日"));
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 180;
                                    builder.Write(data.FS);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 90;
                                    builder.Write(data.FX);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 90;
                                    builder.Write(data.TEM);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 90;
                                    builder.Write(data.ERH);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 100;
                                    builder.Write(data.TQ);
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 170;
                                    builder.Write("");
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 100;
                                    builder.Write("");
                                    builder.InsertCell();
                                    builder.CellFormat.Width = 140;
                                    builder.Write("");
                                    builder.EndRow();
                                }
                               
                            }
                            catch
                            {
                            }
                           
                        }
                        builder.EndTable();
                    }

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
        public List<myerh> ERH(string ID, int sx)
        {
            List<myerh> dataLists = new List<myerh>();
            List<myerh> dataListLS = new List<myerh>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "DB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                //预报取当前时次的预报

                mycon.Open();//打开
                string sql = "";
                sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID = '{2}' and sc=8 and sx <={0} and date='{1}'", sx, DateTime.Now.ToString("yyyy-MM-dd"), ID);
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                if (sqlreader.HasRows)
                {
                    while (sqlreader.Read())
                    {
                        try
                        {
                            dataListLS.Add(new myerh()
                            {
                                sx= sqlreader.GetInt16(sqlreader.GetOrdinal("SX")),
                                erhMin = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 0)
                            });
                        }
                        catch (Exception)
                        {

                            sqlreader.Close();
                            sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID = '{2}' and sc=20 and sx <={0} and date='{1}'", sx + 12, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                            sqlman = new SqlCommand(sql, mycon);
                            sqlreader = sqlman.ExecuteReader();
                            if (sqlreader.HasRows)
                            {
                                while (sqlreader.Read())
                                {

                                    try
                                    {
                                        dataListLS.Add(new myerh()
                                        {
                                            sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX"))-12,
                                            erhMin = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 0)
                                        });
                                    }
                                    catch (Exception)
                                    {
                                    }
                                }
                            }

                        }
                    }
                }
                else
                {
                    sqlreader.Close();
                    sql = String.Format("select * from 全国智能网格预报服务产品24h240 where StatioID = '{2}' and sc=20 and sx <={0} and date='{1}'", sx + 12, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"), ID);
                    sqlman = new SqlCommand(sql, mycon);
                    sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {

                            try
                            {
                                dataListLS.Add(new myerh()
                                {
                                    sx = sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 12,
                                    erhMin = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 0)
                                });
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

            }

            for (int i = 0; i <= 144; i = i + 24)
            {
                List<myerh> dataListLS2 = dataListLS.FindAll(y => y.sx >= i && y.sx <= i + 24).OrderBy(y=>y.erhMin).ToList();
                dataLists.Add(new myerh()
                {
                    sx=i+24,
                    erhMin= dataListLS2[0].erhMin,
                    erhMax= dataListLS2[dataListLS2.Count-1].erhMin
                });
            }
            return dataLists;
        }

        public List<YBList> CLYB(string ybData, List<myerh> myerhs)
        {
            List<YBList> yBLists = new List<YBList>();
            foreach (string sls in ybData.Split('\n'))
            {
                string[] szls = sls.Split(',');
                if (szls[0].Trim() == "53463")
                {
                    for (int i = 0; i < 7; i++)
                    {
                        string myerhstr = "";
                        try
                        {
                            if (myerhs.Exists(y => y.sx == (i + 1) * 24))
                            {
                                myerh eee= myerhs.First(y => y.sx == (i + 1) * 24);
                                myerhstr = eee.erhMin + "-" + eee.erhMax + "%";
                            }
                        }
                        catch
                        {
                        }

                        yBLists.Add(new YBList()
                        {
                            sx = i,
                            TQ = szls[3 + i * 5],
                            TEM = szls[1 + i * 5] + "～" + szls[2 + i * 5] ,
                           FX = szls[4 + i * 5],
                           FS = szls[5 + i * 5],
                           ERH= myerhstr
                        });
                    }
                    break;

                }
            }
           


            return yBLists;
        }

        public class YBList
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public int sx { get; set; }
            public string TQ { get; set; }
            public string TEM { get; set; }
            public string FS { get; set; }
            public string FX { get; set; }
            public string ERH { get; set; }
        }

        public class myerh
        {
            public int sx { get; set; }
            public double erhMax { get; set; }
            public double erhMin { get; set; }
        }
    }
}
