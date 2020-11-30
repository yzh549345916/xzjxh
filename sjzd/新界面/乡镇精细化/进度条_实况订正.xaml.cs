using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;

namespace sjzd
{
    /// <summary>
    /// SettingsWindowContent.xaml 的交互逻辑
    /// </summary>
    public partial class 进度条_实况订正 : UserControl
    {
        private string _YBdata = "";
        public short inputInt = 0;
        public string inputStr1 = "", inputStr2 = "", inputStr3 = "", inputStr4 = "";
        private 进度条ViewModel jdtView = new 进度条ViewModel();
        private List<环保局预报.YBList> mylists = new List<环保局预报.YBList>();
        public string myPath = "";
        public string strError = "";
        public string[,] szYB;

        public 进度条_实况订正()
        {
            InitializeComponent();

            jdtView.myValue = 0;
            DataContext = jdtView;
            LoadingLabel.Text = "数据加载中...";
        }

        public 进度条_实况订正(string name)
        {
            InitializeComponent();

            jdtView.myValue = 0;
            DataContext = jdtView;
            LoadingLabel.Text = "数据加载中...";
            if (name == "开始实况订正")
                开始实况订正();
            else if (name == "生成乡镇精细化预报产品")
            {
                Thread thread = new Thread(生成乡镇精细化预报产品);
                thread.Start();
            }
            else if (name == "区局智能网格生成乡镇精细化")
            {
                Thread thread = new Thread(区局智能网格生成乡镇精细化);
                thread.Start();
            }
            else if (name == "生成社区精细化预报")
            {
                Thread thread = new Thread(生成社区精细化预报);
                thread.Start();
            }
            else if (name == "生成市四区精细化预报产品")
            {
                Thread thread = new Thread(生成市四区精细化预报产品);
                thread.Start();
            }
            else if (name == "生成短期预报产品")
            {
                Thread thread = new Thread(生成短期预报产品);
                thread.Start();
            }
            else if (name == "生成短时预报产品")
            {
                Thread thread = new Thread(生成短时预报产品);
                thread.Start();
            }
            else if (name == "生成防凌预报产品")
            {
                Thread thread = new Thread(生成防凌预报产品);
                thread.Start();
            }
            else if (name == "生成逐3小时预报产品")
            {
                Thread thread = new Thread(生成逐3小时预报产品);
                thread.Start();
            }
            else if (name == "生成中期逐日预报产品")
            {
                Thread thread = new Thread(生成中期逐日预报产品);
                thread.Start();
            }
            else if (name == "生成环保局预报产品")
            {
                Thread thread = new Thread(生成环保局预报产品);
                thread.Start();
            }
            else if (name == "生成赛罕智能网格产品")
            {
                Thread thread = new Thread(生成赛罕智能网格产品);
                thread.Start();
            }

        }

        public void 开始实况订正()
        {
            Thread thread = new Thread(实况订正);
            thread.Start();
        }

        private void RadProgressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int value = Convert.ToInt32(RadProgressBar1.Value / (RadProgressBar1.Maximum - RadProgressBar1.Minimum) * 100);

            if (RadProgressBar1.Value >= RadProgressBar1.Maximum)
            {
                try
                {
                    // PercentageLabel.Text = 100 + " %";
                    LoadingLabel.Text = "数据加载完成";
                    RadWindow radWindow = Parent as RadWindow;
                    radWindow.Close();
                }
                catch (Exception)
                {
                }
            }

            // else
            // PercentageLabel.Text = value + " %";
        }

