using System;
using System.Windows;

namespace sjzd
{
    /// <summary>
    /// 预报产品制作窗口.xaml 的交互逻辑
    /// </summary>
    public partial class 预报产品制作窗口 : Window
    {
        public 预报产品制作窗口()
        {
            InitializeComponent();
        }

        private void SQYBBtu_Click(object sender, RoutedEventArgs e)
        {


            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    社区精细化 sq = new 社区精细化();
                    sq.DCWord(ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void Z308Btu_Click(object sender, RoutedEventArgs e)
        {

            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    逐3小时 z3 = new 逐3小时();
                    z3.DCWord(8, ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }

        }

        private void Z320Btu_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    逐3小时 z3 = new 逐3小时();
                    z3.DCWord(20, ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void SSQButton_Click(object sender, RoutedEventArgs e)
        {
            市四区 ssqWindow = new 市四区();
            ssqWindow.Show();
        }

        private void DQ08Btu_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择2 ryxz = new 预报人员选择2();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0 && ryxz.FBCom.Text.Trim().Length > 0)
                {
                    短期预报 dq = new 短期预报();
                    dq.DCWord(8, ryxz.ZBCom.Text, ryxz.FBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void DQ20Btu_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择2 ryxz = new 预报人员选择2();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0 && ryxz.FBCom.Text.Trim().Length > 0)
                {
                    短期预报 dq = new 短期预报();
                    dq.DCWord(20, ryxz.ZBCom.Text, ryxz.FBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void DS08_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    短时预报 DS = new 短时预报();
                    DS.DCWord(8, ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void DS14_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    短时预报 DS = new 短时预报();
                    DS.DCWord(14, ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void DS20_Click(object sender, RoutedEventArgs e)
        {
            预报人员选择 ryxz = new 预报人员选择();
            if (ryxz.ShowDialog() == false)
            {
                if (ryxz.ZBCom.Text.Trim().Length > 0)
                {
                    短时预报 DS = new 短时预报();
                    DS.DCWord(20, ryxz.ZBCom.Text, ryxz.QFCom.Text);
                }

            }
        }

        private void ZQBtu_Click(object sender, RoutedEventArgs e)
        {
            中期逐日 ZQ = new 中期逐日();
            ZQ.SJCL(DateTime.Now);
        }

        private void KKBtu_Click(object sender, RoutedEventArgs e)
        {
            科开服务窗口 ZQ = new 科开服务窗口();
            ZQ.Show();
        }

        private void STBtu_Click(object sender, RoutedEventArgs e)
        {
            生态选择1 stwin = new 生态选择1();
            stwin.Show();
        }

        private void Hbj_Click(object sender, RoutedEventArgs e)
        {
            环保局预报 hbj = new 环保局预报();
            hbj.DCWord(DateTime.Now, 8);
        }

        private void Hbj20_Click(object sender, RoutedEventArgs e)
        {
            if (DateTime.Now.Hour >= 17)
            {

                环保局预报 hbj = new 环保局预报();
                hbj.DCWord(DateTime.Now, 20);
            }
            else
            {

                环保局预报 hbj = new 环保局预报();
                hbj.DCWord(DateTime.Now.AddDays(-1), 20);
            }
        }

        private void hourYB06_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hourYB20_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hourYB08_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
