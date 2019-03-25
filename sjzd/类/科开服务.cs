using Aspose.Cells;
using Aspose.Words;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace sjzd
{
    class 科开服务
    {
        public void DCWord(Int16 sc,DateTime dt, string StationID,Int16 Days)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = dt.ToString("yyyyMMdd");
            string ybData= bw2.Sjyb(strDate, sc.ToString().PadLeft(2,'0'));
            if (ybData.Trim().Length > 0)
            {
                try
                {

                    string SJsaPath = System.Environment.CurrentDirectory + @"\科开服务\";
                    SJsaPath += dt.ToString("yyyy") + "\\" + dt.ToString("yyyy-MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJsaPath += dt.ToString("yyyy年MM月dd日") +Days+ "天服务预报.docx";
                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.Font.Size = 30;
                    builder.Font.Bold = true;
                    builder.Font.Name = "微软雅黑";
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.Write("呼和浩特市灾害防御中心\r\n气象服务预报");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;

                    //builder.MoveToBookmark("预报日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.Write("\r\n呼和浩特市灾害防御中心        ");
                    builder.Write(dt.ToString("yyyy年MM月dd日") + sc.ToString().PadLeft(2, '0') + "时");
                    builder.InsertParagraph();
                    //builder.InsertParagraph();
                    //builder.MoveToBookmark("预报0");
                    Shape lineShape = new Shape(doc, ShapeType.Line);
                    lineShape.Width = 420;
                    Stroke stroke = lineShape.Stroke;
                    stroke.On = true;
                    stroke.Weight = 5.5;
                    stroke.Color = Color.Red;
                    stroke.LineStyle = ShapeLineStyle.ThinThick;
                    builder.InsertNode(lineShape);
                    builder.InsertParagraph();
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single; 
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;          
                    for (Int16 j = 0; j < Days; j++)
                    {
    
                        if (sc == 8)
                            builder.Write(dt.AddDays(j).ToString("yyyy年MM月dd日\r\n"));
                        else
                            builder.Write(dt.AddDays(j+1).ToString("yyyy年MM月dd日\r\n"));
                        List<YBList> yBLists = CLYB(ybData,j,StationID);
                        if (yBLists.Count > 0)
                        {

                            builder.RowFormat.HeadingFormat = true;
                            builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                            builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐 
                            builder.Font.Bold = false;
                            foreach (YBList yBList in yBLists)
                            {

                                builder.InsertCell();
                                builder.CellFormat.Width = 15;
                                builder.Write(yBList.Name);
                                builder.InsertCell();
                                builder.CellFormat.Width = 15;
                                builder.Write(yBList.TQ);
                                builder.InsertCell();
                                builder.CellFormat.Width = 15;
                                builder.Write(yBList.FXFS);
                                builder.InsertCell();
                                builder.CellFormat.Width = 10;
                                builder.Write(yBList.TEM);
                                builder.EndRow();
                            }
                            builder.EndTable();
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

                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("城镇预报数据获取失败，无法制作产品");
            }


        }
        public void DCWordbyALLDate(Int16 sc, DateTime dt, string StationID, Int16 Days)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = dt.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb(strDate, sc.ToString().PadLeft(2, '0'));
            if (ybData.Trim().Length > 0)
            {
                try
                {

                    string SJsaPath = System.Environment.CurrentDirectory + @"\科开服务\";
                    SJsaPath += dt.ToString("yyyy") + "\\" + dt.ToString("yyyy-MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJsaPath += dt.ToString("yyyy年MM月dd日") + Days + "天服务预报.docx";
                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.Font.Size = 30;
                    builder.Font.Bold = true;
                    builder.Font.Name = "微软雅黑";
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.Write("呼和浩特市灾害防御中心\r\n气象服务预报");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;

                    //builder.MoveToBookmark("预报日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.Write("\r\n呼和浩特市灾害防御中心        ");
                    builder.Write(dt.ToString("yyyy年MM月dd日") + sc.ToString().PadLeft(2, '0') + "时");
                    builder.InsertParagraph();
                    //builder.InsertParagraph();
                    //builder.MoveToBookmark("预报0");
                    Shape lineShape = new Shape(doc, ShapeType.Line);
                    lineShape.Width = 420;
                    Stroke stroke = lineShape.Stroke;
                    stroke.On = true;
                    stroke.Weight = 5.5;
                    stroke.Color = Color.Red;
                    stroke.LineStyle = ShapeLineStyle.ThinThick;
                    builder.InsertNode(lineShape);
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.InsertParagraph();
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.RowFormat.HeadingFormat = true;

                    
                    builder.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐 
                    Table table = builder.StartTable();
                    
                    builder.InsertCell();
                    //表格居中&&宽度自适应
                    
                    builder.CellFormat.Width = 30;
                    builder.Write("日期");
                    builder.InsertCell();
                    builder.CellFormat.Width = 60;
                    builder.Write("地区");
                    builder.InsertCell();
                    builder.CellFormat.Width = 60;
                    builder.Write("天气现象");
                    builder.InsertCell();
                    builder.CellFormat.Width = 100;
                    builder.Write("风向风速");
                    builder.InsertCell();
                    builder.CellFormat.Width = 50;
                    builder.Write("气温");
                    builder.EndRow();
                    for (Int16 j = 0; j < Days; j++)
                    {
                        builder.InsertCell();
                        string dateStr = "";
                        if (sc == 8)
                            dateStr = dt.AddDays(j).ToString("M月d日");
                        else
                            dateStr = dt.AddDays(j + 1).ToString("M月d日");
                       
                       
                        builder.Font.Size = 14;
                        builder.CellFormat.Orientation = TextOrientation.Horizontal;
                        builder.CellFormat.Width = 30;
                        builder.CellFormat.WrapText = true;
                        builder.CellFormat.VerticalMerge = CellMerge.First;
                        
                        builder.Write(dateStr);
                        List<YBList> yBLists = CLYB(ybData, j, StationID);
                        if (yBLists.Count > 0)
                        {

                            
                            foreach (YBList yBList in yBLists)
                            {
                                if(yBList!=yBLists[0])
                                {
                                    builder.InsertCell();// 添加一个单元格
                                    builder.CellFormat.Width = 30;
                                    builder.CellFormat.VerticalMerge = CellMerge.Previous;


                                }
                                
                                builder.InsertCell();
                                builder.CellFormat.VerticalMerge = CellMerge.None;
                                builder.CellFormat.Width = 60;
                                builder.Write(yBList.Name);
                                builder.InsertCell();
                                builder.CellFormat.Width =60;
                                builder.Write(yBList.TQ);
                                builder.InsertCell();
                                builder.CellFormat.Width = 100;
                                builder.Write(yBList.FXFS);
                                builder.InsertCell();
                                builder.CellFormat.Width = 50;
                                builder.Write(yBList.TEM);
                                builder.EndRow();
                                
                            }
                           // builder.EndTable();
                        }
                    }

                    table.StyleIdentifier = StyleIdentifier.GridTable5DarkAccent1;


                    

                    //应用哪些功能应按样式格式化。
                   
                   // table.StyleOptions =TableStyleOptions.FirstColumn | TableStyleOptions.RowBands | TableStyleOptions.FirstRow;
                    table.AutoFit(AutoFitBehavior.AutoFitToContents);
                    table.Alignment = TableAlignment.Center;
                    foreach(var ss in table.ChildNodes)
                    {
                        try
                        {
                            Aspose.Words.Tables.Row row = ss as Aspose.Words.Tables.Row;
                            foreach(var s2 in row.Cells)
                            {
                                try
                                {
                                    Aspose.Words.Tables.Cell cell = s2 as Aspose.Words.Tables.Cell;
                                    cell.CellFormat.VerticalAlignment = CellVerticalAlignment.Center;//垂直居中对齐 
                                }
                                catch { };
                            }
                        }
                        catch { }
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
                MessageBox.Show("城镇预报数据获取失败，无法制作产品");
            }


        }
        public void DCZXWord(Int16 sc, DateTime dt, string StationID, Int16 Days)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = dt.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb(strDate, sc.ToString().PadLeft(2, '0'));
            if (ybData.Trim().Length > 0)
            {
                try
                {

                    string SJsaPath = System.Environment.CurrentDirectory + @"\科开服务\";
                    SJsaPath += dt.ToString("yyyy") + "\\" + dt.ToString("yyyy-MM") + "\\";
                    if (!File.Exists(SJsaPath))
                    {
                        Directory.CreateDirectory(SJsaPath);
                    }
                    SJsaPath += dt.ToString("yyyy年MM月dd日") + Days + "天服务预报折线.docx";
                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.PageSetup.Orientation = Orientation.Landscape;//更改纸张方向
                    builder.Font.Size = 20;
                    builder.Font.Bold = true;
                    builder.Font.Name = "微软雅黑";
                    builder.ParagraphFormat.Alignment = ParagraphAlignment.Center;
                    builder.Write("呼和浩特市灾害防御中心\r\n气象服务预报折线");
                    builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                    builder.CellFormat.Borders.Color = Color.Black;

                    //builder.MoveToBookmark("预报日期");
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Font.Bold = false;
                    builder.Write("\r\n呼和浩特市灾害防御中心           ");
                    builder.Write(dt.ToString("yyyy年MM月dd日") + sc.ToString().PadLeft(2, '0') + "时");
                    builder.InsertParagraph();
                    //builder.InsertParagraph();
                    //builder.MoveToBookmark("预报0");
                    Shape lineShape = new Shape(doc, ShapeType.Line);
                    lineShape.Width = 620;
                    Stroke stroke = lineShape.Stroke;
                    stroke.On = true;
                    stroke.Weight = 5.5;
                    stroke.Color = Color.Red;
                    stroke.LineStyle = ShapeLineStyle.ThinThick;
                    builder.InsertNode(lineShape);
                    builder.InsertParagraph();
                    List<YBZXList> yBZXLists = new List<YBZXList>();
                    for (Int16 j = 0; j < Days; j++)
                    {
                        List<YBList> yBLists = CLYB(ybData, j, StationID);
                        yBLists = yBLists.OrderBy(y => y.ID).ToList();
                        yBZXLists.Add(new YBZXList
                        {
                            date=dt.AddDays(j),
                            yBLists=yBLists,
                            sc=sc
                        });
                    }
                    Shape shapeAll = builder.InsertChart(ChartType.Line, 600, 320);
                    Chart chartAll = shapeAll.Chart;
                    builder.InsertParagraph();
                    chartAll.Series.Clear();
                    chartAll.AxisX.TickLabelSpacing = 2;//坐标间隔
                    chartAll.AxisX.Crosses = AxisCrosses.Minimum;
                    chartAll.Title.Text =  "气温变化折线";
                    for (int i=0;i<yBZXLists[0].yBLists.Count;i++)
                    {
                        
                        Shape shape = builder.InsertChart(ChartType.Line, 600, 420);
                        builder.InsertParagraph();
                        Chart chart = shape.Chart;
                        chart.Series.Clear();
                        chart.AxisX.TickLabelSpacing = 2;//坐标间隔
                        chart.AxisX.Crosses = AxisCrosses.Minimum;
                        chart.Title.Text = yBZXLists[0].yBLists[i].Name + "气温变化折线";
                        string[] timsSZ = new string[Days*2];
                        double[] temSZ = new double[Days * 2];
                        for(int j=0;j< yBZXLists.Count;j++)
                        {
                            timsSZ[j * 2] = yBZXLists[j].date.ToString("MM月dd日");
                            timsSZ[j * 2+1] = yBZXLists[j].date.ToString("MM月dd日");
                            string temStr=yBZXLists[j].yBLists[i].TEM;
                             temStr=temStr.Replace("℃", "");
                            temStr = temStr.Replace("～", " ");
                            temSZ[j * 2] = Convert.ToDouble(temStr.Split()[0]);
                            temSZ[j * 2+1] = Convert.ToDouble(temStr.Split()[1]);
                        }
                        ChartSeries series0 = chart.Series.Add(yBZXLists[0].yBLists[i].Name, timsSZ, temSZ);
                        ChartSeries series1 = chartAll.Series.Add(yBZXLists[0].yBLists[i].Name, timsSZ, temSZ);
                        series0.Smooth = true;
                        series1.Smooth = true;
                        for (int j=0;j<temSZ.Length;j++)
                        {
                            ChartDataLabel label = series0.DataLabels.Add(j);
                            label.ShowValue = true;
                            label.NumberFormat.FormatCode = "#,##0\"℃\"";
                            if (yBZXLists[0].yBLists[i].ID=="53463")
                            {
                                ChartDataLabel label2 = series1.DataLabels.Add(j);
                                label2.ShowValue = true;
                                label2.NumberFormat.FormatCode = "#,##0\"℃\"";
                            }
                           
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

                catch (Exception ex)
                {

                }
            }
            else
            {
                MessageBox.Show("城镇预报数据获取失败，无法制作产品");
            }


        }

        public void DCExcel(Int16 sc, DateTime date, string StationID)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = date.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb(strDate, sc.ToString().PadLeft(2, '0'));
            if (ybData.Trim().Length > 0)
            {
                DataTable dt = new DataTable("chart");

                DataColumn dc1 = new DataColumn("日期", Type.GetType("System.DateTime"));
                DataColumn dc2 = new DataColumn("低温", Type.GetType("System.Double"));
                DataColumn dc3 = new DataColumn("高温", Type.GetType("System.Double"));
                DataColumn dc4 = new DataColumn("天气", Type.GetType("System.String"));
                dt.Columns.Add(dc1);
                dt.Columns.Add(dc2);
                dt.Columns.Add(dc3);
                dt.Columns.Add(dc4);
                if (sc == 20)
                    date = date.AddDays(1);
                string mbPath = Environment.CurrentDirectory + @"\模版\科开表格.xlsx";
                string path = Environment.CurrentDirectory + @"\模版\测试啊.xlsx";
                for (Int16 j = 0; j < 5; j++)
                {
                    DataRow dr = dt.NewRow();
                    dr["日期"] = date.AddDays(j);
                    dr["低温"] = 1- j;
                    dr["高温"] = 10+j;
                    dr["天气"] = "大风吹";
                    dt.Rows.Add(dr);
                    
                    ExportExcel(dt, mbPath, "测试", path);

                    List<YBList> yBLists = CLYB(ybData, j, StationID);
                }
                MessageBoxResult dr2 = MessageBox.Show("产品制作完成,保存路径为：\r\n" + path + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                if (dr2 == MessageBoxResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

            }
               


            
            // Workbook xlBook = new Workbook(); //工作簿 
            // Aspose.Cells.Style style1 = xlBook.Styles[xlBook.Styles.Add()];
            // style1.Pattern = Aspose.Cells.BackgroundType.Solid;//单元格的线：实线
            // style1.HorizontalAlignment = Aspose.Cells.TextAlignmentType.Center;//字体居中
            // style1.Font.Name = "宋体";//文字字体
            // style1.Font.Size = 8;//文字大小
            // style1.IsTextWrapped = true;//单元格内容自动换行
            // style1.ForegroundColor = System.Drawing.Color.FromArgb(50, 205, 50);//设置背景色 可以参考颜色代码对照表
            // style1.Font.IsBold = true;//粗体
            //// Cells.Merge(1, 2, 2, 2);//合并单元格
            // //Cells.SetRowHeight(0, 38);//设置行高 
            //// Cells.SetColumnWidth(0, 25);//设置列宽
            // Cells[0, 1].PutValue("2016年中科核安（标准产品）名称型号库存对照表");//添加内容
            // style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;//应用边界线 左边界线
            // style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin; //应用边界线 右边界线
            // style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;//应用边界线 上边界线
            // style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;//应用边界线 下边界线

            // //excel数据列 循环列添加样式
            // for (int i = 0; i < dgv.ColumnCount - 1; i++)
            // {
            //     Cells[0, i].SetStyle(style1);
            // }
            // //excel数据行 循环行添加样式
            // for (int i = 0; i < dt.Rows.Count; i++)
            // {
            //     for (int j = 0; j < 10; j++)
            //     {
            //         xlSheet.Cells[i + 1, j].SetStyle(style2);
            //         Cells[i + 1, 0].SetStyle(style3);
            //     }
            // }
        }

        /// <summary>
        /// 将DataTable导出指定路径的Excel文件
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <param name="tableName">Excel文件的表头内容</param>
        /// <param name="path">文件保存的全路径</param>
        public static void ExportExcel(DataTable dt,string mbPath, string headerText, string path)
        {
            Workbook workbook = new Workbook(mbPath); //工作簿 
            Worksheet sheet = workbook.Worksheets[0]; //工作表 
            Cells cells = sheet.Cells;//单元格 

            //为标题设置样式     
            Aspose.Cells.Style styleTitle = workbook.Styles[workbook.Styles.Add()];//新增样式 
            styleTitle.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            styleTitle.Font.Name = "宋体";//文字字体 
            styleTitle.Font.Size = 18;//文字大小 
            styleTitle.Font.IsBold = true;//粗体 

            //样式2 
            Aspose.Cells.Style style2 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style2.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style2.Font.Name = "宋体";//文字字体 
            style2.Font.Size = 14;//文字大小 
            style2.Font.IsBold = true;//粗体 
            style2.IsTextWrapped = true;//单元格内容自动换行 
            style2.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style2.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式3 
            Aspose.Cells.Style style3 = workbook.Styles[workbook.Styles.Add()];//新增样式 
            style3.HorizontalAlignment = TextAlignmentType.Center;//文字居中 
            style3.Font.Name = "宋体";//文字字体 
            style3.Font.Size = 12;//文字大小 
            style3.Borders[Aspose.Cells.BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[Aspose.Cells.BorderType.RightBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[Aspose.Cells.BorderType.TopBorder].LineStyle = CellBorderType.Thin;
            style3.Borders[Aspose.Cells.BorderType.BottomBorder].LineStyle = CellBorderType.Thin;

            //样式4
            Aspose.Cells.Style style4 = style3;//新增样式 

            style4.Number = 17;
            int Colnum = dt.Columns.Count;//表格列数 
            int Rownum = dt.Rows.Count;//表格行数 

            //生成行1 标题行    
            cells.Merge(0, 0, 1, Colnum);//合并单元格 
            cells[0, 0].PutValue(headerText);//填写内容 
            cells[0, 0].SetStyle(styleTitle);
            cells.SetRowHeight(0, 38);

            //生成行2 列名行 
            for (int i = 0; i < Colnum; i++)
            {
                cells[1, i].PutValue(dt.Columns[i].ColumnName);
                cells[1, i].SetStyle(style2);
                cells.SetRowHeight(1, 25);
            }

            //生成数据行 
            for (int i = 0; i < Rownum; i++)
            {
                for (int k = 0; k < Colnum; k++)
                {
                    cells[2 + i, k].PutValue(dt.Rows[i][k]);
                    if(k== Colnum-1)
                        cells[2 + i, k].SetStyle(style4);
                    else
                        cells[2 + i, k].SetStyle(style3);

                }
                cells.SetRowHeight(2 + i, 24);
            }

            workbook.Save(path);
        }

        public List<YBList> CLYB(string ybData,Int16 day, string StationID)
        {
            List<YBList> yBLists = new List<YBList>();
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
            string con = util.Read("OtherConfig", "xzjxhDB");
            using (SqlConnection mycon = new SqlConnection(con))
            {
                mycon.Open();//打开
                string sql = String.Format("select * from 社区精细化预报站点 where Station_levl = 93 order by StatioID");
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    try
                    {
                        string strID = sqlreader.GetString(sqlreader.GetOrdinal("GJStatioID"));
                        foreach (string ss in StationID.Split(','))
                        {
                            if (strID == ss.Trim())
                            {
                                yBLists.Add(new YBList()
                                {
                                    ID = strID,
                                    Name = sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                                });
                                break;
                            }
                        }

                        
                    }
                    catch (Exception ex)
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
                        yBLists[i].TQ = szls[3+5*day];
                        yBLists[i].TEM = szls[1 + 5 * day] + "～" + szls[2 + 5 * day] + "℃";
                        if (szls[4 + 5 * day].Contains("转") && szls[5 + 5 * day].Contains("转"))//如果风向风速都含“转”，则合并
                        {
                            yBLists[i].FXFS =  Regex.Split(szls[4 + 5 * day], "转", RegexOptions.IgnoreCase)[0] + Regex.Split(szls[5 + 5 * day], "转", RegexOptions.IgnoreCase)[0] + "转" + Regex.Split(szls[4 + 5 * day], "转", RegexOptions.IgnoreCase)[1] + Regex.Split(szls[5 + 5 * day], "转", RegexOptions.IgnoreCase)[1];
                        }
                        else
                        {
                            yBLists[i].FXFS = szls[4 + 5 * day] + szls[5 + 5 * day];
                        }


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
            public string FXFS { get; set; }
        }
        public class YBZXList
        {
            public List<YBList> yBLists { get; set; }
            public int sc { get; set; }
            public DateTime date { get; set; }

        }
    }
}
