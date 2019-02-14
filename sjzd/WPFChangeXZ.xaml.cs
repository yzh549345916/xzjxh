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

namespace sjzd
{
    /// <summary>
    /// WPFChangeQX.xaml 的交互逻辑
    /// </summary>
 

    public partial class WPFChangeXZ : Window
    {
        private string qxStr = "";
        private string xzStr = "";
        public WPFChangeXZ()
        {
            InitializeComponent();


            try
            {
                Dictionary<int, string> mydic = new Dictionary<int, string>()
                {

                };
                ConfigClass1 configClass1 = new ConfigClass1();
                qxStr = configClass1.IDName(-1);
                string[] qxSZ = qxStr.Split('\n');
                for (int i = 0; i < qxSZ.Length; i++)
                {
                    string[] szLS = qxSZ[i].Split(',');
                    mydic.Add(i, szLS[0]);
                }
                QXList.ItemsSource = mydic;
                QXList.SelectedValuePath = "Key";
                QXList.DisplayMemberPath = "Value";
            }
            catch
            {

            }
        }

        private void QXList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                XZlist.SelectionChanged -= XZList_SelectionChanged;
                XZlist.ItemsSource = null;
                XZlist.SelectedIndex = -1;
                Dictionary<int, string> mydic = new Dictionary<int, string>()
                {

                };
                ConfigClass1 configClass1 = new ConfigClass1();
                xzStr = configClass1.IDName(Convert.ToInt32(QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim()));
                string[] qxSZ = xzStr.Split('\n');
                for (int i = 0; i < qxSZ.Length; i++)
                {
                    string[] szLS = qxSZ[i].Split(',');
                    mydic.Add(i, szLS[1]);
                }
                XZlist.ItemsSource = mydic;
                XZlist.SelectedValuePath = "Key";
                XZlist.DisplayMemberPath = "Value";
                stationID_Copy.Text = "";
                stationID.Text = "";
                CStationName.Text = "";
                CStationID_Copy.Text = "";
                CStationID.Text = "";
                DQLon.Text = "";
                DQLat.Text = "";
                DQHigh.Text = "";
                XGLon.Text = "";
                XGLat.Text = "";
                XGHigh.Text = "";
                XZlist.SelectionChanged += XZList_SelectionChanged;
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
                string strLS = XZlist.SelectedItem.ToString().Split(',')[1];
                string[] szLS = qxSZ[i].Split(',');
                if (szLS[1] == strLS.Substring(0, strLS.Length - 1).Trim())
                {
                    try
                    {
                        stationID.Text = szLS[0];
                        CStationName.Text = szLS[1];
                        CStationID.Text = szLS[0];
                        CStationID_Copy.Text = szLS[2];
                        stationID_Copy.Text = szLS[2];
                        区局智能网格 znwg = new 区局智能网格();
                        string[] sz1 = znwg.HQStationByID(stationID.Text).Split();
                        DQLon.Text = sz1[2];
                        DQLat.Text = sz1[3];
                        DQHigh.Text = sz1[4];
                        XGLon.Text = DQLon.Text;
                        XGLat.Text = DQLat.Text;
                        XGHigh.Text = DQHigh.Text;
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
                区局智能网格 qjzn = new 区局智能网格();
                qjzn.SaveStation(CStationID.Text.Trim(), CStationName.Text.Trim(), 14, Convert.ToDouble(XGLon.Text.Trim()), Convert.ToDouble(XGLat.Text.Trim()), Convert.ToDouble(XGHigh.Text.Trim()));
                ConfigClass1 configClass1 = new ConfigClass1();
                if (configClass1.XGQXXZ(Convert.ToInt32(CStationID_Copy.Text.Trim()), CStationID.Text.Trim(), (QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim()),
                    CStationName.Text, stationID.Text))
                {
                    try
                    {
                        XZlist.SelectionChanged -= XZList_SelectionChanged;
                        XZlist.ItemsSource = null;
                        Dictionary<int, string> mydic = new Dictionary<int, string>()
                        {

                        };
                        int ID = 1;
                        xzStr = configClass1.IDName(Convert.ToInt32(QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim())); ;
                        string[] qxSZ = xzStr.Split('\n');
                        for (int i = 0; i < qxSZ.Length; i++)
                        {
                            string[] szLS = qxSZ[i].Split(',');
                            mydic.Add(i, szLS[1]);
                        }
                        XZlist.SelectionChanged += XZList_SelectionChanged;
                        XZlist.ItemsSource = mydic;
                        XZlist.SelectedValuePath = "Key";
                        XZlist.DisplayMemberPath = "Value";
                        stationID_Copy.Text = "";
                        stationID.Text = "";
                        CStationName.Text = "";
                        CStationID_Copy.Text = "";
                        DQLat.Text = "";
                        DQLon.Text = "";
                        DQHigh.Text = "";
                        XGHigh.Text = "";
                        XGLat.Text = "";
                        XGLon.Text = "";
                        CStationID.Text = "";
                        if (MessageBox.Show("保存成功，是否同步本地文件", "注意", MessageBoxButton.YesNo,
                                MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            configClass1.TBBD();
                        }


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
                if (MessageBox.Show("确定要删除该乡镇吗", "注意", MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    ConfigClass1 configClass1 = new ConfigClass1();
                    //同步数据库旗县到本地文件

                    if (configClass1.DeleteXZ(stationID.Text.Trim()))
                    {
                        try
                        {
                            XZlist.SelectionChanged -= XZList_SelectionChanged;
                            XZlist.SelectedIndex = -1;
                            XZlist.ItemsSource = null;
                            Dictionary<int, string> mydic = new Dictionary<int, string>()
                            {

                            };
                            int ID = 1;
                            configClass1 = new ConfigClass1();
                            xzStr = configClass1.IDName(Convert.ToInt32(QXList.SelectedItem.ToString().Split(',')[1].Split(']')[0].Trim()));
                            string[] qxSZ = xzStr.Split('\n');
                            for (int i = 0; i < qxSZ.Length; i++)
                            {
                                string[] szLS = qxSZ[i].Split(',');
                                mydic.Add(i, szLS[1]);
                            }

                            XZlist.ItemsSource = mydic;
                            XZlist.SelectedValuePath = "Key";
                            XZlist.DisplayMemberPath = "Value";
                            stationID.Text = "";
                            CStationName.Text = "";
                            DQLat.Text = "";
                            DQLon.Text = "";
                            DQHigh.Text = "";
                            XGHigh.Text = "";
                            XGLat.Text = "";
                            XGLon.Text = "";
                            CStationID.Text = "";
                            stationID_Copy.Text = "";
                            CStationID_Copy.Text = "";
                            XZlist.SelectedIndex = -1;
                            XZlist.SelectionChanged += XZList_SelectionChanged;
                        }
                        catch
                        {

                        }
                        if (MessageBox.Show("乡镇删除成功，是否同步本地设置文件", "注意", MessageBoxButton.YesNo,
                                MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            //同步数据库旗县到本地文件
                            configClass1.TBBD();
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
