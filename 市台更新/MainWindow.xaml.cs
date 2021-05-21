using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace 市台更新
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string updatePath = "";
        string myVersion = "";
        string myLastVersion = "";

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                Process[] processList = Process.GetProcesses();
                foreach (Process process in processList)
                {
                    //如果程序启动了，则杀死
                    if (process.ProcessName == "sjzd")
                    {
                        process.Kill();
                    }
                }

                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                updatePath = util.Read("version", "updatePath");
                myVersion = util.Read("version", "myVersion");
                string versionList = util.Read("version", "versionList");
                foreach (string ver in versionList.Split(','))
                {
                    myLastVersion = ver;
                    if (backup())
                    {
                        if (newUpdate())
                        {
                            util.Write(myLastVersion, "version", "myVersion");
                        }
                    }
                }

                MessageBoxResult dr = MessageBox.Show("更新完成,是否重新打开平台", "请注意", MessageBoxButton.YesNo);
                if (dr == MessageBoxResult.Yes)
                {
                    string dlPath = Environment.CurrentDirectory + @"\sjzd.exe";
                    string strML = Environment.CurrentDirectory;
                    Process pr = new Process(); //声明一个进程类对象
                    pr.StartInfo.WorkingDirectory = strML;
                    pr.StartInfo.FileName = dlPath; //指定运行的程序
                    pr.Start(); //运行
                }
            }
            catch
            {
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
                            string destinationFile = Environment.CurrentDirectory + '\\' +
                                                     ff.Substring(updatePathLocal.Length,
                                                         ff.Length - updatePathLocal.Length);
                            string dirStr = destinationFile.Substring(0,
                                destinationFile.Length - szls[szls.Length - 1].Length);
                            if (!Directory.Exists(dirStr))
                            {
                                Directory.CreateDirectory(dirStr);
                            }

                            FileInfo fileInfo = new FileInfo(ff);
                            if (fileInfo.Name == "新界面.txt")
                            {
                                更新参数(fileInfo.FullName);
                            }
                            else
                            {
                                File.Copy(ff, destinationFile, true);
                            }
                           
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
                    MessageBox.Show("请确认升级文件的路径是否正确配置。");
                    return false;
                    this.Close();
                }
            }
            catch
            {
                return false;
            }
        }

        private void 更新参数(string path)
        {
            try
            {
                XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
                using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
                {
                    string line;
                    // 从文件读取并显示行，直到文件的末尾 
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Trim().Length > 0)
                        {
                            string[] szls1 = line.Split(',');
                            if (szls1.Length > 1)
                            {
                                util.Write(szls1[0], szls1.Skip(1).Take(szls1.Length - 1).ToArray());
                            }
                        }
                       
                    }
                }
            }
            catch
            {
            }
        }
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
                FileSystemInfo[] fileinfo = dir.GetFileSystemInfos(); //获取目录下（不包含子目录）的文件和子目录
                foreach (FileSystemInfo i in fileinfo)
                {
                    if (i is DirectoryInfo) //判断是否文件夹
                    {
                        if (i.Name != "备份") //过滤备份文件夹
                        {
                            if (!Directory.Exists(destPath + "\\" + i.Name))
                            {
                                Directory.CreateDirectory(destPath + "\\" + i.Name); //目标目录下不存在此文件夹即创建子文件夹
                            }

                            CopyDirectory(i.FullName, destPath + "\\" + i.Name); //递归调用复制子文件夹
                        }
                    }
                    else
                    {
                        File.Copy(i.FullName, destPath + "\\" + i.Name, true); //不是文件夹即复制文件，true表示可以覆盖同名文件
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