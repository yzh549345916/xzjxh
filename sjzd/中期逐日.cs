using System;
using System.IO;
using System.Text;
using System.Windows;

namespace sjzd
{
    class 中期逐日
    {
        public void SJCL(DateTime dt)
        {
            string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            try
            {
                string YBPath = "";

                string ZQPath = "";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "中期逐日预报发布路径")
                        {
                            ZQPath = line.Split('=')[1];
                        }
                        else if (line.Split('=')[0] == "城镇指导预报路径")
                        {
                            YBPath = line.Split('=')[1] + dt.ToString("yyyy") + "\\" + dt.ToString("yy.MM") + "\\20\\呼市气象台指导预报" + dt.ToString("MMdd") + ".txt";
                        }
                    }
                }

                ZQPath += dt.ToString("yyyy") + "\\" + dt.ToString("yyyy-MM") + "\\";
                if (!File.Exists(ZQPath))
                {
                    Directory.CreateDirectory(ZQPath);
                }

                ZQPath += "呼市气象台中期逐日预报" + dt.ToString("MMdd") + ".txt"; ;
                string data = "";
                if (File.Exists(YBPath))
                {
                    using (StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312")))
                    {
                        bool bs = false;
                        string line = "";
                        data = "                  呼和浩特市气象台" + dt.ToString("yyyy年MM月dd日") + "中期逐日预报" + "\r\n\r\n";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains("72--96小时预报"))
                                bs = true;
                            if (bs)
                            {
                                data += line + "\r\n";
                            }
                        }
                    }
                }
                else
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
                            using (StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312")))
                            {
                                bool bs = false;
                                string line = "";
                                data = sr.ReadLine() + "\r\n";
                                while ((line = sr.ReadLine()) != null)
                                {
                                    if (line.Contains("72--96小时预报"))
                                        bs = true;
                                    if (bs)
                                    {
                                        data += line + "\r\n";
                                    }
                                }
                            }
                        }
                    }
                }

                if (data.Trim().Length > 0)
                {
                    //File.WriteAllText(ZQPath, data, Encoding.GetEncoding("GB2312"));
                    try
                    {
                        StreamWriter sw2 = new StreamWriter(ZQPath, false, Encoding.GetEncoding("GB2312"));
                        sw2.Write(data);
                        sw2.Close();
                        MessageBoxResult dr = MessageBox.Show("产品制作完成,保存路径为：\r\n" + ZQPath + "\n是否打开？", "提示", MessageBoxButton.YesNo);
                        if (dr == MessageBoxResult.Yes)
                        {
                            try
                            {
                                静态类.OpenBrowser(ZQPath);

                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }



            }

            catch (Exception)
            {

            }
        }
        public string SJCLNew(ref string path, ref string error)
        {
            DateTime dt = DateTime.Now;
            string configpathPath = System.Environment.CurrentDirectory + @"\设置文件\路径设置.txt";
            try
            {
                string YBPath = "";

                string ZQPath = "";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.GetEncoding("GB2312")))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "中期逐日预报发布路径")
                        {
                            ZQPath = line.Split('=')[1];
                        }
                        else if (line.Split('=')[0] == "城镇指导预报路径")
                        {
                            YBPath = line.Split('=')[1] + dt.ToString("yyyy") + "\\" + dt.ToString("yy.MM") + "\\20\\呼市气象台指导预报" + dt.ToString("MMdd") + ".txt";
                        }
                    }
                }

                ZQPath += dt.ToString("yyyy") + "\\" + dt.ToString("yyyy-MM") + "\\";
                if (!File.Exists(ZQPath))
                {
                    Directory.CreateDirectory(ZQPath);
                }

                ZQPath += "呼市气象台中期逐日预报" + dt.ToString("MMdd") + ".txt";
                path = ZQPath;
                string data = "";
                if (File.Exists(YBPath))
                {
                    using (StreamReader sr = new StreamReader(YBPath, Encoding.GetEncoding("GB2312")))
                    {
                        bool bs = false;
                        string line = "";
                        data = "                  呼和浩特市气象台" + dt.ToString("yyyy年MM月dd日") + "中期逐日预报" + "\r\n\r\n";
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.Contains("72--96小时预报"))
                                bs = true;
                            if (bs)
                            {
                                data += line + "\r\n";
                            }
                        }
                    }
                    return data;
                }
                else
                {
                    error = YBPath + "路径错误，是否手动选择乡镇指导预报文件";
                    return "";

                }





            }

            catch (Exception)
            {

            }
            return "";
        }
    }
}
