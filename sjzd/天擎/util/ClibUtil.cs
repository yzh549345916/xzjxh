using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cma.Music;

namespace sjzd.天擎.util
{
    class ClibUtil
    {
        /*
         * 输出返回结果，RetArray2D
        */
        public void outputRst(RetArray2D retArray2D)
        {
            /* 1. 请求信息 */
            Console.WriteLine("请求信息：request，所属类：RequestInfo");
            Console.WriteLine("\t" + "成员：");
            Console.WriteLine("\t" + "returnCode（返回码）：" + retArray2D.request.errorCode);
            Console.WriteLine("\t" + "returnMessage（提示信息）：" + retArray2D.request.errorMessage);
            Console.WriteLine("\t" + "requestElems（检索请求的要素）：" + retArray2D.request.requestElems);
            Console.WriteLine("\t" + "requestParams（检索请求的参数）：" + retArray2D.request.requestParams);
            Console.WriteLine("\t" + "requestTime（请求发起时间）：" + retArray2D.request.requestTime);
            Console.WriteLine("\t" + "responseTime（请求返回时间）：" + retArray2D.request.responseTime);
            Console.WriteLine("\t" + "takeTime（请求耗时，单位ms）：" + retArray2D.request.takeTime);
            Console.WriteLine();

            /* 2. 返回的数据  */
            Console.WriteLine("结果集：retArray2D，所属类：RetArray2D");
            if (retArray2D.data.Count == 0)
            {
                Console.Write("\t成员：data，类：String[][]，值(记录数：0)\n");
            }
            else
            {
                Console.Write("\t成员：data，类：String[][]，值(记录数：{0}，要素数：{1}）：\n", retArray2D.data.Count, retArray2D.data[0].Length);
                //遍历数据：retArray2D.data
                //行数：retArray2D.data.length
                Console.WriteLine("\t---------------------------------------------------------------------");
                for (int iRow = 0; iRow < retArray2D.data.Count; iRow++)
                {
                    Console.Write("\t");
                    //列数： retArray2D.data[iRow].length
                    for (int iCol = 0; iCol < retArray2D.data[iRow].Length; iCol++)
                    {
                        Console.Write(retArray2D.data[iRow][iCol] + "\t");
                    }
                    Console.WriteLine();
                    //DEMO中，最多只输出10行
                    if (iRow > 10)
                    {
                        Console.WriteLine("\t......");
                        break;
                    }
                }
            }
            Console.WriteLine("\t---------------------------------------------------------------------");
        }


        /*
         * 输出返回结果，RetGridArray2D
         */
        public void outputRst(RetGridArray2D retGridArray2D)
        {
            /* 1. 请求信息 */
            Console.WriteLine("请求信息：request，所属类：RequestInfo");
            Console.WriteLine("\t" + "成员：");
            Console.WriteLine("\t" + "returnCode（返回码）：" + retGridArray2D.request.errorCode);
            Console.WriteLine("\t" + "returnMessage（提示信息）：" + retGridArray2D.request.errorMessage);
            Console.WriteLine("\t" + "requestElems（检索请求的要素）：" + retGridArray2D.request.requestElems);
            Console.WriteLine("\t" + "requestParams（检索请求的参数）：" + retGridArray2D.request.requestParams);
            Console.WriteLine("\t" + "requestTime（请求发起时间）：" + retGridArray2D.request.requestTime);
            Console.WriteLine("\t" + "responseTime（请求返回时间）：" + retGridArray2D.request.responseTime);
            Console.WriteLine("\t" + "takeTime（请求耗时，单位ms）：" + retGridArray2D.request.takeTime);
            Console.WriteLine();

            /* 2. 返回的数据  */
            Console.WriteLine("结果集：retGridArray2D，所属类：RetGridArray2D");
            Console.WriteLine("\t" + "成员：startLat（网格起始纬度）：" + retGridArray2D.startLat);
            Console.WriteLine("\t" + "成员：endLat（网格终止纬度）：" + retGridArray2D.endLat);
            Console.WriteLine("\t" + "成员：startLon（网格起始经度）：" + retGridArray2D.startLon);
            Console.WriteLine("\t" + "成员：endLon（网格终止经度）：" + retGridArray2D.endLon);
            Console.WriteLine("\t" + "成员：latCount（纬向格点数）：" + retGridArray2D.latCount);
            Console.WriteLine("\t" + "成员：lonCount（经向格点数）：" + retGridArray2D.lonCount);
            Console.WriteLine("\t" + "成员：latStep（纬向格距）：" + retGridArray2D.latStep);
            Console.WriteLine("\t" + "成员：lonStep（经向格距）：" + retGridArray2D.lonStep);

            Console.Write("\t成员：data，类：String[][]，值(行：{0}，列：{1}）：\n", retGridArray2D.latCount, retGridArray2D.lonCount);
            //遍历数据：retArray2D.data
            //行数：retArray2D.data.length
            Console.WriteLine("\t-------------------------------------------------------------------------------------------------------------");
            Console.Write("\t{0}", "纬度/经度|");
            float lon = retGridArray2D.startLon;
            for (int iLon = 0; iLon < retGridArray2D.lonCount; iLon++)
            {
                //保留4为小数
                Console.Write("{0:N4}|", lon);
                lon += retGridArray2D.lonStep;
            }
            Console.WriteLine();

            Console.Write("\t----------");
            for (int iLon = 0; iLon < retGridArray2D.lonCount; iLon++)
            {
                Console.Write("----------");
            }
            Console.WriteLine();

            //遍历数据：retArray2D.data
            float lat = retGridArray2D.startLat;
            //行数：retGridArray2D.data.length
            for (int iLat = 0; iLat < retGridArray2D.latCount; iLat++)
            {
                //保留4为小数
                Console.Write("\t{0:N4}|", lat);
                //列数：  retGridArray2D.lonCount
                for (int iLon = 0; iLon < retGridArray2D.lonCount; iLon++)
                {
                    //保留4为小数
                    Console.Write("{0:N4}|", retGridArray2D.data[iLat][iLon]);
                }
                Console.WriteLine();
                lat += retGridArray2D.latStep;

                //DEMO中，最多只输出10行
                if (iLat > 10)
                {
                    Console.WriteLine("\t......");
                    break;
                }
            }

            Console.Write("\t----------");
            for (int iLon = 0; iLon < retGridArray2D.lonCount; iLon++)
            {
                Console.Write("----------");
            }

        }

