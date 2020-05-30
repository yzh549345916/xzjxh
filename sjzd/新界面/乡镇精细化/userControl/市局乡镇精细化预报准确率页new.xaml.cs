using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
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
    /// 市局乡镇精细化预报准确率页.xaml 的交互逻辑
    /// </summary>
    public partial class 市局乡镇精细化预报准确率页new : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private readonly ObservableCollection<市局乡镇精细化预报准确率ViewModel> 准确率列表 = new My市局乡镇精细化准确率ViewModel().Clubs;
        public 市局乡镇精细化预报准确率页new()
        {
            InitializeComponent();
            BTLabel.Content = "市局乡镇精细化准确率查询";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            ZQLList.ItemsSource = 准确率列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
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
                string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                BTLabel.Content = startDate + "至" + endDate + "市局乡镇精细化准确率查询";

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
                准确率列表.Clear();
                for (int i = 0; i < intQXGS + 1; i++)
                {
                    if (i < intQXGS)
                    {
                        float[] zqlFloat = tj.SJZQL(startDate, endDate, QXID[i]);
                        准确率列表.Add(new 市局乡镇精细化预报准确率ViewModel(QXName[i], zqlFloat[0], zqlFloat[1], zqlFloat[2], zqlFloat[3], zqlFloat[4], zqlFloat[5], zqlFloat[6], zqlFloat[7], zqlFloat[8]));
                    }
                    else
                    {
                        float[] zqlFloat = tj.SJQSZQL(startDate, endDate);
                        准确率列表.Add(new 市局乡镇精细化预报准确率ViewModel("全市", zqlFloat[0], zqlFloat[1], zqlFloat[2], zqlFloat[3], zqlFloat[4], zqlFloat[5], zqlFloat[6], zqlFloat[7], zqlFloat[8]));
                    }

                }



            }
            else
            {
                RadWindow.Alert("请选择起止时间");
            }
            RadSplashScreenManager.Close();
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xls";
                市局乡镇精细化预报准确率ViewModel[] dcsz = 准确率列表.ToArray();
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
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].旗县);
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
        private void OnConfirmClosed_打开产品(object sender, WindowClosedEventArgs e)
        {

            if (e.DialogResult == true)
            {
                Process.Start(xlsPath);
            }

        }
        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.SelectedDate = null; //将已经选取的结束时间清空
            ObservableCollection<DateTime> blackoutDates = new ObservableCollection<DateTime>();
            DateTime dateTime = new DateTime();
            DateTime dateTime1 = Convert.ToDateTime(sDate.SelectedDate).AddDays(-1);
            while (dateTime.CompareTo(dateTime1) <= 0)
            {
                blackoutDates.Add(dateTime);
                dateTime = dateTime.AddDays(1);
            }

            eDate.BlackoutDates = blackoutDates; //结束时间随着开始时间的改变增加新的范围
            try
            {
                DateTime dt1 = Convert.ToDateTime(sDate.SelectedDate);
                DateTime dt = dt1.AddDays(1 - dt1.Day);
                dt = dt.AddMonths(1).AddDays(-1);
                eDate.SelectedDate = dt;
            }
            catch
            {
            }
        }

    }

    public class My市局乡镇精细化准确率ViewModel : ViewModelBase
    {
        private ObservableCollection<市局乡镇精细化预报准确率ViewModel> clubs;

        public ObservableCollection<市局乡镇精细化预报准确率ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<市局乡镇精细化预报准确率ViewModel> CreateClubs()
        {
            ObservableCollection<市局乡镇精细化预报准确率ViewModel> clubs = new ObservableCollection<市局乡镇精细化预报准确率ViewModel>();
            return clubs;
        }
    }
}
