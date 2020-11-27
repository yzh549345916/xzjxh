using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 数值预报呼和浩特表格 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private string xlsPath = "";
        private ObservableCollection<数值预报呼和浩特ViewModel> 查询列表 = new My数值预报呼和浩特表格ViewModel().Clubs;
        private int sc = 8;
        public 数值预报呼和浩特表格()
        {
            InitializeComponent();
            BTLabel.Content = "呼和浩特数值预报查询";
            GRPFList.ItemsSource = 查询列表;


            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            YSSelect.SelectionChanged += YSSelect_SelectionChanged;
            Thread thread = new Thread(时间初始化);
            thread.Start();
        }
        public 数值预报呼和浩特表格(int mysc)
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
                数值预报呼和浩特处理类 szyb = new 数值预报呼和浩特处理类();

                Dispatcher.Invoke(() =>
                {
                    YBSCSelect.SelectedDate = DateTime.Now.Date;
                });

                DateTime dateTime = szyb.获取最新时间("YB_TEM", "ECSK");
                TimeSpan timeSpan = DateTime.Now.Date.AddHours(8) - DateTime.Now.Date;
                if (DateTime.Now.Hour > 12)
                    timeSpan = DateTime.Now.Date.AddHours(20) - DateTime.Now.Date;
                if (dateTime.CompareTo(DateTime.Now.AddDays(-2)) > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        sDate.SelectedValue = dateTime;
                        YBSCSelect.SelectedTime = timeSpan;
                    });
                }
            }
            catch (Exception)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = "时间初始化异常",
                    Header = "警告"
                });
            }
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            try
            {
                try
                {
                    if (YBSCSelect.SelectedTime != null && YBSCSelect.SelectedTime.Value.Hours != sc)
                        sc = YBSCSelect.SelectedTime.Value.Hours;
                }
                catch
                {
                }
                if (sDate.SelectedDate != null && sDate.SelectedDate.ToString().Length != 0)
                {
                    查询列表.Clear();
                    List<数值预报呼和浩特ViewModel> myLists = 获取呼和浩特数值预报(YSSelect.SelectedIndex);
                    foreach (数值预报呼和浩特ViewModel item in myLists)
                    {
                        查询列表.Add(item);
                    }
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


        public class SKList
        {
            public string ID { get; set; }
            public double Tmax { get; set; }
            public DateTime 高温时间 { get; set; }
            public double Tmin { get; set; }
            public DateTime 低温时间 { get; set; }
        }
        /// <summary>
        /// 根据索引处理呼和浩特数值预报
        /// </summary>
        /// <param name="ysIndex">0：温度；1:10米风；2：降水量</param>
        /// <returns></returns>
        private List<数值预报呼和浩特ViewModel> 获取呼和浩特数值预报(int ysIndex)
        {
            List<数值预报呼和浩特ViewModel> myLists = new List<数值预报呼和浩特ViewModel>();
            数值预报呼和浩特处理类 szyb = new 数值预报呼和浩特处理类();
            List<数值预报呼和浩特处理类.IDName> idNames = szyb.获取站点信息();
            DateTime myQBTime = Convert.ToDateTime(sDate.SelectedValue);
            DateTime myYBTime = Convert.ToDateTime(YBSCSelect.SelectedValue);
            if (idNames.Count > 0)
            {
                try
                {
                    if (ysIndex == 0)
                    {
                        clo5.IsVisible = true;
                        clo5.Header = "最低气温";
                        clo6.IsVisible = true;
                        clo6.Header = "最高气温";
                        clo7.IsVisible = false;
                        clo8.IsVisible = false;
                        clo10.IsVisible = false;
                        List<数值预报呼和浩特处理类.YBList> ysLists = szyb.GetTemListsbyGSandDate(myQBTime.Date, myQBTime.Hour, "ECSK");
                        for (int i = 1; i <= 5; i++)
                        {
                            foreach (var idname in idNames)
                            {
                                List<数值预报呼和浩特处理类.YBList> myLS = ysLists.Where(y => y.id == idname.区站号 && y.dateTime >= myYBTime.AddDays(i - 1) && y.dateTime <= myYBTime.AddDays(i)).OrderBy(y => y.sx).ToList();
                                if (myLS.Count > 0)
                                {
                                    List<数值预报呼和浩特详情ViewModel> 详情 = new List<数值预报呼和浩特详情ViewModel>();
                                    foreach (var item in myLS)
                                    {
                                        详情.Add(new 数值预报呼和浩特详情ViewModel(item.dateTime, item.ys));
                                    }

                                    myLists.Add(new 数值预报呼和浩特ViewModel()
                                    {
                                        站点名称 = idname.站点名称,
                                        区站号 = idname.区站号,
                                        所属旗县 = idname.所属旗县,
                                        测站级别 = idname.测站级别名称,
                                        IsExpanded = true,
                                        值1 = myLS.Min(y => y.ys),
                                        值2 = myLS.Max(y => y.ys),
                                        预报时间 = myYBTime.AddDays(i - 1),
                                        详情 = 详情
                                    });
                                }
                            }
                        }
                    }
                    else if (ysIndex == 1)
                    {
                        clo5.IsVisible = true;
                        clo5.Header = "风向(°)";
                        clo6.IsVisible = true;
                        clo6.Header = "风速(m/s)";
                        clo7.IsVisible = true;
                        clo7.Header = "风向";
                        clo8.IsVisible = true;
                        clo8.Header = "风速";
                        clo10.IsVisible = true;
                        clo10.Header = "极大风速(m/s)";
                        List<数值预报呼和浩特处理类.YBList> ys1Lists = szyb.GetWindListsbyGSandDate(myQBTime.Date, myQBTime.Hour, "ECSK");
                        List<数值预报呼和浩特处理类.YBList> ys2Lists = szyb.GetJDWindListsbyGSandDate(myQBTime.Date, myQBTime.Hour, "ECSK");


                        foreach (var idname in idNames)
                        {
                            List<数值预报呼和浩特处理类.YBList> lsLists = ys1Lists.Where(y => y.id == idname.区站号).ToList();
                            foreach (var item in lsLists)
                            {
                                try
                                {
                                    double jdwind = -999999;
                                    if (ys2Lists.Exists(y => y.id == item.id && y.dateTime == item.dateTime))
                                    {
                                        jdwind = ys2Lists.First(y => y.id == item.id && y.dateTime == item.dateTime).ys2;
                                    }
                                    double fx = 0, fs = 0;
                                    string[] fxfsStr = GetFXFS(item.ys, item.ys2, ref fx, ref fs).Split(',');
                                    myLists.Add(new 数值预报呼和浩特ViewModel()
                                    {
                                        站点名称 = idname.站点名称,
                                        区站号 = idname.区站号,
                                        所属旗县 = idname.所属旗县,
                                        测站级别 = idname.测站级别名称,
                                        IsExpanded = false,
                                        值1 = fx,
                                        值2 = fs,
                                        值3 = jdwind,
                                        预报时间 = item.dateTime,
                                        字符串1 = fxfsStr[0],
                                        字符串2 = fxfsStr[1]

                                    });
                                }
                                catch
                                {
                                }
                            }
                        }



                    }

                }
                catch
                {
                }
            }

            return myLists;
        }
        private void YSSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BTLabel.Content = "呼和浩特数值预报";
                if (sDate.SelectedDate != null)
                    BTLabel.Content += Convert.ToDateTime(sDate.SelectedValue).ToString("M月dd日HH时起报");
                if (YSSelect != null)
                    BTLabel.Content += $"{YSSelect.Text}";
                BTLabel.Content += "查询";
            }
            catch
            {
            }
        }

        private void sDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                BTLabel.Content = "呼和浩特数值预报";
                if (sDate.SelectedDate != null)
                    BTLabel.Content += Convert.ToDateTime(sDate.SelectedValue).ToString("M月dd日HH时起报");
                if (YSSelect != null)
                    BTLabel.Content += $"{YSSelect.Text}";
                BTLabel.Content += "查询";
            }
            catch
            {
            }
        }
        public string GetFXFS(double v, double u, ref double fx, ref double fs)
        {
            string fxfs = "";
            fx = 999.9; //风向

            if (u > 0 & v > 0)
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u < 0 & v > 0)
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u < 0 & v < 0)
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u > 0 & v < 0)
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if (u == 0 & v > 0)
            {
                fx = 180;
            }
            else if (u == 0 & v < 0)
            {
                fx = 0;
            }
            else if (u > 0 & v == 0)
            {
                fx = 270;
            }
            else if (u < 0 & v == 0)
            {
                fx = 90;
            }
            else if (u == 0 & v == 0)
            {
                fx = 999.9;
            }

            //风速是uv分量的平方和

            fs = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
            int intfx = Convert.ToInt32(Math.Round(fx / 45, 0));
            switch (intfx)
            {
                case 0:
                    fxfs = "北风";
                    break;
                case 1:
                    fxfs = "东北风";
                    break;
                case 2:
                    fxfs = "东风";
                    break;
                case 3:
                    fxfs = "东南风";
                    break;
                case 4:
                    fxfs = "南风";
                    break;
                case 5:
                    fxfs = "西南风";
                    break;
                case 6:
                    fxfs = "西风";
                    break;
                case 7:
                    fxfs = "西北风";
                    break;
                case 999017:
                    fxfs = "静风";
                    break;
                default:
                    fxfs = "北风";
                    break;
            }

            fxfs += ',';
            if (fs >= 0 && fs <= 0.2)
            {
                fxfs += "0级";
            }
            else if (fs >= 0.3 && fs <= 1.5)
            {
                fxfs += "1级";
            }
            else if (fs >= 1.6 && fs <= 3.3)
            {
                fxfs += "2级";
            }
            else if (fs >= 3.4 && fs <= 5.4)
            {
                fxfs += "3级";
            }
            else if (fs >= 5.5 && fs <= 7.9)
            {
                fxfs += "4级";
            }
            else if (fs >= 8 && fs <= 10.7)
            {
                fxfs += "5级";
            }
            else if (fs >= 10.8 && fs <= 13.8)
            {
                fxfs += "6级";
            }
            else if (fs >= 13.9 && fs <= 17.1)
            {
                fxfs += "7级";
            }
            else if (fs >= 17.2 && fs <= 20.7)
            {
                fxfs += "8级";
            }
            else if (fs >= 20.8 && fs <= 24.4)
            {
                fxfs += "9级";
            }
            else if (fs >= 24.5 && fs <= 28.4)
            {
                fxfs += "10级";
            }
            else if (fs >= 28.5 && fs <= 32.6)
            {
                fxfs += "11级";
            }
            else if (fs >= 32.7 && fs <= 36.9)
            {
                fxfs += "12级";
            }
            else if (fs >= 37 && fs <= 41.4)
            {
                fxfs += "13级";
            }
            else if (fs >= 41.5 && fs <= 46.1)
            {
                fxfs += "14级";
            }
            else if (fs >= 46.2 && fs <= 50.9)
            {
                fxfs += "15级";
            }
            else if (fs >= 51 && fs <= 56)
            {
                fxfs += "16级";
            }
            else if (fs >= 56.1 && fs <= 61.2)
            {
                fxfs += "17级";
            }
            else if (fs >= 61.3)
            {
                fxfs += "17级以上";
            }
            else
            {
                fxfs += "3级";
            }
            return fxfs;
        }
    }

    public class My数值预报呼和浩特表格ViewModel : ViewModelBase
    {
        private ObservableCollection<数值预报呼和浩特ViewModel> clubs;

        public ObservableCollection<数值预报呼和浩特ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<数值预报呼和浩特ViewModel> CreateClubs()
        {
            ObservableCollection<数值预报呼和浩特ViewModel> clubs = new ObservableCollection<数值预报呼和浩特ViewModel>();
            return clubs;
        }
    }
}