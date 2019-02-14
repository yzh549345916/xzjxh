using System;
using System.Linq;
using System.Text;
using Aspose.Words;
using Aspose.Words.Tables;
using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Fields;
using System.IO;
using System.Windows;
using System.Data;
using System.Drawing;

public class ZDSZCL
{
    string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
    string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";
    string RKTime = "20";//预报的时次,程序中会重新给该值从配置文件中赋值
                         //指导预报数组转换为报文
    public void ZDSZ2BW(string[,] szYB)
    {
        string line = "";
        using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
        {


            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("市局预报读取指导预报时次"))
                {
                    RKTime = line.Substring("市局预报读取指导预报时次=".Length);
                }

            }
        }

        string strDate = DateTime.Now.ToString("yyyy-MM-dd") + " " + RKTime + ":00:00";
        DateTime dtLS = Convert.ToDateTime(strDate);
        dtLS = dtLS.ToUniversalTime();
        string BWName = "Z_SEVP_C_BABJ_" + DateTime.UtcNow.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SCMOC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
        string CSBWPathCon = Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";

        string CSBWPath = "";
        using (StreamReader sr3 = new StreamReader(CSBWPathCon, Encoding.Default))
        {
            while ((line = sr3.ReadLine()) != null)
            {
                if (line.Contains("市局初始报文保存地址"))
                {
                    CSBWPath = line.Split('=')[1];
                }
            }
        }
        CSBWPath += BWName;//存的未订正的报文的全路径
        string ZDXQConf = Environment.CurrentDirectory + @"\设置文件\旗县乡镇具体.txt";//保存站点的经纬度信息,报文中需要
        string BWData = "ZCZC\r\nFSC150 BABJ " + dtLS.ToString("ddhhmm") + "\r\n" + dtLS.ToString("yyyyMMddhh") + "时中央台指导产品\r\nSCMOC  " + dtLS.ToString("yyyyMMddhh") + "\r\n" + szYB.GetLength(0).ToString() + "\r\n";//保存报文内容
        string WeatherDZ = System.Environment.CurrentDirectory + @"\设置文件\天气对照.txt";
        string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
        string FSDZ = System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
        for (int i = 0; i < szYB.GetLength(0); i++)
        {
            string strLS = BWData;
            string ID = szYB[i, 1];
            using (StreamReader sr1 = new StreamReader(ZDXQConf, Encoding.Default))
            {
                while ((line = sr1.ReadLine()) != null)
                {
                    if (line.Contains('='))
                    {
                        string[] szls = (line.Split('=')[1]).Split();
                        if (szls[0] == ID)
                        {
                            try
                            {
                                float jd = (Convert.ToSingle(szls[2])) / 100, wd = (Convert.ToSingle(szls[1])) / 100, gd = (Convert.ToSingle(szls[6])) / 10;

                                BWData += ID + jd.ToString().PadLeft(8) + wd.ToString().PadLeft(8) + gd.ToString().PadLeft(8) + " 14 21\r\n";
                            }
                            catch (Exception ex)
                            {

                            }
                        }

                    }

                }
            }
            if (BWData.Length == strLS.Length)
            {
                BWData += ID + 0.ToString().PadLeft(8) + 0.ToString().PadLeft(8) + 0.ToString().PadLeft(8) + " 14 21\r\n";
            }

            string[] weather = new string[7], FXFS = new string[7];
            float[] Tmax = new float[7], Tmin = new float[7];
            int intLS1 = 0, intLS2 = 0, intLS3 = 0, intLS4 = 0;
            for (int j = 2; j < szYB.GetLength(1); j++)
            {
                if ((j - 2) % 4 == 0)
                {
                    weather[intLS1++] = szYB[i, j];
                }
                else if ((j - 3) % 4 == 0)
                {
                    FXFS[intLS2++] = szYB[i, j];
                }
                else if ((j - 4) % 4 == 0)
                {
                    Tmin[intLS3++] = Convert.ToSingle(szYB[i, j]);
                }
                else if ((j - 5) % 4 == 0)
                {
                    Tmax[intLS4++] = Convert.ToSingle(szYB[i, j]);
                }

            }
            intLS1 = 0; intLS2 = 0; intLS3 = 0; intLS4 = 0;
            string weatherLS1 = "", FXLS1 = "", FSLS1 = "", weatherLS2 = "", FXLS2 = "", FSLS2 = "";
            for (int j = 0; j < 14; j++)
            {
                string line1 = "";
                int intls = 12 * (j + 1);
                BWData += intls.ToString().PadLeft(3);

                if (j % 2 == 0)
                {
                    for (int k = 0; k < 18; k++)
                    {
                        BWData += 999.9.ToString().PadLeft(6);
                    }
                    string[] WeaSz = weather[intLS1++].Split('转');
                    if (WeaSz.Length > 1)
                    {
                        weatherLS1 = WeaSz[0];
                        weatherLS2 = WeaSz[1];

                        using (StreamReader sr2 = new StreamReader(WeatherDZ, Encoding.Default))
                        {

                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Split('=')[0]==(weatherLS1))
                                {
                                    weatherLS1 = line1.Split('=')[1];
                                    weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                }
                                else if (line1.Split('=')[0] == (weatherLS2))
                                {
                                    weatherLS2 = line1.Split('=')[1];
                                    weatherLS2 = (Convert.ToSingle(weatherLS2)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                    }
                    else if (WeaSz.Length <= 1)
                    {
                        weatherLS1 = WeaSz[0];
                        using (StreamReader sr2 = new StreamReader(WeatherDZ, Encoding.Default))
                        {

                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Split('=')[0] == (weatherLS1))
                                {
                                    weatherLS1 = line1.Split('=')[1];
                                    weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        weatherLS2 = weatherLS1;
                    }
                    BWData += weatherLS1;
                    FXFS[intLS2] = FXFS[intLS2].Replace("旋转/不定", "不定风");//因为按照“转”字拆分风向风俗，当指导预报风向出现“旋转/不定”的风时候，会导致数组拆分失败，故将其替换为“不定风”
                    if (!(FXFS[intLS2].Contains('风')) && !(FXFS[intLS2].Contains('级')))
                    {
                        FXLS1 = ""; FSLS1 = ""; FXLS2 = ""; FSLS2 = "";
                        FXLS1 = 999.9.ToString("f1").PadLeft(6);
                        FXLS2 = FXLS1;
                        FSLS1 = 999.9.ToString("f1").PadLeft(6);
                        FSLS2 = FXLS1;
                        intLS2++;
                    }//判断该时次风的内容是否为空
                    else if ((FXFS[intLS2].Split('风').Length == 3) && (FXFS[intLS2].Split('级').Length == 3))
                    {
                        string[] fxfsStr = FXFS[intLS2++].Split('转');
                        FXLS1 = fxfsStr[0].Split('风')[0] + '风';
                        FSLS1 = fxfsStr[0].Split('风')[1];
                        FXLS2 = fxfsStr[1].Split('风')[0] + '风';
                        FSLS2 = fxfsStr[1].Split('风')[1];
                        using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FXLS1))
                                {
                                    FXLS1 = line1.Split('=')[1];
                                    FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                }
                                else if (line1.Contains(FXLS2))
                                {
                                    FXLS2 = line1.Split('=')[1];
                                    FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FSLS1))
                                {
                                    FSLS1 = line1.Split('=')[1];
                                    FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                }
                                else if (line1.Contains(FSLS2))
                                {
                                    FSLS2 = line1.Split('=')[1];
                                    FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                }

                            }
                        }

                    }
                    else if (!(FXFS[intLS2].Contains('级')) && (FXFS[intLS2].Contains('风')))
                    {
                        string[] fxfsStr = FXFS[intLS2++].Split('转');
                        FXLS1 = fxfsStr[0]; FXLS2 = "";
                        FSLS1 = 999.9.ToString("f1").PadLeft(6);
                        FSLS2 = FSLS1;
                        if (fxfsStr.Length == 1)
                        {
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            FXLS2 = FXLS1;

                        }
                        else
                        {
                            FXLS2 = fxfsStr[1];
                            using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS1))
                                    {
                                        FXLS1 = line1.Split('=')[1];
                                        FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FXLS2))
                                    {
                                        FXLS2 = line1.Split('=')[1];
                                        FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }

                        }
                    }
                    else if ((FXFS[intLS2].Contains('级')) && !(FXFS[intLS2].Contains('风')))
                    {
                        string[] fxfsStr = FXFS[intLS2++].Split('转');
                        FSLS1 = fxfsStr[0]; FSLS2 = "";
                        FXLS1 = 999.9.ToString("f1").PadLeft(6);
                        FXLS2 = FXLS1;
                        if (fxfsStr.Length == 1)
                        {
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            FSLS2 = FSLS1;

                        }
                        else
                        {
                            FSLS2 = fxfsStr[1];
                            using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                while ((line1 = sr2.ReadLine()) != null)
                                {
                                    if (line1.Contains(FSLS1))
                                    {
                                        FSLS1 = line1.Split('=')[1];
                                        FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(FSLS2))
                                    {
                                        FSLS2 = line1.Split('=')[1];
                                        FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                        }

                    }
                    else if ((FXFS[intLS2].Split('风').Length == 3) && (FXFS[intLS2].Split('级').Length == 2))
                    {
                        FSLS1 = FXFS[intLS2].Split('风')[2];
                        string[] fxfsStr = FXFS[intLS2++].Split('转');
                        FXLS1 = fxfsStr[0];
                        FXLS2 = fxfsStr[1].Split('风')[0] + '风';
                        using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FXLS1))
                                {
                                    FXLS1 = line1.Split('=')[1];
                                    FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                }
                                else if (line1.Contains(FXLS2))
                                {
                                    FXLS2 = line1.Split('=')[1];
                                    FXLS2 = (Convert.ToSingle(FXLS2)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FSLS1))
                                {
                                    FSLS1 = line1.Split('=')[1];
                                    FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        FSLS2 = FSLS1;


                    }
                    else if ((FXFS[intLS2].Split('风').Length == 2) && (FXFS[intLS2].Split('级').Length == 3))
                    {
                        FXLS1 = FXFS[intLS2].Split('风')[0] + '风';
                        string[] fxfsStr = (FXFS[intLS2++].Split('风')[1]).Split('转');
                        FSLS1 = fxfsStr[0];
                        FSLS2 = fxfsStr[1];
                        using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FSLS1))
                                {
                                    FSLS1 = line1.Split('=')[1];
                                    FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                }
                                else if (line1.Contains(FSLS2))
                                {
                                    FSLS2 = line1.Split('=')[1];
                                    FSLS2 = (Convert.ToSingle(FSLS2)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FXLS1))
                                {
                                    FXLS1 = line1.Split('=')[1];
                                    FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                }
                            }
                        }
                        FXLS2 = FXLS1;
                    }
                    else if ((FXFS[intLS2].Split('风').Length == 2) && (FXFS[intLS2].Split('级').Length == 2))
                    {
                        FXLS1 = FXFS[intLS2].Split('风')[0] + '风';
                        FSLS1 = FXFS[intLS2++].Split('风')[1];
                        using (StreamReader sr2 = new StreamReader(FXDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FXLS1))
                                {
                                    FXLS1 = line1.Split('=')[1];
                                    FXLS1 = (Convert.ToSingle(FXLS1)).ToString("f1").PadLeft(6);
                                }
                            }
                        }
                        FXLS2 = FXLS1;
                        using (StreamReader sr2 = new StreamReader(FSDZ, Encoding.Default))
                        {
                            while ((line1 = sr2.ReadLine()) != null)
                            {
                                if (line1.Contains(FSLS1))
                                {
                                    FSLS1 = line1.Split('=')[1];
                                    FSLS1 = (Convert.ToSingle(FSLS1)).ToString("f1").PadLeft(6);
                                }

                            }
                        }
                        FSLS2 = FSLS1;
                    }
                    BWData += FXLS1 + FSLS1 + "\r\n";
                }
                else
                {
                    for (int k = 0; k < 10; k++)
                    {
                        BWData += 999.9.ToString().PadLeft(6);
                    }
                    BWData += Tmax[intLS3++].ToString("f1").PadLeft(6) + Tmin[intLS4++].ToString("f1").PadLeft(6);
                    for (int k = 0; k < 6; k++)
                    {
                        BWData += 999.9.ToString().PadLeft(6);
                    }
                    BWData += weatherLS2 + FXLS2 + FSLS2 + "\r\n";
                    weatherLS1 = ""; FXLS1 = ""; FSLS1 = ""; weatherLS2 = ""; FXLS2 = ""; FSLS2 = "";

                }
            }
        }

        try
        {
            using (FileStream fsr = new FileStream(CSBWPath, FileMode.Create))
            {
                StreamWriter sw = new StreamWriter(fsr, Encoding.Default);
                sw.Write(BWData);
                sw.Flush();
                sw.Close();
            }
        }
        catch (Exception ex)
        {

        }

    }//该方法将指导预报数组转换为报文与产品，输入数组行数为旗县乡镇个数，每行内容为：旗县名称+区站号+未来七天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*7

    //输入数组每行内容为：旗县名称+区站号+未来三天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*3。方法将数组中的指定要素保存至发布单
    public void DCWord(string[,] szYB)
    {
        try
        {
            string SJMBPath = Environment.CurrentDirectory + @"\模版\市局乡镇精细化预报模板.docx";
            string SJsaPath = "";
            using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
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
            SJsaPath += DateTime.Now.ToString("yyyy") + "\\" + DateTime.Now.ToString("MM") + "月\\";
            if (!File.Exists(SJsaPath))
            {
                Directory.CreateDirectory(SJsaPath);
            }
            SJsaPath += DateTime.Now.ToString("MM.dd") + "发布单.docx";
            Document doc = new Document(SJMBPath);
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.CellFormat.Borders.LineStyle = LineStyle.Single;
            builder.CellFormat.Borders.Color = Color.Black;
            builder.MoveToBookmark("标题日期");
            builder.Font.Size = 16;
            builder.Font.Name = "宋体";
            builder.Write(DateTime.Now.ToString("yyyy年MM月dd日"));

            builder.MoveToBookmark("预报24");
            builder.InsertCell();
            builder.Font.Name = "宋体";
            builder.Font.Size = 11;
            builder.Write("名称");
            builder.InsertCell();
            builder.Write("天气现象");
            builder.InsertCell();
            builder.Write("最低温度");
            builder.InsertCell();
            builder.Write("最高温度");
            builder.EndRow();
            for (int i = 0; i < szYB.GetLength(0); i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if ((j != 1) && (j % 4 != 3))
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }


                }
                builder.EndRow();
            }
            builder.EndTable();
            builder.MoveToBookmark("预报48");
            builder.InsertCell();
            builder.Font.Name = "宋体";
            builder.Font.Size = 11;
            builder.Write("名称");
            builder.InsertCell();
            builder.Write("天气现象");
            builder.InsertCell();
            builder.Write("最低温度");
            builder.InsertCell();
            builder.Write("最高温度");
            builder.EndRow();
            for (int i = 0; i < szYB.GetLength(0); i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (j == 0)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 6)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 8)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 9)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }

                }
                builder.EndRow();
            }
            builder.EndTable();
            builder.MoveToBookmark("预报72");
            builder.InsertCell();
            builder.Font.Name = "宋体";
            builder.Font.Size = 11;
            builder.Write("名称");
            builder.InsertCell();
            builder.Write("天气现象");
            builder.InsertCell();
            builder.Write("最低温度");
            builder.InsertCell();
            builder.Write("最高温度");
            builder.EndRow();
            for (int i = 0; i < szYB.GetLength(0); i++)
            {
                for (int j = 0; j < 14; j++)
                {
                    if (j == 0)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 10)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 12)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }
                    else if (j == 13)
                    {
                        builder.InsertCell();
                        builder.Font.Name = "宋体";
                        builder.Font.Size = 11;
                        builder.Write(szYB[i, j]);
                    }

                }
                builder.EndRow();
            }
            builder.EndTable();

            doc.Save(SJsaPath);
            MessageBox.Show("产品发布完成,保存路径为：\r\n"+ SJsaPath);
            
        }
        
        catch(Exception ex)
        {

        }
    }

    public void ZDYBBWtoSZ(string YBDate)
    {
        string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
        string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
        string QXCPNamepath = System.Environment.CurrentDirectory + @"\设置文件\指导预报与产品旗县名称对照.txt";
        string YBpath = "";
        string line;
        using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
        {

            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("乡镇精细化预报报文路径"))
                {
                    YBpath = line.Substring("乡镇精细化预报报文路径=".Length);
                    break;
                }
            }
        }
        string sjBWID = "";
        using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
        {
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Split('=')[0] == ("市本级"))
                {
                    sjBWID = line.Split('=')[1];
                    break;
                }
            }
        }
        int intCount = 0;//记录该报文中的站点数
        string strParPath = "*" + sjBWID + "*" + YBDate + "*";
        string[] fileNameList = Directory.GetFiles(YBpath, strParPath);
        if (fileNameList.Length > 0)
        {
            Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
            Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                                               //寻找指定日期中该旗县的最晚和最早报文在fileNameList文件列表中的序号，最晚报文的全路径的为fileNameList[maxXH]，最早的为fileNameList[minXH]
            for (Int16 j = 0; j < fileNameList.Length; j++)
            {
                string strLS = fileNameList[j].Split('_')[4];
                intLS = Convert.ToInt16(strLS.Substring(strLS.Length - 2));
                if (j == 0)
                {
                    maxLS = intLS;
                    minLS = intLS;
                }
                else
                {
                    if (intLS > maxLS)
                    {
                        maxLS = intLS;
                        maxXH = j;
                    }
                    if (intLS < minLS)
                    {
                        minLS = intLS;
                        minXH = j;
                    }
                }
            }
            using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
            {
                int lineCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    if (lineCount == 4)
                    {
                        intCount = Convert.ToInt32(line);
                        break;
                    }
                    lineCount++;
                }
            }
            float[] Tmax24 = new float[intCount], Tmin24 = new float[intCount], Tmax48 = new float[intCount], Tmax72 = new float[intCount], Tmin48 = new float[intCount], Tmin72 = new float[intCount];
            string[] StationID = new string[intCount], Rain24 = new string[intCount], Rain48 = new string[intCount], Rain72 = new string[intCount];
            string[] FX24 = new string[intCount], FS24 = new string[intCount], FX48 = new string[intCount], FS48 = new string[intCount], FX72 = new string[intCount], FS72 = new string[intCount];
            string WeatherDZ = System.Environment.CurrentDirectory + @"\设置文件\天气对照.txt";
            float WeatherLS = 0, FXLS = 0, FSLS = 0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
            string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
            string FSDZ = System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
            using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
            {
                int lineCount = 0;
                int k = 0;
                while (((line = sr.ReadLine()) != null) && k < intCount)//k代表乡镇的序号
                {
                    string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (lineCount == (15 * k + 5))
                    {
                        StationID[k] = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    else if (lineCount == (15 * k + 6))
                    {
                        WeatherLS = Convert.ToSingle(szLS[19]);
                        FXLS = Convert.ToSingle(szLS[20]);
                        FSLS = Convert.ToSingle(szLS[21]);

                    }
                    else if (lineCount == (15 * k + 7))
                    {
                        Tmax24[k] = Convert.ToSingle(szLS[11]);
                        Tmin24[k] = Convert.ToSingle(szLS[12]);
                        float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);

                        if (WeatherLS == WeatherLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        Rain24[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                Rain24[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FXLS == FXLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        FX24[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(FXLS1.ToString()))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FX24[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FSLS == FSLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        FS24[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FS24[k] = LS1 + "转" + LS2;
                            }
                        }
                    }

                    else if (lineCount == (15 * k + 8))
                    {
                        WeatherLS = Convert.ToSingle(szLS[19]);
                        FXLS = Convert.ToSingle(szLS[20]);
                        FSLS = Convert.ToSingle(szLS[21]);

                    }
                    else if (lineCount == (15 * k + 9))
                    {
                        Tmax48[k] = Convert.ToSingle(szLS[11]);
                        Tmin48[k] = Convert.ToSingle(szLS[12]);
                        float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                        if (WeatherLS == WeatherLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        Rain48[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                Rain48[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FXLS == FXLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        FX48[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(FXLS1.ToString()))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FX48[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FSLS == FSLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        FS48[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FS48[k] = LS1 + "转" + LS2;
                            }
                        }
                    }
                    else if (lineCount == (15 * k + 10))
                    {
                        WeatherLS = Convert.ToSingle(szLS[19]);
                        FXLS = Convert.ToSingle(szLS[20]);
                        FSLS = Convert.ToSingle(szLS[21]);

                    }
                    else if (lineCount == (15 * k + 11))
                    {
                        Tmax72[k] = Convert.ToSingle(szLS[11]);
                        Tmin72[k] = Convert.ToSingle(szLS[12]);
                        float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1 = Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                        if (WeatherLS == WeatherLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        Rain72[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                Rain72[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FXLS == FXLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        FX72[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FXDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (line1.Contains(FXLS.ToString()))
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (line1.Contains(FXLS1.ToString()))
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FX72[k] = LS1 + "转" + LS2;
                            }
                        }
                        if (FSLS == FSLS1)
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        FS72[k] = line1.Split('=')[0];
                                        break;
                                    }

                                }
                            }
                        }
                        else
                        {
                            using (StreamReader sr1 = new StreamReader(FSDZ, Encoding.Default))
                            {
                                string line1 = "";
                                string LS1 = "", LS2 = "";
                                while ((line1 = sr1.ReadLine()) != null)
                                {
                                    if (Convert.ToSingle(line1.Split('=')[1]) == FSLS)
                                    {
                                        LS1 = line1.Split('=')[0];
                                    }
                                    else if (Convert.ToSingle(line1.Split('=')[1]) == FSLS1)
                                    {
                                        LS2 = line1.Split('=')[0];
                                    }

                                }
                                FS72[k] = LS1 + "转" + LS2;
                            }
                        }
                        k++;
                    }

                    lineCount++;
                }
            }
            //该旗县所有乡镇的预报信息已经保存，开始转换为数组
            string[,] zdybSZ = new string[intCount, 14];//每行内容为：旗县名称+区站号+未来三天分别的天气、风向风速、最低气温、最高气温，因此列数为2+4*3
            using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))
            {
                line = sr.ReadToEnd();
            }
            using (StreamReader sr = new StreamReader(QXCPNamepath, Encoding.Default))
            {
                string line1 = "";
                while ((line1 = sr.ReadLine()) != null)
                {
                    string[] szLS = line1.Split('=');
                    if (szLS.Length > 1)
                    {
                        line = line.Replace(szLS[0], szLS[1]);
                    }
                }
            }
            string[] XZSZ = line.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            Int16 QXGS = Convert.ToInt16((XZSZ[0].Split(':'))[1]);
            int szcount = 0;//保存报文导出数组的当前序号
            for (int i = 1; i < QXGS * 2 + 1; i++)
            {
                if (i % 2 == 0)//如果是保存ID的行
                {
                    string[] IDSZLS = XZSZ[i].Split(',');
                    for (int j = 0; j < IDSZLS.Length; j++)
                    {
                        for (int k = 0; k < intCount; k++)
                        {
                            if (StationID[k] == IDSZLS[j])//如果该站点与乡镇文本中索引的当前ID匹配
                            {
                                zdybSZ[szcount, 0] = XZSZ[i - 1].Split(',')[j];//名称为ID行前一行对应的位置
                                zdybSZ[szcount, 1] = StationID[k];
                                zdybSZ[szcount, 2] = Rain24[k];
                                zdybSZ[szcount, 3] = FX24[k] + FS24[k];
                                zdybSZ[szcount, 4] = Tmin24[k].ToString();
                                zdybSZ[szcount, 5] = Tmax24[k].ToString();
                                zdybSZ[szcount, 6] = Rain48[k];
                                zdybSZ[szcount, 7] = FX48[k] + FS48[k];
                                zdybSZ[szcount, 8] = Tmin48[k].ToString();
                                zdybSZ[szcount, 9] = Tmax48[k].ToString();
                                zdybSZ[szcount, 10] = Rain72[k];
                                zdybSZ[szcount, 11] = FX72[k] + FS24[k];
                                zdybSZ[szcount, 12] = Tmin72[k].ToString();
                                zdybSZ[szcount, 13] = Tmax72[k].ToString();
                                szcount++;
                                break;
                            }
                        }
                    }
                }
            }
            DCWord(zdybSZ);

        }

    }//

    public void centerBW2ZDBW()
    {
        string line = "";
        using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
        {


            // 从文件读取并显示行，直到文件的末尾 
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("市局预报读取指导预报时次"))
                {
                    RKTime = line.Substring("市局预报读取指导预报时次=".Length);
                }

            }
        }
        string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
        string sjBWID = "";
        using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
        {
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Split('=')[0] == ("市本级"))
                {
                    sjBWID = line.Split('=')[1];
                    break;
                }
            }
        }
        string strDate = DateTime.Now.ToString("yyyy-MM-dd") + " " + RKTime + ":00:00";
        DateTime dtLS = Convert.ToDateTime(strDate);
        dtLS = dtLS.ToUniversalTime();
        string centerBWName = "Z_SEVP_C_BABJ_" + DateTime.UtcNow.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SCMOC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
        string ZDBWName = "Z_SEVP_C_" + sjBWID + '_' + DateTime.UtcNow.ToString("yyyyMMdd") + "083000" + "_P_RFFC-SPCC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
        string CSBWPathCon = Environment.CurrentDirectory + @"\设置文件\报文保存路径.txt";

        string CSBWPath = "",ZDBWPath="";
        using (StreamReader sr3 = new StreamReader(CSBWPathCon, Encoding.Default))
        {
            while ((line = sr3.ReadLine()) != null)
            {
                if (line.Split('=')[0] == "市局初始报文保存地址")
                {
                    CSBWPath = line.Split('=')[1];
                }
                else if (line.Split('=')[0]== "市局指导预报报文保存地址")
                {
                    ZDBWPath = line.Split('=')[1];
                }
            }
        }
        CSBWPath += centerBWName;//存的未订正的报文的全路径
        
        string strParPath = "*" + sjBWID + "*" + DateTime.UtcNow.ToString("yyyyMMdd") + "*";
        string[] fileNameList = Directory.GetFiles(ZDBWPath, strParPath);
        for(int i=0;i<fileNameList.Length;i++)
        {
            File.Delete(fileNameList[i]);
        }
        ZDBWPath += ZDBWName;
        string YBData = "";
        using (StreamReader sr = new StreamReader(CSBWPath,Encoding.Default))
        {
            YBData = sr.ReadToEnd();
        }
        YBData= YBData.Replace("BABJ",sjBWID);
        YBData = YBData.Replace("SCMOC", "SPCC");
        FileStream fs = new FileStream(ZDBWPath,FileMode.Create);
        StreamWriter sw = new StreamWriter(fs,Encoding.Default);
        sw.Write(YBData);
        sw.Flush();
        sw.Close();
        fs.Close();
    }
}

