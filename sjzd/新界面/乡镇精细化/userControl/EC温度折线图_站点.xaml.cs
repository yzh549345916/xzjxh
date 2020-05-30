using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.SplashScreen;
using Style = Aspose.Cells.Style;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class EC温度折线图_站点 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private List<CIMISS.ECTEF0> _temLists;
        private List<CIMISS.YS> _skLists;
        private int _sc = 8;
        public EC温度折线图_站点(List<CIMISS.ECTEF0> temLists, List<CIMISS.YS> skLists,string idStr,int sc)
        {
            InitializeComponent();
            _temLists = temLists;
            _skLists = skLists;
            _sc = sc;
            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            if (idStr.Length>0)
            {
                string[] idsz = idStr.Split(',');
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                int id466 = 0;
                for(int i=0;i<idsz.Length;i++)
                {
                    mydic.Add(i, idsz[i]);
                    if (idsz[i] == "53466")
                        id466 = i;
                }
                QXSelect.ItemsSource = mydic;
                QXSelect.SelectedValuePath = "Key";
                QXSelect.DisplayMemberPath = "Value";
                QXSelect.SelectionChanged += QXSelect_SelectionChanged;
                QXSelect.SelectedValue = id466;
                
                
                
            }
           
           

        }
        private ChartAnimationBase GetAnimation(AnimationInfo animationDescriptor)
        {
            ChartAnimationBase animation;
            switch (animationDescriptor.AnimationType)
            {
                case AnimationType.Move:
                    animation = new ChartMoveAnimation
                    {
                        MoveAnimationType = (MoveAnimationType)animationDescriptor.Properties[2],
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[3])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[4])),
                        Easing = animationDescriptor.Properties[5] as IEasingFunction
                    };
                    break;

                case AnimationType.Scale:
                    animation = new ChartScaleAnimation
                    {
                        ScaleMode = (ScaleMode)animationDescriptor.Properties[2],
                        MinScale = Convert.ToDouble(animationDescriptor.Properties[3]),
                        MaxScale = Convert.ToDouble(animationDescriptor.Properties[4]),
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[5])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[6])),
                        Easing = animationDescriptor.Properties[7] as IEasingFunction
                    };
                    break;

                case AnimationType.Fade:
                    animation = new ChartFadeAnimation
                    {
                        MinOpacity = Convert.ToDouble(animationDescriptor.Properties[2]),
                        MaxOpacity = Convert.ToDouble(animationDescriptor.Properties[3]),
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[4])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[5])),
                        Easing = animationDescriptor.Properties[6] as IEasingFunction
                    };
                    break;
                case AnimationType.DropAndFade:
                    animation = new ChartDropFadeAnimation
                    {
                        MoveAnimationType = (MoveAnimationType)animationDescriptor.Properties[2],
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[3])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[4])),
                        Easing = animationDescriptor.Properties[5] as IEasingFunction
                    };
                    break;
                case AnimationType.Reveal:
                    animation = new ChartRevealAnimation
                    {
                        AnimationDirection = (AnimationDirection)animationDescriptor.Properties[2],
                        Orientation = (Orientation)animationDescriptor.Properties[3],
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[4])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[5])),
                        Easing = animationDescriptor.Properties[6] as IEasingFunction
                    };
                    break;
                case AnimationType.PieChartAngleRange:
                    animation = new PieChartAngleRangeAnimation
                    {
                        InitialStartAngle = Convert.ToDouble(animationDescriptor.Properties[2]),
                        InitialSweepAngle = Convert.ToDouble(animationDescriptor.Properties[3]),
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[4])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[5])),
                        Easing = animationDescriptor.Properties[6] as IEasingFunction
                    };
                    break;
                case AnimationType.PieChartRadiusFactor:
                    animation = new PieChartRadiusFactorAnimation
                    {
                        Duration = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[2])),
                        Delay = TimeSpan.FromMilliseconds(Convert.ToDouble(animationDescriptor.Properties[3])),
                        Easing = animationDescriptor.Properties[4] as IEasingFunction
                    };
                    break;
                default:
                    animation = new ChartMoveAnimation();
                    break;
            }

            return animation;
        }

        private void QXSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            chart2.VerticalAxis.Title = "温度(℃)";
            chart2.Series.Clear();
            string id = QXSelect.Text;
            var myEC = _temLists.Where(y => y.StationID == id).OrderBy(y=>y.DateTime);
           
            List<DateTime> maxminDate = new List<DateTime>();
            for (int i = 0; i < 5; i++)
            {
                DateTime myDate = DateTime.Now.Date.AddHours(_sc).AddDays(i);
                List<CIMISS.ECTEF0> temLists = myEC.Where(y => y.DateTime.CompareTo(myDate) >= 0 && y.DateTime.CompareTo(myDate.AddDays(1)) <= 0).OrderBy(y => y.TEM).ToList();
                if(temLists.Count>=5)
                {
                    if (!maxminDate.Exists(y => y == temLists[0].DateTime))
                        maxminDate.Add(temLists[0].DateTime);
                    if (!maxminDate.Exists(y => y == temLists[temLists.Count - 1].DateTime))
                        maxminDate.Add(temLists[temLists.Count - 1].DateTime);
                }
            }
            SplineSeries lineSeries2 = new SplineSeries();

            foreach (CIMISS.ECTEF0 item in myEC)
            {
                if(maxminDate.Exists(y=>y== item.DateTime))
                {
                    lineSeries2.DataPoints.Add(new CategoricalDataPoint
                    {
                        Category = item.DateTime.ToString("d日H时"),
                        Value = Math.Round(item.TEM, 2),
                        Label = item.DateTime.ToString("d日H时：") + Math.Round(item.TEM, 2) + "℃"
                    });
                }
                else
                {
                    lineSeries2.DataPoints.Add(new CategoricalDataPoint
                    {
                        Category = item.DateTime.ToString("d日H时"),
                        Value = Math.Round(item.TEM, 2),
                        Label = ""
                    });
                }

            }
            SineEase sineEase = new SineEase
            {
                EasingMode = EasingMode.EaseIn
            };

            lineSeries2.PointTemplate = chart2.FindResource("PointTemplate1") as DataTemplate;
            lineSeries2.SeriesAnimation = GetAnimation(new AnimationInfo("Move From Left", AnimationType.Reveal, AnimationDirection.In, Orientation.Horizontal, 2500, 100, sineEase));
            lineSeries2.TrackBallInfoTemplate = chart2.FindResource("trackBallInfoTemplate") as DataTemplate;
            lineSeries2.ShowLabels = true;
            maxminDate.Clear();
            foreach(CIMISS.ECTEF0 item in myEC)
            {
                DateTime timeLS = item.DateTime;
                if (timeLS.CompareTo(DateTime.Now) > 0)
                    break;
                maxminDate.Add(item.DateTime);
            }
            var mySK = _skLists.Where(y => y.StationID == id).OrderBy(y => y.DateTime);
            SplineSeries lineSeries1 = new SplineSeries();
            lineSeries1.DisplayName = "实况";
            foreach (DateTime dateLS in maxminDate)
            {
                CIMISS.YS ys = mySK.First(y => y.DateTime == dateLS);
                lineSeries1.DataPoints.Add(new CategoricalDataPoint
                {
                    Category = ys.DateTime.ToString("d日H时"),
                    Value = Math.Round(ys.TEM, 2),
                    });
            }
            lineSeries1.PointTemplate = chart2.FindResource("PointTemplate2") as DataTemplate;
            lineSeries1.SeriesAnimation = GetAnimation(new AnimationInfo("Move From Left", AnimationType.Reveal, AnimationDirection.In, Orientation.Horizontal, 1500, 100, sineEase));
            lineSeries1.TrackBallInfoTemplate = chart2.FindResource("trackBallInfoTemplateSK") as DataTemplate;
            lineSeries1.ShowLabels = true;
            chart2.Series.Add(lineSeries2);
            chart2.Series.Add(lineSeries1);
        }
    }

   
}