using Aspose.Words;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace sjzd
{
    class 短时预报
    {
        public void DCWord(Int16 sc, string ybName, string qfName)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            Int16 sc2 = sc;
            if (sc2 == 14)
                sc2 = 8;
            string ybData = bw2.Sjyb(strDate, sc2.ToString().PadLeft(2, '0'));
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
                            if (line.Split('=')[0] == "呼和浩特短时预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + sc.ToString().PadLeft(2, '0') + "\\" + DateTime.Now.ToString("M月") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJMBPath = Environment.CurrentDirectory + @"\模版\短时预报模板.doc";
                    SJsaPath += "短时预报" + sc.ToString().PadLeft(2, '0') + "（" + DateTime.Now.ToString("yyMMdd") + "）.doc";

                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日") + sc.ToString().PadLeft(2, '0'));
                    builder.Font.Size = 12;
                    builder.Font.Name = "宋体";
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("天气"); //开始添加值
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[3].Contains("转"))
                            {
                                builder.Write(Regex.Split(szls[3], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else
                            {
                                builder.Write(szls[3]);
                            }

                            break;

                        }
                    }
                    builder.MoveToBookmark("风"); //开始添加值
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[4].Contains("转") && !szls[5].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                builder.Write(Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] + szls[5]);
                            }
                            else if (szls[5].Contains("转") && !szls[4].Contains("转"))
                            {
                                builder.Write(szls[4] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else if (szls[5].Contains("转") && szls[4].Contains("转"))
                            {
                                builder.Write(Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] +
                                              Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else
                            {
                                builder.Write(szls[4] + szls[5]);
                            }


                            break;

                        }
                    }

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

        public string DCWordNew(Int16 sc, string ybName, string qfName, ref string error)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            Int16 sc2 = sc;
            if (sc2 == 14)
                sc2 = 8;
            string ybData = bw2.Sjyb(strDate, sc2.ToString().PadLeft(2, '0'));
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
                            if (line.Split('=')[0] == "呼和浩特短时预报发布路径")
                            {
                                SJsaPath = line.Split('=')[1];
                            }
                        }
                    }
                    SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + sc.ToString().PadLeft(2, '0') + "\\" + DateTime.Now.ToString("M月") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJMBPath = Environment.CurrentDirectory + @"\模版\短时预报模板.doc";
                    SJsaPath += "短时预报" + sc.ToString().PadLeft(2, '0') + "（" + DateTime.Now.ToString("yyMMdd") + "）.doc";

                    Document doc = new Document(SJMBPath);
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.MoveToBookmark("标题日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Write(DateTime.Now.ToString("yyyy年MM月dd日") + sc.ToString().PadLeft(2, '0'));
                    builder.Font.Size = 12;
                    builder.Font.Name = "宋体";
                    builder.MoveToBookmark("预报员");
                    builder.Write(ybName);
                    builder.MoveToBookmark("签发");
                    builder.Write(qfName);
                    builder.MoveToBookmark("天气"); //开始添加值
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[3].Contains("转"))
                            {
                                builder.Write(Regex.Split(szls[3], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else
                            {
                                builder.Write(szls[3]);
                            }

                            break;

                        }
                    }
                    builder.MoveToBookmark("风"); //开始添加值
                    foreach (string sls in ybData.Split('\n'))
                    {
                        string[] szls = sls.Split(',');
                        if (szls[0].Trim() == "53463")
                        {
                            if (szls[4].Contains("转") && !szls[5].Contains("转"))//如果风向风速都含“转”，则合并
                            {
                                builder.Write(Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] + szls[5]);
                            }
                            else if (szls[5].Contains("转") && !szls[4].Contains("转"))
                            {
                                builder.Write(szls[4] + Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else if (szls[5].Contains("转") && szls[4].Contains("转"))
                            {
                                builder.Write(Regex.Split(szls[4], "转", RegexOptions.IgnoreCase)[0] +
                                              Regex.Split(szls[5], "转", RegexOptions.IgnoreCase)[0]);
                            }
                            else
                            {
                                builder.Write(szls[4] + szls[5]);
                            }


                            break;

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
                error = "城镇预报数据获取失败，无法制作产品";
            }

            return "";
        }
    }
}
