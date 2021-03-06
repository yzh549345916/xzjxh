﻿using Aspose.Cells;
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

namespace sjzd
{
    /// <summary>
    /// 集体评分逐日查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 集体评分逐日查询页new : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private readonly ObservableCollection<乡镇精细化预报集体评分ViewModel> 评分列表 = new My乡镇精细化集体评分ViewModel().Clubs;
        private string xlsPath = "";
        public 集体评分逐日查询页new()
        {
            InitializeComponent();
            BTLabel.Content = "集体评分逐日查询";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            JTPFList.ItemsSource = 评分列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }


        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                评分列表.Clear();
                if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
                {
                    string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                    Int16 intQXGS = 0;
                    using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))//统计旗县个数
                    {
                        string line1 = "";
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            if (line1.Split(':')[0] == "旗县个数")
                            {
                                intQXGS = Convert.ToInt16(line1.Split(':')[1]);
                                break;
                            }
                        }
                    }
                    string[] QXID = new string[intQXGS];
                    string[] QXName = new string[intQXGS];
                    string[,] TJXX = new string[intQXGS + 1, 8];//行数为旗县个数加一，最后一行市局，列数为名称、晴雨评分、高温评分、低温评分、晴雨技巧、高温技巧、低温技巧、总技巧
                    string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                    string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                    BTLabel.Content = startDate + "至" + endDate + "全市各旗县集体评分";

                    using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))//第二行开始每两行为旗县及乡镇区站号列表
                    {
                        string line1 = "";
                        Int16 lineCount = 0;
                        Int16 count = 0;
                        while (lineCount < intQXGS * 2 + 1)
                        {
                            line1 = sr.ReadLine();
                            if (lineCount > 0)
                            {
                                if (lineCount % 2 == 1)
                                {
                                    QXName[count] = line1.Split(',')[0];
                                }
                                else
                                {
                                    QXID[count++] = line1.Split(',')[0];

                                }
                            }
                            lineCount++;
                        }
                    }

                    TJ tj = new TJ();


                    for (int i = 0; i < intQXGS + 1; i++)
                    {
                        if (i < intQXGS)
                        {
                            float[] zqlFloat = tj.QXZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率
                            float[] XSList = new float[8];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                            //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                            XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2);
                            XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2);
                            XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2);
                            XSList[3] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[0] + 0.3 * XSList[1] + 0.3 * XSList[2]), 2);//总评分
                            float[] QXJDWCSZ = tj.QXJDWC(startDate, endDate, QXID[i]);
                            float[] SJJDWCSZ = tj.SJJDWC(startDate, endDate, QXID[i]);
                            float[] SJzqlFloat = tj.SJZQL(startDate, endDate, QXID[i]);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率，主要计算技巧用晴雨准确率
                            float[] WDJQ = new float[6];//保存三天的最高、最低温度技巧
                            try
                            {
                                for (int j = 0; j < 6; j++)
                                {
                                    if (SJJDWCSZ[j] == 0)
                                    {
                                        WDJQ[j] = 1.01F * 100;
                                    }
                                    else
                                    {
                                        WDJQ[j] = (SJJDWCSZ[j] - QXJDWCSZ[j]) / SJJDWCSZ[j];
                                        WDJQ[j] = (float)Math.Round(WDJQ[j] * 100, 2);
                                    }
                                }

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            float[] QYJQ = new float[3];
                            try
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    //if(SJzqlFloat[2 + j * 3] == 100)
                                    //{
                                    //    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    //    QYJQ[j] = (float)Math.Round(QYJQ[j] , 2);
                                    //}
                                    //else
                                    //{
                                    //    QYJQ[j] = (zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3]) / (100 - SJzqlFloat[2 + j * 3]);
                                    //    QYJQ[j]= (float)Math.Round(QYJQ[j] * 100, 2);
                                    //}
                                    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }

                            XSList[4] = (float)Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6) / 24, 2);//晴雨技巧
                            XSList[5] = (float)Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6) / 24, 2);//高温技巧
                            XSList[6] = (float)Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) / 24, 2);//低温技巧
                            XSList[7] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[4] + 0.3 * XSList[5] + 0.3 * XSList[6]), 2);//总技巧
                            评分列表.Add(new 乡镇精细化预报集体评分ViewModel(QXName[i], XSList[0], XSList[1], XSList[2], XSList[3], XSList[4], XSList[5], XSList[6], XSList[7]));
                        }
                        else
                        {


                            float[] zqlFloat = tj.SJQSZQL(startDate, endDate);
                            float[] XSList = new float[4];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                            //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                            XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2);
                            XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2);
                            XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2);
                            XSList[3] = (float)Math.Round(Convert.ToSingle(0.4 * XSList[0] + 0.3 * XSList[1] + 0.3 * XSList[2]), 2);//总评分
                            评分列表.Add(new 乡镇精细化预报集体评分ViewModel("市台", XSList[0], XSList[1], XSList[2], XSList[3], 0, 0, 0, 0));
                        }
                    }
                }
                else
                {
                    RadWindow.Alert("请选择起止时间");
                }
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
                乡镇精细化预报集体评分ViewModel[] dcsz = 评分列表.ToArray();
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
                    cellSheet.Cells[0, 0].PutValue("旗县名称");
                    cellSheet.Cells[0, 1].PutValue("晴雨准确率");
                    cellSheet.Cells[0, 2].PutValue("高温准确率");
                    cellSheet.Cells[0, 3].PutValue("低温准确率");
                    cellSheet.Cells[0, 4].PutValue("平均准确率");
                    cellSheet.Cells[0, 5].PutValue("晴雨技巧");
                    cellSheet.Cells[0, 6].PutValue("高温技巧");
                    cellSheet.Cells[0, 7].PutValue("低温技巧");
                    cellSheet.Cells[0, 8].PutValue("技巧总评分");


                    for (int i = 0; i < dcsz.Length; i++)
                    {
                        cellSheet.Cells[i + 1, 0].PutValue(dcsz[i].Name);
                        cellSheet.Cells[i + 1, 0].SetStyle(style1);
                        cellSheet.Cells[i + 1, 1].PutValue(Math.Round(dcsz[i].QYPF, 2));
                        cellSheet.Cells[i + 1, 1].SetStyle(style1);
                        cellSheet.Cells[i + 1, 2].PutValue(Math.Round(dcsz[i].GWPF, 2));
                        cellSheet.Cells[i + 1, 2].SetStyle(style1);
                        cellSheet.Cells[i + 1, 3].PutValue(Math.Round(dcsz[i].DWPF, 2));
                        cellSheet.Cells[i + 1, 3].SetStyle(style1);
                        cellSheet.Cells[i + 1, 4].PutValue(Math.Round(dcsz[i].ZHPF, 2));
                        cellSheet.Cells[i + 1, 4].SetStyle(style1);
                        cellSheet.Cells[i + 1, 5].PutValue(Math.Round(dcsz[i].QYJQ, 2));
                        cellSheet.Cells[i + 1, 5].SetStyle(style1);
                        cellSheet.Cells[i + 1, 6].PutValue(Math.Round(dcsz[i].GWJQ, 2));
                        cellSheet.Cells[i + 1, 6].SetStyle(style1);
                        cellSheet.Cells[i + 1, 7].PutValue(Math.Round(dcsz[i].DWJQ, 2));
                        cellSheet.Cells[i + 1, 7].SetStyle(style1);
                        cellSheet.Cells[i + 1, 8].PutValue(Math.Round(dcsz[i].AllJQ, 2));
                        cellSheet.Cells[i + 1, 8].SetStyle(style1);
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
    public class My乡镇精细化集体评分ViewModel : ViewModelBase
    {
        private ObservableCollection<乡镇精细化预报集体评分ViewModel> clubs;

        public ObservableCollection<乡镇精细化预报集体评分ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<乡镇精细化预报集体评分ViewModel> CreateClubs()
        {
            ObservableCollection<乡镇精细化预报集体评分ViewModel> clubs = new ObservableCollection<乡镇精细化预报集体评分ViewModel>();
            return clubs;
        }
    }
}