        /*
         * 输出返回结果，RetFilesInfo
         */
        public void outputRst(RetFilesInfo retFilesInfo)
        {
            /* 1. 请求信息 */
            Console.WriteLine("请求信息：request，所属类：RequestInfo");
            Console.WriteLine("\t" + "成员：");
            Console.WriteLine("\t" + "returnCode（返回码）：" + retFilesInfo.request.errorCode);
            Console.WriteLine("\t" + "returnMessage（提示信息）：" + retFilesInfo.request.errorMessage);
            Console.WriteLine("\t" + "requestElems（检索请求的要素）：" + retFilesInfo.request.requestElems);
            Console.WriteLine("\t" + "requestParams（检索请求的参数）：" + retFilesInfo.request.requestParams);
            Console.WriteLine("\t" + "requestTime（请求发起时间）：" + retFilesInfo.request.requestTime);
            Console.WriteLine("\t" + "responseTime（请求返回时间）：" + retFilesInfo.request.responseTime);
            Console.WriteLine("\t" + "takeTime（请求耗时，单位ms）：" + retFilesInfo.request.takeTime);
            Console.WriteLine();

            /* 2. 返回的数据  */
            Console.WriteLine("结果集：retFilesInfo，所属类：RetFilesInfo");
            Console.Write("\t" + "成员：fileInfos，类：FileInfo[]，值(记录数：{0}）：\n", retFilesInfo.fileInfos.Length);
            //遍历数据：retFilesInfo.fileInfos
            //行数：retFilesInfo.fileInfos.length
            Console.WriteLine("\t---------------------------------------------------------------------");
            for (int iRow = 0; iRow < retFilesInfo.fileInfos.Length; iRow++)
            {
                Console.Write("\t");
                Console.Write("\t" + "fileName（文件名）：" + retFilesInfo.fileInfos[iRow].fileName);
                //在仅获取文件信息列表的接口中，savePath为空
                Console.Write("\t" + "savePath（下载到客户端的全路径）：" + retFilesInfo.fileInfos[iRow].savePath);
                Console.Write("\t" + "suffix（文件后缀）：" + retFilesInfo.fileInfos[iRow].suffix);
                Console.Write("\t" + "size（文件大小byte）：" + retFilesInfo.fileInfos[iRow].size);
                Console.Write("\t" + "fileUrl（服务端文件url）：" + retFilesInfo.fileInfos[iRow].fileUrl);
                //对非图片文件，该成员为空；对图片文件，可为空（使用fileUrl）。
                Console.Write("\t" + "imgBase64（图片文件base64编码）：" + retFilesInfo.fileInfos[iRow].imgBase64);
                Console.WriteLine();
                //DEMO中，最多只输出10行
                if (iRow > 10)
                {
                    Console.WriteLine("\t......");
                    break;
                }
            }
            Console.WriteLine("\t---------------------------------------------------------------------");
        }
    }
}
