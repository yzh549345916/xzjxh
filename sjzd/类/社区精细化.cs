using Aspose.Words;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace sjzd
{
    class 社区精细化
    {

        //输入数组每行内容为：旗县名称+区站号+未来三天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*3。方法将数组中的指定要素保存至发布单
        public void DCWord(string ybName, string qfName)
        {
            List<YBList> dataList = CLSJ();
            if (dataList.Count > 0)
            {
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = Environment.CurrentDirectory + @"\模版\社区街道精细化预报模板.docx";
                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "社区街道精细化预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "月\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy年MM月dd日") + "社区街道精细化预报.docx";
                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    for (int i = 0; i < 7; i++)
                    {
                        try
                        {
                            string bq = "预报" + i * 3;
                            builder.MoveToBookmark(bq);
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write("名称");
                            builder.InsertCell();
                            builder.Write("定时气温");
                            builder.InsertCell();
                            builder.Write("风向");
                            builder.InsertCell();
                            builder.Write("风速");
                            builder.InsertCell();
                            builder.Write("相对湿度");
                            builder.InsertCell();
                            builder.Write("降水量");
                            builder.EndRow();
                            List<YBList> listLS = dataList.FindAll(y => y.SX == 3 * i);
                            for (int j = 0; j < listLS.Count; j++)
                            {
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].Name);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].TEM.ToString());
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].FX);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].FS);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].ERH.ToString());
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].PRE.ToString());
                                builder.EndRow();
                            }
                            builder.EndTable();
                        }
                        catch (Exception)
                        {
                        }
                    }


                    doc.Save(SJsaPath);
                    MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + SJsaPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        try
                        {
                            System.Diagnostics.Process.Start(SJsaPath);
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
                MessageBox.Show("国家级智能网格数据获取失败，无法制作产品");
            }


        }
        public string DCWordNew(string ybName, string qfName, ref string error)
        {
            List<YBList> dataList = CLSJ();
            if (dataList.Count > 0)
            {
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = Environment.CurrentDirectory + @"\模版\社区街道精细化预报模板.docx";
                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "社区街道精细化预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "月\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy年MM月dd日") + "社区街道精细化预报.docx";
                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    for (int i = 0; i < 7; i++)
                    {
                        try
                        {
                            string bq = "预报" + i * 3;
                            builder.MoveToBookmark(bq);
                            builder.InsertCell();
                            builder.Font.Name = "宋体";
                            builder.Font.Size = 11;
                            builder.Write("名称");
                            builder.InsertCell();
                            builder.Write("定时气温");
                            builder.InsertCell();
                            builder.Write("风向");
                            builder.InsertCell();
                            builder.Write("风速");
                            builder.InsertCell();
                            builder.Write("相对湿度");
                            builder.InsertCell();
                            builder.Write("降水量");
                            builder.EndRow();
                            List<YBList> listLS = dataList.FindAll(y => y.SX == 3 * i);
                            for (int j = 0; j < listLS.Count; j++)
                            {
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].Name);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].TEM.ToString());
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].FX);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].FS);
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].ERH.ToString());
                                builder.InsertCell();
                                builder.Font.Name = "宋体";
                                builder.Font.Size = 11;
                                builder.Write(listLS[j].PRE.ToString());
                                builder.EndRow();
                            }
                            builder.EndTable();
                        }
                        catch (Exception)
                        {
                        }
                    }


                    doc.Save(SJsaPath);
                    return SJsaPath;


                }

                catch (Exception)
                {

                }
            }
            else
            {
                error = "国家级智能网格数据获取失败，无法制作产品";
            }
            return "";

        }

        public string GetFXFS(double v, double u)
        {
            string fxfs = "";
            double fx = 999.9; //风向

            if (u > 0 & v > 0)
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u < 0 & v > 0)
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u < 0 & v < 0)
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u > 0 & v < 0)
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u == 0 & v > 0)
            {
                fx = 180;
            }
            else if (u == 0 & v < 0)
            {
                fx = 0;
            }
            else if (u > 0 & v == 0)
            {
                fx = 270;
            }
            else if (u < 0 & v == 0)
            {
                fx = 90;
            }
            else if (u == 0 & v == 0)
            {
                fx = 999.9;
            }

            //风速是uv分量的平方和

            double fs = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
            int intfx = Convert.ToInt32(Math.Round(fx / 45, 0));
            switch (intfx)
            {
                case 0:
                    fxfs = "北风";
                    break;
                case 1:
                    fxfs = "东北风";
                    break;
                case 2:
                    fxfs = "东风";
                    break;
                case 3:
                    fxfs = "东南风";
                    break;
                case 4:
                    fxfs = "南风";
                    break;
                case 5:
                    fxfs = "西南风";
                    break;
                case 6:
                    fxfs = "西风";
                    break;
                case 7:
                    fxfs = "西北风";
                    break;
                case 999017:
                    fxfs = "静风";
                    break;
                default:
                    fxfs = "北风";
                    break;
            }

            fxfs += ',';
            if (fs >= 0 && fs <= 0.2)
            {
                fxfs += "0级";
            }
            else if (fs >= 0.3 && fs <= 1.5)
            {
                fxfs += "1级";
            }
            else if (fs >= 1.6 && fs <= 3.3)
            {
                fxfs += "2级";
            }
            else if (fs >= 3.4 && fs <= 5.4)
            {
                fxfs += "3级";
            }
            else if (fs >= 5.5 && fs <= 7.9)
            {
                fxfs += "4级";
            }
            else if (fs >= 8 && fs <= 10.7)
            {
                fxfs += "5级";
            }
            else if (fs >= 10.8 && fs <= 13.8)
            {
                fxfs += "6级";
            }
            else if (fs >= 13.9 && fs <= 17.1)
            {
                fxfs += "7级";
            }
            else if (fs >= 17.2 && fs <= 20.7)
            {
                fxfs += "8级";
            }
            else if (fs >= 20.8 && fs <= 24.4)
            {
                fxfs += "9级";
            }
            else if (fs >= 24.5 && fs <= 28.4)
            {
                fxfs += "10级";
            }
            else if (fs >= 28.5 && fs <= 32.6)
            {
                fxfs += "11级";
            }
            else if (fs >= 32.7 && fs <= 36.9)
            {
                fxfs += "12级";
            }
            else if (fs >= 37 && fs <= 41.4)
            {
                fxfs += "13级";
            }
            else if (fs >= 41.5 && fs <= 46.1)
            {
                fxfs += "14级";
            }
            else if (fs >= 46.2 && fs <= 50.9)
            {
                fxfs += "15级";
            }
            else if (fs >= 51 && fs <= 56)
            {
                fxfs += "16级";
            }
            else if (fs >= 56.1 && fs <= 61.2)
            {
                fxfs += "17级";
            }
            else if (fs >= 61.3)
            {
                fxfs += "17级以上";
            }
            else
            {
                fxfs += "3级";
            }
            return fxfs;
        }


        public List<YBList> CLSJ()
        {
            List<YBList> list = new List<YBList>();
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
                string con = util.Read("OtherConfig", "xzjxhDB");
                string strID = "";
                List<IDName> iDNames = new List<IDName>();
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = "select * from 社区精细化预报站点 where Station_levl='91'";
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        try
                        {
                            string idLS = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID"));
                            if (!strID.Contains(idLS))
                            {
                                strID += '\'' + idLS + '\'' + ',';
                            }

                            iDNames.Add(new IDName()
                            {
                                ID = sqlreader.GetString(sqlreader.GetOrdinal("StatioID")),
                                Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                                GJID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),


                            });
                        }
                        catch
                        {
                        }
                    }
                }

                if (strID.Length > 2)
                {
                    strID = strID.Substring(0, strID.Length - 1);
                }

                con = util.Read("OtherConfig", "DB");
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open();//打开
                    string sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=20 and sx in (12,15,18,21,24,27,30,33) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        while (sqlreader.Read())
                        {

                            try
                            {
                                string fxfs = GetFXFS(Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2),
                                    Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2));
                                List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                for (int j = 0; j < ll.Count; j++)
                                {
                                    list.Add(new YBList()
                                    {
                                        Name = ll[j].Name,
                                        ID = ll[j].ID,
                                        TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                        ERH = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                        PRE = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2),
                                        SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 12),
                                        FX = fxfs.Split(',')[0],
                                        FS = fxfs.Split(',')[1],
                                    });
                                }


                            }
                            catch (Exception)
                            {
                                list.Clear();
                                sqlreader.Close();
                                sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=8 and sx in (24,27,30,33,36,39,42,45) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                sqlman = new SqlCommand(sql, mycon);
                                sqlreader = sqlman.ExecuteReader();
                                if (sqlreader.HasRows)
                                {
                                    while (sqlreader.Read())
                                    {

                                        try
                                        {
                                            string fxfs = GetFXFS(Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2),
                                                Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2));
                                            List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                            for (int j = 0; j < ll.Count; j++)
                                            {
                                                list.Add(new YBList()
                                                {
                                                    Name = ll[j].Name,
                                                    ID = ll[j].ID,
                                                    TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                                    ERH = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                                    PRE = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2),
                                                    SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                                    FX = fxfs.Split(',')[0],
                                                    FS = fxfs.Split(',')[1],
                                                });
                                            }

                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }

                                return list;
                            }
                        }
                    }
                    else
                    {
                        sqlreader.Close();
                        sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=8 and sx in (24,27,30,33,36,39,42,45) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                        sqlman = new SqlCommand(sql, mycon);
                        sqlreader = sqlman.ExecuteReader();
                        if (sqlreader.HasRows)
                        {
                            while (sqlreader.Read())
                            {

                                try
                                {
                                    string fxfs = GetFXFS(Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2),
                                        Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2));
                                    List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                    for (int j = 0; j < ll.Count; j++)
                                    {
                                        list.Add(new YBList()
                                        {
                                            Name = ll[j].Name,
                                            ID = ll[j].ID,
                                            TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                            ERH = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                            PRE = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2),
                                            SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                            FX = fxfs.Split(',')[0],
                                            FS = fxfs.Split(',')[1],
                                        });
                                    }

                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (list.Count > 1)
                list = list.OrderBy(y => y.ID).ToList();
            return list;
        }
        public class YBList
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public double TEM { get; set; }
            public string FX { get; set; }
            public string FS { get; set; }
            public double ERH { get; set; }
            public double PRE { get; set; }
            public Int16 SX { get; set; }

        }

        public class IDName
        {
            public string ID { get; set; }
            public string GJID { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// 根据书签位置,插入图片
        /// </summary>
        /// <param name="doc">文档</param>
        /// <param name="bookMarkName">书签</param>
        /// <param name="imgName">图片名称(包含图片地址)</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        private static void insertImg(Document doc, string bookMarkName, string imgName, double width, double height)
        {
            if (doc.Range.Bookmarks[bookMarkName] != null)
            {
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.MoveToBookmark(bookMarkName);
                builder.InsertImage(imgName, width, height);
            }
        }
    }
}
