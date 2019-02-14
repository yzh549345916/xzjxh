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
using System.Management;
using System.Collections.ObjectModel;
using System.Net;

namespace 旗县端
{
    /// <summary>
    /// 登录窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 管理员登陆 : Window
    {
        bool SQLSuc=true;
        ObservableCollection<people> peopleList = new ObservableCollection<people>();
        int intCount = 0;//记录当前旗县人员个数
        string con;//这里是保存连接数据库的字符串
        string configPath = System.Environment.CurrentDirectory + @"\config\QXList.txt";
        string DQID = "";
        string basePath = System.Environment.CurrentDirectory + @"\config\user";


        public 管理员登陆()
        {
            InitializeComponent();

            using (StreamReader sr = new StreamReader(configPath, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "当前旗县ID")
                        DQID = line.Split('=')[1];
                }
            }

            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";//改
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
            HQUserID(ref SQLSuc);

            

        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            旗县设置窗口 qxszWindow = new 旗县设置窗口();
            qxszWindow.Show();
            this.Close();
        }

        private void HQUserID(ref bool SQLSuc)
        {
            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQID + ".txt";
            string ss = "";
            SQLSuc = true;
            using (SqlConnection mycon = new SqlConnection(con))
            {
                try
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from USERID where QXID='{0}'", DQID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("Name")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=';
                        ss += sqlreader.GetString(sqlreader.GetOrdinal("admin")) + '\n';
                    }
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    using (FileStream fs = new FileStream(idPath, FileMode.Create))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                        sw.Write(ss);
                        sw.Flush();
                        sw.Close();
                    }

                }
                catch (Exception ex)
                {
                    SQLSuc = false;
                    MessageBox.Show(ex.Message + "\n如果错误为数据库连接失败将连接本地人员名单");
                }
            }
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                string line = "";
                intCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        string[] szls = line.Split('=');
                        mydic.Add(intCount++, szls[0]);
                    }
                }
                userchoose.ItemsSource = mydic;
                userchoose.SelectedValuePath = "Key";
                userchoose.DisplayMemberPath = "Value";
            }
            userchoose.SelectedValue = 0;
        }

        private void DL_Click(object sender, RoutedEventArgs e)
        {

            string idPath = System.Environment.CurrentDirectory + @"\config\user\" + DQID + ".txt";
            string[,] userSZ = new string[intCount, 3];
            using (StreamReader sr = new StreamReader(idPath, Encoding.Default))
            {
                int i = 0;
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        if (line.Contains(userchoose.Text))
                        {
                            string passStr = line.Split('=')[1];
                            string admin = line.Split('=')[2];
                            if (passStr == passWord.Password)
                            {
                                if(admin.Trim() == "1")
                                {

                                    QXXZConfig qXXZConfig = new QXXZConfig();
                                    qXXZConfig.Show();
                                    string strIPMac = passStr+ userchoose.Text+ "  "+GetIP() + "  " + GetMacAddress()+"访问";
                                    using (SqlConnection mycon = new SqlConnection(con))
                                    {
                                        try
                                        {
                                            
                                            mycon.Open();//打开
                                            string sql = string.Format(@"insert into 旗县访问记录 values('{0}','{1}','{2}')", DateTime.Now.ToString("yyyy-MM-dd"), DateTime.Now.ToLongTimeString(), strIPMac);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                            SqlCommand sqlman = new SqlCommand(sql, mycon);
                                            sqlman.ExecuteNonQuery();

                                            this.Close();
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("您没有访问权限");
                                }


                            }
                            else
                            {
                                MessageBox.Show("密码错误，请重新输入");
                            }
                        }
                    }
                }

            }


        }

        public string GetIP()
        {
            string ssfh = "";
            //获得主机名
            try
            {
                string HostName = Dns.GetHostName();
                ssfh += "主机名：" + HostName;

                ssfh += "  IP地址：" + GetIPAddress();
            }
            catch
            {

            }
            return ssfh;

        }

        public string GetIPAddress()
        {
            try
            {
                //获取IP地址 
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString(); 
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }

        public string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "物理地址：";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac += mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "物理地址：unknow";
            }
            finally
            {
            }

        }




        public class people
        {
            public string DLdate { get; set; }
            public string userName { get; set; }
        }








    }
}
