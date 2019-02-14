using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Collections.ObjectModel;

namespace LS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        public MainWindow()
        {
            InitializeComponent();
            t1.Text = "✔";
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            eDate.BlackoutDates.Add(dr2);//结束时间不可选的范围
        }

        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                Int16 intQXGS = 0;
                using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//统计旗县个数
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
                string[,] TJXX = new string[intQXGS+1, 10];//行数为旗县个数加一，最后一行合计，列数为三天的最高最低晴雨准确率，为9.再加上一列名称，共10列
                string startDate = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                string endDate = Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd");
                BTLabel.Content = startDate + "至" + endDate + "市局乡镇精细化预报准确率";
                ObservableCollection<ZQLTJ1> sjzqlTJ1 = new ObservableCollection<ZQLTJ1>();
                using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//第二行开始每两行为旗县及乡镇区站号列表
                {
                    string line1 = "";
                    Int16 lineCount = 0;
                    Int16 count = 0;
                    while (lineCount < intQXGS * 2 + 1)
                    {
                        line1 = sr.ReadLine();
                        if (lineCount > 0)
                        {
                            if(lineCount%2==1)
                            {
                                QXName[count] = line1.Split(',')[0];
                            }
                            else
                            {
                                QXID[count++]= line1.Split(',')[0];
                                
                            }
                        }
                        lineCount++;
                    }
                }
                TJ tj = new TJ();
                for (int i=0;i<intQXGS+1;i++)
                {
                    if (i < intQXGS)
                    {
                        float[] zqlFloat = tj.SJZQL(startDate, endDate, QXID[i]);
                        sjzqlTJ1.Add(new ZQLTJ1()
                        {
                            Name = QXName[i],
                            SJ24TmaxZQL = zqlFloat[0],
                            SJ24TminZQL = zqlFloat[1],
                            SJ24QYZQL = zqlFloat[2],
                            SJ48TmaxZQL = zqlFloat[3],
                            SJ48TminZQL = zqlFloat[4],
                            SJ48QYZQL = zqlFloat[5],
                            SJ72TmaxZQL = zqlFloat[6],
                            SJ72TminZQL = zqlFloat[7],
                            SJ72QYZQL = zqlFloat[8],
                        });
                    }
                    else
                    {
                        float[] zqlFloat = tj.SJQSZQL(startDate, endDate);
                        sjzqlTJ1.Add(new ZQLTJ1()
                        {
                            Name = "全市",
                            SJ24TmaxZQL = zqlFloat[0],
                            SJ24TminZQL = zqlFloat[1],
                            SJ24QYZQL = zqlFloat[2],
                            SJ48TmaxZQL = zqlFloat[3],
                            SJ48TminZQL = zqlFloat[4],
                            SJ48QYZQL = zqlFloat[5],
                            SJ72TmaxZQL = zqlFloat[6],
                            SJ72TminZQL = zqlFloat[7],
                            SJ72QYZQL = zqlFloat[8],
                        });
                    }
                    
                }
                ((this.FindName("ZQLList")) as DataGrid).ItemsSource = sjzqlTJ1;


            }
            else
            {
                MessageBox.Show("请选择起止时间");
            }
        }

        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围

        }

        public class ZQLTJ1//统计信息列表
        {
            public string Name { get; set; }
            public float SJ24TmaxZQL { get; set; }
            public float SJ24TminZQL { get; set; }
            public float SJ24QYZQL { get; set; }
            public float SJ48TmaxZQL { get; set; }
            public float SJ48TminZQL { get; set; }
            public float SJ48QYZQL { get; set; }
            public float SJ72TmaxZQL { get; set; }
            public float SJ72TminZQL { get; set; }
            public float SJ72QYZQL { get; set; }
        }
    }
}
