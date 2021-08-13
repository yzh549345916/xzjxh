using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Cma.Music.Client;
using music_sdk_net;
using Cma.Music;

namespace sjzd.天擎.util
{
    class RestUtil
    {
       /* 
        * TODO:REST服务地址，依具体环境设置(如果使用负载均衡地址比如10.40.17.54，端口号为80,可不写端口号)
        */
        const string host = "10.40.17.25:28408";//负载均衡地址：10.40.17.54
        const int timeoutInMilliSeconds =  1000 * 60 * 2 ; //2 MINUTE
        static string getwayFlag = "\"flag\":\"slb\""; //网关返回错误标识

        /*
         * REST请求服务，获取数据
         */
        public String getRestData(string param) {
      HttpWebRequest request;
      HttpWebResponse response = null;
      StreamReader reader;
      StringBuilder sbSource=null;
      if (param == null) { throw new ArgumentNullException("param"); }
      try
      {
          /* 建立web请求*/
          request = WebRequest.Create("http://" + host + "/music-ws/api?" + param) as HttpWebRequest;
          Console.WriteLine("http://" + host + "/music-ws/api?" + param);
          request.Timeout = timeoutInMilliSeconds;
          // Get response  
          response = request.GetResponse() as HttpWebResponse;
          if (request.HaveResponse == true && response != null)
          {
              // Get the response stream  
              reader = new StreamReader(response.GetResponseStream());

              // Read it into a StringBuilder  
              sbSource = new StringBuilder(reader.ReadToEnd());
          }  
      }
      catch (WebException wex)
      {
          if (wex.Response != null)
          {
              using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
              {
                  Console.WriteLine(
                      "The server returned '{0}' with the status code {1} ({2:d}).",
                      errorResponse.StatusDescription, errorResponse.StatusCode,
                      errorResponse.StatusCode);
              }
          }  
      }
      finally
      {
          if (response != null) { response.Close(); }
      } 

      return Convert.ToString(sbSource);
     }
             /*
       * REST请求服务，保存数据
       */
      public string setRestData(string @params,string inString) {
        byte[] retBytes = null;
        string returnStr = "";
        HttpWebRequest request;
        Stream responseStream = null;
        //@params=@params+"&instring="+inString;
        try {
            List<string[]> inputData = new List<string[]>();
            inputData.Add(new string[] { inString });
            byte[] storeData = new DataFormatChange().getPbStoreArray2DBytes(inputData, 0, null, 0, "", "");
            /* 建立web请求*/
            request = WebRequest.Create("http://" + host + "/music-ws/write?" + @params) as HttpWebRequest;
            Console.WriteLine("http://" + host + "/music-ws/write?" + @params);
            request.Timeout = timeoutInMilliSeconds;
            request.ContentType = "multipart/form-data";
            request.Method = "POST";
            request.ContentLength = storeData.Length;
            request.AllowAutoRedirect = true;
            Stream requestStream = request.GetRequestStream();
            //values = Encoding.GetEncoding("UTF-8").GetBytes(values);
            requestStream.Write(storeData, 0, storeData.Length);
            requestStream.Close();
            
            responseStream = request.GetResponse().GetResponseStream();
            int b = -1;
            List<byte> retByteList = new List<byte>();
            while ((b = responseStream.ReadByte()) != -1)
            {
                retByteList.Add((byte)b);
            }
            retBytes = retByteList.ToArray();
            if ((!retBytes.Equals(null)) && (retBytes.Length > 0))
            {
                string retStr = System.Text.Encoding.Default.GetString(retBytes);
                if (retStr.Contains(getwayFlag))
                {
                    GatewayInfo gatewayInfo = new GatewayInfo();
                    if (JsonUtil.ParseJson(retStr, gatewayInfo))
                    {
                        returnStr = gatewayInfo.ReturnCode + ":" + gatewayInfo.ReturnMessage;
                    }
                    else
                    {
                        returnStr = "-10001:parse slb string error," + retStr;
                    }
                }
                else
                {
                        Cma.Music.Pb.RequestInfo pbRequestInfo = new Cma.Music.Pb.RequestInfo();
                        pbRequestInfo = Cma.Music.Pb.RequestInfo.Parser.ParseFrom(retBytes);
                        RequestInfo requestInfo = new DataFormatChange().getRequestInfo(pbRequestInfo);
                        returnStr = requestInfo.errorCode + ":" + requestInfo.errorMessage;
                }
            }
            else
            {
                    returnStr = "-10001:write data response is null or empty";
            }
        }
        catch (WebException wex)
        {
            if (wex.Response != null)
            {
                using (HttpWebResponse errorResponse = (HttpWebResponse)wex.Response)
                {
                    Console.WriteLine(
                        "The server returned '{0}' with the status code {1} ({2:d}).",
                        errorResponse.StatusDescription, errorResponse.StatusCode,
                        errorResponse.StatusCode);
                }
            }
        }
        catch (System.Exception e)
        {
             Console.WriteLine(string.Format("POST操作发生异常：{0}", e.Message));
             throw e;
        }
        finally
        {
            if (responseStream != null) { responseStream.Close(); }
        }
        return returnStr;
      }
    }
}
