using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Telerik.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Animation;
using Telerik.Windows.Controls.ChartView;

namespace sjzd
{
    /// <summary>
    /// mapUsercontrol.xaml 的交互逻辑
    /// </summary>
    public partial class 乡镇精细化主界面 : UserControl
    {
        private List<进度条.检验结果> myDataLists;
        private string selectStationID = "BFHT";
        public 乡镇精细化主界面()
        {
            InitializeComponent();
            LoadSnow();
            sampleRadCarousel.ReflectionSettings.Visibility = Visibility.Visible;
            sampleRadCarousel.ReflectionSettings.Opacity = 0.8;
            OrientationBehavior.SetOrientation(chart4, Orientation.Horizontal);
            Thread thread = new Thread(数据初始化);
            thread.Start();
        }

        private void sampleRadCarousel_SelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            selectStationID = (DataContext as Stations).SelectedStation.StationID;
            Thread thread = new Thread(处理图表);
            thread.Start();
        }


        private void 数据初始化()
        {
            Dispatcher.Invoke(() =>
            {
                RadWindow settingsDialog = new RadWindow();

                settingsDialog.Content = new 进度条();
                settingsDialog.ResizeMode = ResizeMode.CanResize;
                settingsDialog.Header = "正在读取数据";
                settingsDialog.Owner = this;
                settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settingsDialog.HideMinimizeButton = false;
                settingsDialog.HideMaximizeButton = true;
                settingsDialog.CanClose = false;
                settingsDialog.Closed += 进度条窗口关闭;
                settingsDialog.ShowDialog();
            });
        }
        public void 处理图表()
        {
            if (selectStationID.Trim().Length > 0 && myDataLists.Count > 0)
            {
                Dispatcher?.Invoke(delegate
                {
                    //要做的事
                    chart1.VerticalAxis.Title = "准确率(%)";
                    chart2.VerticalAxis.Title = "平均准确率(%)";
                    chart3.VerticalAxis.Title = "综合技巧";
                    chart4.VerticalAxis.Title = "技巧";
                    chart1.Series.Clear();
                    chart2.Series.Clear();
                    chart3.Series.Clear();
                    chart4.Series.Clear();

                    List<进度条.检验结果> listLS = myDataLists.Where(y => y.StationID == selectStationID).ToList();
                    SineEase sineEase = new SineEase
                    {
                        EasingMode = EasingMode.EaseIn
                    };
                    #region 其他准确率
                    BarSeries barSeries = new BarSeries();
                    foreach (进度条.检验结果 item in listLS)
                    {
                        barSeries.DataPoints.Add(new CategoricalDataPoint
                        {
                            Category = item.Date.ToString("yy年M月"),
                            Value = Math.Round(item.QYPF, 2),
                            Label = "晴雨准确率"
                        });
                    }
                    BounceEase backEase = new BounceEase
                    {
                        EasingMode = EasingMode.EaseOut,
                        Bounciness = 2,
                        Bounces = 15
                    };
                    barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Top, 1500, 150, backEase));
                    barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                    barSeries.LegendSettings = new SeriesLegendSettings
                    {
                        Title = " 晴雨 \n准确率"
                    };
                    chart1.Series.Add(barSeries);
                    barSeries = new BarSeries();
                    foreach (进度条.检验结果 item in listLS)
                    {
                        barSeries.DataPoints.Add(new CategoricalDataPoint
                        {
                            Category = item.Date.ToString("yy年M月"),
                            Value = Math.Round(item.DWPF),
                            Label = "低温准确率"
                        });
                    }
                    barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Top, 1500, 250, backEase));
                    barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                    barSeries.LegendSettings = new SeriesLegendSettings
                    {
                        Title = " 低温 \n准确率"
                    };
                    chart1.Series.Add(barSeries);
                    barSeries = new BarSeries();
                    foreach (进度条.检验结果 item in listLS)
                    {
                        barSeries.DataPoints.Add(new CategoricalDataPoint
                        {
                            Category = item.Date.ToString("yy年M月"),
                            Value = Math.Round(item.GWPF),
                            Label = "高温准确率"
                        });
                    }
                    barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Top, 1500, 350, backEase));
                    barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                    barSeries.LegendSettings = new SeriesLegendSettings
                    {
                        Title = " 高温 \n准确率"
                    };
                    chart1.Series.Add(barSeries);
                    #endregion
                    #region 平均准确率
                    SplineSeries lineSeries2 = new SplineSeries();
                    foreach (进度条.检验结果 item in listLS)
                    {
                        lineSeries2.DataPoints.Add(new CategoricalDataPoint
                        {
                            Category = item.Date.ToString("yy年M月"),
                            Value = Math.Round(item.ZHPF, 2),
                        });
                    }
                    lineSeries2.PointTemplate = chart2.FindResource("PointTemplate1") as DataTemplate;
                    lineSeries2.SeriesAnimation = GetAnimation(new AnimationInfo("Move From Left", AnimationType.Reveal, AnimationDirection.In, Orientation.Horizontal, 2500, 100, sineEase));
                    lineSeries2.TrackBallInfoTemplate = chart2.FindResource("trackBallInfoTemplate") as DataTemplate;
                    chart2.Series.Add(lineSeries2);
                    #endregion
                    #region 综合技巧
                    BubbleSeries bubbleSeries1 = new BubbleSeries();
                    if (selectStationID == "BFHT")
                    {
                        listLS = myDataLists.Where(y => y.StationID != selectStationID && y.Date == myDataLists[myDataLists.Count - 1].Date).ToList();

                        chart3.VerticalAxis.Title = myDataLists[myDataLists.Count - 1].Date.ToString("yy年MM月") + "综合技巧";
                        foreach (进度条.检验结果 item in listLS)
                        {
                            bubbleSeries1.DataPoints.Add(new BubbleDataPoint
                            {
                                Category = item.Name,
                                Value = Math.Round(item.AllJQ, 2),
                                BubbleSize = 20
                            });
                        }
                    }
                    else
                    {
                        foreach (进度条.检验结果 item in listLS)
                        {
                            bubbleSeries1.DataPoints.Add(new BubbleDataPoint
                            {
                                Category = item.Date.ToString("yy年M月"),
                                Value = Math.Round(item.AllJQ, 2),
                                BubbleSize = 20
                            });
                        }
                    }
                    bubbleSeries1.TrackBallInfoTemplate = chart3.FindResource("trackBallInfoTemplate") as DataTemplate;
                    BounceEase bounceEase = new BounceEase() { Bounces = 3, EasingMode = EasingMode.EaseOut, Bounciness = 2 };
                    bubbleSeries1.PointAnimation = GetAnimation(new AnimationInfo("Scale ", AnimationType.Scale, ScaleMode.Both, 0, 1, 1800, 80, bounceEase));
                    chart3.Series.Add(bubbleSeries1);
                    #endregion
                    #region 技巧

                    backEase = new BounceEase
                    {
                        EasingMode = EasingMode.EaseOut,
                        Bounciness = 8,
                        Bounces = 8
                    };
                    if (selectStationID == "BFHT")
                    {
                        listLS = myDataLists.Where(y => y.StationID != selectStationID && y.Date == myDataLists[myDataLists.Count - 1].Date).ToList();

                        chart4.VerticalAxis.Title = myDataLists[myDataLists.Count - 1].Date.ToString("yy年MM月") + "技巧";
                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Name,
                                Value = Math.Round(item.QYJQ, 2),
                                Label = "晴雨技巧"
                            });
                        }
                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 150, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "晴雨\n技巧"
                        };
                        chart4.Series.Add(barSeries);

                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Name,
                                Value = Math.Round(item.DWJQ, 2),
                                Label = "低温技巧"
                            });
                        }
                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 250, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "低温\n技巧"
                        };
                        chart4.Series.Add(barSeries);
                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Name,
                                Value = Math.Round(item.GWJQ, 2),
                                Label = "高温技巧"
                            });
                        }
                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 250, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "高温\n技巧"
                        };
                        chart4.Series.Add(barSeries);
                    }
                    else
                    {
                        listLS = myDataLists.Where(y => y.StationID == selectStationID && y.Date >= myDataLists[myDataLists.Count - 1].Date.AddMonths(-6)).ToList();

                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Date.ToString("yy年M月"),
                                Value = Math.Round(item.QYJQ, 2),
                                Label = "晴雨技巧"
                            });
                        }

                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 150, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "晴雨\n技巧"
                        };
                        chart4.Series.Add(barSeries);

                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Date.ToString("yy年M月"),
                                Value = Math.Round(item.DWJQ, 2),
                                Label = "低温技巧"
                            });
                        }
                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 250, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "低温\n技巧"
                        };
                        chart4.Series.Add(barSeries);
                        barSeries = new BarSeries();
                        foreach (进度条.检验结果 item in listLS)
                        {
                            barSeries.DataPoints.Add(new CategoricalDataPoint
                            {
                                Category = item.Date.ToString("yy年M月"),
                                Value = Math.Round(item.GWJQ, 2),
                                Label = "高温技巧"
                            });
                        }
                        barSeries.PointAnimation = GetAnimation(new AnimationInfo("Move From Bottom", AnimationType.Move, MoveAnimationType.Right, 1500, 250, backEase));
                        barSeries.TooltipTemplate = chart1.FindResource("ToolTipTemplate") as DataTemplate;
                        barSeries.LegendSettings = new SeriesLegendSettings
                        {
                            Title = "高温\n技巧"
                        };
                        chart4.Series.Add(barSeries);
                    }
                    #endregion
                });
            }

        }

        private void 进度条窗口关闭(object sender, EventArgs e)
        {
            try
            {
                myDataLists = ((sender as RadWindow).Content as 进度条).结果;
                //Stations stations = new Stations();
                // stations.SelectedStation
                (this.DataContext as Stations).SelectedStation = (this.DataContext as Stations).StationsCollection[2];

            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow
                });
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

        private void Timer_Tick(object sender, EventArgs e)
        {
            double top;
            for (int i = 0; i < Snowflake.Count; i++)
            {
                top = Canvas.GetTop(Snowflake[i]);
                if (top >= SnowflakeY[i])
                {
                    Canvas.SetTop(Snowflake[i], -10);
                    ReSnowflake.Add(Snowflake[i]);
                    Snowflake.RemoveAt(i);
                    SnowflakeY.RemoveAt(i);
                }
                else
                {
                    Canvas.SetTop(Snowflake[i], top + Speed);
                }
            }
        }


        #region 雪花特效相关

        private bool bsSnow = true;

        /// <summary>
        /// 设置雪花下落速度
        /// </summary>
        private double Speed = double.Parse(ConfigurationManager.AppSettings["Speed"]);

        /// <summary>
        /// 雪花密度
        /// </summary>
        private int SleepTime = int.Parse(ConfigurationManager.AppSettings["SleepTime"]);

        /// <summary>
        /// 雪花最大尺寸
        /// </summary>
        private int snowSize = int.Parse(ConfigurationManager.AppSettings["snowSize"]);

        /// <summary>
        /// 雪花容器
        /// </summary>
        private int SnowflakeLen = int.Parse(ConfigurationManager.AppSettings["SnowflakeLen"]), ReSnowflakeLen = int.Parse(ConfigurationManager.AppSettings["ReSnowflakeLen"]);

        //private const int SnowflakeLen = 35, ReSnowflakeLen = 35;

        private Image i = new Image();
        private List<Image> Snowflake;
        private List<Image> ReSnowflake;
        private bool[] IsBottom;
        private const int SnowflakeWidth = 10;
        private Random random;
        private List<int> SnowflakeY;
        private ThreadStart ts;
        private Thread td;

        /// <summary>
        /// 加载灯 笼蒙版
        /// </summary>
        private void LoadBackGround()
        {
            BitmapImage img = new BitmapImage(new Uri(@"Image/1.jpg", UriKind.Relative));
            _denglong.Source = img;
        }

        /// <summary>
        /// 加载雪花效果
        /// </summary>
        private void LoadSnow()
        {
            random = new Random();
            Init(@"Image/snow.png", @"Image/snow.png");

            ts = ThreadFun;

            td = new Thread(ts);
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.LowQuality);
            CompositionTarget.Rendering += Timer_Tick;
            td.Name = "雪花";
            td.Start();
        }

        private void Init(string uri1, string uri2)
        {
            int CanWidth = (int)g.Width, pos;
            Image sf;
            BitmapImage x1, x2;
            x1 = new BitmapImage(new Uri(uri1, UriKind.Relative));
            x2 = new BitmapImage(new Uri(uri2, UriKind.Relative));
            Snowflake = new List<Image>();
            ReSnowflake = new List<Image>();
            IsBottom = new bool[SnowflakeLen];
            SnowflakeY = new List<int>();
            for (int i = 0; i < SnowflakeLen / 2; i++)
            {
                pos = random.Next(0, (int)ActualWidth);
                SnowflakeY.Add(GetEnd(pos));
                sf = new Image();
                sf.Source = x1;
                sf.Width = random.Next(15, snowSize);
                sf.Height = sf.Width;
                Canvas.SetLeft(sf, pos);
                Canvas.SetTop(sf, GetPosY(pos));
                g.Children.Add(sf);
                Snowflake.Add(sf);
            }

            for (int i = SnowflakeLen / 2; i < SnowflakeLen; i++)
            {
                pos = random.Next(0, (int)ActualWidth);
                SnowflakeY.Add(GetEnd(pos));
                sf = new Image();
                sf.Source = x2;
                sf.Width = random.Next(15, snowSize);
                sf.Height = sf.Width;
                Canvas.SetLeft(sf, pos);
                Canvas.SetTop(sf, GetPosY(pos));
                g.Children.Add(sf);
                Snowflake.Add(sf);
            }

            for (int i = 0; i < ReSnowflakeLen; i++)
            {
                sf = new Image();
                sf.Source = x2;
                sf.Width = random.Next(15, snowSize);
                sf.Height = sf.Width;
                Canvas.SetLeft(sf, -10);
                Canvas.SetTop(sf, -10);
                g.Children.Add(sf);
                ReSnowflake.Add(sf);
            }
        }

        private int GetEnd(int x)
        {
            int z = 0, y = 0;
            int BD;
            if (x <= 140)
            {
                z = (int)(0.3571 * x + 143);
                y = z;
            }
            else if (140 <= x && x <= ActualHeight)
            {
                z = (int)(0.4743 * x + 126);
                BD = (int)(0.1423 * x + 167);
                y = random.Next(BD, z);
            }
            else
            {
                z = (int)ActualHeight;
                BD = (int)(0.1423 * x + 167);
                y = random.Next(BD, z);
            }

            return y;
        }

        private int GetPosY(int x)
        {
            int z = 0, y = 0;
            int BD;
            if (x <= 140)
            {
                z = (int)(0.3571 * x + 143);
                y = random.Next(z);
            }
            else if (140 <= x && x <= ActualHeight)
            {
                z = (int)(0.4743 * x + 126);
                BD = (int)(0.1423 * x + 167);
                y = random.Next(z);
            }
            else
            {
                z = (int)ActualHeight;
                BD = (int)(0.1423 * x + 167);
                y = random.Next(z);
            }

            return y;
        }

        private void ThreadFun()
        {
            int pos;

            while (bsSnow)
            {
                //if (Snowflake.Count > 60)
                //{
                //    SleepTime = 2000;
                //}
                //else if (Snowflake.Count < 50)
                //{
                //    SleepTime = 1500;
                //}
                //else if (Snowflake.Count < 45)
                //{
                //    SleepTime = 1200;
                //}
                //else if (Snowflake.Count < 35)
                //{
                //    SleepTime = 900;
                //}
                for (int i = 0; i < ReSnowflake.Count; i++)
                {
                    if (!bsSnow)
                        break;
                    Thread.Sleep(random.Next(SleepTime));
                    Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate
                   {
                        //textBlock4.Text = SleepTime.ToString();
                        pos = random.Next(0, (int)ActualWidth);
                       Canvas.SetLeft(ReSnowflake[i], pos);
                       Snowflake.Add(ReSnowflake[i]);
                        //SnowflakeY.Add(GetEnd(pos));
                        SnowflakeY.Add((int)ActualHeight);
                       ReSnowflake.RemoveAt(i);
                   });
                }
            }
        }

        private void RadButton_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void UserControl_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            //RadWindow settingsDialog = new RadWindow();
            //settingsDialog.Content = new 乡镇精细化右键菜单();
            //settingsDialog.ResizeMode = ResizeMode.CanResize;
            //settingsDialog.Owner = Application.Current.MainWindow;
            //settingsDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            //settingsDialog.HideMinimizeButton = true;
            //settingsDialog.HideMaximizeButton = true;

            //settingsDialog.CanClose = false;
            //settingsDialog.Show();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            bsSnow = false;
        }

        #endregion
    }

    public enum AnimationType
    {
        Move,
        Scale,
        Fade,
        DropAndFade,
        Reveal,
        PieChartAngleRange,
        PieChartRadiusFactor
    }

    public class AnimationInfo
    {
        public AnimationInfo(params object[] parameters)
        {
            AnimationName = parameters[0].ToString();
            AnimationType = (AnimationType)parameters[1];
            Properties = parameters;
        }

        public string AnimationName { get; set; }
        public AnimationType AnimationType { get; set; }
        public object[] Properties { get; set; }
    }
}