using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;
using UserControl = System.Windows.Controls.UserControl;

namespace sjzd
{
    /// <summary>
    ///     到报率查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 到报率查询页new : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private readonly ObservableCollection<乡镇精细化到报率ViewModel> 到报率列表 = new MyViewModel().Clubs;
        private string xlsPath = "";

        public 到报率查询页new()
        {
            InitializeComponent();
            BTLabel.Content = "乡镇精细化到报率查询";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            ZQLList.ItemsSource = 到报率列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            if (!(sDate.SelectedDate.ToString().Length == 0) && !(eDate.SelectedDate.ToString().Length == 0))
            {
                到报率列表.Clear();
                string configXZPath = Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                string QXNameDZPath = Environment.CurrentDirectory + @"\设置文件\旗县名称显示对照.txt";
                short intQXGS = 0;
                try
                {
                    using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"))) //统计旗县个数
                    {
                        string line1 = "";
                        while ((line1 = sr.ReadLine()) != null)
                            if (line1.Split(':')[0] == "旗县个数")
                            {
                                intQXGS = Convert.ToInt16(line1.Split(':')[1]);
                                break;
                            }
                    }

                    string[] QXID = new string[intQXGS];
                    string[] QXName = new string[intQXGS];
                    using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"))) //第二行开始每两行为旗县及乡镇区站号列表
                    {
                        string line1 = "";
                        short lineCount = 0;
                        short count = 0;
                        while (lineCount < intQXGS * 2 + 1)
                        {
                            line1 = sr.ReadLine();
                            if (lineCount > 0)
                            {
                                if (lineCount % 2 == 1)
                                    QXName[count] = line1.Split(',')[0];
                                else
                                    QXID[count++] = line1.Split(',')[0];
                            }

                            lineCount++;
                        }
                    }

                    using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line1 = "";
                        while ((line1 = sr.ReadLine()) != null)
                            for (int i = 0; i < QXName.Length; i++)
                                if (line1.Split('=')[0] == QXName[i])
                                    QXName[i] = line1.Split('=')[1];
                    }

                    string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                    string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                    BTLabel.Content = startDate + "至" + endDate + "乡镇精细化到报率查询";
                    TJ tJ = new TJ();
                    string[,] DBLSZ = tJ.DBLTJ(QXID, QXName, startDate, endDate);

                    for (int i = 0; i < DBLSZ.GetLength(0); i++)
                        到报率列表.Add(new 乡镇精细化到报率ViewModel(DBLSZ[i, 0], Convert.ToSingle(DBLSZ[i, 1]),
                            Convert.ToSingle(DBLSZ[i, 2]), DBLSZ[i, 3], Convert.ToSingle(DBLSZ[i, 4]), DBLSZ[i, 5]));
                }
                catch (Exception)
                {
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

        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
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

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xls";
                乡镇精细化到报率ViewModel[] dcsz = 到报率列表.ToArray();
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
                    cellSheet.PageSetup.CenterHorizontally = true; //水平居中
                    cellSheet.PageSetup.CenterVertically = true;
                    Aspose.Cells.Style style1 = workbook.CreateStyle(); //新增样式  
                    style1.HorizontalAlignment = TextAlignmentType.Center; //文字居中
                    style1.VerticalAlignment = TextAlignmentType.Center;
                    cellSheet.Cells[0, 0].PutValue("旗县");
                    cellSheet.Cells[0, 1].PutValue("到报率(%)");
                    cellSheet.Cells[0, 2].PutValue("缺报率(%)");
                    cellSheet.Cells[0, 3].PutValue("缺报日期");
                    cellSheet.Cells[0, 4].PutValue("逾期率(%)");
                    cellSheet.Cells[0, 5].PutValue("逾期日期");

                    for (int i = 0; i < dcsz.Length; i++)
                    {
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].旗县);
                        cellSheet.Cells[i + 1, 0].SetStyle(style1);
                        cellSheet.Cells[i + 1, 1].PutValue(Math.Round(dcsz[i].到报率, 2));
                        cellSheet.Cells[i + 1, 1].SetStyle(style1);
                        cellSheet.Cells[i + 1, 2].PutValue(Math.Round(dcsz[i].缺报率, 2));
                        cellSheet.Cells[i + 1, 2].SetStyle(style1);
                        cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].缺报日期);
                        cellSheet.Cells[i + 1, 3].SetStyle(style1);
                        cellSheet.Cells[i + 1, 4].PutValue(Math.Round(dcsz[i].逾期率, 2));
                        cellSheet.Cells[i + 1, 4].SetStyle(style1);
                        cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].逾期日期);
                        cellSheet.Cells[i + 1, 5].SetStyle(style1);
                    }

                    //cellSheet.AutoFitColumns();
                    int columnCount = cellSheet.Cells.MaxColumn; //获取表页的最大列数
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
                静态类.OpenBrowser(xlsPath);
            }

        }
    }

    public class MyViewModel : ViewModelBase
    {
        private ObservableCollection<乡镇精细化到报率ViewModel> clubs;

        public ObservableCollection<乡镇精细化到报率ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<乡镇精细化到报率ViewModel> CreateClubs()
        {
            ObservableCollection<乡镇精细化到报率ViewModel> clubs = new ObservableCollection<乡镇精细化到报率ViewModel>();

            // club = new 乡镇精细化到报率ViewModel("Liverpool", 11,33,"ces",22,"wfw");
            // clubs.Add(club);


            return clubs;
        }
    }
}