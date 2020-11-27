using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 空气质量监测.xaml 的交互逻辑
    /// </summary>
    public partial class 空气质量监测 : Window
    {
        ObservableCollection<XSList> xslist = new ObservableCollection<XSList>();
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        private string configPath = Environment.CurrentDirectory + @"\设置文件\空气质量监测.txt";
        private string dbPath = "";
        public 空气质量监测()
        {
            InitializeComponent();
            CSH();
        }

        private void CXBtu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DateTime sdt = Convert.ToDateTime(Convert.ToDateTime(sDate.SelectedDate).ToString("yyyy-MM-dd ") + staSCchoose.Text + ":00:00");
                DateTime edt = Convert.ToDateTime(Convert.ToDateTime(eDate.SelectedDate).ToString("yyyy-MM-dd ") + endSCchoose.Text + ":00:01");
                CXbyTime(sdt, edt, MSSelect.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void CSH()
        {
            string cityStr = "";
            try
            {
                using (StreamReader sr = new StreamReader(configPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "数据库路径")
                        {
                            dbPath = line.Split('=')[1];
                        }
                        else if (line.Split('=')[0] == "城市列表")
                        {
                            cityStr = line.Split('=')[1];
                        }
                    }
                }

                DateTime dtls = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                sDate.SelectedDate = dtls.AddDays(-1);
                eDate.SelectedDate = dtls;
                staSCchoose.Text = DateTime.Now.ToString("HH");
                endSCchoose.Text = DateTime.Now.ToString("HH");
                if (cityStr.Length > 0)
                {
                    string[] citySZ = cityStr.Split(',');
                    Dictionary<int, string> mydic = new Dictionary<int, string>();
                    mydic.Add(0, "所有");
                    for (int i = 0; i < citySZ.Length; i++)
                    {
                        mydic.Add(i + 1, citySZ[i]);
                    }
                    MSSelect.ItemsSource = mydic;
                    MSSelect.SelectedValuePath = "Key";
                    MSSelect.DisplayMemberPath = "Value";
                    MSSelect.SelectedValue = 0;
                }
                else
                {
                    MessageBox.Show("城市列表获取失败，请检查设置文件");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("初始化失败" + ex.Message);
            }
        }

        private void sDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            eDate.BlackoutDates.Remove(dr1);//现将原来禁止的时间范围删除，否则会报错
            dr1 = new CalendarDateRange(new DateTime(), Convert.ToDateTime(sDate.Text).AddDays(-1));
            eDate.SelectedDate = null;//将已经选取的结束时间清空
            eDate.BlackoutDates.Add(dr1);//结束时间随着开始时间的改变增加新的范围
        }

        void CXbyTime(DateTime sdt, DateTime edt, string cityName)
        {
            xslist.Clear();
            string conStr = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + dbPath + ";Persist Security Info=False";
            using (OleDbConnection tempconn = new OleDbConnection(conStr))
            {
                OleDbDataAdapter oleDapAdapter; //检索与填充数据，一般填充SQL语句    
                DataSet ds = new DataSet();     //填充ds，保存数据 

                string sql = "";
                if (cityName == "所有")
                {
                    sql = string.Format(@"select * from data_pp where real_time >= {0} and real_time <={1} order by sta_id asc, real_time asc", "\"" + sdt.ToString("yyyy-MM-dd HH:mm:ss") + "\"", "\"" + edt.ToString("yyyy-MM-dd HH:mm:ss") + "\"");
                }
                else
                {
                    sql = string.Format(@"select * from data_pp where real_time >= {0} and real_time <={1} and city={2} order by sta_id asc, real_time asc", "\"" + sdt.ToString("yyyy-MM-dd HH:mm:ss") + "\"", "\"" + edt.ToString("yyyy-MM-dd HH:mm:ss") + "\"", "\"" + cityName + "\"");
                }


                oleDapAdapter = new OleDbDataAdapter(sql, tempconn);
                oleDapAdapter.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        xslist.Add(new XSList()
                        {

                            cityName = ds.Tables[0].Rows[i]["city"].ToString(),
                            StationID = ds.Tables[0].Rows[i]["sta_id"].ToString(),
                            rTime = Convert.ToDateTime(ds.Tables[0].Rows[i]["real_time"]),
                            so2 = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["so2"].ToString())),
                            no2 = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["no2"].ToString())),
                            o3 = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["o3"].ToString())),
                            co = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["co"].ToString())),
                            pm10 = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["pm10"].ToString())),
                            pm25 = Convert.ToSingle(my_password.DecryptString("Wa@2ct0k", ds.Tables[0].Rows[i]["pm25"].ToString())),
                            aqi = Convert.ToSingle(ds.Tables[0].Rows[i]["aqi"]),
                            quality = ds.Tables[0].Rows[i]["quality"].ToString(),

                        });
                    }
                    ((this.FindName("dataGrid")) as System.Windows.Controls.DataGrid).ItemsSource = xslist;
                }



            }
        }

        public class XSList//
        {
            public string cityName { get; set; }
            public string StationID { get; set; }
            public DateTime rTime { get; set; }
            public float so2 { get; set; }
            public float no2 { get; set; }
            public float o3 { get; set; }
            public float co { get; set; }
            public float pm10 { get; set; }
            public float pm25 { get; set; }
            public float aqi { get; set; }
            public string quality { get; set; }
        }
    }


}
