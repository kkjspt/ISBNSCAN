using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ISBNSCAN
{
    class submitDataToServer
    {
        /// <summary>
        /// 向指定的kkjspt.com服务器提交数据,服务器端已经做了去重处理
        /// </summary>
        /// <param name="postString"></param>
        public static void loadUrl2(string postString)
        {
            ////string url = "http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/isbnsubmit/?" + postData;
            ////HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            ////request.Timeout = 7000;
            ////HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            //string url = "http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/isbnsubmit/?";

            //// ASCIIEncoding encoding = new ASCIIEncoding();
            //Encoding encoding = Encoding.GetEncoding("utf-8");
            //byte[] data = encoding.GetBytes(postData);

            //// Prepare web request
            //HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
            //myRequest.Method = "POST";
            //myRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            //myRequest.ContentLength = data.Length;
            //Stream newStream = myRequest.GetRequestStream();
            //// Send the data.
            //newStream.Write(data, 0, data.Length);
            //newStream.Close();
            
            // postString = "arg1=a&arg2=b";//这里即为传递的参数，可以用工具抓包分析，也可以自己分析，主要是form里面每一个name都要加进来  
            byte[] postData = Encoding.UTF8.GetBytes(postString);//编码，尤其是汉字，事先要看下抓取网页的编码方式  
            string url = "http://www.kkjspt.cn/kukusoft/isbnsearch/V1.0.3/isbnsubmit/?";
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可  
            byte[] responseData = webClient.UploadData(url, "POST", postData);//得到返回字符流  
            string srcString = Encoding.UTF8.GetString(responseData);//解码 
        }

        public static string loadUrl(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return ret;
        }
    }
}
