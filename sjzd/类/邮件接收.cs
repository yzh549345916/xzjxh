using Aspose.Email.Clients.Imap;
using Aspose.Email.Tools.Search;
using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Email;
using Aspose.Email.Clients;
using Aspose.Email.Clients.Base;
using Aspose.Email.Clients.Pop3;

namespace sjzd.类
{
    class 邮件接收
    {
        public static void 查看气象台163()
        {
            // Connect and log in to POP3
            const string host = "imap.163.com";
            const int port = 993;
            const string username = "hsqxjdzzh@163.com";
            const string password = "MTNOZEPSJGLAGLNZ";
            ImapClient client = new ImapClient(host, port, username, password);

            try
            {
                client.SupportedEncryption = EncryptionProtocols.Tls;
                client.SecurityOptions = SecurityOptions.SSLImplicit;
                ImapMessageInfoCollection list = client.ListMessages();
               
               
                MailQueryBuilder builder = new MailQueryBuilder();

                // ExStart:CombineQueriesWithAND
                // Emails from specific host, get all emails that arrived before today and all emails that arrived since 7 days ago
               // builder.From.Contains("yewu_zzy@163.com");
                builder.InternalDate.Before(DateTime.Now);
                builder.InternalDate.Since(DateTime.Now.AddDays(-7));
                // ExEnd:CombineQueriesWithAND

                // Build the query and Get list of messages
                MailQuery query = builder.GetQuery();
                ImapMessageInfoCollection messages = client.ListMessages(query);
                Console.WriteLine("Imap: " + messages.Count + " message(s) found.");

                builder = new MailQueryBuilder();

                // ExStart:CombiningQueriesWithOR
                // Specify OR condition
                builder.Or(builder.Subject.Contains("test"), builder.From.Contains("noreply@host.com"));
                // ExEnd:CombiningQueriesWithOR

                // Build the query and Get list of messages
                query = builder.GetQuery();
                messages = client.ListMessages(query);
                Console.WriteLine("Imap: " + messages.Count + " message(s) found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        public static void 查看气象台163pop3()
        {
            // Connect and log in to POP3
            const string host = "pop.163.com";
            const int port = 995;
            const string username = "hsqxjdzzh@163.com";
            const string password = "MTNOZEPSJGLAGLNZ";
            Pop3Client client = new Pop3Client(host, port, username, password);
            try
            {
                
                string dstEmail = Environment.CurrentDirectory + @"\临时\" + "InsertHeaders.eml";
                string dataDir = Environment.CurrentDirectory + @"\临时\" ;
                // ExStart:ApplyCaseSensitiveFilters
                // IgnoreCase is True
                MailQueryBuilder builder1 = new MailQueryBuilder();
                builder1.From.Contains("yewu_zzy", true);
                builder1.InternalDate.Before(DateTime.Now.AddDays(1));
                builder1.InternalDate.Since(DateTime.Now.AddDays(-7));
                MailQuery query1 = builder1.GetQuery();
                Pop3MessageInfoCollection messageInfoCol1 = client.ListMessages(query1);
                MailMessage msg = client.FetchMessage(1);
                foreach (var item in msg.Attachments)
                {
                    item.Save(dataDir+ item.Name);
                }
                msg.Save(dataDir + "first-1.eml");
                //client.SaveMessage(1, dstEmail);
                client.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
