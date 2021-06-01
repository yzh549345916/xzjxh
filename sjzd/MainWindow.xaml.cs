using Aspose.Words;
using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using MessageBox = System.Windows.MessageBox;


namespace sjzd
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //private NotifyIcon _notifyIcon = null;
        public MainWindow()
        {
            InitializeComponent();


        }



        //将报文转换为数组导出至word产品,待编写完报文处理程序后继续编写此处程序

        private void QXConfig_Click(object sender, RoutedEventArgs e)
        {
            QXXZConfig windowConfig = new QXXZConfig();
            windowConfig.Show();
        }

        private void SJZD_YBCZ_Click(object sender, RoutedEventArgs e)
        {
            bool XZQXYZ = false;//标致乡镇与所属旗县预报是否完全一致
            classCZSJZD CZSJZD1 = new classCZSJZD();
            string[,] YBSZ = CZSJZD1.ZDYBCL(CZSJZD1.readXZYBtxt());
            try
            {
                using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\设置文件\设置.txt", Encoding.GetEncoding("GB2312")))
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
                MessageBox.Show(ex.Message);
            }
            try
            {

                ZDSZCL zdszcl = new ZDSZCL();
                if (!XZQXYZ)
                {
                    string strError = "";
                    classHQSK hqsk = new classHQSK();
                    string strQXSK = hqsk.CIMISSHQQXSK();
                    string strXZSK = hqsk.CIMISSHQXZSK(strQXSK, ref strError);//错误内容包括站点实况确实提示
                    string[,] szYB = CZSJZD1.CZCL(YBSZ, strQXSK, strXZSK, ref strError);//错误内容包括乡镇与旗县温差大于5℃提示
                                                                                        //errorBox.Text = strError;//后续根据错误字符串是否为0弹框显示
                    errorBox.Text = strError;

                    zdszcl.ZDSZ2BW(szYB);
                }
                else
                {
                    try
                    {
                        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
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
                                if ((2 * i + 1) == lineCount)
                                {
                                    XZGS += line.Split(',').GetLength(0);
                                    i++;
                                }
                                lineCount++;

                            }
                        }
                        string[,] szYB = new string[XZGS, 30];//数组行数为旗县个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
                        using (StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                        {
                            i = 0;
                            lineCount = 0;
                            string line = "";
                            int idCount = 0, nameCount = 0;
                            while ((line = sr.ReadLine()) != null)
                            {

                                if ((2 * i == lineCount) && i != 0)
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
                                if ((2 * i + 1) == lineCount)
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
                        zdszcl.ZDSZ2BW(szYB);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                if (MessageBoxResult.Yes == MessageBox.Show("已根据实况与指导预报生成乡镇精细化预报报文，是否需要修改。\r\n如果选是，则自动进入发报软件。\r\n如果选否，则直接生成最终的指导预报报文与产品清单。", "请选择", MessageBoxButton.YesNo))
                {
                    string FBPath = "";
                    string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "CITYFORECAST路径")
                            {
                                FBPath = line.Split('=')[1];
                            }
                        }
                    }
                    静态类.OpenBrowser(FBPath);
                    //p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                }
                else
                {
                    zdszcl.centerBW2ZDBW();
                    zdszcl.ZDYBBWtoSZ(DateTime.Now.ToString("yyyyMMdd"));

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ZZFBD_Click(object sender, RoutedEventArgs e)
        {
            //string error="";
            // 环保局预报 hb = new 环保局预报();
            // hb.DCWord( DateTime.Now.AddDays(-2),20);
            ZDSZCL zdszcl = new ZDSZCL();
            zdszcl.ZDYBBWtoSZ(DateTime.Now.ToString("yyyyMMdd"));

        }

        private void SSQButton_Click(object sender, RoutedEventArgs e)
        {
            市四区 ssqWindow = new 市四区();
            ssqWindow.Show();
        }

        private void YBJYButton_Click(object sender, RoutedEventArgs e)
        {

            预报检验窗口 YBJYwindow = new 预报检验窗口();
            YBJYwindow.Show();
        }



        private void airBtu_Click(object sender, RoutedEventArgs e)
        {
            空气质量监测 kqwindow = new 空气质量监测();
            kqwindow.Show();
        }

        private void QJZNBtu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                classCZSJZD CZSJZD1 = new classCZSJZD();
                string[,] YBSZ = CZSJZD1.ZDYBCL(CZSJZD1.readXZYBtxt());
                区局智能网格 qjClass1 = new 区局智能网格();
                if (YBSZ.GetLength(0) > 0)
                {
                    string error = "";
                    string[,] szYB = qjClass1.CLZNData(YBSZ, ref error);
                    ZDSZCL zdszcl = new ZDSZCL();
                    zdszcl.ZDSZ2BW(szYB);
                    if (MessageBoxResult.Yes == MessageBox.Show("已根据省级智能网格数据生成乡镇精细化预报报文，是否需要修改。\r\n如果选是，则自动进入发报软件。\r\n如果选否，则直接生成最终的指导预报报文与产品清单。", "请选择", MessageBoxButton.YesNo))
                    {
                        string FBPath = "";
                        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                        using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                if (line.Split('=')[0] == "CITYFORECAST路径")
                                {
                                    FBPath = line.Split('=')[1];
                                }
                            }
                        }
                        静态类.OpenBrowser(FBPath);
                        //p.WaitForExit();//关键，等待外部程序退出后才能往下执行
                    }
                    else
                    {
                        zdszcl.centerBW2ZDBW();
                        zdszcl.ZDYBBWtoSZ(DateTime.Now.ToString("yyyyMMdd"));

                    }
                }
                else
                {
                    MessageBox.Show("城镇指导预报获取失败");
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
        }

        private void CPZZ_Click(object sender, RoutedEventArgs e)
        {

            预报产品制作窗口 cpzz = new 预报产品制作窗口();
            cpzz.Show();
        }


    }


    //该类为利用实况与城镇指导预报差值计算乡镇精细化指导预报的方法
    public class classCZSJZD
    {
        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
        public string readXZYBtxt()//该方法读取城镇指导预报,返回指导预报整个内容
        {
            string YBPath = "";
            string YBdata = "";
            StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的路径配置文件中所有文本，寻找城镇指导预报路径
            while ((line = sr.ReadLine()) != null)
            {
                string[] linShi1 = line.Split('=');
                if (linShi1[0] == "城镇指导预报路径")
                {
                    YBPath = linShi1[1];
                }
            }
            sr.Close();
            string CZBWTime = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
            {

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr1.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市局读取城镇指导预报文件夹时次")
                    {
                        CZBWTime = line.Split('=')[1].Trim();
                        CZBWTime = '\\' + CZBWTime + '\\';
                    }

                }
            }
            YBPath = YBPath + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yy") + "." + DateTime.Now.ToString("MM") + CZBWTime + "呼市气象台指导预报" + DateTime.Now.ToString("MMdd") + ".txt";//文件路径为：基本路径+年后两位.月两位\06\呼市气象台指导预报+两位月两位日.txt
            //判断城镇指导预报是否存在，如果不存在，提示是否手动选择文件
            try
            {
                sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                YBdata = sr.ReadToEnd().ToString();
            }
            catch
            {
                MessageBoxResult result1 = System.Windows.MessageBox.Show(YBPath + "路径错误，是否手动选择乡镇指导预报文件", "错误", MessageBoxButton.YesNo);
                if (result1 == System.Windows.MessageBoxResult.Yes)
                {
                    Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog()
                    {
                        Filter = "文本 (*.txt)|*.txt"
                    };
                    bool? result = openFileDialog.ShowDialog();
                    if (result == true)
                    {
                        YBPath = openFileDialog.FileName;
                        sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                        YBdata = sr.ReadToEnd().ToString();
                    }
                }
            }


            return YBdata;
        }
        public string readXZYBtxtNew(ref string error)//该方法读取城镇指导预报,返回指导预报整个内容
        {
            string YBPath = "";
            string YBdata = "";
            StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的路径配置文件中所有文本，寻找城镇指导预报路径
            while ((line = sr.ReadLine()) != null)
            {
                string[] linShi1 = line.Split('=');
                if (linShi1[0] == "城镇指导预报路径")
                {
                    YBPath = linShi1[1];
                }
            }
            sr.Close();
            string CZBWTime = "";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
            {

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr1.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市局读取城镇指导预报文件夹时次")
                    {
                        CZBWTime = line.Split('=')[1].Trim();
                        CZBWTime = '\\' + CZBWTime + '\\';
                    }

                }
            }
            YBPath = YBPath + DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("yy") + "." + DateTime.Now.ToString("MM") + CZBWTime + "呼市气象台指导预报" + DateTime.Now.ToString("MMdd") + ".txt";//文件路径为：基本路径+年后两位.月两位\06\呼市气象台指导预报+两位月两位日.txt
            //判断城镇指导预报是否存在，如果不存在，提示是否手动选择文件
            try
            {
                sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312"));
                YBdata = sr.ReadToEnd().ToString();
            }
            catch
            {
                error = YBPath + "\r\n路径错误，是否手动选择乡镇指导预报文件";

            }


            return YBdata;
        }

        //返回数组每行内容为：旗县区站号+未来七天分别的天气、风向风速、最低气温、最高气温，
        public string[,] ZDYBCL(string YBData)
        {
            try
            {
                StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
                String line;
                //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
                line = sr.ReadLine();
                sr.Close();
                string[] linShi1 = line.Split(':');
                int intQXGS = Convert.ToInt16(linShi1[1]);
                string[,] zDYBSZ = new string[intQXGS, 7 * 4 + 1];//数组行数为旗县个数，每行内容为：旗县名称+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为1+4*7

                //给每行第一列赋值，为旗县的名称
                int lineCount = 0, i = 0;
                sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
                while (i < intQXGS)
                {
                    line = sr.ReadLine();
                    if ((2 * i + 1) == lineCount)
                    {
                        zDYBSZ[i++, 0] = line.Split(',')[0];
                    }
                    lineCount++;

                }
                sr.Close();
                string[] YBDataLines = YBData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (i = 0; i < intQXGS; i++)
                {
                    int k = 1;
                    for (int j = 0; j < YBDataLines.Length; j++)
                    {
                        linShi1 = YBDataLines[j].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                        //linShi1 = System.Text.RegularExpressions.Regex.Split(YBDataLines[j]，);
                        if (zDYBSZ[i, 0] == linShi1[0]||(linShi1[0] == "呼和浩特" && zDYBSZ[i, 0] == "赛罕区"))
                        {
                            zDYBSZ[i, k++] = linShi1[1];
                            zDYBSZ[i, k++] = linShi1[2];
                            zDYBSZ[i, k++] = linShi1[3];
                            zDYBSZ[i, k++] = linShi1[4];
                        }
                    }
                }
                lineCount = 0;
                i = 0;

                sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
                while (i < intQXGS)
                {
                    line = sr.ReadLine();
                    if ((2 * i + 2) == lineCount)
                    {
                        zDYBSZ[i++, 0] = line.Split(',')[0];
                    }
                    lineCount++;

                }
                sr.Close();
                return zDYBSZ;
            }
            catch
            {
            }

            return new string[1, 1];
        }//YBData为导出的指导预报内容
        public string DCWordNew(string[,] szYB, string ZBName, string FBName, string QFName, ref string error)
        {
            string myconfigpathPath = System.Environment.CurrentDirectory + @"\设置文件\市四区\市四区配置.txt";
            string SJsaPath = "";
            try
            {
                string SJMBPath = Environment.CurrentDirectory + @"\模版\市四区模板.doc";

                using (StreamReader sr = new StreamReader(myconfigpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "产品发布路径")
                        {
                            SJsaPath = line.Split('=')[1];
                        }
                    }
                }
                SJsaPath += DateTime.Now.ToString("yyyy-MM") + "\\";
                if (!File.Exists(SJsaPath))
                {
                    Directory.CreateDirectory(SJsaPath);
                }
                SJsaPath += DateTime.Now.ToString("yyyyMMdd") + ".doc";
                Document doc = new Document(SJMBPath);
                DocumentBuilder builder = new DocumentBuilder(doc);
                builder.CellFormat.Borders.LineStyle = LineStyle.Single;
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.MoveToBookmark("日期");
                builder.Font.Size = 12;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));
                string data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 2] + "，" + szYB[i, 3] + "，" + szYB[i, 4] + "～" + szYB[i, 5] + "℃" + "\r\n";
                }
                data = data[0..^2];
                builder.MoveToBookmark("预报24");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 6] + "，" + szYB[i, 7] + "，" + szYB[i, 8] + "～" + szYB[i, 9] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报48");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                data = "";
                for (int i = 0; i < szYB.GetLength(0); i++)
                {
                    data += szYB[i, 0] + "：" + szYB[i, 10] + "，" + szYB[i, 11] + "，" + szYB[i, 12] + "～" + szYB[i, 13] + "℃" + "\r\n";
                }
                data = data.Substring(0, data.Length - 2);
                builder.MoveToBookmark("预报72");
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(data);
                builder.MoveToBookmark("日期241");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.ToString("dd"));
                builder.MoveToBookmark("日期242");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期481");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(1).ToString("dd"));
                builder.MoveToBookmark("日期482");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期721");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(2).ToString("dd"));
                builder.MoveToBookmark("日期722");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(DateTime.Now.AddDays(3).ToString("dd"));
                builder.MoveToBookmark("主班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(ZBName);
                builder.MoveToBookmark("副班");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(FBName);
                builder.MoveToBookmark("签发");
                builder.CellFormat.Borders.Color = System.Drawing.Color.Black;
                builder.Font.Size = 13;
                builder.Font.Name = "宋体";
                builder.Write(QFName);
                doc.Save(SJsaPath);
                return SJsaPath;


            }

            catch (Exception ex)
            {
                error = ex.Message;
            }

            return SJsaPath;
        }
        public string[,] CZCL(string[,] zdybSZ, string QXSK, string XZSK, ref string strError)//输入为处理后的指导预报数组，旗县实况，各乡镇的实况.//输出数组行数为旗县乡镇个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
        {

            double d1 = 0, d2 = 0;
            //计算所有旗县与乡镇的个数
            StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            String line;

            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);

            string[,] szQXSK = new string[intQXGS, 5];//将旗县实况字符串转换为数组，每列分别为：站点名称、所属旗县、区站号、最高气温、最低气温(还有最后一列降水量，因为此处不用，故列数为6-1)
            int i = 0;
            for (i = 0; i < intQXGS; i++)
            {
                for (int j = 0; j < 5; j++)//注意该处，如果过去24h降水量没有时出现降水量这组为空时，列数就不是6而是5，此时强行赋值会报错
                {
                    szQXSK[i, j] = (QXSK.Split('\n')[i]).Split('\t')[j];
                }
            }
            int XZGS = 0;
            int lineCount = 0;
            i = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                line = sr.ReadLine();
                if ((2 * i + 1) == lineCount)
                {
                    XZGS += line.Split(',').GetLength(0);
                    i++;
                }
                lineCount++;

            }
            sr.Close();

            string[,] szYB = new string[XZGS, 30];//数组行数为旗县个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7
            //给数组每行第一列赋值：旗县乡镇名称
            lineCount = 0;
            int intLS = 0;
            i = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (i < intQXGS)
            {
                line = sr.ReadLine();
                if ((2 * i + 1) == lineCount)
                {
                    for (int j = 0; j < line.Split(',').Length; j++)
                    {
                        szYB[intLS++, 0] = line.Split(',')[j];
                    }
                    i++;
                }
                lineCount++;

            }
            sr.Close();


            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            lineCount = 0;
            i = 0;
            int intHS = 0; intLS = 1;
            while (i < intQXGS)
            {
                line = sr.ReadLine();
                if ((2 * i + 2) == lineCount)//寻找乡镇名单中的区站号行
                {
                    string strQXID = line.Split(',')[0];
                    double[] szMax = new double[7], szMin = new double[7];//7天旗县指导最高温度-旗县实况最高温度；7天旗县指导最低温度-旗县实况最低温度；
                    string[] strTQ = new string[7], strFXFS = new string[7];//指导预报7天天气与风向风速
                    int intCount1 = 0, intCount2 = 0, intCount3 = 0, intCount4 = 0;
                    for (int j = 1; j < zdybSZ.GetLength(1); j++)//因为指导预报数组和旗县实况已经按照乡镇名单排序，因此不用遍历寻找，只需与乡镇名单用一个序号i即可。遍历每列数据，保存每个时次指导实况差值
                    {
                        if ((j - 1) % 4 == 0)
                        {
                            strTQ[intCount1++] = zdybSZ[i, j];
                        }
                        else if ((j - 2) % 4 == 0)
                        {
                            strFXFS[intCount2++] = zdybSZ[i, j];
                        }
                        else if ((j - 3) % 4 == 0)
                        {
                            szMin[intCount3++] = Math.Round((Convert.ToDouble(zdybSZ[i, j]) - Convert.ToDouble(szQXSK[i, 4])), 1);//旗县实况数组编号为4的列是最低气温
                        }
                        else if ((j - 4) % 4 == 0)
                        {
                            szMax[intCount4++] = Math.Round((Convert.ToDouble(zdybSZ[i, j]) - Convert.ToDouble(szQXSK[i, 3])), 1);//旗县实况数组编号为3的列是最高气温
                        }
                    }
                    for (intLS = 1; intLS < szYB.GetLength(1); intLS++)
                    {
                        szYB[intHS, intLS] = zdybSZ[i, intLS - 1];//从旗县指导预报数组中保存该旗县的预报至整个乡镇精细化预报的数组
                    }
                    int intQXHS = intHS;//保存所属旗县的行数，为了后面做差比较温度差，防止乡镇与旗县温差过大
                    intHS++;
                    for (int j = 1; j < line.Split(',').Length; j++)//遍历该旗县每个乡镇
                    {
                        intCount1 = 0; intCount2 = 0; intCount3 = 0; intCount4 = 0;
                        double douMax = 0, douMin = 0;
                        //寻找该乡镇的最低最高温度
                        for (int k = 0; k < XZSK.Split('\n').Length; k++)
                        {
                            if (XZSK.Split('\n')[k].Contains(line.Split(',')[j]))
                            {
                                try
                                {
                                    douMin = Math.Round(Convert.ToDouble((XZSK.Split('\n')[k]).Split('\t')[4]), 1);//按换行符和制表符分割乡镇实况字符串，每行第5个为最低温，第4个为最高温度
                                }
                                catch (Exception)
                                {
                                    douMin = d1;
                                }
                                try
                                {
                                    douMax = Math.Round(Convert.ToDouble((XZSK.Split('\n')[k]).Split('\t')[3]), 1);
                                }
                                catch
                                {
                                    douMax = d2;
                                }
                                d2 = douMax;
                                d1 = douMin;
                                break;
                            }
                        }
                        for (intLS = 1; intLS < szYB.GetLength(1); intLS++)
                        {
                            if (intLS == 1)
                            {
                                szYB[intHS, intLS] = line.Split(',')[j];
                            }
                            else if ((intLS - 2) % 4 == 0)
                            {
                                szYB[intHS, intLS] = strTQ[intCount1++];
                            }
                            else if ((intLS - 3) % 4 == 0)
                            {
                                szYB[intHS, intLS] = strFXFS[intCount2++];
                            }
                            else if ((intLS - 4) % 4 == 0)
                            {
                                string QXName = "";
                                if (Math.Abs((douMin + szMin[intCount3]) - Convert.ToDouble(szYB[intQXHS, intLS])) >= 5)
                                {
                                    using (StreamReader sr3 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                                    {
                                        string line1;
                                        while ((line1 = sr3.ReadLine()) != null)
                                        {
                                            if (line1.Contains(szYB[intHS, 1]))
                                            {
                                                QXName = line1.Split(',')[0];
                                                break;
                                            }
                                        }
                                    }
                                    // strError += szYB[intHS, 0] + '(' + szYB[intHS, 1] + ')' + ((intCount3 + 1) * 24).ToString() + "小时的最低温度与所属旗县"+QXName+"的最低温度相差5℃以上\r\n";//如果乡镇与旗县温度绝对值相差5度以上警告
                                }
                                szYB[intHS, intLS] = (Math.Round(douMin + szMin[intCount3++])).ToString("f1");
                            }
                            else if (((intLS - 5) % 4 == 0) && intLS != 1)
                            {
                                string QXName = "";
                                if (Math.Abs((douMax + szMax[intCount4]) - Convert.ToDouble(szYB[intQXHS, intLS])) >= 5)
                                {
                                    using (StreamReader sr3 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312")))
                                    {
                                        string line1;
                                        while ((line1 = sr3.ReadLine()) != null)
                                        {
                                            if (line1.Contains(szYB[intHS, 1]))
                                            {
                                                QXName = line1.Split(',')[0];
                                                break;
                                            }
                                        }
                                    }
                                    //strError += szYB[intHS, 0] + '(' + szYB[intHS, 1] + ')' + ((intCount4 + 1) * 24).ToString() + "小时的最高温度与所属旗县"+QXName+ "的最高温度相差5℃以上\r\n";//如果乡镇与旗县温度绝对值相差5度以上警告
                                }
                                szYB[intHS, intLS] = (Math.Round(douMax + szMax[intCount4++])).ToString("f1");
                            }
                        }
                        intHS++;
                    }
                    i++;
                }
                lineCount++;

            }
            sr.Close();
            string QXNameDZ = System.Environment.CurrentDirectory + @"\设置文件\指导预报与产品旗县名称对照.txt";
            using (StreamReader sr1 = new StreamReader(QXNameDZ, Encoding.GetEncoding("GB2312")))
            {
                string strLs = "";
                while ((strLs = sr1.ReadLine()) != null)
                {
                    for (int j = 0; j < szYB.GetLength(0); j++)
                    {
                        if (strLs.Contains(szYB[j, 0]))
                        {
                            szYB[j, 0] = strLs.Split('=')[1];
                        }

                    }
                }
            }
            return szYB;
        }
        public void readXZYBCIMISS()
        {

        }

    }

    public class classHQSK
    {
        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
        public string CIMISSHQQXSK()
        {
            string DZTime = "15";
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
            {
                string line1 = "";

                // 从文件读取数据库配置信息 
                while ((line1 = sr1.ReadLine()) != null)
                {


                    if (line1.Contains("订正市局指导实况时次="))
                    {
                        DZTime = line1.Substring("订正市局指导实况时次=".Length);
                    }
                }
            }
            string strToday = DateTime.UtcNow.ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
                                                          //检索时间段


            paramsqx.Add("times", strToday);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string QXID = "";

            //每两行第一列为旗县ID
            int lineCount = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))
                {
                    QXID += line.Split(',')[0] + ',';
                }
                lineCount++;

            }
            sr.Close();
            QXID = QXID.Substring(0, QXID.Length - 1);

            paramsqx.Add("staIds", QXID);//选择区站号，该处后面调整，从乡镇名单中获取
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C,TEM_Max_24h,TEM_Min_24h,PRE_24h");// 检索要素：站号、站名、最高温度、盟市、旗县
            // 可选参数
            //paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大，最高温降序
            /*   2.4 返回文件的格式 */
            String dataFormat = "tabText";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            string[] SZlinshi = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            strData = "";
            /*删掉CIMISS返回数据第一行的返回信息以及第二行的列标题，只保留数据*/
            for (
                int i = 0; i < SZlinshi.Length; i++)
            {
                if (i > 1)
                {
                    strData += SZlinshi[i] + '\n';
                }
            }
            strData = strData.Substring(0, strData.Length - 1);
            //对旗县实况排序，使得旗县的顺序与旗县名单文件中的一致，便于程序后续处理
            lineCount = 0;
            strLS = strData;
            strData = "";
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))
                {
                    QXID = line.Split(',')[0];
                    SZlinshi = strLS.Split('\n');
                    for (int j = 0; j < SZlinshi.Length; j++)
                    {
                        if (SZlinshi[j].Contains(QXID))//判断该行是否存在该旗县区站号，如果包含，就把整行数据保存
                        {
                            strData += SZlinshi[j] + '\n';
                        }
                    }
                }
                lineCount++;

            }
            sr.Close();
            strData = strData.Substring(0, strData.Length - 1);
            return strData;

        }

        public string CIMISSHQXZSK(string strQXData, ref string strError)//函数输入为旗县CIMISS实况数据，为了当乡镇实况数据缺失时候用旗县数据暂时替换处理；输出为错误提示，提醒缺少的站点；返回为各乡镇的实况，包括数据缺失站点的临时替换数据；
        {
            string strData = "";
            string DZTime = "15";//实况订正的截至时次，一般早于预报时间
            string SKStarTime = "20";//实况订正开始的时次
            using (StreamReader sr1 = new StreamReader(DBconPath, Encoding.GetEncoding("GB2312")))
            {
                string line1 = "";

                // 从文件读取数据库配置信息 
                while ((line1 = sr1.ReadLine()) != null)
                {


                    if (line1.Contains("订正市局指导实况时次="))
                    {
                        DZTime = line1.Substring("订正市局指导实况时次=".Length);
                    }
                    else if (line1.Split('=')[0] == "实况时次")
                    {
                        SKStarTime = line1.Split('=')[1];
                    }
                }
            }
            string strToday = DateTime.UtcNow.ToString("yyyyMMdd") + DZTime + "0000";
            string strLS;
            strLS = DateTime.ParseExact(strToday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strToday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC

            /* 1. 定义client对象 */
            DataQueryClient client = new DataQueryClient();

            /* 2.   调用方法的参数定义，并赋值 */
            /*   2.1 用户名&密码 */
            String userId = "BEHT_BFHT_2131";// 
            String pwd = "YZHHGDJM";// 
            /*   2.2 接口ID */



            String interfaceId1 = "statSurfEleByStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_HOR"); // 资料代码
            //检索时间段
            DateTime dtls = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime();

            /*string strYesterday = dtls.ToString("yyyyMMdd") + SKStarTime + "0000";
            strLS = DateTime.ParseExact(strYesterday, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH:mm:ss");
            strYesterday = Convert.ToDateTime(strLS).ToUniversalTime().ToString("yyyyMMddHH0000");//ToUniversalTime将时间转换为UTC*/
            string strYesterday = Convert.ToDateTime(strLS).AddDays(-1).ToUniversalTime().ToString("yyyyMMddHH0000");
            string timeRange1 = "(" + strYesterday + "," + strToday + "]";
            paramsqx.Add("timeRange", timeRange1);

            /*以下程序功能为：根据设置文件夹下的旗县乡镇设置文件获取CIMISS查询需要配置的台站号*/
            StreamReader sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            String line;
            //读取设置文件的旗县乡镇文件中第一行，确认旗县镇数
            line = sr.ReadLine();
            sr.Close();
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt16(linShi1[1]);
            string XZID = "";
            //每两行第一列为旗县ID 
            int lineCount = 0;
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))//避免取到第一行的旗县个数，因此lineCount>1，站号为编号偶数行故对2取余
                {
                    for (int i = 1; i < line.Split(',').Length; i++)//提取乡镇区站号，编号为0是旗县站号，故从1开始
                    {
                        XZID += line.Split(',')[i] + ',';
                    }

                }
                lineCount++;

            }
            sr.Close();
            XZID = XZID.Substring(0, XZID.Length - 1);

            paramsqx.Add("staIds", XZID);//选择区站号，从乡镇名单中获取
            paramsqx.Add("elements", "Station_Name,Cnty,Station_Id_C");// 检索要素：站号、旗县、区站号
            paramsqx.Add("statEles", "MAX_TEM_Max,MIN_TEM_MIN,SUM_PRE_1h");// 统计要素最高温度的最大值，与最低温度的最小值以及小时降水量
            // 可选参数
            paramsqx.Add("orderby", "Station_ID_C:ASC"); // 排序：按照站号从小到大
            /*   2.4 返回文件的格式 */
            String dataFormat = "tabText";
            StringBuilder retStrXZ = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, retStrXZ);
            // 释放接口服务连接资源
            client.destroyResources();
            strData = Convert.ToString(retStrXZ);

            string[] SZlinshi = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
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
            /*以下程序检查导出的实况数据的站点是否完整*/
            sr = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));
            lineCount = 0;
            while (lineCount < intQXGS * 2 + 1)
            {
                line = sr.ReadLine();
                if ((lineCount > 1) && (lineCount % 2 == 0))
                {
                    for (int i = 1; i < line.Split(',').Length; i++)
                    {
                        if (!strData.Contains(line.Split(',')[i]))//如果导出的实况数据中没有区站号为line.Split(',')[i]的站点
                        {
                            StreamReader sr1 = new StreamReader(configXZPath, Encoding.GetEncoding("GB2312"));//新建一个流，重新遍历乡镇名单文件，找到该乡镇对应的旗县
                            strLS = sr1.ReadToEnd();//整个乡镇名单文本
                            string[] szLS = strLS.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);//名单按行分组
                            strLS = szLS[lineCount - 1];//区站号前一行为站名，因此Linecount-1确认站名，列数与区站号一致，
                            szLS = strLS.Split(',');//该旗县及其乡镇的名称数组
                            strError += szLS[i] + "(" + line.Split(',')[i] + ")的实况数据不存在，实况暂时用其旗县站点" + line.Split(',')[0] + "的实况代替，请及时确认站点信息，设置旗县站点\r\n";
                            string[] szQXData = strQXData.Split('\n');
                            for (int j = 0; j < szQXData.Length; j++)//确认站名后用乡镇的站名代替第一组，后面的内容用其所在旗县的实况代替
                            {
                                if (szQXData[j].Contains(line.Split(',')[0]))
                                {
                                    string[] szLS2 = szQXData[j].Split('\t');//保存对应旗县的实况数组
                                    for (int l = 0; l < szLS2.Length; l++)
                                    {
                                        if (l == 0)
                                            strData += '\n' + szLS[i];
                                        else if (l == 2)
                                            strData += '\t' + line.Split(',')[i];
                                        else
                                            strData += '\t' + szLS2[l];

                                    }


                                    break;
                                }
                            }
                            sr1.Close();
                        }
                    }

                }
                lineCount++;

            }
            sr.Close();

            return strData;

        }


    }



}
