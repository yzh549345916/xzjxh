using System;
using System.IO;
using System.Text;
using System.Windows;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// 数据库设置窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 数据库设置窗口 : Window
    {
        Int16 SFJG = 0;
        string con;//= "Server=172.18.142.151;Database=xzjxhyb_DB;user id=sa;password=134679;"; //这里是保存连接数据库的字符串172.18.142.151 id=sa;password=134679;
        Int16 RKMM = 0, RKH = 0, QXRKH = 0, QXRKM = 0, SJRKH = 0, SJRKM = 0;//实况入库的分钟和小时,窗口初始化程序中会重新给该值从配置文件中赋值
        string RKTime = "20";//实况入库的时次,窗口初始化程序中会重新给该值从配置文件中赋值
        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string DBConfigTxt = "";
                using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "区域站雨量筒是否加盖")
                        {
                            string SFJGStr = "0";
                            if (SFJGcheck.IsChecked == true)
                            {
                                SFJGStr = "1";
                            }
                            else
                            {
                                SFJGStr = "0";
                            }
                            DBConfigTxt += "区域站雨量筒是否加盖=" + SFJGStr + "\r\n";


                        }
                        else
                        {
                            DBConfigTxt += line + "\r\n";
                        }
                    }
                    DBConfigTxt = DBConfigTxt.Substring(0, DBConfigTxt.Length - 2);

                }
                FileStream myFs = new FileStream(DBconPath, FileMode.Create);
                StreamWriter mySw = new StreamWriter(myFs, Encoding.Default);
                mySw.Write(DBConfigTxt);
                mySw.Close();
                myFs.Close();
                MessageBox.Show("设置保存成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        public 数据库设置窗口()
        {
            InitializeComponent();
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        con = line.Substring("sql管理员=".Length);
                    }
                    else if (line.Contains("实况入库小时"))
                    {
                        RKH = Convert.ToInt16(line.Substring("实况入库小时=".Length));
                    }
                    else if (line.Contains("实况入库分钟"))
                    {
                        RKMM = Convert.ToInt16(line.Substring("实况入库分钟=".Length));
                    }
                    else if (line.Split('=')[0] == "实况时次")
                    {
                        RKTime = line.Split('=')[1];
                    }
                    else if (line.Contains("旗县预报入库小时"))
                    {
                        QXRKH = Convert.ToInt16(line.Substring("旗县预报入库小时=".Length));
                    }
                    else if (line.Contains("旗县预报入库分钟"))
                    {
                        QXRKM = Convert.ToInt16(line.Substring("旗县预报入库分钟=".Length));
                    }
                    else if (line.Split('=')[0] == "市局预报入库小时")
                    {
                        SJRKH = Convert.ToInt16(line.Split('=')[1]);
                    }
                    else if (line.Split('=')[0] == "市局预报入库分钟")
                    {
                        SJRKM = Convert.ToInt16(line.Split('=')[1]);
                    }

                    else if (line.Split('=')[0] == "区域站雨量筒是否加盖")
                    {
                        try
                        {
                            SFJG = Convert.ToInt16(line.Split('=')[1]);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }

                    }
                }
            }
            if (SFJG != 0)
            {
                SFJGcheck.IsChecked = true;
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
