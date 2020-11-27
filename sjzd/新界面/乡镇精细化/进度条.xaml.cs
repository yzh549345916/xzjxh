using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{

    /// <summary>
    /// SettingsWindowContent.xaml 的交互逻辑
    /// </summary>
    public partial class 进度条 : UserControl
    {
        private 进度条ViewModel jdtView = new 进度条ViewModel();
        public List<检验结果> 结果;
        public 进度条()
        {
            InitializeComponent();

            jdtView.myValue = 0;
            DataContext = jdtView;
            LoadingLabel.Text = "数据加载中...";
            PercentageLabel.Text = 0 + " %";
            Thread thread = new Thread(检验结果查询);
            thread.Start();
        }


        private void RadProgressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = Convert.ToInt32(RadProgressBar1.Value / (RadProgressBar1.Maximum - RadProgressBar1.Minimum) * 100);

            if (RadProgressBar1.Value >= RadProgressBar1.Maximum)
            {
                try
                {
                    PercentageLabel.Text = 100 + " %";
                    LoadingLabel.Text = "数据加载完成";
                    RadWindow radWindow = Parent as RadWindow;
                    radWindow.Close();
                }
                catch (Exception)
                {
                }
            }
            else
                PercentageLabel.Text = value + " %";
        }

        public void 检验结果查询()
        {
            DateTime endDateTime = DateTime.Now;
            DateTime mysDate = endDateTime.AddDays(1 - endDateTime.Day).Date.AddMonths(-12), myeDate = endDateTime.AddDays(1 - endDateTime.Day).Date.AddMonths(0).AddSeconds(-1);
            if (DateTime.Now.Day <= 5)
            {
                mysDate = mysDate.AddMonths(-1);
                myeDate = myeDate.AddMonths(-1);
            }
            结果 = 获取检验结果(mysDate, myeDate);
            jdtView.myValue += 100;
        }

        public List<检验结果> 获取检验结果(DateTime mysDate, DateTime myeDate)
        {
            List<检验结果> dataLists = new List<检验结果>();
            try
            {
                string con = "";
                string DBconPath = Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
                try
                {

                    using (StreamReader sr = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line1;

                        // 从文件读取并显示行，直到文件的末尾 
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            if (line1.Contains("sql管理员"))
                            {
                                con = line1.Substring("sql管理员=".Length);
                                break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                try
                {
                    using (SqlConnection mycon1 = new SqlConnection(con)) //创建SQL连接对象)
                    {
                        mycon1.Open(); //打开
                        string sql = string.Format(@"select * from 检验结果_站点_月 where Date>='{0}' AND Date<='{1}'", mysDate.ToString("yyyy-MM-dd"), myeDate.ToString("yyyy-MM-dd"));

                        try
                        {
                            using (SqlCommand sqlman = new SqlCommand(sql, mycon1))
                            {
                                using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                                {
                                    while (sqlreader.Read())
                                    {
                                        try
                                        {
                                            dataLists.Add(new 检验结果
                                            {
                                                StationID = sqlreader.IsDBNull(sqlreader.GetOrdinal("StationID")) ? "" : sqlreader.GetString(sqlreader.GetOrdinal("StationID")),
                                                Date = sqlreader.GetDateTime(sqlreader.GetOrdinal("Date")),
                                                Name = sqlreader.IsDBNull(sqlreader.GetOrdinal("Name")) ? "" : sqlreader.GetString(sqlreader.GetOrdinal("Name")),
                                                QYPF = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYPF")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYPF")),
                                                GWPF = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWPF")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWPF")),
                                                DWPF = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWPF")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWPF")),
                                                ZHPF = sqlreader.IsDBNull(sqlreader.GetOrdinal("ZHPF")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("ZHPF")),
                                                QYJQ = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYJQ")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYJQ")),
                                                GWJQ = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWJQ")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWJQ")),
                                                DWJQ = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWJQ")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWJQ")),
                                                AllJQ = sqlreader.IsDBNull(sqlreader.GetOrdinal("AllJQ")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("AllJQ")),
                                                QYPF24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYPF24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYPF24")),
                                                QYPF48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYPF48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYPF48")),
                                                QYPF72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYPF72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYPF72")),
                                                GWPF24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWPF24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWPF24")),
                                                GWPF48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWPF48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWPF48")),
                                                GWPF72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWPF72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWPF72")),
                                                DWPF24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWPF24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWPF24")),
                                                DWPF48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWPF48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWPF48")),
                                                DWPF72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWPF72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWPF72")),
                                                QYJQ24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYJQ24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYJQ24")),
                                                QYJQ48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYJQ48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYJQ48")),
                                                QYJQ72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("QYJQ72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("QYJQ72")),
                                                GWJQ24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWJQ24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWJQ24")),
                                                GWJQ48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWJQ48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWJQ48")),
                                                GWJQ72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("GWJQ72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("GWJQ72")),
                                                DWJQ24 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWJQ24")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWJQ24")),
                                                DWJQ48 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWJQ48")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWJQ48")),
                                                DWJQ72 = sqlreader.IsDBNull(sqlreader.GetOrdinal("DWJQ72")) ? -999 : sqlreader.GetDouble(sqlreader.GetOrdinal("DWJQ72"))
                                            });
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                        }

                        mycon1.Close();
                    }
                }
                catch (Exception)
                {
                }

                //for (int intMonth = 0; intMonth < 12; intMonth++)
                //{
                //    try
                //    {
                //        string startDate = mysDate.AddMonths(intMonth).ToString("yyyy-MM-dd");
                //        string endDate = myeDate.AddMonths(intMonth).ToString("yyyy-MM-dd");
                //    }
                //    catch
                //    {
                //    }
                //    finally
                //    {
                //        jdtView.myValue += 8.5;
                //    }
                //}
            }
            catch
            {
            }

            return dataLists.OrderBy(y => y.Date).ThenBy(y => y.StationID).ToList();
        }

        public class 检验结果
        {
            public string Name { get; set; }
            public string StationID { get; set; }
            public DateTime Date { get; set; }
            public double QYPF { get; set; }
            public double GWPF { get; set; }
            public double DWPF { get; set; }
            public double ZHPF { get; set; }
            public double QYJQ { get; set; }
            public double GWJQ { get; set; }
            public double DWJQ { get; set; }
            public double AllJQ { get; set; }
            public double QYPF24 { get; set; }
            public double GWPF24 { get; set; }
            public double DWPF24 { get; set; }
            public double QYPF48 { get; set; }
            public double GWPF48 { get; set; }
            public double DWPF48 { get; set; }
            public double QYPF72 { get; set; }
            public double GWPF72 { get; set; }
            public double DWPF72 { get; set; }
            public double QYJQ24 { get; set; }
            public double GWJQ24 { get; set; }
            public double DWJQ24 { get; set; }
            public double QYJQ48 { get; set; }
            public double GWJQ48 { get; set; }
            public double DWJQ48 { get; set; }
            public double QYJQ72 { get; set; }
            public double GWJQ72 { get; set; }
            public double DWJQ72 { get; set; }
        }
    }
}