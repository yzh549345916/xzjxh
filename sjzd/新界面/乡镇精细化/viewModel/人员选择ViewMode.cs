using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Telerik.Windows.Controls;

namespace sjzd.新界面.乡镇精细化.viewModel
{
    class 人员选择ViewMode : ViewModelBase
    {
        private ObservableCollection<人员类> agencies;

        public ObservableCollection<人员类> Agencies
        {
            get
            {
                if (agencies == null)
                {
                    agencies = new ObservableCollection<人员类>();
                    string PeopleConfig = System.Environment.CurrentDirectory + @"\设置文件\市四区\值班人员.txt";
                    int intCount = 0;
                    try
                    {
                        using (StreamReader sr = new StreamReader(PeopleConfig, Encoding.Default))
                        {
                            string line = "";
                            while ((line = sr.ReadLine()) != null)
                            {
                                try
                                {
                                    if (line.Length > 0)
                                    {
                                        agencies.Add(new 人员类(line.Split('=')[0], intCount++));

                                    }
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }

                    }
                    catch
                    {

                    }
                }
                return agencies;
            }
        }
    }
}
