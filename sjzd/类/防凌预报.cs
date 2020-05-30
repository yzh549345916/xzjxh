using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Tables;

namespace sjzd
{
    internal class 防凌预报
    {
        public string DCWordNew(string zbStr,string qfStr,string shStr,string lxStr,ref string error)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            short sc2 = 20;
            string ybData = bw2.Sjyb(strDate, sc2.ToString().PadLeft(2, '0'));
            List<YBList> yBListsall = CLYBAll(ybData).Where(y => y.ID == "53467" || y.ID == "53562").OrderBy(y => y.序号).ThenBy(y => y.ID).ThenBy(y => y.sc).ToList();
            if (yBListsall.Count == 0)
            {
                error = "城镇预报数据获取失败，无法制作产品";
                return "";
            }

            string configpathPath = Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            try
            {
                string SJsaPath = "";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "呼和浩特防凌预报发布路径")
                        {
                            SJsaPath = line.Split('=')[1];
                        }
                    }
                }

                SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + lxStr + "\\" ;
                string direPath = SJsaPath;
                if (!File.Exists(SJsaPath))
                {
                    Directory.CreateDirectory(SJsaPath);
                }

                SJsaPath += $"黄河呼市段{lxStr}气象服务专报" + DateTime.Now.ToString("yyyy年MM月dd日") + ".docx";

                Document doc = new Document();
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.CellFormat.Borders.Color = Color.Black;
                builder.Font.Size = 43;
                builder.Font.Color = Color.Red;
                builder.Font.Bold = false;
                builder.Font.Name = "华文行楷";
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Write("黄河凌汛专项气象服务");
                builder.Font.Color = Color.Black;
                builder.Font.Bold = false;
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center; //垂直居中对齐
                builder.Font.Size = 16;
                builder.Font.Name = "黑体";
                builder.Font.Bold = false;
                builder.InsertParagraph();
                builder.Write($"（{DateTime.Now:yyyy年}第XX期）\r\n");
                builder.InsertParagraph();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                builder.Write("呼和浩特市气象局");
                builder.InsertParagraph();
                builder.Write(DateTime.Now.ToString("yyyy年MM月dd日") + "                       " + $"签发：{qfStr}");
                builder.InsertParagraph();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                Shape lineShape = new Shape(doc, ShapeType.Line);
                lineShape.Width = 420;
                Stroke stroke = lineShape.Stroke;
                stroke.On = true;
                stroke.Weight = 2.5;
                stroke.Color = Color.Red;
                stroke.LineStyle = ShapeLineStyle.Single;
                builder.InsertNode(lineShape);
                builder.Font.Size = 18;
                builder.Font.Bold = true;
                builder.Font.Name = "黑体 ";
                builder.InsertParagraph();
                builder.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                builder.ParagraphFormat.LineSpacing = 24.0; //12为单倍行距，18为1.5倍
                builder.Write("黄河呼和浩特段开河监测及未来天气预报");
                builder.InsertParagraph();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                builder.ParagraphFormat.LineSpacing = 18; //12为单倍行距，18为1.5倍
                builder.Font.Size = 16;
                builder.Write("一、黄河呼和浩特段开河实况监测");
                builder.InsertParagraph();
                builder.Font.Name = "仿宋_GB2312";
                builder.Font.Size = 15;
                builder.Font.Bold = false;
                builder.ParagraphFormat.FirstLineIndent = 30;
                builder.ParagraphFormat.LineSpacing = 18;
                builder.Write(" ");
                builder.InsertParagraph();
                builder.Write("高分四号卫星黄河凌汛遥感监测图如下：");
                builder.InsertParagraph();
                builder.ParagraphFormat.FirstLineIndent = 0;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.InsertParagraph();
                builder.InsertBreak(BreakType.PageBreak);
                builder.ParagraphFormat.LineSpacing = 18.0;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                builder.Font.Size = 16;
                builder.Font.Bold = true;
                builder.Font.Name = "黑体";
                builder.Write("二、黄河呼和浩特段各站气温实况分析");
                builder.InsertParagraph();
                builder.Font.Name = "仿宋_GB2312";
                builder.Font.Bold = true;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Font.Size = 12;
                builder.ParagraphFormat.LineSpacing = 18;
                builder.Write("表1  过去24小时黄河呼和浩特段气温实况(08时—08时)（单位：℃）");
                builder.InsertParagraph();
                try
                {
                    CIMISS cIMISS = new CIMISS();
                    List<CIMISS.YS> myTem = cIMISS.获取小时温度(DateTime.Now.Date.AddDays(-1).AddHours(9), DateTime.Now.Date.AddDays(0).AddHours(8), "53467,53562");
                    List<SKList> sKLists = new List<SKList>();
                    float tMin = myTem.Where(y => y.StationID == "53467").OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                    float tMax = myTem.Where(y => y.StationID == "53467").OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
                    sKLists.Add(new SKList
                    {
                        ID = "53467",
                        Tmin = Math.Round(tMin, 1),
                        Tmax = Math.Round(tMax, 1),
                        sc = -24
                    });
                    tMin = myTem.Where(y => y.StationID == "53562").OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                    tMax = myTem.Where(y => y.StationID == "53562").OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
                    sKLists.Add(new SKList
                    {
                        ID = "53562",
                        Tmin = Math.Round(tMin, 1),
                        Tmax = Math.Round(tMax, 1),
                        sc = -24
                    });
                    myTem = cIMISS.获取小时温度(DateTime.Now.Date.AddDays(-2).AddHours(9), DateTime.Now.Date.AddDays(-1).AddHours(8), "53467,53562");
                    tMin = myTem.Where(y => y.StationID == "53467").OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                    tMax = myTem.Where(y => y.StationID == "53467").OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
                    sKLists.Add(new SKList
                    {
                        ID = "53467",
                        Tmin = Math.Round(tMin, 1),
                        Tmax = Math.Round(tMax, 1),
                        sc = -48
                    });
                    tMin = myTem.Where(y => y.StationID == "53562").OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                    tMax = myTem.Where(y => y.StationID == "53562").OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
                    sKLists.Add(new SKList
                    {
                        ID = "53562",
                        Tmin = Math.Round(tMin, 1),
                        Tmax = Math.Round(tMax, 1),
                        sc = -48
                    });

                    Table table = builder.StartTable();


                    builder.ParagraphFormat.LineSpacing = 12.0;
                    builder.RowFormat.HeadingFormat = true;
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center; //垂直居中对齐
                    builder.InsertCell();
                    builder.Font.Size = 12;
                    builder.Font.Bold = true;
                    builder.CellFormat.Orientation = TextOrientation.Horizontal;
                    builder.CellFormat.Width = 100;
                    builder.InsertCell();
                    builder.CellFormat.Width = 80;
                    builder.Write("最高气温");
                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    builder.Write("与前一天比较");
                    builder.InsertCell();
                    builder.CellFormat.Width = 80;
                    builder.Write("最低气温");
                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    builder.Write("与前一天比较");
                    builder.EndRow();

                    //builder.Write("测站");
                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    builder.Write("托县");

                    builder.InsertCell();
                    builder.Font.Bold = false;
                    builder.CellFormat.Width = 80;
                    if (sKLists.Exists(y => y.ID == "53467" && y.sc == -24))
                    {
                        builder.Write(sKLists.First(y => y.ID == "53467" && y.sc == -24).Tmax.ToString());
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    if (sKLists.Exists(y => y.ID == "53467" && y.sc == -24) && sKLists.Exists(y => y.ID == "53467" && y.sc == -48))
                    {
                        double dls1 = sKLists.First(y => y.ID == "53467" && y.sc == -24).Tmax;
                        double dls2 = sKLists.First(y => y.ID == "53467" && y.sc == -48).Tmax;
                        string strLS = "相等";
                        dls1 = dls1 - dls2;
                        dls2 = Math.Round(Math.Abs(dls1),2);
                        if (dls1 > 0)
                            strLS = $"升{dls2}";
                        else if (dls1 < 0)
                            strLS = $"降{dls2}";
                        builder.Write(strLS);
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.InsertCell();
                    builder.CellFormat.Width = 80;
                    if (sKLists.Exists(y => y.ID == "53467" && y.sc == -24))
                    {
                        builder.Write(sKLists.First(y => y.ID == "53467" && y.sc == -24).Tmin.ToString());
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    if (sKLists.Exists(y => y.ID == "53467" && y.sc == -24) && sKLists.Exists(y => y.ID == "53467" && y.sc == -48))
                    {
                        double dls1 = sKLists.First(y => y.ID == "53467" && y.sc == -24).Tmin;
                        double dls2 = sKLists.First(y => y.ID == "53467" && y.sc == -48).Tmin;
                        string strLS = "相等";
                        dls1 = dls1 - dls2;
                        dls2 = Math.Round(Math.Abs(dls1), 2);
                        if (dls1 > 0)
                            strLS = $"升{dls2}";
                        else if (dls1 < 0)
                            strLS = $"降{dls2}";
                        builder.Write(strLS);
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.EndRow();
                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    builder.Font.Bold = true;
                    builder.Write("清水河");
                    builder.Font.Bold = false;


                    builder.InsertCell();
                    builder.CellFormat.Width = 80;
                    if (sKLists.Exists(y => y.ID == "53562" && y.sc == -24))
                    {
                        builder.Write(sKLists.First(y => y.ID == "53562" && y.sc == -24).Tmax.ToString());
                    }
                    else
                    {
                        builder.Write("");
                    }


                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    if (sKLists.Exists(y => y.ID == "53562" && y.sc == -24) && sKLists.Exists(y => y.ID == "53562" && y.sc == -48))
                    {
                        double dls1 = sKLists.First(y => y.ID == "53562" && y.sc == -24).Tmax;
                        double dls2 = sKLists.First(y => y.ID == "53562" && y.sc == -48).Tmax;
                        string strLS = "相等";
                        dls1 = dls1 - dls2;
                        dls2 = Math.Round(Math.Abs(dls1), 2);
                        if (dls1 > 0)
                            strLS = $"升{dls2}";
                        else if (dls1 < 0)
                            strLS = $"降{dls2}";
                        builder.Write(strLS);
                    }
                    else
                    {
                        builder.Write("");
                    }


                    builder.InsertCell();
                    builder.CellFormat.Width = 80;
                    if (sKLists.Exists(y => y.ID == "53562" && y.sc == -24))
                    {
                        builder.Write(sKLists.First(y => y.ID == "53562" && y.sc == -24).Tmin.ToString());
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    if (sKLists.Exists(y => y.ID == "53562" && y.sc == -24) && sKLists.Exists(y => y.ID == "53562" && y.sc == -48))
                    {
                        double dls1 = sKLists.First(y => y.ID == "53562" && y.sc == -24).Tmin;
                        double dls2 = sKLists.First(y => y.ID == "53562" && y.sc == -48).Tmin;
                        string strLS = "相等";
                        dls1 = dls1 - dls2;
                        dls2 = Math.Round(Math.Abs(dls1), 2);
                        if (dls1 > 0)
                            strLS = $"升{dls2}";
                        else if (dls1 < 0)
                            strLS = $"降{dls2}";
                        builder.Write(strLS);
                    }
                    else
                    {
                        builder.Write("");
                    }

                    builder.EndRow();

                    #region 表头加斜线  务必放到表格操作最后进行

                    Run run = new Run(doc, "站点");
                    run.Font.Bold = true;
                    run.Font.Size = 12;
                    run.Font.Name = "仿宋_GB2312";
                    Run run2 = new Run(doc, "气象要素");
                    run2.Font.Bold = true;
                    run2.Font.Size = 12;
                    run2.Font.Name = "仿宋_GB2312";
                    Border diagonalBorder = table.FirstRow.FirstCell.CellFormat.Borders[BorderType.DiagonalDown];
                    diagonalBorder.Color = Color.Black;
                    diagonalBorder.LineStyle = LineStyle.Single;
                    diagonalBorder.LineWidth = 1.5;
                    Paragraph pp = new Paragraph(doc);
                    pp.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                    pp.AppendChild(run);
                    table.FirstRow.FirstCell.AppendChild(pp);
                    table.FirstRow.FirstCell.FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                    table.FirstRow.FirstCell.FirstParagraph.AppendChild(run2);

                    #endregion

                    table.AutoFit(AutoFitBehavior.FixedColumnWidths);
                    table.Alignment = TableAlignment.Center;
                    builder.EndTable();
                }
                catch
                {
                }


                builder.InsertParagraph();
                builder.ParagraphFormat.LineSpacing = 18.0;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                builder.Font.Size = 16;
                builder.Font.Bold = true;
                builder.Font.Name = "黑体";
                builder.Write("三、黄河呼和浩特段未来五天天气预报");
                builder.InsertParagraph();
                builder.ParagraphFormat.FirstLineIndent = 30;
                builder.Font.Size = 15;
                builder.Font.Bold = false;
                builder.Font.Name = "仿宋_GB2312";
                try
                {
                    string line = "", YBpath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "呼和浩特短期预报发布路径")
                            {
                                YBpath = line.Split('=')[1];
                                break;
                            }
                        }
                    }

                    if (YBpath.Length > 0)
                    {
                        DateTime date = DateTime.Now;
                        YBpath += $"{date:yyyy}\\{date:yyyy-MM}\\{date:yyyyMMdd}10.doc";
                    }

                    if (File.Exists(YBpath))
                    {
                        string myFile = Environment.CurrentDirectory + '\\' + Path.GetFileName(YBpath);
                        File.Copy(YBpath, myFile);


                        Document doc2 = new Document(myFile);
                        if (doc2.FirstSection.Body.Paragraphs.Count > 0)
                        {
                            ParagraphCollection pargraphs = doc2.FirstSection.Body.Paragraphs; //word中的所有段落

                            for (int i = 0; i < pargraphs.Count; i++)
                            {
                                string s = pargraphs[i].GetText();
                                if (s.Contains("一、24小时天气预报"))
                                {
                                    builder.Write(pargraphs[i - 1].GetText());
                                }
                            }
                        }

                        File.Delete(myFile);
                    }
                }
                catch (Exception)
                {
                }

                builder.ParagraphFormat.FirstLineIndent = 0;


                // builder.InsertParagraph();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.Font.Bold = true;
                builder.Font.Size = 12;
                builder.Write("表2 黄河呼和浩特段未来5天气象要素预报(20时—20时)");
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                //插入预报表格
                Table tableYB = builder.StartTable();


                builder.ParagraphFormat.LineSpacing = 12.0;
                builder.RowFormat.HeadingFormat = true;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center; //垂直居中对齐
                builder.InsertCell();
                builder.Font.Size = 12;
                builder.Font.Bold = true;
                builder.CellFormat.Orientation = TextOrientation.Horizontal;
                builder.CellFormat.Width = 70;
                builder.Write("日\r\n期");
                builder.InsertCell();
                builder.CellFormat.Width = 100;
                
                builder.InsertCell();
                builder.CellFormat.Width = 150;
                builder.Write("托县");
                builder.InsertCell();
                builder.CellFormat.Width = 150;
                builder.Write("清水河");
                builder.EndRow();
                for (int i = 1; i <= 5; i++)
                {
                    DateTime dateTime = DateTime.Now.AddDays(i);
                    builder.InsertCell();
                    if (i == 1)
                        tableYB.PreferredWidth = PreferredWidth.FromPercent(60);
                    builder.Font.Size = 12;
                    builder.Font.Bold = true;
                    builder.CellFormat.Orientation = TextOrientation.Horizontal;
                    builder.CellFormat.Width = 70;
                    builder.CellFormat.WrapText = true;
                    builder.CellFormat.VerticalMerge = CellMerge.First;
                    builder.Write(dateTime.ToString("MM月dd日"));//后续改为Mdd
                    builder.Font.Bold = true;
                    builder.InsertCell(); // 添加一个单元格
                    builder.CellFormat.VerticalMerge = CellMerge.None;

                    builder.CellFormat.Width = 100;
                    builder.Write("气温预报（℃）");
                    List<YBList> yblists = yBListsall.Where(y => y.sc == i * 24).ToList();
                    builder.InsertCell();// 添加一个单元格
                    builder.Font.Bold = false;
                    builder.CellFormat.Width = 150;
                    builder.Write(yblists.First(y=>y.ID=="53467").TEM);
                    builder.InsertCell();// 添加一个单元格
                    builder.Write(yblists.First(y => y.ID == "53562").TEM);
                    builder.EndRow();

                    builder.InsertCell();// 添加一个单元格
                    builder.CellFormat.Width = 70;
                    builder.CellFormat.VerticalMerge = CellMerge.Previous;
                    builder.Font.Bold = true;
                    builder.InsertCell(); // 添加一个单元格
                    builder.CellFormat.Width = 100;
                    builder.Write("风向风速");
                   builder.InsertCell();// 添加一个单元格
                    builder.CellFormat.VerticalMerge = CellMerge.None;
                    builder.Font.Bold = false;
                    builder.CellFormat.Width = 150;
                    builder.Write(yblists.First(y => y.ID == "53467").FXFS);
                    builder.InsertCell();// 添加一个单元格
                    builder.Write(yblists.First(y => y.ID == "53562").FXFS);
                    builder.EndRow();
                }
                Run run3 = new Run(doc, "气象要素");
                run3.Font.Bold = true;
                run3.Font.Size = 12;
                run3.Font.Name = "仿宋_GB2312";
                Run run4 = new Run(doc, "站点");
                run4.Font.Bold = true;
                run4.Font.Size = 12;
                run4.Font.Name = "仿宋_GB2312";
                Border diagonalBorder2 = tableYB.FirstRow.Cells[1].CellFormat.Borders[BorderType.DiagonalDown];
                diagonalBorder2.Color = Color.Black;
                diagonalBorder2.LineStyle = LineStyle.Single;
                diagonalBorder2.LineWidth = 1.5;
                Paragraph pp2 = new Paragraph(doc);
                pp2.ParagraphFormat.Alignment = ParagraphAlignment.Left;
                pp2.AppendChild(run3);
                tableYB.FirstRow.Cells[1].AppendChild(pp2);
                tableYB.FirstRow.Cells[1].FirstParagraph.ParagraphFormat.Alignment = ParagraphAlignment.Right;
                tableYB.FirstRow.Cells[1].FirstParagraph.AppendChild(run4);
                tableYB.AutoFit(AutoFitBehavior.FixedColumnWidths);
                tableYB.Alignment = TableAlignment.Center;
                builder.EndTable();
                //builder.InsertBreak(BreakType.PageBreak);
                //builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                //builder.Font.Bold = true;
                //builder.Font.Size = 12;
                //builder.Write("图1 黄河呼和浩特段未来5天气温变化趋势");
                //builder.InsertParagraph();
                //builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                //Shape shape53467 = builder.InsertChart(ChartType.Line, 400, 220);

                //builder.InsertParagraph();
                //// builder.MoveToBookmark("清水河折线图"); //开始添加值 builder.MoveToBookmark("清水河折线图");
                //Shape shape53562 = builder.InsertChart(ChartType.Line, 400, 220);
                //Chart chart53467 = shape53467.Chart;
                //Chart chart53562 = shape53562.Chart;
                //chart53467.Series.Clear();
                //chart53562.Series.Clear();
                //chart53467.AxisX.Crosses = AxisCrosses.Minimum;
                //chart53562.AxisX.Crosses = AxisCrosses.Minimum;
                //chart53467.Title.Text = "托县未来5天气温变化趋势";
                //chart53562.Title.Text = "清水河未来5天气温变化趋势";
                //string[] timsSZ = new string[5];
                //double[] tmaxSZ = new double[5];
                //double[] tminSZ = new double[5];
                //for (int i=0;i<5;i++)
                //{
                //    timsSZ[i] = DateTime.Now.AddDays(i).ToString("yyyy-MM-dd");
                //    double tminLS = -999, tmaxLS = -999;
                //    if(yBListsall.Exists(y=>y.ID== "53467"&&y.sc==(i+1)*24))
                //    {
                //        YBList itemLS = yBListsall.First(y => y.ID == "53467" && y.sc == (i + 1) * 24);
                //        tminLS = itemLS.Tmin;
                //        tmaxLS = itemLS.Tmax;
                //    }
                //    tmaxSZ[i] = tmaxLS;
                //    tminSZ[i] = tminLS;
                //}
                //ChartSeries series1 = chart53467.Series.Add("最低气温", timsSZ, tminSZ);
                //series1.Smooth = true;

                //for (int j = 0; j < 5; j++)
                //{
                //    ChartDataLabel label = series1.DataLabels.Add(j);
                //    label.ShowValue = true;
                //   label.NumberFormat.FormatCode = "#,##0.0\"℃\"";
                //    ChartDataPoint point = series1.DataPoints.Add(j);
                //    point.Marker.Symbol = MarkerSymbol.Circle;
                //    point.Marker.Size = 5;
                //}
                //ChartSeries series2 = chart53467.Series.Add("最高气温", timsSZ, tmaxSZ);
                //series2.Smooth = true;
                //for (int j = 0; j < 5; j++)
                //{
                //    ChartDataLabel label = series2.DataLabels.Add(j);
                //    label.ShowValue = true;
                //  label.NumberFormat.FormatCode = "#,##0.0\"℃\"";
                //    ChartDataPoint point = series2.DataPoints.Add(j);
                //    point.Marker.Symbol = MarkerSymbol.Circle;
                //    point.Marker.Size = 5;
                //}

                //for (int i = 0; i < 5; i++)
                //{
                //    double tminLS = -999, tmaxLS = -999;
                //    if (yBListsall.Exists(y => y.ID == "53562" && y.sc == (i + 1) * 24))
                //    {
                //        YBList itemLS = yBListsall.First(y => y.ID == "53562" && y.sc == (i + 1) * 24);
                //        tminLS = itemLS.Tmin;
                //        tmaxLS = itemLS.Tmax;
                //    }
                //    tmaxSZ[i] = tmaxLS;
                //    tminSZ[i] = tminLS;
                //}
                //ChartSeries series3 = chart53562.Series.Add("最低气温", timsSZ, tminSZ);
                //series3.Smooth = true;

                //for (int j = 0; j < 5; j++)
                //{
                //    ChartDataLabel label = series3.DataLabels.Add(j);
                //    label.ShowValue = true;
                //   label.NumberFormat.FormatCode = "#,##0.0\"℃\"";
                //    ChartDataPoint point = series3.DataPoints.Add(j);
                //    point.Marker.Symbol = MarkerSymbol.Circle;
                //    point.Marker.Size = 5;
                //}
                //ChartSeries series4 = chart53562.Series.Add("最高气温", timsSZ, tmaxSZ);
                //series4.Smooth = true;
                //for (int j = 0; j < 5; j++)
                //{
                //    ChartDataLabel label = series4.DataLabels.Add(j);
                //    label.ShowValue = true;

                //   label.NumberFormat.FormatCode = "#,##0.0\"℃\"";
                //    ChartDataPoint point = series4.DataPoints.Add(j);
                //    point.Marker.Symbol = MarkerSymbol.Circle;
                //    point.Marker.Size = 5;
                //}
                builder.InsertParagraph();
                builder.ParagraphFormat.LineSpacing = 12.0; //12为单倍行距，18为1.5倍
                builder.InsertParagraph();
                builder.ParagraphFormat.FirstLineIndent = 0;
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                Shape lineShape2 = new Shape(doc, ShapeType.Line);
                lineShape2.Width = 420;
                Stroke stroke2 = lineShape2.Stroke;
                stroke2.On = true;
                stroke2.Weight = 1;
                stroke2.Color = Color.Black;
                stroke2.LineStyle = ShapeLineStyle.Single;
                builder.InsertNode(lineShape2);

                builder.ParagraphFormat.Alignment = ParagraphAlignment.Justify;
                builder.ParagraphFormat.LineSpacing = 15;
                builder.Font.Size = 11;
                builder.Font.Bold = false;
                builder.Write("呈报：市委、政府、内蒙古气象局领导\r\n报送：内蒙古气象局应急与减灾处、科技与预报处、决策办、气候中心、市防汛办");
                builder.InsertParagraph();
                builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                Shape lineShape3 = new Shape(doc, ShapeType.Line);
                lineShape3.Width = 420;
                Stroke stroke3 = lineShape3.Stroke;
                stroke3.On = true;
                stroke3.Weight = 1;
                stroke3.Color = Color.Black;
                stroke3.LineStyle = ShapeLineStyle.Single;
                builder.InsertNode(lineShape3);
                builder.InsertParagraph();
                builder.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
                builder.ParagraphFormat.LineSpacing = 12.0; //12为单倍行距，18为1.5倍
                builder.Font.Size = 12;
                builder.Font.Bold = true;

                builder.Write($"制作：{zbStr}                           审核：{shStr}");
                doc.Save(SJsaPath);
                try
                {
                    System.Diagnostics.Process.Start("explorer.exe", direPath);
                }
                catch { }
                return SJsaPath;
            }

            catch (Exception)
            {
            }


            return "";
        }

        public List<YBList> CLYBAll(string ybData)
        {
            List<YBList> yBLists = new List<YBList>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "xzjxhDB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open(); //打开
                string sql = "select * from 社区精细化预报站点 where Station_levl = 93 order by StatioID";
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    try
                    {
                        yBLists.Add(new YBList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            sc = 24
                        });
                        yBLists.Add(new YBList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            sc = 48
                        });
                        yBLists.Add(new YBList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            sc = 72
                        });
                        yBLists.Add(new YBList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            sc = 96
                        });
                        yBLists.Add(new YBList
                        {
                            ID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID")),
                            Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                            sc = 120
                        });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }

            for (int i = 0; i < yBLists.Count; i++)
            {
                yBLists[i].Name = 处理站点名称(yBLists[i].ID);
                foreach (string sls in ybData.Split('\n'))
                {
                    string[] szls = sls.Split(',');
                    if (szls[0].Trim() == yBLists[i].ID)
                    {
                        if (yBLists[i].sc == 24)
                        {
                            yBLists[i].TQ = szls[3];
                            yBLists[i].TEM = szls[1] + "～" + szls[2];
                            yBLists[i].Tmin = Convert.ToDouble(szls[1]);
                            yBLists[i].Tmax = Convert.ToDouble(szls[2]);
                            yBLists[i].FXFS = CLFXFS(szls[4], szls[5]);
                            yBLists[i].FX = szls[4];
                            yBLists[i].FS = szls[5];
                            yBLists[i].序号 = yBLists[i].ID == "53463" ? 0 : 99;
                            yBLists[i].Name = 处理站点名称(yBLists[i].ID);
                            break;
                        }

                        if (yBLists[i].sc == 48)
                        {
                            yBLists[i].TQ = szls[8];
                            yBLists[i].TEM = szls[6] + "～" + szls[7];
                            yBLists[i].Tmin = Convert.ToDouble(szls[6]);
                            yBLists[i].Tmax = Convert.ToDouble(szls[7]);
                            yBLists[i].FXFS = CLFXFS(szls[9], szls[10]);
                            yBLists[i].FX = szls[9];
                            yBLists[i].FS = szls[10];
                            yBLists[i].序号 = yBLists[i].ID == "53463" ? 0 : 99;
                            break;
                        }

                        if (yBLists[i].sc == 72)
                        {
                            yBLists[i].TQ = szls[13];
                            yBLists[i].TEM = szls[11] + "～" + szls[12];
                            yBLists[i].Tmin = Convert.ToDouble(szls[11]);
                            yBLists[i].Tmax = Convert.ToDouble(szls[12]);
                            yBLists[i].FXFS = CLFXFS(szls[14], szls[15]);
                            yBLists[i].FX = szls[14];
                            yBLists[i].FS = szls[15];
                            yBLists[i].序号 = yBLists[i].ID == "53463" ? 0 : 99;
                            break;
                        }

                        if (yBLists[i].sc == 96)
                        {
                            yBLists[i].TQ = szls[18];
                            yBLists[i].TEM = szls[16] + "～" + szls[17];
                            yBLists[i].Tmin = Convert.ToDouble(szls[16]);
                            yBLists[i].Tmax = Convert.ToDouble(szls[17]);
                            yBLists[i].FXFS = CLFXFS(szls[19], szls[20]);
                            yBLists[i].FX = szls[19];
                            yBLists[i].FS = szls[20];
                            yBLists[i].序号 = yBLists[i].ID == "53463" ? 0 : 99;
                            break;
                        }

                        yBLists[i].TQ = szls[23];
                        yBLists[i].TEM = szls[21] + "～" + szls[22];
                        yBLists[i].Tmin = Convert.ToDouble(szls[21]);
                        yBLists[i].Tmax = Convert.ToDouble(szls[22]);
                        yBLists[i].FXFS = CLFXFS(szls[24], szls[25]);
                        yBLists[i].FX = szls[24];
                        yBLists[i].FS = szls[25];
                        yBLists[i].序号 = yBLists[i].ID == "53463" ? 0 : 99;
                        break;
                    }
                }
            }

            return yBLists;
        }

        private string CLFXFS(string s1, string s2)
        {
            if (s1.Contains("转") && s2.Contains("转")) //如果风向风速都含“转”，则合并
            {
                return Regex.Split(s1, "转", RegexOptions.IgnoreCase)[0] + Regex.Split(s2, "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(s1, "转", RegexOptions.IgnoreCase)[1] + Regex.Split(s2, "转", RegexOptions.IgnoreCase)[1];
            }

            return s1 + s2;
        }

        private string 处理站点名称(string ID)
        {
            if (ID == "53463")
                return "市区";
            if (ID == "53464")
                return "土左";
            if (ID == "53368")
                return "武川";
            if (ID == "53467")
                return "托县";
            if (ID == "53469")
                return "和林";
            if (ID == "53562")
                return "清水河";
            return "呼和浩特市";
        }

        public class YBList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public string TQ { get; set; }
            public int 序号 { get; set; }
            public string TEM { get; set; }
            public string FX { get; set; }
            public string FS { get; set; }
            public string FXFS { get; set; }
            public double Tmax { get; set; }
            public double Tmin { get; set; }
            public int sc { get; set; }
        }

        public class SKList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public double Tmax { get; set; }
            public double Tmin { get; set; }
            public int sc { get; set; }
        }
    }
}