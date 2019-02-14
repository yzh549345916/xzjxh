using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;


namespace xzjxhyb_DBmain
{
    public class saveQXYB
    {
        //方法输入为待入库的预报文件的日期YYYYMMDD
        public  string  saveQXCS(string YBDate)
        {

            string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
            string configXZPath = System.Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            string YBpath = "";
            string line;
            int JLGS = 0, SucGS = 0;//统计应该入库的记录总个数与成功入库的个数.
            using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
            {

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("乡镇精细化预报报文路径"))
                    {
                        YBpath = line.Substring("乡镇精细化预报报文路径=".Length);
                    }                  
                }
            }
            using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//获取旗县的个数
            {
                line= sr.ReadLine();
            }
            string[] linShi1 = line.Split(':');
            int intQXGS = Convert.ToInt32(linShi1[1]);
            string[] szQXID = new string[intQXGS];
            int lineCount=0,i = 0;
            using (StreamReader sr = new StreamReader(configXZPath, Encoding.Default))//将各旗县的ID保存到数组中

            {
                while (i < intQXGS)
                {
                    line = sr.ReadLine();
                    if ((2 * i +2) == lineCount)
                    {
                        szQXID[i++] = line.Split(',')[0];
                    }
                    lineCount++;

                }
            }
            //根据乡镇列表与站号报文缩写对照表将报文名称缩写与对应的区站号保存到二维数组
            string[,] szQXBWID = new string[intQXGS,2];
            using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
            {
                lineCount = 0;
                while ((line = sr.ReadLine()) != null)
                {
                    for(int j =0;j<intQXGS;j++)
                    {
                        if(line.Contains(szQXID[j]))
                        {
                            szQXBWID[lineCount,1]= line.Substring((szQXID[j]+'=').Length);
                            szQXBWID[lineCount++, 0] = szQXID[j];
                        }
                    }
                }
            }

