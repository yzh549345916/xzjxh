using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace xzjxhyb_DBmain
{
    class 城镇20转08
    {
        private string configpathPath = Environment.CurrentDirectory + @"\设置文件\路径设置.txt";

        public void ZDSZ2BW()
        {
            DateTime date08 = DateTime.Now.Date;
            DateTime date20 = date08.AddDays(-1);
            string YBpath = "";
            
            string path08 = "";
            using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "市台指导区局路径")
                    {
                        YBpath = line.Split('=')[1];
                    }else if (line.Split('=')[0] == "市台指导市台路径")
                    {
                        path08 = line.Split('=')[1];
                    }
                }
            }
            string BWBS = "";
            using (StreamReader sr = new StreamReader(Environment.CurrentDirectory + @"\设置文件\报文标识.txt", Encoding.GetEncoding("GB2312")))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "呼市气象台")
                    {
                        BWBS = line.Split('=')[1];
                    }
                }
            }
            string YBDate = date20.ToString("yyyyMMdd");
            string Path20= 获取20时预报路径(YBpath, BWBS, YBDate);
            if (Path20.Length > 0)
            {
                path08 += "Z_SEVP_C_BFNM_" + date08.ToString("yyyyMMdd") + "020501" + "_P_RFFC-SPCC-" + date08.ToString("yyyyMMddHHmm") + "-16812.TXT";//
                string data = "";
                using (StreamReader sr = new StreamReader(Path20, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    int countLine = 0;
                    
                    
                    string[] data20=sr.ReadToEnd().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int countStation = Convert.ToInt32(data20[4].Trim());
                    data = $"{data20[0]}\r\n{data20[1].Replace(date20.ToString("dd0830"), date08.ToString("dd2245"))}\r\n{data20[2].Replace(date20.ToString("yyyyMMdd12"), date08.ToString("yyyyMMdd00"))}\r\n{data20[3].Replace(date20.ToString("yyyyMMdd12"), date08.ToString("yyyyMMdd00"))}\r\n{data20[4]}\r\n";
                    for (int i = 5; i < data20.Length; i++)
                    {
                        int i1 = (i - 5) % 15;
                        if (i1 > 0)
                        {
                            string[] szLS1 = data20[i].Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
                            string tq1 = szLS1[19];
                            string fx1 = szLS1[20];
                            string fs1 = szLS1[21];
                            if (i1 < 14)
                            {
                                string[] szLS2 = data20[i+1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                tq1 = szLS2[19];
                                fx1 = szLS2[20]; 
                                fs1 = szLS2[21];
                            }

                            if (i1 % 2 == 1)
                            {
                                for (int j = 0; j < 19; j++)
                                {
                                    data += szLS1[j] + " ";
                                }

                                data += $"{tq1} {fx1} {fs1}\r\n";
                            }
                            else
                            {
                                string tmax = szLS1[11];
                                string tmin = szLS1[12];
                                if (i1 < 14)
                                {
                                    string[] szLS2 = data20[i + 1].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                    tq1 = szLS2[19];
                                    fx1 = szLS2[20];
                                    fs1 = szLS2[21];
                                    if (i1 < 14)
                                    {
                                        tmin = data20[i + 2].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)[12];
                                    }
                                }
                               
                                for (int j = 0; j < 11; j++)
                                {
                                    data += szLS1[j] + " ";
                                }
                                data += $"{tmax} {tmin} 999.9 999.9 999.9 999.9 999.9 999.9 {tq1} {fx1} {fs1}\r\n";
                            }
                        }
                        else
                        {
                            data += data20[i] + "\r\n";
                        }
                    }
                    FileStream fs = new FileStream(path08, FileMode.Create);
                    StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("GB2312"));
                    sw.Write(data);
                    sw.Flush();
                    sw.Close();
                    fs.Close();

                }
            }


        }
        public string 获取20时预报路径(string qjPath, string BWBS, string YBDate)
        {
            string QJPath = qjPath;
            if (qjPath.Length > 0 && BWBS.Length > 0)
            {
                qjPath += YBDate + "\\";
                string strParPath = "*" + BWBS + "_" + YBDate + "0830" + "*-SPCC-" + YBDate + "12" + "*";
                string[] fileNameList = Directory.GetFiles(qjPath, strParPath);
                short maxXH = 0, minXH = 0; //maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                short maxLS = 0, minLS = 99, intLS; //maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                for (short j = 0; j < fileNameList.Length; j++)
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
                catch (Exception)
                {
                    return "";
                }
            }

            return "";
        }
    }
}
