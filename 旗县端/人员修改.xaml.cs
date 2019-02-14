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

namespace 旗县端
{
    /// <summary>
    /// WPFChangeQX.xaml 的交互逻辑
    /// </summary>
 

    public partial class 人员修改 : Window
    {
        string DQID = "";
        private string qxStr = "";
        private string xzStr = "";
        public 人员修改()
        {
            InitializeComponent();
            CSH();
        }
        private void CSH()
        {

            try
            {
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\config\QXList.txt", Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "当前旗县ID")
                            DQID = line.Split('=')[1];
                    }
                }
                Dictionary<int, string> mydic = new Dictionary<int, string>()
                {

                };
                ConfigClass1 configClass1 = new ConfigClass1();
                xzStr = configClass1.PeopleID(Convert.ToInt32(DQID));
                string[] qxSZ = xzStr.Split('\n');
                for (int i = 0; i < qxSZ.Length; i++)
                {
                    string[] szLS = qxSZ[i].Split(',');
                    mydic.Add(i, szLS[0]);
                }
                PeopleList.ItemsSource = mydic;
                PeopleList.SelectedValuePath = "Key";
                PeopleList.DisplayMemberPath = "Value";

                stationID.Text = "";
                CStationName.Text = "";
                Admin.SelectedIndex = 0;
                CStationID.Text = "";
                PeopleList.SelectionChanged += XZList_SelectionChanged;
            }
            catch
            {
            }
        }



        


        private void XZList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string[] qxSZ = xzStr.Split('\n');
            for (int i = 0; i < qxSZ.Length; i++)
            {
                string strLS = PeopleList.SelectedItem.ToString().Split(',')[1];
                string[] szLS = qxSZ[i].Split(',');
                if (szLS[0] == strLS.Substring(0, strLS.Length - 1).Trim())
                {
                    try
                    {
                        stationID.Text = szLS[1];
                        CStationName.Text = szLS[0];
                        CStationID.Text = szLS[1];
                        if (szLS[2].Trim() == "1")
                            Admin.SelectedIndex = 1;
                        else
                            Admin.SelectedIndex = 0;
                    }
                    catch
                    {

                    }
                    break;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否保存更改", "注意", MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                ConfigClass1 configClass1 = new ConfigClass1();
                if (configClass1.XGPeople((DQID), CStationName.Text, CStationID.Text, Admin.SelectedIndex.ToString(), PeopleList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim()))
                {
                    try
                    {
                        PeopleList.SelectionChanged -= XZList_SelectionChanged;
                        PeopleList.ItemsSource = null;
                        Dictionary<int, string> mydic = new Dictionary<int, string>()
                        {

                        };
                        int ID = 1;
                        xzStr = configClass1.PeopleID(Convert.ToInt32(DQID));
                        string[] qxSZ = xzStr.Split('\n');
                        for (int i = 0; i < qxSZ.Length; i++)
                        {
                            string[] szLS = qxSZ[i].Split(',');
                            mydic.Add(i, szLS[0]);
                        }
                        PeopleList.SelectionChanged += XZList_SelectionChanged;
                        PeopleList.ItemsSource = mydic;
                        PeopleList.SelectedValuePath = "Key";
                        PeopleList.DisplayMemberPath = "Value";

                        stationID.Text = "";
                        CStationName.Text = "";
                        Admin.SelectedIndex = 0;
                        CStationID.Text = "";



                    }
                    catch(Exception ex)
                    {

                    }
                }
            }

        }

        private void DeleteBtu_Click(object sender, RoutedEventArgs e)
        {
            if (stationID.Text.Trim() != "")
            {
                if (MessageBox.Show("确定要删除该人员吗", "注意", MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    ConfigClass1 configClass1 = new ConfigClass1();

                    if (configClass1.DeletePeople(PeopleList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim()))
                    {
                        try
                        {
                            PeopleList.SelectionChanged -= XZList_SelectionChanged;
                            PeopleList.ItemsSource = null;
                            PeopleList.SelectedIndex = -1;
                            Dictionary<int, string> mydic = new Dictionary<int, string>()
                            {

                            };

                            xzStr = configClass1.PeopleID(Convert.ToInt32(DQID));
                            string[] qxSZ = xzStr.Split('\n');
                            for (int i = 0; i < qxSZ.Length; i++)
                            {
                                string[] szLS = qxSZ[i].Split(',');
                                mydic.Add(i, szLS[0]);
                            }
                            PeopleList.ItemsSource = mydic;
                            PeopleList.SelectedValuePath = "Key";
                            PeopleList.DisplayMemberPath = "Value";

                            stationID.Text = "";
                            CStationName.Text = "";
                            Admin.SelectedIndex = 0;
                            CStationID.Text = "";
                            PeopleList.SelectionChanged += XZList_SelectionChanged;
                        }
                        catch
                        {

                        }

                    }
                }
            }
        }

        private void QuitBtu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


    }
}
