using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace QX_update
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        string updatePath = "";
        string pathConfig = System.Environment.CurrentDirectory + @"\config\pathConfig.txt";
        string myLastVersion = "";
        string myVersion = "";
        public MainWindow()
        {
            InitializeComponent();

            Process[] processList = Process.GetProcesses();
            foreach (Process process in processList)
            {
                //如果程序启动了，则杀死
                if (process.ProcessName == "旗县端")
                {
                    process.Kill();
                }
            }

            using (StreamReader sr = new StreamReader(pathConfig, Encoding.Default))
            {
                string line = "";
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "升级文件路径")
                    {
                        updatePath = line.Split('=')[1];
                    }
                }
            }
            string versionPath = System.Environment.CurrentDirectory + @"\设置文件\version.txt";//改

            using (StreamReader sr = new StreamReader(versionPath, Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Split('=')[0] == "myLastVersion")
                    {
                        myLastVersion = line.Trim().Split('=')[1];
                    }
                    else if (line.Split('=')[0] == "myVersion")
                    {
                        myVersion = line.Trim().Split('=')[1];
                    }
                }
            }

            if (backup())
            {
                if (newUpdate())
                {
                    string data = "";
                    try
                    {
                        using (StreamReader sr = new StreamReader(versionPath, Encoding.Default))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                try
                                {
                                    if (line.Split('=')[0] == "myVersion")
                                    {
                                    }
                                    else
                                    {
                                        data += line + "\r\n";
                                    }
                                }
                                catch
                                { }
                            }
                        }
                        data += "myVersion=" + myLastVersion;
                        using (FileStream fs = new FileStream(versionPath, FileMode.Create))
                        {
                            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                            sw.Write(data);
                            sw.Flush();
                            sw.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            this.Close();

        }
        public bool newUpdate()
        {
            try
            {

                string updatePathLocal = updatePath + myLastVersion + "\\";
                if (Directory.Exists(updatePathLocal))
                {
                    try
                    {
                        string[] file = Directory.GetFiles(updatePathLocal, "*", SearchOption.AllDirectories);
                        foreach (string ff in file)
                        {
                            string[] szls = ff.Split('\\');
                            string destinationFile = Environment.CurrentDirectory + '\\' + ff.Substring(updatePathLocal.Length, ff.Length - updatePathLocal.Length);
                            string dirStr = destinationFile.Substring(0, destinationFile.Length - szls[szls.Length - 1].Length);
                            if (!Directory.Exists(dirStr))
                            {
                                Directory.CreateDirectory(dirStr);
                            }
                            File.Copy(ff, destinationFile, true);
                        }
                        MessageBoxResult dr = MessageBox.Show("更新完成,是否重新打开乡镇精细化预报检验平台", "请注意", MessageBoxButton.YesNo);
                        if (dr == MessageBoxResult.Yes)
                        {
                            string dlPath = Environment.CurrentDirectory + @"\旗县端.exe";
                            string strML = Environment.CurrentDirectory;
                            Process pr = new Process();//声明一个进程类对象
                            pr.StartInfo.WorkingDirectory = strML;
                            pr.StartInfo.FileName = dlPath;//指定运行的程序
                            pr.Start();//运行
                        }
                        return true;
                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                        return false;
                    }

                }
                else
                {
                    MessageBox.Show("请确认升级文件的路径是否正确配置，配置文件在" + pathConfig + "中。");
                    return false;
                    this.Close();

                }

            }
            catch
            {
                return false;
            }
        }

        public void oldUpdate()
        {
            if (Directory.Exists(updatePath))
            {
                try
                {
                    string[] file = Directory.GetFiles(updatePath, "*", SearchOption.AllDirectories);
                    foreach (string ff in file)
                    {
                        string[] szls = ff.Split('\\');
                        string destinationFile = Environment.CurrentDirectory + '\\' + ff.Substring(updatePath.Length, ff.Length - updatePath.Length);
                        string dirStr = destinationFile.Substring(0, destinationFile.Length - szls[szls.Length - 1].Length);
                        if (!Directory.Exists(dirStr))
                        {
                            Directory.CreateDirectory(dirStr);
                        }
                        File.Copy(ff, destinationFile, true);
                    }
                    MessageBoxResult dr = MessageBox.Show("更新完成,是否重新打开乡镇精细化预报检验平台", "请注意", MessageBoxButton.YesNo);
                    if (dr == MessageBoxResult.Yes)
                    {
                        string dlPath = Environment.CurrentDirectory + @"\旗县端.exe";
                        string strML = Environment.CurrentDirectory;
                        Process pr = new Process();//声明一个进程类对象
                        pr.StartInfo.WorkingDirectory = strML;
                        pr.StartInfo.FileName = dlPath;//指定运行的程序
                        pr.Start();//运行
                    }

                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                MessageBox.Show("请确认升级文件的路径是否正确配置，配置文件在" + pathConfig + "中。");
                this.Close();

            }
        }
        /// <summary>
        /// 备份文件
        /// </summary>
        public bool backup()
        {
            try
            {
                string srcPath = Environment.CurrentDirectory;
                string destPath = Environment.CurrentDirectory + @"\备份\" + myVersion;
                if (Directory.Exists(destPath))
                {
                    Directory.Delete(destPath, true);
                    Directory.CreateDirectory(destPath);
                    if (CopyDirectory(srcPath, destPath))
                        return true;
                    else
                        return false;
                }
                else
                {
                    Directory.CreateDirectory(destPath);
                    if (CopyDirectory(srcPath, destPath))
                        return true;
                    else
                        return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        public static bool CopyDirectory(string srcPath, string destPath)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(srcPath);
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();  //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo)     //判断是否文件夹
                    {
                        if (i.Name != "备份")//过滤备份文件夹
                        {
                            if (!Directory.Exists(destPath + "\\" + i.Name))
                            {
                                Directory.CreateDirectory(destPath + "\\" + i.Name);   //目标目录下不存在此文件夹即创建子文件夹
                            }
                            CopyDirectory(i.FullName, destPath + "\\" + i.Name);    //递归调用复制子文件夹
                        }
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true);      //不是文件夹即复制文件，true表示可以覆盖同名文件
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
