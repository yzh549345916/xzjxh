using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using sjzd.类;
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
    public partial class EC温度08表格 : UserControl
    {
        private string idStr = "53368,53464,53466,53467,53469,53562,53463";
        private List<CIMISS.ECTEF0> myTem = new List<CIMISS.ECTEF0>();
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private ObservableCollection<实况查询ViewModel> 查询列表 = new My实况查询ViewModel().Clubs;
        private int sc = 8;
        public EC温度08表格()
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
        public EC温度08表格(int mysc)
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
                mysql环境气象 mysqlHJ = new mysql环境气象();
                DateTime dateTime = mysqlHJ.获取EC地面最新时次();;
                TimeSpan timeSpan = DateTime.Now.Date.AddHours(8) - DateTime.Now.Date;
                if (DateTime.Now.Hour > 12)
                    timeSpan = DateTime.Now.Date.AddHours(20) - DateTime.Now.Date;
                if (dateTime.CompareTo(DateTime.Now.AddDays(-2)) > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        sDate.SelectedDate = dateTime.Date;
                        SCSelect.SelectedTime = dateTime - dateTime.Date;
                        YBSCSelect.SelectedTime = timeSpan;
                    });
                }
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
                if (YBSCSelect.SelectedTime != null && YBSCSelect.SelectedTime.Value.Hours != sc)
                    sc = YBSCSelect.SelectedTime.Value.Hours;
            }
            catch
            {
            }

            if (sDate.SelectedDate.ToString().Length != 0 && SCSelect.SelectedDate.ToString().Length != 0)
            {
                try
                {
                    查询列表.Clear();
                    myTem.Clear();
                    DateTime sDateTime = Convert.ToDateTime(sDate.SelectedDate);
                    string startDate = sDateTime.ToString("yyyy年MM月dd日");
                    int myHour = SCSelect.SelectedTime.Value.Hours;
                    BTLabel.Content = "EC" + startDate + myHour + "时起报最高、最低气温查询";
                    List<DateTime> errorDateList = new List<DateTime>();
                    mysql环境气象 mysqlHJ = new mysql环境气象();
                    myTem = mysqlHJ.根据区站号起报时间获取EC温度(idStr, sDateTime.AddHours(myHour));



                    if (myTem.Count > 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            DateTime myDate = DateTime.Now.Date.AddHours(sc).AddDays(i);
                            List<实况查询详情ViewModel> 温度详情 = 处理EC温度详情(myTem, myDate, myDate.AddDays(1));
                            List<SKList> sKLists = new List<SKList>();
                            foreach (string item in idStr.Split(','))
                            {
                                try
                                {
                                    SKList sKList = 处理EC(myTem, item, myDate, myDate.AddDays(1), ref errorDateList);
                                    if (sKList.ID.Length != 0)
                                    {
                                        sKLists.Add(sKList);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }


                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "低温", sKLists.First(y => y.ID == "53368").Tmin, sKLists.First(y => y.ID == "53464").Tmin, sKLists.First(y => y.ID == "53466").Tmin, sKLists.First(y => y.ID == "53467").Tmin, sKLists.First(y => y.ID == "53469").Tmin, sKLists.First(y => y.ID == "53562").Tmin, sKLists.First(y => y.ID == "53463").Tmin, sKLists.First(y => y.ID == "53368").低温时间, sKLists.First(y => y.ID == "53464").低温时间, sKLists.First(y => y.ID == "53466").低温时间, sKLists.First(y => y.ID == "53467").低温时间, sKLists.First(y => y.ID == "53469").低温时间, sKLists.First(y => y.ID == "53562").低温时间, sKLists.First(y => y.ID == "53463").低温时间)
                            {
                                详情 = 温度详情,
                                IsExpanded = true
                            });
                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "高温", sKLists.First(y => y.ID == "53368").Tmax, sKLists.First(y => y.ID == "53464").Tmax, sKLists.First(y => y.ID == "53466").Tmax, sKLists.First(y => y.ID == "53467").Tmax, sKLists.First(y => y.ID == "53469").Tmax, sKLists.First(y => y.ID == "53562").Tmax, sKLists.First(y => y.ID == "53463").Tmax, sKLists.First(y => y.ID == "53368").高温时间, sKLists.First(y => y.ID == "53464").高温时间, sKLists.First(y => y.ID == "53466").高温时间, sKLists.First(y => y.ID == "53467").高温时间, sKLists.First(y => y.ID == "53469").高温时间, sKLists.First(y => y.ID == "53562").高温时间, sKLists.First(y => y.ID == "53463").高温时间)
                            {
                                IsExpanded = false
                            });
                        }

                        if (errorDateList.Count > 0)
                        {
                            RadSplashScreenManager.Close();
                            string error = "该起报时间的缺报数缺少较多，请注意是否需要更换起报时间。\r\n具体缺报的时间有:\r\n";
                            foreach (var item in errorDateList)
                            {
                                error += item.ToString("M月d日H时") + "\r\n";
                            }

                            ScrollViewer scrollViewer = new ScrollViewer();
                            scrollViewer.Content = error;
                            RadWindow radWindow = new RadWindow
                            {
                                Content = scrollViewer,
                                ResizeMode = ResizeMode.CanResize,
                                MaxHeight = 500,
                                MinWidth = 400,
                                Header = "警告",
                                ShouldUpdateActiveState = false
                            };
                            RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                            radWindow.Show();
                            return;
                        }
                    }
                    else
                    {
                        RadSplashScreenManager.Close();
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "所选起报时间的预报不存在",
                            Header = "警告"
                        });
                        return;
                    }
                }
                catch (Exception)
                {
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
                var row = cell.ParentRow.Item as 实况查询ViewModel;
                GridViewColumn c1 = cell.Column;
                if (row != null && c1 != null)
                {
                    显示EC折线(c1.Header.ToString(), row.日期);
                }
            }
        }

        private void 显示EC折线(string id, DateTime eDate)
        {
            if (myTem.Count > 0)
            {
                List<CIMISS.ECTEF0> temLists = myTem.Where(y => y.DateTime.CompareTo(eDate) <= 0 && y.DateTime.CompareTo(eDate.AddDays(-1)) >= 0 && y.StationID == id).ToList();
                if (temLists.Count > 0)
                {
                    EC温度折线图 zx = new EC温度折线图(temLists);
                    RadWindow radWindow = new RadWindow
                    {
                        Content = zx,
                        Header = id + "站" + eDate.ToString("M月d日H时过去24小时EC温度变化曲线")
                    };
                    RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                    radWindow.Show();
                }
            }
        }

        private SKList 处理EC(List<CIMISS.ECTEF0> myTem, string ID, DateTime sDate, DateTime eDate, ref List<DateTime> error)
        {
            try
            {
                List<CIMISS.ECTEF0> temLists = myTem.Where(y => y.DateTime.CompareTo(sDate) >= 0 && y.DateTime.CompareTo(eDate) <= 0 && y.StationID == ID).OrderBy(y => y.TEM).ToList();
                if (temLists.Count < 5)
                {
                    for (DateTime dateTimeLS = sDate; dateTimeLS.CompareTo(eDate) <= 0; dateTimeLS = dateTimeLS.AddHours(6))
                    {
                        if (!temLists.Exists(y => y.DateTime == dateTimeLS) && !error.Exists(y => y == dateTimeLS))
                        {
                            error.Add(dateTimeLS);
                        }
                    }
                }

                return new SKList
                {
                    ID = ID,
                    Tmin = Math.Round(temLists[0].TEM, 1),
                    Tmax = Math.Round(temLists[temLists.Count - 1].TEM, 1),
                    高温时间 = temLists[temLists.Count - 1].DateTime,
                    低温时间 = temLists[0].DateTime
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

        private List<实况查询详情ViewModel> 处理EC温度详情(List<CIMISS.ECTEF0> myTem, DateTime sDate, DateTime eDate)
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

        private void DCButton2_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            try
            {
                if (YBSCSelect.SelectedTime != null && YBSCSelect.SelectedTime.Value.Hours != sc)
                    sc = YBSCSelect.SelectedTime.Value.Hours;
            }
            catch
            {
            }
            if (!(sDate.SelectedDate.ToString().Length == 0) && !(SCSelect.SelectedDate.ToString().Length == 0))
            {
                try
                {
                    查询列表.Clear();
                    myTem.Clear();
                    DateTime sDateTime = Convert.ToDateTime(sDate.SelectedDate);
                    string startDate = sDateTime.ToString("yyyy年MM月dd日");
                    int myHour = SCSelect.SelectedTime.Value.Hours;
                    BTLabel.Content = "EC" + startDate + myHour + "时起报最高、最低气温查询";
                    List<DateTime> errorDateList = new List<DateTime>();
                    CIMISS cIMISS = new CIMISS();
                    string LSPath = Environment.CurrentDirectory + "\\临时\\";
                    if (!Directory.Exists(LSPath))
                        Directory.CreateDirectory(LSPath);

                    #region 删除14天前的临时文件

                    DirectoryInfo root = new DirectoryInfo(LSPath);
                    FileInfo[] files = root.GetFiles();
                    foreach (FileInfo fileInfo in files)
                    {
                        if (fileInfo.CreationTime.CompareTo(DateTime.Now.AddDays(-14)) <= 0)
                            fileInfo.Delete();
                    }

                    #endregion

                    LSPath = LSPath + $"{startDate + myHour}时EC高低温临时.txt";
                    if (!File.Exists(LSPath))
                    {
                        List<CIMISS.ECTEF0> myTem2 = cIMISS.获取EC温度(sDateTime.AddHours(myHour), idStr);
                        string strData = "";
                        foreach (var item in myTem2)
                        {
                            strData += $"{item.DateTime}\t{item.StationID}\t{item.TEM}\r\n";
                        }

                        using (StreamWriter sw = new StreamWriter(LSPath, false, Encoding.GetEncoding("GB2312")))
                        {
                            sw.Write(strData);
                        }
                    }

                    using (StreamReader sr = new StreamReader(LSPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            try
                            {
                                myTem.Add(new CIMISS.ECTEF0
                                {
                                    DateTime = Convert.ToDateTime(line.Split('\t')[0]),
                                    StationID = Convert.ToString(line.Split('\t')[1]),
                                    TEM = Convert.ToDouble(line.Split('\t')[2])
                                });
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (myTem.Count > 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            DateTime myDate = DateTime.Now.Date.AddHours(sc).AddDays(i);
                            List<实况查询详情ViewModel> 温度详情 = 处理EC温度详情(myTem, myDate, myDate.AddDays(1));
                            List<SKList> sKLists = new List<SKList>();
                            foreach (string item in idStr.Split(','))
                            {
                                try
                                {
                                    SKList sKList = 处理EC(myTem, item, myDate, myDate.AddDays(1), ref errorDateList);
                                    if (sKList.ID.Length != 0)
                                    {
                                        sKLists.Add(sKList);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }


                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "低温", sKLists.First(y => y.ID == "53368").Tmin, sKLists.First(y => y.ID == "53464").Tmin, sKLists.First(y => y.ID == "53466").Tmin, sKLists.First(y => y.ID == "53467").Tmin, sKLists.First(y => y.ID == "53469").Tmin, sKLists.First(y => y.ID == "53562").Tmin, sKLists.First(y => y.ID == "53463").Tmin, sKLists.First(y => y.ID == "53368").低温时间, sKLists.First(y => y.ID == "53464").低温时间, sKLists.First(y => y.ID == "53466").低温时间, sKLists.First(y => y.ID == "53467").低温时间, sKLists.First(y => y.ID == "53469").低温时间, sKLists.First(y => y.ID == "53562").低温时间, sKLists.First(y => y.ID == "53463").低温时间)
                            {
                                详情 = 温度详情,
                                IsExpanded = true
                            });
                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "高温", sKLists.First(y => y.ID == "53368").Tmax, sKLists.First(y => y.ID == "53464").Tmax, sKLists.First(y => y.ID == "53466").Tmax, sKLists.First(y => y.ID == "53467").Tmax, sKLists.First(y => y.ID == "53469").Tmax, sKLists.First(y => y.ID == "53562").Tmax, sKLists.First(y => y.ID == "53463").Tmax, sKLists.First(y => y.ID == "53368").高温时间, sKLists.First(y => y.ID == "53464").高温时间, sKLists.First(y => y.ID == "53466").高温时间, sKLists.First(y => y.ID == "53467").高温时间, sKLists.First(y => y.ID == "53469").高温时间, sKLists.First(y => y.ID == "53562").高温时间, sKLists.First(y => y.ID == "53463").高温时间)
                            {
                                IsExpanded = false
                            });
                        }
                        List<CIMISS.YS> mySK = cIMISS.获取小时温度(sDateTime, DateTime.Now, idStr);
                        EC温度折线图_站点 zx = new EC温度折线图_站点(myTem, mySK, idStr, sc);
                        RadWindow radWindow2 = new RadWindow
                        {
                            Content = zx,
                            Header = sDateTime.AddHours(myHour).ToString("M月d日H时起报EC温度变化曲线"),
                            WindowStartupLocation = WindowStartupLocation.CenterScreen
                        };
                        RadWindowInteropHelper.SetShowInTaskbar(radWindow2, true);
                        radWindow2.Show();


                        if (errorDateList.Count > 0)
                        {
                            RadSplashScreenManager.Close();
                            string error = "该起报时间的缺报数缺少较多，请注意是否需要更换起报时间。\r\n具体缺报的时间有:\r\n";
                            foreach (var item in errorDateList)
                            {
                                error += item.ToString("M月d日H时") + "\r\n";
                            }

                            ScrollViewer scrollViewer = new ScrollViewer();
                            scrollViewer.Content = error;
                            RadWindow radWindow = new RadWindow
                            {
                                Content = scrollViewer,
                                ResizeMode = ResizeMode.CanResize,
                                MaxHeight = 500,
                                MinWidth = 400,
                                Header = "警告",
                                ShouldUpdateActiveState = false
                            };
                            RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                            radWindow.Show();
                            return;
                        }


                    }
                    else
                    {
                        RadSplashScreenManager.Close();
                        File.Delete(LSPath);
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "所选起报时间的预报不存在",
                            Header = "警告"
                        });
                        return;
                    }
                }
                catch (Exception)
                {
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

        public class SKList
        {
            public string ID { get; set; }
            public double Tmax { get; set; }
            public DateTime 高温时间 { get; set; }
            public double Tmin { get; set; }
            public DateTime 低温时间 { get; set; }
        }

        #region 旧的备份
        private void 时间初始化old()
        {
            try
            {
                CIMISS cIMISS = new CIMISS();
                DateTime dateTime = cIMISS.获取EC2米温度最新时间();
                TimeSpan timeSpan = DateTime.Now.Date.AddHours(8) - DateTime.Now.Date;
                if (DateTime.Now.Hour > 12)
                    timeSpan = DateTime.Now.Date.AddHours(20) - DateTime.Now.Date;
                if (dateTime.CompareTo(DateTime.Now.AddDays(-2)) > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        sDate.SelectedDate = dateTime.Date;
                        SCSelect.SelectedTime = dateTime - dateTime.Date;
                        YBSCSelect.SelectedTime = timeSpan;
                    });
                }
            }
            catch
            {
            }
        }
        private void CXButton_Clickold(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            try
            {
                if (YBSCSelect.SelectedTime != null && YBSCSelect.SelectedTime.Value.Hours != sc)
                    sc = YBSCSelect.SelectedTime.Value.Hours;
            }
            catch
            {
            }

            if (sDate.SelectedDate.ToString().Length != 0 && SCSelect.SelectedDate.ToString().Length != 0)
            {
                try
                {
                    查询列表.Clear();
                    myTem.Clear();
                    DateTime sDateTime = Convert.ToDateTime(sDate.SelectedDate);
                    string startDate = sDateTime.ToString("yyyy年MM月dd日");
                    int myHour = SCSelect.SelectedTime.Value.Hours;
                    BTLabel.Content = "EC" + startDate + myHour + "时起报最高、最低气温查询";
                    List<DateTime> errorDateList = new List<DateTime>();
                    CIMISS cIMISS = new CIMISS();
                    string LSPath = Environment.CurrentDirectory + "\\临时\\";
                    if (!Directory.Exists(LSPath))
                        Directory.CreateDirectory(LSPath);

                    #region 删除14天前的临时文件

                    DirectoryInfo root = new DirectoryInfo(LSPath);
                    FileInfo[] files = root.GetFiles();
                    foreach (FileInfo fileInfo in files)
                    {
                        if (fileInfo.CreationTime.CompareTo(DateTime.Now.AddDays(-14)) <= 0)
                            fileInfo.Delete();
                    }

                    #endregion

                    LSPath = LSPath + $"{startDate + myHour}时EC高低温临时.txt";
                    if (!File.Exists(LSPath))
                    {
                        List<CIMISS.ECTEF0> myTem2 = cIMISS.获取EC温度(sDateTime.AddHours(myHour), idStr);
                        string strData = "";
                        foreach (var item in myTem2)
                        {
                            strData += $"{item.DateTime}\t{item.StationID}\t{item.TEM}\r\n";
                        }

                        using (StreamWriter sw = new StreamWriter(LSPath, false, Encoding.GetEncoding("GB2312")))
                        {
                            sw.Write(strData);
                        }
                    }

                    using (StreamReader sr = new StreamReader(LSPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            try
                            {
                                myTem.Add(new CIMISS.ECTEF0
                                {
                                    DateTime = Convert.ToDateTime(line.Split('\t')[0]),
                                    StationID = Convert.ToString(line.Split('\t')[1]),
                                    TEM = Convert.ToDouble(line.Split('\t')[2])
                                });
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (myTem.Count > 0)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            DateTime myDate = DateTime.Now.Date.AddHours(sc).AddDays(i);
                            List<实况查询详情ViewModel> 温度详情 = 处理EC温度详情(myTem, myDate, myDate.AddDays(1));
                            List<SKList> sKLists = new List<SKList>();
                            foreach (string item in idStr.Split(','))
                            {
                                try
                                {
                                    SKList sKList = 处理EC(myTem, item, myDate, myDate.AddDays(1), ref errorDateList);
                                    if (sKList.ID.Length != 0)
                                    {
                                        sKLists.Add(sKList);
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }


                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "低温", sKLists.First(y => y.ID == "53368").Tmin, sKLists.First(y => y.ID == "53464").Tmin, sKLists.First(y => y.ID == "53466").Tmin, sKLists.First(y => y.ID == "53467").Tmin, sKLists.First(y => y.ID == "53469").Tmin, sKLists.First(y => y.ID == "53562").Tmin, sKLists.First(y => y.ID == "53463").Tmin, sKLists.First(y => y.ID == "53368").低温时间, sKLists.First(y => y.ID == "53464").低温时间, sKLists.First(y => y.ID == "53466").低温时间, sKLists.First(y => y.ID == "53467").低温时间, sKLists.First(y => y.ID == "53469").低温时间, sKLists.First(y => y.ID == "53562").低温时间, sKLists.First(y => y.ID == "53463").低温时间)
                            {
                                详情 = 温度详情,
                                IsExpanded = true
                            });
                            查询列表.Add(new 实况查询ViewModel(myDate.AddDays(1), "高温", sKLists.First(y => y.ID == "53368").Tmax, sKLists.First(y => y.ID == "53464").Tmax, sKLists.First(y => y.ID == "53466").Tmax, sKLists.First(y => y.ID == "53467").Tmax, sKLists.First(y => y.ID == "53469").Tmax, sKLists.First(y => y.ID == "53562").Tmax, sKLists.First(y => y.ID == "53463").Tmax, sKLists.First(y => y.ID == "53368").高温时间, sKLists.First(y => y.ID == "53464").高温时间, sKLists.First(y => y.ID == "53466").高温时间, sKLists.First(y => y.ID == "53467").高温时间, sKLists.First(y => y.ID == "53469").高温时间, sKLists.First(y => y.ID == "53562").高温时间, sKLists.First(y => y.ID == "53463").高温时间)
                            {
                                IsExpanded = false
                            });
                        }

                        if (errorDateList.Count > 0)
                        {
                            RadSplashScreenManager.Close();
                            string error = "该起报时间的缺报数缺少较多，请注意是否需要更换起报时间。\r\n具体缺报的时间有:\r\n";
                            foreach (var item in errorDateList)
                            {
                                error += item.ToString("M月d日H时") + "\r\n";
                            }

                            ScrollViewer scrollViewer = new ScrollViewer();
                            scrollViewer.Content = error;
                            RadWindow radWindow = new RadWindow
                            {
                                Content = scrollViewer,
                                ResizeMode = ResizeMode.CanResize,
                                MaxHeight = 500,
                                MinWidth = 400,
                                Header = "警告",
                                ShouldUpdateActiveState = false
                            };
                            RadWindowInteropHelper.SetShowInTaskbar(radWindow, true);
                            radWindow.Show();
                            return;
                        }
                    }
                    else
                    {
                        RadSplashScreenManager.Close();
                        File.Delete(LSPath);
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "所选起报时间的预报不存在",
                            Header = "警告"
                        });
                        return;
                    }
                }
                catch (Exception)
                {
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
        #endregion
    }

    public class MyEC温度08表格ViewModel : ViewModelBase
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