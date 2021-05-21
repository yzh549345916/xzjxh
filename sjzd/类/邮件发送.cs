using Aspose.Email;
using Aspose.Email.Clients.Smtp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Aspose.Email.Clients;

namespace sjzd.类
{
    class 邮件发送
    {
        public void Email书记短信(string path, string title, string skData, ref string error)
        {
            
            XmlConfig util = new XmlConfig(Environment.CurrentDirectory + @"\设置文件\新界面设置.xml");
            MailAddressCollection attendees = new MailAddressCollection();
            string myaddress = util.Read("emailConfig", "emailTo");
            if (myaddress.Trim().Length == 0)
            {
                return;
            }
            foreach (string ad in myaddress.Split(','))
            {
                attendees.Add(ad.Trim());
            }
            int myCount = 1;
            try
            {
                myCount = Convert.ToInt32(util.Read("emailConfig", "myCount"));
            }
            catch
            {
            }
            int emailCount = 1;
            try
            {
                emailCount = Convert.ToInt32(util.Read("emailConfig", "emailCount"));
            }
            catch
            {
            }
            if (emailCount < myCount)
                myCount = emailCount;
            for (int i = 0; i < emailCount; i++)
            {
                int countLS = myCount + i;
                try
                {
                    string myfrom = util.Read("emailConfig", $"emailFrom{countLS % emailCount}", "address"), mypassword = util.Read("emailConfig", $"emailFrom{countLS % emailCount}", "password");
                    MailMessage mailMessage = new MailMessage
                    {
                        From = myfrom,
                        To = attendees,
                        Subject = title,
                        CC = myfrom,
                        Body = skData
                    };
                    mailMessage.AddAttachment(new Attachment(path));
                    int port = 0;
                    // HttpProxy proxy = new HttpProxy("172.18.142.150", 808);
                    using (SmtpClient client = new SmtpClient(获取邮件服务器(myfrom, ref port), port, myfrom, mypassword))
                    {
                        // client.Proxy = proxy;
                        ServicePointManager.ServerCertificateValidationCallback = delegate
                        {
                            return true;
                        };
                        try
                        {
                            //client.UseDefaultCredentials = true;
                            client.Send(mailMessage);
                        }
                        catch (Exception ee)
                        {
                            error += $"使用{myfrom}邮箱发送失败，错误为：{ee.Message}\n";
                            continue;
                        }
                    }
                    error += $"使用{myfrom}邮箱发送成功\n";
                    try
                    {
                        util.Write(((countLS + 1) % emailCount).ToString(), "emailConfig", "myCount");
                    }
                    catch
                    {
                    }

                    //后续考虑是否更新配置发送邮件序号
                    return;
                }
                catch (Exception e)
                {
                }
            }


        }
        public string 获取邮件服务器(string address, ref int port)
        {
            if (address.Contains("@qq.com"))
            {
                port = 465;
                return "smtp.qq.com";
            }
            port = 465;
            return "smtp.163.com";
        }
    }
}
