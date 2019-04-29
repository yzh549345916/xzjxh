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
using Aspose.Cells;

namespace sjzd
{
    class 生态与农业气象信息
    {
        string cimissUserid = "", cimissPassword = "";
        public void DCWord(int QS,int allQS, int year, int month, Int16 xun, string ybName)
        {
            try
            {
                string xunStr = "上旬";
                if (xun == 2)
                {
                    xunStr = "中旬";
                }
                else if (xun == 3)
                {
                    xunStr = "下旬";
                }
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\生态与农业气象信息设置.xml");
                string fbPath = util.Read("Path", "FB") + year + "年生态与农业气象信息\\";
                string xybPath = util.Read("Path", "XYB");
                cimissUserid = util.Read("CIMISS", "ID");
                cimissPassword = util.Read("CIMISS", "Password");
                
                string ysExcelPath = fbPath + year + "气象要素\\" + year + "年气象要素.xls";

                fbPath = fbPath + string.Format("呼和浩特DQ{3}（{0}）{1}月{2}生态与农业气象信息.doc", allQS, month, xunStr,QS);
                string stationID = util.Read("StationConfig");
                if (xybPath.Trim().Length > 0 && fbPath.Trim().Length > 0)
                {
                    List<YBList> dqData = ZRtoXun(year, month, xun, stationID);
                    List<YBList> lastYearData = ZRtoXun((short)(year - 1), month, xun, stationID);
                    List<YBList> tqData = getTQSJ(year, month, xun, stationID);
                    if (dqData.Count == 0)
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
                        if (File.Exists(MBPath))
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
                            if (xun == 2)
                            {
                                timeStr = month.ToString() + "月中旬";
                            }
                            else if (xun == 3)
                            {
                                timeStr = month.ToString() + "月下旬";
                            }
                            builder.MoveToBookmark("标题日期");
                            builder.Write(timeStr);
                            builder.MoveToBookmark("图1时间");
                            builder.Write(timeStr);
                            builder.MoveToBookmark("图2时间");
                            builder.Write(timeStr);
                            List<YBList> dqLS = dqData.OrderByDescending(y => y.Tem).ToList();
                            string strLS = $"{timeStr}我市平均气温{dqLS[dqLS.Count - 1].Tem}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Tem}℃（{dqLS[0].Name}），";
                            List<YBList> tqbjData = new List<YBList>();
                            foreach (YBList yB in dqData)
                            {
                                YBList bList = tqData.Find(y => y.ID == yB.ID);
                                tqbjData.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre, 0),
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
                                    Pre = Math.Round((yB.Pre - bList.Pre), 1),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            
                            string btStr = year + "年" + timeStr + "我市";
                            //温度
                            try
                            {
                                if (!tqbjData.Exists(y => y.Tem > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = tqbjData.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                    btStr += "气温偏低；";
                                    strLS += $"较历年同期偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                                else if (!tqbjData.Exists(y => y.Tem < 0))
                                {
                                    dqLS.Clear();
                                    btStr += "气温偏高；";
                                    dqLS = tqbjData.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                    strLS += $"较历年同期偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                                else
                                {
                                    btStr += "气温";
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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏高，其余地区偏低；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';

                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏低，其余地区偏高；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                    }
                                }
                                if (!qnbjData.Exists(y => y.Tem > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                    strLS += $"较去年同期偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else if (!qnbjData.Exists(y => y.Tem < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

                                        }
                                        strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                }
                                if (month > 3 && month < 9)//4-8月高温
                                {
                                    dqLS.Clear();
                                    dqLS = dqData.OrderBy(y => y.TmaxDays).ToList();
                                    Int16 count = 1;
                                    for (int i = 1; i < dqLS.Count; i++)
                                    {
                                        if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                            count++;
                                    }
                                    if (count == 1)
                                    {
                                        dqLS.Clear();
                                        dqLS = dqData.OrderBy(y => y.Tmax).ToList();
                                        strLS += $"旬内日极端最高气温{dqLS[0].Tmax.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmax.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），均出现在{dqLS[0].TmaxDays}日。";
                                    }
                                    else
                                    {
                                        string strls2 = dqLS[0].Name + '、';
                                        int countls = 0;
                                        for (int i = 1; i < dqLS.Count; i++)
                                        {
                                            if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                            {
                                                strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[i - 1].TmaxDays}日，";
                                                countls++;
                                                if (countls == count - 1)
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
                            //降水
                            try
                            {
                                strLS = timeStr;
                                dqLS.Clear();
                                dqLS = dqData.OrderBy(y => y.Pre).ToList();
                                if (dqLS[0].Pre == 0)
                                {
                                    if (dqLS[dqLS.Count - 1].Pre == 0)
                                    {
                                        strLS += $"我市无降水。";
                                    }
                                    else
                                    {
                                        strLS += "我市降水";
                                        foreach (YBList yB in dqLS)
                                        {
                                            if (yB.Pre > 0)
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
                                    btStr += "降水偏少；";
                                    dqLS.Clear();
                                    dqLS = tqbjData.FindAll(y => y.Pre != 0).OrderByDescending(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"与历年同期相比偏少{(dqLS[0].Pre * -1).ToString("F0")}%（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}与历年同期相比偏少{(dqLS[0].Pre * -1).ToString("F0")}%（图2）。";
                                    }

                                }
                                else if (!tqbjData.Exists(y => y.Pre < 0))
                                {
                                    btStr += "降水偏多；";
                                    dqLS.Clear();
                                    dqLS = tqbjData.FindAll(y => y.Pre != 0).OrderBy(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"与历年同期相比偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}与历年同期相比偏多{dqLS[0].Pre.ToString("F0")}%（图2）。";
                                    }

                                }
                                else
                                {
                                    btStr += "降水";
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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏多，其余地区偏少；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏少，其余地区偏多；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                }
                                if (!qnbjData.Exists(y => y.Pre > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Pre != 0).OrderByDescending(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏少{(dqLS[0].Pre * -1).ToString("F1")}mm（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏少{(dqLS[0].Pre * -1).ToString("F1")}mm。";
                                    }

                                }
                                else if (!qnbjData.Exists(y => y.Pre < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Pre != 0).OrderBy(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏多{dqLS[0].Pre.ToString("F1")}mm（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏多{dqLS[0].Pre.ToString("F1")}mm。";
                                    }

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                            //日照
                            try
                            {
                                dqLS.Clear();
                                dqLS = dqData.OrderByDescending(y => y.Sun).ToList();
                                strLS = $"{timeStr}我市日照时数{dqLS[dqLS.Count - 1].Sun.ToString("F1")}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Sun.ToString("F1")}小时（{dqLS[0].Name}）（图3）。";
                                if (!tqbjData.Exists(y => y.Sun > 0))
                                {
                                    btStr += "日照偏多。";
                                    dqLS.Clear();
                                    dqLS = tqbjData.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较历年同期偏多{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较历年同期偏多{(dqLS[0].Sun * -1).ToString("F1")}。";
                                    }

                                }
                                else if (!tqbjData.Exists(y => y.Sun < 0))
                                {
                                    btStr += "日照偏少。";
                                    dqLS.Clear();
                                    dqLS = tqbjData.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较历年同期偏少{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较历年同期偏少{dqLS[0].Sun.ToString("F1")}。";
                                    }

                                }
                                else
                                {
                                    btStr += "日照";
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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏多，其余地区偏少。";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏少，其余地区偏多。";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                }
                                if (!qnbjData.Exists(y => y.Sun > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏少{(dqLS[0].Sun * -1).ToString("F1")}（图3）。";
                                    }

                                }
                                else if (!qnbjData.Exists(y => y.Sun < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjData.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏多{dqLS[0].Sun.ToString("F1")}（图3）。";
                                    }

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                            //大风
                            try
                            {
                                strLS = timeStr;
                                dqLS.Clear();
                                dqLS = dqData.OrderBy(y => y.Wind).ToList();
                                if (dqLS[0].Wind == 0)
                                {
                                    if (dqLS[dqLS.Count - 1].Wind == 0)
                                    {
                                        strLS += "我市未出现大风。";
                                    }
                                    else
                                    {
                                        strLS += "我市";
                                        foreach (YBList yB in dqLS)
                                        {
                                            if (yB.Wind > 0)
                                            {
                                                strLS += $"{yB.Name}出现{yB.Wind}次、";
                                            }
                                        }
                                        strLS = strLS.Substring(0, strLS.Length - 1) + "大风。";
                                    }
                                }
                                else
                                {
                                    strLS += "我市";
                                    foreach (YBList yB in dqLS)
                                    {
                                        if (yB.Wind > 0)
                                        {
                                            strLS += $"{yB.Name}出现{yB.Wind}次、";
                                        }
                                    }
                                    strLS = strLS.Substring(0, strLS.Length - 1) + "大风。";
                                }

                                builder.MoveToBookmark("大风");
                                builder.Write(strLS);
                            }
                            catch
                            {
                            }
                            try
                            {
                                xybPath += year;
                                DirectoryInfo dir = new DirectoryInfo(xybPath);
                                List<FileInfo> inf = dir.GetFiles("*.doc").Where(y => !y.Name.Contains("$")).OrderBy(y => y.LastWriteTime).ToList();
                                if (inf.Count > 1)
                                {
                                    string fileName = inf[inf.Count - 1].FullName;
                                    int count = 0;
                                    foreach (FileInfo fi in inf)
                                    {
                                        int countls = 0;
                                        try

                                        {
                                            countls = Convert.ToInt32(fi.Name.Replace($"呼市{year}年中期旬报第", "").Replace("期.DOC", ""));
                                            if (count <= countls)
                                            {
                                                count = countls;
                                                fileName = fi.FullName;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    strLS = "";
                                    Document doc2 = new Document(fileName);
                                    if (doc2.FirstSection.Body.Paragraphs.Count > 0)
                                    {
                                        ParagraphCollection pargraphs = doc2.FirstSection.Body.Paragraphs;//word中的所有段落
                                        bool bsls = false;
                                        foreach (var pp in pargraphs)
                                        {
                                            string s = pp.GetText();
                                            if (s.Contains("天气趋势预报"))
                                            {
                                                bsls = true;
                                            }
                                            if (bsls)
                                            {
                                                if (!s.Contains("天气趋势预报") && s.Length > 10)
                                                    strLS += s.Replace("\r", "").Replace("\r\n", "").Replace("\n", "");
                                            }
                                        }
                                    }
                                    string xunls = "";
                                    if (xun == 3)
                                    {
                                        if (month < 12)
                                        {
                                            xunls = (month + 1).ToString() + "月上旬";
                                        }
                                        else
                                        {
                                            xunls = "1月上旬";
                                        }

                                    }
                                    else
                                    {
                                        if (xun == 1)
                                        {
                                            xunls = month + "月中旬";
                                        }
                                        else
                                        {
                                            xunls = month + "月下旬";
                                        }
                                    }
                                    strLS = "呼市气象台预计： " + strLS.Replace("预计,", "").Replace("预计，", "").Replace("旬内", xunls);
                                    builder.MoveToBookmark("气候展望");
                                    builder.Write(strLS);

                                }

                            }
                            catch
                            {
                            }
                            builder.MoveToBookmark("提要");
                            builder.Write(btStr);
                            doc.Save(fbPath);
                            if (!File.Exists(ysExcelPath))
                            {
                                try
                                {
                                    File.Copy(Environment.CurrentDirectory + @"\生态\气象要素.xls", ysExcelPath);
                                }
                                catch
                                {
                                }
                            }
                            BCYBExcel(ysExcelPath, dqData, tqbjData, qnbjData, tqData, lastYearData, month, xun);
                            MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + fbPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                            if (dr == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(fbPath);
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
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void DCWordMonth(int QS, int allQS, int year, int month,  string ybName)
        {
            try
            {
               
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\生态与农业气象信息设置.xml");
                string fbPath = util.Read("Path", "FB") + year + "年生态与农业气象信息\\";
                string xybPath = util.Read("Path", "XYB");
                cimissUserid = util.Read("CIMISS", "ID");
                cimissPassword = util.Read("CIMISS", "Password");
                string ysExcelPath = fbPath + year + "气象要素\\" + year + "年气象要素.xls";
                string qxysPath = fbPath + year + "气象要素\\";
                fbPath = fbPath + string.Format("呼和浩特DQ{2}（{0}）{1}月及下旬生态与农业气象信息.doc", allQS, month,QS);
                string stationID = util.Read("StationConfig");
                
                if (xybPath.Trim().Length > 0 && fbPath.Trim().Length > 0)
                {
                    List<YBList> dqDataMonth = ZRtoMonth(year, month, stationID);
                    List<YBList> lastYearDataMonth = ZRtoMonth((short)(year - 1), month,  stationID);
                    List<YBList> tqDataMonth = getTQSJ(year, month, stationID);
                    List<YBList> dqData1 = ZRtoXun(year, month, 1, stationID);
                    List<YBList> lastYearData1 = ZRtoXun((short)(year - 1), month, 1, stationID);
                    List<YBList> tqData1 = getTQSJ(year, month, 1, stationID);
                    List<YBList> dqData2 = ZRtoXun(year, month, 2, stationID);
                    List<YBList> lastYearData2 = ZRtoXun((short)(year - 1), month, 2, stationID);
                    List<YBList> tqData2 = getTQSJ(year, month, 2, stationID);
                    List<YBList> dqData3 = ZRtoXun(year, month, 3, stationID);
                    List<YBList> lastYearData3 = ZRtoXun((short)(year - 1), month, 3, stationID);
                    List<YBList> tqData3 = getTQSJ(year, month, 3, stationID);
                    if (dqDataMonth.Count == 0|| lastYearDataMonth.Count == 0 || tqDataMonth.Count==0)
                    {
                        MessageBox.Show("月相关数据获取失败");
                    }
                    else if (dqData1.Count == 0|| tqData1.Count == 0 || lastYearData1.Count == 0)
                    {
                        MessageBox.Show("上旬相关数据获取失败");
                    }
                    else if (dqData2.Count == 0 || tqData2.Count == 0 || lastYearData2.Count == 0)
                    {
                        MessageBox.Show("中旬相关数据获取失败");
                    }
                    else if (dqData3.Count == 0 || tqData3.Count == 0 || lastYearData3.Count == 0)
                    {
                        MessageBox.Show("下旬相关数据获取失败");
                    }
                    else
                    {
                        string MBPath = Environment.CurrentDirectory + @"\模版\生态与农业气象信息下旬模板.doc";
                        if (File.Exists(MBPath))
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
                            string timeStr = month.ToString() + "月";
                            builder.MoveToBookmark("标题日期");
                            builder.Write(timeStr+"及下旬");
                            builder.MoveToBookmark("图1时间");
                            builder.Write(timeStr);
                            builder.MoveToBookmark("图2时间");
                            builder.Write(timeStr);
                            List<YBList> dqLS = dqDataMonth.OrderByDescending(y => y.Tem).ToList();
                            string strLS = $"{year}年{timeStr}我市平均气温{dqLS[dqLS.Count - 1].Tem}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Tem}℃（{dqLS[0].Name}），";
                            List<YBList> tqbjDataMonth = new List<YBList>();
                            foreach (YBList yB in dqDataMonth)
                            {
                                YBList bList = tqDataMonth.Find(y => y.ID == yB.ID);
                                tqbjDataMonth.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre, 0),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> tqbjData1 = new List<YBList>();
                            foreach (YBList yB in dqData1)
                            {
                                YBList bList = tqData1.Find(y => y.ID == yB.ID);
                                tqbjData1.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre, 0),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> tqbjData2 = new List<YBList>();
                            foreach (YBList yB in dqData2)
                            {
                                YBList bList = tqData2.Find(y => y.ID == yB.ID);
                                tqbjData2.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre, 0),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> tqbjData3 = new List<YBList>();
                            foreach (YBList yB in dqData3)
                            {
                                YBList bList = tqData3.Find(y => y.ID == yB.ID);
                                tqbjData3.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round(((yB.Pre - bList.Pre) * 100) / bList.Pre, 0),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> qnbjDataMonth = new List<YBList>();
                            foreach (YBList yB in dqDataMonth)
                            {
                                YBList bList = lastYearDataMonth.Find(y => y.ID == yB.ID);
                                qnbjDataMonth.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round((yB.Pre - bList.Pre), 1),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> qnbjData1 = new List<YBList>();
                            foreach (YBList yB in dqData1)
                            {
                                YBList bList = lastYearData1.Find(y => y.ID == yB.ID);
                                qnbjData1.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round((yB.Pre - bList.Pre), 1),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> qnbjData2 = new List<YBList>();
                            foreach (YBList yB in dqData2)
                            {
                                YBList bList = lastYearData2.Find(y => y.ID == yB.ID);
                                qnbjData2.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round((yB.Pre - bList.Pre), 1),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            List<YBList> qnbjData3 = new List<YBList>();
                            foreach (YBList yB in dqData3)
                            {
                                YBList bList = lastYearData3.Find(y => y.ID == yB.ID);
                                qnbjData3.Add(new YBList()
                                {
                                    ID = yB.ID,
                                    Name = yB.Name,
                                    Tem = yB.Tem - bList.Tem,
                                    Pre = Math.Round((yB.Pre - bList.Pre), 1),
                                    Sun = yB.Sun - bList.Sun,
                                });
                            }
                            string btStr = year + "年" + timeStr + "我市";
                            //温度
                            try
                            {
                                if (!tqbjDataMonth.Exists(y => y.Tem > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = tqbjDataMonth.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                    btStr += "气温偏低；";
                                    strLS += $"较历年同期相比偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                                else if (!tqbjDataMonth.Exists(y => y.Tem < 0))
                                {
                                    dqLS.Clear();
                                    btStr += "气温偏高；";
                                    dqLS = tqbjDataMonth.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                    strLS += $"较历年同期相比偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                }
                                else
                                {
                                    btStr += "气温";
                                    List<YBList> list1 = tqbjDataMonth.FindAll(y => y.Tem < 0);
                                    List<YBList> list2 = tqbjDataMonth.FindAll(y => y.Tem > 0);
                                    List<YBList> list3 = tqbjDataMonth.FindAll(y => y.Tem == 0);
                                    if (list1.Count > list2.Count)
                                    {
                                        dqLS.Clear();
                                        dqLS = list1.OrderByDescending(y => y.Tem).ToList();
                                        strLS += $"与历年同期相比，";
                                        foreach (YBList yB in list2)
                                        {
                                            strLS += $"{yB.Name}偏高{yB.Tem.ToString("F1")}℃，";
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏高，其余地区偏低；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';

                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏低，其余地区偏高；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）（图1）。";
                                    }
                                }
                                strLS += "其中上旬";
                                try
                                {
                                    if (!tqbjData1.Exists(y => y.Tem > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData1.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                        strLS += $"偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else if (!tqbjData1.Exists(y => y.Tem < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData1.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                        strLS += $"偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData1.FindAll(y => y.Tem < 0);
                                        List<YBList> list2 = tqbjData1.FindAll(y => y.Tem > 0);
                                        List<YBList> list3 = tqbjData1.FindAll(y => y.Tem == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "中旬";
                                try
                                {
                                    if (!tqbjData2.Exists(y => y.Tem > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData2.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                        strLS += $"偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else if (!tqbjData2.Exists(y => y.Tem < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData2.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                        strLS += $"偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData2.FindAll(y => y.Tem < 0);
                                        List<YBList> list2 = tqbjData2.FindAll(y => y.Tem > 0);
                                        List<YBList> list3 = tqbjData2.FindAll(y => y.Tem == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "下旬";
                                try
                                {
                                    if (!tqbjData3.Exists(y => y.Tem > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData3.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                        strLS += $"偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else if (!tqbjData3.Exists(y => y.Tem < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData3.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                        strLS += $"偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData3.FindAll(y => y.Tem < 0);
                                        List<YBList> list2 = tqbjData3.FindAll(y => y.Tem > 0);
                                        List<YBList> list3 = tqbjData3.FindAll(y => y.Tem == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Tem).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "月平均气温";
                                if (!qnbjDataMonth.Exists(y => y.Tem > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Tem != 0).OrderByDescending(y => y.Tem).ToList();
                                    strLS += $"较去年同期偏低{(dqLS[0].Tem * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Tem * -1).ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else if (!qnbjDataMonth.Exists(y => y.Tem < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Tem != 0).OrderBy(y => y.Tem).ToList();
                                    strLS += $"较去年同期偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                }
                                else
                                {
                                    List<YBList> list1 = qnbjDataMonth.FindAll(y => y.Tem < 0);
                                    List<YBList> list2 = qnbjDataMonth.FindAll(y => y.Tem > 0);
                                    List<YBList> list3 = qnbjDataMonth.FindAll(y => y.Tem == 0);
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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期持平，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期持平，";

                                        }
                                        strLS += $"其余地区偏高{dqLS[0].Tem.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tem.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                }
                                if (month > 3 && month < 9)//4-8月高温
                                {
                                    dqLS.Clear();
                                    dqLS = dqDataMonth.OrderBy(y => y.TmaxDays).ToList();
                                    Int16 count = 1;
                                    for (int i = 1; i < dqLS.Count; i++)
                                    {
                                        if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                            count++;
                                    }
                                    if (count == 1)
                                    {
                                        dqLS.Clear();
                                        dqLS = dqDataMonth.OrderBy(y => y.Tmax).ToList();
                                        strLS += $"月内日极端最高气温{dqLS[0].Tmax.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmax.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），均出现在{dqLS[0].TmaxDays}日。";
                                    }
                                    else
                                    {
                                        string strls2 = dqLS[0].Name + '、';
                                        int countls = 0;
                                        for (int i = 1; i < dqLS.Count; i++)
                                        {
                                            if (dqLS[i].TmaxDays != dqLS[i - 1].TmaxDays)
                                            {
                                                strls2 = strls2.Substring(0, strls2.Length - 1) + $"出现在{dqLS[i - 1].TmaxDays}日，";
                                                countls++;
                                                if (countls == count - 1)
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
                                        dqLS = dqDataMonth.OrderBy(y => y.Tmax).ToList();
                                        strLS += $"月内日极端最高气温{dqLS[0].Tmax.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmax.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），{strls2}";
                                    }
                                }
                                else
                                {
                                    dqLS.Clear();
                                    dqLS = dqDataMonth.OrderBy(y => y.TminDays).ToList();
                                    Int16 count = 1;
                                    for (int i = 1; i < dqLS.Count; i++)
                                    {
                                        if (dqLS[i].TminDays != dqLS[i - 1].TminDays)
                                            count++;
                                    }
                                    if (count == 1)
                                    {
                                        dqLS.Clear();
                                        dqLS = dqDataMonth.OrderBy(y => y.Tmin).ToList();
                                        strLS += $"月内日极端最低气温{dqLS[0].Tmin.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmin.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），均出现在{dqLS[0].TminDays}日。";
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
                                        dqLS = dqDataMonth.OrderBy(y => y.Tmin).ToList();
                                        strLS += $"月内日极端最低气温{dqLS[0].Tmin.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Tmin.ToString("F1")}℃（{dqLS[dqLS.Count - 1].Name}），{strls2}";
                                    }
                                }

                                builder.MoveToBookmark("气温");
                                builder.Write(strLS);
                            }
                            catch
                            {
                            }
                            //降水
                            try
                            {
                                strLS = timeStr;
                                dqLS.Clear();
                                dqLS = dqDataMonth.OrderBy(y => y.Pre).ToList();
                                if (dqLS[0].Pre == 0)
                                {
                                    if (dqLS[dqLS.Count - 1].Pre == 0)
                                    {
                                        strLS += $"我市无降水。";
                                    }
                                    else
                                    {
                                        strLS += "我市降水量";
                                        foreach (YBList yB in dqLS)
                                        {
                                            if (yB.Pre > 0)
                                            {
                                                strLS += $"{yB.Name}{yB.Pre}mm，";
                                            }
                                        }
                                        strLS += "其余地区无降水。";
                                    }
                                }
                                else
                                {
                                    strLS += "我市降水量";
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
                                if (!tqbjDataMonth.Exists(y => y.Pre > 0))
                                {
                                    btStr += "降水偏少；";
                                    dqLS.Clear();
                                    dqLS = tqbjDataMonth.FindAll(y => y.Pre != 0).OrderByDescending(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"与历年同期相比偏少{(dqLS[0].Pre * -1).ToString("F0")}%（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}与历年同期相比偏少{(dqLS[0].Pre * -1).ToString("F0")}%（图2）。";
                                    }

                                }
                                else if (!tqbjDataMonth.Exists(y => y.Pre < 0))
                                {
                                    btStr += "降水偏多；";
                                    dqLS.Clear();
                                    dqLS = tqbjDataMonth.FindAll(y => y.Pre != 0).OrderBy(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"与历年同期相比偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}与历年同期相比偏多{dqLS[0].Pre.ToString("F0")}%（图2）。";
                                    }

                                }
                                else
                                {
                                    btStr += "降水";
                                    List<YBList> list1 = tqbjDataMonth.FindAll(y => y.Pre < 0);
                                    List<YBList> list2 = tqbjDataMonth.FindAll(y => y.Pre > 0);
                                    List<YBList> list3 = tqbjDataMonth.FindAll(y => y.Pre == 0);
                                    if (list1.Count > list2.Count)
                                    {
                                        dqLS.Clear();
                                        dqLS = list1.OrderByDescending(y => y.Pre).ToList();
                                        strLS += $"与历年同期相比，";
                                        foreach (YBList yB in list2)
                                        {
                                            strLS += $"{yB.Name}偏多{yB.Pre.ToString("F0")}%，";
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏多，其余地区偏少；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏少，其余地区偏多；";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏多{dqLS[0].Pre.ToString("F0")}%（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F0")}%（{dqLS[dqLS.Count - 1].Name}）（图2）。";
                                    }
                                }
                                strLS += "月降水量";
                                if (!qnbjDataMonth.Exists(y => y.Pre > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Pre != 0).OrderByDescending(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏少{(dqLS[0].Pre * -1).ToString("F1")}mm（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Pre * -1).ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏少{(dqLS[0].Pre * -1).ToString("F1")}mm。";
                                    }

                                }
                                else if (!qnbjDataMonth.Exists(y => y.Pre < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Pre != 0).OrderBy(y => y.Pre).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏多{dqLS[0].Pre.ToString("F1")}mm（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Pre.ToString("F1")}mm（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏多{dqLS[0].Pre.ToString("F1")}mm。";
                                    }

                                }
                                else
                                {
                                    List<YBList> list1 = qnbjDataMonth.FindAll(y => y.Pre < 0);
                                    List<YBList> list2 = qnbjDataMonth.FindAll(y => y.Pre > 0);
                                    List<YBList> list3 = qnbjDataMonth.FindAll(y => y.Pre == 0);
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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                            //日照
                            try
                            {
                                dqLS.Clear();
                                dqLS = dqDataMonth.OrderByDescending(y => y.Sun).ToList();
                                strLS = $"{timeStr}我市日照时数{dqLS[dqLS.Count - 1].Sun.ToString("F1")}（{dqLS[dqLS.Count - 1].Name}）～{dqLS[0].Sun.ToString("F1")}小时（{dqLS[0].Name}）（图3）。";
                                if (!tqbjDataMonth.Exists(y => y.Sun > 0))
                                {
                                    btStr += "日照偏多。";
                                    dqLS.Clear();
                                    dqLS = tqbjDataMonth.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较历年同期偏多{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较历年同期偏多{(dqLS[0].Sun * -1).ToString("F1")}。";
                                    }

                                }
                                else if (!tqbjDataMonth.Exists(y => y.Sun < 0))
                                {
                                    btStr += "日照偏少。";
                                    dqLS.Clear();
                                    dqLS = tqbjDataMonth.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较历年同期偏少{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较历年同期偏少{dqLS[0].Sun.ToString("F1")}。";
                                    }

                                }
                                else
                                {
                                    btStr += "日照";
                                    List<YBList> list1 = tqbjDataMonth.FindAll(y => y.Sun < 0);
                                    List<YBList> list2 = tqbjDataMonth.FindAll(y => y.Sun > 0);
                                    List<YBList> list3 = tqbjDataMonth.FindAll(y => y.Sun == 0);
                                    if (list1.Count > list2.Count)
                                    {
                                        dqLS.Clear();
                                        dqLS = list1.OrderByDescending(y => y.Sun).ToList();
                                        strLS += $"与历年同期相比，";
                                        foreach (YBList yB in list2)
                                        {
                                            strLS += $"{yB.Name}偏多{yB.Sun.ToString("F1")}小时，";
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏多，其余地区偏少。";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

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
                                            btStr += yB.Name + "、";
                                        }
                                        btStr = btStr.Substring(0, btStr.Length - 1) + "偏少，其余地区偏多。";
                                        if (list3.Count > 0)
                                        {
                                            foreach (YBList yB in list3)
                                            {
                                                strLS += yB.Name + '、';
                                            }
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期相同，";

                                        }
                                        strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                }
                                strLS += "其中上旬";
                                try
                                {
                                    if (!tqbjData1.Exists(y => y.Sun > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData1.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                        strLS += $"偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else if (!tqbjData1.Exists(y => y.Sun < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData1.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                        strLS += $"偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData1.FindAll(y => y.Sun < 0);
                                        List<YBList> list2 = tqbjData1.FindAll(y => y.Sun > 0);
                                        List<YBList> list3 = tqbjData1.FindAll(y => y.Sun == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "中旬";
                                try
                                {
                                    if (!tqbjData2.Exists(y => y.Sun > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData2.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                        strLS += $"偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else if (!tqbjData2.Exists(y => y.Sun < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData2.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                        strLS += $"偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData2.FindAll(y => y.Sun < 0);
                                        List<YBList> list2 = tqbjData2.FindAll(y => y.Sun > 0);
                                        List<YBList> list3 = tqbjData2.FindAll(y => y.Sun == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）；";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "下旬";
                                try
                                {
                                    if (!tqbjData3.Exists(y => y.Sun > 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData3.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                        strLS += $"偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else if (!tqbjData3.Exists(y => y.Sun < 0))
                                    {
                                        dqLS.Clear();
                                        dqLS = tqbjData3.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                        strLS += $"偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                    }
                                    else
                                    {
                                        List<YBList> list1 = tqbjData3.FindAll(y => y.Sun < 0);
                                        List<YBList> list2 = tqbjData3.FindAll(y => y.Sun > 0);
                                        List<YBList> list3 = tqbjData3.FindAll(y => y.Sun == 0);
                                        if (list1.Count > list2.Count)
                                        {
                                            dqLS.Clear();
                                            dqLS = list1.OrderByDescending(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                        }
                                        else
                                        {
                                            dqLS.Clear();
                                            dqLS = list2.OrderBy(y => y.Sun).ToList();
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
                                                strLS = strLS.Substring(0, strLS.Length - 1) + "与历年同期持平，";

                                            }
                                            strLS += $"其余地区偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）。";
                                        }
                                    }
                                }
                                catch
                                {
                                }
                                strLS += "月日照时数";
                                if (!qnbjDataMonth.Exists(y => y.Sun > 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Sun != 0).OrderByDescending(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏少{(dqLS[0].Sun * -1).ToString("F1")}（{dqLS[0].Name}）～{(dqLS[dqLS.Count - 1].Sun * -1).ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏少{(dqLS[0].Sun * -1).ToString("F1")}（图3）。";
                                    }

                                }
                                else if (!qnbjDataMonth.Exists(y => y.Sun < 0))
                                {
                                    dqLS.Clear();
                                    dqLS = qnbjDataMonth.FindAll(y => y.Sun != 0).OrderBy(y => y.Sun).ToList();
                                    if (dqLS.Count > 1)
                                    {
                                        strLS += $"较去年同期偏多{dqLS[0].Sun.ToString("F1")}（{dqLS[0].Name}）～{dqLS[dqLS.Count - 1].Sun.ToString("F1")}小时（{dqLS[dqLS.Count - 1].Name}）（图3）。";
                                    }
                                    else
                                    {
                                        strLS += $"{dqLS[0].Name}较去年同期偏多{dqLS[0].Sun.ToString("F1")}（图3）。";
                                    }

                                }
                                else
                                {
                                    List<YBList> list1 = qnbjDataMonth.FindAll(y => y.Sun < 0);
                                    List<YBList> list2 = qnbjDataMonth.FindAll(y => y.Sun > 0);
                                    List<YBList> list3 = qnbjDataMonth.FindAll(y => y.Sun == 0);
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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                                            strLS = strLS.Substring(0, strLS.Length - 1) + "与去年同期相同，";

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
                            //大风
                            try
                            {
                                strLS = timeStr;
                                dqLS.Clear();
                                dqLS = dqDataMonth.OrderBy(y => y.Wind).ToList();
                                if (dqLS[0].Wind == 0)
                                {
                                    if (dqLS[dqLS.Count - 1].Wind == 0)
                                    {
                                        strLS += "我市未出现大风。";
                                    }
                                    else
                                    {
                                        strLS += "我市";
                                        foreach (YBList yB in dqLS)
                                        {
                                            if (yB.Wind > 0)
                                            {
                                                strLS += $"{yB.Name}出现{yB.Wind}次、";
                                            }
                                        }
                                        strLS = strLS.Substring(0, strLS.Length - 1) + "大风。";
                                    }
                                }
                                else
                                {
                                    strLS += "我市";
                                    foreach (YBList yB in dqLS)
                                    {
                                        if (yB.Wind > 0)
                                        {
                                            strLS += $"{yB.Name}出现{yB.Wind}次、";
                                        }
                                    }
                                    strLS = strLS.Substring(0, strLS.Length - 1) + "大风。";
                                }

                                builder.MoveToBookmark("大风");
                                builder.Write(strLS);
                            }
                            catch
                            {
                            }
                            try
                            {
                                xybPath += year;
                                DirectoryInfo dir = new DirectoryInfo(xybPath);
                                List<FileInfo> inf = dir.GetFiles("*.doc").Where(y => !y.Name.Contains("$")).OrderBy(y => y.LastWriteTime).ToList();
                               
                                if (inf.Count > 0)
                                {
                                    string fileName = inf[inf.Count - 1].FullName;
                                    int count = 0;
                                    foreach(FileInfo fi in inf)
                                    {
                                        int countls = 0;
                                        try

                                        {
                                            countls = Convert.ToInt32(fi.Name.Replace($"呼市{year}年中期旬报第", "").Replace("期.DOC", ""));
                                            if(count<=countls)
                                            {
                                                count = countls;
                                                fileName = fi.FullName;
                                            }
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    strLS = "";
                                    Document doc2 = new Document(fileName);
                                    if (doc2.FirstSection.Body.Paragraphs.Count > 0)
                                    {
                                        ParagraphCollection pargraphs = doc2.FirstSection.Body.Paragraphs;//word中的所有段落
                                        bool bsls = false;
                                        foreach (var pp in pargraphs)
                                        {
                                            string s = pp.GetText();
                                            if (s.Contains("天气趋势预报"))
                                            {
                                                bsls = true;
                                            }
                                            if (bsls)
                                            {
                                                if (!s.Contains("天气趋势预报") && s.Length > 10)
                                                    strLS += s.Replace("\r", "").Replace("\r\n", "").Replace("\n", "");
                                            }
                                        }
                                    }
                                    string xunls = "";
                                    if (month < 12)
                                    {
                                        xunls = (month + 1).ToString() + "月上旬";
                                    }
                                    else
                                    {
                                        xunls = "1月上旬";
                                    }
                                    strLS = "呼市气象台预计： " + strLS.Replace("预计,", "").Replace("预计，", "").Replace("旬内", xunls);
                                    builder.MoveToBookmark("气候展望");
                                    builder.Write(strLS);

                                }

                            }
                            catch
                            {
                            }
                            //地下水位
                            try
                            {
                                string dxPat = util.Read("Path", "DXSW");
                                List<SW> sWs = new List<SW>();
                                DirectoryInfo dir = new DirectoryInfo(dxPat);
                                DirectoryInfo[] dirs = dir.GetDirectories();
                                List<FileInfo> fileInfos = new List<FileInfo>();
                                foreach (DirectoryInfo ls in dirs)
                                {
                                    if (ls.Name.Contains(year + month.ToString().PadLeft(2, '0')) || (month != 12 ? ls.Name.Contains(year + (month + 1).ToString().PadLeft(2, '0')) : ls.Name.Contains((year + 1).ToString() + "01")))
                                    {
                                        List<FileInfo> inf = ls.GetFiles().Where(y => y.FullName.Contains("53464") || y.FullName.Contains("53466") || y.FullName.Contains("53467") || y.FullName.Contains("53469") || y.FullName.Contains("53562") || y.FullName.Contains("53368")).OrderBy(y => y.LastWriteTime).ToList();
                                        fileInfos.AddRange(inf);
                                    }

                                }
                                fileInfos = fileInfos.OrderByDescending(y => y.LastWriteTime).ToList();
                                foreach (FileInfo file in fileInfos)
                                {
                                    try
                                    {
                                        string myID = file.Name.Split('_')[3];
                                        StreamReader sr1 = file.OpenText();
                                        string[] szls = sr1.ReadToEnd().Split('@');
                                        szls = szls[szls.Length - 3].Split(',');
                                        double mySw = Convert.ToDouble(szls[szls.Length - 1]) / 100;
                                        if (sWs.Find(y => y.ID == myID && y.Dxsw < 99.99) == null)
                                        {
                                            SW swls = new SW();
                                            if ((swls = sWs.Find(y => y.ID == myID)) != null)
                                            {
                                                sWs.Remove(swls);
                                                sWs.Add(new SW()
                                                {
                                                    ID = myID,
                                                    Dxsw = mySw
                                                });
                                            }
                                            else
                                            {
                                                sWs.Add(new SW()
                                                {
                                                    ID = myID,
                                                    Dxsw = mySw
                                                });
                                            }
                                        }
                                        if (sWs.FindAll(y => y.Dxsw < 99.99).Count >= 6)
                                        {
                                            break;
                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                    }
                                }
                                if (sWs.FindAll(y => y.Dxsw < 99.99).Count < 6)
                                {
                                    MessageBox.Show("地下水位数据异常，请仔细核对");
                                }

                                if (sWs.Count > 0)
                                {
                                    DateTime dateTimedq = Convert.ToDateTime(year.ToString() + '-' + month.ToString().PadLeft(2, '0') + "-01");
                                    string pathLs = qxysPath + dateTimedq.ToString("yyyy") + "地下水位.txt";
                                    string strls = "";
                                    MessageBoxResult dr2 = MessageBox.Show("是否更新地下水位信息？" , "提示", MessageBoxButton.YesNo);
                                    bool bsSaveDX = (dr2 == MessageBoxResult.Yes);
                                    if(bsSaveDX)
                                    {
                                        if (File.Exists(pathLs))
                                        {

                                            using (StreamReader sr = new StreamReader(pathLs, Encoding.Default))
                                            {

                                                string line = "";
                                                while ((line = sr.ReadLine()) != null)
                                                {
                                                    try
                                                    {
                                                        if (line.Split('\t')[0] != year.ToString() + month.ToString().PadLeft(2, '0'))
                                                        {
                                                            strls += line + "\r\n";
                                                        }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }


                ;
                                            }
                                        }
                                        foreach (SW sW in sWs)
                                        {
                                            strls += year.ToString() + month.ToString().PadLeft(2, '0') + '\t' + sW.ID + '\t' + sW.Dxsw.ToString("F2") + "\r\n";
                                        }
                                        strls = strls.Substring(0, strls.Length - 2);
                                        File.WriteAllText(pathLs, strls, Encoding.Default);
                                    }
                                   
                                    strls = "";
                                    List<SW> sWsLastMonth = new List<SW>();
                                    List<SW> sWsLastYear = new List<SW>();
                                    DateTime dateTimelastmonth = dateTimedq.AddMonths(-1);
                                    DateTime dateTimelastyear = dateTimedq.AddYears(-1);
                                    pathLs = qxysPath + dateTimelastyear.ToString("yyyy") + "地下水位.txt";
                                    if (File.Exists(pathLs))
                                    {
                                        using (StreamReader sr = new StreamReader(pathLs, Encoding.Default))
                                        {

                                            string line = "";
                                            while ((line = sr.ReadLine()) != null)
                                            {
                                                try
                                                {
                                                    if (line.Split('\t')[0] == dateTimelastyear.ToString("yyyy") + month.ToString().PadLeft(2, '0'))
                                                    {
                                                        sWsLastYear.Add(new SW()
                                                        {
                                                            ID = line.Split('\t')[1],
                                                            Dxsw = Convert.ToDouble(line.Split('\t')[2]),
                                                        });
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }


            ;
                                        }
                                    }
                                    pathLs = qxysPath + dateTimelastmonth.ToString("yyyy") + "地下水位.txt";
                                    if (File.Exists(pathLs))
                                    {
                                        using (StreamReader sr = new StreamReader(pathLs, Encoding.Default))
                                        {

                                            string line = "";
                                            while ((line = sr.ReadLine()) != null)
                                            {
                                                try
                                                {
                                                    if (line.Split('\t')[0] == dateTimelastmonth.ToString("yyyyMM"))
                                                    {
                                                        sWsLastMonth.Add(new SW()
                                                        {
                                                            ID = line.Split('\t')[1],
                                                            Dxsw = Convert.ToDouble(line.Split('\t')[2]),
                                                        });
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                            }


            ;
                                        }
                                    }
                                    if (sWsLastMonth.Count > 0 & sWsLastYear.Count > 0)
                                    {
                                      
                                       
                                        List<SW> sWsls = new List<SW>();
                                        for(int i=0;i<sWs.Count;i++)
                                        {
                                            YBList yBList = dqDataMonth.Find(y => y.ID == sWs[i].ID);
                                            if(yBList!=null)
                                            {
                                                sWs[i].Name = yBList.Name;
                                            }
                                        }
                                        if (bsSaveDX)
                                        {
                                            try
                                            {
                                                string pathDX = ysExcelPath.Replace("气象要素.xls", "地下水位.xls");
                                                if (!File.Exists(pathDX))
                                                {
                                                    try
                                                    {
                                                        File.Copy(Environment.CurrentDirectory + @"\生态\地下水位.xls", pathDX);
                                                    }
                                                    catch
                                                    {
                                                    }
                                                }
                                                BCSWExcel(pathDX, sWs, sWsLastMonth, sWsLastYear, year, month);
                                            }
                                            catch
                                            {
                                            }
                                        }

                                        for (int i = 0; i < sWsLastYear.Count; i++)
                                        {
                                            SW yBList = sWs.Find(y => y.ID == sWsLastYear[i].ID);
                                            if (yBList != null)
                                            {
                                                sWsLastYear[i].Dxsw = sWsLastYear[i].Dxsw-yBList.Dxsw;
                                                sWsLastYear[i].Name = yBList.Name;
                                            }
                                        }
                                        for (int i = 0; i < sWsLastMonth.Count; i++)
                                        {
                                            SW yBList = sWs.Find(y => y.ID == sWsLastMonth[i].ID);
                                            if (yBList != null)
                                            {
                                                sWsLastMonth[i].Dxsw = sWsLastMonth[i].Dxsw-yBList.Dxsw  ;
                                                sWsLastMonth[i].Name = yBList.Name;
                                            }
                                        }
                                        sWs = sWs.OrderByDescending(y => y.Dxsw).ToList();
                                        strls = $"{year}年{month}月末地下水位{ sWs[sWs.Count - 1].Dxsw}（{ sWs[sWs.Count - 1].Name}）～{ sWs[0].Dxsw}m（{ sWs[0].Name}）。";
                                        if (!sWsLastYear.Exists(y => y.Dxsw > 0))
                                        {
                                           
                                            sWsls.Clear();
                                            sWsls = sWsLastYear.FindAll(y => y.Dxsw != 99.99).OrderByDescending(y => y.Dxsw).ToList();
                                            if (sWsls.Count > 1)
                                            {
                                                strls += $"与去年同期相比上升{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                strls += $"{sWsls[0].Name}与去年同期相比上升{(sWsls[0].Dxsw * -1).ToString("F2")}。";
                                            }

                                        }
                                        else if (!sWsLastYear.Exists(y => y.Dxsw < 0))
                                        {
                                           
                                            sWsls.Clear();
                                            sWsls = sWsLastYear.FindAll(y => y.Dxsw != 0).OrderBy(y => y.Dxsw).ToList();
                                            if (sWsls.Count > 1)
                                            {
                                                strls += $"与去年同期相比下降{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                strls += $"{sWsls[0].Name}与去年同期相比下降{sWsls[0].Dxsw.ToString("F2")}。";
                                            }

                                        }
                                        else
                                        {
                                            List<SW> list1 = sWsLastYear.FindAll(y => y.Dxsw < 0);
                                            List<SW> list2 = sWsLastYear.FindAll(y => y.Dxsw > 0);
                                            List<SW> list3 = sWsLastYear.FindAll(y => y.Dxsw == 0);
                                            if (list1.Count > list2.Count)
                                            {
                                                sWsls.Clear();
                                                sWsls = list1.OrderByDescending(y => y.Dxsw).ToList();
                                                strls += $"与去年同期相比，";
                                                foreach (SW yB in list2)
                                                {
                                                    strls += $"{yB.Name}上升{yB.Dxsw.ToString("F2")}m，";
                                                }
                                                if (list3.Count > 0)
                                                {
                                                    foreach (SW yB in list3)
                                                    {
                                                        strls += yB.Name + '、';
                                                    }
                                                    strls = strls.Substring(0, strls.Length - 1) + "与去年同期相同，";

                                                }
                                                strls += $"其余地区下降{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                sWsls.Clear();
                                                sWsls = list2.OrderBy(y => y.Dxsw).ToList();
                                                strls += $"与去年同期相比，";
                                                foreach (SW yB in list1)
                                                {
                                                    strls += $"{yB.Name}下降{(yB.Dxsw * -1).ToString("F2")}m，";
                                                }
                                                if (list3.Count > 0)
                                                {
                                                    foreach (SW yB in list3)
                                                    {
                                                        strls += yB.Name + '、';
                                                    }
                                                    strls = strls.Substring(0, strls.Length - 1) + "与去年同期相同，";

                                                }
                                                strls += $"其余地区上升{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                        }
                                        btStr += "地下水位";
                                        if (!sWsLastMonth.Exists(y => y.Dxsw > 0))
                                        {
                                            sWsls.Clear();
                                            sWsls = sWsLastMonth.FindAll(y => y.Dxsw != 0).OrderByDescending(y => y.Dxsw).ToList();
                                            if (sWsls.Count > 1)
                                            {
                                                strls += $"与上月末相比上升{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）（图4）。";
                                                btStr += $"与上月末相比上升{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                strls += $"{sWsls[0].Name}较上月末上升{(sWsls[0].Dxsw * -1).ToString("F2")}（图4）。";
                                            }

                                        }
                                        else if (!sWsLastMonth.Exists(y => y.Dxsw < 0))
                                        {
                                            sWsls.Clear();
                                            sWsls = sWsLastMonth.FindAll(y => y.Dxsw != 0).OrderBy(y => y.Dxsw).ToList();
                                            if (sWsls.Count > 1)
                                            {
                                                strls += $"与上月末相比下降{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）（图4）。";
                                                btStr+= $"与上月末相比下降{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                strls += $"{sWsls[0].Name}与上月末相比下降{sWsls[0].Dxsw.ToString("F2")}（图4）。";
                                            }

                                        }
                                        else
                                        {
                                            List<SW> list1 = sWsLastMonth.FindAll(y => y.Dxsw < 0);
                                            List<SW> list2 = sWsLastMonth.FindAll(y => y.Dxsw > 0);
                                            List<SW> list3 = sWsLastMonth.FindAll(y => y.Dxsw == 0);
                                            if (list1.Count > list2.Count)
                                            {
                                                sWsls.Clear();
                                                sWsls = list1.OrderByDescending(y => y.Dxsw).ToList();
                                                strls += $"与上月末相比，";
                                                btStr+= $"与上月末相比，";
                                                foreach (SW yB in list2)
                                                {
                                                    strls += $"{yB.Name}上升{yB.Dxsw.ToString("F2")}m，";
                                                    btStr += $"{yB.Name}上升{yB.Dxsw.ToString("F2")}m，";
                                                }
                                                if (list3.Count > 0)
                                                {
                                                    foreach (SW yB in list3)
                                                    {
                                                        strls += yB.Name + '、';
                                                        btStr += yB.Name + '、';
                                                    }
                                                    strls = strls.Substring(0, strls.Length - 1) + "与上月末持平，";
                                                    btStr = btStr.Substring(0, strls.Length - 1) + "与上月末持平，";
                                                }
                                                strls += $"其余地区下降{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）（图4）。";
                                                btStr += $"其余地区下降{(sWsls[0].Dxsw * -1).ToString("F2")}（{sWsls[0].Name}）～{(sWsls[sWsls.Count - 1].Dxsw * -1).ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                            else
                                            {
                                                sWsls.Clear();
                                                sWsls = list2.OrderBy(y => y.Dxsw).ToList();
                                                strls += $"与上月末相比，";
                                                btStr += $"与上月末相比，";
                                                foreach (SW yB in list1)
                                                {
                                                    strls += $"{yB.Name}下降{(yB.Dxsw * -1).ToString("F2")}m，";
                                                    btStr += $"{yB.Name}下降{(yB.Dxsw * -1).ToString("F2")}m，";
                                                }
                                                if (list3.Count > 0)
                                                {
                                                    foreach (SW yB in list3)
                                                    {
                                                        strls += yB.Name + '、';
                                                        btStr += yB.Name + '、';
                                                    }
                                                    strls = strls.Substring(0, strls.Length - 1) + "与历年同期相同，";
                                                    btStr = btStr.Substring(0, strls.Length - 1) + "与历年同期相同，";

                                                }
                                                strls += $"其余地区上升{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）（图4）。";
                                                btStr += $"其余地区上升{sWsls[0].Dxsw.ToString("F2")}（{sWsls[0].Name}）～{sWsls[sWsls.Count - 1].Dxsw.ToString("F2")}m（{sWsls[sWsls.Count - 1].Name}）。";
                                            }
                                        }
                                       

                                    }
                                    builder.MoveToBookmark("地下水位");
                                    builder.Write(strls);
                                }
                               

                            }
                            catch (Exception ex)
                            {
                            }

                            builder.MoveToBookmark("提要");
                            builder.Write(btStr);
                            doc.Save(fbPath);
                            if(!File.Exists(ysExcelPath))
                            {
                               try
                               {
                                    File.Copy(Environment.CurrentDirectory + @"\生态\气象要素.xls", ysExcelPath);
                                }
                               catch
                               {
                               }
                            }
                            BCYBExcel(ysExcelPath, dqData3, tqbjData3, qnbjData3, tqData3, lastYearData3, month,3);
                            BCYBExcel(ysExcelPath, dqDataMonth, tqbjDataMonth, qnbjDataMonth, tqDataMonth, lastYearDataMonth, month);
                            MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + fbPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                            if (dr == MessageBoxResult.Yes)
                            {
                                try
                                {
                                    System.Diagnostics.Process.Start(fbPath);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class SW
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public double Dxsw { get; set; }
        }
        public void BCYBExcel(string path, List<YBList> dqYb, List<YBList> bjTQ, List<YBList> bjLast, List<YBList> TQ, List<YBList> Last, int month,Int16 xun)
        {
            try
            {
                Workbook workbook = new Workbook(path);
                WorksheetCollection sheets = workbook.Worksheets; //添加工作表 
                Worksheet cellSheet = sheets[$"{month}月"];
                if(cellSheet==null)//如果标签不存在则复制新建
                {
                    
                    cellSheet = sheets.Insert(month-1, SheetType.Worksheet, $"{month}月");
                    cellSheet.Copy(sheets[0]);
                    //sheet.AddCopy(0);
                    // sheet[sheet.Count - 1].Name = "14月";
                }
                int startHS = 0;
                if(xun==1)
                {
                    startHS = 1;
                }
                else if(xun == 2)
                {
                    startHS = 3 + dqYb.Count;
                }
                else 
                {
                    startHS = 5 + dqYb.Count*2;
                }
                dqYb = dqYb.OrderBy(y => y.ID).ToList();
                for(int i=0;i< dqYb.Count;i++)
                {
                    YBList ybtqbj = bjTQ.Find(y => y.ID == dqYb[i].ID);
                    YBList ybqnbj = bjLast.Find(y => y.ID == dqYb[i].ID);
                    YBList ybqn = Last.Find(y => y.ID == dqYb[i].ID);
                    YBList ybtq = TQ.Find(y => y.ID == dqYb[i].ID);
                    cellSheet.Cells[startHS+i, 0].PutValue(dqYb[i].ID);
                    cellSheet.Cells[startHS + i, 1].PutValue(dqYb[i].Name);
                    cellSheet.Cells[startHS + i, 2].PutValue(dqYb[i].Tem);
                    cellSheet.Cells[startHS + i, 3].PutValue(ybtqbj.Tem);
                    cellSheet.Cells[startHS + i, 4].PutValue(dqYb[i].Tmin);
                    cellSheet.Cells[startHS + i, 5].PutValue(dqYb[i].TminDays);
                    cellSheet.Cells[startHS + i, 6].PutValue(dqYb[i].Pre);
                    cellSheet.Cells[startHS + i, 7].PutValue(ybtqbj.Pre);
                    cellSheet.Cells[startHS + i, 8].PutValue(dqYb[i].Sun);
                    cellSheet.Cells[startHS + i, 9].PutValue(ybtqbj.Sun);
                    cellSheet.Cells[startHS + i, 10].PutValue(dqYb[i].Wind);
                    cellSheet.Cells[startHS + i, 12].PutValue(ybqn.Tem);
                    cellSheet.Cells[startHS + i, 13].PutValue(ybqnbj.Tem);
                    cellSheet.Cells[startHS + i, 14].PutValue(ybqn.Pre);
                    cellSheet.Cells[startHS + i, 15].PutValue(ybqnbj.Pre);
                    cellSheet.Cells[startHS + i, 16].PutValue(ybqn.Sun);
                    cellSheet.Cells[startHS + i, 17].PutValue(ybqnbj.Sun);
                    cellSheet.Cells[startHS + i, 19].PutValue(dqYb[i].Tmax);
                    cellSheet.Cells[startHS + i, 20].PutValue(dqYb[i].TmaxDays);
                }
                sheets.ActiveSheetIndex = month - 1;
                workbook.Save(path);

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void BCYBExcel(string path, List<YBList> dqYb, List<YBList> bjTQ, List<YBList> bjLast, List<YBList> TQ, List<YBList> Last, int month)
        {
            try
            {
                Workbook workbook = new Workbook(path);
                WorksheetCollection sheets = workbook.Worksheets; //添加工作表 
                Worksheet cellSheet = sheets[$"{month}月"];
                if (cellSheet == null)//如果标签不存在则复制新建
                {

                    cellSheet = sheets.Insert(month - 1, SheetType.Worksheet, $"{month}月");
                    cellSheet.Copy(sheets[0]);
                    //sheet.AddCopy(0);
                    // sheet[sheet.Count - 1].Name = "14月";
                }
                int startHS = 7 + dqYb.Count * 3;
                cellSheet.Cells[startHS -1,1].PutValue($"{month}月");
                cellSheet.Cells[startHS - 1, 9].PutValue("月最低");
                cellSheet.Cells[startHS - 1, 9].PutValue("出现日期");
                cellSheet.Cells[startHS - 1, 19].PutValue("月最高");
                cellSheet.Cells[startHS - 1,20].PutValue("出现日期");
                dqYb = dqYb.OrderBy(y => y.ID).ToList();
                for (int i = 0; i < dqYb.Count; i++)
                {
                    YBList ybtqbj = bjTQ.Find(y => y.ID == dqYb[i].ID);
                    YBList ybqnbj = bjLast.Find(y => y.ID == dqYb[i].ID);
                    YBList ybqn = Last.Find(y => y.ID == dqYb[i].ID);
                    YBList ybtq = TQ.Find(y => y.ID == dqYb[i].ID);
                    cellSheet.Cells[startHS + i, 0].PutValue(dqYb[i].ID);
                    cellSheet.Cells[startHS + i, 1].PutValue(dqYb[i].Name);
                    cellSheet.Cells[startHS + i, 2].PutValue(dqYb[i].Tem);
                    cellSheet.Cells[startHS + i, 3].PutValue(ybtqbj.Tem);
                    cellSheet.Cells[startHS + i, 4].PutValue(dqYb[i].Pre);
                    cellSheet.Cells[startHS + i, 5].PutValue(ybtqbj.Pre);
                    cellSheet.Cells[startHS + i, 6].PutValue(dqYb[i].Sun);
                    cellSheet.Cells[startHS + i, 7].PutValue(ybtqbj.Sun);

                    cellSheet.Cells[startHS + i, 8].PutValue(dqYb[i].Wind);
                    cellSheet.Cells[startHS + i, 9].PutValue(dqYb[i].Tmin);
                    cellSheet.Cells[startHS + i, 10].PutValue(dqYb[i].TminDays);
                   
                    cellSheet.Cells[startHS + i, 12].PutValue(ybqn.Tem);
                    cellSheet.Cells[startHS + i, 13].PutValue(ybqnbj.Tem);
                    cellSheet.Cells[startHS + i, 14].PutValue(ybqn.Pre);
                    cellSheet.Cells[startHS + i, 15].PutValue(ybqnbj.Pre);
                    cellSheet.Cells[startHS + i, 16].PutValue(ybqn.Sun);
                    cellSheet.Cells[startHS + i, 17].PutValue(ybqnbj.Sun);
                    cellSheet.Cells[startHS + i, 19].PutValue(dqYb[i].Tmax);
                    cellSheet.Cells[startHS + i, 20].PutValue(dqYb[i].TmaxDays);
                }
                sheets.ActiveSheetIndex = month - 1;
                workbook.Save(path);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public void BCSWExcel(string path, List<SW> dqYb, List<SW> LastMonth, List<SW> LastYear, int year, int month)
        {
            try
            {
                Workbook workbook = new Workbook(path);
                WorksheetCollection sheets = workbook.Worksheets; //添加工作表 
                Worksheet cellSheet = sheets[$"{year}"];
                if (cellSheet == null)//如果标签不存在则复制新建
                {

                    cellSheet = sheets.Insert(1, SheetType.Worksheet, $"{year}");
                    cellSheet.Copy(sheets[0]);
                }
                cellSheet.Cells[34, 0].PutValue($"{year}年{month}月");
                cellSheet.Cells[35, 0].PutValue($"{year-1}年{month}月");
                cellSheet.Cells[36, 0].PutValue($"{year}年{month-1}月");
                dqYb = dqYb.OrderBy(y => y.ID).ToList();
                for (int i = 0; i < dqYb.Count; i++)
                {
                    SW ybqn = LastMonth.Find(y => y.ID == dqYb[i].ID);
                    SW ybtq = LastYear.Find(y => y.ID == dqYb[i].ID);
                    cellSheet.Cells[2, i+1].PutValue(dqYb[i].Name);
                    cellSheet.Cells[2 + month, i + 1].PutValue(ybtq.Dxsw);
                    cellSheet.Cells[17 , i + 1].PutValue(dqYb[i].Name);
                    cellSheet.Cells[17 + month, i + 1].PutValue(dqYb[i].Dxsw);
                    cellSheet.Cells[33, i + 1].PutValue(dqYb[i].Name);
                    cellSheet.Cells[34, i + 1].PutValue(dqYb[i].Dxsw);
                    cellSheet.Cells[35, i + 1].PutValue(ybtq.Dxsw);
                    cellSheet.Cells[36, i + 1].PutValue(ybqn.Dxsw);
                    cellSheet.Cells[38, i + 1].PutValue(dqYb[i].Dxsw- ybtq.Dxsw);
                    cellSheet.Cells[39, i + 1].PutValue(dqYb[i].Dxsw- ybqn.Dxsw);
                }
                sheets.ActiveSheetIndex = sheets.IndexOf(cellSheet);
                workbook.Save(path);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
        public List<YBList> getTQSJ(int year, int month, Int16 xun, string stationID)
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
        public List<YBList> getTQSJ(int year, int month, string stationID)
        {
            List<YBList> yBLists = new List<YBList>();
            DataQueryClient client = new DataQueryClient();
            string userId = cimissUserid;
            string pwd = cimissPassword;
            /*   2.2 接口ID */
            String interfaceId1 = "getSurfMMonEleByMonthsOfYearAndStaID";
            /*   2.3 接口参数，多个参数间无顺序 */
            Dictionary<String, String> paramsqx = new Dictionary<String, String>();
            // 必选参数
            paramsqx.Add("dataCode", "SURF_CHN_MON_MMUT_19812010"); // 资料代码
            //检索时间段
            paramsqx.Add("monsOfYear", month.ToString());
            paramsqx.Add("elements", "Station_Name,Station_Id_C,TEM_Avg,PRE_Time_2020_MMUT,SSH");
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
        public int getDF(int year, int month, Int16 xun, string stationID)
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
        public List<YBList> ZRtoMonth(int year, int month,  string stationID)
        {
            List<YBList> yBLists = new List<YBList>();
            List<ZRList> zRLists = getZR(year, month,  stationID);
            if (zRLists.Count > 0)
            {
                string[] szStr = stationID.Split(',');
                for (int i = 0; i < szStr.Length; i++)
                {
                    List<ZRList> zRs = zRLists.FindAll(y => y.ID == szStr[i]);
                    if (zRs.Count > 0)
                    {
                        YBList yBList = new YBList()
                        {
                            ID = zRs[0].ID,
                            Name = zRs[0].Name,
                            Pre = zRs[0].Pre,
                            Sun = zRs[0].Sun,
                            Tem = zRs[0].Tem,
                            Wind = zRs[0].Wind,
                            Tmax = zRs[0].Tmax,
                            TmaxDays = zRs[0].Days,
                            Tmin = zRs[0].Tmin,
                            TminDays = zRs[0].Days,

                        };
                        if (yBList.ID == "53463")
                            yBList.Name = "市区北部";
                        else if (yBList.ID == "53466")
                            yBList.Name = "市区南部";
                        if (zRs.Count > 1)
                        {
                            for (int j = 1; j < zRs.Count; j++)
                            {
                                yBList.Pre += zRs[j].Pre;
                                yBList.Sun += zRs[j].Sun;
                                yBList.Tem += zRs[j].Tem;
                                yBList.Wind += zRs[j].Wind;
                                if (yBList.Tmax < zRs[j].Tmax)
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
                        yBList.Tem = Math.Round(yBList.Tem / zRs.Count, 1);
                        yBList.Pre = Math.Round(yBList.Pre, 1);
                        yBList.Sun = Math.Round(yBList.Sun, 1);
                        yBLists.Add(yBList);
                    }
                }
            }
            return yBLists;
        }
        public List<ZRList> getZR(int year, int month, string stationID)
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
            DateTime dtls = Convert.ToDateTime(year.ToString() + '-' + month.ToString().PadLeft(2, '0') + '-' + "01").AddMonths(1).AddDays(-1);
            timeRange = year.ToString() + month.ToString().PadLeft(2, '0') + "01000000," + dtls.ToString("yyyyMMdd") + "000000";
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
                                    Days = Convert.ToInt16(szls[2]),
                                    Tem = Convert.ToDouble(szls[3]),
                                    Tmax = Convert.ToDouble(szls[4]),
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
        public List<YBList> ZRtoXun(int year, int month, Int16 xun, string stationID)
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
        public List<ZRList> getZR(int year, int month, Int16 xun, string stationID)
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
                DateTime dtls = Convert.ToDateTime(year.ToString() + '-'+month.ToString().PadLeft(2, '0') + '-' + "01").AddMonths(1).AddDays(-1);
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
