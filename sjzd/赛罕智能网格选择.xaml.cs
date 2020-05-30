using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using sjzd.类;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 赛罕智能网格选择 : RadWindow
    {
        public 赛罕智能网格选择()
        {
            InitializeComponent();
            try
            {
                市台检验数据库 stjysjk = new 市台检验数据库();
                List<市台检验数据库.StationList> qfLists = stjysjk.获取赛罕智能网格站点列表(14);
                foreach (var item in qfLists)
                {
                    RadComboBoxItem comboBoxItem = new RadComboBoxItem
                    {
                        Content = item.Name,
                        
                        
                    };
                    ZBCom.Items.Add(comboBoxItem);
                }
              

                try
                {
                    if(DateTime.Now.Hour<=18&& DateTime.Now.Hour >= 7)
                        LXCom.SelectedIndex = 0;
                    else
                        LXCom.SelectedIndex = 0;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch(Exception e)
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
            LXCom.Text = "";
            this.DialogResult = false;
            this.Close();
        }

        private void LXCom_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var radComboBox = (RadComboBox)sender;
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            util.Write(radComboBox.Text,"CPcfig", "FLYB", "LX");
        }
    }
}
