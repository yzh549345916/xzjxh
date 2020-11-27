using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using Telerik.Windows.Controls.ChartView;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 实况温度折线图 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        string idStr = "53368,53464,53466,53467,53469,53562,53463";
        public 实况温度折线图()
        {
            InitializeComponent();

            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";

        }
        public 实况温度折线图(List<CIMISS.ECTEF0> temLists)
        {
            InitializeComponent();


            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            chart2.VerticalAxis.Title = "温度(℃)";
            chart2.Series.Clear();
            SplineSeries lineSeries2 = new SplineSeries();
            double tmax = -9999, tmin = 9999;
            foreach (CIMISS.ECTEF0 item in temLists)
            {
                if (item.TEM > tmax)
                    tmax = item.TEM;
                if (item.TEM < tmin)
                    tmin = item.TEM;
            }
            foreach (CIMISS.ECTEF0 item in temLists)
            {
                if (item.TEM == tmax || item.TEM == tmin)
                {
                    lineSeries2.DataPoints.Add(new CategoricalDataPoint
                    {
                        Category = item.DateTime.ToString("d日H时"),
                        Value = Math.Round(item.TEM, 2),
                        Label = item.DateTime.ToString("d日H时：") + Math.Round(item.TEM, 2)
                    });
                }
                else
                {
                    lineSeries2.DataPoints.Add(new CategoricalDataPoint
                    {
                        Category = item.DateTime.ToString("d日H时"),
                        Value = Math.Round(item.TEM, 2),
                        Label = Math.Round(item.TEM, 2)
                    });
                }

            }
            SineEase sineEase = new SineEase
            {
                EasingMode = EasingMode.EaseIn
            };

            lineSeries2.PointTemplate = chart2.FindResource("PointTemplate1") as DataTemplate;
            lineSeries2.SeriesAnimation = GetAnimation(new AnimationInfo("Move From Left", AnimationType.Reveal, AnimationDirection.In, Orientation.Horizontal, 1500, 100, sineEase));
            lineSeries2.TrackBallInfoTemplate = chart2.FindResource("trackBallInfoTemplate") as DataTemplate;
            lineSeries2.ShowLabels = true;
            chart2.Series.Add(lineSeries2);

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
    }


}