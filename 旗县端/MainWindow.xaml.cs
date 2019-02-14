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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace 旗县端
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BUFB_Click(object sender, RoutedEventArgs e)
        {
            登录窗口 dlwindow = new 登录窗口();
            dlwindow.Show();
            
        }

        private void UpdataBu_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult dr = MessageBox.Show("更新将关闭当前客户端，请确认是否继续", "请注意", MessageBoxButton.YesNo);
            if (dr == MessageBoxResult.Yes)
            {
                try
                {
                    string updatePath = Environment.CurrentDirectory + @"\QX-update.exe";
                    string strML = Environment.CurrentDirectory;
                    Process pr = new Process();//声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = updatePath;//指定运行的程序
                    pr.Start();//运行
                    System.Environment.Exit(0);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void JYBu_Click(object sender, RoutedEventArgs e)
        {
            预报检验窗口 YBJYwindow = new 预报检验窗口();
            YBJYwindow.Show();
        }

        private void Config_Click(object sender, RoutedEventArgs e)
        {
            旗县设置窗口 qxszWindow = new 旗县设置窗口();
            qxszWindow.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                区局智能网格 znwg = new 区局智能网格();
                string error = "";
                string data = znwg.CIMISS_ZDbyID("53368", ref error);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
