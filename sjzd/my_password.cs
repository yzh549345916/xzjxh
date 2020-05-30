using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace sjzd
{
    public class my_password
    {
        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetMD5HashFromFile(string fileName)
        {
            string tmp_md5 = "";
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                tmp_md5 = sb.ToString();
            }
            catch (Exception)
            {
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
            return tmp_md5;
        }
        /// <summary>
        /// md5
        /// </summary>
        public static string Encrypt_md5(string strPwd)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.Default.GetBytes(strPwd);//将字符编码为一个字节序列 
            byte[] md5data = md5.ComputeHash(data);//计算data字节数组的哈希值 
            md5.Clear();
            string str = "";
            for (int i = 0; i < md5data.Length - 1; i++)
            {
                str += md5data[i].ToString("x").PadLeft(2, '0');
            }
            return str;
        }

        /// <summary>   
        /// IV随便设置为8位   
        /// </summary>   
        private static byte[] IV = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>   
        /// 加密字符串   
        /// </summary>   
        /// <returns>返回密文</returns>   
        public static String EncryptString(String key, String str)
        {
            try
            {
                byte[] data = Encoding.Default.GetBytes(str);
                byte[] result = EncryptData(key, data);
                if (result != null)
                    return Convert.ToBase64String(result, 0, result.Length);
                else
                    return "";
            }
            catch (System.Exception)
            {
                return "";
            }
        }
        /// <summary>   
        /// 加密二进制数据   
        /// </summary>   
        /// <returns>返回二进制密文</returns>   
        public static byte[] EncryptData(String key, byte[] data)
        {
            byte[] bKey = Encoding.Default.GetBytes(key.Substring(0, 8));
            byte[] bIV = IV;
            try
            {
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                desc.Mode = CipherMode.ECB;//兼容其他语言的Des加密算法   
                desc.Padding = PaddingMode.Zeros;//自动补0   
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateEncryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            catch
            {
                return null;
            }
        }
        /// <summary>   
        /// 把密文解密为明文   
        /// </summary>   
        /// <returns>返回明文</returns>   
        public static String DecryptString(String key, String decryptStr)
        {
            try
            {
                byte[] data = Convert.FromBase64String(decryptStr);
                byte[] result = DecryptData(key, data);
                if (result != null)
                    return Encoding.Default.GetString(result, 0, result.Length).Replace("\0", "");
                else
                    return "";
            }
            catch (System.Exception)
            {
                return "";
            }

        }

        /// <summary>   
        /// 把二进制密文解密为明文二进制   
        /// </summary>   
        /// <returns>返回明文二进制</returns>   
        public static byte[] DecryptData(String key, byte[] data)
        {
            try
            {
                byte[] bKey = Encoding.Default.GetBytes(key.Substring(0, 8));
                byte[] bIV = IV;
                DESCryptoServiceProvider desc = new DESCryptoServiceProvider();
                desc.Mode = CipherMode.ECB;//兼容其他语言的Des加密算法   
                desc.Padding = PaddingMode.Zeros;//自动补0   
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, desc.CreateDecryptor(bKey, bIV), CryptoStreamMode.Write);
                cStream.Write(data, 0, data.Length);
                cStream.FlushFinalBlock();
                return mStream.ToArray();
            }
            catch
            {
                return null;
            }
        }
    }
}
