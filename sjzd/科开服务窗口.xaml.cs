﻿using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 科开服务窗口 : RadWindow
    {
        private string StationID = "";
        string bwPath = "";
        public 科开服务窗口()
        {
            InitializeComponent();
            sDate.SelectedDate = DateTime.Now;
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\科开设置.xml");
            StationID = util.Read("StationConfig").Trim();
            if (StationID.Length == 0)
            {
                StationID = "53463";
            }

            string[] szls = StationID.Split(',');
            foreach (string ss in szls)
            {
                switch (ss)
                {
                    case "53463":
                        S53463.IsChecked = true;
                        break;
                    case "53466":
                        S53466.IsChecked = true;
                        break;
                    case "53464":
                        S53464.IsChecked = true;
                        break;
                    case "53467":
                        S53467.IsChecked = true;
                        break;
                    case "53469":
                        S53469.IsChecked = true;
                        break;
                    case "53368":
                        S53368.IsChecked = true;
                        break;
                    case "53562":
                        S53562.IsChecked = true;
                        break;
                    default:
                        break;
                }

            }
        }


        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\科开设置.xml");
                util.Write(StationID.Trim(), "StationConfig");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            科开服务 kk = new 科开服务();
            if (selectWord.SelectedIndex == 0)
            {
                if (bwPath.Trim().Length == 0)
                {
                    kk.DCWord(Convert.ToInt16(SCCom.Text), Convert.ToDateTime(sDate.SelectedDate), StationID, Convert.ToInt16(DayCom.Text));
                }
                else
                {
                    kk.DCWord(Convert.ToInt16(SCCom.Text), Convert.ToDateTime(sDate.SelectedDate), StationID, Convert.ToInt16(DayCom.Text), bwPath);
                }

            }
            else if (selectWord.SelectedIndex == 1)
            {
                if (bwPath.Trim().Length == 0)
                {
                    kk.DCWordbyALLDate(Convert.ToInt16(SCCom.Text), Convert.ToDateTime(sDate.SelectedDate), StationID, Convert.ToInt16(DayCom.Text));
                }
                else

                {
                    kk.DCWordbyALLDate(Convert.ToInt16(SCCom.Text), Convert.ToDateTime(sDate.SelectedDate), StationID, Convert.ToInt16(DayCom.Text), bwPath);
                }

            }

        }

        private void Z308Btu_Copy_Click(object sender, RoutedEventArgs e)
        {

            this.Close();
        }

        private void S53463_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox vv = sender as CheckBox;
            string ID = vv.Name.Replace("S", "");
            string[] szls = StationID.Split(',');
            bool bs = false;
            foreach (string ss in szls)
            {
                if (ss == ID)
                {
                    bs = true;
                }
            }

            if (!bs)
            {
                if (StationID.Length > 0)
                    StationID = StationID + ',' + ID;
                else
                    StationID = ID;

            }
        }

        private void S53463_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox vv = sender as CheckBox;
            string ID = vv.Name.Replace("S", "");
            string[] szls = StationID.Split(',');
            StationID = "";
            foreach (string ss in szls)
            {
                if (ss != ID)
                {
                    StationID += ss + ',';
                }
            }

            if (StationID.Length > 0)
            {
                StationID = StationID.Substring(0, StationID.Length - 1);
            }


        }



        private void ExcelBtu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\科开设置.xml");
                util.Write(StationID.Trim(), "StationConfig");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            科开服务 kk = new 科开服务();
            kk.DCZXWord(Convert.ToInt16(SCCom.Text), Convert.ToDateTime(sDate.SelectedDate), StationID, Convert.ToInt16(DayCom.Text));
        }

        private void DQBD_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog pOpenShpFile = new OpenFileDialog();
            pOpenShpFile.Multiselect = false;

            if (pOpenShpFile.ShowDialog() == true)
            {
                if (pOpenShpFile.FileName.Length > 0)
                {
                    bwPath = pOpenShpFile.FileName;
                }
            }
        }
    }
}
