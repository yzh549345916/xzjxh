using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 生态选择1 : RadWindow
    {
        public 生态选择1()
        {
            InitializeComponent();
            try
            {
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\生态与农业气象信息设置.xml");
                string peopleStr = util.Read("People");
                int intCount = 0;
                peopleStr = peopleStr.Replace("，", ",");
                string[] strsz = peopleStr.Split(',');
                foreach (string ss in strsz)
                {
                    if (ss.Trim().Length > 0)
                    {
                        mydic.Add(intCount++, ss);
                    }
                }

                ZBCom.ItemsSource = mydic;
                ZBCom.SelectedValuePath = "Key";
                ZBCom.DisplayMemberPath = "Value";
                ZBCom.SelectedIndex = 0;
                QSALLT.Text = util.Read("Other", "ZQS").Trim();
                QST.Text = util.Read("Other", "QS").Trim();
                sDate.SelectedDate = DateTime.Now.AddDays(-3);
                if (DateTime.Now.Day < 10)
                {
                    sDate.SelectedDate = DateTime.Now.AddMonths(-1);
                    xunSel.SelectedIndex = 2;
                }
                else if (DateTime.Now.Day >= 10 && DateTime.Now.Day < 20)
                {
                    xunSel.SelectedIndex = 0;
                }
                else
                {
                    xunSel.SelectedIndex = 1;
                }
            }
            catch
            {
            }
        }



        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\生态与农业气象信息设置.xml");
            try
            {
                util.Write((Convert.ToInt32(QSALLT.Text.Trim()) + 1).ToString(), "Other", "ZQS");
            }
            catch
            {
                util.Write(QSALLT.Text.Trim(), "Other", "ZQS");
            }
            try
            {
                util.Write((Convert.ToInt32(QST.Text.Trim()) + 1).ToString(), "Other", "QS");
            }
            catch
            {
                util.Write(QST.Text.Trim(), "Other", "QS");
            }

            生态与农业气象信息 st = new 生态与农业气象信息();
            DateTime dt1 = Convert.ToDateTime(sDate.SelectedDate);
            int intqs = 0, intzqs = 0;
            try
            {
                intqs = Convert.ToInt32(QST.Text.Trim());
            }
            catch
            {
            }
            try
            {
                intzqs = Convert.ToInt32(QSALLT.Text.Trim());
            }
            catch
            {
            }
            if (xunSel.SelectedIndex < 2)
            {
                st.DCWord(intqs, intzqs, dt1.Year, dt1.Month, Convert.ToInt16(xunSel.SelectedIndex + 1), ZBCom.Text);
            }
            else
            {
                st.DCWordMonth(intqs, intzqs, dt1.Year, dt1.Month, ZBCom.Text);
            }

            this.Close();
        }

        private void Z308Btu_Copy_Click(object sender, RoutedEventArgs e)
        {
            ZBCom.Text = "";
            this.Close();
        }
    }
}
