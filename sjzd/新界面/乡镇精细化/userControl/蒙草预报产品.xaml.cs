using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;
using Style = Aspose.Cells.Style;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 蒙草预报产品 : RadWindow
    {
        private string idStr = "53368,53464,53466,53467,53469,53562,53463";
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private ObservableCollection<蒙草预报产品ViewModel> 查询列表 = new My蒙草预报产品ViewModel().Clubs;

        public 蒙草预报产品()
        {
            InitializeComponent();
            GRPFList.ItemsSource = 查询列表;
            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            数据初始化();
        }

        public void 数据初始化()
        {
            RadSplashScreenManager.Show();
            查询列表.Clear();
            try
            {
                蒙草预报 mcyb = new 蒙草预报();
                List<蒙草预报.IDName> idNames = mcyb.获取蒙草站点信息();
                List<蒙草预报.ZNYBList> lists = mcyb.获取国家智能网格(8);

                if (idNames.Count > 0)
                {
                    foreach (蒙草预报.IDName item in idNames)
                    {
                        try
                        {
                            for (short i = 3; i <= 72; i += 3)
                            {
                                if (lists.Exists(y => y.ID == item.ID && y.SX == i))
                                {
                                    蒙草预报.ZNYBList ybLS = lists.First(y => y.ID == item.ID && y.SX == i);
                                    查询列表.Add(new 蒙草预报产品ViewModel
                                    {
                                        ID = item.ID,
                                        名称 = item.Name,
                                        IsExpanded = false,
                                        时间 = DateTime.Now.Date.AddHours(8 + i),
                                        天气 = ybLS.TQ,
                                        气温 = ybLS.TEM,
                                        相对湿度 = ybLS.ERH,
                                        降水量 = ybLS.PRE_3H,
                                        风力 = ybLS.FS,
                                        风向 = ybLS.FX
                                    });
                                }
                                else
                                {
                                    查询列表.Add(new 蒙草预报产品ViewModel
                                    {
                                        ID = item.ID,
                                        名称 = item.Name,
                                        IsExpanded = false,
                                        时间 = DateTime.Now.Date.AddHours(8 + i)
                                    });
                                }
                            }
                        }
                        catch
                        {
                        }

                        try
                        {
                            TextBox mytxt = sp1.FindName(item.Name) as TextBox;
                            if (mytxt != null)
                            {
                                string myStr = "";
                                for (short j = 0; j < 2; j++)
                                {
                                    List<蒙草预报.ZNYBList> listLS = lists.Where(y => y.ID == item.ID && y.SX > 24 * (j + 1) && y.SX <= 24 * (j + 2)).ToList();
                                    if (listLS.Count > 0)
                                    {
                                        string tq1 = listLS[0].TQ, tq2 = listLS[0].TQ, fx1 = listLS[0].FX, fx2 = listLS[0].FX, fs1 = listLS[0].FS, fs2 = listLS[0].FS;
                                        double tmax = listLS[0].TEM, tmin = listLS[0].TEM;
                                        foreach (var ybLS in listLS)
                                        {
                                            if (!tq2.Equals(ybLS.TQ))
                                                tq2 = ybLS.TQ;
                                            if (!fx2.Equals(ybLS.FX))
                                                fx2 = ybLS.FX;
                                            if (!fs2.Equals(ybLS.FS))
                                                fs2 = ybLS.FS;
                                            if (tmax < ybLS.TEM)
                                                tmax = ybLS.TEM;
                                            if (tmin > ybLS.TEM)
                                                tmin = ybLS.TEM;
                                        }

                                        string tq = tq1.Equals(tq2) ? tq2 : $"{tq1}转{tq2}";
                                        string fx = fx1.Equals(fx2) ? fx2 : $"{fx1}转{fx2}";
                                        string fs = fs1.Equals(fs2) ? fs2 : $"{fs1}转{fs2}";
                                        myStr += $"{DateTime.Now.AddDays(j).Day}日白天至{DateTime.Now.AddDays(j + 1).Day}日白天：项目区{tq}，{fx}{fs}，{tmin}-{tmax}℃。\r\n";
                                    }
                                }

                                mytxt.Text = myStr;
                            }
                        }
                        catch

                        {
                        }
                    }
                }
            }
            catch

            {
            }

            RadSplashScreenManager.Close();
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
            string strPath = Environment.CurrentDirectory + $@"\产品\蒙草临时\{DateTime.Now:yyyy年MM月}\";
            if (!Directory.Exists(strPath))
                Directory.CreateDirectory(strPath);
            strPath += $"{DateTime.Now:yyyy年MM月dd日}蒙草预报服务产品.xls";
            List<蒙草导出数据> mcMB = new List<蒙草导出数据>();
            蒙草预报 mcyb = new 蒙草预报();
            List<蒙草预报.IDName> idNames = mcyb.获取蒙草站点信息();
            foreach (var id in idNames)
            {
                List<蒙草预报产品ViewModel> listLS = 查询列表.Where(y => y.ID == id.ID).OrderBy(y => y.时间).ToList();
                if (listLS.Count > 1)
                {
                    for (int i = 1; i < listLS.Count; i++)
                    {
                        string qw = "";
                        if (listLS[i].气温 > listLS[i - 1].气温)
                        {
                            qw = $"{listLS[i - 1].气温}—{listLS[i].气温}";
                        }
                        else if (listLS[i].气温 < listLS[i - 1].气温)
                        {
                            qw = $"{listLS[i].气温}—{listLS[i - 1].气温}";
                        }
                        else
                        {
                            qw = listLS[i].气温.ToString();
                        }

                        mcMB.Add(new 蒙草导出数据
                        {
                            名称 = id.Name,
                            开始时间 = listLS[i - 1].时间,
                            结束时间 = listLS[i].时间,
                            天气 = listLS[i].天气 == listLS[i - 1].天气 ? listLS[i - 1].天气 : $"{listLS[i - 1].天气}转{listLS[i].天气}",
                            风力 = listLS[i].风力,
                            风向 = listLS[i].风向 == listLS[i - 1].风向 ? listLS[i - 1].风向 : $"{listLS[i - 1].风向}转{listLS[i].风向}",
                            湿度 = listLS[i].相对湿度,
                            降水量 = listLS[i].降水量,
                            气温 = qw,
                        });
                    }
                }
            }

            蒙草导出数据[] dcsz = mcMB.ToArray();
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
                cellSheet.Cells[0, 0].PutValue("项目区");
                cellSheet.Cells[0, 1].PutValue("开始时间");
                cellSheet.Cells[0, 2].PutValue("结束时间");
                cellSheet.Cells[0, 3].PutValue("降雨量(mm)");
                cellSheet.Cells[0, 4].PutValue("气温（℃）");
                cellSheet.Cells[0, 5].PutValue("湿度（％）");
                cellSheet.Cells[0, 6].PutValue("天气现象");
                cellSheet.Cells[0, 7].PutValue("风向");
                cellSheet.Cells[0, 8].PutValue("风力");
                cellSheet.Cells[0, 9].PutValue("48-72小时天气预报");

                for (int i = 0; i < dcsz.Length; i++)
                {
                    cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].名称);
                    cellSheet.Cells[i + 1, 0].SetStyle(style2);
                    cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].开始时间.ToString(""));
                    cellSheet.Cells[i + 1, 1].SetStyle(style2);
                    cellSheet.Cells[i + 1, 2].PutValue(dcsz[i].结束时间.ToString(""));
                    cellSheet.Cells[i + 1, 2].SetStyle(style2);
                    cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].降水量);
                    cellSheet.Cells[i + 1, 3].SetStyle(style2);
                    cellSheet.Cells[i + 1, 4].PutValue(dcsz[i].气温);
                    cellSheet.Cells[i + 1, 4].SetStyle(style2);
                    cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].湿度);
                    cellSheet.Cells[i + 1, 5].SetStyle(style2);
                    cellSheet.Cells[i + 1, 6].PutValue(dcsz[i].天气);
                    cellSheet.Cells[i + 1, 6].SetStyle(style2);
                    cellSheet.Cells[i + 1, 7].PutValue(dcsz[i].风向);
                    cellSheet.Cells[i + 1, 7].SetStyle(style2);
                    cellSheet.Cells[i + 1, 8].PutValue(dcsz[i].风力);
                    cellSheet.Cells[i + 1, 8].SetStyle(style2);
                }

                foreach (var id in idNames)
                {
                    try
                    {
                        TextBox mytxt = sp1.FindName(id.Name) as TextBox;
                        if (mytxt != null)
                        {
                            try
                            {
                                for (int i = 0; i < dcsz.Length; i++)
                                {
                                    if (dcsz[i].名称 == id.Name)
                                    {
                                        cellSheet.Cells[i + 1, 9].PutValue(mytxt.Text);
                                        cellSheet.Cells[i + 1, 9].SetStyle(style2);
                                        break;
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                    }
                    catch
                    {
                    }
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
                Confirm(new DialogParameters
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
                Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "警告"
                });
            }
        }

        private void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            数据初始化();
        }

        public class 蒙草导出数据
        {
            public string 名称 { get; set; }
            public DateTime 开始时间 { get; set; }
            public DateTime 结束时间 { get; set; }
            public string 气温 { get; set; }
            public double 湿度 { get; set; }
            public double 降水量 { get; set; }
            public string 天气 { get; set; }
            public string 风向 { get; set; }
            public string 风力 { get; set; }
        }
    }

    public class My蒙草预报产品ViewModel : ViewModelBase
    {
        private ObservableCollection<蒙草预报产品ViewModel> clubs;

        public ObservableCollection<蒙草预报产品ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<蒙草预报产品ViewModel> CreateClubs()
        {
            ObservableCollection<蒙草预报产品ViewModel> clubs = new ObservableCollection<蒙草预报产品ViewModel>();
            return clubs;
        }
    }
}