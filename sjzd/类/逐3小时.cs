using System;
using System.Linq;
using System.Text;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Fields;
using System.IO;
using System.Windows;
using System.Data;
using System.Drawing;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace sjzd
{
    class 逐3小时
    {

        //输入数组每行内容为：旗县名称+区站号+未来三天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*3。方法将数组中的指定要素保存至发布单

        public void DCWord(Int16 sc,string ybName,string qfName)
        {
            List<YBList> dataList = CLSJ(sc);
            if (dataList.Count>0)
            {
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = "";
                    
                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "呼和浩特3小时精细化预报发布路径")
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
                    if (sc == 8)
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\呼和浩特3小时精细化预报模板08.doc";
                        SJsaPath += "呼和浩特3小时精细化预报10时"+DateTime.Now.ToString("yyyyMMdd")+".doc";
                    }
                    else
                    {
                        SJMBPath = Environment.CurrentDirectory + @"\模版\呼和浩特3小时精细化预报模板20.doc";
                        SJsaPath += "呼和浩特3小时精细化预报17时" + DateTime.Now.ToString("yyyyMMdd") + ".doc";
                    }
                    
                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.CellFormat.FitText = true;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("table"); //开始添加值
                    Aspose.Words.Tables.Table table = builder.StartTable();
                    builder.RowFormat.HeadingFormat = true;
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐
                    for (Int16 i = 81; i <= 85; i++)
                    {
                        builder.InsertCell();// 添加一个单元格
                      
                        builder.Font.Color = System.Drawing.Color.Red;
                        builder.Font.Size = 14;
                        builder.CellFormat.Orientation = TextOrientation.Horizontal;
                        builder.CellFormat.Width = 30;
                        builder.CellFormat.WrapText = true;
                      
                        builder.CellFormat.VerticalMerge = Aspose.Words.Tables.CellMerge.First;
                        
                        switch (i)
                        {
                            case 81:
                                builder.Write("呼和浩特市四区");
                                break;
                            case 82:
                                builder.Write("旗县");
                                break;
                            case 83:
                                builder.Write("旅游景区");
                                break;
                            case 84:
                                builder.Write("工业园区");
                                break;
                            default:
                                builder.Write("农业园区");
                                break;
                        }

                        builder.Font.Color = System.Drawing.Color.Black;
                       
                        builder.Font.Size = 12;
                        List<YBList> listLS1 = dataList.FindAll(y => y.LB ==  i).OrderBy(y => y.ID).ThenBy(y => y.SX).ToList();
                        string IDStr = "";
                        double temLast = 0;
                        for (int j = 0; j < listLS1.Count; j++)
                        {
                            
                           
                            
                            if (IDStr != listLS1[j].ID)
                            {
                                if (j > 0)
                                {
                                    builder.EndRow();
                                    builder.InsertCell();// 添加一个单元格
                                    builder.CellFormat.Width = 30;
                                    builder.CellFormat.VerticalMerge = CellMerge.Previous;

                                }

                                temLast = listLS1[j].TEM;
                                IDStr = listLS1[j].ID;
                               
                                builder.InsertCell();// 添加一个单元格
                                builder.CellFormat.Width =55;
                                builder.CellFormat.Orientation = TextOrientation.Horizontal;
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.Write(listLS1[j].Name);
                            }
                            else
                            {
                                builder.InsertCell();// 添加一个单元格
                                builder.CellFormat.Width = 55;
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.Write(listLS1[j].TQ+"\r\n");
                                string imageName = Environment.CurrentDirectory + @"\模版\天气图标\";
                                if (sc == 8)
                                {
                                    imageName += @"白天\";
                                }
                                else
                                    imageName += @"夜晚\";

                                if (listLS1[j].TQ.Contains("雨"))
                                {
                                    if (listLS1[j].TQ == "小雨"|| listLS1[j].TQ == "冻雨")
                                    {
                                        imageName += "小雨.png";
                                    }
                                    else if (listLS1[j].TQ == "雨夹雪")
                                    {
                                        imageName += "雨夹雪.png";
                                    }
                                    else
                                    {
                                        imageName += "大雨.png";
                                    }
                                }
                                else if (listLS1[j].TQ.Contains("雪"))
                                {
                                    if (listLS1[j].TQ == "小雪" )
                                    {
                                        imageName += "小雪.png";
                                    }
                                    else if (listLS1[j].TQ == "雨夹雪")
                                    {
                                        imageName += "雨夹雪.png";
                                    }
                                    else
                                    {
                                        imageName += "大雪.png";
                                    }
                                }
                                else if(listLS1[j].TQ=="晴")
                                {
                                    imageName += "晴.png";
                                }
                                else if (listLS1[j].TQ == "多云")
                                {
                                    imageName += "多云.png";
                                }
                                else
                                {
                                    imageName += "阴.png";
                                }
                                builder.InsertImage(imageName, 30, 30);
                                builder.InsertCell();// 添加一个单元格
                              
                                builder.CellFormat.Width = 55;
                              
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.Write(temLast.ToString());
                                builder.InsertCell();// 添加一个单元格
                               
                                builder.CellFormat.Width = 55;
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.Write(listLS1[j].TEM.ToString());
                                temLast = listLS1[j].TEM;

                            }

                        }
                        builder.EndRow();
                    }
                    builder.EndTable();
                  



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

                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("国家级智能网格数据获取失败，无法制作产品");
            }


        }

        public string GetTQ(double ect, double pre,int pph)
        {
            string tq = "";
            if (pre > 0)
            {
                if (pph == 3)
                {
                    if (pre < 1)
                        tq= "小雪";
                    else if (pre >= 1 && pre < 3)
                        tq = "中雪";
                    else if (pre >= 3 && pre <6)
                        tq = "大雪";
                    else if (pre >= 6 && pre < 12)
                        tq = "暴雪";
                    else if (pre >= 12 && pre < 24)
                        tq = "大暴雪";
                    else if (pre >= 24)
                        tq = "特大暴雪";
                }
                else if (pph == 2)
                {
                    tq = "雨夹雪";
                }
                else if (pph == 4)
                {
                    tq = "冻雨";
                }
                else
                {
                    if (pre < 5)
                        tq = "小雨";
                    else if(pre>=5&&pre<15)
                        tq = "中雨";
                    else if (pre >= 15 && pre < 30)
                        tq = "大雨";
                    else if (pre >= 30 && pre < 70)
                        tq = "暴雨";
                    else if (pre >= 70 && pre < 140)
                        tq = "大暴雨";
                    else if (pre >= 140 )
                        tq = "特大暴雨";
                }
            }
            else
            {
                if (ect <= 20)
                    tq = "晴";
                else if (ect >= 90)
                    tq = "阴";
                else
                    tq = "多云";
            }
            return tq;
        }


        public List<YBList> CLSJ(int sc)
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
                    string sql = String.Format("select * from 社区精细化预报站点 where Station_levl in (81,82,83,84,85)");
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
                                LB = sqlreader.GetInt16(sqlreader.GetOrdinal("Station_levl")),
                                GJID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            });
                        }
                        catch(Exception ex)
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
                    //预报取上一时次的预报

                    mycon.Open();//打开
                    string sql = "";
                    if (sc == 8)
                    {
                        sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=20 and sx in (12,15,18,21,24) and date='{1}'", strID,DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=8 and sx in (12,15,18,21,24) and date='{1}'", strID, DateTime.Now.ToString("yyyy-MM-dd"));
                    }
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        double pre = 0, ect = 0;
                        int pph = 0;
                        while (sqlreader.Read())
                        {
                            

                            try
                            {
                                pre = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                ect= Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                pph= Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                //IDName ls=iDNames.Find((IDName y) =>(y.ID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID"))));
                                List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                for (int j = 0; j < ll.Count; j++)
                                {
                                    list.Add(new YBList()
                                    {
                                        Name = ll[j].Name,
                                        ID = ll[j].ID,
                                        TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                        TQ = GetTQ(ect, pre, pph),
                                        SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 12),
                                        LB = ll[j].LB,
                                    });
                                }

                            }
                            catch (Exception ex1)
                            {
                                list.Clear();
                                //预报取上一时次的预报

                                sqlreader.Close();
                                if (sc == 8)
                                {
                                    sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=8 and sx in (24,27,30,33,36) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                }
                                else
                                {
                                    sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=20 and sx in (24,27,30,33,36) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                                }
                                sqlman = new SqlCommand(sql, mycon);
                                sqlreader = sqlman.ExecuteReader();
                                if (sqlreader.HasRows)
                                {
                                    while (sqlreader.Read())
                                    {

                                        try
                                        {
                                            pre = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                            ect = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                            pph = Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                            List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                            for (int j = 0; j < ll.Count; j++)
                                            {
                                                list.Add(new YBList()
                                                {
                                                    Name = ll[j].Name,
                                                    ID = ll[j].ID,
                                                    TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                                    TQ = GetTQ(ect, pre, pph),
                                                    SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                                    LB = ll[j].LB,
                                                });
                                            }

                                        }
                                        catch (Exception ex)
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
                        double pre = 0, ect = 0;
                        int pph = 0;
                        if (sc == 8)
                        {
                            sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=8 and sx in (24,27,30,33,36) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                        }
                        else
                        {
                            sql = String.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ({0}) and sc=20 and sx in (24,27,30,33,36) and date='{1}'", strID, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
                        }
                        sqlman = new SqlCommand(sql, mycon);
                        sqlreader = sqlman.ExecuteReader();
                        if (sqlreader.HasRows)
                        {
                            while (sqlreader.Read())
                            {

                                try
                                {
                                    pre = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                    ect = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                    pph = Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                    List<IDName> ll = iDNames.FindAll((IDName y) => (y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID")))).ToList();
                                    for (int j = 0; j < ll.Count; j++)
                                    {
                                        list.Add(new YBList()
                                        {
                                            Name = ll[j].Name,
                                            ID = ll[j].ID,
                                            TEM = Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                            TQ = GetTQ(ect, pre, pph),
                                            SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                            LB = ll[j].LB,
                                        });
                                    }

                                }
                                catch (Exception ex)
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
            public string Name { get; set; }
            public string ID { get; set; }
            public double TEM { get; set; }
            public string TQ { get; set; }
            public Int16 LB { get; set; }
            public Int16 SX { get; set; }
        }

        public class IDName
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public Int16 LB { get; set; }
            public string GJID { get; set; }
        }


    }
}
