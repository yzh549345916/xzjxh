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
using System.Data.SqlClient;

namespace sjzd
{
    /// <summary>
    /// WPFAddQX.xaml 的交互逻辑
    /// </summary>
    public partial class WPFAddQX : Window
    {
        string con = "";
        public WPFAddQX()
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

            ConfigClass1 configClass1 = new ConfigClass1();
            configClass1.TBBD();
            XHText.Text= configClass1.HqzxXH(-1).ToString();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
           
            if(QXID.Text.Trim().Length > 0 && QXName.Text.Trim().Length > 0 && QXBS.Text.Trim().Length > 0 && LonText.Text.Trim().Length > 0 && LatText.Text.Trim().Length > 0 && HighText.Text.Trim().Length > 0)
            {
                Int16 countLS1 = 0;
                using (SqlConnection mycon = new SqlConnection(con))
                {
                    try
                    {

                        mycon.Open();//打开
                        string sql = string.Format(@"SELECT COUNT(*) FROM QX Where ID = '{0}'", QXID.Text.Trim());  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader reader = sqlman.ExecuteReader();
                        if (reader.Read())
                        {
                            countLS1 = Convert.ToInt16(reader[0]);
                        }

                    }
                    catch (Exception ex)
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

                            mycon.Open();//打开
                            string sql = string.Format(@"insert into QX values('{0}','{1}','{2}','{3}')", Convert.ToInt32(XHText.Text.Trim()), QXID.Text.Trim(), "-1", QXName.Text.Trim());  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                            jlCount = sqlman.ExecuteNonQuery();
                            if (jlCount <= 0)
                                MessageBox.Show("新增旗县失败");
                            else
                            {
                                区局智能网格 qjzn = new 区局智能网格();
                                qjzn.SaveStation(QXID.Text.Trim(), QXName.Text.Trim(), 13, Convert.ToDouble(LonText.Text.Trim()), Convert.ToDouble(LatText.Text.Trim()), Convert.ToDouble(HighText.Text.Trim()));
                                using (SqlConnection mycon2 = new SqlConnection(con))
                                {
                                    try
                                    {
                                        mycon2.Open();
                                        string sql2 = string.Format(@"insert into QXBS values('{0}','{1}')", QXID.Text.Trim(), QXBS.Text.Trim());
                                        SqlCommand sqlman2 = new SqlCommand(sql2, mycon2);
                                        jlCount = sqlman2.ExecuteNonQuery();
                                        if (jlCount <= 0)
                                            MessageBox.Show("旗县标识保存失败");
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.Message);
                                    }
                                }
                                ConfigClass1 configClass1 = new ConfigClass1();
                                XHText.Text = configClass1.HqzxXH(-1).ToString();
                                QXName.Text = "";
                                QXID.Text = "";
                                QXBS.Text = "";
                                LonText.Text = "";
                                LatText.Text = "";
                                HighText.Text = "";
                                if (MessageBox.Show("旗县新增成功，是否同步本地设置文件", "注意", MessageBoxButton.YesNo,
                                        MessageBoxImage.Information) == MessageBoxResult.Yes)
                                {
                                    //同步数据库旗县到本地文件
                                    configClass1.TBBD();
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("新增旗县失败");
                        }

                    }
                }
                else
                {
                    MessageBox.Show("新增旗县失败,旗县区站号已经存在");
                }
            }
            else
            {
                MessageBox.Show("请输入完整的信息");
            }

        }

        private void GetStationWZ_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (QXID.Text.Trim().Length > 0)
                {
                    区局智能网格 znwg = new 区局智能网格();
                    string error = "";
                    string data = znwg.CIMISS_ZDbyID(QXID.Text.Trim(), ref error);
                    if (data.Trim().Length > 0)
                    {
                        string[] szLS = data.Split();
                        LatText.Text = szLS[3];
                        LonText.Text = szLS[4];
                        HighText.Text = szLS[5];
                    }
                    else
                    {
                        MessageBox.Show("从CIMISS获取站点信息失败，请确认区站号是否正确，如果无误请手动填入站点的经纬度、海拔高度信息");
                    }
                }
                else
                {
                    MessageBox.Show("请先输入区站号");
                }
            }
            catch
            {

            }
        }
    }
}
