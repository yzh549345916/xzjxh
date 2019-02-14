using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Aspose.Cells;

namespace sjzd
{
    /// <summary>
    /// 逐日评分详情.xaml 的交互逻辑
    /// </summary>
    public partial class 逐日评分详情 : Page
    {
        ObservableCollection<ZRPF> zrpf = new ObservableCollection<ZRPF>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public 逐日评分详情()
        {
            
            InitializeComponent();
            BTLabel.Content = "逐日评分详情查询";
            string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
            string QXNameDZPath = Environment.CurrentDirectory + @"\设置文件\旗县名称显示对照.txt";
            Int16 intQXGS = 0;

            try
            {
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
                using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.Default))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < QXName.Length; i++)
                        {
                            if (line1.Split('=')[0] == QXName[i])
                            {
                                QXName[i] = line1.Split('=')[1];
                            }
                        }
                    }
                }
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                for (int i = 0; i < intQXGS; i++)
                {
                    mydic.Add(i, QXName[i]);
                }
                QXSelect.ItemsSource = mydic;
                QXSelect.SelectedValuePath = "Key";
                QXSelect.DisplayMemberPath = "Value";
                QXSelect.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sDate.SelectedDate.ToString().Length == 0))
            {
                zrpf.Clear();
                string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                string QXNameDZPath = Environment.CurrentDirectory + @"\设置文件\旗县名称显示对照.txt";
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
                string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");//查询日期
                BTLabel.Content = Convert.ToDateTime((sDate.SelectedDate)).ToString("yyyy年MM月dd日") + SXSelect.Text + "小时预报逐日评分详情";

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
                using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.Default))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < QXName.Length; i++)
                        {
                            if (line1.Split('=')[0] == QXName[i])
                            {
                                QXName[i] = line1.Split('=')[1];
                            }
                        }
                    }
                }
                TJ tj = new TJ();
                
                string QXname = QXSelect.Text,StationID="";//待查询旗县站号
                /*using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.Default))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        try
                        {
                            if (line1.Split('=')[1] == QXname)
                            {
                                QXname = line1.Split('=')[0];
                                break;
                            }
                        }
                        catch
                        {

                        }
                        
                    }
                }*/
                for(int i=0;i<QXName.Length;i++)
                {
                    if(QXname==QXName[i])
                    {
                        StationID = QXID[i];
                        break;
                    }
                }
                string[,] zrData=tj.ZRPF(startDate,SXSelect.Text,StationID);
                for(int i =0;i<zrData.GetLength(0); i++)
                {
                    zrpf.Add(new ZRPF()
                    {
                        StationID = zrData[i, 0],
                        QXGW = zrData[i, 1],
                        SKGW = zrData[i, 2],
                        SJGW = zrData[i, 3],
                        QXDW = zrData[i, 4],
                        SKDW = zrData[i, 5],
                        SJDW = zrData[i, 6],
                        QXTQ = zrData[i, 7],
                        QXQY = zrData[i, 8],
                        SKJS = zrData[i, 9],
                        SJTQ = zrData[i, 10],
                        SJQY = zrData[i, 11],
                        Name=zrData[i,12],
                    });
                }

                ((this.FindName("GRPFList")) as System.Windows.Controls.DataGrid).ItemsSource = zrpf;


            }
            else
            {
                System.Windows.MessageBox.Show("请选择查询时间");
            }

        }

        public class ZRPF//统计信息列表
        {

            public string Name { get; set; }
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
            public string SJTQ { get; set; }
            public string SJQY { get; set; }
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            /*DataTable dt;
            dt= GRPFList.ItemsSourc]*/
            //DataTable dt = (DataTable)GRPFList.ItemsSource;
            FolderBrowserDialog m_Dialog = new FolderBrowserDialog();
            DialogResult result = m_Dialog.ShowDialog(); 

            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string strPath = m_Dialog.SelectedPath+ "\\"+ QXSelect.Text+BTLabel.Content+".xls";
            ZRPF[] dcsz = zrpf.ToArray();
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
                cellSheet.Cells[0, 0].PutValue("名称");
                cellSheet.Cells[0, 1].PutValue("区站号");
                cellSheet.Cells[0, 2].PutValue("本台高温");
                cellSheet.Cells[0, 3].PutValue("实况高温");
                cellSheet.Cells[0, 4].PutValue("市台高温");
                cellSheet.Cells[0, 5].PutValue("本台低温");
                cellSheet.Cells[0, 6].PutValue("实况低温");
                //cellSheet.Cells[0, 6].SetStyle
                cellSheet.Cells[0, 7].PutValue("市台低温");
                cellSheet.Cells[0, 8].PutValue("本台天气");
                cellSheet.Cells[0, 9].PutValue("本台晴雨");
                cellSheet.Cells[0, 10].PutValue("实况降水量");
                cellSheet.Cells[0, 11].PutValue("市台天气");
                cellSheet.Cells[0, 12].PutValue("市台晴雨");
                //cellSheet.Cells.SetColumnWidthPixel(0, 300);
                

                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i+1, 0].PutValue(dcsz[i].Name);
                    cellSheet.Cells[i + 1, 0].SetStyle(style1);
                    cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].StationID);
                    cellSheet.Cells[i + 1, 1].SetStyle(style1);
                    cellSheet.Cells[i + 1, 2].PutValue(dcsz[i].QXGW);
                    cellSheet.Cells[i + 1, 2].SetStyle(style1);
                    cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].SKGW);
                    cellSheet.Cells[i + 1, 3].SetStyle(style1);
                    cellSheet.Cells[i + 1, 4].PutValue(dcsz[i].SJGW);
                    cellSheet.Cells[i + 1, 4].SetStyle(style1);
                    cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].QXDW);
                    cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    cellSheet.Cells[i + 1, 6].PutValue(dcsz[i].SKDW);
                    cellSheet.Cells[i + 1, 6].SetStyle(style1);
                    cellSheet.Cells[i + 1, 7].PutValue(dcsz[i].SJDW);
                    cellSheet.Cells[i + 1, 7].SetStyle(style1);
                    cellSheet.Cells[i + 1, 8].PutValue(dcsz[i].QXTQ);
                    cellSheet.Cells[i + 1, 8].SetStyle(style1);
                    cellSheet.Cells[i + 1, 9].PutValue(dcsz[i].QXQY);
                    cellSheet.Cells[i + 1, 9].SetStyle(style1);
                    cellSheet.Cells[i + 1, 10].PutValue(dcsz[i].SKJS);
                    cellSheet.Cells[i + 1, 10].SetStyle(style1);
                    cellSheet.Cells[i + 1, 11].PutValue(dcsz[i].SJTQ);
                    cellSheet.Cells[i + 1, 11].SetStyle(style1);
                    cellSheet.Cells[i + 1, 12].PutValue(dcsz[i].SJQY);
                    cellSheet.Cells[i + 1, 12].SetStyle(style1);
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
                MessageBoxResult dr = System.Windows.MessageBox.Show("已成功导出数据至" + strPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                if (dr == MessageBoxResult.Yes)
                {
                    try
                    {
                        System.Diagnostics.Process.Start(strPath);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }


    }
}
