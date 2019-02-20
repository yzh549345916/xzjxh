using Aspose.Cells;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace 旗县端
{
    /// <summary>
    /// 集体评分逐日查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 集体评分逐日查询页 : Page
    {
        ObservableCollection<JTPF> jtpf = new ObservableCollection<JTPF>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 集体评分逐日查询页()
        {
            InitializeComponent();
            BTLabel.Content = "集体评分逐日查询";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                jtpf.Clear();
                if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
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
                    string[,] TJXX = new string[intQXGS + 1, 8];//行数为旗县个数加一，最后一行市局，列数为名称、晴雨评分、高温评分、低温评分、晴雨技巧、高温技巧、低温技巧、总技巧
                    string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                    string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                    BTLabel.Content = startDate + "至" + endDate + "全市各旗县集体评分";
                    
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
                    

                    for (int i = 0; i < intQXGS + 1; i++)
                    {
                        if (i < intQXGS)
                        {
                             float[] zqlFloat = tj.QXZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率
                            float[] XSList = new float[8];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                            //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                            XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24,2);
                            XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24,2);
                            XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24,2);
                            XSList[3] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[0] + 0.3 * XSList[1] + 0.3 * XSList[2]),2);//总评分
                            float[] QXJDWCSZ = tj.QXJDWC(startDate, endDate, QXID[i]);
                            float[] SJJDWCSZ = tj.SJJDWC(startDate, endDate, QXID[i]);
                            float[] SJzqlFloat = tj.SJZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率，主要计算技巧用晴雨准确率
                            float[] WDJQ = new float[6];//保存三天的最高、最低温度技巧
                            try
                            {
                                for(int j=0;j<6;j++)
                                {
                                    if(SJJDWCSZ[j]==0)
                                    {
                                        WDJQ[j] = 1.01F * 100;
                                    }
                                    else
                                    {
                                        WDJQ[j] = (SJJDWCSZ[j] - QXJDWCSZ[j]) / SJJDWCSZ[j];
                                        WDJQ[j] = (float)Math.Round(WDJQ[j]*100, 2);
                                    }
                                }

                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            float[] QYJQ = new float[3];
                            try
                            {
                                for(int j=0;j<3;j++)
                                {
                                    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                    //if(SJzqlFloat[2 + j * 3] == 100)
                                    //{
                                    //    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    //    QYJQ[j] = (float)Math.Round(QYJQ[j] , 2);
                                    //}
                                    //else
                                    //{
                                    //    QYJQ[j] = (zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3]) / (100 - SJzqlFloat[2 + j * 3]);
                                    //    QYJQ[j]= (float)Math.Round(QYJQ[j] * 100, 2);
                                    //}
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                            XSList[4] = (float)Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6) / 24,2);//晴雨技巧
                            XSList[5] = (float)Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6) / 24,2);//高温技巧
                            XSList[6] = (float)Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) / 24,2);//低温技巧
                            XSList[7] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[4] + 0.3 * XSList[5] + 0.3 * XSList[6]),2);//总技巧
                             jtpf.Add(new JTPF()
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
                            jtpf.Add(new JTPF()
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
                            });
                        }

                    }
                ((this.FindName("JTPFList")) as DataGrid).ItemsSource = jtpf;

                }
                else
                {
                    MessageBox.Show("请选择起止时间");
                }
            }

        }
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围
            try
            {
                DateTime dt1 = Convert.ToDateTime(sDate.SelectedDate);
                DateTime dt = dt1.AddDays(1 - dt1.Day);
                dt = dt.AddMonths(1).AddDays(-1);
                eDate.SelectedDate = dt;
            }
            catch (Exception ex)
            {
            }
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog m_Dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = m_Dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string strPath = m_Dialog.SelectedPath + "\\" + BTLabel.Content + ".xls";
            JTPF[] dcsz = jtpf.ToArray();
            try
            {
                Workbook workbook = new Workbook();
                Worksheet cellSheet = workbook.Worksheets[0];

                /*cellSheet.PageSetup.LeftMargin = 0.3;//左边距
                cellSheet.PageSetup.RightMargin = 0.3;//右边距
                cellSheet.PageSetup.TopMargin = 1;//上边距
                cellSheet.PageSetup.BottomMargin = 0.5;//下边距
                cellSheet.PageSetup.FooterMargin = 0.5;//页脚
                cellSheet.PageSetup.HeaderMargin = 0.5;//页眉
                cellSheet.PageSetup.Orientation = PageOrientationType.Landscape;*/
                cellSheet.PageSetup.CenterHorizontally = true;//水平居中
                cellSheet.PageSetup.CenterVertically = true;
                Aspose.Cells.Style style1 = workbook.CreateStyle();//新增样式  
                style1.HorizontalAlignment = TextAlignmentType.Center;//文字居中
                style1.VerticalAlignment = TextAlignmentType.Center;
                cellSheet.Cells[0, 0].PutValue("旗县名称");
                cellSheet.Cells[0, 1].PutValue("晴雨准确率");
                cellSheet.Cells[0, 2].PutValue("高温准确率");
                cellSheet.Cells[0, 3].PutValue("低温准确率");
                cellSheet.Cells[0, 4].PutValue("平均准确率");
                cellSheet.Cells[0, 5].PutValue("晴雨技巧");
                cellSheet.Cells[0, 6].PutValue("高温技巧");
                cellSheet.Cells[0, 7].PutValue("低温技巧");
                cellSheet.Cells[0, 8].PutValue("技巧总评分");


                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].Name);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(Math.Round(dcsz[i].QYPF, 2));
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(Math.Round(dcsz[i].GWPF, 2));
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(Math.Round(dcsz[i].DWPF, 2));
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    cellSheet.Cells[i + 1, 4].PutValue(Math.Round(dcsz[i].ZHPF, 2));
                    cellSheet.Cells[i + 1, 4].SetStyle(style1);
                    cellSheet.Cells[i + 1, 5].PutValue(Math.Round(dcsz[i].QYJQ, 2));
                    cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    cellSheet.Cells[i + 1, 6].PutValue(Math.Round(dcsz[i].GWJQ, 2));
                    cellSheet.Cells[i + 1, 6].SetStyle(style1);
                    cellSheet.Cells[i + 1, 7].PutValue(Math.Round(dcsz[i].DWJQ, 2));
                    cellSheet.Cells[i + 1, 7].SetStyle(style1);
                    cellSheet.Cells[i + 1, 8].PutValue(Math.Round(dcsz[i].AllJQ, 2));
                    cellSheet.Cells[i + 1, 8].SetStyle(style1);
                }
                //cellSheet.AutoFitColumns();
                int columnCount = cellSheet.Cells.MaxColumn;  //获取表页的最大列数
                cellSheet.AutoFitColumns();
                for (int col = 0; col < columnCount + 1; col++)
                {
                    cellSheet.Cells[0, col].SetStyle(style1);
                    cellSheet.Cells.SetColumnWidthPixel(col, cellSheet.Cells.GetColumnWidthPixel(col) + 30);
                }
                workbook.Save(strPath);
                MessageBoxResult dr = MessageBox.Show("已成功导出数据至" + strPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                if (dr == MessageBoxResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(strPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }

        public class JTPF//统计信息列表
        {
            public string Name { get; set; }
            public float QYPF { get; set; }
            public float GWPF { get; set; }
            public float DWPF { get; set; }
            public float ZHPF { get; set; }
            public float QYJQ { get; set; }
            public float GWJQ { get; set; }
            public float DWJQ { get; set; }
            public float AllJQ { get; set; }
        }
    }
}
