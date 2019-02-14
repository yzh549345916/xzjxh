using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 预报人员选择2 : Window
    {
        public 预报人员选择2()
        {
            InitializeComponent();
            Dictionary<int, string> mydic = new Dictionary<int, string>();
            string PeopleConfig = System.Environment.CurrentDirectory + @"\设置文件\市四区\值班人员.txt";
            int intCount = 0;
            try
            {
                using (StreamReader sr = new StreamReader(PeopleConfig, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            string[] szls = line.Split('=');
                            mydic.Add(intCount++, szls[0]);
                        }
                    }
                }
                ZBCom.ItemsSource = mydic;
                ZBCom.SelectedValuePath = "Key";
                ZBCom.DisplayMemberPath = "Value";
                FBCom.ItemsSource = mydic;
                FBCom.SelectedValuePath = "Key";
                FBCom.DisplayMemberPath = "Value";
                QFCom.ItemsSource = mydic;
                QFCom.SelectedValuePath = "Key";
                QFCom.DisplayMemberPath = "Value";
                QFCom.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Z308Btu_Copy_Click(object sender, RoutedEventArgs e)
        {
            ZBCom.Text = "";
            FBCom.Text = "";
            this.Close();
        }
    }
}
