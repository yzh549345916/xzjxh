using Aspose.Cells;
using sjzd.新界面.乡镇精细化.viewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using sjzd.天擎;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.MultiColumnComboBox;
using Telerik.Windows.Controls.Navigation;
using Telerik.Windows.Controls.SplashScreen;
using Telerik.Windows.Data;
using Style = Aspose.Cells.Style;
using sjzd.类;

namespace sjzd
{
    /// <summary>
    /// 个人评分页.xaml 的交互逻辑
    /// </summary>
    public partial class 天擎获取实况 : UserControl
    {
        private SplashScreenDataContext splashScreenDataContext;
        private string 天擎资料代码 = "SURF_CHN_MUL_DAY";
        private TimeSpan myTimespan = TimeSpan.FromDays(1);
        ObservableCollection<天擎数据名称列表> datas = new ObservableCollection<天擎数据名称列表>();
        private DateTime sdate = new DateTime(), edate = new DateTime();
        private string myIDStr = "", elements = "";
        private string dataType = "";
        private string strPath = "";
        private bool 拆分标识 = false;
        private int 拆分循环次数 = 1;
        private bool 是否站点拆分 = false;
        private StringToDisplay callDuration = new StringToDisplay();
        public 天擎获取实况()
        {
            InitializeComponent();
            t1.DataContext = callDuration;
            GridViewItemsSourceProvider gridItem = new GridViewItemsSourceProvider();
            gridItem.ItemsSource = datas;
            dataSelect.ItemsSourceProvider = gridItem;
            splashScreenDataContext = RadSplashScreenManager.SplashScreenDataContext as SplashScreenDataContext;
            splashScreenDataContext.IsIndeterminate = true;
            splashScreenDataContext.Content = "正在加载数据";
            splashScreenDataContext.Footer = "Copyright © 2020 杨泽华, All rights reserved";

        }
        private ObservableCollection<天擎数据名称列表> 小时数据列表初始化()
        {
            string[] codes =
                "Cnty,Station_Name,NetCode,Admin_Code_CHN,Country,City,Town_code,V_ACODE_4SEARCH,REGIONCODE,D_SOURCE_ID,COUNTRYCODE,Station_levl,Province,Town,Station_Id_C,Lat,Lon,Alti,DATA_ID,IYMDHM,RYMDHM,UPDATE_TIME,Datetime,Year,Mon,Day,Hour,Station_Id_d,PRS_Sensor_Alti,WIN_S_Sensor_Heigh,TEM_RHU_Sensor_Heigh,VIS_Sensor_Heigh,Station_Type,V08010,V02183,REP_CORR_ID,PRS,PRS_Sea,PRS_Change_3h,PRS_Change_24h,PRS_Max,PRS_Max_OTime,PRS_Min,PRS_Min_OTime,TEM,TEM_Max,TEM_Max_OTime,TEM_Min,TEM_Min_OTime,TEM_ChANGE_24h,TEM_Max_24h,TEM_Min_24h,DPT,RHU,RHU_Min,RHU_Min_OTIME,VAP,PRE_1h,PRE_3h,PRE_6h,PRE_12h,PRE_24h,PRE_Arti_Enc_CYC,PRE,EVP_Big,WIN_D_Avg_2mi,WIN_S_Avg_2mi,WIN_D_Avg_10mi,WIN_S_Avg_10mi,WIN_D_S_Max,WIN_S_Max,WIN_S_Max_OTime,WIN_D_INST,WIN_S_INST,WIN_D_INST_Max,WIN_S_Inst_Max,WIN_S_INST_Max_OTime,WIN_D_Inst_Max_6h,WIN_S_Inst_Max_6h,WIN_D_Inst_Max_12h,WIN_S_Inst_Max_12h,GST,GST_Max,GST_Max_Otime,GST_Min,GST_Min_OTime,GST_Min_12h,GST_5cm,GST_10cm,GST_15cm,GST_20cm,GST_40Cm,GST_80cm,GST_160cm,GST_320cm,LGST,LGST_Max,LGST_Max_OTime,LGST_Min,LGST_Min_OTime,VIS_HOR_1MI,VIS_HOR_10MI,VIS_Min,VIS_Min_OTime,VIS,CLO_Cov,CLO_Cov_Low,CLO_COV_LM,CLO_Height_LoM,CLO_FOME_1,CLO_Fome_2,CLO_Fome_3,CLO_Fome_4,CLO_FOME_5,CLO_FOME_6,CLO_FOME_7,CLO_Fome_8,CLO_Fome_Low,CLO_FOME_MID,CLO_Fome_High,WEP_Now,WEP_Past_CYC,WEP_Past_1,WEP_Past_2,SCO,Snow_Depth,Snow_PRS,FRS_1st_Top,FRS_1st_Bot,FRS_2nd_Top,FRS_2nd_Bot,Q_PRS,Q_PRS_Sea,Q_PRS_Change_3h,Q_PRS_Change_24h,Q_PRS_Max,Q_PRS_Max_OTime,Q_PRS_Min,Q_PRS_Min_OTime,Q_TEM,Q_TEM_Max,Q_TEM_Max_OTime,Q_TEM_Min,Q_TEM_Min_OTime,Q_TEM_ChANGE_24h,Q_TEM_Max_24h,Q_TEM_Min_24h,Q_DPT,Q_RHU,Q_RHU_Min,Q_RHU_Min_OTIME,Q_VAP,Q_PRE_1h,Q_PRE_3h,Q_PRE_6h,Q_PRE_12h,Q_PRE_24h,Q_PRE_Arti_Enc_CYC,Q_PRE,Q_EVP_Big,Q_WIN_D_Avg_2mi,Q_WIN_S_Avg_2mi,Q_WIN_D_Avg_10mi,Q_WIN_S_Avg_10mi,Q_WIN_D_S_Max,Q_WIN_S_Max,Q_WIN_S_Max_OTime,Q_WIN_D_INST,Q_WIN_S_INST,Q_WIN_D_INST_Max,Q_WIN_S_Inst_Max,Q_WIN_S_INST_Max_OTime,Q_WIN_D_Inst_Max_6h,Q_WIN_S_Inst_Max_6h,Q_WIN_D_Inst_Max_12h,Q_WIN_S_Inst_Max_12h,Q_GST,Q_GST_Max,Q_GST_Max_Otime,Q_GST_Min,Q_GST_Min_OTime,Q_GST_Min_12h,Q_GST_5cm,Q_GST_10cm,Q_GST_15cm,Q_GST_20cm,Q_GST_40Cm,Q_GST_80cm,Q_GST_160cm,Q_GST_320cm,Q_LGST,Q_LGST_Max,Q_LGST_Max_OTime,Q_LGST_Min,Q_LGST_Min_OTime,Q_VIS_HOR_1MI,Q_VIS_HOR_10MI,Q_VIS_Min,Q_VIS_Min_OTime,Q_VIS,Q_CLO_Cov,Q_CLO_Cov_Low,Q_CLO_COV_LM,Q_CLO_Height_LoM,Q_CLO_FOME_1,Q_CLO_Fome_2,Q_CLO_Fome_3,Q_CLO_Fome_4,Q_CLO_FOME_5,Q_CLO_FOME_6,Q_CLO_FOME_7,Q_CLO_Fome_8,Q_CLO_Fome_Low,Q_CLO_FOME_MID,Q_CLO_Fome_High,Q_WEP_Now,Q_WEP_Past_CYC,Q_WEP_Past_1,Q_WEP_Past_2,Q_SCO,Q_Snow_Depth,Q_Snow_PRS,Q_FRS_1st_Top,Q_FRS_1st_Bot,Q_FRS_2nd_Top,Q_FRS_2nd_Bot,RETAIN1,RETAIN2,RETAIN3,V_RETAIN4,V_RETAIN5,V_RETAIN6,V_RETAIN7,V_RETAIN8,V_RETAIN9,V_RETAIN10,Q20214,V02180,V13196,V02176,V02177,V02175,V20214,EICE,EICED_NS,EICET_NS,EICEW_NS,EICED_WE,EICET_WE,EICEW_WE,WIN_D,WIN_S,WEP_Record".Split(',');
            string[] names =
                "区县名,站名,站网代码,行政编码,国家名称,地市名,镇编码,行政编码2,区域代码,数据来源,国家代码,测站级别,省名,乡镇名,区站号(字符),纬度,经度,测站高度,资料标识,入库时间,收到时间,更新时间,资料时间,年,月,日,时,区站号/观测平台标识(数字),气压传感器海拔高度,风速传感器距地面高度,温湿传感器离地面高度,能见度传感器离地面高度,测站类型,地面限定符（温度数据）,云探测系统,更正报标志,气压,海平面气压,3小时变压,24小时变压,最高本站气压,最高本站气压出现时间,最低本站气压,最低本站气压出现时间,温度/气温,最高气温,最高气温出现时间,最低气温,最低气温出现时间,过去24小时变温,过去24小时最高气温,过去24小时最低气温,露点温度,相对湿度,最小相对湿度,最小相对湿度出现时间,水汽压,过去1小时降水量,过去3小时降水量,过去6小时降水量,过去12小时降水量,过去24小时降水量,人工加密观测降水量描述周期,降水量,蒸发(大型),2分钟平均风向(角度),2分钟平均风速,10分钟平均风向(角度),10分钟平均风速,小时内最大风速的风向,最大风速,最大风速出现时间,瞬时风向(角度),瞬时风速,极大风速的风向(角度),极大风速,极大风速出现时间,过去6小时极大瞬时风向,过去6小时极大瞬时风速,过去12小时极大瞬时风向,过去12小时极大瞬时风速,地面温度,最高地面温度,最高地面温度出现时间,最低地面温度,最低地面温度出现时间,过去12小时地面最低温度,5cm地温,10cm地温,15cm地温,20cm地温,40cm地温,80cm地温,160cm地温,320cm地温,草面(雪面)温度,草面(雪面)最高温度,草面(雪面)最高温度出现时间,草面(雪面)最低温度,草面(雪面)最低温度出现时间,1分钟平均能见度,10分钟平均能见度,最小水平能见度,最小水平能见度出现时间,水平能见度(人工),总云量,低云量,云量(低云或中云),云底高度,云状1,云状2,云状3,云状4,云状5,云状6,云状7,云状8,低云状,中云状,高云状,现在天气,过去天气描述事件周期,过去天气1,过去天气2,地面状态,积雪深度,雪压,第一冻土层上界值,第一冻土层下界值,第二冻土层上界值,第二冻土层下界值,气压质控码,海平面气压质量控制标志,3小时变压质控码,24小时变压质控码,日最高本站气压质控码,日最高本站气压出现时间质控码,日最低本站气压质控码,日最低本站气压出现时间质控码,温度/气温质控码,日最高气温质控码,日最高气温出现时间质控码,1小时内最低气温质控码,小时内最低气温出现时间质控码,24小时变温质控码,过去24小时最高气温质控码,过去24小时最低气温质控码,露点温度质控码,相对湿度质控码,最小相对湿度质控码,最小相对湿度出现时间质控码,水汽压质控码,小时降水量质控码,过去3小时降水量质控码,过去6小时降水量质控码,过去12小时降水量质控码,24小时降水量质控码,人工加密观测降水量描述时间周期质控码,分钟降水量质控码,日蒸发量（大型）质控码,2分钟平均风向质控码值,2分钟平均风速成质控码值,10分钟风向质控码,10分钟平均风速质控码,日最大风速的风向质控码,日最大风速质控码,日最大风速出现时间质控码,瞬时风向(角度)质控码,瞬时风速质控码,日极大风速的风向质控码,日极大风速质控码,日极大风速出现时间质控码,过去6小时极大瞬时风向质控码,过去6小时极大瞬时风速质控码,过去12小时极大瞬时风向质控码,过去12小时极大瞬时风速质控码,地面温度质控码,日最高地面温度质控码,日最高地面温度出现时间质控码,日最低地面温度质控码,日最低地面温度出现时间质控码,过去12小时最低地面温度质控码,5cm地温质控码,10cm地温质控码,15cm地温质控码,20cm地温质控码,40cm地温质控码,80cm地温质控码,160cm地温质控码,320cm地温质控码,草面（雪面）温度质控码,日草面（雪面）最高温度质控码,日草面（雪面）最高温度出现时间质控码,日草面（雪面）最低温度质控码,日草面（雪面）最低温度出现时间质控码,1分钟平均水平能见度质控码,10分钟平均水平能见度质控码,日最小水平能见度质控码,日最小水平能见度出现时间质控码,水平能见度质控码,总云量质控码,低云量质控码,低云或中云的云量质控码,云底高度质控码,云状1质控码,云状2质控码,云状3质控码,云状4质控码,云状5质控码,云状6质控码,云状7质控码,云状8质控码,低云状质控码,中云状质控码,高云状质控码,现在天气质控码,过去天气描述时间周期质控码,过去天气1质控码,过去天气2质控码,地面状态质控码,路面雪层厚度质控码,雪压质控码,第一冻土层上界值质控码,第一冻土层下界值质控码,第二冻土层上界值质控码,第二冻土层下界值质控码,保留字段1,保留字段2,保留字段3,保留字段4,保留字段5,保留字段6,保留字段7,保留字段8,保留字段9,保留字段10,冰雹的最大重量质控码,天气现象检测系统,蒸发水位,地面状态测量方法,积雪深度的测量方法,降水测量方法,冰雹的最大重量,电线积冰-现象,电线积冰-南北方向直径,电线积冰-南北方向厚度,电线积冰-南北方向重量,电线积冰-东西方向直径,电线积冰-东西方向厚度,电线积冰-东西方向重量,电线积冰－风向,电线积冰－风速,天气现象记录".Split(',');
            if (codes.Length == names.Length)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    datas.Add(new 天擎数据名称列表()
                    {
                        DataCode = codes[i],
                        DataName = names[i]
                    });
                }
            }
            return datas;
        }
        



        private void DCButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<天擎数据名称列表> selectItems = dataSelect.SelectedItems.OfType<天擎数据名称列表>().ToList();
                elements = "";
                foreach (天擎数据名称列表 selecti in selectItems)
                {
                    elements += selecti.DataCode + ",";
                }

                if (elements.Length > 0)
                {
                    elements = elements.Substring(0, elements.Length - 1);
                }
                时间间隔改变();
                sdate = (DateTime)sDate.SelectedValue;
                edate = (DateTime)eDate.SelectedValue;
                myIDStr = stationsIDInu.Text.Trim();
                if (cfCheck.IsChecked == true)
                {
                    拆分标识 = true;
                    拆分循环次数 = (int)cfNum.Value;
                }
                else
                {
                    拆分标识 = false;
                }

                if (stationCfCheck.IsChecked == true)
                {
                    是否站点拆分 = true;
                }
                else
                {
                    是否站点拆分 = false;
                }
                dataType = LXCom.Text;
                RadOpenFolderDialog openFileDialog = new RadOpenFolderDialog();
                openFileDialog.Owner = this;
                openFileDialog.ShowDialog();
                if (openFileDialog.DialogResult == true)
                {
                    strPath = openFileDialog.FileName ;

                }

                if (!是否站点拆分)
                {
                    Thread th1 = new Thread(拆分);
                    th1.Start();
                }
                else
                {
                    Thread th1 = new Thread(站点拆分);
                    th1.Start();
                }
                
            }
            catch
            {
            }
        }
        public void 拆分()
        {
            if (!拆分标识)
            {
                天擎实况 tqsk = new 天擎实况();
                bool bs = false;
                string myPath = strPath + "\\" + $"{myIDStr}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}" + ".txt";
                for (DateTime dateTimeLs = sdate; dateTimeLs <= edate; dateTimeLs += myTimespan)
                {
                    string data = tqsk.实况数据(天擎资料代码, dateTimeLs, dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)), myIDStr, elements);
                    if (data.Length > 0)
                    {
                        try
                        {
                            if (!bs)
                            {

                                int countLS = data.IndexOf("\n", 0);
                                data = data.Remove(0, countLS + 1);
                                if (File.Exists(myPath))
                                {
                                    File.Delete(myPath);
                                }
                                File.AppendAllText(myPath, data);
                                bs = true;
                            }
                            else
                            {
                                int countLS = data.IndexOf("\n", data.IndexOf("\n") + 1);
                                data = data.Remove(0, countLS + 1);
                                File.AppendAllText(myPath, data);
                            }

                        }
                        catch
                        {
                        }

                        Thread.Sleep(500);
                    }
                    callDuration.Text =$"{DateTime.Now}处理{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}\r\n" + callDuration.Text;
                    Thread.Sleep(500);
                }

                callDuration.Text =
                    $"{DateTime.Now}  {myIDStr}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}处理完成" + callDuration.Text;
            }
            else
            {
                天擎实况 tqsk = new 天擎实况();
                bool bs = false;
                int count = 1;
                string myPath = "";
                for (DateTime dateTimeLs = sdate; dateTimeLs <= edate; dateTimeLs += myTimespan)
                {
                    string data = tqsk.实况数据(天擎资料代码, dateTimeLs, dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)), myIDStr, elements);
                    if (data.Length > 0)
                    {
                        try
                        {
                            
                            if (!bs)
                            { 
                                myPath = strPath + "\\" + $"{myIDStr}站{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs+ myTimespan.Multiply(拆分循环次数).Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}" + ".txt";
                                int countLS = data.IndexOf("\n", 0);
                                data = data.Remove(0, countLS + 1);
                                if (File.Exists(myPath))
                                {
                                    File.Delete(myPath);
                                }
                                File.AppendAllText(myPath, data);
                                bs = true;
                            }
                            else
                            {
                                int countLS = data.IndexOf("\n", data.IndexOf("\n") + 1);
                                data = data.Remove(0, countLS + 1);
                                File.AppendAllText(myPath, data);
                            }
                            if (count++ == 拆分循环次数)
                            {
                                bs = false;
                                count = 1;
                            }
                            else
                            {
                                bs = true;
                            }
                            callDuration.Text = $"{DateTime.Now}处理{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}\r\n" + callDuration.Text;
                        }
                        catch
                        {
                        }

                        Thread.Sleep(500);
                    }
                }
                callDuration.Text =
                    $"{DateTime.Now}  {myIDStr}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}处理完成" + callDuration.Text;
            }
           
        }
        public void 站点拆分()
        {
            if (!拆分标识)
            {
                天擎实况 tqsk = new 天擎实况();
                bool bs = false;
                string[] myids = myIDStr.Split(',');
                foreach (string myid in myids)
                {
                    string myPath = strPath + "\\" + $"{myid}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}" + ".txt";
                    for (DateTime dateTimeLs = sdate; dateTimeLs <= edate; dateTimeLs += myTimespan)
                    {
                        string data = tqsk.实况数据(天擎资料代码, dateTimeLs, dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)), myid, elements);
                        if (data.Length > 0)
                        {
                            try
                            {
                                if (!bs)
                                {

                                    int countLS = data.IndexOf("\n", 0);
                                    data = data.Remove(0, countLS + 1);
                                    if (File.Exists(myPath))
                                    {
                                        File.Delete(myPath);
                                    }
                                    File.AppendAllText(myPath, data);
                                    bs = true;
                                }
                                else
                                {
                                    int countLS = data.IndexOf("\n", data.IndexOf("\n") + 1);
                                    data = data.Remove(0, countLS + 1);
                                    File.AppendAllText(myPath, data);
                                }

                            }
                            catch
                            {
                            }

                            Thread.Sleep(500);
                        }
                        callDuration.Text = $"{DateTime.Now}处理{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}\r\n" + callDuration.Text;
                        Thread.Sleep(500);
                    }

                    callDuration.Text =
                        $"{DateTime.Now}  {myid}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}处理完成" + callDuration.Text;
                }
                callDuration.Text =
                    $"{DateTime.Now}  {sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}所有站点处理完成" + callDuration.Text;


            }
            else
            {
                天擎实况 tqsk = new 天擎实况();
                bool bs = false;
                int count = 1;
                string myPath = "";
                string[] myids = myIDStr.Split(',');
                foreach (string myid in myids)
                {
                    for (DateTime dateTimeLs = sdate; dateTimeLs <= edate; dateTimeLs += myTimespan)
                    {
                        string data = tqsk.实况数据(天擎资料代码, dateTimeLs, dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)), myid, elements);
                        if (data.Length > 0)
                        {
                            try
                            {

                                if (!bs)
                                {
                                    myPath = strPath + "\\" + $"{myid}站{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs + myTimespan.Multiply(拆分循环次数).Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}" + ".txt";
                                    int countLS = data.IndexOf("\n", 0);
                                    data = data.Remove(0, countLS + 1);
                                    if (File.Exists(myPath))
                                    {
                                        File.Delete(myPath);
                                    }
                                    File.AppendAllText(myPath, data);
                                    bs = true;
                                }
                                else
                                {
                                    int countLS = data.IndexOf("\n", data.IndexOf("\n") + 1);
                                    data = data.Remove(0, countLS + 1);
                                    File.AppendAllText(myPath, data);
                                }
                                if (count++ == 拆分循环次数)
                                {
                                    bs = false;
                                    count = 1;
                                }
                                else
                                {
                                    bs = true;
                                }
                                callDuration.Text = $"{DateTime.Now}处理{dateTimeLs:yyyy年MM月dd日HH时}至{dateTimeLs + myTimespan.Add(TimeSpan.FromSeconds(-1)):yyyy年MM月dd日HH时}{dataType}\r\n" + callDuration.Text;
                            }
                            catch
                            {
                            }

                            Thread.Sleep(500);
                        }
                    }
                    callDuration.Text =
                        $"{DateTime.Now}  {myid}站{sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}处理完成" + callDuration.Text;
                }
                callDuration.Text =
                    $"{DateTime.Now}  {sdate:yyyy年MM月dd日HH时}至{edate:yyyy年MM月dd日HH时}{dataType}所有站点处理完成" + callDuration.Text;
            }

        }

        private void LXCom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LXCom.SelectedIndex == 1)
            {
                天擎资料代码 = "SURF_CHN_MUL_DAY";
                datas.Clear();
                datas = 日数据列表初始化();
            }
            else if (LXCom.SelectedIndex == 2)
            {
                天擎资料代码 = "SURF_CHN_MUL_MON";
                datas.Clear();
                datas = 月数据列表初始化();
            }
            else if (LXCom.SelectedIndex == 3)
            {
                天擎资料代码 = "SURF_CHN_MUL_YER";
                datas.Clear();
                datas = 年数据列表初始化();
            }
            else if (LXCom.SelectedIndex == 0)
            {
                天擎资料代码 = "SURF_CHN_MUL_HOR";
                datas.Clear();
                datas = 小时数据列表初始化();
            }
            else if (LXCom.SelectedIndex == 4)
            {
                天擎资料代码 = "SURF_CHN_MUL_MIN";
                datas.Clear();
                datas = 中国地面分钟数据数据列表初始化();
            }
        }


        private void cfCheck_Checked(object sender, RoutedEventArgs e)
        {
            拆分标识 = true;
            cfNum.IsEnabled = true;
        }

        private void cfCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            拆分标识 = false;
            cfNum.IsEnabled = false;
        }

        private void 时间间隔改变()
        {
            if (tpNum.Value.HasValue&& tpCom!=null)
            {
                if (tpCom.SelectedIndex == 0)
                {
                    myTimespan = TimeSpan.FromMinutes((double)tpNum.Value);
                }
                else if (tpCom.SelectedIndex == 1)
                {
                    myTimespan = TimeSpan.FromHours((double)tpNum.Value);
                }
                else if (tpCom.SelectedIndex == 2)
                {
                    myTimespan = TimeSpan.FromDays((double)tpNum.Value);
                }
            }
            
        }
        #region 要素列表初始化
        private ObservableCollection<天擎数据名称列表> 中国地面分钟数据数据列表初始化()
        {
            string[] codes =
                "City,V_ACODE_4SEARCH,Admin_Code_CHN,Province,REGIONCODE,COUNTRYCODE,Town,Station_levl,Country,Cnty,D_SOURCE_ID,Station_Name,Town_code,NetCode,Station_Id_C,Lat,Lon,Alti,D_RETAIN_ID,DATA_ID,IYMDHM,RYMDHM,UPDATE_TIME,Datetime,Year,Mon,Day,Hour,Min,Station_Id_d,Station_Type,REP_CORR_ID,V08010,PRS_Sensor_Alti,VIS_Sensor_Heigh,TEM_RHU_Sensor_Heigh,WIN_S_Sensor_Heigh,V02180,V02183,V02175,V02177,PRS,PRS_Sea,WIN_S_Gust_Max,WIN_D_Gust_Max,WIN_D_INST,WIN_S_INST,WIN_D_Avg_1mi,WIN_S_Avg_1mi,WIN_D_Avg_2mi,WIN_S_Avg_2mi,WIN_D_Avg_10mi,WIN_S_Avg_10mi,WIN_D_S_Max,TEM,DPT,GST_5cm,GST_10cm,GST_15cm,GST_20cm,GST_40Cm,GST_80cm,GST_160cm,GST_320cm,GST,LGST,RHU,VAP,PRE,Snow_Depth,EVP_Big,VIS_HOR_1MI,VIS_HOR_10MI,CLO_Cov,CLO_Height_LoM,NPRE_Conti_Max_EDay_Year,Q_PRS,Q_PRS_Sea,Q_WIN_D_INST,Q_WIN_S_INST,Q_WIN_D_Avg_1mi,Q_WIN_S_Avg_1mi,Q_WIN_D_Avg_2mi,Q_WIN_S_Avg_2mi,Q_WIN_D_Avg_10mi,Q_WIN_S_Avg_10mi,Q_WIN_D_S_Max,Q_TEM,Q_DPT,Q_GST_5cm,Q_GST_10cm,Q_GST_15cm,Q_GST_20cm,Q_GST_40Cm,Q_GST_80cm,Q_GST_160cm,Q_GST_320cm,Q_GST,Q_LGST,Q_RHU,Q_VAP,Q_PRE,Q_Snow_Depth,Q_VIS_HOR_1MI,Q_VIS_HOR_10MI,Q_CLO_Cov,Q_CLO_Height_LoM".Split(',');
            string[] names =
                "地市名,行政编码2,行政编码,省名,区域代码,国家代码,乡镇名,测站级别,国家名称,区县名,数据来源,站名,镇编码,站网代码,区站号(字符),纬度,经度,测站高度,记录标识,资料标识,入库时间,收到时间,更新时间,资料时间,年,月,日,时,分,区站号/观测平台标识(数字),测站类型,更正报标志,地面限定符（温度数据）,气压传感器海拔高度,能见度传感器离地面高度,温湿传感器离地面高度,风速传感器距地面高度,主要天气现况检测系统,云探测系统,降水测量方法,积雪深度的测量方法,气压,海平面气压,最大阵风风速,最大阵风风向,瞬时风向(角度),瞬时风速,1分钟平均风向,1分钟平均风速,2分钟平均风向(角度),2分钟平均风速,10分钟平均风向(角度),10分钟平均风速,小时内最大风速的风向,温度/气温,露点温度,5cm地温,10cm地温,15cm地温,20cm地温,40cm地温,80cm地温,160cm地温,320cm地温,地面温度,草面(雪面)温度,相对湿度,水汽压,降水量,积雪深度,蒸发(大型),1分钟平均能见度,10分钟平均能见度,总云量,云底高度,最长无降水止日年份,气压质控码,海平面气压质量控制标志,瞬时风向(角度)质控码,瞬时风速质控码,1分钟平均风向质控码,1分钟平均风速质控码,2分钟平均风向质控码值,2分钟平均风速成质控码值,10分钟风向质控码,10分钟平均风速质控码,日最大风速的风向质控码,温度/气温质控码,露点温度质控码,5cm地温质控码,10cm地温质控码,15cm地温质控码,20cm地温质控码,40cm地温质控码,80cm地温质控码,160cm地温质控码,320cm地温质控码,地面温度质控码,草面（雪面）温度质控码,相对湿度质控码,水汽压质控码,分钟降水量质控码,路面雪层厚度质控码,1分钟平均水平能见度质控码,10分钟平均水平能见度质控码,总云量质控码,云底高度质控码".Split(',');
            if (codes.Length == names.Length)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    datas.Add(new 天擎数据名称列表()
                    {
                        DataCode = codes[i],
                        DataName = names[i]
                    });
                }
            }
            return datas;
        }
        private ObservableCollection<天擎数据名称列表> 日数据列表初始化()
        {
            string[] codes =
                "Cnty,Station_Name,NetCode,Admin_Code_CHN,Country,City,Town_code,V_ACODE_4SEARCH,REGIONCODE,D_SOURCE_ID,COUNTRYCODE,Station_levl,Province,Town,Station_Id_C,Lat,Lon,Alti,D_RETRIEVAL_TIME_DAY,D_RETAIN_ID,DATA_ID,IYMDHM,RYMDHM,Query_Time,UPDATE_TIME,Datetime,Year,Mon,Day,Station_Id_d,PRS_Sensor_Alti,REP_CORR_ID,PRS_Avg,PRS_Max,PRS_Max_OTime,PRS_Min,PRS_Min_OTime,PRS_Sea_Avg,TEM_Avg,TEM_Max,TEM_Max_OTime,TEM_Min,TEM_Min_OTime,VAP_Avg,RHU_Avg,RHU_Min,RHU_Min_OTIME,CLO_Cov_Avg,CLO_Cov_Low_Avg,WIN_D_S_10min_Hourly,VIS_Min,VIS_Min_OTime,PRE_Max_1h,PRE_Max_1h_OTime,PRE_Time_2008,PRE_Time_0820,PRE_Time_2020,PRE_Time_0808,SPRE_Time_2008,SPRE_Time_0820,SPRE_Time_2020,SPRE_Time_0808,EVP,EVP_Big,Snow_Depth,Snow_PRS,EICE,EICED_NS,EICET_NS,EICEW_NS,EICED_WE,EICET_WE,EICEW_WE,TEM,WIN_D,WIN_S,WIN_D_2mi_Avg_C,WIN_S_2mi_Avg,VIS_Avg_10mi_Hourly,WIN_S_10mi_Avg,WIN_D_S_Max,WIN_S_Max,WIN_S_Max_OTime,WIN_D_INST_Max,WIN_S_Inst_Max,WIN_S_INST_Max_OTime,GST_Avg,GST_Max,GST_Max_Otime,GST_Min,GST_Min_OTime,GST_Avg_5cm,GST_Avg_10cm,GST_Avg_15cm,GST_Avg_20cm,GST_Avg_40cm,GST_Avg_80cm,GST_Avg_160cm,GST_Avg_320cm,FRS_1st_Top,FRS_1st_Bot,FRS_2nd_Top,FRS_2nd_Bot,SSH,Sunrist_Time,Sunset_Time,LGST_Avg,LGST_Max,LGST_Max_OTime,LGST_Min,LGST_Min_OTime,SCO,Rain,PRE_OTime,Snow,Snow_OTime,Hail,HAIL_OTime,Fog,Fog_OTime,Mist,Dew,Frost,Glaze,GLAZE_OTime,SoRi,SoRi_OTime,DrSnow,DrSnow_OTime,SnowSt,SnowSt_OTime,Tord,Tord_OTime,GSS,ICE,SaSt,SaSt_OTime,FlSa,FlSa_OTime,FlDu,FlDu_OTime,Smoke,Haze,DuWhr,IcePri,Thund,THUND_OTime,Lit,Aur,AUR_OTime,GaWIN,GaWIN_OTime,Squa,SQUA_OTime,WEP_Sumary,WEP_Record,Q_PRS_Avg,Q_PRS_Max,Q_PRS_Max_OTime,Q_PRS_Min,Q_PRS_Min_OTime,Q_PRS_Sea_Avg,Q_TEM_Avg,Q_TEM_Max,Q_TEM_Max_OTime,Q_TEM_Min,Q_TEM_Min_OTime,Q_VAP_Avg,Q_RHU_Avg,Q_RHU_Min,Q_RHU_Min_OTIME,Q_CLO_Cov_Avg,Q_CLO_Cov_Low_Avg,Q_VIS_Min,Q_VIS_Min_OTime,Q_PRE_Max_1h,Q_PRE_Max_1h_OTime,Q_PRE_Time_2008,Q_PRE_Time_0820,Q_PRE_Time_2020,Q_PRE_Time_0808,Q_SPRE_Time_2008,Q_SPRE_Time_0820,Q_SPRE_Time_2020,Q_SPRE_Time_0808,Q_EVP,Q_EVP_Big,Q_Snow_Depth,Q_Snow_PRS,Q_EICE,Q_EICED_NS,Q_EICET_NS,Q_EICEW_NS,Q_EICED_WE,Q_EICET_WE,Q_EICEW_WE,Q_TEM,Q_WIN_D,Q_WIN_S,Q_WIN_D_2mi_Avg_C,Q_WIN_S_2mi_Avg,Q_VIS_Avg_10mi_Hourly,Q_WIN_S_10mi_Avg,Q_WIN_D_S_Max,Q_WIN_S_Max,Q_WIN_S_Max_OTime,Q_WIN_D_INST_Max,Q_WIN_S_Inst_Max,Q_WIN_S_INST_Max_OTime,Q_GST_Avg,Q_GST_Max,Q_GST_Max_Otime,Q_GST_Min,Q_GST_Min_OTime,Q_GST_Avg_5cm,Q_GST_Avg_10cm,Q_GST_Avg_15cm,Q_GST_Avg_20cm,Q_GST_Avg_40cm,Q_GST_Avg_80cm,Q_GST_Avg_160cm,Q_GST_Avg_320cm,Q_FRS_1st_Top,Q_FRS_1st_Bot,Q_FRS_2nd_Top,Q_FRS_2nd_Bot,Q_SSH,Q_Sunrist_Time,Q_Sunset_Time,Q_LGST_Avg,Q_LGST_Max,Q_LGST_Max_OTime,Q_LGST_Min,Q_LGST_Min_OTime,Q_SCO,Q_Rain,Q_PRE_OTime,Q_Snow,Q_Snow_OTime,Q_Hail,Q_HAIL_OTime,Q_Fog,Q_Fog_OTime,Q_Mist,Q_Dew,Q_Frost,Q_Glaze,Q_GLAZE_OTime,Q_SoRi,Q_SoRi_OTime,Q_DrSnow,Q_DrSnow_OTime,Q_SnowSt,Q_SnowSt_OTime,Q_Tord,Q_Tord_OTime,Q_GSS,Q_ICE,Q_SaSt,Q_SaSt_OTime,Q_FlSa,Q_FlSa_OTime,Q_FlDu,Q_FlDu_OTime,Q_Smoke,Q_Haze,Q_DuWhr,Q_IcePri,Q_Thund,Q_THUND_OTime,Q_Lit,Q_Aur,Q_AUR_OTime,Q_GaWIN,Q_GaWIN_OTime,Q_Squa,Q_SQUA_OTime,Q_WEP_Sumary,Q_WEP_Record".Split(',');
            string[] names =
                "区县名,站名,站网代码,行政编码,国家名称,地市名,镇编码,行政编码2,区域代码,数据来源,国家代码,测站级别,省名,乡镇名,区站号(字符),纬度,经度,测站高度,日值资料检索时间,记录标识,资料标识,入库时间,收到时间,小时资料检索时间,更新时间,资料时间,年,月,日,区站号/观测平台标识(数字),气压传感器海拔高度,更正报标志,平均气压,最高本站气压,最高本站气压出现时间,最低本站气压,最低本站气压出现时间,平均海平面气压,平均气温,最高气温,最高气温出现时间,最低气温,最低气温出现时间,平均水气压,平均相对湿度,最小相对湿度,最小相对湿度出现时间,平均总云量,平均低云量,逐小时10分钟平均能见度?,最小水平能见度,最小水平能见度出现时间,1小时最大降水量,日小时最大降水量出现时间,20-08时雨量筒观测降水量,08-20时雨量筒观测降水量,20-20时降水量,08-08时降水量,20-08时固态降水量,08-20时固态降水量,20-20时月固态降水量,08-08时月固态降水量,蒸发(小型）,蒸发(大型),积雪深度,雪压,电线积冰-现象,电线积冰-南北方向直径,电线积冰-南北方向厚度,电线积冰-南北方向重量,电线积冰-东西方向直径,电线积冰-东西方向厚度,电线积冰-东西方向重量,温度/气温,风向,风速,2分钟平均风向(角度),平均2分钟风速,逐小时10分钟平均风向风速,平均10分钟风速,日最大风速的风向(角度),最大风速,最大风速出现时间,极大风速的风向(角度),极大风速,极大风速出现时间,平均地面温度,最高地面温度,最高地面温度出现时间,最低地面温度,最低地面温度出现时间,平均5cm地温,平均10cm地温,平均15cm地温,平均20cm地温,平均40cm地温,平均80cm地温,平均160cm地温,平均320cm地温,第一冻土层上界值,第一冻土层下界值,第二冻土层上界值,第二冻土层下界值,日照时数（直接辐射计算值）,日出时间,日落时间,平均草面(雪面)温度,草面(雪面)最高温度,草面(雪面)最高温度出现时间,草面(雪面)最低温度,草面(雪面)最低温度出现时间,地面状态,雨,雨出现时间,雪,雪出现时间,冰雹,冰雹出现时间,雾,雾出现时间,轻雾,露,霜,雨凇,雨凇出现时间,雾凇,雾凇出现时间,吹雪,吹雪出现时间,雪暴,雪暴出现时间,龙卷风,龙卷风出现时间,积雪,结冰,沙尘暴,沙尘暴出现时间,扬沙,扬沙出现时间,浮尘,浮尘出现时间,烟,霾,尘卷风,冰针,雷暴,雷暴出现时间,闪电,极光,极光出现时间,大风,大风出现时间,飑,飑出现时间,天气现象摘要,天气现象记录,日平均本站气压质控码,日最高本站气压质控码,日最高本站气压出现时间质控码,日最低本站气压质控码,日最低本站气压出现时间质控码,日平均海平面气压质控码,日平均气温质控码,日最高气温质控码,日最高气温出现时间质控码,1小时内最低气温质控码,小时内最低气温出现时间质控码,日平均水汽压质控码,日平均相对湿度质控码,最小相对湿度质控码,最小相对湿度出现时间质控码,日平均总云量质控码,日平均低云量质控码,日最小水平能见度质控码,日最小水平能见度出现时间质控码,日小时最大降水量质控码,日小时最大降水量出现时间质控码,20-08时雨量筒观测降水量质控码,08-20时雨量筒观测降水量质控码,20-20时降水量质控码,08-08时降水量质控码,20-08时固态降水量质控码,08-20时固态降水量质控码,20-20时固态降水量质控码,08-08时固态降水量质控码,日蒸发量（小型）质控码,日蒸发量（大型）质控码,路面雪层厚度质控码,雪压质控码,电线积冰-现象质控码,电线积冰-南北方向直径质控码,电线积冰-南北方向厚度质控码,电线积冰-南北方向重量质控码,电线积冰-东西方向直径质控码,电线积冰-东西方向厚度质控码,电线积冰-东西方向重量质控码,温度/气温质控码,瞬时风向质控码,瞬时风速质控码,逐小时2分钟平均风速风向质控码,日平均2分钟风速质控码,逐小时10分钟平均风速风向质控码,日平均10分钟风速质控码,日最大风速的风向质控码,日最大风速质控码,日最大风速出现时间质控码,日极大风速的风向质控码,日极大风速质控码,日极大风速出现时间质控码,日平均地面温度质控码,日最高地面温度质控码,日最高地面温度出现时间质控码,日最低地面温度质控码,日最低地面温度出现时间质控码,日平均5cm地温质控码,日平均10cm地温质控码,日平均15cm地温质控码,日平均20cm地温质控码,日平均40cm地温质控码,日平均80cm地温质控码,日平均160cm地温质控码,日平均320cm地温质控码,第一冻土层上界值质控码,第一冻土层下界值质控码,第二冻土层上界值质控码,第二冻土层下界值质控码,日总日照时数质控码,日出时间质控码,日落时间质控码,日平均草面（雪面）温度质控码,日草面（雪面）最高温度质控码,日草面（雪面）最高温度出现时间质控码,日草面（雪面）最低温度质控码,日草面（雪面）最低温度出现时间质控码,地面状态质控码,雨质控码,雨出现时间质控码,雪质控码,雪出现时间质控码,冰雹质控码,冰雹出现时间质控码,雾质控码,雾出现时间质控码,轻雾质控码,露质控码,霜质控码,雨凇质控码,雨凇出现时间质控码,雾凇质控码,雾凇出现时间质控码,吹雪质控码,吹雪出现时间质控码,雪暴质控码,雪暴出现时间质控码,龙卷风质控码,龙卷风出现时间质控码,积雪质控码,结冰质控码,沙尘暴质控码,沙尘暴出现时间质控码,扬沙质控码,扬沙出现时间质控码,浮尘质控码,浮尘出现时间质控码,烟质控码,霾质控码,尘卷风质控码,冰针质控码,雷暴质控码,雷暴出现时间质控码,闪电质控码,极光质控码,极光出现时间质控码,大风质控码,大风出现时间质控码,飑质控码,飑出现时间质控码,天气现象摘要质控码,天气现象记录质控码".Split(',');
            if (codes.Length == names.Length)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    datas.Add(new 天擎数据名称列表()
                    {
                        DataCode = codes[i],
                        DataName = names[i]
                    });
                }
            }
            return datas;
        }
        private ObservableCollection<天擎数据名称列表> 月数据列表初始化()
        {
            string[] codes =
                "Cnty,Station_Name,NetCode,Admin_Code_CHN,Country,City,Town_code,V_ACODE_4SEARCH,REGIONCODE,D_SOURCE_ID,COUNTRYCODE,Station_levl,Province,Town,Station_Id_C,Lat,Lon,Alti,DATA_ID,IYMDHM,RYMDHM,UPDATE_TIME,Datetime,Year,Mon,Station_Id_d,PRS_Sensor_Alti,PRS_Avg,PRS_Max,PRS_Max_Days,PRS_Max_ODay_C,PRS_Min,PRS_Min_Days,PRS_Min_ODay_C,PRS_Sea_Avg,TEM_Avg,TEM_Max_Avg,TEM_Min_Avg,TEM_Max,TEM_Max_Days,TEM_Max_ODay_C,TEM_Min,TEM_Min_Days,TEM_Min_ODay_C,TEM_Avg_Dev,TEM_Max_Dev,V12304_040,TEM_Dev_Max_ODay_C,TEM_Min_Dev_Mon,TEM_Dev_Min_Days,TEM_Dev_Min_ODay_C,TEM_Max_A30C_Days,TEM_Max_A35C_Days,TEM_Max_A40C_Days,TEM_Min_B2C_Days,TEM_Min_B0C_Days,TEM_Min_Bn2C_Days,TEM_Min_Bn15C_Days,TEM_Min_Bn30C_Days,TEM_Min_Bn40C_Days,TEM_Avg_A26_Days,TEM_Avg_B18_Days,VAP_Avg,RHU_Avg,RHU_Min,RHU_Min_Days,RHU_Min_ODay_C,CLO_Cov_Avg,CLO_Cov_Low_Avg,CLO_Cov_Avg_B2_Days,CLO_Cov_A8_Days,CLO_Low_Cov_Avg_B2_Days,CLO_Cov_Avg_A8_Days,PRE_Time_2020,PRE_Time_0808,SPRE_Time_2020,SPRE_Time_0808,PRE_Max_Day,PRE_Max_Mon_Days,PRE_Max_ODay_C,PRE_A0p1mm_Days,PRE_A1mm_Days,PRE_A5mm_Days,PRE_A10mm_Days,PRE_A25mm_Days,PRE_A50mm_Days,PRE_A100mm_Days,PRE_A150mm_Days,PRE_Day_A250_Days,Days_Max_Coti_PRE,PRE_Conti_Max,EDay_Max_Coti_PRE,NPRE_LCDays,NPRE_LCDays_EDay,PRE_Max_Conti,Days_Max_Conti_PRE,PRE_Coti_Max_EDay,PRE_Max_1h,PRE_Max_1h_Days,PRE_Max_1h_ODay_C,PRE_Days,Hail_Days,Mist_Days,V04330_001,Glaze_Days,SoRi_Days,ICE_Days,FlSa_Days,FlDu_Days,Haze_Days,Tord_Days,GaWIN_Days,SaSt_Days,Fog_Days,Thund_Days,Frost_Days,Snow_Days,GSS_Days,EICE_Days,VIS_B10km_Freq,VIS_B5km_Freq,VIS_B1km_Freq,EVP,EVP_Big,Snow_Depth_Max,V13334_040,V13334_060_C,Snow_PRS,Snow_PRS_Max_Days,V13330_060,Snow_Depth_A1cm_Days,Snow_Depth_A5cm_Days,Snow_Depth_A10cm_Days,Snow_Depth_A20cm_Days,Snow_Depth_A30cm_Days,EICE_Wei_Max,EICEW_Max_Diam,EICEW_Max_Thick,V20440_040,EICEW_Max_ODay_C,WIN_S_2mi_Avg,WIN_S_Max,WIN_D_S_Max_C,Days_WIN_S_Max,WIN_S_Max_ODay_C,WIN_S_A5ms_Days,WIN_S_Max_A10ms_Days,WIN_S_A12ms_Days,V11042_15,WIN_S_A17ms_Days,WIN_S_Inst_Max,WIN_D_INST_Max_C,V11046_040,WIN_S_INST_Max_ODay_C,WIN_NNE_Freq,WIN_NE_Freq,WIN_ENE_Freq,WIN_E_Freq,WIN_ESE_Freq,WIN_SE_Freq,WIN_SSE_Freq,WIN_S_Freq,WIN_SSW_Freq,WIN_SW_Freq,WIN_WSW_Freq,WIN_W_Freq,WIN_WNW_Freq,WIN_NW_Freq,WIN_NNW_Freq,WIN_N_Freq,WIN_C_Freq,WIN_D_Max_C,WIN_D_Max_Freq,WIN_D_SMax_C,WIN_D_SMax_Freq,WIN_S_Avg_NNE,WIN_S_Avg_NE,WIN_S_Avg_ENE,WIN_S_Avg_E,WIN_S_Avg_ESE,WIN_S_Avg_SE,WIN_S_Avg_SSE,WIN_S_Avg_S,WIN_S_Avg_SSW,WIN_S_Avg_SW,WIN_S_Avg_WSW,WIN_S_Avg__W,WIN_S_Avg_WNW,WIN_S_Avg_NW,WIN_S_Avg_NNW,WIN_S_Avg__N,WIN_S_Max_NNE,WIN_S_Max_NE,WIN_S_Max_ENE,WIN_S_Max_E,WIN_S_Max_ESE,WIN_S_Max_SE,WIN_S_Max_SSE,WIN_S_Max_S,WIN_S_Max_SSW,WIN_S_Max_SW,WIN_S_Max_WSW,WIN_S_Max_W,WIN_S_Max_WNW,WIN_S_Max_NW,WIN_S_Max_NNW,WIN_S_Max_N,GST_Avg,EGST_Max_Avg_Mon,GST_Min_Avg,GST_Min_Bel_0_Days,GST_Max,EGST_Max_Days,EGST_Max_ODay_C,GST_Min,Days_GST_Min,GST_Min_Ten_ODay_C,GST_Avg_5cm,GST_Avg_10cm,GST_Avg_15cm,GST_Avg_20cm,GST_Avg_40cm,GST_Avg_80cm,GST_Avg_160cm,GST_Avg_320cm,LGST_Avg,LGST_Max_Avg,LGST_Min_Avg,LGST_Max,LGST_Max_Days,LGST_Max_ODay_C,LGST_Min,LGST_Min_Days,LGST_Min_ODay_C,FRS_Depth_Max,FRS_Depth_Max_Days,FRS_Depth_Max_ODay_C,SSH,SSP_Mon,SSP_A60_Mon_Days,SSP_B20_Mon_Days".Split(',');
            string[] names =
                "区县名,站名,站网代码,行政编码,国家名称,地市名,镇编码,行政编码2,区域代码,数据来源,国家代码,测站级别,省名,乡镇名,区站号(字符),纬度,经度,测站高度,资料标识,入库时间,收到时间,更新时间,资料时间,年,月,区站号/观测平台标识(数字),气压传感器海拔高度,平均气压,最高本站气压,极端最高本站气压出现日数,极端最高本站气压出现日,最低本站气压,极端最低本站气压出现日数,极端最低本站气压出现日,平均海平面气压,平均气温,平均最高气温,平均最低气温,最高气温,极端最高气温出现日数,极端最高气温出现日,最低气温,极端最低气温出现日数,极端最低气温出现日,平均气温日较差,最高气温日较差,最大气温日较差出现日数,最高气温日较差出现日,最低气温日较差,最小气温日较差出现日数,最低气温日较差出现日,日最高气温≥30℃日数,日最高气温≥35℃日数,日最高气温≥40℃日数,日最低气温＜2Cel日数,日最低气温＜0℃日数,日最低气温＜-2℃日数,日最低气温＜-15℃日数,日最低气温＜-30℃日数,日最低气温＜-40Cel日数,冷度日数(日平均气温≥26.0℃),暖度日数(日平均气温≤18.0℃),平均水气压,平均相对湿度,最小相对湿度,最小相对湿度出现日数,最小相对湿度出现日,平均总云量,平均低云量,日平均总云量<2.0日数,日平均总云量>8.0日数,日平均低云量<2.0日数,日平均低云量>8.0日数,20-20时降水量,08-08时降水量,20-20时月固态降水量,08-08时月固态降水量,最大日降水量,月最大日降水量出现日数,最大日降水量出现日,日降水量≥0.1mm日数,日降水量≥1mm日数,日降水量≥5mm日数,日降水量≥10mm日数,日降水量≥25mm日数,日降水量≥50mm日数,日降水量≥100mm日数,日降水量≥150mm日数,日降水量>=250mm日数,最长连续降水日数,最长连续降水量,最长连续降水止日,最长连续无降水日数,最长连续无降水止日,最大连续降水量,最大连续降水日数,最大连续降水止日,1小时最大降水量,1小时最大降雨量出现日数,1小时最大降水量出现日,雨日数,冰雹日数,轻雾日数,露日数,雨凇日数,雾凇日数,结冰日数,扬沙日数,浮尘日数,霾日数,龙卷风日数,大风日数,沙尘暴日数,雾日数,雷暴日数,霜日数,降雪日数,积雪日数,电线积冰(雨凇+雾凇)日数,能见度≤10km出现频率,能见度<5km出现频,能见度≤1000m出现频率,蒸发(小型）,蒸发(大型),旬内最大积雪深度,最大积雪深度日数,最大积雪深度出现日,雪压,最大雪压出现日数,旬内最大雪压出现日,积雪深度≥1cm日数,积雪深度≥5cm日数,积雪深度≥10cm日数,积雪深度≥20cm日数,积雪深度≥30cm日数,电线积冰最大重量,电线积冰最大重量的相应直径,电线积冰最大重量的相应厚度,电线积冰最大重量出现日数,电线积冰最大重量出现日,平均2分钟风速,最大风速,日最大风速的风向(角度),最大风速出现日数,最大风速出现日,最大风速≥5.0m/s日数,最大风速≥10m/s日数,最大风速≥12m/s日数,最大风速≥15m/s日数,最大风速≥17m/s日数,极大风速,极大风速的风向(角度),极大风速之出现日数,极大风速出现日,NNE风向出现频率,NE风向出现频率,ENE风向出现频率,E风向出现频率,ESE风向出现频率,SE风向出现频率,SSE风向出现频率,S风向出现频率,SSW风向出现频率,SW风向出现频率,WSW风向出现频率,W风向出现频率,WNW风向出现频率,NW风向出现频率,NNW风向出现频率,N风向出现频率,C风向(静风)出现频率,最多风向,最多风向出现频率,次多风向,次多风向频率,NNE风的平均风速,NE风的平均风速,ENE的平均风速,E风的平均风速,ESE风的平均风速,SE风的平均风速,SSE风的平均风速,S风的平均风速,SSW风的平均风速,SW风的平均风速,WSW风的平均风速,W风的平均风速,WNW风的平均风速,NW风的平均风速,NNW风的平均风速,N风的平均风速,NNE风的最大风速,NE风的最大风速,ENE风的最大风速,E风的最大风速,ESE风的最大风速,SE风的最大风速,SSE风的最大风速,S风的最大风速,SSW风的最大风速,SW风的最大风速,WSW风的最大风速,W风的最大风速,WNW风的最大风速,NW风的最大风速,NNW风的最大风速,N风的最大风速,平均地面温度,月平均最高地面温度,月平均最低地面温度,日最低地面温度≤0.0℃日数,最高地面温度,极端最高地面气温出现日数,极端最高地面温度出现日,最低地面温度,极端最低地面气温出现日数,旬极端最低地面温度出现日,平均5cm地温,平均10cm地温,平均15cm地温,平均20cm地温,平均40cm地温,平均80cm地温,平均160cm地温,平均320cm地温,平均草面(雪面)温度,平均最高草面(雪面)温度,平均最低草面(雪面)温度,草面(雪面)最高温度,极端最高草面(雪面)温度出现日数,极端草面(雪面)最高温度出现日,草面(雪面)最低温度,极端最低草面(雪面)温度出现日数,极端草面(雪面)最低温度出现日,最大冻土深度,最大冻土深度出现日数,最大冻土深度出现日,日照时数（直接辐射计算值）,月日照百分率,月日照百分率≥60%日数,月日照百分率≤20%日数".Split(',');
            if (codes.Length == names.Length)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    datas.Add(new 天擎数据名称列表()
                    {
                        DataCode = codes[i],
                        DataName = names[i]
                    });
                }
            }
            return datas;
        }
        private ObservableCollection<天擎数据名称列表> 年数据列表初始化()
        {
            string[] codes =
                "Cnty,Station_Name,NetCode,Admin_Code_CHN,Country,City,Town_code,V_ACODE_4SEARCH,REGIONCODE,D_SOURCE_ID,COUNTRYCODE,Station_levl,Province,Town,Station_Id_C,Lat,Lon,Alti,DATA_ID,IYMDHM,RYMDHM,UPDATE_TIME,Datetime,Year,Station_Id_d,PRS_Sensor_Alti,PRS_Avg,PRS_Max,PRS_Max_Days,PRS_Max_Odate,PRS_Min,PRS_Min_Days,PRS_Min_Odate,PRS_Sea_Avg,TEM_Avg,TEM_Year_Dev,TEM_Max_Avg,TEM_Min_Avg,TEM_Max,TEM_Max_Days,V12011_067,TEM_Min,TEM_Min_Days,V12012_067,TEM_Avg_Dev,TEM_Max_Dev,V12304_040,V12304_067,TEM_Min_Dev_Mon,TEM_Dev_Min_Days,TEM_Dev_Min_Odate,TEM_Max_A30C_Days,TEM_Max_A30_LCDays,TEM_Max_A30_LCDays_EMon,TEM_Max_A30_LCDays_EDay,TEM_Max_A35C_Days,TEM_Max_A35_LCDays,TEM_Max_A35_LCDays_EMon,TEM_Max_A35_LCDays_EDay,TEM_Max_A40C_Days,TEM_Max_A40_LCDays,TEM_Max_A40_LCDays_EMon,TEM_Max_A40_LCDays_EDay,TEM_Min_B2C_Days,TEM_Min_B2_LCDays,TEM_Min_B2_LCDays_EMon,TEM_Min_B2_LCDays_EDay,TEM_Min_B0C_Days,TEM_Min_B0_LCDays,TEM_Min_B0_LCDays_EMon,TEM_Min_B0_LCDays_EDay,TEM_Min_Bn2C_Days,TEM_Min_BM2_LCDays,TEM_Min_BM2_LCDays_EMon,TEM_Min_BM2_LCDays_EDay,TEM_Min_Bn15C_Days,TEM_Min_BM15_LCDays,TEM_Min_BM15_LCDays_EMon,TEM_Min_BM15_LCDays_EDay,TEM_Min_Bn30C_Days,TEM_Min_BM30_LCDays,TEM_Min_BM30_LCDays_EMon,TEM_Min_BM30_LCDays_EDay,TEM_Min_Bn40C_Days,TEM_Min_BM40_LCDays,TEM_Min_BM40_LCDays_EMon,TEM_Min_BM40_LCDays_EDay,TEM_Avg_StPas_0_SMon,TEM_Avg_StPas_0_SDay,TEM_Avg_StPas_0_EMon,TEM_Avg_StPas_0_EDay,ACTEM_TEM_Avg_StPas_0,PRE_TEM_Avg_StPas_0,SSH_TEM_Avg_StPas_0,TEM_Avg_StPas_5_SMon,TEM_Avg_StPas_5_SDay,TEM_Avg_StPas_5_EMon,TEM_Avg_StPas_5_EDay,ACTEM_TEM_Avg_StPas_5,PRE_TEM_Avg_StPas_5,SSH_TEM_Avg_StPas_5,TEM_Avg_StPas_10_SMon,TEM_Avg_StPas_10_SDay,TEM_Avg_StPas_10_EMon,TEM_Avg_StPas_10_EDay,ACTEM_TEM_Avg_StPas_10,PRE_TEM_Avg_StPas_10,SSH_TEM_Avg_StPas_10,TEM_Avg_StPas_15_SMon,TEM_Avg_StPas_15_SDay,TEM_Avg_StPas_15_EMon,TEM_Avg_StPas_15_EDay,ACTEM_TEM_Avg_StPas_15,PRE_TEM_Avg_StPas_15,SSH_TEM_Avg_StPas_15,TEM_Avg_StPas_20_SMon,TEM_Avg_StPas_20_SDay,TEM_Avg_StPas_20_EMon,TEM_Avg_StPas_20_EDay,ACTEM_TEM_Avg_StPas_20,PRE_TEM_Avg_StPas_20,SSH_TEM_Avg_StPas_20,TEM_Avg_StPas_22_SMon,TEM_Avg_StPas_22_SDay,TEM_Avg_StPas_22_EMon,TEM_Avg_StPas_22_EDay,ACTEM_TEM_Avg_StPas_22,PRE_TEM_Avg_StPas_22,SSH_TEM_Avg_StPas_22,TEM_Avg_A26_Days,TEM_Avg_B18_Days,VAP_Avg,RHU_Avg,RHU_Min,RHU_Min_Days,V13007_067,CLO_Cov_Avg,CLO_Cov_Low_Avg,CLO_Cov_Avg_B2_Days,CLO_Cov_A8_Days,CLO_Low_Cov_Avg_B2_Days,CLO_Cov_Avg_A8_Days,PRE_Time_2020,PRE_Time_0808,SPRE_Time_2020,SPRE_Time_0808,PRE_Max_Day,PRE_Max_Mon_Days,V13052_067,PRE_A0p1mm_Days,PRE_A1mm_Days,PRE_A5mm_Days,PRE_A10mm_Days,PRE_A25mm_Days,PRE_A50mm_Days,PRE_A100mm_Days,PRE_A150mm_Days,PRE_Day_A250_Days,Days_Max_Coti_PRE,PRE_Conti_Max,PRE_LCDays_EMon,EDay_Max_Coti_PRE,NPRE_LCDays,NPRE_LCDays_EMon,NPRE_LCDays_EDay,PRE_Max_Conti,Days_Max_Conti_PRE,PRE_Coti_Max_EMon,PRE_Coti_Max_EDay,PRE_Max_1h,PRE_Max_1h_Days,PRE_Max_1h_Odate,PRE_Max_1h_OMon,PRE_Max_1h_ODay,PRE_Max_5mi_Year,PRE_Max_5mi_Year_Stime,PRE_Max_10mi_Year,PRE_Max_10mi_Year_Stime,PRE_Max_15mi_Year,PRE_Max_15mi_Year_Stime,PRE_Max_20mi_Year,PRE_Max_20mi_Year_Stime,PRE_Max_30mi_Year,PRE_Max_30mi_Year_Stime,PRE_Max_45mi_Year,PRE_Max_45mi_Year_Stime,PRE_Max_60mi_Year,PRE_Max_60mi_Year_Stime,PRE_Max_90mi_Year,PRE_Max_90mi_Year_Stime,PRE_Max_120mi_Year,PRE_Max_120mi_Year_Stime,PRE_Max_180mi_Year,PRE_Max_180mi_Year_Stime,PRE_Max_240mi_Year,PRE_Max_240mi_Year_Stime,PRE_Max_360mi_Year,PRE_Max_360mi_Year_Stime,PRE_Max_540mi_Year,PRE_Max_540mi_Year_Stime,PRE_Max_720mi_Year,PRE_Max_720mi_Year_Stime,V13382_1440,V13382_1440_STIME,PRE_Days,Hail_Days,Mist_Days,V04330_001,Glaze_Days,SoRi_Days,ICE_Days,FlSa_Days,FlDu_Days,Haze_Days,Tord_Days,GaWIN_Days,SaSt_Days,Fog_Days,Thund_Days,Frost_Days,Snow_Days,GSS_Days,EICE_Days,VIS_B10km_Freq,VIS_B5km_Freq,VIS_B1km_Freq,EVP,EVP_Big,Snow_Depth_Max,V13334_040,V13334_067,Snow_PRS,Snow_PRS_Max_Days,V13330_067,Snow_Depth_A1cm_Days,Snow_Depth_A5cm_Days,Snow_Depth_A10cm_Days,Snow_Depth_A20cm_Days,Snow_Depth_A30cm_Days,EICE_Wei_Max,EICEW_Max_Diam,EICEW_Max_Thick,V20440_040,EICED_Max_Odate,EICEW_Max_OMon,EICEW_Max_ODay,WIN_S_2mi_Avg,WIN_S_Max,WIN_D_S_Max_C,Days_WIN_S_Max,V11042_067,WIN_S_A5ms_Days,WIN_S_Max_A10ms_Days,WIN_S_A12ms_Days,V11042_15,WIN_S_A17ms_Days,WIN_S_Inst_Max,WIN_D_INST_Max_C,V11046_040,WIN_S_INST_Max_ODate_C,WIN_NNE_Freq,WIN_NE_Freq,WIN_ENE_Freq,WIN_E_Freq,WIN_ESE_Freq,WIN_SE_Freq,WIN_SSE_Freq,WIN_S_Freq,WIN_SSW_Freq,WIN_SW_Freq,WIN_WSW_Freq,WIN_W_Freq,WIN_WNW_Freq,WIN_NW_Freq,WIN_NNW_Freq,WIN_N_Freq,WIN_C_Freq,WIN_D_Max_C,WIN_D_Max_Freq,WIN_D_SMax_C,WIN_D_SMax_Freq,WIN_S_Avg_NNE,WIN_S_Avg_NE,WIN_S_Avg_ENE,WIN_S_Avg_E,WIN_S_Avg_ESE,WIN_S_Avg_SE,WIN_S_Avg_SSE,WIN_S_Avg_S,WIN_S_Avg_SSW,WIN_S_Avg_SW,WIN_S_Avg_WSW,WIN_S_Avg__W,WIN_S_Avg_WNW,WIN_S_Avg_NW,WIN_S_Avg_NNW,WIN_S_Avg__N,WIN_S_Max_NNE,WIN_S_Max_NE,WIN_S_Max_ENE,WIN_S_Max_E,WIN_S_Max_ESE,WIN_S_Max_SE,WIN_S_Max_SSE,WIN_S_Max_S,WIN_S_Max_SSW,WIN_S_Max_SW,WIN_S_Max_WSW,WIN_S_Max_W,WIN_S_Max_WNW,WIN_S_Max_NW,WIN_S_Max_NNW,WIN_S_Max_N,GST_Avg,EGST_Max_Avg_Mon,GST_Min_Avg,GST_Min_Bel_0_Days,GST_Max,EGST_Max_Days,V12311_067,GST_Min,Days_GST_Min,V12121_067,GST_Avg_5cm,GST_Avg_10cm,GST_Avg_15cm,GST_Avg_20cm,GST_Avg_40cm,GST_Avg_80cm,GST_Avg_160cm,GST_Avg_320cm,LGST_Avg,LGST_Max_Avg,LGST_Min_Avg,LGST_Max,LGST_Max_Days,LGST_Max_Odate,LGST_Min,LGST_Min_Days,LGST_Min_Odate,FRS_Depth_Max,FRS_Depth_Max_Days,FRS_Depth_Max_Odate,SSH,SSP_Mon,SSP_A60_Mon_Days,SSP_B20_Mon_Days".Split(',');
            string[] names =
                "区县名,站名,站网代码,行政编码,国家名称,地市名,镇编码,行政编码2,区域代码,数据来源,国家代码,测站级别,省名,乡镇名,区站号(字符),纬度,经度,测站高度,资料标识,入库时间,收到时间,更新时间,资料时间,年,区站号/观测平台标识(数字),气压传感器海拔高度,平均气压,最高本站气压,极端最高本站气压出现日数,极端最高本站气压出现月日,最低本站气压,极端最低本站气压出现日数,极端最高本站气压出现月日,平均海平面气压,平均气温,气温年较差,平均最高气温,平均最低气温,最高气温,极端最高气温出现日数,极端最高气温出现月日,最低气温,极端最低气温出现日数,极端最低气温出现月日,平均气温日较差,最高气温日较差,最大气温日较差出现日数,最大气温日较差出现月日,最低气温日较差,最小气温日较差出现日数,最小气温日较差出现月日,日最高气温≥30℃日数,日最高气温≥30.0℃最长连续日数,日最高气温≥30.0℃最长连续日数之止月,日最高气温≥30.0℃最长连续日数之止日,日最高气温≥35℃日数,日最高气温≥35.0℃最长连续日数,日最高气温≥35.0℃最长连续日数之止月,日最高气温≥35.0℃最长连续日数之止日,日最高气温≥40℃日数,日最高气温≥40.0℃最长连续日数,日最高气温≥40.0℃最长连续日数之止月,日最高气温≥40.0℃最长连续日数之止日,日最低气温＜2Cel日数,日最低气温<2.0℃最长连续日数,日最低气温<2.0℃最长连续日数之止月,日最低气温<2.0℃最长连续日数之止日,日最低气温＜0℃日数,日最低气温<0.0℃最长连续日数,日最低气温<0.0℃最长连续日数之止月,日最低气温<0.0℃最长连续日数之止日,日最低气温＜-2℃日数,日最低气温<-2.0℃最长连续日数,日最低气温<-2.0℃最长连续日数之止月,日最低气温<-2.0℃最长连续日数之止日,日最低气温＜-15℃日数,日最低气温<-15.0℃最长连续日数,日最低气温<-15.0℃最长连续日数之止月,日最低气温<-15.0℃最长连续日数之止日,日最低气温＜-30℃日数,日最低气温<-30.0℃最长连续日数,日最低气温<-30.0℃最长连续日数之止月,日最低气温<-30.0℃最长连续日数之止日,日最低气温＜-40Cel日数,日最低气温<-40.0℃最长连续日数,日最低气温<-40.0℃最长连续日数之止月,日最低气温<-40.0℃最长连续日数之止日,日平均气温稳定通过0.0℃之起始月,日平均气温稳定通过0.0℃之起日,日平均气温稳定通过0.0℃之止月,日平均气温稳定通过0.0℃之止日,日平均气温稳定通过0.0℃之积温,日平均气温稳定通过0.0℃之降水量,日平均气温稳定通过0.0℃之日照时数,日平均气温稳定通过5.0℃之起始月,日平均气温稳定通过5.0℃之起日,日平均气温稳定通过5.0℃之止月,日平均气温稳定通过5.0℃之止日,日平均气温稳定通过5.0℃之积温,日平均气温稳定通过5.0℃之降水量,日平均气温稳定通过5.0℃之日照时数,日平均气温稳定通过10.0℃之起始月,日平均气温稳定通过10.0℃之起日,日平均气温稳定通过10.0℃之止月,日平均气温稳定通过10.0℃之止日,日平均气温稳定通过10.0℃之积温,日平均气温稳定通过10.0℃之降水量,日平均气温稳定通过10.0℃之日照时数,日平均气温稳定通过15.0℃之起始月,日平均气温稳定通过15.0℃之起日,日平均气温稳定通过15.0℃之止月,日平均气温稳定通过15.0℃之止日,日平均气温稳定通过15.0℃之积温,日平均气温稳定通过15.0℃之降水量,日平均气温稳定通过15.0℃之日照时数,日平均气温稳定通过20.0℃之起始月,日平均气温稳定通过20.0℃之起日,日平均气温稳定通过20.0℃之止月,日平均气温稳定通过20.0℃之止日,日平均气温稳定通过20.0℃之积温,日平均气温稳定通过20.0℃之降水量,日平均气温稳定通过20.0℃之日照时数,日平均气温稳定通过22.0℃之起始月,日平均气温稳定通过22.0℃之起日,日平均气温稳定通过22.0℃之止月,日平均气温稳定通过22.0℃之止日,日平均气温稳定通过22.0℃之积温,日平均气温稳定通过22.0℃之降水量,日平均气温稳定通过22.0℃之日照时数,冷度日数(日平均气温≥26.0℃),暖度日数(日平均气温≤18.0℃),平均水气压,平均相对湿度,最小相对湿度,最小相对湿度出现日数,最小相对湿度出现月日,平均总云量,平均低云量,日平均总云量<2.0日数,日平均总云量>8.0日数,日平均低云量<2.0日数,日平均低云量>8.0日数,20-20时降水量,08-08时降水量,20-20时月固态降水量,08-08时月固态降水量,最大日降水量,月最大日降水量出现日数,最大日降水量出现月日,日降水量≥0.1mm日数,日降水量≥1mm日数,日降水量≥5mm日数,日降水量≥10mm日数,日降水量≥25mm日数,日降水量≥50mm日数,日降水量≥100mm日数,日降水量≥150mm日数,日降水量>=250mm日数,最长连续降水日数,最长连续降水量,最长连续降水止月,最长连续降水止日,最长连续无降水日数,最长连续无降水止月,最长连续无降水止日,最大连续降水量,最大连续降水日数,最大连续降水止月,最大连续降水止日,1小时最大降水量,1小时最大降雨量出现日数,1小时最大降水量出现月日,1小时最大降水量出现月,1小时最大降水量出现日,年最大5分钟降水量,年最大5分钟降水起始时间（MMddhhmm）,年最大10分钟降水量,年最大10分钟降水起始时间,年最大15分钟降水量,年最大15分钟降水起始时间,年最大20分钟降水量,年最大20分钟降水起始时间,年最大30分钟降水量,年最大30分钟降水起始时间,年最大45分钟降水量,年最大45分钟降水起始时间,年最大60分钟降水量,年最大60分钟降水起始时间,年最大90分钟降水量,年最大90分钟降水起始时间,年最大120分钟降水量,年最大120分钟降水起始时间,年最大180分钟降水量,年最大180分钟降水起始时间,年最大240分钟降水量,年最大240分钟降水起始时间,年最大360分钟降水量,年最大360分钟降水起始时间,年最大540分钟降水量,年最大540分钟降水起始时间,年最大720分钟降水量,年最大720分钟降水起始时间,年最大1440分钟降水量,年最大1440分钟降水起始时间,雨日数,冰雹日数,轻雾日数,露日数,雨凇日数,雾凇日数,结冰日数,扬沙日数,浮尘日数,霾日数,龙卷风日数,大风日数,沙尘暴日数,雾日数,雷暴日数,霜日数,降雪日数,积雪日数,电线积冰(雨凇+雾凇)日数,能见度≤10km出现频率,能见度<5km出现频,能见度≤1000m出现频率,蒸发(小型）,蒸发(大型),旬内最大积雪深度,最大积雪深度日数,最大积雪深度出现月日,雪压,最大雪压出现日数,最大雪压出现月日,积雪深度≥1cm日数,积雪深度≥5cm日数,积雪深度≥10cm日数,积雪深度≥20cm日数,积雪深度≥30cm日数,电线积冰最大重量,电线积冰最大重量的相应直径,电线积冰最大重量的相应厚度,电线积冰最大重量出现日数,电线积冰最大重量出现月日,电线积冰最大重量出现月,电线积冰最大重量出现日,平均2分钟风速,最大风速,日最大风速的风向(角度),最大风速出现日数,最大风速出现月日,最大风速≥5.0m/s日数,最大风速≥10m/s日数,最大风速≥12m/s日数,最大风速≥15m/s日数,最大风速≥17m/s日数,极大风速,极大风速的风向(角度),极大风速之出现日数,极大风速出现日期,NNE风向出现频率,NE风向出现频率,ENE风向出现频率,E风向出现频率,ESE风向出现频率,SE风向出现频率,SSE风向出现频率,S风向出现频率,SSW风向出现频率,SW风向出现频率,WSW风向出现频率,W风向出现频率,WNW风向出现频率,NW风向出现频率,NNW风向出现频率,N风向出现频率,C风向(静风)出现频率,最多风向,最多风向出现频率,次多风向,次多风向频率,NNE风的平均风速,NE风的平均风速,ENE的平均风速,E风的平均风速,ESE风的平均风速,SE风的平均风速,SSE风的平均风速,S风的平均风速,SSW风的平均风速,SW风的平均风速,WSW风的平均风速,W风的平均风速,WNW风的平均风速,NW风的平均风速,NNW风的平均风速,N风的平均风速,NNE风的最大风速,NE风的最大风速,ENE风的最大风速,E风的最大风速,ESE风的最大风速,SE风的最大风速,SSE风的最大风速,S风的最大风速,SSW风的最大风速,SW风的最大风速,WSW风的最大风速,W风的最大风速,WNW风的最大风速,NW风的最大风速,NNW风的最大风速,N风的最大风速,平均地面温度,月平均最高地面温度,月平均最低地面温度,日最低地面温度≤0.0℃日数,最高地面温度,极端最高地面气温出现日数,极端最高地面气温出现月日,最低地面温度,极端最低地面气温出现日数,极端最低地面气温出现月日,平均5cm地温,平均10cm地温,平均15cm地温,平均20cm地温,平均40cm地温,平均80cm地温,平均160cm地温,平均320cm地温,平均草面(雪面)温度,平均最高草面(雪面)温度,平均最低草面(雪面)温度,草面(雪面)最高温度,极端最高草面(雪面)温度出现日数,极端最高草面(雪面)温度出现月日,草面(雪面)最低温度,极端最低草面(雪面)温度出现日数,极端最低草面(雪面)温度出现月日,最大冻土深度,最大冻土深度出现日数,最大冻土深度出现月日,日照时数（直接辐射计算值）,月日照百分率,月日照百分率≥60%日数,月日照百分率≤20%日数".Split(',');
            if (codes.Length == names.Length)
            {
                for (int i = 0; i < codes.Length; i++)
                {
                    datas.Add(new 天擎数据名称列表()
                    {
                        DataCode = codes[i],
                        DataName = names[i]
                    });
                }
            }
            return datas;
        }
        #endregion
        private void RadMaskedTextInput_ValueChanged(object sender, Telerik.Windows.RadRoutedEventArgs e)
        {
            myIDStr = stationsIDInu.Text;
        }
    }

    public class 天擎数据名称列表 : INotifyPropertyChanged
    {
        private string _DataCode;
        private string _DataName;
        public string DataCode
        {
            get => _DataCode;
            set
            {
                if (value != _DataCode)
                {
                    _DataCode = value;
                    OnPropertyChanged("DataCode");
                }
            }
        }
        public string DataName
        {
            get => _DataName;
            set
            {
                if (value != _DataName)
                {
                    _DataName = value;
                    OnPropertyChanged("DataName");
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, args);
        }

        private void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }
    }

}