using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace LS
{



    public class errorcode
    {

        string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
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
                    if (line.Contains("实况时次"))
                    {
                        RKTime = line.Substring("实况时次=".Length);
                    }

                }
            }

            string strDate = DateTime.Now.ToString("yyyy-MM-dd") + " " + RKTime + ":00:00";
            DateTime dtLS = Convert.ToDateTime(strDate);
            dtLS = dtLS.ToUniversalTime();
            string BWName = "Z_SEVP_C_BABJ_" + DateTime.UtcNow.ToString("yyyyMMddhhmm") + "00" + "_P_RFFC-SCMOC-" + dtLS.ToString("yyyyMMddhhmm") + "-16812.TXT";//因为初始市局未订正的报文使用的中央的报文格式，所以报文缩写为北京BABJ
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
                                float jd = (Convert.ToSingle(szls[2])) / 100, wd = (Convert.ToSingle(szls[1])) / 100, gd = (Convert.ToSingle(szls[6])) / 10;

                                BWData += ID + jd.ToString().PadLeft(8) + wd.ToString().PadLeft(8) + gd.ToString().PadLeft(8) + " 14 21\r\n";

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
                                    if (line1.Contains(weatherLS1))
                                    {
                                        weatherLS1 = line1.Split('=')[1];
                                        weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                    }
                                    else if (line1.Contains(weatherLS2))
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
                                    if (line1.Contains(weatherLS1))
                                    {
                                        weatherLS1 = line1.Split('=')[1];
                                        weatherLS1 = (Convert.ToSingle(weatherLS1)).ToString("f1").PadLeft(6);
                                    }

                                }
                            }
                            weatherLS2 = weatherLS1;
                        }
                        BWData += weatherLS1;
                        if (!(FXFS[intLS2].Contains('风')) && !(FXFS[intLS2].Contains('级')))
                        {
                            FXLS1 = ""; FSLS1 = ""; FXLS2 = ""; FSLS2 = "";
                            FXLS1 = 999.9.ToString("f1").PadLeft(6);
                            FXLS2 = FXLS1;
                            FSLS1 = 999.9.ToString("f1").PadLeft(6);
                            FSLS2 = FXLS1;
                            intLS2++;
                        }//判断该时次风的内容是否为空
                        else
                        {
                            if (FXFS[intLS2].Contains('级'))//如果包含风速内容
                            {
                                string[] fxfsStr = FXFS[intLS2++].Split('风');
                                FXLS1 = ""; FSLS1 = ""; FXLS2 = ""; FSLS2 = "";
                                if (fxfsStr.Length == 2)
                                {
                                    FXLS1 = fxfsStr[0] + '风';
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
                                else if (fxfsStr.Length == 3)
                                {

                                    FXLS1 = fxfsStr[0] + '风';
                                    FXLS2 = fxfsStr[1].Replace("转", "") + '风';
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
                                else if (fxfsStr.Length == 1)
                                {
                                    FXLS1 = 999.9.ToString("f1").PadLeft(6);
                                    FXLS2 = FXLS1;
                                }
                                string[] fssz = fxfsStr[fxfsStr.Length - 1].Split('转');
                                if (fssz.Length == 1)
                                {
                                    FSLS1 = fssz[0];
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
                                else if (fssz.Length == 2)
                                {

                                    FSLS1 = fssz[0];
                                    FSLS2 = fssz[1];
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
                            else
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


        }
    }
}
