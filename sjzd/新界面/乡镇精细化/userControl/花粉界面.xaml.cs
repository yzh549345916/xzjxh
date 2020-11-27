using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.SplashScreen;

namespace sjzd
{
    /// <summary>
    /// 预报人员选择.xaml 的交互逻辑
    /// </summary>
    public partial class 花粉预报 : RadWindow
    {
        private List<CIMISS.PreYS> mySK = new List<CIMISS.PreYS>();
        private List<防凌预报.YBList> sjyb = new List<防凌预报.YBList>();
        private string skPath = "";
        private SplashScreenDataContext splashScreenDataContext;
        private List<YBList> znwgyBLists = new List<YBList>();
        string _con = "";
        public 花粉预报()
        {
            InitializeComponent();
            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                skPath = util.Read("CPcfig", "HFYB", "Path");
                _con = "Server=" + util.Read("DBcfig", "QXDB", "Server");
                DecryptAndEncryptionHelper helper = new DecryptAndEncryptionHelper(ConfigInformation.Key, ConfigInformation.Vector);
                _con = _con + helper.Decrypto(util.Read("DBcfig", "QXDB", "Database"));
            }
            catch
            {
            }

            qxSelect.SelectedIndex = 0;
        }


        private void RadButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void HfybBtu_Click(object sender, RoutedEventArgs e)
        {
            RadSplashScreenManager.Show();
            string mystr = "";
            try
            {
                if (YdSk.Value != null && qxSelect.SelectedIndex >= 0)
                {
                    string error = 花粉实况入库(DateTime.Now.Date, (double)YdSk.Value, qxSelect.Text);
                    if (error.Length > 0)
                    {
                        mystr = error;
                    }
                    else
                    {
                        mystr = $"{qxSelect.Text}站{DateTime.Now.Date:yyyy年MM月dd日}花粉实况入库成功";
                    }
                }
            }
            catch
            {
            }
            try
            {
                if (TdYb.Value != null && qxSelect.SelectedIndex >= 0)
                {
                    string error = 花粉预报入库(DateTime.Now.Date.AddDays(2), (double)TdYb.Value, qxSelect.Text);
                    if (error.Length > 0)
                    {
                        mystr += "\r\n" + error;
                    }
                    else
                    {
                        mystr += $"\r\n{qxSelect.Text}站{DateTime.Now.Date.AddDays(2):yyyy年MM月dd日}花粉预报入库成功";
                    }
                }

            }
            catch
            {
            }
            try
            {
                double skD = 获取数据库花粉实况(qxSelect.Text, DateTime.Now.Date);
                if (skD >= 0)
                {
                    switchsk.IsChecked = true;
                    sKRk.Value = skD;
                }
                else
                {
                    switchsk.IsChecked = false;
                    sKRk.Value = null;
                }
            }
            catch
            {
            }
            try
            {
                double skD = 获取数据库花粉预报(qxSelect.Text, DateTime.Now.Date.AddDays(2));
                if (skD >= 0)
                {
                    switchyb.IsChecked = true;
                    yBRk.Value = skD;
                }
                else
                {
                    switchyb.IsChecked = false;
                    yBRk.Value = null;
                }
            }
            catch
            {
            }
            RadSplashScreenManager.Close();
            Alert(new DialogParameters
            {
                Content = mystr,
                Owner = Application.Current.MainWindow,
                Header = "注意"
            });
        }

