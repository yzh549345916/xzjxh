using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 逐日评分详情.xaml 的交互逻辑
    /// </summary>
    public partial class 城镇预报逐日评分详情 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private ObservableCollection<城镇预报逐日评分ViewModel> 评分列表 = new My城镇预报逐日评分ViewModel().Clubs;
        private string xlsPath = "";
        public 城镇预报逐日评分详情()
        {

            InitializeComponent();
            BTLabel.Content = "城镇精细化预报逐日评分详情查询";


            try
            {
                string sc = scchoose.SelectedItem.ToString();
                sc = sc.Split(':')[1].Trim();
                gwCSH(sc);
                scchoose.SelectionChanged += scchoose_SelectionChanged;
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Header = "警告"
                });
            }
            GRPFList.ItemsSource = 评分列表;
            this.splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            this.splashScreenDataContext.IsIndeterminate = true;
            this.splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
        }
        private void CXButton_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();

            if (!(sDate.SelectedDate.ToString().Length == 0))
            {
                BTLabel.Content = $"{Convert.ToDateTime(sDate.SelectedDate):yyyy-MM-dd}{gwchoose.Text}{scchoose.Text}时{SXSelect.Text}小时城镇预报逐日评分详情";
                评分列表.Clear();
                城镇预报统计类 tj = new 城镇预报统计类();
                ObservableCollection<城镇预报逐日评分ViewModel> listls =tj.zrpftj(scchoose.Text, Convert.ToDateTime(sDate.SelectedDate), gwchoose.Text, SXSelect.Text);
                foreach(城镇预报逐日评分ViewModel item in listls)
                {
                    评分列表.Add(item);
                }

            }
            else
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = "请选择起止时间",
                    Header = "警告"
                });
            }
            RadSplashScreenManager.Close();
        }
        private void scchoose_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string sc = scchoose.SelectedItem.ToString();
            sc = sc.Split(':')[1].Trim();
            gwCSH(sc);
        }
        void gwCSH(string sc)
        {
            try
            {
                int gwchooseCount = 0;
                Dictionary<int, string> mydic = new Dictionary<int, string>();
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\城镇预报\GWList.txt",
                    Encoding.Default))
                {
                    string line = "";

                    Int16 intCount = 0;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0 && line.Split('=')[0] == sc + "岗位列表")
                        {
                            string[] szls = line.Split('=')[1].Split(',');
                            foreach (string ssls in szls)
                            {
                                mydic.Add(intCount++, ssls);

                            }
                            break;
                        }

                    }
                    gwchoose.ItemsSource = mydic;
                    gwchoose.SelectedValuePath = "Key";
                    gwchoose.DisplayMemberPath = "Value";
                }
                gwchoose.SelectedValue = 0;
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }
        private void OnConfirmClosed_打开产品(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true) Process.Start(xlsPath);
        }

        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            导出查询表格();




        }
        private void 导出查询表格()
        {
            RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
            openFileDialog.Owner = this;
            openFileDialog.ShowDialog();
            if (openFileDialog.DialogResult == true)
            {
                string strPath = openFileDialog.FileName + "\\" + BTLabel.Content + ".xlsx";

                try
                {
                    ;
                    using (Stream stream = File.Create(strPath))
                    {
                        GRPFList.ExportToXlsx(stream, new GridViewDocumentExportOptions
                        {
                            ShowColumnHeaders = true,
                            ShowColumnFooters = true,
                            ShowGroupFooters = true,
                            ShowGroupRows = true,
                            ExportDefaultStyles = true,
                            AutoFitColumnsWidth = true,
                            ShowGroupHeaderRowAggregates = true
                        });
                    }

                    xlsPath = strPath;
                    RadWindow.Confirm(new DialogParameters
                    {
                        Content = "已成功导出数据至" + strPath + "\n是否打开？",
                        Closed = OnConfirmClosed_打开产品,
                        CancelButtonContent = "否",
                        Header = "提示",
                        OkButtonContent = "是"
                    });
                }
                catch (Exception ex)
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = ex.Message,
                        Header = "警告"
                    });
                }
            }
        }


    }

    public class My城镇预报逐日评分ViewModel : ViewModelBase
    {
        private ObservableCollection<城镇预报逐日评分ViewModel> clubs;

        public ObservableCollection<城镇预报逐日评分ViewModel> Clubs
        {
            get
            {
                if (clubs == null) clubs = CreateClubs();

                return clubs;
            }
        }

        private ObservableCollection<城镇预报逐日评分ViewModel> CreateClubs()
        {
            ObservableCollection<城镇预报逐日评分ViewModel> clubs = new ObservableCollection<城镇预报逐日评分ViewModel>();
            return clubs;
        }
    }
}
