using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using sjzd.类;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.SplashScreen;
using GridViewColumn = Telerik.Windows.Controls.GridViewColumn;
using Style = Aspose.Cells.Style;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 区台新方法表格 : UserControl
    {
        private string idStr = "53368,53464,53466,53467,53469,53562,53463";
        private List<mysql数据库类.区台温度> myTem = new List<mysql数据库类.区台温度>();
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private ObservableCollection<区台新方法温度预报表ViewModel> 查询列表 = new My区台新方法表格ViewModel().Clubs;
        private int sc = 8;
        public 区台新方法表格()
        {
            InitializeComponent();
            BTLabel.Content = "最高、最低气温查询";
            GRPFList.ItemsSource = 查询列表;


            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";

            Thread thread = new Thread(时间初始化);
            thread.Start();
        }
        public 区台新方法表格(int mysc)
        {
            InitializeComponent();
            sc = mysc;
            BTLabel.Content = "最高、最低气温查询";
            GRPFList.ItemsSource = 查询列表;


            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            Thread thread = new Thread(时间初始化);
            thread.Start();
        }

        private void 时间初始化()
        {
            try
            {
                TimeSpan timeSpan = DateTime.Now.Date.AddHours(8) - DateTime.Now.Date;
                if (DateTime.Now.Hour > 12)
                    timeSpan = DateTime.Now.Date.AddHours(20) - DateTime.Now.Date;
                Dispatcher.Invoke(() =>
                {
                    SCSelect.SelectedTime = timeSpan;
                });
            }
            catch
            {
            }
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            try
            {
                查询列表.Clear();
                myTem.Clear();
                sc = SCSelect.SelectedTime.Value.Hours;

                mysql数据库类 mysqlClass = new mysql数据库类();
                String sxStr = sc.ToString().PadLeft(2, '0');
                DateTime qbDate = mysqlClass.获取区台新方法最新起报时间(sc);
                BTLabel.Content = "区台新方法" + qbDate.ToString("MM月dd日") + sxStr + "时起报最高、最低气温查询";
                if (qbDate > DateTime.MinValue)
                {
                    DateTime sDate = DateTime.Now.Date.AddHours(sc);
                    myTem = mysqlClass.根据区站号起报时间获取区台新方法温度(idStr, qbDate).Where(y => y.MyDate >= sDate).ToList();
                    if (myTem.Count > 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            DateTime myDate = sDate.AddDays(i);
                            List<mysql数据库类.区台温度> listsLS = myTem.Where(y => y.MyDate == myDate.AddDays(1)).ToList();
                            if (listsLS.Count == 0)
                            {
                                break;
                            }
                            List<实况查询详情ViewModel> 温度详情 = 处理区台新方法温度详情(myTem, myDate, myDate.AddDays(1));
                            查询列表.Add(new 区台新方法温度预报表ViewModel(myDate.AddDays(1), "低温", 判断低温是否存在("53368", listsLS), 判断低温是否存在("53464", listsLS), 判断低温是否存在("53466", listsLS), 判断低温是否存在("53467", listsLS), 判断低温是否存在("53469", listsLS), 判断低温是否存在("53562", listsLS), 判断低温是否存在("53463", listsLS))
                            {
                                详情 = 温度详情,
                                IsExpanded = true
                            });
                            查询列表.Add(new 区台新方法温度预报表ViewModel(myDate.AddDays(1), "高温", 判断高温是否存在("53368", listsLS), 判断高温是否存在("53464", listsLS), 判断高温是否存在("53466", listsLS), 判断高温是否存在("53467", listsLS), 判断高温是否存在("53469", listsLS), 判断高温是否存在("53562", listsLS), 判断高温是否存在("53463", listsLS))
                            {
                                IsExpanded = false
                            });
                        }
                        if (qbDate.Date == DateTime.Now.Date.AddDays(-1))
                        {
                            RadSplashScreenManager.Close();
                            RadWindow.Alert(new DialogParameters
                            {
                                Content = $"今天{sxStr}时起报的数据暂时还没有！！！\n使用昨天{sxStr}时起报的数据代替",
                                Header = "注意"
                            });
                        }
                    }
                    else
                    {
                        RadSplashScreenManager.Close();
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "暂无数据呦",
                            Header = "警告"
                        });
                    }
                }
                else
                {
                    RadSplashScreenManager.Close();
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = "暂无数据呦",
                        Header = "警告"
                    });
                }

            }
            catch
            {
            }
            finally
            {
                RadSplashScreenManager.Close();
            }
        }

        private void RadGridView_CellLoaded(object sender, CellEventArgs e)
        {
            if (e.Cell is GridViewCell)
            {
                GridViewColumn c1 = e.Cell.Column;
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
                var row = cell.ParentRow.Item as 区台新方法温度预报表ViewModel;
                GridViewColumn c1 = cell.Column;
                if (row != null && c1 != null)
                {
                    显示折线(c1.Header.ToString(), row.日期);
                }
            }
        }

        private void 显示折线(string id, DateTime eDate)
        {
            if (myTem.Count > 0)
            {
                List<mysql数据库类.区台温度> temLists = myTem.Where(y => y.MyDate.CompareTo(eDate) <= 0 && y.MyDate.CompareTo(eDate.AddDays(-1)) >= 0 && y.StationID == id).ToList();
                if (temLists.Count > 0)
                {
                    区台新方法温度折线 zx = new 区台新方法温度折线(temLists);
                    RadWindow radWindow = new RadWindow
                    {
                        Content = zx,
                        Header = id + "站" + eDate.ToString("M月d日H时过去24小时区台新方法温度变化曲线")
                    };
                    RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                    radWindow.Show();
                }
            }
        }


        private List<实况查询详情ViewModel> 处理区台新方法温度详情(List<mysql数据库类.区台温度> myTem, DateTime sDate, DateTime eDate)
        {
            try
            {
                List<实况查询详情ViewModel> mylists = new List<实况查询详情ViewModel>();
                List<mysql数据库类.区台温度> temLists = myTem.Where(y => y.MyDate.CompareTo(sDate) >= 0 && y.MyDate.CompareTo(eDate) <= 0).OrderBy(y => y.MyDate).ToList();
                List<DateTime> dateLists = new List<DateTime>();

                foreach (var item in temLists)
                {
                    if (!dateLists.Exists(y => y == item.MyDate))
                    {
                        dateLists.Add(item.MyDate);
                    }
                }

                foreach (var myDate in dateLists)
                {
                    var listsLS = temLists.Where(y => y.MyDate.CompareTo(myDate) == 0).ToList();
                    mylists.Add(new 实况查询详情ViewModel(myDate, 判断温度是否存在("53368", listsLS), 判断温度是否存在("53464", listsLS), 判断温度是否存在("53466", listsLS), 判断温度是否存在("53467", listsLS), 判断温度是否存在("53469", listsLS), 判断温度是否存在("53562", listsLS), 判断温度是否存在("53463", listsLS)));
                }

                return mylists;
            }
            catch (Exception e)
            {
            }

            return new List<实况查询详情ViewModel>();
        }

        private double 判断温度是否存在(String id, List<mysql数据库类.区台温度> temLists)
        {
            return temLists.Exists(y => y.StationID == id) ? temLists.First(y => y.StationID == id).TEM : -99999;
        }
        private double 判断高温是否存在(String id, List<mysql数据库类.区台温度> temLists)
        {
            return temLists.Exists(y => y.StationID == id) ? temLists.First(y => y.StationID == id).TMAX : -99999;
        }
        private double 判断低温是否存在(String id, List<mysql数据库类.区台温度> temLists)
        {
            return temLists.Exists(y => y.StationID == id) ? temLists.First(y => y.StationID == id).TMIN : -99999;
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
            RadWindow.Confirm(new DialogParameters
            {
                Content = "是否导出自定义表格",
                Closed = OnConfirmClosed_导出表格,
                CancelButtonContent = "否",
                Header = "提示",
                OkButtonContent = "是"
            });
        }
        private void OnConfirmClosed_导出表格(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                导出自定义表格();
            }
            else
            {
                导出查询表格();
            }

        }
        private void 导出自定义表格()
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xls";
                区台新方法温度预报表ViewModel[] dcsz = 查询列表.ToArray();
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
                    Style style2 = workbook.CreateStyle();
                    ;
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
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].日期.ToString("M月d日H时"));
                        cellSheet.Cells[i + 1, 0].SetStyle(style2);
                        cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].类型);
                        cellSheet.Cells[i + 1, 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 1 + 1].PutValue(Math.Round(dcsz[i].值53368, 2));
                        cellSheet.Cells[i + 1, 1 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 2 + 1].PutValue(Math.Round(dcsz[i].值53464, 2));
                        cellSheet.Cells[i + 1, 2 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 3 + 1].PutValue(Math.Round(dcsz[i].值53466, 2));
                        cellSheet.Cells[i + 1, 3 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 4 + 1].PutValue(Math.Round(dcsz[i].值53467, 2));
                        cellSheet.Cells[i + 1, 4 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 5 + 1].PutValue(Math.Round(dcsz[i].值53469, 2));
                        cellSheet.Cells[i + 1, 5 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 6 + 1].PutValue(Math.Round(dcsz[i].值53562, 2));
                        cellSheet.Cells[i + 1, 6 + 1].SetStyle(style2);
                        cellSheet.Cells[i + 1, 7 + 1].PutValue(Math.Round(dcsz[i].值53463, 2));
                        cellSheet.Cells[i + 1, 7 + 1].SetStyle(style2);

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

        private void 导出查询表格()
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xlsx";

                try
                {
                    ;
                    using (Stream stream = File.Create(strPath))
                    {
                        GRPFList.ExportToXlsx(stream, new GridViewDocumentExportOptions
                        {
                            ShowColumnHeaders = true,
                            ShowColumnFooters = true,
                            ShowGroupFooters = true,
                            ShowGroupRows = true,
                            ExportDefaultStyles = true,
                            AutoFitColumnsWidth = true,
                            ShowGroupHeaderRowAggregates = true
                        });
                    }

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

    public class My区台新方法表格ViewModel : ViewModelBase
    {
        private ObservableCollection<区台新方法温度预报表ViewModel> clubs;

        public ObservableCollection<区台新方法温度预报表ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<区台新方法温度预报表ViewModel> CreateClubs()
        {
            ObservableCollection<区台新方法温度预报表ViewModel> clubs = new ObservableCollection<区台新方法温度预报表ViewModel>();
            return clubs;
        }
    }
}