            for(i=0;i<intQXGS;i++)
            {
                int intCount = 0;//记录该旗县乡镇个数
                string strParPath = "*" + szQXBWID[i, 1] + "*" + YBDate + "*";
                string[] fileNameList = Directory.GetFiles(YBpath, strParPath);
                if(fileNameList.Length>0)
                {
                    Int16 maxXH=0, minXH=0;//maxXH与minXH记录最大最小（即最晚的更正报和最早的报文的文件名在该天该旗县的所有报文列表中的序号）
                    Int16 maxLS = 0, minLS = 99, intLS;//maxLS与minLS保存报文名称和更正报次数有关的两位数字。分别为最晚与最早的
                    //寻找指定日期中该旗县的最晚和最早报文在fileNameList文件列表中的序号，最晚报文的全路径的为fileNameList[maxXH]，最早的为fileNameList[minXH]
                    for (Int16 j =0;j<fileNameList.Length;j++)
                    {
                        string strLS = fileNameList[j].Split('_')[4];
                        intLS = Convert.ToInt16(strLS.Substring(strLS.Length-2));
                        if (j==0)
                        {
                           maxLS=intLS;
                           minLS = intLS;
                        }
                        else
                        {
                            if(intLS > maxLS)
                            {
                                maxLS = intLS;
                                maxXH = j;
                            }
                            if(intLS<minLS)
                            {
                                minLS = intLS;
                                minXH = j;
                            }
                        }
                    }
                    
                    string FQXID = szQXBWID[i,0];
                    DateTime YBtime = File.GetLastWriteTime(fileNameList[minXH]);
                    using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                    {
                        lineCount = 0;
                        
                        while ((line = sr.ReadLine()) != null)
                        {
                            if(lineCount==4)
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
                    float WeatherLS = 0,FXLS=0,FSLS=0;//保存天气、风向、风速的编码临时信息，为了判断前12小时和后12小时的天气是否一致
                    string FXDZ = System.Environment.CurrentDirectory + @"\设置文件\风向对照.txt";
                    string FSDZ= System.Environment.CurrentDirectory + @"\设置文件\风速对照.txt";
                    using (StreamReader sr = new StreamReader(fileNameList[maxXH], Encoding.Default))
                    {
                        
                        lineCount = 0;
                        int k = 0;
                        while (((line = sr.ReadLine()) != null)&&k<intCount)//k代表乡镇的序号
                        {
                            string[] szLS = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if(lineCount==(15*k+5))
                            {
                                StationID[k] = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                            }
                            else if(lineCount==(15*k+6))
                            {
                                WeatherLS = Convert.ToSingle(szLS[19]);
                                FXLS = Convert.ToSingle(szLS[20]);
                                FSLS = Convert.ToSingle(szLS[21]);

                            }
                            else if(lineCount == (15 * k + 7))
                            {
                                Tmax24[k] = Convert.ToSingle(szLS[11]);
                                Tmin24[k] = Convert.ToSingle(szLS[12]);
                                float WeatherLS1 = Convert.ToSingle(szLS[19]), FXLS1= Convert.ToSingle(szLS[20]), FSLS1 = Convert.ToSingle(szLS[21]);
                               
                                if(WeatherLS == WeatherLS1)
                                {
                                    using (StreamReader sr1 = new StreamReader(WeatherDZ, Encoding.Default))
                                    {
                                        string line1 = "";
                                        while((line1=sr1.ReadLine())!=null)
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
                                                LS1= line1.Split('=')[0];
                                            }
                                            else if(line1.Contains(WeatherLS1.ToString().PadLeft(2, '0')))
                                            {
                                                LS2= line1.Split('=')[0];
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
                                            if (Convert.ToSingle(line1.Split('=')[1])==FSLS)
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
                                            if (Convert.ToSingle(line1.Split('=')[1])==FSLS)
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
                                JLGS++;
                            }

                            lineCount++;

                        }
                    }

                    //该旗县所有乡镇的预报信息已经保存，开始保存至数据库
                    string con="";
                    using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
                    {
                        string line1;

                        // 从文件读取并显示行，直到文件的末尾 
                        while ((line1 = sr.ReadLine()) != null)
                        {
                            if (line1.Contains("sql管理员"))
                            {
                                con = line1.Substring("sql管理员=".Length);
                                break;
                            }
                        }
                    }
                    using (SqlConnection mycon = new SqlConnection(con))
                    {
                        mycon.Open();
                        string myDate = YBDate.Substring(0, 4) + '-' + YBDate.Substring(4, 2) + '-' + YBDate.Substring(6, 2);
                        for (int j = 0; j < intCount; j++)
                        {
                            string sql = string.Format(@"insert into QXYB values('{0}','{1}','{2}','','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}')", StationID[j], myDate, FQXID, Tmax24[j], Tmin24[j],Rain24[j],FX24[j],FS24[j], Tmax48[j], Tmin48[j], Rain48[j],FX48[j],FS48[j], Tmax72[j], Tmin72[j], Rain72[j],FX72[j],FS72[j], YBtime);
                            try
                            {
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                sqlman.ExecuteNonQuery();                            //执行数据库语句并返回一个int值（受影响的行数）  
                                SucGS++;
                            }
                            catch(Exception ex)
                            {

                            }
                        }

                    }                      

                }

                string[] cutToPath = new string[fileNameList.Length];
                for(int j =0;j<fileNameList.Length;j++)
                {
                    string[] szLS = fileNameList[j].Split('\\');
                    if(!Directory.Exists(YBpath + @"已入库\"))
                    {
                        Directory.CreateDirectory(YBpath + @"已入库\");
                    }
                    cutToPath[j] = YBpath +@"已入库\"+szLS[szLS.Length-1];
                    File.Move(fileNameList[j],cutToPath[j]);
                }
                
            }
            string ss = string.Format("共计{0}条记录，成功入库{1}条记录。",JLGS,SucGS);
            return '\n'+DateTime.Now.ToString()+"保存"+YBDate+"旗县预报至数据库"+','+ss;

        }

    }
}