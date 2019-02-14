using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace 旗县端
{
    class ConfigClass1
    {
        string _con = "";

        public ConfigClass1()
        {
            string DBconPath = System.Environment.CurrentDirectory + @"\设置文件\DBconfig.txt";
            using (StreamReader sr = new StreamReader(DBconPath, Encoding.Default))
            {
                string line;

                // 从文件读取并显示行，直到文件的末尾 
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.Contains("sql管理员"))
                    {
                        _con = line.Substring("sql管理员=".Length);
                    }
                }
            }
        }
        public string IDbyID(int FID)//返回指定上级ID下的所有ID,以逗号分隔
        {
            string strFH = "";
            ObservableCollection<menuList> menulist1 = new ObservableCollection<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from QX where FatherID = '{0}' order by XH", FID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            id = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    foreach (var v1 in menulist1)
                    {
                        strFH += v1.id + ',';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }
        public int HqzxXH(int FID)
        {
            int XHFH = 1;
            ObservableCollection<menuList> menulist1 = new ObservableCollection<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from QX where FatherID = '{0}' order by XH", FID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            xh = sqlreader.GetInt32(sqlreader.GetOrdinal("XH")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                        string idls = "";
                        for (int i = 1; i < 99; i++)
                        {
                            bool bs1 = false;
                            foreach (var m in menulist1)
                            {
                                if (m.xh == i)
                                {
                                    bs1 = true;
                                    break;
                                }
                            }

                            if (!bs1)
                            {
                                XHFH = i;
                                break;
                            }
                        }
                    
                }
                else
                {
                    XHFH = 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return XHFH;
        }
        /// <summary>
        /// 修改站点信息
        /// </summary>
        /// <param name="XH">站点序号</param>
        /// <param name="ID">区站号</param>
        /// <param name="FID">上级区站号，如果为旗县，上级为-1</param>
        /// <param name="Name">站点名称</param>
        /// <param name="原来的区站号">站点名称</param>
        public bool XGQXXZ(int XH,string ID,string FID,string Name,string CSID,string BS)
        {
            bool bsBool = false;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"update QX set xh='{0}',id='{1}',FatherID='{2}',name='{3}' where ID = '{4}'", XH,ID,FID,Name,CSID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    try
                    {
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            bsBool = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }

                if (bsBool)
                {
                    using (SqlConnection mycon = new SqlConnection(_con))
                    {
                        mycon.Open(); //打开
                        string sql = string.Format(@"update QXBS set BS='{0}',id='{1}' where ID = '{2}'", BS,
                            ID,CSID); //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        try
                        {
                            sqlman.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                }


            }
            catch(Exception ex)
            {
            }

            return bsBool;
        }
        public bool XGQXXZ(int XH, string ID, string FID, string Name, string CSID)
        {
            bool bsBool = false;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"update QX set xh='{0}',id='{1}',FatherID='{2}',name='{3}' where ID = '{4}'", XH, ID, FID, Name, CSID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    try
                    {
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            bsBool = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }


            }
            catch (Exception ex)
            {
            }

            return bsBool;
        }

        public bool XGPeople(string QXID, string ID, string Name,string Admin, string CSID)
        {
            bool bsBool = false;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"update USERID set QXID='{0}',id='{1}',name='{2}',admin='{3}' where ID = '{4}'", QXID, ID, Name, Admin, CSID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    try
                    {
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            bsBool = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }


            }
            catch (Exception ex)
            {
            }

            return bsBool;
        }

        public string IDName(int FID)//返回指定上级ID下的所有ID和名称，ID与名称以','分割，每组以换行符分割
        {
            string strFH = "";
            ObservableCollection<menuList> menulist1 = new ObservableCollection<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from QX where FatherID = '{0}' order by XH", FID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            xh= sqlreader.GetInt32(sqlreader.GetOrdinal("xh")),
                            id = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("name")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    foreach (var v1 in menulist1)
                    {
                        strFH += v1.id + ',' + v1.name + ',' + v1.xh + '\n';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }
        public string IDName()//返回所有ID和名称，ID与名称以','分割，每组以换行符分割
        {
            string strFH = "";
            ObservableCollection<menuList> menulist1 = new ObservableCollection<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from QX order by FatherID");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            xh = sqlreader.GetInt32(sqlreader.GetOrdinal("xh")),
                            id = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("name")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    foreach (var v1 in menulist1)
                    {
                        strFH += v1.id + ',' + v1.name + ',' + v1.xh + '\n';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }

        public string PeopleID(int QXID)//返回指定旗县ID下的所有人员的ID、名称、是否管理员，ID与名称以','分割，每组以换行符分割
        {
            string strFH = "";
            ObservableCollection<PeopleL> menulist1 = new ObservableCollection<PeopleL>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from USERID where QXID = '{0}' order by ID", QXID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new PeopleL()
                        {
                            admin = sqlreader.GetString(sqlreader.GetOrdinal("admin")),
                            id = sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                            name = sqlreader.GetString(sqlreader.GetOrdinal("name")),
                        });
                    }
                }

                if (menulist1.Count > 0)
                {
                    foreach (var v1 in menulist1)
                    {
                        strFH += v1.id + ',' + v1.name + ',' + v1.admin + '\n';
                    }

                    strFH = strFH.Substring(0, strFH.Length - 1);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return strFH;
        }
        //数据站点信息同步至本地设置文件，后续增加同步发报软件设置
        public void TBBD()
        {
            int QXGS = 0;
            ObservableCollection<menuList> menulist1 = new ObservableCollection<menuList>();
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"select * from QX where FatherID = '-1' order by XH");  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        menulist1.Add(new menuList()
                        {
                            xh = sqlreader.GetInt32(sqlreader.GetOrdinal("XH")),
                            name=sqlreader.GetString(sqlreader.GetOrdinal("name")),
                            id=sqlreader.GetString(sqlreader.GetOrdinal("ID")),
                        });
                    }
                }

                FBRJBD(menulist1);
                QXGS = menulist1.Count;
                string strData = "旗县个数:" + QXGS.ToString() + "\r\n";
                foreach (var mm in menulist1)
                {
                    string str1 = mm.name+ ",", str2 =mm.id+ ",";
                    using (SqlConnection mycon = new SqlConnection(_con))
                    {
                        mycon.Open();//打开
                        string sql = string.Format(@"select * from QX where FatherID = '{0}' order by XH",mm.id);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                        SqlCommand sqlman = new SqlCommand(sql, mycon);
                        SqlDataReader sqlreader = sqlman.ExecuteReader();
                        while (sqlreader.Read())
                        {
                            str1 += sqlreader.GetString(sqlreader.GetOrdinal("name"))+',';
                            str2 += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + ',';
                        }

                        str1 = str1.Substring(0,str1.Length-1);
                        str2 = str2.Substring(0, str2.Length - 1);
                        strData += str1 + "\r\n" + str2 + "\r\n";
                    }
                }
                string path = Environment.CurrentDirectory + @"\设置文件\旗县乡镇.txt";
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
                {
                    sw.Write(strData);
                    sw.Flush();
                }
                //同步旗县报文标识
                string BSStr = "";
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = @"select * from QXBS order by ID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    SqlDataReader sqlreader = sqlman.ExecuteReader();
                    while (sqlreader.Read())
                    {
                        BSStr += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=' +
                                 sqlreader.GetString(sqlreader.GetOrdinal("BS")) + "\r\n";



                    }

                    BSStr += "市本级=BFQX\r\n中央=";
                    using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt", false, Encoding.Default))
                    {
                        sw.Write(BSStr);
                        sw.Flush();
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public bool OtherCon(string name, string content)
        {
            bool bsBool = false;
            try
            {
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"update OtherConfig set data='{0}' where name = '{1}'",content,name);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    try
                    {
                        if (sqlman.ExecuteNonQuery() > 0)
                        {
                            bsBool = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }


            }
            catch (Exception ex)
            {
            }

            return bsBool;
        }
        /// <summary>
        /// 同步发报软件本地旗县乡镇的配置信息
        /// </summary>
        public void FBRJBD(ObservableCollection<menuList> menulist1)
        {
            try
            {
                string FBPath = "";//发报软件配置文件路径
                string DZBPath = System.Environment.CurrentDirectory + @"\设置文件\旗县ID报文缩写对照.txt";
                using (StreamReader sr = new StreamReader(DZBPath, Encoding.Default))
                {
                    DZBPath = sr.ReadToEnd();
                }

                string[] SZDZ = DZBPath.Split(new char[] {'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
                 string configpathPath = System.Environment.CurrentDirectory + @"\config\YBpath.txt";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "发报软件路径")
                        {
                            FBPath = line.Split('=')[1];
                            FBPath = Path.GetDirectoryName(FBPath) + @"\MakeYbIni\ForeCast.ini";
                            break;
                        }
                    }

                }

                if (FBPath.Length > 0&& File.Exists(FBPath))
                {
                    string strLS1 = "";
                    string strData = "";
                    using (StreamReader sr = new StreamReader(FBPath, Encoding.Default))
                    {
                        string[] SzData1 = sr.ReadToEnd().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        string DQID = "";
                        string DQQX=SzData1[4].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[1].Split('=')[1];
                        string[] SzLS1 = SzData1[3].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string ssls in SzLS1)
                        {
                            if ((!ssls.Contains("=="))&&ssls.Contains(DQQX))
                            {
                                if (DQQX == "呼和浩特市")
                                {

                                }
                                DQID = ssls.Split('&')[2];
                                
                                break;
                            }
                        }
                        for (int i = 0; i < menulist1.Count; i++)
                        {
                            foreach (string sls in SZDZ)
                            {
                                if (sls.Split('=')[0] == menulist1[i].id)
                                {
                                    strLS1 += menulist1[i].name + '=' + Convert.ToChar('A' + i) + '&'+sls.Split('=')[1] + '&' + menulist1[i].id+"\r\n";
                                    break;
                                }
                            }
                        }
                        foreach (string sls in SZDZ)
                        {
                            if (sls.Split('=')[0] == "市本级")
                            {
                                strLS1 += "呼和浩特市" + '=' +  "O&" + sls.Split('=')[1] + '&'  + "53466\r\n";
                                break;
                            }
                        }

                        SzData1[3] = SzLS1[0] +"\r\n"+ strLS1 + SzLS1[SzLS1.Length - 1] + "\r\n";
                        string s1 = "", s2 = "[呼和浩特市]\r\n";
                        string s3 = "";
                        foreach (var mm in menulist1)
                        {
                            s3= string.Format("{0}={1} 4108 11147 0 0 0 15900\r\n", mm.name, mm.id);
                            s1 += string.Format("[{0}]\r\n", mm.name)+s3;
                            s2 += s3;
                            using (SqlConnection mycon = new SqlConnection(_con))
                            {
                                mycon.Open();//打开
                                string sql = string.Format(@"select * from QX where FatherID = '{0}' order by XH", mm.id);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                                SqlCommand sqlman = new SqlCommand(sql, mycon);
                                SqlDataReader sqlreader = sqlman.ExecuteReader();
                                while (sqlreader.Read())
                                {
                                    try
                                    {
                                        s3 = string.Format("{0}={1} 4108 11147 0 0 0 15900\r\n", sqlreader.GetString(sqlreader.GetOrdinal("name")), sqlreader.GetString(sqlreader.GetOrdinal("ID")));
                                        s2 += s3;
                                        s1 += s3;
                                    }
                                    catch
                                    {

                                    }
                                }


                            }
                            if (mm.id == DQID)
                            {
                                if (DQQX != "呼和浩特市")
                                {
                                    SzData1[4] = SzData1[4].Replace(DQQX, mm.name);
                                }
                                
                                
                            }
                        }

                        SzLS1 = SzData1[SzData1.Length - 1]
                            .Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                        SzData1[SzData1.Length - 1] = SzLS1[0]+"\r\n" + s1 + s2 + "\r\n" + SzLS1[SzLS1.Length - 2] + "\r\n" + SzLS1[SzLS1.Length - 1];
                        foreach (string ss in SzData1)
                        {
                            strData += ss + ';';
                        }

                        strData = strData.Substring(0, strData.Length-1);
                    }
                    
                    using (StreamWriter sw = new StreamWriter(FBPath, false, Encoding.Default))
                    {
                        sw.Write(strData);
                        sw.Flush();
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /// 根据旗县区站号更新发报软件设置文件中的当前旗县
        /// </summary>
        /// <param name="ID"></param>
        public void FBRJTBID(string ID)
        {
            try
            {
                string FBPath = "";//发报软件配置文件路径
                

                string configpathPath = System.Environment.CurrentDirectory + @"\config\YBpath.txt";
                using (StreamReader sr = new StreamReader(configpathPath, Encoding.Default))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Split('=')[0] == "发报软件路径")
                        {
                            FBPath = line.Split('=')[1];
                            FBPath = Path.GetDirectoryName(FBPath) + @"\MakeYbIni\ForeCast.ini";
                            break;
                        }
                    }

                }

                if (FBPath.Length > 0 && File.Exists(FBPath))
                {

                    string strData = "";
                    string DQName = "";
                    using (StreamReader sr = new StreamReader(FBPath, Encoding.Default))
                    {
                        string[] SzData1 = sr.ReadToEnd().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        string[] SzLS1 = SzData1[3].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                        string DQQX = SzData1[4].Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[1].Split('=')[1];
                        foreach (string ss in SzLS1)
                        {
                            try
                            {
                                if (ss.Split('=')[1].Split('&')[2] == ID)
                                {
                                    DQName = ss.Split('=')[0];
                                    if (DQName != "呼和浩特市")
                                    {
                                        SzData1[4] = SzData1[4].Replace(DQQX, DQName);
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        
                        foreach (string ss in SzData1)
                        {
                            strData += ss + ';';
                        }

                        strData = strData.Substring(0, strData.Length - 1);
                    }

                    using (StreamWriter sw = new StreamWriter(FBPath, false, Encoding.Default))
                    {
                        sw.Write(strData);
                        sw.Flush();
                    }
                }
                else
                {
                    MessageBox.Show("旗县更改信息同步发报软件设置失败，原因：没有找到发报软件设置文件" + FBPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("旗县更改信息同步发报软件设置失败，原因：\r\n"+ex.Message);
            }
        }

        /// <summary>
        /// 获取旗县报文标识字符串：旗县区站号=报文标识
        /// </summary>
        /// <returns></returns>
        public string HQBS()
        {
            string BSStr = "";
            using (SqlConnection mycon = new SqlConnection(_con))
            {
                mycon.Open();//打开
                string sql = @"select * from QXBS order by ID";  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                SqlCommand sqlman = new SqlCommand(sql, mycon);
                SqlDataReader sqlreader = sqlman.ExecuteReader();
                while (sqlreader.Read())
                {
                    BSStr += sqlreader.GetString(sqlreader.GetOrdinal("ID")) + '=' +
                             sqlreader.GetString(sqlreader.GetOrdinal("BS")) + "\r\n";

                }

                BSStr = BSStr.Substring(0, BSStr.Length-2);
            }

            return BSStr;
        }

        public bool DeletePeople(string ID)
        {
            bool FHbool = false;
            try
            {
                string bs1 = "";
                
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"Delete  from USERID where ID = '{0}'", ID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    if (sqlman.ExecuteNonQuery() > 0)
                    {
                        FHbool = true;
                        bs1 = "\r\n删除人员成功";
                    }
                    else
                        bs1 = "\r\n删除该人员失败";
                    MessageBox.Show(bs1);


                }

            }
            catch (Exception ex)
            {
            }

            return FHbool;
        }
        public bool DeleteXZ(string QXID)
        {
            bool FHbool = false;
            try
            {
                string bs1 = "";
               
                using (SqlConnection mycon = new SqlConnection(_con))
                {
                    mycon.Open();//打开
                    string sql = string.Format(@"Delete  from QX where ID = '{0}'", QXID);  //SQL查询语句 (Name,StationID,Date)。按照数据库中的表的字段顺序保存
                    SqlCommand sqlman = new SqlCommand(sql, mycon);
                    if (sqlman.ExecuteNonQuery() > 0)
                    {
                        FHbool = true;
                        bs1 = "\r\n删除该乡镇成功";
                    }
                    else
                        bs1 = "\r\n删除该乡镇失败，没有该区站号";
                    MessageBox.Show(bs1);


                }
            }
            catch (Exception ex)
            {
            }

            return FHbool;
        }
        public class menuList
        {
            public int xh { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }

        public class PeopleL
        {
            public string admin { get; set; }
            public string name { get; set; }
            public string id { get; set; }
        }
    }
}
