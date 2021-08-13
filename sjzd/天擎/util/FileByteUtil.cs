using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
namespace sjzd.天擎.util
{
    class FileByteUtil
    {
        public static byte[] httpDownloadByte(String httpUrl)
        {
            String url = null;
            byte[] bt;
            try
            {
                url = httpUrl;
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Referer = "http://";
                HttpWebResponse rs = (HttpWebResponse)req.GetResponse();
                System.IO.Stream st = rs.GetResponseStream();
                st.Flush();
                using (MemoryStream ms = new MemoryStream())
                {
                    int b;
                    while ((b = st.ReadByte()) != -1)
                    {
                        ms.WriteByte((byte)b);
                    }
                    bt = ms.ToArray();
                }
            }
            catch (IOException e)
            {
                throw new System.Exception("Get file error", e);
            }

            return bt;

        } //end public static byte[] httpDownloadByte



    }
}
