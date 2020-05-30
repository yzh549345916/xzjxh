using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 逐日评分详情.xaml 的交互逻辑
    /// </summary>
    public partial class 逐日评分详情new : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private ObservableCollection<乡镇精细化逐日评分ViewModel> 评分列表 = new My乡镇精细化逐日评分ViewModel().Clubs;
        private string xlsPath = "";
        public 逐日评分详情new()
        {

            InitializeComponent();
            BTLabel.Content = "乡镇精细化逐日评分详情查询";
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
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "警告"
                });
            }
            GRPFList.ItemsSource = 评分列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            if (!(sDate.SelectedDate.ToString().Length == 0))
            {
                评分列表.Clear();
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
                BTLabel.Content = Convert.ToDateTime((sDate.SelectedDate)).ToString("yyyy年MM月dd日") + SXSelect.Text + "小时乡镇精细化预报逐日评分详情";

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

                string QXname = QXSelect.Text, StationID = "";//待查询旗县站号

                for (int i = 0; i < QXName.Length; i++)
                {
                    if (QXname == QXName[i])
                    {
                        StationID = QXID[i];
                        break;
                    }
                }
                string[,] zrData = tj.ZRPF(startDate, SXSelect.Text, StationID);
                for (int i = 0; i < zrData.GetLength(0); i++)
                {
                    评分列表.Add(new 乡镇精细化逐日评分ViewModel()
                    {
                        StationID = zrData[i, 0],
                        QXGW = Convert.ToSingle(zrData[i, 1]),
                        SKGW = Convert.ToSingle(zrData[i, 2]),
                        SJGW = Convert.ToSingle(zrData[i, 3]),
                        QXDW = Convert.ToSingle(zrData[i, 4]),
                        SKDW = Convert.ToSingle(zrData[i, 5]),
                        SJDW = Convert.ToSingle(zrData[i, 6]),
                        QXTQ = zrData[i, 7],
                        QXQY = zrData[i, 8],
                        SKJS = zrData[i, 9],
                        SJTQ = zrData[i, 10],
                        SJQY = zrData[i, 11],
                        Name = zrData[i, 12],
                    });
                }




            }
            else
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = "请选择起止时间",
                    Header = "警告"
                });
            }
            RadSplashScreenManager.Close();
        }

        private void OnConfirmClosed_打开产品(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true) Process.Start(xlsPath);
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + QXSelect.Text + BTLabel.Content + ".xls";
                乡镇精细化逐日评分ViewModel[] dcsz = 评分列表.ToArray();
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
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].Name);
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
                    xlsPath = strPath;
                    RadWindow.Confirm(new DialogParameters
                    {
                        Content = "已成功导出数据至" + strPath + "\n是否打开？",
                        Closed = OnConfirmClosed_打开产品,
                        CancelButtonContent = "否",
                        Header = "提示",
                        OkButtonContent = "是"
                    });
                }
                catch (Exception ex)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = ex.Message,
                        Header = "警告"
                    });
                }
            }




        }



    }

    public class My乡镇精细化逐日评分ViewModel : ViewModelBase
    {
        private ObservableCollection<乡镇精细化逐日评分ViewModel> clubs;

        public ObservableCollection<乡镇精细化逐日评分ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<乡镇精细化逐日评分ViewModel> CreateClubs()
        {
            ObservableCollection<乡镇精细化逐日评分ViewModel> clubs = new ObservableCollection<乡镇精细化逐日评分ViewModel>();
            return clubs;
        }
    }
}
