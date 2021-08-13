﻿using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
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
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 个人评分页new : UserControl
    {

        private SplashScreenDataContext splashScreenDataContext;
        private ObservableCollection<乡镇精细化预报个人评分ViewModel> 评分列表 = new My乡镇精细化个人评分ViewModel().Clubs;
        private string xlsPath = "";
        public 个人评分页new()
        {
            InitializeComponent();
            BTLabel.Content = "乡镇精细化个人评分查询";
            string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
            string QXNameDZPath = Environment.CurrentDirectory + @"\设置文件\旗县名称显示对照.txt";
            Int16 intQXGS = 0;

            try
            {
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
                using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < QXName.Length; i++)
                        {
                            if (line1.Split('=')[0] == QXName[i])
                            {
                                QXName[i] = line1.Split('=')[1];
                            }
                        }
                    }
                }
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                for (int i = 0; i <= intQXGS; i++)
                {
                    if (i == 0)
                    {
                        mydic.Add(i, "全市");
                    }
                    else
                    {
                        mydic.Add(i, QXName[i - 1]);
                    }
                }
                QXSelect.ItemsSource = mydic;
                QXSelect.SelectedValuePath = "Key";
                QXSelect.DisplayMemberPath = "Value";
                QXSelect.SelectedValue = 0;
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
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                评分列表.Clear();
                string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                string QXNameDZPath = Environment.CurrentDirectory + @"\设置文件\旗县名称显示对照.txt";
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
                BTLabel.Content = startDate + "至" + endDate + QXSelect.Text + "乡镇精细化个人评分查询";

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
                using (StreamReader sr = new StreamReader(QXNameDZPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";
                    while ((line1 = sr.ReadLine()) != null)
                    {
                        for (int i = 0; i < QXName.Length; i++)
                        {
                            if (line1.Split('=')[0] == QXName[i])
                            {
                                QXName[i] = line1.Split('=')[1];
                            }
                        }
                    }
                }
                TJ tj = new TJ();

                string[,] PeopleListSZ = tj.PeoleListTJ(startDate, endDate);//指定时间所有旗县所有预报员ID列表
                string[,] GRPFSZ = new string[PeopleListSZ.GetLength(0), 14];//数组保存个人评分信息
                for (int i = 0; i < PeopleListSZ.GetLength(0); i++)
                {
                    if (PeopleListSZ[i, 0] == "3015404")
                    {

                    }
                    GRPFSZ[i, 0] = PeopleListSZ[i, 0];
                    GRPFSZ[i, 13] = PeopleListSZ[i, 1];
                    string qbdate = "";
                    float[] zqlFloat = tj.GRZQL(startDate, endDate, PeopleListSZ[i, 0], ref qbdate);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率
                                                                                                    //zqlFloat数组与XSList数组的高低温晴雨准确率不一致，计算时需略作调整
                    GRPFSZ[i, 5] = Convert.ToString(Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6) / 24, 2));
                    GRPFSZ[i, 6] = Convert.ToString(Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6) / 24, 2));
                    GRPFSZ[i, 7] = Convert.ToString(Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6) / 24, 2));
                    GRPFSZ[i, 8] = Convert.ToString(Math.Round(Convert.ToSingle(0.4 * Convert.ToDouble(GRPFSZ[i, 5]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 6]) + 0.3 * Convert.ToDouble(GRPFSZ[i, 7])), 2));//总评分
                    float[] GRJDWCSZ = tj.GRJDWC(startDate, endDate, PeopleListSZ[i, 0]);
                    float[] SJJDWCSZ = tj.GRSJJDWC(startDate, endDate, PeopleListSZ[i, 0]);
                    float[] SJzqlFloat = tj.GRSJZQL(startDate, endDate, PeopleListSZ[i, 0], qbdate);//返回数组分别为三天预报的最高、最低温度、晴雨准确率以及缺报率，主要计算技巧用晴雨准确率
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
                                WDJQ[j] = (SJJDWCSZ[j] - GRJDWCSZ[j]) / SJJDWCSZ[j];
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
                            //if (SJzqlFloat[2 + j * 3] == 100)
                            //{
                            //    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                            //    QYJQ[j] = (float)Math.Round(QYJQ[j] , 2);
                            //}
                            //else
                            //{
                            //    QYJQ[j] = (zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3]) / (100 - SJzqlFloat[2 + j * 3]);
                            //    QYJQ[j] = (float)Math.Round(QYJQ[j] * 100, 2);
                            //}
                            QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
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

                for (int i = 0; i < intQXGS; i++)
                {
                    Int16 ZBJS = 0;//值班基数
                    string[,] ZBSZ = tj.ZBXXTJ(startDate, endDate, QXID[i], ref ZBJS);
                    for (int j = 0; j < ZBSZ.GetLength(0); j++)
                    {
                        for (int k = 0; k < PeopleListSZ.GetLength(0); k++)
                        {
                            if (ZBSZ[j, 0] == GRPFSZ[k, 0])
                            {
                                GRPFSZ[k, 2] = QXName[i];//所属旗县名称
                                GRPFSZ[k, 3] = ZBSZ[j, 1];//值班次数
                                GRPFSZ[k, 4] = ZBJS.ToString();//
                            }
                        }
                    }

                }
                Int16 countInt = 0;//保存值班次数大于基数的人员的个数
                for (int i = 0; i < PeopleListSZ.GetLength(0); i++)
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
                for (int i = 0; i < PeopleListSZ.GetLength(0); i++)
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
                    for (int j = 0; j < PeopleListSZ.GetLength(0); j++)
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

                List<乡镇精细化预报个人评分ViewModel> lisLS = new List<乡镇精细化预报个人评分ViewModel>();
                for (int i = 0; i < PeopleListSZ.GetLength(0); i++)
                {
                    lisLS.Add(new 乡镇精细化预报个人评分ViewModel(GRPFSZ[i, 2], GRPFSZ[i, 0], GRPFSZ[i, 13], Convert.ToInt16(GRPFSZ[i, 1]), Convert.ToInt16(GRPFSZ[i, 3]), Convert.ToInt16(GRPFSZ[i, 4]), Convert.ToSingle(GRPFSZ[i, 5]), Convert.ToSingle(GRPFSZ[i, 6]), Convert.ToSingle(GRPFSZ[i, 7]), Convert.ToSingle(GRPFSZ[i, 8]), Convert.ToSingle(GRPFSZ[i, 9]), Convert.ToSingle(GRPFSZ[i, 10]), Convert.ToSingle(GRPFSZ[i, 11]), Convert.ToSingle(GRPFSZ[i, 12])));
                }
                foreach (var t in lisLS)
                {
                    if (t.PM<999&&(t.DWJQ < 0 || t.GWJQ < 0 || t.QYJQ < 0 || t.AllJQ < 0))
                    {
                        t.PM = 998;
                    }
                }

                if (QXSelect.Text == "全市")
                {

                    lisLS = lisLS.OrderBy(p => p.PM).ThenByDescending(y => y.AllJQ).ToList();//按照排名升序排列
                    Int16 myPM = 1;
                    foreach (var item in lisLS)
                    {
                        if (item.PM < 900)
                        {
                            item.PM = myPM++;
                        }
                    }
                    foreach (var item in lisLS)
                    {
                        if (item.PM == 998)
                        {
                            item.PM = myPM++;
                        }
                    }
                    lisLS.ForEach(p => 评分列表.Add(p));
                }
                else
                {
                    lisLS = lisLS.OrderBy(p => p.PM).ThenByDescending(y => y.AllJQ).Where(c => c.Name == QXSelect.Text).ToList();//按照排名升序排列
                    Int16 myPM = 1;
                    foreach (var item in lisLS)
                    {
                        if (item.PM < 900)
                        {
                            item.PM = myPM++;
                        }
                    }
                    foreach (var item in lisLS)
                    {
                        if (item.PM == 998)
                        {
                            item.PM = myPM++;
                        }
                    }
                    lisLS.ForEach(p => 评分列表.Add(p));

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
            if (e.DialogResult == true) 静态类.OpenBrowser(xlsPath);
        }
        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xls";
                乡镇精细化预报个人评分ViewModel[] dcsz = 评分列表.ToArray();
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

    public class My乡镇精细化个人评分ViewModel : ViewModelBase
    {
        private ObservableCollection<乡镇精细化预报个人评分ViewModel> clubs;

        public ObservableCollection<乡镇精细化预报个人评分ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<乡镇精细化预报个人评分ViewModel> CreateClubs()
        {
            ObservableCollection<乡镇精细化预报个人评分ViewModel> clubs = new ObservableCollection<乡镇精细化预报个人评分ViewModel>();
            return clubs;
        }
    }
}
