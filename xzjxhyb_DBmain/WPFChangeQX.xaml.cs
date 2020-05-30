using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace xzjxhyb_DBmain
{
    /// <summary>
    /// WPFChangeQX.xaml 的交互逻辑
    /// </summary>


    public partial class WPFChangeQX : Window
    {
        private string qxStr = "";
        private string bsStr = "";
        public WPFChangeQX()
        {
            InitializeComponent();


            try
            {
                Dictionary<int, string> mydic = new Dictionary<int, string>()
                {

                };
                ConfigClass1 configClass1 = new ConfigClass1();
                qxStr = configClass1.IDName(-1);
                bsStr = configClass1.HQBS();
                string[] qxSZ = qxStr.Split('\n');
                for (int i = 0; i < qxSZ.Length; i++)
                {
                    string[] szLS = qxSZ[i].Split(',');
                    mydic.Add(i, szLS[1]);
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
            string[] qxSZ = qxStr.Split('\n');
            string strLS = QXList.SelectedItem.ToString().Split(',')[1];
            strLS = strLS.Substring(0, strLS.Length - 1).Trim();
            for (int i = 0; i < qxSZ.Length; i++)
            {
                string[] szLS = qxSZ[i].Split(',');
                if (szLS[1] == strLS)
                {
                    try
                    {
                        stationID.Text = szLS[0];
                        CStationName.Text = szLS[1];
                        CStationID.Text = szLS[0];
                        CStationID_Copy.Text = szLS[2];
                        stationID_Copy.Text = szLS[2];
                    }
                    catch
                    {

                    }
                    break;
                }
            }

            qxSZ = bsStr.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string ss in qxSZ)
            {
                if (stationID.Text.Trim() == ss.Split('=')[0])
                {
                    DQBS.Text = ss.Split('=')[1];
                    XGBS.Text = ss.Split('=')[1];
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("是否保存更改", "注意", MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                ConfigClass1 configClass1 = new ConfigClass1();
                if (configClass1.XGQXXZ(Convert.ToInt32(CStationID_Copy.Text.Trim()), CStationID.Text.Trim(), "-1",
                    CStationName.Text, stationID.Text, XGBS.Text.Trim()))
                {
                    try
                    {
                        QXList.SelectionChanged -= QXList_SelectionChanged;
                        QXList.ItemsSource = null;
                        Dictionary<int, string> mydic = new Dictionary<int, string>()
                        {

                        };
                        qxStr = configClass1.IDName(-1);
                        string[] qxSZ = qxStr.Split('\n');
                        for (int i = 0; i < qxSZ.Length; i++)
                        {
                            string[] szLS = qxSZ[i].Split(',');
                            mydic.Add(i, szLS[1]);
                        }
                        QXList.SelectionChanged += QXList_SelectionChanged;
                        QXList.ItemsSource = mydic;
                        QXList.SelectedValuePath = "Key";
                        QXList.DisplayMemberPath = "Value";
                        stationID_Copy.Text = "";
                        bsStr = configClass1.HQBS();
                        stationID.Text = "";
                        XGBS.Text = "";
                        DQBS.Text = "";
                        CStationName.Text = "";
                        CStationID_Copy.Text = "";
                        CStationID.Text = "";
                        if (MessageBox.Show("保存成功，是否同步本地文件", "注意", MessageBoxButton.YesNo,
                                MessageBoxImage.Information) == MessageBoxResult.Yes)
                        {
                            configClass1.TBBD();
                        }


                    }
                    catch (Exception)
                    {

                    }
                }
            }

        }

        private void DeleteBtu_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("谨慎删除，如果删除该旗县，则将一并删除该旗县下的所有乡镇", "注意", MessageBoxButton.YesNo,
                    MessageBoxImage.Information) == MessageBoxResult.Yes)
            {
                ConfigClass1 configClass1 = new ConfigClass1();
                //同步数据库旗县到本地文件

                if (configClass1.DeleteQX(stationID.Text.Trim()))
                {
                    try
                    {
                        QXList.SelectionChanged -= QXList_SelectionChanged;
                        QXList.SelectedIndex = -1;
                        QXList.ItemsSource = null;
                        Dictionary<int, string> mydic = new Dictionary<int, string>()
                        {

                        };
                        configClass1 = new ConfigClass1();
                        qxStr = configClass1.IDName(-1);
                        string[] qxSZ = qxStr.Split('\n');
                        for (int i = 0; i < qxSZ.Length; i++)
                        {
                            string[] szLS = qxSZ[i].Split(',');
                            mydic.Add(i, szLS[1]);
                        }
                        bsStr = configClass1.HQBS();
                        QXList.ItemsSource = mydic;
                        QXList.SelectedValuePath = "Key";
                        QXList.DisplayMemberPath = "Value";
                        stationID.Text = "";
                        CStationName.Text = "";
                        CStationID.Text = "";
                        DQBS.Text = "";
                        XGBS.Text = "";
                        QXList.SelectedIndex = -1;
                        QXList.SelectionChanged += QXList_SelectionChanged;
                    }
                    catch
                    {

                    }
                    if (MessageBox.Show("旗县删除成功，是否同步本地设置文件", "注意", MessageBoxButton.YesNo,
                            MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        //同步数据库旗县到本地文件
                        configClass1.TBBD();
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
