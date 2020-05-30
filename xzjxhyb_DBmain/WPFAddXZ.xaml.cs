using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// WPFAddQX.xaml 的交互逻辑
    /// </summary>
    public partial class WPFAddXZ : Window
    {
        string con = "";

        public WPFAddXZ()
        {
            InitializeComponent();

            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                    }
                }
            }

            try
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    mycon.Open(); //打开
                    string sql =
                        string.Format(
                            @"select * from QX where FatherID = '-1' order by XH"); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    Dictionary<int, string> mydic = new Dictionary<int, string>();
                    int intCount = 0;
                    while (sqlreader.Read())
                    {
                        mydic.Add(intCount++, sqlreader.GetString(sqlreader.GetOrdinal("ID")));

                    }

                    QXList.ItemsSource = mydic;
                    QXList.SelectedValuePath = "Key";
                    QXList.DisplayMemberPath = "Value";
                    QXList.SelectedValue = 0;
                }

            }
            catch
            {
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            Int16 countLS1 = 0;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {

                    mycon.Open(); //打开
                    string sql = string.Format(@"SELECT COUNT(*) FROM QX Where ID = '{0}'",
                        QXID.Text.Trim()); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader reader = sqlman.ExecuteReader();
                    if (reader.Read())
                    {
                        countLS1 = Convert.ToInt16(reader[0]);
                    }

                }
                catch (Exception)
                {

                }

            }

            if (countLS1 == 0)
            {
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    int jlCount = 0;
                    try
                    {

                        mycon.Open(); //打开
                        string sql = string.Format(@"insert into QX values('{0}','{1}','{2}','{3}')",
                            Convert.ToInt32(XHText.Text.Trim()), QXID.Text.Trim(), QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim(),
                            QXName.Text.Trim()); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        jlCount = sqlman.ExecuteNonQuery();
                        if (jlCount <= 0)
                            MessageBox.Show("新增乡镇失败");
                        else
                        {
                            try
                            {
                                QXName.Text = "";
                                QXID.Text = "";
                                ConfigClass1 configClass1 = new ConfigClass1();
                                XHText.Text = configClass1
                                    .HqzxXH(Convert.ToInt32(QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim())).ToString();
                            }
                            catch
                            {

                            }
                            if (MessageBox.Show("乡镇新增成功，是否同步本地设置文件", "注意", MessageBoxButton.YesNo,
                                    MessageBoxImage.Information) == MessageBoxResult.Yes)
                            {
                                ConfigClass1 configClass1 = new ConfigClass1();
                                //同步数据库旗县到本地文件
                                configClass1.TBBD();
                            }

                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("新增乡镇失败");
                    }

                }
            }
            else
            {
                MessageBox.Show("新增乡镇失败,乡镇区站号已经存在");
            }

        }

        private void QXList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ConfigClass1 configClass1 = new ConfigClass1();
                XHText.Text = configClass1
                    .HqzxXH(Convert.ToInt32(QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim())).ToString();
            }
            catch
            {

            }
        }
    }
}
