using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.SplashScreen;
using Style = Aspose.Cells.Style;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 实况查询 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private ObservableCollection<实况查询ViewModel> 查询列表 = new My实况查询ViewModel().Clubs;
        private List<CIMISS.YS> mySKTem = new List<CIMISS.YS>();
        string idStr = "53368,53464,53466,53467,53469,53562,53463";
        public 实况查询()
        {
            InitializeComponent();
            BTLabel.Content = "最高、最低气温查询";
            GRPFList.ItemsSource = 查询列表;
            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";

        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            mySKTem.Clear();
            if (!(sDate.SelectedDate.ToString().Length == 0) && !(eDate.SelectedDate.ToString().Length == 0) && !(SCSelect.SelectedDate.ToString().Length == 0))
            {
                查询列表.Clear();
                DateTime sDateTime = Convert.ToDateTime(sDate.SelectedDate);
                DateTime eDateTime = Convert.ToDateTime(eDate.SelectedDate);
                string startDate = sDateTime.ToString("yyyy年MM月dd日");
                string endDate = eDateTime.ToString("yyyy年MM月dd日");
                int myHour = SCSelect.SelectedTime.Value.Hours;
                BTLabel.Content = startDate + "至" + endDate + myHour + "时最高、最低气温查询";

                for (DateTime lsDateTime = sDateTime; lsDateTime.CompareTo(eDateTime) <= 0; lsDateTime = lsDateTime.AddDays(1))
                {
                    CIMISS cIMISS = new CIMISS();

                    List<CIMISS.YS> myTem = cIMISS.获取小时温度(lsDateTime.Date.AddDays(-1).AddHours(myHour + 1), lsDateTime.AddHours(myHour), idStr);
                    mySKTem.AddRange(myTem);
                    List<SKList> sKLists = new List<SKList>();
                    foreach (string item in idStr.Split(','))
                    {
                        try
                        {
                            SKList sKList = 处理实况(myTem, item);
                            if (sKList.ID.Length != 0)
                            {
                                sKLists.Add(sKList);
                            }
                        }
                        catch
                        {
                        }
                    }

                    try
                    {
                        查询列表.Add(new 实况查询ViewModel(lsDateTime.AddHours(myHour), "低温", sKLists.First(y => y.ID == "53368").Tmin, sKLists.First(y => y.ID == "53464").Tmin, sKLists.First(y => y.ID == "53466").Tmin, sKLists.First(y => y.ID == "53467").Tmin, sKLists.First(y => y.ID == "53469").Tmin, sKLists.First(y => y.ID == "53562").Tmin, sKLists.First(y => y.ID == "53463").Tmin, sKLists.First(y => y.ID == "53368").低温时间, sKLists.First(y => y.ID == "53464").低温时间, sKLists.First(y => y.ID == "53466").低温时间, sKLists.First(y => y.ID == "53467").低温时间, sKLists.First(y => y.ID == "53469").低温时间, sKLists.First(y => y.ID == "53562").低温时间, sKLists.First(y => y.ID == "53463").低温时间));
                        查询列表.Add(new 实况查询ViewModel(lsDateTime.AddHours(myHour), "高温", sKLists.First(y => y.ID == "53368").Tmax, sKLists.First(y => y.ID == "53464").Tmax, sKLists.First(y => y.ID == "53466").Tmax, sKLists.First(y => y.ID == "53467").Tmax, sKLists.First(y => y.ID == "53469").Tmax, sKLists.First(y => y.ID == "53562").Tmax, sKLists.First(y => y.ID == "53463").Tmax, sKLists.First(y => y.ID == "53368").高温时间, sKLists.First(y => y.ID == "53464").高温时间, sKLists.First(y => y.ID == "53466").高温时间, sKLists.First(y => y.ID == "53467").高温时间, sKLists.First(y => y.ID == "53469").高温时间, sKLists.First(y => y.ID == "53562").高温时间, sKLists.First(y => y.ID == "53463").高温时间));
                    }
                    catch
                    {
                    }
                }
                RadSplashScreenManager.Close();
            }
            else
            {
                RadSplashScreenManager.Close();
                RadWindow.Alert(new DialogParameters
                {
                    Content = "请选择起止时间",
                    Header = "警告"
                });
            }


        }
        private void RadGridView_CellLoaded(object sender, CellEventArgs e)
        {
            if (e.Cell is GridViewCell)
            {
                Telerik.Windows.Controls.GridViewColumn c1 = e.Cell.Column;
                if (c1 != null)
                {
                    string header = c1.Header.ToString();

                    foreach (string ss in idStr.Split(','))
                    {
                        if (header.Contains(ss))
                        {
                            e.Cell.AddHandler(MouseDoubleClickEvent, new MouseButtonEventHandler(OnCellMouseDoubleClickEvent), true);
                        }
                    }
                }
            }
        }
        private void OnCellMouseDoubleClickEvent(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as GridViewCell;

            if (cell != null)
            {
                var row = cell.ParentRow.Item as 实况查询ViewModel;
                Telerik.Windows.Controls.GridViewColumn c1 = cell.Column;
                if (row != null && c1 != null)
                {
                    显示实况折线(c1.Header.ToString(), row.日期, row.类型);
                }
            }
        }
        private void 显示实况折线(string id, DateTime eDate, string 类型)
        {
            if (mySKTem.Count > 0)
            {
                List<CIMISS.YS> mysk = mySKTem.Where(y => y.DateTime.CompareTo(eDate) <= 0 && y.DateTime.CompareTo(eDate.AddDays(-1)) > 0 && y.StationID == id).ToList();
                List<CIMISS.ECTEF0> temLists = new List<CIMISS.ECTEF0>();
                if (类型 == "高温")
                {
                    foreach (var item in mysk)
                    {
                        temLists.Add(new CIMISS.ECTEF0()
                        {
                            StationID = item.StationID,
                            DateTime = item.DateTime,
                            TEM = item.TEM_Max
                        });
                    }
                }
                else
                {
                    foreach (var item in mysk)
                    {
                        temLists.Add(new CIMISS.ECTEF0()
                        {
                            StationID = item.StationID,
                            DateTime = item.DateTime,
                            TEM = item.TEM_Min
                        });
                    }
                }
                if (temLists.Count > 0)
                {
                    实况温度折线图 zx = new 实况温度折线图(temLists);
                    RadWindow radWindow = new RadWindow
                    {
                        Content = zx,
                        Header = id + "站" + eDate.ToString($"M月d日H时过去24小时{类型}变化曲线"),
                        WindowStartupLocation = WindowStartupLocation.CenterScreen
                    };
                    RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                    radWindow.Show();
                }
            }
        }
        private SKList 处理实况(List<CIMISS.YS> myTem, string ID)
        {
            try
            {
                float tMin = myTem.Where(y => y.StationID == ID).OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                DateTime timeMin = myTem.First(y => y.TEM_Min == tMin).TEM_Min_OTime;
                float tMax = myTem.Where(y => y.StationID == ID).OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
                DateTime timeMax = myTem.First(y => y.TEM_Max == tMax).TEM_Max_OTime;
                return new SKList
                {
                    ID = ID,
                    Tmin = Math.Round(tMin, 1),
                    Tmax = Math.Round(tMax, 1),
                    高温时间 = timeMax,
                    低温时间 = timeMin
                };
            }
            catch
            {
            }

            return new SKList
            {
                ID = ""
            };
        }
        private List<实况查询详情ViewModel> 处理实况温度详情(List<CIMISS.ECTEF0> myTem, DateTime sDate, DateTime eDate)
        {
            try
            {
                List<实况查询详情ViewModel> mylists = new List<实况查询详情ViewModel>();
                List<CIMISS.ECTEF0> temLists = myTem.Where(y => y.DateTime.CompareTo(sDate) >= 0 && y.DateTime.CompareTo(eDate) <= 0).OrderBy(y => y.DateTime).ToList();
                List<DateTime> dateLists = new List<DateTime>();

                foreach (var item in temLists)
                {
                    if (!dateLists.Exists(y => y == item.DateTime))
                    {
                        dateLists.Add(item.DateTime);
                    }
                }

                foreach (var myDate in dateLists)
                {
                    var listsLS = temLists.Where(y => y.DateTime == myDate);
                    mylists.Add(new 实况查询详情ViewModel(myDate, listsLS.First(y => y.StationID == "53368").TEM, listsLS.First(y => y.StationID == "53464").TEM, listsLS.First(y => y.StationID == "53466").TEM, listsLS.First(y => y.StationID == "53467").TEM, listsLS.First(y => y.StationID == "53469").TEM, listsLS.First(y => y.StationID == "53562").TEM, listsLS.First(y => y.StationID == "53463").TEM));
                }

                return mylists;
            }
            catch
            {
            }

            return new List<实况查询详情ViewModel>();
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

                eDate.SelectedDate = dt1.AddDays(1);
            }
            catch
            {
            }
        }

        private void OnConfirmClosed_打开产品(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {

                静态类.OpenBrowser(xlsPath);
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
                实况查询ViewModel[] dcsz = 查询列表.ToArray();
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
                    Style style1 = workbook.CreateStyle(); //新增样式  
                    style1.HorizontalAlignment = TextAlignmentType.Center; //文字居中
                    style1.VerticalAlignment = TextAlignmentType.Center;
                    style1.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style1.Borders[BorderType.TopBorder].Color = Color.Black;
                    style1.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style1.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style1.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style1.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style1.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style1.Borders[BorderType.RightBorder].Color = Color.Black;
                    style1.Font.IsBold = false;
                    Style style2 = workbook.CreateStyle(); ;
                    style2.HorizontalAlignment = TextAlignmentType.Center; //文字居中
                    style2.VerticalAlignment = TextAlignmentType.Center;
                    style2.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
                    style2.Borders[BorderType.TopBorder].Color = Color.Black;
                    style2.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
                    style2.Borders[BorderType.BottomBorder].Color = Color.Black;
                    style2.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
                    style2.Borders[BorderType.LeftBorder].Color = Color.Black;
                    style2.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
                    style2.Borders[BorderType.RightBorder].Color = Color.Black;
                    style2.Font.IsBold = true;
                    cellSheet.Cells[0, 1].PutValue("类型");
                    cellSheet.Cells[0, 2].PutValue("53368");
                    cellSheet.Cells[0, 3].PutValue("53464");
                    cellSheet.Cells[0, 4].PutValue("53466");
                    cellSheet.Cells[0, 5].PutValue("53467");
                    cellSheet.Cells[0, 6].PutValue("53469");
                    cellSheet.Cells[0, 7].PutValue("53562");
                    cellSheet.Cells[0, 8].PutValue("53463");
                    for (int i = 0; i < dcsz.Length; i++)
                    {
                        cellSheet.Cells[i * 2 + 1, 0].PutValue(dcsz[i].日期.ToString("M月d日H时"));
                        cellSheet.Cells[i * 2 + 1, 0].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 0].PutValue("出现时间");
                        cellSheet.Cells[i * 2 + 2, 0].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 1].PutValue(dcsz[i].类型);
                        cellSheet.Cells[i * 2 + 1, 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 1].PutValue(dcsz[i].类型);
                        cellSheet.Cells[i * 2 + 2, 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 1 + 1].PutValue(Math.Round(dcsz[i].值53368, 2));
                        cellSheet.Cells[i * 2 + 1, 1 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 1 + 1].PutValue(dcsz[i].时间53368.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 1 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 2 + 1].PutValue(Math.Round(dcsz[i].值53464, 2));
                        cellSheet.Cells[i * 2 + 1, 2 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 2 + 1].PutValue(dcsz[i].时间53464.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 2 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 3 + 1].PutValue(Math.Round(dcsz[i].值53466, 2));
                        cellSheet.Cells[i * 2 + 1, 3 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 3 + 1].PutValue(dcsz[i].时间53466.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 3 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 4 + 1].PutValue(Math.Round(dcsz[i].值53467, 2));
                        cellSheet.Cells[i * 2 + 1, 4 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 4 + 1].PutValue(dcsz[i].时间53467.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 4 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 5 + 1].PutValue(Math.Round(dcsz[i].值53469, 2));
                        cellSheet.Cells[i * 2 + 1, 5 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 5 + 1].PutValue(dcsz[i].时间53469.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 5 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 6 + 1].PutValue(Math.Round(dcsz[i].值53562, 2));
                        cellSheet.Cells[i * 2 + 1, 6 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 6 + 1].PutValue(dcsz[i].时间53562.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 6 + 1].SetStyle(style1);
                        cellSheet.Cells[i * 2 + 1, 7 + 1].PutValue(Math.Round(dcsz[i].值53463, 2));
                        cellSheet.Cells[i * 2 + 1, 7 + 1].SetStyle(style2);
                        cellSheet.Cells[i * 2 + 2, 7 + 1].PutValue(dcsz[i].时间53463.ToString("d日H时"));
                        cellSheet.Cells[i * 2 + 2, 7 + 1].SetStyle(style1);
                    }

                    //cellSheet.AutoFitColumns();
                    int columnCount = cellSheet.Cells.MaxColumn; //获取表页的最大列数
                    cellSheet.AutoFitColumns();
                    for (int col = 0; col < columnCount + 1; col++)
                    {
                        cellSheet.Cells[0, col].SetStyle(style2);
                        cellSheet.Cells.SetColumnWidthPixel(col, cellSheet.Cells.GetColumnWidthPixel(col) + 11);
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

        public class SKList
        {
            public string ID { get; set; }
            public double Tmax { get; set; }
            public DateTime 高温时间 { get; set; }
            public double Tmin { get; set; }
            public DateTime 低温时间 { get; set; }
        }
    }

    public class My实况查询ViewModel : ViewModelBase
    {
        private ObservableCollection<实况查询ViewModel> clubs;

        public ObservableCollection<实况查询ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<实况查询ViewModel> CreateClubs()
        {
            ObservableCollection<实况查询ViewModel> clubs = new ObservableCollection<实况查询ViewModel>();
            return clubs;
        }
    }
}