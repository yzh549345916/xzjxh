using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 城镇个人评分72小时 : UserControl
    {

        private SplashScreenDataContext splashScreenDataContext;
        private ObservableCollection<城镇预报120小时个人评分ViewModel> 评分列表 = new My城镇精细化个人评分ViewModel().Clubs;
        private string xlsPath = "";
        public 城镇个人评分72小时()
        {
            InitializeComponent();
            BTLabel.Content = "城镇预报72小时个人评分查询";


            try
            {


                DateTime dt = DateTime.Now;
                sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "警告"
                });
            }
            GRPFList.ItemsSource = 评分列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            try
            {
                if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
                {
                    评分列表.Clear();
                    string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                    string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                    BTLabel.Content = startDate + "至" + endDate + QXSelect.Text + "城镇预报72小时个人评分查询";

                    城镇预报统计类 tj = new 城镇预报统计类();
                    string peopleStr = tj.tjPeople(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate));



                    if (peopleStr.Length > 0)
                    {
                        string[] peoplesz = peopleStr.Split('\n');
                        string[,] GRPFSZ = new string[peoplesz.Length, 13];//数组保存个人评分信息
                        for (int i = 0; i < peoplesz.Length; i++)
                        {

                            GRPFSZ[i, 0] = peoplesz[i].Split(',')[0];
                            GRPFSZ[i, 2] = peoplesz[i].Split(',')[1];
                            float[] zqlFloat = tj.GRZQL(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GRPFSZ[i, 0]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率
                            GRPFSZ[i, 5] = Convert.ToString(Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2));
                            GRPFSZ[i, 6] = Convert.ToString(Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2));
                            GRPFSZ[i, 7] = Convert.ToString(Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2));
                            GRPFSZ[i, 8] = Convert.ToString(Math.Round(Convert.ToSingle(0.4 * Convert.ToDouble(GRPFSZ[i, 5]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 6]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 7])), 2));//总评分
                            float[] GRJDWCSZ = tj.GRJDWC(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GRPFSZ[i, 0]);
                            float[] ZDJDWCSZ = tj.GRZDJDWC(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GRPFSZ[i, 0]);
                            float[] ZDzqlFloat = tj.GRZYZQL(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), GRPFSZ[i, 0]);//返回数组分别为中央指导三天预报的最高、最低温度、晴雨准确率以及缺报率，主要计算技巧用晴雨准确率
                            float[] WDJQ = new float[6];//保存三天的最高、最低温度技巧
                            try
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    if (ZDJDWCSZ[j] == 0)
                                    {
                                        WDJQ[j] = 1.01F * 100;
                                    }
                                    else
                                    {
                                        WDJQ[j] = (ZDJDWCSZ[j] - GRJDWCSZ[j]) / ZDJDWCSZ[j];
                                        WDJQ[j] = (float)Math.Round(WDJQ[j] * 100, 2);
                                    }
                                }

                            }
                            catch (Exception)
                            {

                            }
                            float[] QYJQ = new float[3];
                            try
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    //if (ZDzqlFloat[2 + j * 3] == 100)
                                    //{
                                    //    QYJQ[j] = zqlFloat[2 + j * 3] - ZDzqlFloat[2 + j * 3];
                                    //    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                    //}
                                    //else if(zqlFloat[2 + j * 3]==100)
                                    //{
                                    //    zqlFloat[2 + j * 3] = 99.99F;
                                    //    QYJQ[j] = (zqlFloat[2 + j * 3] - ZDzqlFloat[2 + j * 3]) / (100 - ZDzqlFloat[2 + j * 3]);
                                    //    QYJQ[j] = (float)Math.Round(QYJQ[j] * 100, 2);
                                    //}
                                    //else
                                    //{
                                    //    QYJQ[j] = (zqlFloat[2 + j * 3] - ZDzqlFloat[2 + j * 3]) / (100 - ZDzqlFloat[2 + j * 3]);
                                    //    QYJQ[j] = (float)Math.Round(QYJQ[j] * 100, 2);
                                    //}

                                    QYJQ[j] = zqlFloat[2 + j * 3] - ZDzqlFloat[2 + j * 3];
                                    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                }
                            }
                            catch (Exception)
                            {

                            }
                            GRPFSZ[i, 9] = Convert.ToString(Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6) / 24, 2));//晴雨技巧
                            GRPFSZ[i, 10] = Convert.ToString(Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6) / 24, 2));//高温技巧
                            GRPFSZ[i, 11] = Convert.ToString(Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) / 24, 2));//低温技巧
                            GRPFSZ[i, 12] = Convert.ToString(Math.Round(Convert.ToSingle(0.4 * Convert.ToDouble(GRPFSZ[i, 9]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 10]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 11])), 2));//总技巧
                        }
                        Int16 ZBJS = 0;//值班基数
                        string[,] ZBSZ = tj.ZBXXTJ(Convert.ToDateTime(sDate.SelectedDate), Convert.ToDateTime(eDate.SelectedDate), ref ZBJS);
                        for (int j = 0; j < ZBSZ.GetLength(0); j++)
                        {
                            for (int k = 0; k < peoplesz.GetLength(0); k++)
                            {
                                if (ZBSZ[j, 0] == GRPFSZ[k, 0])
                                {
                                    GRPFSZ[k, 3] = ZBSZ[j, 1];//值班次数
                                    GRPFSZ[k, 4] = ZBJS.ToString();//
                                }
                            }
                        }

                        Int16 countInt = 0;//保存值班次数大于基数的人员的个数
                        for (int i = 0; i < peoplesz.GetLength(0); i++)
                        {
                            if (Convert.ToInt16(GRPFSZ[i, 3]) >= Convert.ToInt16(GRPFSZ[i, 4]))//如果值班次数大于值班基数
                            {
                                countInt++;

                            }
                            else//如果值班次数小于值班基数
                            {

                            }
                        }
                        double[] JQPJSZ = new double[countInt];
                        Int16 intLS = 0;
                        for (int i = 0; i < peoplesz.GetLength(0); i++)
                        {

                            if (Convert.ToInt16(GRPFSZ[i, 3]) >= Convert.ToInt16(GRPFSZ[i, 4]))//如果值班次数大于值班基数
                            {
                                JQPJSZ[intLS++] = Convert.ToDouble(GRPFSZ[i, 12]);

                            }
                            else//如果值班次数小于值班基数排名999
                            {
                                GRPFSZ[i, 1] = "999";
                            }
                            if (GRPFSZ[i, 5] == "NaN")
                            {
                                GRPFSZ[i, 1] = "999";
                            }
                        }
                        Array.Sort(JQPJSZ);
                        for (int i = 0; i < JQPJSZ.Length; i++)
                        {
                            for (int j = 0; j < peoplesz.GetLength(0); j++)
                            {

                                if (Convert.ToInt16(GRPFSZ[j, 3]) >= Convert.ToInt16(GRPFSZ[j, 4]))//如果值班次数大于值班基数
                                {
                                    if (JQPJSZ[i] == Convert.ToDouble(GRPFSZ[j, 12]))
                                    {
                                        GRPFSZ[j, 1] = (countInt - i).ToString();
                                    }

                                }
                                else//如果值班次数小于值班基数排名999
                                {

                                }
                            }
                        }
                        List<城镇预报120小时个人评分ViewModel> lisLS = new List<城镇预报120小时个人评分ViewModel>();
                        for (int i = 0; i < peoplesz.GetLength(0); i++)
                        {
                            lisLS.Add(new 城镇预报120小时个人评分ViewModel()
                            {
                                PeopleID = GRPFSZ[i, 0],
                                PM = Convert.ToInt16(GRPFSZ[i, 1]),
                                PeopleName = GRPFSZ[i, 2],
                                ZBCS = Convert.ToInt16(GRPFSZ[i, 3]),
                                ZBJS = Convert.ToInt16(GRPFSZ[i, 4]),
                                QYPF = Convert.ToSingle(GRPFSZ[i, 5]),
                                GWPF = Convert.ToSingle(GRPFSZ[i, 6]),
                                DWPF = Convert.ToSingle(GRPFSZ[i, 7]),
                                ZHPF = Convert.ToSingle(GRPFSZ[i, 8]),
                                QYJQ = Convert.ToSingle(GRPFSZ[i, 9]),
                                GWJQ = Convert.ToSingle(GRPFSZ[i, 10]),
                                DWJQ = Convert.ToSingle(GRPFSZ[i, 11]),
                                AllJQ = Convert.ToSingle(GRPFSZ[i, 12]),

                            });
                        }
                        for (int i = 0; i < lisLS.Count; i++)
                        {
                            if (lisLS[i].DWJQ < 0 || lisLS[i].GWJQ < 0 || lisLS[i].QYJQ < 0 || lisLS[i].AllJQ < 0)
                            {
                                lisLS[i].PM = 999;
                            }
                        }
                        lisLS = lisLS.OrderBy(y => y.PM).ThenByDescending(y => y.AllJQ).ToList();
                        Int16 myPM = 1;
                        foreach (var item in lisLS)
                        {
                            if (item.PM < 999)
                            {
                                item.PM = myPM++;
                            }
                        }
                        foreach (var item in lisLS)
                        {
                            评分列表.Add(item);
                        }
                    }
                    else
                    {
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = "所选时间段没有登录记录，请重新选择起止时间",
                            Header = "警告"
                        });

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
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "警告"
                });
            }

            RadSplashScreenManager.Close();
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
                城镇预报120小时个人评分ViewModel[] dcsz = 评分列表.ToArray();
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
                    cellSheet.Cells[0, 0].PutValue("预报员ID");
                    cellSheet.Cells[0, 1].PutValue("姓名");
                    cellSheet.Cells[0, 2].PutValue("排名");
                    cellSheet.Cells[0, 3].PutValue("旗县名称");
                    cellSheet.Cells[0, 4].PutValue("值班次数");
                    cellSheet.Cells[0, 5].PutValue("值班基数");
                    cellSheet.Cells[0, 6].PutValue("晴雨准确率");
                    cellSheet.Cells[0, 7].PutValue("高温准确率");
                    cellSheet.Cells[0, 8].PutValue("低温准确率");
                    cellSheet.Cells[0, 9].PutValue("平均准确率");
                    cellSheet.Cells[0, 10].PutValue("晴雨技巧");
                    cellSheet.Cells[0, 11].PutValue("高温技巧");
                    cellSheet.Cells[0, 12].PutValue("低温技巧");
                    cellSheet.Cells[0, 13].PutValue("技巧总评分");

                    for (int i = 0; i < dcsz.Length; i++)
                    {
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].PeopleID);
                        cellSheet.Cells[i + 1, 0].SetStyle(style1);
                        cellSheet.Cells[i + 1, 1].PutValue(dcsz[i].PeopleName);
                        cellSheet.Cells[i + 1, 1].SetStyle(style1);
                        cellSheet.Cells[i + 1, 2].PutValue(dcsz[i].PM);
                        cellSheet.Cells[i + 1, 2].SetStyle(style1);
                        cellSheet.Cells[i + 1, 3].PutValue(dcsz[i].Name);
                        cellSheet.Cells[i + 1, 3].SetStyle(style1);
                        cellSheet.Cells[i + 1, 4].PutValue(dcsz[i].ZBCS);
                        cellSheet.Cells[i + 1, 4].SetStyle(style1);
                        cellSheet.Cells[i + 1, 5].PutValue(dcsz[i].ZBJS);
                        cellSheet.Cells[i + 1, 5].SetStyle(style1);
                        cellSheet.Cells[i + 1, 6].PutValue(Math.Round(dcsz[i].QYPF, 2));
                        cellSheet.Cells[i + 1, 6].SetStyle(style1);
                        cellSheet.Cells[i + 1, 7].PutValue(Math.Round(dcsz[i].GWPF, 2));
                        cellSheet.Cells[i + 1, 7].SetStyle(style1);
                        cellSheet.Cells[i + 1, 8].PutValue(Math.Round(dcsz[i].DWPF, 2));
                        cellSheet.Cells[i + 1, 8].SetStyle(style1);
                        cellSheet.Cells[i + 1, 9].PutValue(Math.Round(dcsz[i].ZHPF, 2));
                        cellSheet.Cells[i + 1, 9].SetStyle(style1);
                        cellSheet.Cells[i + 1, 10].PutValue(Math.Round(dcsz[i].QYJQ, 2));
                        cellSheet.Cells[i + 1, 10].SetStyle(style1);
                        cellSheet.Cells[i + 1, 11].PutValue(Math.Round(dcsz[i].GWJQ, 2));
                        cellSheet.Cells[i + 1, 11].SetStyle(style1);
                        cellSheet.Cells[i + 1, 12].PutValue(Math.Round(dcsz[i].DWJQ, 2));
                        cellSheet.Cells[i + 1, 12].SetStyle(style1);
                        cellSheet.Cells[i + 1, 13].PutValue(Math.Round(dcsz[i].AllJQ, 2));
                        cellSheet.Cells[i + 1, 13].SetStyle(style1);
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


    }


}
