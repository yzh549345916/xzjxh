using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace sjzd.天擎.util
{
    class SignGen
    {
        public static string getSign(Dictionary<string, string> signParams)
        {
            string sign = string.Empty;
            string paramString = string.Empty;
            signParams = signParams.OrderBy(o => o.Key, StringComparer.Ordinal).ToDictionary(o => o.Key, p => p.Value);
            foreach (var key in signParams)
            {
                if (key.ToString() != null && !key.ToString().Trim().Equals(""))
                {
                    paramString += key.Key + "=" + key.Value.Trim();
                    paramString += "&";
                }
            }
            if (!paramString.Equals(string.Empty))
            {
                paramString = paramString.Substring(0, paramString.Length - 1);
            }
            //对参数字符串进行MD5加密
            MD5 md5 = MD5.Create();
            byte[] bs = Encoding.UTF8.GetBytes(paramString);
            byte[] hs = md5.ComputeHash(bs);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hs)
            {
                sb.Append(b.ToString("X2"));
             }
            sign = sb.ToString();
            return sign;
        }
    }
}
