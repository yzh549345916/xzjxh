using Aspose.Words;
using Aspose.Words.Tables;
using cma.cimiss.client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace sjzd
{
    class 生态与农业气象信息
    {
        string cimissUserid = "", cimissPassword = "";
        public void DCWord(int QS,int allQS, Int16 year, Int16 month, Int16 xun, string ybName)
        {
            string xunStr = "上旬";
            if(xun==2)
            {
                xunStr = "中旬";
            }
            else if(xun==3)
            {
                xunStr = "下旬";
            }
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\生态与农业气象信息设置.xml");
            string fbPath = util.Read("Path", "FB")+ year+ "年生态与农业气象信息\\";
            string xybPath = util.Read("Path", "XYB");
            cimissUserid= util.Read("CIMISS", "ID");
            cimissPassword = util.Read("CIMISS", "Password");
            string ysExcelPath = fbPath + year + "气象要素\\"+year + "气象要素.xls";
            fbPath = fbPath + string.Format("呼和浩特DQ1（{0}）{1}月{2}生态与农业气象信息.doc",allQS,month, xunStr);
            string stationID= util.Read("StationConfig");
            if (xybPath.Trim().Length > 0&& fbPath.Trim().Length>0)
            {
                List<YBList> dqData= ZRtoXun(year, month, xun, stationID);
                List<YBList> lastYearData = ZRtoXun((short)(year - 1), month, xun, stationID);
                List<YBList> tqData = getTQSJ(year, month, xun, stationID);
                if(dqData.Count==0)
                {
                    MessageBox.Show("本旬数据获取失败");
                }
                else if (lastYearData.Count == 0)
                {
                    MessageBox.Show("去年同期数据获取失败");
                }
                else if (tqData.Count == 0)
                {
                    MessageBox.Show("历史同期数据获取失败");
                }

                else
                {
                    string MBPath = Environment.CurrentDirectory + @"\模版\生态与农业气象信息模板.doc";
                    if(File.Exists(MBPath))
                    {
                        Document doc = new Document(MBPath);
                        DocumentBuilder builder = new DocumentBuilder(doc);
                        builder.MoveToBookmark("期数");
                        builder.Write(QS.ToString());
                        builder.MoveToBookmark("总期数");
                        builder.Write(allQS.ToString());
                        builder.MoveToBookmark("分析");
                        builder.Write(ybName);
                        builder.MoveToBookmark("日期");
                        builder.Write(DateTime.Now.ToString("yyyy年M月d"));
                        string timeStr = month.ToString() + "月上旬";
                        if(xun==2)
                        {
                            timeStr = month.ToString() + "月中旬";
                        }
                        else if(xun==3)
                        {
                            timeStr = month.ToString() + "月下旬";
                        }
                        builder.MoveToBookmark("标题日期");
                        builder.Write(timeStr);
                        builder.MoveToBookmark("图1时间");
                        builder.Write(timeStr);
                        builder.MoveToBookmark("图2时间");
                        builder.Write(timeStr);
                        List<YBList> dqLS= dqData.OrderByDescending(y => y.Tem).ToList();
                        string strLS = $"{timeStr}我市平均气温{dqLS[dqLS.Count-1].Tem}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Tem}℃（{dqLS[0].Name}），";
                        List<YBList> tqbjData = new List<YBList>();
                        foreach (YBList yB in dqData)
                        {
                            YBList bList= tqData.Find(y => y.ID == yB.ID);
                            tqbjData.Add(new YBList()
                            {
                                ID=yB.ID,
                                Name=yB.Name,
                                Tem=yB.Tem-bList.Tem,
                                Pre=Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre,0),
                                Sun = yB.Sun - bList.Sun,
                            });
                        }
                        List<YBList> qnbjData = new List<YBList>();
                        foreach (YBList yB in dqData)
                        {
                            YBList bList = lastYearData.Find(y => y.ID == yB.ID);
                            qnbjData.Add(new YBList()
                            {
                                ID = yB.ID,
                                Name = yB.Name,
                                Tem = yB.Tem - bList.Tem,
                                Pre = Math.Round((yB.Pre - bList.Pre) , 1),
                                Sun = yB.Sun - bList.Sun,
                            });
                        }
                        try
                        {
                            if (!tqbjData.Exists(y => y.Tem > 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderByDescending(y => y.Tem).ToList();
                                strLS += $"较历年同期偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                            }
                            else if (!tqbjData.Exists(y => y.Tem < 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderBy(y => y.Tem).ToList();
                                strLS += $"较历年同期偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                            }
                            else
                            {
                                List<YBList> list1 = tqbjData.FindAll(y => y.Tem < 0);
                                List<YBList> list2 = tqbjData.FindAll(y => y.Tem > 0);
                                List<YBList> list3 = tqbjData.FindAll(y => y.Tem == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Tem).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏高{yB.Tem.ToString("F1")}℃，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Tem).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏低{(yB.Tem * -1).ToString("F1")}℃，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                            }
                            if (!qnbjData.Exists(y => y.Tem > 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderByDescending(y => y.Tem).ToList();
                                strLS += $"较去年同期偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else if (!qnbjData.Exists(y => y.Tem < 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderBy(y => y.Tem).ToList();
                                strLS += $"较去年同期偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else
                            {
                                List<YBList> list1 = qnbjData.FindAll(y => y.Tem < 0);
                                List<YBList> list2 = qnbjData.FindAll(y => y.Tem > 0);
                                List<YBList> list3 = qnbjData.FindAll(y => y.Tem == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Tem).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏高{yB.Tem.ToString("F1")}℃，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Tem).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏低{(yB.Tem * -1).ToString("F1")}℃，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                            }
                            if (month > 3 && month < 9)//4-8月高温
                            {
                                dqLS.Clear();
                                dqLS = dqData.OrderBy(y => y.TmaxDays).ToList();
                                Int16 count = 1;
                                for(int i=1;i<dqLS.Count;i++)
                                {
                                    if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                        count++;
                                }
                                if(count==1)
                                {
                                    dqLS.Clear();
                                    dqLS = dqData.OrderBy(y => y.Tmax).ToList();
                                    strLS += $"旬内日极端最高气温{dqLS[0].Tmax.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmax.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），均出现在{dqLS[0].TmaxDays}日。";
                                }
                                else
                                {
                                    string strls2 =  dqLS[0].Name + '、';
                                    int countls = 0;
                                    for (int i = 1; i < dqLS.Count; i++)
                                    {
                                        if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                        {
                                            strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[i - 1].TmaxDays}日，";
                                            countls++;
                                            if(countls==count-1)
                                            {
                                                strls2 += $"其余地区出现在{dqLS[i].TmaxDays}日。";
                                                break;
                                            }
                                            strls2 += $"{ dqLS[i].Name}、";
                                        }
                                        else
                                        {
                                            strls2 += dqLS[i].Name + '、';
                                        }
                                    }
                                    //strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[dqLS.Count - 1].TmaxDays}日。";
                                    dqLS.Clear();
                                    dqLS = dqData.OrderBy(y => y.Tmax).ToList();
                                    strLS += $"旬内日极端最高气温{dqLS[0].Tmax.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmax.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），{strls2}";
                                }
                            }
                            else
                            {
                                dqLS.Clear();
                                dqLS = dqData.OrderBy(y => y.TminDays).ToList();
                                Int16 count = 1;
                                for (int i = 1; i < dqLS.Count; i++)
                                {
                                    if (dqLS[i].TminDays != dqLS[i - 1].TminDays)
                                        count++;
                                }
                                if (count == 1)
                                {
                                    dqLS.Clear();
                                    dqLS = dqData.OrderBy(y => y.Tmin).ToList();
                                    strLS += $"旬内日极端最低气温{dqLS[0].Tmin.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmin.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），均出现在{dqLS[0].TminDays}日。";
                                }
                                else
                                {
                                    string strls2 = dqLS[0].Name + '、';
                                    int countls = 0;
                                    for (int i = 1; i < dqLS.Count; i++)
                                    {
                                        if (dqLS[i].TminDays != dqLS[i - 1].TminDays)
                                        {
                                            strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[i - 1].TminDays}日，";
                                            countls++;
                                            if (countls == count - 1)
                                            {
                                                strls2 += $"其余地区出现在{dqLS[i].TminDays}日。";
                                                break;
                                            }
                                            strls2 += $"{ dqLS[i].Name}、";
                                        }
                                        else
                                        {
                                            strls2 += dqLS[i].Name + '、';
                                        }
                                    }
                                    //strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[dqLS.Count - 1].TminDays}日。";
                                    dqLS.Clear();
                                    dqLS = dqData.OrderBy(y => y.Tmin).ToList();
                                    strLS += $"旬内日极端最低气温{dqLS[0].Tmin.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmin.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），{strls2}";
                                }
                            }

                            builder.MoveToBookmark("气温");
                            builder.Write(strLS);
                        }
                        catch
                        {
                        }
                        try
                        {
                            strLS = timeStr;
                            dqLS.Clear();
                            dqLS = dqData.OrderBy(y => y.Pre).ToList();
                            if (dqLS[0].Pre==0)
                            {
                                if(dqLS[dqLS.Count - 1].Pre == 0)
                                {
                                    strLS +=   $"我市无降水。";
                                }
                                else
                                {
                                    strLS += "我市降水";
                                    foreach (YBList yB in dqLS)
                                    {
                                        if(yB.Pre>0)
                                        {
                                            strLS += $"{yB.Name}{yB.Pre}mm，";
                                        }
                                    }
                                    strLS += "其余地区无降水。";
                                }
                            }
                            else
                            {
                                foreach (YBList yB in dqLS)
                                {
                                    if (yB.Pre > 0)
                                    {
                                        strLS += $"{yB.Name}{yB.Pre}mm，";
                                    }
                                }
                                strLS = strLS.Substring(0, strLS.Length - 1) + '。';
                            }
                            //strLS += "与历年同期相比，";
                            if (!tqbjData.Exists(y => y.Pre > 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderByDescending(y => y.Pre).ToList();
                                strLS += $"与历年同期相比偏少{(dqLS[0].Pre * -1).ToString("F0")}%（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                            }
                            else if (!tqbjData.Exists(y => y.Pre < 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderBy(y => y.Pre).ToList();
                                strLS += $"与历年同期相比偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                            }
                            else
                            {
                                List<YBList> list1 = tqbjData.FindAll(y => y.Pre < 0);
                                List<YBList> list2 = tqbjData.FindAll(y => y.Pre > 0);
                                List<YBList> list3 = tqbjData.FindAll(y => y.Pre == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Pre).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏多{yB.Pre.ToString("F0")}%，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏少{(dqLS[0].Pre * -1).ToString("F0")}%（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Pre).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏少{(yB.Pre * -1).ToString("F0")}%，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                }
                            }
                            if (!qnbjData.Exists(y => y.Pre > 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderByDescending(y => y.Pre).ToList();
                                strLS += $"较去年同期偏少{(dqLS[0].Pre * -1).ToString("F1")}mm（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else if (!qnbjData.Exists(y => y.Pre < 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderBy(y => y.Pre).ToList();
                                strLS += $"较去年同期偏多{dqLS[0].Pre.ToString("F1")}mm（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else
                            {
                                List<YBList> list1 = qnbjData.FindAll(y => y.Pre < 0);
                                List<YBList> list2 = qnbjData.FindAll(y => y.Pre > 0);
                                List<YBList> list3 = qnbjData.FindAll(y => y.Pre == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Pre).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏多{yB.Pre.ToString("F1")}mm，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏少{(dqLS[0].Pre * -1).ToString("F1")}mm（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Pre).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏少{(yB.Pre * -1).ToString("F1")}mm，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏多{dqLS[0].Pre.ToString("F1")}mm（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                            }
                            builder.MoveToBookmark("降水");
                            builder.Write(strLS);
                        }
                        catch
                        {
                        }
                        try
                        {
                            dqLS.Clear();
                            dqLS = dqData.OrderByDescending(y => y.Sun).ToList();
                            strLS = $"{timeStr}我市日照时数{dqLS[dqLS.Count - 1].Sun.ToString("F1")}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Sun.ToString("F1")}小时（{dqLS[0].Name}）（图3）。";
                            if (!tqbjData.Exists(y => y.Sun > 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderByDescending(y => y.Sun).ToList();
                                strLS += $"较历年同期偏多{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else if (!tqbjData.Exists(y => y.Sun < 0))
                            {
                                dqLS.Clear();
                                dqLS = tqbjData.OrderBy(y => y.Sun).ToList();
                                strLS += $"较历年同期偏少{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                            }
                            else
                            {
                                List<YBList> list1 = tqbjData.FindAll(y => y.Sun < 0);
                                List<YBList> list2 = tqbjData.FindAll(y => y.Sun > 0);
                                List<YBList> list3 = tqbjData.FindAll(y => y.Sun == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Sun).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏多{yB.Sun.ToString("F1")}小时，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Sun).ToList();
                                    strLS += $"与历年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏少{(yB.Sun * -1).ToString("F1")}小时，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                    }
                                    strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                            }
                            if (!qnbjData.Exists(y => y.Sun > 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderByDescending(y => y.Sun).ToList();
                                strLS += $"较去年同期偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                            }
                            else if (!qnbjData.Exists(y => y.Sun < 0))
                            {
                                dqLS.Clear();
                                dqLS = qnbjData.OrderBy(y => y.Sun).ToList();
                                strLS += $"较去年同期偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                            }
                            else
                            {
                                List<YBList> list1 = qnbjData.FindAll(y => y.Sun < 0);
                                List<YBList> list2 = qnbjData.FindAll(y => y.Sun > 0);
                                List<YBList> list3 = qnbjData.FindAll(y => y.Sun == 0);
                                if (list1.Count > list2.Count)
                                {
                                    dqLS.Clear();
                                    dqLS = list1.OrderByDescending(y => y.Sun).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list2)
                                    {
                                        strLS += $"{yB.Name}偏多{yB.Sun.ToString("F1")}小时，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = list2.OrderBy(y => y.Sun).ToList();
                                    strLS += $"与去年同期相比，";
                                    foreach (YBList yB in list1)
                                    {
                                        strLS += $"{yB.Name}偏少{(yB.Sun * -1).ToString("F1")}小时，";
                                    }
                                    if (list3.Count > 0)
                                    {
                                        foreach (YBList yB in list3)
                                        {
                                            strLS += yB.Name + '、';
                                        }
                                        strLS += strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                    }
                                    strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                }
                            }


                            builder.MoveToBookmark("日照");
                            builder.Write(strLS);
                        }
                        catch
                        {
                        }
                        string SJsaPath = Environment.CurrentDirectory + @"\模版\测试.doc";
                        doc.Save(SJsaPath);
                        MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + SJsaPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                        if (dr == MessageBoxResult.Yes)
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(SJsaPath);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("word模板加载失败，请确认文件：" + MBPath + "\n是否存在");
                    }
                    
                }
            }
            else
            {
                MessageBox.Show("路径设置错误");
            }


        }
        public List<YBList> getDQSJ(Int16 year, Int16 month, Int16 xun,string stationID)
        {
            List<YBList> yBLists = new List<YBList>();
            DataQueryClient client = new DataQueryClient();
            string userId = cimissUserid;
            string pwd = cimissPassword;
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_TEN"); // 资料代码
            //检索时间段
            paramsqx.Add("times",year+month.ToString().PadLeft(2,'0')+ xun.ToString().PadLeft(2, '0')+ "000000");
            paramsqx.Add("elements", "Station_Name,Station_Id_C,TEM_Avg,TEM_Max,TEM_Max_ODay_C,TEM_Min,TEM_Min_ODay_C,PRE_Time_2020,SSH");
            paramsqx.Add("staIds", stationID);
            string dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            try
            {
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    strData = strData.Replace("\r\n", "\n");
                    string[] szData = strData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if(szData.Length>2)
                    {
                        for(int i=2;i<szData.Length;i++)
                        {
                            try
                            {
                                
                                string[] szls = szData[i].Split();
                                yBLists.Add(new YBList()
                                {
                                    Name = szls[0],
                                    ID = szls[1],
                                    Tem = Convert.ToDouble(szls[2]),
                                    Tmax = Convert.ToDouble(szls[3]),
                                    //TmaxDays = szls[4],
                                    Tmin = Convert.ToDouble(szls[5]),
                                    //TminDays = szls[6],
                                    Pre = Convert.ToDouble(szls[7]) > 1000 ? 0 : Convert.ToDouble(szls[7]),
                                    Sun = Convert.ToDouble(szls[8]) > 1000 ? 0 : Convert.ToDouble(szls[8]),
                                    Wind = getDF(year, month, xun, szls[1])
                                });
                            }
                            catch(Exception ex)
                            { }
                        }
                    }
                }

            }
            catch
            {
                
            }
            return yBLists;

        }
        public List<YBList> getTQSJ(Int16 year, Int16 month, Int16 xun, string stationID)
        {
            List<YBList> yBLists = new List<YBList>();
            DataQueryClient client = new DataQueryClient();
            string userId = cimissUserid;
            string pwd = cimissPassword;
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfMTenEleByTensOfYearAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_TEN_MMUT_19812010"); // 资料代码
            //检索时间段
            paramsqx.Add("tensOfYear",((month - 1) * 3 + xun).ToString());
            paramsqx.Add("elements", "Station_Name,Station_Id_C,TEM_Avg,PRE,SSH");
            paramsqx.Add("staIds", stationID);
            string dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            try
            {
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    strData = strData.Replace("\r\n", "\n");
                    string[] szData = strData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (szData.Length > 2)
                    {
                        for (int i = 2; i < szData.Length; i++)
                        {
                            try
                            {

                                string[] szls = szData[i].Split();
                                yBLists.Add(new YBList()
                                {
                                    Name = szls[0],
                                    ID = szls[1],
                                    Tem = Convert.ToDouble(szls[2]),
                                    Pre = Convert.ToDouble(szls[3]) > 1000 ? 0 : Convert.ToDouble(szls[3]),
                                    Sun = Convert.ToDouble(szls[4]) > 1000 ? 0 : Convert.ToDouble(szls[4]),
                                });
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }

            }
            catch
            {

            }
            return yBLists;

        }
        public int getDF(Int16 year, Int16 month, Int16 xun, string stationID)
        {
            int days = 0;
            DataQueryClient client = new DataQueryClient();
            string userId = cimissUserid;
            string pwd = cimissPassword;
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeRangeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_DAY"); // 资料代码
            //检索时间段
            string timeRange = "";
            if (xun == 1)
            {
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "01000000," + year.ToString() + month.ToString().PadLeft(2, '0') + "10000000";
            }
            else if (xun == 2)
            {
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "11000000," + year.ToString() + month.ToString().PadLeft(2, '0') + "20000000";
            }
            else
            {
                DateTime dtls = Convert.ToDateTime(year.ToString() + month.ToString().PadLeft(2, '0') + "01").AddMonths(1).AddDays(-1);
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "21000000," + dtls.ToString("yyyyMMdd") + "000000";
            }
            timeRange = '[' + timeRange + ']';
            paramsqx.Add("timeRange", timeRange);
            paramsqx.Add("eleValueRanges", "GaWIN:1");
            paramsqx.Add("elements", "Station_Name,Station_Id_C,Year,Mon,Day,GaWIN");
            paramsqx.Add("staIds", stationID);
            string dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            try
            {
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    strData = strData.Replace("\r\n", "\n");
                    string[] szData = strData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (szData.Length > 2)
                    {
                        days = szData.Length - 2;
                    }
                }

            }
            catch
            {

            }
            return days;

        }

        public List<YBList> ZRtoXun(Int16 year, Int16 month, Int16 xun, string stationID)
        {
            List<YBList> yBLists = new List<YBList>();
            List<ZRList> zRLists = getZR(year, month, xun, stationID);
            if(zRLists.Count>0)
            {
                string[] szStr = stationID.Split(',');
                for(int i=0;i<szStr.Length;i++)
                {
                    List<ZRList> zRs= zRLists.FindAll(y => y.ID == szStr[i]);
                    if(zRs.Count>0)
                    {
                        YBList yBList = new YBList()
                        {
                            ID=zRs[0].ID,
                            Name=zRs[0].Name,
                            Pre= zRs[0].Pre,
                            Sun= zRs[0].Sun,
                            Tem= zRs[0].Tem,
                            Wind= zRs[0].Wind,
                            Tmax = zRs[0].Tmax,
                            TmaxDays= zRs[0].Days,
                            Tmin= zRs[0].Tmin,
                            TminDays= zRs[0].Days,

                        };
                        if (yBList.ID == "53463")
                            yBList.Name = "市区北部";
                        else if (yBList.ID == "53466")
                            yBList.Name = "市区南部";
                        if (zRs.Count>1)
                        {
                            for(int j=1;j<zRs.Count;j++)
                            {
                                yBList.Pre += zRs[j].Pre;
                                yBList.Sun += zRs[j].Sun;
                                yBList.Tem += zRs[j].Tem;
                                yBList.Wind += zRs[j].Wind;
                                if(yBList.Tmax<zRs[j].Tmax)
                                {
                                    yBList.Tmax = zRs[j].Tmax;
                                    yBList.TmaxDays = zRs[j].Days;
                                }
                                if (yBList.Tmin > zRs[j].Tmin)
                                {
                                    yBList.Tmin = zRs[j].Tmin;
                                    yBList.TminDays = zRs[j].Days;
                                }
                            }
                        }
                        yBList.Tem = Math.Round(yBList.Tem / zRs.Count,1);
                        yBList.Pre = Math.Round(yBList.Pre, 1);
                        yBList.Sun = Math.Round(yBList.Sun, 1);
                        yBLists.Add(yBList);
                    }
                }
            }
            return yBLists;
        }
        public List<ZRList> getZR(Int16 year, Int16 month, Int16 xun, string stationID)
        {
            List<ZRList> zRLists = new List<ZRList>();
            DataQueryClient client = new DataQueryClient();
            string userId = cimissUserid;
            string pwd = cimissPassword;
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfEleByTimeRangeAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MUL_DAY"); // 资料代码
            //检索时间段
            string timeRange = "";
            if (xun == 1)
            {
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "01000000," + year.ToString() + month.ToString().PadLeft(2, '0') + "10000000";
            }
            else if (xun == 2)
            {
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "11000000," + year.ToString() + month.ToString().PadLeft(2, '0') + "20000000";
            }
            else
            {
                DateTime dtls = Convert.ToDateTime(year.ToString() + month.ToString().PadLeft(2, '0') + "01").AddMonths(1).AddDays(-1);
                timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "21000000," + dtls.ToString("yyyyMMdd") + "000000";
            }
            timeRange = '[' + timeRange + ']';
            paramsqx.Add("timeRange", timeRange);
            //paramsqx.Add("eleValueRanges", "GaWIN:1");
            paramsqx.Add("elements", "Station_Name,Station_Id_C,Day,TEM_Avg,TEM_Max,TEM_Min,PRE_Time_2020,SSH,GaWIN");
            paramsqx.Add("staIds", stationID);
            string dataFormat = "Text";
            StringBuilder QXSK = new StringBuilder();//返回字符串
            // 初始化接口服务连接资源
            client.initResources();
            // 调用接口
            int rst = client.callAPI_to_serializedStr(userId, pwd, interfaceId1, paramsqx, dataFormat, QXSK);
            // 释放接口服务连接资源
            client.destroyResources();
            string strData = Convert.ToString(QXSK);
            try
            {
                string strLS = strData.Split('\n')[0].Split()[0].Split('=')[1];
                rst = Convert.ToInt32(Regex.Replace(strLS, "\"", ""));
                if (rst == 0)
                {
                    strData = strData.Replace("\r\n", "\n");
                    string[] szData = strData.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    if (szData.Length > 2)
                    {
                        for (int i = 2; i < szData.Length; i++)
                        {
                            try
                            {

                                string[] szls = szData[i].Split();
                                zRLists.Add(new ZRList()
                                {
                                    Name = szls[0],
                                    ID = szls[1],
                                    Days=Convert.ToInt16(szls[2]),
                                    Tem = Convert.ToDouble(szls[3]),
                                    Tmax= Convert.ToDouble(szls[4]),
                                    Tmin = Convert.ToDouble(szls[5]),
                                    Pre = Convert.ToDouble(szls[6]) > 1000 ? 0 : Convert.ToDouble(szls[6]),
                                    Sun = Convert.ToDouble(szls[7]) > 1000 ? 0 : Convert.ToDouble(szls[7]),
                                    Wind = Convert.ToInt32(szls[8]) > 1000 ? 0 : Convert.ToInt32(szls[8]),
                                });
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                }

            }
            catch
            {

            }
            return zRLists;

        }
        public class YBList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public double Tem { get; set; }
            public double Tmax { get; set; }
            public Int16 TmaxDays { get; set; }
            public double Tmin{ get; set; }
            public Int16 TminDays { get; set; }
            public double Pre { get; set; }
            public double Sun { get; set; }
            public int Wind { get; set; }
        }

        public class ZRList
        {
            public string Name { get; set; }
            public string ID { get; set; }
            public double Tem { get; set; }
            public int Wind { get; set; }
            public double Pre { get; set; }
            public double Sun { get; set; }
            public Int16 Days { get; set; }
            public double Tmax { get; set; }
            public double Tmin { get; set; }
        }
    }
}
