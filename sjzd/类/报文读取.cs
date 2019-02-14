using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sjzd
{
    class 报文读取
    {
        string configpathPath = Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
        public string Sjyb(string YBDate, string strTime)
        {
            string strFH = "";
            string line = "";

            string YBpath = "";
            try
            {
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "市台指导区局路径")
                        {
                            YBpath = line.Split('=')[1];
                            break;
                        }
                    }
                }
               
                string BWBS = "";
                using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\报文标识.txt", Encoding.Default))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "呼市气象台")
                        {
                            BWBS = line.Split('=')[1];
                        }
                    }
                }

                string pathYB=DQLB10(YBpath, BWBS, strTime, YBDate);
                if (pathYB.Trim().Length > 0)
                {
                 
                    int lineCount = 0;
                    int intCount = 0;
                    using (StreamReader sr = new StreamReader(pathYB, Encoding.Default))
                    {


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
                    float[] Tmax24 = new float[intCount],
                        Tmin24 = new float[intCount],
                        Tmax48 = new float[intCount],
                        Tmax72 = new float[intCount],
                        Tmin48 = new float[intCount],
                        Tmax96 = new float[intCount],
                        Tmin96 = new float[intCount],
                        Tmax120 = new float[intCount],
                        Tmin120 = new float[intCount],
                        Tmin72 = new float[intCount];
                    string[] StationID = new string[intCount],
                        Rain24 = new string[intCount],
                        Rain48 = new string[intCount],
                        Rain96 = new string[intCount],
                        Rain120 = new string[intCount],
                        Rain72 = new string[intCount];
                    string[] FX24 = new string[intCount],
                        FS24 = new string[intCount],
                        FX48 = new string[intCount],
                        FS48 = new string[intCount],
                        FX96 = new string[intCount],
                        FS96 = new string[intCount],
                        FX120 = new string[intCount],
                        FS120 = new string[intCount],
                        FX72 = new string[intCount],
                        FS72 = new string[intCount];
                    string WeatherDZ = System.Environment.CurrentDirectory + @"\设置文件\天气对照.txt";
                    float WeatherLS = 0, FXLS = 0, FSLS = 0; //保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                    string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
                    string FSDZ = System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
                    using (StreamReader sr = new StreamReader(pathYB, Encoding.Default))
                    {
                        lineCount = 0;
                        int k = 0;
                        while (((line = sr.ReadLine()) != null) && k < intCount) //k代表乡镇的序号
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
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);

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
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
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
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
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

                            }
                            else if (lineCount == (15 * k + 12))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 13))
                            {
                                Tmax96[k] = Convert.ToSingle(szLS[11]);
                                Tmin96[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain96[k] = line1.Split('=')[0];
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
                                        Rain96[k] = LS1 + "转" + LS2;
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
                                                FX96[k] = line1.Split('=')[0];
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
                                        FX96[k] = LS1 + "转" + LS2;
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
                                                FS96[k] = line1.Split('=')[0];
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
                                        FS96[k] = LS1 + "转" + LS2;
                                    }
                                }
                            }
                            else if (lineCount == (15 * k + 14))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if (lineCount == (15 * k + 15))
                            {
                                Tmax120[k] = Convert.ToSingle(szLS[11]);
                                Tmin120[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]),
                                    FXLS1 = Convert.ToSingle(szLS[20]),
                                    FSLS1 = Convert.ToSingle(szLS[21]);
                                if (WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while ((line1 = sr1.ReadLine()) != null)
                                        {
                                            if (line1.Contains(WeatherLS.ToString().PadLeft(2, '0')))
                                            {
                                                Rain120[k] = line1.Split('=')[0];
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
                                        Rain120[k] = LS1 + "转" + LS2;
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
                                                FX120[k] = line1.Split('=')[0];
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
                                        FX120[k] = LS1 + "转" + LS2;
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
                                                FS120[k] = line1.Split('=')[0];
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
                                        FS120[k] = LS1 + "转" + LS2;
                                    }
                                }
                                k++;

                            }
                            lineCount++;

                        }
                    }

                    for (int i = 0; i < intCount; i++)
                    {
                        strFH += StationID[i]+','+ Tmin24[i] + ',' + Tmax24[i] + ',' + Rain24[i] + ',' + FX24[i] + ',' + FS24[i] + ',' +
                                 Tmin48[i] + ',' + Tmax48[i] + ',' + Rain48[i] + ',' + FX48[i] + ',' + FS48[i] + ',' +
                                 Tmin72[i] + ',' + Tmax72[i] + ',' + Rain72[i] + ',' + FX72[i] + ',' + FS72[i] + ',' +
                                 Tmin96[i] + ',' + Tmax96[i] + ',' + Rain96[i] + ',' + FX96[i] + ',' + FS96[i] + ',' +
                                 Tmin120[i] + ',' + Tmax120[i] + ',' + Rain120[i] + ',' + FX120[i] + ',' + FS120[i] +
                                 '\n';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);



                }


            }
            catch (Exception ex)
            {
                
            }
           
            return strFH;


        }
        /// <summary>
        /// 根据目录信息返回文件名，过滤10点报文
        /// </summary>
        /// <param name="qjPath"></param>
        /// <param name="BWBS"></param>
        /// <param name="SC"></param>
        /// <param name="YBDate"></param>
        /// <returns></returns>
        public string DQLB(string qjPath,string BWBS,string SC,string YBDate)
        {
            if (qjPath.Length > 0  && BWBS.Length > 0)
            {
                if (SC == "08")
                {
                    DateTime dt = DateTime.ParseExact(YBDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    qjPath += dt.AddDays(-1).ToString("yyyyMMdd") + "\\";
                    string strParPath = "*" + BWBS + "_" + dt.AddDays(-1).ToString("yyyyMMdd2245") + "*-SPCC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
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
                    try
                    {
                        return fileNameList[maxXH];
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
                else
                {
                    qjPath += YBDate + "\\";
                    string strParPath = "*" + BWBS + "_" + YBDate + "0830" + "*-SPCC-" + YBDate + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
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
                    try
                    {
                        return fileNameList[maxXH];
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

            return "";
        }
        /// <summary>
        /// 根据目录信息返回文件名，不过滤10点报文
        /// </summary>
        /// <param name="qjPath"></param>
        /// <param name="BWBS"></param>
        /// <param name="SC"></param>
        /// <param name="YBDate"></param>
        /// <returns></returns>
        public string DQLB10(string qjPath, string BWBS, string SC, string YBDate)
        {
            string QJPath = qjPath;
            if (qjPath.Length > 0 && BWBS.Length > 0)
            {
                if (SC == "08")
                {
                    DateTime dt = DateTime.ParseExact(YBDate, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                    qjPath = QJPath+ dt.ToString("yyyyMMdd") + "\\";
                    string strParPath = "*" + BWBS + "_" + dt.ToString("yyyyMMdd0230") + "*-SPCC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                    if (!Directory.Exists(qjPath))
                    {
                        qjPath = QJPath + dt.AddDays(-1).ToString("yyyyMMdd") + "\\";
                        strParPath = "*" + BWBS + "_" + dt.AddDays(-1).ToString("yyyyMMdd2245") + "*-SPCC-" + dt.ToString("yyyyMMdd") + "00" + "*";
                    }
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    if (fileNameList.Length <= 0)
                    {
                        qjPath = QJPath + dt.AddDays(-1).ToString("yyyyMMdd") + "\\";
                         strParPath = "*" + BWBS + "_" + dt.AddDays(-1).ToString("yyyyMMdd2245") + "*-SPCC-" + dt.ToString("yyyyMMdd") + "00" + "*";

                         fileNameList = Directory.GetFiles(qjPath, strParPath);
                    }
                   
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
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
                    try
                    {
                        return fileNameList[maxXH];
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
                else
                {
                    qjPath += YBDate + "\\";
                    string strParPath = "*" + BWBS + "_" + YBDate + "0830" + "*-SPCC-" + YBDate + "12" + "*";
                    string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                    int intCount = 0;//记录该报文中的站点数
                    Int16 maxXH = 0, minXH = 0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
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
                    try
                    {
                        return fileNameList[maxXH];
                    }
                    catch (Exception ex)
                    {
                        return "";
                    }
                }
            }

            return "";
        }
    }
}
