using System;
using System.Collections.Generic;
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
using System.IO;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using Aspose.Cells;

namespace 旗县端
{
    /// <summary>
    /// 旗县乡镇精细化预报准确率页.xaml 的交互逻辑
    /// </summary>
    public partial class 旗县乡镇精细化预报准确率页 : Page
    {
        ObservableCollection<TJ.ZQLTJ1> sjzqlTJ1 = new ObservableCollection<TJ.ZQLTJ1>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 旗县乡镇精细化预报准确率页()
        {
            InitializeComponent();
            BTLabel.Content = "旗县乡镇精细化预报准确率逐日查询";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                sjzqlTJ1.Clear();
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
                string[,] TJXX = new string[intQXGS, 10];//行数为旗县个数加一，最后一行合计，列数为三天的最高最低晴雨准确率，为9.再加上一列名称，共10列
                string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                BTLabel.Content = startDate + "至" + endDate + "各旗县乡镇精细化预报准确率";

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
                
                for (int i = 0; i < intQXGS; i++)
                {

                        float[] zqlFloat = tj.QXZQL(startDate, endDate, QXID[i]);
                        sjzqlTJ1.Add(new TJ.ZQLTJ1()
                        {
                            Name = QXName[i],
                            SJ24TmaxZQL = zqlFloat[0],
                            SJ24TminZQL = zqlFloat[1],
                            SJ24QYZQL = zqlFloat[2],
                            SJ48TmaxZQL = zqlFloat[3],
                            SJ48TminZQL = zqlFloat[4],
                            SJ48QYZQL = zqlFloat[5],
                            SJ72TmaxZQL = zqlFloat[6],
                            SJ72TminZQL = zqlFloat[7],
                            SJ72QYZQL = zqlFloat[8],
                        });

                }
                ((this.FindName("ZQLList")) as DataGrid).ItemsSource = sjzqlTJ1;


            }
            else
            {
                MessageBox.Show("请选择起止时间");
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
            TJ.ZQLTJ1[] dcsz = sjzqlTJ1.ToArray();
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
                cellSheet.Cells[0, 0].PutValue("旗县");
                cellSheet.Cells[0, 1].PutValue("24小时最高温度准确率");
                cellSheet.Cells[0, 2].PutValue("24小时最低温度准确率");
                cellSheet.Cells[0, 3].PutValue("24小时晴雨准确率");
                cellSheet.Cells[0, 4].PutValue("48小时最高温度准确率");
                cellSheet.Cells[0, 5].PutValue("48小时最低温度准确率");
                cellSheet.Cells[0, 6].PutValue("48小时晴雨准确率");
                cellSheet.Cells[0, 7].PutValue("72小时最高温度准确率");
                cellSheet.Cells[0, 8].PutValue("72小时最低温度准确率");
                cellSheet.Cells[0, 9].PutValue("72小时晴雨准确率");


                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].Name);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(Math.Round(dcsz[i].SJ24TmaxZQL, 2));
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(Math.Round(dcsz[i].SJ24TminZQL, 2));
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(Math.Round(dcsz[i].SJ24QYZQL, 2));
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    cellSheet.Cells[i + 1, 4].PutValue(Math.Round(dcsz[i].SJ48TmaxZQL, 2));
                    cellSheet.Cells[i + 1, 4].SetStyle(style1);
                    cellSheet.Cells[i + 1, 5].PutValue(Math.Round(dcsz[i].SJ48TminZQL, 2));
                    cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    cellSheet.Cells[i + 1, 6].PutValue(Math.Round(dcsz[i].SJ48QYZQL, 2));
                    cellSheet.Cells[i + 1, 6].SetStyle(style1);
                    cellSheet.Cells[i + 1, 7].PutValue(Math.Round(dcsz[i].SJ72TmaxZQL, 2));
                    cellSheet.Cells[i + 1, 7].SetStyle(style1);
                    cellSheet.Cells[i + 1, 8].PutValue(Math.Round(dcsz[i].SJ72TminZQL, 2));
                    cellSheet.Cells[i + 1, 8].SetStyle(style1);
                    cellSheet.Cells[i + 1, 9].PutValue(Math.Round(dcsz[i].SJ72QYZQL, 2));
                    cellSheet.Cells[i + 1, 9].SetStyle(style1);
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
    }
}
