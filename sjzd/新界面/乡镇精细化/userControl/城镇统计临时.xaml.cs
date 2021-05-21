using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using sjzd.新界面.类;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 集体评分逐日查询页.xaml 的交互逻辑
    /// </summary>
    public partial class 城镇统计临时 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private readonly ObservableCollection<城镇预报集体评分ViewModel> 评分列表 = new My城镇统计临时ViewModel().Clubs;
        private string xlsPath = "";
        public 城镇统计临时()
        {
            InitializeComponent();
            BTLabel.Content = "城镇统计临时";
            DateTime dt = DateTime.Now;
            sDate.SelectedDate = dt.AddDays(1 - dt.Day).AddMonths(-1);
            JTPFList.ItemsSource = 评分列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            gwCSH("08");
        }
        void gwCSH(string sc)
        {
            try
            {
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\城镇预报\GWList.txt",
                    Encoding.GetEncoding("GB2312")))
                {
                    string line = "";

                    Int16 intCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0 && line.Split('=')[0] == sc + "岗位列表")
                        {
                            string[] szls = line.Split('=')[1].Split(',');
                            foreach (string ssls in szls)
                            {
                                mydic.Add(intCount++, ssls);

                            }
                            break;
                        }

                    }
                    gwchoose.ItemsSource = mydic;
                    gwchoose.SelectedValuePath = "Key";
                    gwchoose.DisplayMemberPath = "Value";
                }
                gwchoose.SelectedValue = 0;
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            if ((sDate.SelectedDate.ToString().Length != 0) && (eDate.SelectedDate.ToString().Length != 0))
            {
                评分列表.Clear();
                if ((sDate.SelectedDate.ToString().Length != 0) && (eDate.SelectedDate.ToString().Length != 0))
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
                   
                    DateTime mySDate = Convert.ToDateTime(sDate.SelectedDate);
                    DateTime myEDate = Convert.ToDateTime(eDate.SelectedDate);
                    string startDate = mySDate.ToString("yyyy-MM-dd");
                    string endDate = myEDate.ToString("yyyy-MM-dd");
                    BTLabel.Content = startDate + "至" + endDate + gwchoose.Text + "城镇统计临时";
                    //gwchoose.Text
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

                    统计临时 tj = new 统计临时();


                    for (int i = 0; i < intQXGS + 1; i++)
                    {
                        if (i < intQXGS)
                        {
                            float[] zqlFloat = tj.GWQXZQL120(mySDate, myEDate, QXID[i],gwchoose.Text,scchoose.Text);//返回数组分别为5天预报的最高、最低温度、晴雨准确率以及缺报率
                            float[] XSList = new float[8];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                            XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6 + zqlFloat[11] * 2 ) / 26, 2);
                            XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6 + zqlFloat[9] * 2 ) / 26, 2);
                            XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6 + zqlFloat[10] * 2 ) / 26, 2);
                            XSList[3] = (float)Math.Round(Convert.ToSingle(0.2 * XSList[0] + 0.4 * XSList[1] + 0.4 * XSList[2]), 2);//总评分
                            float[] QXJDWCSZ = tj.GWQXJDWC120(mySDate, myEDate, QXID[i], gwchoose.Text, scchoose.Text);
                            float[] SJJDWCSZ = tj.GWQXZDJDWC120(mySDate, myEDate, QXID[i], gwchoose.Text, scchoose.Text);
                            float[] SJzqlFloat = tj.GWQXZYZQL120(mySDate, myEDate, QXID[i], gwchoose.Text, scchoose.Text);
                            float[] WDJQ = new float[10];//保存5天的最高、最低温度技巧
                            try
                            {
                                for (int j = 0; j < 10; j++)
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
                            float[] QYJQ = new float[5];
                            try
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            XSList[4] = (float)Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6 + QYJQ[3] * 2 ) / 26, 2);//晴雨技巧
                            XSList[5] = (float)Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6 + WDJQ[6] * 2 ) / 26, 2);//高温技巧
                            XSList[6] = (float)Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) + WDJQ[7] * 2 / 26, 2);//低温技巧
                            XSList[7] = (float)Math.Round(Convert.ToSingle(0.2 * XSList[4] + 0.4 * XSList[5] + 0.4 * XSList[6]), 2);//总技巧
                            城镇预报集体评分ViewModel myitem = new 城镇预报集体评分ViewModel(QXName[i], XSList[0], XSList[1], XSList[2], XSList[3],
                                XSList[4], XSList[5], XSList[6], XSList[7]);
                            myitem.update(zqlFloat[2], zqlFloat[0], zqlFloat[1], zqlFloat[5], zqlFloat[3], zqlFloat[4],
                                zqlFloat[8], zqlFloat[6], zqlFloat[7], zqlFloat[11], zqlFloat[9], zqlFloat[10],
                                zqlFloat[14], zqlFloat[12], zqlFloat[13], QYJQ[0], WDJQ[0], WDJQ[1], QYJQ[1], WDJQ[2],
                                WDJQ[3], QYJQ[2], WDJQ[4], WDJQ[5], QYJQ[3], WDJQ[6], WDJQ[7], QYJQ[4], WDJQ[8],
                                WDJQ[9]);
                            评分列表.Add(myitem);
                        }
                        else
                        {

                            float[] zqlFloat = tj.GWZQL120(mySDate, myEDate, gwchoose.Text, scchoose.Text);//返回数组分别为5天预报的最高、最低温度、晴雨准确率以及缺报率
                            float[] XSList = new float[8];//保存该旗县的晴雨评分、高温评分、低温评分、综合总评分、晴雨技巧、高温技巧、低温技巧、以及技巧总评分。
                            XSList[0] = (float)Math.Round((zqlFloat[2] * 10 + zqlFloat[5] * 8 + zqlFloat[8] * 6 + zqlFloat[11] * 2 ) / 26, 2);
                            XSList[1] = (float)Math.Round((zqlFloat[0] * 10 + zqlFloat[3] * 8 + zqlFloat[6] * 6 + zqlFloat[9] * 2 ) / 26, 2);
                            XSList[2] = (float)Math.Round((zqlFloat[1] * 10 + zqlFloat[4] * 8 + zqlFloat[7] * 6 + zqlFloat[10] * 2 ) / 26, 2);
                            XSList[3] = (float)Math.Round(Convert.ToSingle(0.2 * XSList[0] + 0.4 * XSList[1] + 0.4 * XSList[2]), 2);//总评分
                            float[] QXJDWCSZ = tj.GWJDWC120(mySDate, myEDate, gwchoose.Text, scchoose.Text);
                            float[] SJJDWCSZ = tj.GWZDJDWC120(mySDate, myEDate,  gwchoose.Text, scchoose.Text);
                            float[] SJzqlFloat = tj.GWZYZQL120(mySDate, myEDate,  gwchoose.Text, scchoose.Text);
                            float[] WDJQ = new float[10];//保存5天的最高、最低温度技巧
                            try
                            {
                                for (int j = 0; j < 10; j++)
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
                            float[] QYJQ = new float[5];
                            try
                            {
                                for (int j = 0; j < 3; j++)
                                {
                                    QYJQ[j] = zqlFloat[2 + j * 3] - SJzqlFloat[2 + j * 3];
                                    QYJQ[j] = (float)Math.Round(QYJQ[j], 2);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                            XSList[4] = (float)Math.Round((QYJQ[0] * 10 + QYJQ[1] * 8 + QYJQ[2] * 6 + QYJQ[3] * 2 ) / 26, 2);//晴雨技巧
                            XSList[5] = (float)Math.Round((WDJQ[0] * 10 + WDJQ[2] * 8 + WDJQ[4] * 6 + WDJQ[6] * 2 ) / 26, 2);//高温技巧
                            XSList[6] = (float)Math.Round((WDJQ[1] * 10 + WDJQ[3] * 8 + WDJQ[5] * 6) + WDJQ[7] * 2  / 26, 2);//低温技巧
                            XSList[7] = (float)Math.Round(Convert.ToSingle(0.2 * XSList[4] + 0.4 * XSList[5] + 0.4 * XSList[6]), 2);//总技巧
                            城镇预报集体评分ViewModel myitem = new 城镇预报集体评分ViewModel("呼和浩特市", XSList[0], XSList[1], XSList[2], XSList[3],
                                XSList[4], XSList[5], XSList[6], XSList[7]);
                            myitem.update(zqlFloat[2], zqlFloat[0], zqlFloat[1], zqlFloat[5], zqlFloat[3], zqlFloat[4],
                                zqlFloat[8], zqlFloat[6], zqlFloat[7], zqlFloat[11], zqlFloat[9], zqlFloat[10],
                                zqlFloat[14], zqlFloat[12], zqlFloat[13], QYJQ[0], WDJQ[0], WDJQ[1], QYJQ[1], WDJQ[2],
                                WDJQ[3], QYJQ[2], WDJQ[4], WDJQ[5], QYJQ[3], WDJQ[6], WDJQ[7], QYJQ[4], WDJQ[8],
                                WDJQ[9]);
                            评分列表.Add(myitem);

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
                城镇预报集体评分ViewModel[] dcsz = 评分列表.ToArray();
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
                    cellSheet.Cells[0, 9].PutValue("24小时最低温度准确率");
                    cellSheet.Cells[0, 10].PutValue("24小时最高温度准确率");
                    cellSheet.Cells[0, 11].PutValue("24小时晴雨准确率");
                    cellSheet.Cells[0, 12].PutValue("48小时最低温度准确率");
                    cellSheet.Cells[0, 13].PutValue("48小时最高温度准确率");
                    cellSheet.Cells[0, 14].PutValue("48小时晴雨准确率");
                    cellSheet.Cells[0, 15].PutValue("72小时最低温度准确率");
                    cellSheet.Cells[0, 16].PutValue("72小时最高温度准确率");
                    cellSheet.Cells[0, 17].PutValue("72小时晴雨准确率");
                    cellSheet.Cells[0, 18].PutValue("96小时最低温度准确率");
                    cellSheet.Cells[0, 19].PutValue("96小时最高温度准确率");
                    cellSheet.Cells[0, 20].PutValue("96小时晴雨准确率");
                    cellSheet.Cells[0, 21].PutValue("120小时最低温度准确率");
                    cellSheet.Cells[0, 22].PutValue("120小时最高温度准确率");
                    cellSheet.Cells[0, 23].PutValue("120小时晴雨准确率");
                    cellSheet.Cells[0, 24].PutValue("24小时最低温度技巧");
                    cellSheet.Cells[0, 25].PutValue("24小时最高温度技巧");
                    cellSheet.Cells[0, 26].PutValue("24小时晴雨技巧");
                    cellSheet.Cells[0, 27].PutValue("48小时最低温度技巧");
                    cellSheet.Cells[0, 28].PutValue("48小时最高温度技巧");
                    cellSheet.Cells[0, 29].PutValue("48小时晴雨技巧");
                    cellSheet.Cells[0, 30].PutValue("72小时最低温度技巧");
                    cellSheet.Cells[0, 31].PutValue("72小时最高温度技巧");
                    cellSheet.Cells[0, 32].PutValue("72小时晴雨技巧");
                    cellSheet.Cells[0, 33].PutValue("96小时最低温度技巧");
                    cellSheet.Cells[0, 34].PutValue("96小时最高温度技巧");
                    cellSheet.Cells[0, 35].PutValue("96小时晴雨技巧");

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
                        cellSheet.Cells[i + 1, 9].PutValue(Math.Round(dcsz[i].SJ24TminZql, 2));
                        cellSheet.Cells[i + 1, 10].PutValue(Math.Round(dcsz[i].SJ24TmaxZql, 2));
                        cellSheet.Cells[i + 1, 11].PutValue(Math.Round(dcsz[i].SJ24Qyzql, 2));
                        cellSheet.Cells[i + 1, 12].PutValue(Math.Round(dcsz[i].SJ48TminZql, 2));
                        cellSheet.Cells[i + 1, 13].PutValue(Math.Round(dcsz[i].SJ48TmaxZql, 2));
                        cellSheet.Cells[i + 1, 14].PutValue(Math.Round(dcsz[i].SJ48Qyzql, 2));
                        cellSheet.Cells[i + 1, 15].PutValue(Math.Round(dcsz[i].SJ72TminZql, 2));
                        cellSheet.Cells[i + 1, 16].PutValue(Math.Round(dcsz[i].SJ72TmaxZql, 2));
                        cellSheet.Cells[i + 1, 17].PutValue(Math.Round(dcsz[i].SJ72Qyzql, 2));
                        cellSheet.Cells[i + 1, 18].PutValue(Math.Round(dcsz[i].SJ96TminZql, 2));
                        cellSheet.Cells[i + 1, 19].PutValue(Math.Round(dcsz[i].SJ96TmaxZql, 2));
                        cellSheet.Cells[i + 1, 20].PutValue(Math.Round(dcsz[i].SJ96Qyzql, 2));
                        cellSheet.Cells[i + 1, 21].PutValue(Math.Round(dcsz[i].SJ120TminZql, 2));
                        cellSheet.Cells[i + 1, 22].PutValue(Math.Round(dcsz[i].SJ120TmaxZql, 2));
                        cellSheet.Cells[i + 1, 23].PutValue(Math.Round(dcsz[i].SJ120Qyzql, 2));
                        cellSheet.Cells[i + 1, 24].PutValue(Math.Round(dcsz[i].SJ24TminJq, 2));
                        cellSheet.Cells[i + 1, 25].PutValue(Math.Round(dcsz[i].SJ24TmaxJq, 2));
                        cellSheet.Cells[i + 1, 26].PutValue(Math.Round(dcsz[i].SJ24Qyjq, 2));
                        cellSheet.Cells[i + 1, 27].PutValue(Math.Round(dcsz[i].SJ48TminJq, 2));
                        cellSheet.Cells[i + 1, 28].PutValue(Math.Round(dcsz[i].SJ48TmaxJq, 2));
                        cellSheet.Cells[i + 1, 29].PutValue(Math.Round(dcsz[i].SJ48Qyjq, 2));
                        cellSheet.Cells[i + 1, 30].PutValue(Math.Round(dcsz[i].SJ72TminJq, 2));
                        cellSheet.Cells[i + 1, 31].PutValue(Math.Round(dcsz[i].SJ72TmaxJq, 2));
                        cellSheet.Cells[i + 1, 32].PutValue(Math.Round(dcsz[i].SJ72Qyjq, 2));
                        cellSheet.Cells[i + 1, 33].PutValue(Math.Round(dcsz[i].SJ96TminJq, 2));
                        cellSheet.Cells[i + 1, 34].PutValue(Math.Round(dcsz[i].SJ96TmaxJq, 2));
                        cellSheet.Cells[i + 1, 35].PutValue(Math.Round(dcsz[i].SJ96Qyjq, 2));

                        for (int j = 9; j < 39; j++)
                        {
                            cellSheet.Cells[i + 1, j].SetStyle(style1);
                        }
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
    public class My城镇统计临时ViewModel : ViewModelBase
    {
        private ObservableCollection<城镇预报集体评分ViewModel> clubs;

        public ObservableCollection<城镇预报集体评分ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<城镇预报集体评分ViewModel> CreateClubs()
        {
            ObservableCollection<城镇预报集体评分ViewModel> clubs = new ObservableCollection<城镇预报集体评分ViewModel>();
            return clubs;
        }
    }
}
