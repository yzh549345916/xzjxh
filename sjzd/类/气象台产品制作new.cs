using Aspose.Words;
using Aspose.Words.Tables;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace sjzd.类
{
    class 气象台产品制作new
    {
        public string 书记短信制作( ref string error)
        {
            报文读取 bw2 = new 报文读取();
            string strDate = DateTime.Now.ToString("yyyyMMdd");
            string ybData = bw2.Sjyb7days(strDate, 8.ToString().PadLeft(2, '0'));
            float tMin = 1000, tMax = 1000;
            try
            {
                CIMISS cIMISS = new CIMISS();
                List<CIMISS.YS> myTem = cIMISS.获取小时温度(DateTime.Now.Date.AddDays(-1).AddHours(9), DateTime.Now.Date.AddDays(0).AddHours(8), "53463");
                tMin = myTem.OrderBy(y => y.TEM_Min).ToList()[0].TEM_Min;
                tMax = myTem.OrderByDescending(y => y.TEM_Max).ToList()[0].TEM_Max;
            }
            catch
            {
            }

            if (ybData.Trim().Length > 0)
            {
                List<YBList> mydatas=CLYB(ybData).FindAll(y=>y.sx<=72);
                string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
                try
                {
                    string SJMBPath = "";

                    string SJsaPath = "";
                    using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                    {
                        string line = "";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Split('=')[0] == "书记短信发布路径")
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
                    SJsaPath += "书记短信" + DateTime.Now.ToString("yyyyMMdd") + ".docx";

                    Document doc = new Document();
                    DocumentBuilder builder = new DocumentBuilder(doc);
                    builder.ParagraphFormat.LineSpacing = 24.0;
                    builder.Font.Size = 14;
                    builder.Font.Name = "宋体";
                    builder.Font.Color = Color.Red;
                    builder.Write("（书记短信，仅供内部参考）");
                    builder.InsertParagraph();
                    builder.Font.Color = Color.Black;
                    builder.Font.Bold = true;
                    builder.Font.Name = "Times New Roman";
                    builder.Write("呼和浩特市昨天天气实况及未来72小时天气预报：");
                    builder.InsertParagraph();
                    builder.Font.Bold = false;
                    if (tMax < 1000 && tMin < 1000)
                    {
                        builder.Write($" 1、{DateTime.Now.AddDays(-1):M月dd日}08时～{DateTime.Now:M月dd日}08时实况：，气温 {tMin}～{tMax}℃。");
                    }
                    else
                    {
                        builder.Write($" 1、{DateTime.Now.AddDays(-1):M月dd日}08时～{DateTime.Now:M月dd日}08时实况：，气温℃。");
                    }
                    builder.InsertParagraph();
                    builder.Write("2、预计：");
                    for (int i =1 ; i <= 3; i++)
                    {
                        builder.InsertParagraph();
                        if (mydatas.Exists(y => y.sx == i * 24))
                        {
                            YBList datals = mydatas.First(y => y.sx == i * 24);
                            builder.Write($" {DateTime.Now.AddDays(i-1):d日}：{datals.TQ}，{datals.TEM}。");
                        }
                        else
                        {
                            builder.Write($" {DateTime.Now.AddDays(i-1):d日}：。");
                        }
                    }

                    doc.Save(SJsaPath);
                    return SJsaPath;



                }

                catch (Exception ex)
                {
                    error = ex.Message;
                }
            }
            else
            {
                error = "城镇预报数据获取失败，无法制作产品";
            }

            return "";
        }


        public List<YBList> CLYB(string ybData)
        {
            List<YBList> yBLists = new List<YBList>();
            foreach (string sls in ybData.Split('\n'))
            {
                string[] szls = sls.Split(',');
                if (szls[0].Trim() == "53463")
                {
                    for (int i = 0; i < 7; i++)
                    {
                        yBLists.Add(new YBList()
                        {
                            sx = (i+1)*24,
                            TQ = szls[3 + i * 5],
                            TEM = szls[1 + i * 5] + "℃～" + szls[2 + i * 5] + "℃",
                           FX = szls[4 + i * 5],
                           FS = szls[5 + i * 5],
                        });
                    }
                    break;

                }
            }
           


            return yBLists;
        }

        public class YBList
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public int sx { get; set; }
            public string TQ { get; set; }
            public string TEM { get; set; }
            public string FS { get; set; }
            public string FX { get; set; }
        }

    }
}