        public void 实况订正()
        {
            bool XZQXYZ = false; //标致乡镇与所属旗县预报是否完全一致
            classCZSJZD CZSJZD1 = new classCZSJZD();
            string[,] YBSZ = CZSJZD1.ZDYBCL(CZSJZD1.readXZYBtxt());
            try
            {
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\设置.txt", Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "乡镇与城镇指导一致")
                        {
                            if (line.Split('=')[1] == "是")
                            {
                                XZQXYZ = true;
                            }
                            else
                            {
                                XZQXYZ = false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }

            try
            {
                ZDSZCL zdszcl = new ZDSZCL();
                if (!XZQXYZ)
                {
                    classHQSK hqsk = new classHQSK();
                    string strQXSK = hqsk.CIMISSHQQXSK();
                    string strXZSK = hqsk.CIMISSHQXZSK(strQXSK, ref strError); //错误内容包括站点实况确实提示
                    szYB = CZSJZD1.CZCL(YBSZ, strQXSK, strXZSK, ref strError); //错误内容包括乡镇与旗县温差大于5℃提示
                }
                else
                {
                    try
                    {
                        string configXZPath = Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                        int XZGS = 0, intQXGS = 0;
                        int lineCount = 0;
                        int i = 0;
                        using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                        {
                            string line = sr.ReadLine();
                            intQXGS = Convert.ToInt32(line.Split(':')[1]);
                        }

                        using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                        {
                            while (i < intQXGS)
                            {
                                string line = sr.ReadLine();
                                if (2 * i + 1 == lineCount)
                                {
                                    XZGS += line.Split(',').GetLength(0);
                                    i++;
                                }

                                lineCount++;
                            }
                        }

                        szYB = new string[XZGS, 30]; //数组行数为旗县个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
                        using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                        {
                            i = 0;
                            lineCount = 0;
                            string line = "";
                            int idCount = 0, nameCount = 0;
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (2 * i == lineCount && i != 0)
                                {
                                    int count = 0;
                                    string[] szls = line.Split(',');
                                    for (int j = 0; j < YBSZ.GetLength(0); j++)
                                    {
                                        if (YBSZ[j, 0] == szls[0])
                                        {
                                            count = j;
                                            break;
                                        }
                                    }

                                    for (int j = 0; j < szls.Length; j++)
                                    {
                                        for (int k = 1; k < YBSZ.GetLength(1); k++)
                                        {
                                            szYB[idCount, k + 1] = YBSZ[count, k];
                                        }

                                        szYB[idCount++, 1] = szls[j];
                                    }

                                    if (i == intQXGS)
                                        break;
                                }

                                if (2 * i + 1 == lineCount)
                                {
                                    string[] szls = line.Split(',');
                                    for (int j = 0; j < szls.Length; j++)
                                    {
                                        szYB[nameCount++, 0] = szls[j];
                                    }

                                    i++;
                                }

                                lineCount++;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        RadWindow.Alert(new DialogParameters
                        {
                            Content = ex.Message,
                            Owner = Application.Current.MainWindow,
                            Header = "警告"
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                RadWindow.Alert(new DialogParameters
                {
                    Content = ex.Message,
                    Owner = Application.Current.MainWindow,
                    Header = "警告"
                });
            }

            jdtView.myValue += 100;
        }

        public void 生成乡镇精细化预报产品()
        {
            ZDSZCL zdszcl = new ZDSZCL();
            string error = zdszcl.centerBW2ZDBW();
            if (error.Length > 0)
            {
                Dispatcher.Invoke(() =>
                {
                    RadWindow.Alert(new DialogParameters
                    {
                        Content = "报文处理失败，请确认是否生成当天乡镇精细化预报报文\r\n" + error,
                        Owner = Application.Current.MainWindow,
                        Header = "警告"
                    });
                });
            }
            else
            {
                myPath = zdszcl.ZDYBBWtoSZnew(DateTime.Now.ToString("yyyyMMdd"));
            }

            jdtView.myValue += 100;
        }

        public void 区局智能网格生成乡镇精细化()
        {
            try
            {
                strError = "";
                classCZSJZD CZSJZD1 = new classCZSJZD();
                string myerror = "";
                string myYBdata = CZSJZD1.readXZYBtxtNew(ref myerror);
                if (myerror.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow.Confirm(new DialogParameters
                        {
                            Content = myerror,
                            Closed = OnConfirmClosed_手动选择指导预报,
                            Owner = Application.Current.MainWindow,
                            CancelButtonContent = "否",
                            OkButtonContent = "是"
                        });
                    });
                }
                else
                {
                    string[,] YBSZ = CZSJZD1.ZDYBCL(myYBdata);
                    区局智能网格 qjClass1 = new 区局智能网格();
                    if (YBSZ.GetLength(0) > 0)
                    {
                        szYB = qjClass1.CLZNData(YBSZ, ref strError);
                        if (strError.Length > 0 && strError.Contains("但是乡镇报文温度为-99"))
                        {
                            Dispatcher.Invoke(() =>
                            {
                                RadWindow.Alert(new DialogParameters
                                {
                                    Content = strError,
                                    Owner = Application.Current.MainWindow,
                                    Header = "警告"
                                });
                            });
                            strError = "";
                        }
                    }
                    else
                    {
                        strError = "区局智能网格获取失败";
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        private void OnConfirmClosed_手动选择指导预报(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadOpenFileDialog openFileDialog = new RadOpenFileDialog();
                        openFileDialog.Filter = "文本 (*.txt)|*.txt";
                        openFileDialog.Owner = this;
                        openFileDialog.ShowDialog();
                        if (openFileDialog.DialogResult == true)
                        {
                            string YBPath = openFileDialog.FileName;
                            StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                            string YBdata = sr.ReadToEnd();
                            classCZSJZD CZSJZD1 = new classCZSJZD();
                            string[,] YBSZ = CZSJZD1.ZDYBCL(YBdata);
                            区局智能网格 qjClass1 = new 区局智能网格();
                            if (YBSZ.GetLength(0) > 0)
                            {
                                szYB = qjClass1.CLZNData(YBSZ, ref strError);
                                if (strError.Length > 0 && strError.Contains("但是乡镇报文温度为-99"))
                                {
                                    Dispatcher.Invoke(() =>
                                    {
                                        RadWindow.Alert(new DialogParameters
                                        {
                                            Content = strError,
                                            Owner = Application.Current.MainWindow,
                                            Header = "警告"
                                        });
                                    });
                                    strError = "";
                                }
                            }
                            else
                            {
                                strError = "区局智能网格获取失败";
                            }
                        }
                        else
                        {
                            strError = "指导预报获取失败";
                        }
                    });
                }
                else
                {
                    strError = "指导预报获取失败";
                }
            }
            catch
            {
                strError = "指导预报获取失败";
            }
        }

        private void OnConfirmClosed_市四区手动选择指导预报(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadOpenFileDialog openFileDialog = new RadOpenFileDialog();
                        openFileDialog.Filter = "文本 (*.txt)|*.txt";
                        openFileDialog.Owner = this;
                        openFileDialog.ShowDialog();
                        if (openFileDialog.DialogResult == true)
                        {
                            string YBPath = openFileDialog.FileName;
                            StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                            string myYBdata = sr.ReadToEnd();
                            classCZSJZD CZSJZD1 = new classCZSJZD();
                            string[,] YBSZ = CZSJZD1.ZDYBCL(myYBdata);
                            strError = "";
                            string strData = "", SKData = "";
                            处理市四区数据(ref strData, ref SKData, ref strError);
                            classSSQSJZD zdybCL = new classSSQSJZD();
                            string[,] ssqSZ = zdybCL.CZCL(YBSZ, strData, SKData, ref strError);

                            if (ssqSZ.GetLength(0) > 0)
                            {
                                myPath = CZSJZD1.DCWordNew(ssqSZ, inputStr1, inputStr2, inputStr3, ref strError);
                            }
                            else
                            {
                                strError = "产品生成取失败";
                            }
                        }
                        else
                        {
                            strError = "指导预报获取失败";
                        }
                    });
                }
                else
                {
                    strError = "指导预报获取失败";
                }
            }
            catch
            {
                strError = "指导预报获取失败";
            }
        }

        private void OnConfirmClosed_中期逐日手动选择指导预报(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadOpenFileDialog openFileDialog = new RadOpenFileDialog();
                        openFileDialog.Filter = "文本 (*.txt)|*.txt";
                        openFileDialog.Owner = this;
                        openFileDialog.ShowDialog();
                        if (openFileDialog.DialogResult == true)
                        {
                            try
                            {
                                string YBPath = openFileDialog.FileName;
                                string data = "";
                                using (StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312")))
                                {
                                    bool bs = false;
                                    string line = "";
                                    data = "                  呼和浩特市气象台" + DateTime.Now.ToString("yyyy年MM月dd日") + "中期逐日预报" + "\r\n\r\n";
                                    while ((line = sr.ReadLine()) != null)
                                    {
                                        if (line.Contains("72--96小时预报"))
                                            bs = true;
                                        if (bs)
                                        {
                                            data += line + "\r\n";
                                        }
                                    }
                                }

                                if (data.Trim().Length > 0)
                                {
                                    try
                                    {
                                        StreamWriter sw2 = new StreamWriter(myPath, false, Encoding.GetEncoding("GB2312"));
                                        sw2.Write(data);
                                        sw2.Close();
                                    }
                                    catch
                                    {
                                        strError = "产品生成取失败";
                                    }
                                }
                                else
                                {
                                    strError = "产品生成取失败";
                                }
                            }
                            catch
                            {
                                strError = "产品生成取失败";
                            }
                        }
                        else
                        {
                            strError = "指导预报获取失败";
                        }
                    });
                }
                else
                {
                    strError = "指导预报获取失败";
                }
            }
            catch
            {
                strError = "指导预报获取失败";
            }
        }

        private void OnConfirmClosed_继续生成环保局专报(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    环保局预报 hbj = new 环保局预报();
                    myPath = hbj.处理环保数据(mylists, inputInt, ref strError);
                }
                else
                {
                    strError = "智能网格数据获取失败";
                }
            }
            catch
            {
                strError = "智能网格数据获取失败";
            }
        }

        public void 生成社区精细化预报()
        {
            try
            {
                社区精细化 sq = new 社区精细化();
                strError = "";
                myPath = "";
                myPath = sq.DCWordNew(inputStr1, inputStr2, ref strError);
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        public void 生成市四区精细化预报产品()
        {
            try
            {
                strError = "";
                classSSQSJZD CZSJZD1 = new classSSQSJZD();
                string myerror = "";
                string myYBdata = CZSJZD1.readXZYBtxt(ref myerror);
                if (myerror.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow.Confirm(new DialogParameters
                        {
                            Content = myerror,
                            Closed = OnConfirmClosed_市四区手动选择指导预报,
                            Owner = Application.Current.MainWindow,
                            CancelButtonContent = "否",
                            OkButtonContent = "是"
                        });
                    });
                }
                else
                {
                    string[,] YBSZ = CZSJZD1.ZDYBCL(myYBdata);
                    strError = "";
                    string strData = "", SKData = "";
                    处理市四区数据(ref strData, ref SKData, ref strError);
                    classSSQSJZD zdybCL = new classSSQSJZD();
                    string[,] ssqSZ = zdybCL.CZCL(YBSZ, strData, SKData, ref strError);

                    if (ssqSZ.GetLength(0) > 0)
                    {
                        myPath = CZSJZD1.DCWordNew(ssqSZ, inputStr1, inputStr2, inputStr3, ref strError);
                    }
                    else
                    {
                        strError = "产品生成取失败";
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        public void 生成短期预报产品()
        {
            strError = "";
            短期预报 dq = new 短期预报();
            myPath = dq.DCWordNew(inputInt, inputStr1, inputStr2, inputStr3, ref strError);
            jdtView.myValue += 100;
        }

        public void 生成防凌预报产品()
        {
            strError = "";
            防凌预报 DS = new 防凌预报();
            myPath = DS.DCWordNew(inputStr1, inputStr2, inputStr3, inputStr4, ref strError);
            jdtView.myValue += 100;
        }

        public void 生成短时预报产品()
        {
            strError = "";
            短时预报 DS = new 短时预报();
            myPath = DS.DCWordNew(inputInt, inputStr1, inputStr2, ref strError);
            jdtView.myValue += 100;
        }

        public void 生成逐3小时预报产品()
        {
            strError = "";
            逐3小时 z3 = new 逐3小时();
            myPath = z3.DCWordNew(inputInt, inputStr1, inputStr2, ref strError);
            jdtView.myValue += 100;
        }

        public void 生成中期逐日预报产品()
        {
            try
            {
                strError = "";
                string myerror = "";
                中期逐日 ZQ = new 中期逐日();

                string data = ZQ.SJCLNew(ref myPath, ref myerror);
                if (myerror.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        RadWindow.Confirm(new DialogParameters
                        {
                            Content = myerror,
                            Closed = OnConfirmClosed_中期逐日手动选择指导预报,
                            Owner = Application.Current.MainWindow,
                            CancelButtonContent = "否",
                            OkButtonContent = "是"
                        });
                    });
                }
                else
                {
                    if (data.Trim().Length > 0)
                    {
                        try
                        {
                            StreamWriter sw2 = new StreamWriter(myPath, false, Encoding.GetEncoding("GB2312"));
                            sw2.Write(data);
                            sw2.Close();
                        }
                        catch (Exception)
                        {
                            strError = "产品生成取失败";
                        }
                    }
                    else
                    {
                        strError = "产品生成取失败";
                    }
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        public void 生成环保局预报产品()
        {
            try
            {
                strError = "";
                string myerror = "";
                环保局预报 hbj = new 环保局预报();
                mylists = hbj.DCWordNew(inputInt, ref myerror, ref strError);
                if (myerror.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ScrollViewer scrollViewer = new ScrollViewer();
                        scrollViewer.Content = myerror + "\r\n是否继续生成产品";
                        RadWindow.Confirm(new DialogParameters
                        {
                            Content = scrollViewer,
                            Closed = OnConfirmClosed_继续生成环保局专报,
                            Owner = Application.Current.MainWindow,
                            CancelButtonContent = "否",
                            OkButtonContent = "是",
                            Header = "提示"
                        });
                    });
                }
                else
                {
                    myPath = hbj.处理环保数据(mylists, inputInt, ref strError);
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        public void 生成蒙草预报产品()
        {
            try
            {
                strError = "";
                蒙草预报 mcyb = new 蒙草预报();
                var lists = mcyb.获取国家智能网格(8);


                string data = "区站号\t名称\t预报时效\t温度\t降水量\t风u\t风v\t风向\t风速\t相对湿度\t天气\t能见度\r\n";
                foreach (var item in lists)
                {
                    data += $"{item.ID}\t{item.Name}\t{item.SX}\t{item.TEM}\t{item.PRE_3H}\t{item.WIU10}\t{item.WIV10}\t{item.FX}\t{item.FS}\t{item.ERH}\t{item.VIS}\r\n";
                }
                myPath = Environment.CurrentDirectory + @"\产品\蒙草临时\";
                if (!Directory.Exists(myPath))
                    Directory.CreateDirectory(myPath);
                myPath += $"{DateTime.Now:yyyy年MM月dd日}.txt";
                using (StreamWriter sw = new StreamWriter(myPath, false, Encoding.GetEncoding("GB2312")))
                {
                    sw.Write(data);
                    sw.Flush();
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        public void 生成赛罕智能网格产品()
        {
            try
            {
                strError = "";
                string myerror = "";
                赛罕智能网格产品 hbj = new 赛罕智能网格产品();
                mylists = hbj.DCWordNew(inputInt, inputStr1, ref myerror, ref strError);
                if (myerror.Length > 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        ScrollViewer scrollViewer = new ScrollViewer();
                        scrollViewer.Content = myerror + "\r\n是否继续生成产品";
                        RadWindow.Confirm(new DialogParameters
                        {
                            Content = scrollViewer,
                            Closed = OnConfirmClosed_继续生成赛罕智能网格产品,
                            Owner = Application.Current.MainWindow,
                            CancelButtonContent = "否",
                            OkButtonContent = "是",
                            Header = "提示"
                        });
                    });
                }
                else
                {
                    myPath = hbj.处理环保数据(mylists, inputInt, ref strError);
                }
            }
            catch (Exception ex)
            {
                strError = ex.Message;
            }

            jdtView.myValue += 100;
        }

        private void OnConfirmClosed_继续生成赛罕智能网格产品(object sender, WindowClosedEventArgs e)
        {
            try
            {
                if (e.DialogResult == true)
                {
                    赛罕智能网格产品 hbj = new 赛罕智能网格产品();
                    hbj.idnameCSH(inputStr1);
                    myPath = hbj.处理环保数据(mylists, inputInt, ref strError);
                }
                else
                {
                    strError = "智能网格数据获取失败";
                }
            }
            catch
            {
                strError = "智能网格数据获取失败";
            }
        }

        public void 处理市四区数据(ref string strData, ref string SKData, ref string myerror)
        {
            DateTime dt = DateTime.Now;
            string SSQconPath = Environment.CurrentDirectory + @"\设置文件\市四区\市四区配置.txt";
            string configZDPath = Environment.CurrentDirectory + @"\设置文件\市四区\市四区站点.txt";
            string DZTime = "15";
            try
            {
                using (StreamReader sr1 = new StreamReader(SSQconPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";

                    // 从文件读取数据库配置信息 
                    while ((line1 = sr1.ReadLine()) != null)
                    {
                        if (line1.Split('=')[0] == "订正市局指导实况时次")
                        {
                            DZTime = line1.Split('=')[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            DZTime = DateTime.Now.Hour.ToString();
            string strToday = dt.ToUniversalTime().ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            DateTime dtLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null);
            if (dtLS.CompareTo(DateTime.Now) > 0)
            {
                myerror = "预报时间太早了，是不是考虑晚点再做？";
                return;
            }

            strLS = dtLS.ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000"); //ToUniversalTime将时间转换为UTC
            string XZID = "";
            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            string userId = "BEHT_BFHT_2131"; // 
            string pwd = "YZHHGDJM"; // 
            /*   2.2 接口ID */
            string interfaceId1 = "getSurfEleByTimeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<string, string> paramsqx = new Dictionary<string, string>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            //检索时间段
            paramsqx.Add("times", strToday);
            string StationID = "";
            string ZDXX = "";
            try
            {
                using (StreamReader sr1 = new StreamReader(configZDPath, Encoding.GetEncoding("GB2312")))
                {
                    string line1 = "";
                    while ((line1 = sr1.ReadLine()) != null)
                    {
                        if (line1.Split('=').Length > 2)
                        {
                            ZDXX += line1 + '\n';
                            if (line1.Split('=')[1].Trim() == line1.Split('=')[2].Trim())
                            {
                                StationID += line1.Split('=')[1].Trim() + ',';
                            }
                            else
                            {
                                XZID += line1.Split('=')[1].Trim() + ',';
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                myerror = ex.Message;
                return;
            }

            ZDXX = ZDXX.Substring(0, ZDXX.Length - 1);
            StationID = StationID.Substring(0, StationID.Length - 1);
            XZID = XZID.Substring(0, XZID.Length - 1);
            ;
            paramsqx.Add("staIds", StationID); //选择区站号
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h"); // 检索要素：站名，乡镇，区站号，过去24小时最高、最低温度，降水量
            string dataFormat = "tabText";
            StringBuilder QXSK = new StringBuilder(); //返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            strData = Convert.ToString(QXSK);
            string[] SZlinshi = strData.Split(new[]
            {
                "\r\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            strData = "";
            /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
            for (int i = 0; i < SZlinshi.Length; i++)
            {
                if (i > 1)
                {
                    strData += SZlinshi[i] + '\n';
                }
            }

            strData = strData.Substring(0, strData.Length - 1);

            /* 1. 定义client对象 */
            DataQueryClient client2 = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            //String userId = "BEHT_BFHT_2131";// 
            //String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            string interfaceId2 = "statSurfEleByStaID";
            Dictionary<string, string> params2 = new Dictionary<string, string>();
            params2.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            string strYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000"); //ToUniversalTime将时间转换为UTC
            string timeRange1 = "(" + strYesterday + "," + strToday + "]";
            params2.Add("timeRange", timeRange1);
            params2.Add("staIds", XZID); //选择区站号，从乡镇名单中获取
            params2.Add("elements", "Station_Name,Cnty,Station_Id_C"); // 检索要素：站号、旗县、区站号
            params2.Add("statEles", "MAX_TEM_Max,MIN_TEM_MIN,SUM_PRE_1h"); // 统计要素最高温度的最大值，与最低温度的最小值以及小时降水量
            /*   2.4 返回文件的格式 */
            StringBuilder retStrXZ = new StringBuilder(); //返回字符串
            client2.initResources();
            // 调用接口
            int rst2 = client2.callAPI_to_serializedStr(userId, pwd, interfaceId2, params2, dataFormat, retStrXZ);
            client.destroyResources();
            string xzData = Convert.ToString(retStrXZ);

            string[] SZlinshi2 = xzData.Split(new[]
            {
                "\r\n"
            }, StringSplitOptions.RemoveEmptyEntries);
            xzData = "";
            for (int i = 0; i < SZlinshi2.Length; i++)
            {
                if (i > 1)
                {
                    xzData += SZlinshi2[i] + '\n';
                }
            }

            xzData = xzData.Substring(0, xzData.Length - 1);
            //xzData = strData + '\n' + xzData;//将所有实况存入szData
            string[] xxSZ = ZDXX.Split('\n');
            try
            {
                for (int i = 0; i < xxSZ.Length; i++)
                {
                    string zdLS = xxSZ[i].Split('=')[1]; //该乡镇的区站号
                    string zdLS2 = xxSZ[i].Split('=')[2]; //保存该乡镇订正依据的旗县区站号
                    string MainData = "";
                    string[] mainDaSZ = strData.Split('\n');
                    for (int j = 0; j < mainDaSZ.Length; j++)
                    {
                        if (mainDaSZ[j].Contains(zdLS2))
                        {
                            MainData = mainDaSZ[j].Split('\t')[3] + '\t' + mainDaSZ[j].Split('\t')[4] + '\t' + mainDaSZ[j].Split('\t')[5];
                        }
                    }

                    if (!xzData.Contains(zdLS))
                    {
                        SKData += xxSZ[i].Split('=')[0] + '\t' + xxSZ[i].Split('=')[0] + '\t' + xxSZ[i].Split('=')[1] + '\t' + MainData + '\n';
                    }
                    else
                    {
                        string[] xzSZ = xzData.Split('\n');
                        for (int j = 0; j < xzSZ.Length; j++)
                        {
                            if (xzSZ[j].Contains(zdLS))
                            {
                                string[] szLS = xzSZ[j].Split('\t');
                                if (Convert.ToDouble(szLS[3]) > 999 || Convert.ToDouble(szLS[3]) < -999 || Convert.ToDouble(szLS[4]) > 999 || Convert.ToDouble(szLS[4]) < -999)
                                {
                                    SKData += szLS[0] + '\t' + szLS[1] + '\t' + szLS[2] + '\t' + MainData + '\n';
                                }
                                else
                                {
                                    SKData += xzSZ[j] + '\n';
                                }
                            }
                        }
                    }
                }
            }
            //SKData保存实况资料
            catch (Exception ex)
            {
                myerror = ex.Message;
            }
        }
    }
}