using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// WPFAddQX.xaml 的交互逻辑
    /// </summary>
    public partial class 人员管理页 : RadWindow
    {
        string con = "";

        public 人员管理页()
        {
            InitializeComponent();

            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
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
                    string sql = string.Format(@"SELECT COUNT(*) FROM USERID Where ID = '{0}'",
                        ID.Text.Trim()); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
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
                string SFAdmin = "0";
                if (Admin.SelectedIndex != 0)
                    SFAdmin = "1";
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    int jlCount = 0;
                    try
                    {

                        mycon.Open(); //打开
                        string sql = string.Format(@"insert into USERID values('{0}','{1}','{2}','{3}')",
                            QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim(),
                            ID.Text.Trim(), Name.Text.Trim(), SFAdmin); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        jlCount = sqlman.ExecuteNonQuery();
                        if (jlCount <= 0)
                            MessageBox.Show("新增人员失败");
                        else
                        {
                            try
                            {
                                ID.Text = "";
                                Name.Text = "";
                                Admin.SelectedIndex = 0;
                            }
                            catch
                            {

                            }


                        }

                    }
                    catch (Exception)
                    {
                        MessageBox.Show("新增人员失败");
                    }

                }
            }
            else
            {
                MessageBox.Show("新增人员失败,人员ID已经存在");
            }

        }


    }
}
