using sjzd.类;
using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 预报人员选择3 : RadWindow
    {
        public 预报人员选择3()
        {
            InitializeComponent();
            try
            {
                市台检验数据库 stjysjk = new 市台检验数据库();
                List<市台检验数据库.PeopelList> qfLists = stjysjk.根据岗位获取人员列表("签发");
                foreach (var item in qfLists)
                {
                    RadComboBoxItem comboBoxItem = new RadComboBoxItem
                    {
                        Content = item.Name
                    };
                    QFCom.Items.Add(comboBoxItem);
                }
                List<市台检验数据库.PeopelList> zbLists = stjysjk.根据岗位获取人员列表("主班");
                foreach (var item in zbLists)
                {
                    RadComboBoxItem comboBoxItem = new RadComboBoxItem
                    {
                        Content = item.Name
                    };
                    ZBCom.Items.Add(comboBoxItem);
                }
                List<市台检验数据库.PeopelList> shLists = stjysjk.根据岗位获取人员列表("审核");
                foreach (var item in shLists)
                {
                    RadComboBoxItem comboBoxItem = new RadComboBoxItem
                    {
                        Content = item.Name
                    };
                    SHCom.Items.Add(comboBoxItem);
                }

                try
                {
                    ZBCom.SelectedIndex = 0;
                    SHCom.SelectedIndex = 0;
                    QFCom.SelectedIndex = 0;
                    LXCom.SelectedIndex = 0;
                    XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                    string lxStr = util.Read("CPcfig", "FLYB", "LX");
                    foreach (RadComboBoxItem item in LXCom.Items)
                    {
                        if (item.Content.ToString() == lxStr)
                            LXCom.SelectedItem = item;
                    }
                    LXCom.SelectionChanged += LXCom_SelectionChanged;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception)
            {
            }
        }


        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Z308Btu_Copy_Click(object sender, RoutedEventArgs e)
        {
            ZBCom.Text = "";
            SHCom.Text = "";
            LXCom.Text = "";
            QFCom.Text = "";
            this.DialogResult = false;
            this.Close();
        }

        private void LXCom_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var radComboBox = (RadComboBox)sender;
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            util.Write(radComboBox.Text, "CPcfig", "FLYB", "LX");
        }
    }
}
