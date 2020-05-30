using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using static 旗县端.区局智能网格;

namespace 旗县端
{
    /// <summary>
    /// 逐日评分详情.xaml 的交互逻辑
    /// </summary>
    public partial class 省级智能网格高低温查询 : Page
    {
        string QXIDName = "";
        ObservableCollection<QJZN> qJZNs = new ObservableCollection<QJZN>();
        public 省级智能网格高低温查询()
        {
            string DQID = "53466";
            ConfigClass1 configClass1 = new ConfigClass1();
            InitializeComponent();
            BTLabel.Content = " 省级智能网格预报查询";
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            sDate.SelectedDate = DateTime.Now;
            try
            {
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\QXList.txt", Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "当前旗县ID")
                        {
                            DQID = line.Split('=')[1];
                            break;
                        }
                    }
                }
                QXIDName = configClass1.IDName(-1);
                string[] szls = QXIDName.Split('\n');
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                int count = 0;
                mydic.Add(0, "全市");
                for (int i = 0; i < szls.Length; i++)
                {
                    mydic.Add(i + 1, szls[i].Split(',')[1]);
                    if (szls[i].Split(',')[0] == DQID)
                        count = i + 1;
                }
                QXSelect.ItemsSource = mydic;
                QXSelect.SelectedValuePath = "Key";
                QXSelect.DisplayMemberPath = "Value";
                QXSelect.SelectedValue = count;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sDate.SelectedDate.ToString().Length == 0))
            {
                string QXName = QXSelect.Text;
                qJZNs.Clear();
                ConfigClass1 configClass1 = new ConfigClass1();
                string QXID = "";
                string[] szls = QXIDName.Split('\n');
                string IDName = "";
                for (int i = 0; i < szls.Length; i++)
                {
                    if (szls[i].Split(',')[1] == QXName)
                    {
                        QXID = szls[i].Split(',')[0];
                        break;
                    }
                }

                try
                {
                    if (QXID.Trim().Length == 0)
                    {
                        IDName = configClass1.IDName();
                    }
                    else
                    {
                        IDName = QXID + ',' + QXName + '\n' + configClass1.IDName(Convert.ToInt32(QXID));
                    }


                    string strID = "";
                    foreach (string ss in IDName.Split('\n'))
                    {
                        strID += '\'' + ss.Split(',')[0] + "\',";
                    }
                    strID = strID.Substring(0, strID.Length - 1);
                    string strTime = Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd");
                    Int16 sc = Convert.ToInt16(SCSelect.Text);
                    区局智能网格 znwg = new 区局智能网格();
                    List<YSList> dataList = znwg.HQYS(strTime, sc, strID);
                    int intCount = 1;
                    foreach (string ss in IDName.Split('\n'))
                    {
                        YSList q24 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 24));
                        YSList q48 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 48));
                        YSList q72 = dataList.Find((YSList y) => (y.ID == ss.Split(',')[0] && y.sx == 72));
                        qJZNs.Add(new QJZN()
                        {
                            XH = intCount++,
                            StationID = ss.Split(',')[0],
                            Name = ss.Split(',')[1],
                            GW24 = q24.TMAX,
                            DW24 = q24.TMIN,
                            GW48 = q48.TMAX,
                            DW48 = q48.TMIN,
                            GW72 = q24.TMAX,
                            DW72 = q72.TMIN,
                        });
                    }

                ((this.FindName("GRPFList")) as System.Windows.Controls.DataGrid).ItemsSource = qJZNs;


                }
                catch
                {

                }

                BTLabel.Content = Convert.ToDateTime((sDate.SelectedDate)).ToString("yyyy年MM月dd日") + SCSelect.Text + "时省级智能网格";


            }
            else
            {
                System.Windows.MessageBox.Show("请选择查询时间");
            }

        }

        public class QJZN//统计信息列表
        {
            public int XH { get; set; }
            public string Name { get; set; }
            public string StationID { get; set; }
            public double GW24 { get; set; }
            public double DW24 { get; set; }
            public double GW48 { get; set; }
            public double DW48 { get; set; }
            public double GW72 { get; set; }
            public double DW72 { get; set; }
        }




    }
}