        private double 获取数据库花粉实况(string id, DateTime dateTime)
        {
            double fhDou = -1;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open(); //打开
                    string sql = $"SELECT TOP 1 * FROM [dbo].[花粉浓度预报] WHERE [时间] = '{dateTime:yyyy-MM-dd HH:mm:ss}' and 站号 = '{id}'";
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                fhDou = sqlreader.GetDouble(sqlreader.GetOrdinal("实况"));
                            }
                        }

                    }


                }
            }
            catch (Exception)
            {
                return -1;
            }
            return fhDou;
        }
        private double 获取数据库花粉预报(string id, DateTime dateTime)
        {
            double fhDou = -1;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open(); //打开
                    string sql = $"SELECT TOP 1 * FROM [dbo].[花粉浓度预报] WHERE [时间] = '{dateTime:yyyy-MM-dd HH:mm:ss}' and 站号 = '{id}'";
                    using (SqlCommand sqlman = new SqlCommand(sql, mycon))
                    {
                        using (SqlDataReader sqlreader = sqlman.ExecuteReader())
                        {
                            while (sqlreader.Read())
                            {
                                fhDou = sqlreader.GetDouble(sqlreader.GetOrdinal("预报"));
                            }
                        }

                    }


                }
            }
            catch (Exception)
            {
                return -1;
            }
            return fhDou;
        }

        private double 获取花粉实况(string id)
        {
            double hfsk = -1;
            string myPath = $"{skPath}{id}{DateTime.Now:yyyyMMdd}.txt";
            if (File.Exists(myPath))
            {
                try
                {
                    using (StreamReader sr = new StreamReader(myPath, Encoding.GetEncoding("GB2312"))) //统计旗县个数
                    {
                        string line1 = "";
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            if (line1.Contains('/'))
                            {
                                try
                                {
                                    string[] szls = line1.Split('/');
                                    hfsk = Convert.ToDouble(szls[szls.Length - 1].Trim());
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            return hfsk;
        }

        private void OnConfirmClosed_打开实况文件夹(object sender, WindowClosedEventArgs e)
        {
            if (e.DialogResult == true)
            {
                静态类.OpenBrowser(skPath);
            }
        }

        public string 花粉实况入库(DateTime dateTime, double sk, string id)
        {
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open(); //打开
                    string sql = $"if not exists (select 时间,站号,实况,预报 from 花粉浓度预报 where 站号 = '{id}' and 时间 = '{dateTime:yyyy-MM-dd HH:mm:ss}') INSERT INTO 花粉浓度预报(时间,站号,实况) VALUES('{dateTime:yyyy-MM-dd HH:mm:ss}', '{id}', {sk}) else update 花粉浓度预报 set 实况 = {sk} where 站号 = '{id}' and 时间 = '{dateTime:yyyy-MM-dd HH:mm:ss}'";
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    if (sqlman.ExecuteNonQuery() <= 0)
                    {
                        return $"{id}站{dateTime}花粉实况入库失败";
                    };
                }
            }
            catch (Exception ex)
            {
                return $"{id}站{dateTime}花粉实况入库失败\r\n{ex.Message}";
            }

            return "";
        }
        public string 花粉预报入库(DateTime dateTime, double yb, string id)
        {
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open(); //打开
                    string sql = $"if not exists (select 时间,站号,实况,预报 from 花粉浓度预报 where 站号 = '{id}' and 时间 = '{dateTime:yyyy-MM-dd HH:mm:ss}') INSERT INTO 花粉浓度预报(时间,站号,预报) VALUES('{dateTime:yyyy-MM-dd HH:mm:ss}', '{id}', {yb}) else update 花粉浓度预报 set 预报 = {yb} where 站号 = '{id}' and 时间 = '{dateTime:yyyy-MM-dd HH:mm:ss}'";
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    if (sqlman.ExecuteNonQuery() <= 0)
                    {
                        return $"{id}站{dateTime}花粉预报入库失败";
                    };
                }
            }
            catch (Exception ex)
            {
                return $"{id}站{dateTime}花粉预报入库失败\r\n{ex.Message}";
            }

            return "";
        }
        private void 数据刷新(string id)
        {
            RadSplashScreenManager.Show();
            try
            {
                try
                {
                    YdSk.Value = null;
                    YdPre.Value = null;
                    Rhu.Value = null;
                    tq.Text = "";
                    windSelect.SelectedIndex = -1;
                    TdYb.Value = null;
                    switchsk.IsChecked = false;
                    switchyb.IsChecked = false;
                    sKRk.Value = null;
                    yBRk.Value = null;
                }
                catch
                {
                }
                try
                {
                    double skD = 获取数据库花粉实况(id, DateTime.Now.Date);
                    if (skD >= 0)
                    {
                        switchsk.IsChecked = true;
                        sKRk.Value = skD;
                    }
                    else
                    {
                        switchsk.IsChecked = false;
                        sKRk.Value = null;
                    }
                }
                catch
                {
                }
                try
                {
                    double skD = 获取数据库花粉预报(id, DateTime.Now.Date.AddDays(2));
                    if (skD >= 0)
                    {
                        switchyb.IsChecked = true;
                        yBRk.Value = skD;
                    }
                    else
                    {
                        switchyb.IsChecked = false;
                        yBRk.Value = null;
                    }
                }
                catch
                {
                }
                znwgyBLists.Clear();
                sjyb.Clear();
                try
                {
                    CIMISS cIMISS = new CIMISS();
                    mySK = cIMISS.获取小时降水量(DateTime.Now.Date.AddDays(-1).AddHours(20), DateTime.Now, "53463,53466");
                    YdPre.Value = Math.Round(mySK.Where(y => y.StationID == id).Sum(y => y.Pre), 2);
                }
                catch
                {
                }

                double hfsk = 获取花粉实况(id);


                if (hfsk < 0)
                {
                    try
                    {
                        hfsk = 获取数据库花粉实况(qxSelect.Text, DateTime.Now.Date);
                        if (hfsk >= 0)
                        {
                            YdSk.Value = hfsk;
                        }
                        else
                        {
                            RadSplashScreenManager.Close();
                            Confirm(new DialogParameters
                            {
                                Content = $"{id}站昨日（{DateTime.Now:yyyy年MM月dd日}）花粉实况获取异常\r\n是否打开花粉实况所在文件夹",
                                Closed = OnConfirmClosed_打开实况文件夹,
                                Owner = Application.Current.MainWindow,
                                CancelButtonContent = "否",
                                Header = "警告",
                                OkButtonContent = "是"
                            });
                            RadSplashScreenManager.Show();
                        }
                    }
                    catch
                    {
                    }

                }
                else
                {
                    YdSk.Value = hfsk;
                }

                try
                {
                    znwgyBLists = 获取国家智能网格(20);
                    List<YBList> myLists = znwgyBLists.Where(y => y.SX <= 12 && y.ID == id).ToList();
                    if (myLists.Count > 0)
                    {
                        double maxDouble = 0;
                        foreach (var item in myLists)
                        {
                            if (maxDouble < item.ERH)
                                maxDouble = item.ERH;
                        }

                        Rhu.Value = maxDouble;
                    }
                }
                catch
                {
                }

                try
                {
                    报文读取 bw2 = new 报文读取();
                    string strDate = DateTime.Now.ToString("yyyyMMdd");
                    short sc2 = 20;
                    防凌预报 flyb = new 防凌预报();
                    string ybData = bw2.Sjyb(strDate, sc2.ToString().PadLeft(2, '0'));
                    sjyb = flyb.CLYBAll(ybData).Where(y => y.ID == id && y.sc == 24).OrderBy(y => y.序号).ThenBy(y => y.ID).ThenBy(y => y.sc).ToList();
                }
                catch
                {
                }

                if (sjyb.Count > 0)
                {
                    try
                    {
                        string[] szls = Regex.Split(sjyb[0].TQ, "转", RegexOptions.IgnoreCase);
                        tq.Text = szls[szls.Length - 1];
                    }
                    catch
                    {
                    }

                    try
                    {
                        string[] szls = Regex.Split(sjyb[0].FS, "转", RegexOptions.IgnoreCase);
                        string fsls = szls[szls.Length - 1];
                        if (fsls.Contains("2、3级"))
                            windSelect.SelectedIndex = 0;
                        else if (fsls.Contains("3、4级"))
                            windSelect.SelectedIndex = 1;
                        else
                            windSelect.SelectedIndex = 2;
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            RadSplashScreenManager.Close();
        }

        public string GetTQ(double ect, double pre, int pph)
        {
            string tq = "";
            if (pre > 0)
            {
                if (pph == 3)
                {
                    if (pre < 1)
                        tq = "小雪";
                    else if (pre >= 1 && pre < 3)
                        tq = "中雪";
                    else if (pre >= 3 && pre < 6)
                        tq = "大雪";
                    else if (pre >= 6 && pre < 12)
                        tq = "暴雪";
                    else if (pre >= 12 && pre < 24)
                        tq = "大暴雪";
                    else if (pre >= 24)
                        tq = "特大暴雪";
                }
                else if (pph == 2)
                {
                    tq = "雨夹雪";
                }
                else if (pph == 4)
                {
                    tq = "冻雨";
                }
                else
                {
                    if (pre < 5)
                        tq = "小雨";
                    else if (pre >= 5 && pre < 15)
                        tq = "中雨";
                    else if (pre >= 15 && pre < 30)
                        tq = "大雨";
                    else if (pre >= 30 && pre < 70)
                        tq = "暴雨";
                    else if (pre >= 70 && pre < 140)
                        tq = "大暴雨";
                    else if (pre >= 140)
                        tq = "特大暴雨";
                }
            }
            else
            {
                if (ect <= 20)
                    tq = "晴";
                else if (ect >= 90)
                    tq = "阴";
                else
                    tq = "多云";
            }

            return tq;
        }


        public List<YBList> 获取国家智能网格(int sc)
        {
            List<YBList> list = new List<YBList>();
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\智能网格设置.xml");
                string con = util.Read("OtherConfig", "DB");
                List<IDName> iDNames = new List<IDName>();
                iDNames.Add(new IDName
                {
                    ID = "53466",
                    Name = "赛罕区",
                    LB = 1,
                    GJID = "53466"
                });
                iDNames.Add(new IDName
                {
                    ID = "53463",
                    Name = "呼市市区",
                    LB = 1,
                    GJID = "53463"
                });

                using (SqlConnection mycon = new SqlConnection(con))
                {
                    //预报取上一时次的预报

                    mycon.Open(); //打开
                    string sql = "";
                    if (sc == 8)
                    {
                        sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=8 and sx in (12,15,18,21,24,27,30,33,36) and date='{0:yyyy-MM-dd}'", DateTime.Now);
                    }
                    else
                    {
                        sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=20 and sx in (12,15,18,21,24,27,30,33,36) and date='{0:yyyy-MM-dd}'", DateTime.Now);
                    }

                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    if (sqlreader.HasRows)
                    {
                        double pre = 0, ect = 0;
                        int pph = 0;
                        string fxfs = "";
                        double wiu = 0, wiv = 0;
                        while (sqlreader.Read())
                        {
                            try
                            {
                                try
                                {
                                    pre = sqlreader.IsDBNull(sqlreader.GetOrdinal("PRE_3H")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                    ect = sqlreader.IsDBNull(sqlreader.GetOrdinal("ECT")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                    pph = sqlreader.IsDBNull(sqlreader.GetOrdinal("PPH")) ? 0 : Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                    wiu = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIU10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2);
                                    wiv = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIV10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2);
                                    fxfs = GetFXFS(wiv, wiu);
                                    List<IDName> ll = iDNames.FindAll(y => y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID"))).ToList();
                                    for (int j = 0; j < ll.Count; j++)
                                    {
                                        list.Add(new YBList
                                        {
                                            Name = ll[j].Name,
                                            ID = ll[j].ID,
                                            TEM = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                            TQ = GetTQ(ect, pre, pph),
                                            SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 12),
                                            LB = ll[j].LB,
                                            PRE_3H = pre,
                                            ERH = sqlreader.IsDBNull(sqlreader.GetOrdinal("ERH")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                            VIS = sqlreader.IsDBNull(sqlreader.GetOrdinal("VIS")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("VIS")), 2),
                                            WIU10 = wiu,
                                            WIV10 = wiv,
                                            FXFS = fxfs,
                                            FX = fxfs.Split(',')[0],
                                            FS = fxfs.Split(',')[1]
                                        });
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                            catch (Exception)
                            {
                                list.Clear();
                                //预报取上一时次的预报

                                sqlreader.Close();
                                if (sc == 8)
                                {
                                    sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=20 and sx in (24,27,30,33,36,39,42,45,48) and date='{0:yyyy-MM-dd}'", DateTime.Now.AddDays(-1));
                                }
                                else
                                {
                                    sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=8 and sx in (24,27,30,33,36,39,42,45,48) and date='{0:yyyy-MM-dd}'", DateTime.Now);
                                }

                                sqlman = new SqlCommand(sql, mycon);
                                sqlreader = sqlman.ExecuteReader();
                                if (sqlreader.HasRows)
                                {
                                    while (sqlreader.Read())
                                    {
                                        try
                                        {
                                            pre = sqlreader.IsDBNull(sqlreader.GetOrdinal("PRE_3H")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                            ect = sqlreader.IsDBNull(sqlreader.GetOrdinal("ECT")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                            pph = sqlreader.IsDBNull(sqlreader.GetOrdinal("PPH")) ? 0 : Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                            List<IDName> ll = iDNames.FindAll(y => y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID"))).ToList();
                                            for (int j = 0; j < ll.Count; j++)
                                            {
                                                list.Add(new YBList
                                                {
                                                    Name = ll[j].Name,
                                                    ID = ll[j].ID,
                                                    TEM = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                                    TQ = GetTQ(ect, pre, pph),
                                                    SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                                    LB = ll[j].LB,
                                                    PRE_3H = pre,
                                                    ERH = sqlreader.IsDBNull(sqlreader.GetOrdinal("ERH")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                                    VIS = sqlreader.IsDBNull(sqlreader.GetOrdinal("VIS")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("VIS")), 2),
                                                    WIU10 = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIU10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2),
                                                    WIV10 = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIV10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2)
                                                });
                                            }
                                        }
                                        catch (Exception)
                                        {
                                        }
                                    }
                                }

                                return list;
                            }
                        }
                    }
                    else
                    {
                        sqlreader.Close();
                        double pre = 0, ect = 0;
                        int pph = 0;
                        if (sc == 8)
                        {
                            sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=20 and sx in (24,27,30,33,36,39,42,45,48) and date='{0:yyyy-MM-dd}'", DateTime.Now.AddDays(-1));
                        }
                        else
                        {
                            sql = string.Format("select * from 全国智能网格预报服务产品3h240 where StatioID in ('53466','53463') and sc=8 and sx in (24,27,30,33,36,39,42,45,48) and date='{0:yyyy-MM-dd}'", DateTime.Now);
                        }

                        sqlman = new SqlCommand(sql, mycon);
                        sqlreader = sqlman.ExecuteReader();
                        if (sqlreader.HasRows)
                        {
                            while (sqlreader.Read())
                            {
                                try
                                {
                                    pre = sqlreader.IsDBNull(sqlreader.GetOrdinal("PRE_3H")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("PRE_3H")), 2);
                                    ect = sqlreader.IsDBNull(sqlreader.GetOrdinal("ECT")) ? 0 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ECT")), 2);
                                    pph = sqlreader.IsDBNull(sqlreader.GetOrdinal("PPH")) ? 0 : Convert.ToInt32(sqlreader.GetFloat(sqlreader.GetOrdinal("PPH")));
                                    List<IDName> ll = iDNames.FindAll(y => y.GJID == sqlreader.GetString(sqlreader.GetOrdinal("StatioID"))).ToList();
                                    for (int j = 0; j < ll.Count; j++)
                                    {
                                        list.Add(new YBList
                                        {
                                            Name = ll[j].Name,
                                            ID = ll[j].ID,
                                            TEM = sqlreader.IsDBNull(sqlreader.GetOrdinal("TEM")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("TEM")), 2),
                                            TQ = GetTQ(ect, pre, pph),
                                            SX = Convert.ToInt16(sqlreader.GetInt16(sqlreader.GetOrdinal("SX")) - 24),
                                            LB = ll[j].LB,
                                            PRE_3H = pre,
                                            ERH = sqlreader.IsDBNull(sqlreader.GetOrdinal("ERH")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("ERH")), 2),
                                            VIS = sqlreader.IsDBNull(sqlreader.GetOrdinal("VIS")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("VIS")), 2),
                                            WIU10 = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIU10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIU10")), 2),
                                            WIV10 = sqlreader.IsDBNull(sqlreader.GetOrdinal("WIV10")) ? -999999 : Math.Round(sqlreader.GetFloat(sqlreader.GetOrdinal("WIV10")), 2)
                                        });
                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            return list;
        }

        public string GetFXFS(double v, double u)
        {
            string fxfs = "";
            double fx = 999.9; //风向

            if ((u > 0) & (v > 0))
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u < 0) & (v > 0))
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u < 0) & (v < 0))
            {
                fx = 90 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u > 0) & (v < 0))
            {
                fx = 270 - Math.Atan(v / u) * 180 / Math.PI;
            }
            else if ((u == 0) & (v > 0))
            {
                fx = 180;
            }
            else if ((u == 0) & (v < 0))
            {
                fx = 0;
            }
            else if ((u > 0) & (v == 0))
            {
                fx = 270;
            }
            else if ((u < 0) & (v == 0))
            {
                fx = 90;
            }
            else if ((u == 0) & (v == 0))
            {
                fx = 999.9;
            }

            //风速是uv分量的平方和

            double fs = Math.Sqrt(Math.Pow(u, 2) + Math.Pow(v, 2));
            int intfx = Convert.ToInt32(Math.Round(fx / 45, 0));
            switch (intfx)
            {
                case 0:
                    fxfs = "北风";
                    break;
                case 1:
                    fxfs = "东北风";
                    break;
                case 2:
                    fxfs = "东风";
                    break;
                case 3:
                    fxfs = "东南风";
                    break;
                case 4:
                    fxfs = "南风";
                    break;
                case 5:
                    fxfs = "西南风";
                    break;
                case 6:
                    fxfs = "西风";
                    break;
                case 7:
                    fxfs = "西北风";
                    break;
                case 999017:
                    fxfs = "静风";
                    break;
                default:
                    fxfs = "北风";
                    break;
            }

            fxfs += ',';
            if (fs >= 0 && fs <= 0.2)
            {
                fxfs += "0级";
            }
            else if (fs >= 0.3 && fs <= 1.5)
            {
                fxfs += "1级";
            }
            else if (fs >= 1.6 && fs <= 3.3)
            {
                fxfs += "2级";
            }
            else if (fs >= 3.4 && fs <= 5.4)
            {
                fxfs += "3级";
            }
            else if (fs >= 5.5 && fs <= 7.9)
            {
                fxfs += "4级";
            }
            else if (fs >= 8 && fs <= 10.7)
            {
                fxfs += "5级";
            }
            else if (fs >= 10.8 && fs <= 13.8)
            {
                fxfs += "6级";
            }
            else if (fs >= 13.9 && fs <= 17.1)
            {
                fxfs += "7级";
            }
            else if (fs >= 17.2 && fs <= 20.7)
            {
                fxfs += "8级";
            }
            else if (fs >= 20.8 && fs <= 24.4)
            {
                fxfs += "9级";
            }
            else if (fs >= 24.5 && fs <= 28.4)
            {
                fxfs += "10级";
            }
            else if (fs >= 28.5 && fs <= 32.6)
            {
                fxfs += "11级";
            }
            else if (fs >= 32.7 && fs <= 36.9)
            {
                fxfs += "12级";
            }
            else if (fs >= 37 && fs <= 41.4)
            {
                fxfs += "13级";
            }
            else if (fs >= 41.5 && fs <= 46.1)
            {
                fxfs += "14级";
            }
            else if (fs >= 46.2 && fs <= 50.9)
            {
                fxfs += "15级";
            }
            else if (fs >= 51 && fs <= 56)
            {
                fxfs += "16级";
            }
            else if (fs >= 56.1 && fs <= 61.2)
            {
                fxfs += "17级";
            }
            else if (fs >= 61.3)
            {
                fxfs += "17级以上";
            }
            else
            {
                fxfs += "3级";
            }

            return fxfs;
        }

        private void Rhu_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            花粉预报计算();
        }

        private void tq_TextChanged(object sender, TextChangedEventArgs e)
        {
            花粉预报计算();
        }

        private void windSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            花粉预报计算();
        }

        public void 花粉预报计算()
        {
            string strFH = "";
            if (Rhu.Value != null && tq.Text != "" && windSelect != null && YdSk.Value != null)
            {
                double xs = 0;
                if (tq.Text.Contains("小雨") || tq.Text.Contains("雨夹雪"))
                {
                    xs = -0.2;
                }
                else if (tq.Text.Contains("中雨"))
                {
                    xs = -0.35;
                }
                else if (tq.Text.Contains("大雨"))
                {
                    xs = -0.5;
                }
                //else if (tq.Text.Contains("晴"))
                //{
                //    xs = 0.1;
                //}
                //else if (tq.Text.Contains("多云") || tq.Text.Contains("阴"))
                //{
                //    xs = -0.1;
                //}

                strFH += $"天气为{tq.Text}系数为{xs};";
                try
                {
                    if (YdPre.Value < 0.1 || YdPre.Value == null || YdPre.Value > 9999)
                    {
                        xs += 0;
                        strFH += $"昨天20至今天{DateTime.Now.Hour}时降水量为0，系数为0;";
                    }
                    else if (YdPre.Value > 0 && YdPre.Value < 10)
                    {
                        xs += -0.2;
                        strFH += $"昨天20至今天{DateTime.Now.Hour}时降水量为{YdPre.Value}，降水强度为小雨，系数为-0.2;";
                    }
                    else if (YdPre.Value >= 10 && YdPre.Value < 25)
                    {
                        xs += -0.35;
                        strFH += $"昨天20至今天{DateTime.Now.Hour}时降水量为{YdPre.Value}，降水强度为中雨，系数为-0.35;";
                    }
                    else
                    {
                        xs += -0.5;
                        strFH += $"昨天20至今天{DateTime.Now.Hour}时降水量为{YdPre.Value}，降水强度为大雨及以上，系数为-0.5;";
                    }
                }
                catch
                {
                }

                if (windSelect.SelectedIndex == 0)
                {
                    xs += 0;
                    strFH += $"风速为{windSelect.Text}，系数为0;";
                }
                else if (windSelect.SelectedIndex == 1)
                {
                    xs += 0.15;
                    strFH += $"风速为{windSelect.Text}，系数为0.15;";
                }
                else
                {
                    xs += -0.3;
                    strFH += $"风速为{windSelect.Text}，系数为-0.3;";
                }

                if (Rhu.Value < 50)
                {
                    xs += 0.1;
                    strFH += "相对湿度< 50，系数为0.1;";
                }
                else if (Rhu.Value >= 50 && Rhu.Value <= 70)
                {
                    xs += 0;
                    strFH += "50<=相对湿度<= 70，系数为0。";
                }
                else if (Rhu.Value > 70)
                {
                    xs += -0.1;
                    strFH += "相对湿度>70，系数为-0.1。";
                }

                TdYb.Value = (0.82 * YdSk.Value + 14.56) * (1 + xs);
                strFH += $"最后计算公式为：(0.82 * {YdSk.Value} + 14.56) * (1 + {xs})={TdYb.Value}";
                Shuoming.Text = strFH;
            }
            else
            {
                if (Shuoming != null)
                {
                    Shuoming.Text = "";
                }
            }
        }

        private void qxSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                数据刷新(qxSelect.Text);
            }
            catch
            {
            }
        }

        public class YBList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public double TEM { get; set; }
            public double PRE_3H { get; set; }
            public double VIS { get; set; }
            public double ERH { get; set; }
            public double WIU10 { get; set; }
            public double WIV10 { get; set; }
            public string TQ { get; set; }
            public short LB { get; set; }
            public short SX { get; set; }
            public string FX { get; set; }
            public string FS { get; set; }
            public string FXFS { get; set; }
        }

        public class IDName
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public short LB { get; set; }
            public string GJID { get; set; }
        }
    }
}