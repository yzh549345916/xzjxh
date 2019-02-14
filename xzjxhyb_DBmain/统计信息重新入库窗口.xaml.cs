using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// 统计信息重新入库窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 统计信息重新入库窗口 : Window
    {
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        CalendarDateRange dr1 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue), dr2 = new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue);
        string con;//= "Server=172.18.142.151;Database=xzjxhyb_DB;user id=sa;password=134679;"; //这里是保存连接数据库的字符串172.18.142.151 id=sa;password=134679;
        public 统计信息重新入库窗口()
        {
            InitializeComponent();
            sDate.BlackoutDates.Add(new CalendarDateRange((DateTime.Now.Date).AddDays(+1), DateTime.MaxValue));//开始时间不可选的范围，当前日期以后
            eDate.BlackoutDates.Add(dr2);//结束时间不可选的范围
            progressBar1.Maximum = 100;//进度条最大值为100
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取数据库配置信息 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                    }


                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if ((!(sDate.SelectedDate.ToString().Length == 0)) && (!(eDate.SelectedDate.ToString().Length == 0)))
            {
                string strError = "";
                string ss = "";
                double douLS;//赋值保存进度条的进度数
                UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
                UpdateProgressBarDelegate updateText = new UpdateProgressBarDelegate(txtboxJD.SetValue);

                DateTime dateStartDate = Convert.ToDateTime(sDate.SelectedDate), dateEndDate = Convert.ToDateTime(eDate.SelectedDate);//获取选择的起止时间
                DateTime dateLS = dateStartDate, dateLS2 = dateLS;
                int intLS = 0;
                for (int i = 0; DateTime.Compare(dateLS2, dateEndDate) <= 0; i++)//判断总共需要循环的次数，决定进度条的进度
                {
                    intLS++;
                    dateLS2 = dateLS2.AddDays(1);
                }
                for (int i = 0; DateTime.Compare(dateLS, dateEndDate) <= 0; i++)//临时日期初始值为开始日期，每个循环加1天，一直到大于截止日期
                {
                    douLS = (i + 1) * 100 / intLS;
                    string strDate = dateLS.ToString("yyyyMMdd");
                    Dispatcher.Invoke(updatePbDelegate, System.Windows.Threading.DispatcherPriority.Background, new object[] { ProgressBar.ValueProperty, Math.Ceiling(douLS) });//委托更新显示进度条
                    Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, strDate });//委托更新显示文本
                    try
                    {
                        TJAddDB tjAddDB = new TJAddDB();
                        

                        ss += strDate + "统计信息入库" + '\n';
                        ss += tjAddDB.FirstTJ( dateLS);
                    }
                    catch (Exception ex)
                    {
                        ss += ex.Message + '\n';
                    }

                    dateLS = dateLS.AddDays(1);

                }
                Dispatcher.Invoke(updateText, System.Windows.Threading.DispatcherPriority.Background, new object[] { TextBox.TextProperty, "完成" });
                tHistory.Text = ss;
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
    }
}
