using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace publiclib.Utils
{
    /// <summary>
    /// http请求
    /// </summary>
    public class TzRequest
    {
        public class ContetntType
        {
            public const string Json = "application/json";
            public const string Xml = "text/xml";
            public const string Text = "text/plain";
            public const string X_WWW_Form_Urlencoded = "application/x-www-form-urlencoded";
            public const string FormData = "multipart/form-data";
        }

        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static string HttpGet(string Url, string contentType)
        {
            try
            {
                string retString = string.Empty;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "GET";
                request.ContentType = contentType;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream myResponseStream = response.GetResponseStream();
                StreamReader streamReader = new StreamReader(myResponseStream);
                retString = streamReader.ReadToEnd();
                streamReader.Close();
                myResponseStream.Close();
                return retString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 发送WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <param name="encoding">编码</param>
        /// <param name="ContentType">参数格式</param>
        /// <param name="Method">POST|GET</param>
        /// <param name="Timeout">超时时间</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string Send(bool isReturn, string url, string postData, Encoding encoding, string ContentType, string Method, int Timeout)
        {
            encoding = encoding ?? Encoding.UTF8;
            ContentType = ContentType ?? TzRequest.ContetntType.X_WWW_Form_Urlencoded;
            Method = Method ?? "POST";

            string result = string.Empty;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(url);
            myReq.Method = Method;
            myReq.Timeout = Timeout;
            myReq.ContentType = ContentType;
            byte[] byteArray = encoding.GetBytes(postData);
            myReq.ContentLength = byteArray.Length;
            myReq.ServicePoint.ConnectionLimit = int.MaxValue;
            using (Stream reqStream = myReq.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }

            string returnString = string.Empty;
            if (isReturn)
            {
                using (HttpWebResponse response = (HttpWebResponse)myReq.GetResponse())
                {
                    using (Stream resStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            returnString = reader.ReadToEnd();
                        }
                    }
                }
            }
            result = returnString;
            return result;
        }

        /// <summary>
        /// 发送WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <param name="encoding">编码</param>
        /// <param name="ContentType">参数格式</param>
        /// <param name="Method">POST|GET</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string Send(bool isReturn, string url, string postData, Encoding encoding, string ContentType, string Method)
        {
            int Timeout = 10000;
            return Send(isReturn, url, postData, encoding, ContentType, Method, Timeout);
        }

        /// <summary>
        /// 发送WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <param name="encoding">编码</param>
        /// <param name="ContentType">参数格式</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string Send(bool isReturn, string url, string postData, Encoding encoding, string ContentType)
        {
            string Method = "POST";
            return Send(isReturn, url, postData, encoding, ContentType, Method);
        }

        /// <summary>
        /// 发送WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <param name="encoding">编码</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string Send(bool isReturn, string url, string postData, Encoding encoding)
        {
            string ContentType = "text/plain";
            return Send(isReturn, url, postData, encoding, ContentType);
        }

        /// <summary>
        /// 发送WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string Send(bool isReturn, string url, string postData)
        {
            Encoding encoding = Encoding.UTF8;
            return Send(isReturn, url, postData, encoding);
        }

        /// <summary>
        /// 带证书请求-常用于微信发红包
        /// </summary>
        /// <param name="requestUrl">请求的地址</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <param name="requestContent">请求的字符串</param>
        /// <param name="isPost">是否是POST</param>
        /// <param name="encoding">字符集编码</param>
        /// <param name="certificatePath">证书路径</param>
        /// <param name="certPassword">证书密码</param>
        /// <param name="msg">异常消息</param>
        /// <returns>请求结果</returns>
        public string SendRequestWithCertificate(string requestUrl, int timeout, string requestContent, bool isPost, string encoding, string certificatePath, string certPassword, out string msg)
        {
            msg = string.Empty;
            string result = string.Empty;
            try
            {
                string cert = string.Format(@"{0}", certificatePath);
                string password = certPassword;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                X509Certificate2 cer = new X509Certificate2(cert, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);//此处遇到一坑：在阿里云提供的服务器上用X509Certificate总是失败，改为X509Certificate2后成功


                byte[] bytes = System.Text.Encoding.GetEncoding(encoding).GetBytes(requestContent);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUrl);
                request.ClientCertificates.Add(cer);
                request.ContentType = "text/plain";// "application/x-www-form-urlencoded";
                request.Referer = requestUrl;
                request.Method = isPost ? "POST" : "GET";
                request.ContentLength = bytes.Length;
                request.Timeout = timeout * 1000;
                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream resStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(resStream, System.Text.Encoding.GetEncoding(encoding)))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message + ex.StackTrace;
            }


            return result;
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
                return true;
            return false;
        }

        public static string PostUrl(string url, string postData)
        {
            string result = string.Empty;

            System.Net.Http.HttpContent httpContent = new System.Net.Http.StringContent(postData);
            //设置Http的内容标头
            httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //设置Http的内容标头的字符
            httpContent.Headers.ContentType.CharSet = "utf-8";

            using (System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient())
            {
                //异步Post
                System.Net.Http.HttpResponseMessage response = httpClient.PostAsync(url, httpContent).Result;
                //输出Http响应状态码
                string statusCode = response.StatusCode.ToString();
                ////确保Http响应成功
                //if (response.IsSuccessStatusCode)
                //{
                //异步读取json
                result = response.Content.ReadAsStringAsync().Result;
                //}
            }

            return result;
        }

        /// <summary>
        /// 发送阿里云API- WebRequest请求,获取原返回
        /// </summary>
        /// <param name="isReturn">是否需要服务器响应</param>
        /// <param name="url">接口地址</param>
        /// <param name="postData">参数</param>
        /// <param name="header">请求头</param>
        /// <param name="encoding">编码</param>
        /// <param name="ContentType">参数格式</param>
        /// <param name="Method">POST|GET</param>
        /// <param name="Timeout">超时时间</param>
        /// <returns>{Code:int,Success:bool,Message:string,Data:object}</returns>
        public static string SendAliyun(bool isReturn, string url, string postData, WebHeaderCollection header, Encoding encoding = null, string ContentType = null, string Method = null, int Timeout = 10000)
        {
            encoding = encoding ?? Encoding.UTF8;
            ContentType = ContentType ?? TzRequest.ContetntType.Json;
            Method = Method ?? "POST";

            string result = string.Empty;
            ServicePointManager.DefaultConnectionLimit = int.MaxValue;
            HttpWebRequest myReq = (HttpWebRequest)HttpWebRequest.Create(url);
            myReq.Headers = header;
            myReq.Method = Method;
            myReq.Timeout = Timeout;
            myReq.ContentType = ContentType;
            byte[] byteArray = encoding.GetBytes(postData);
            myReq.ContentLength = byteArray.Length;
            myReq.ServicePoint.ConnectionLimit = int.MaxValue;
            using (Stream reqStream = myReq.GetRequestStream())
            {
                reqStream.Write(byteArray, 0, byteArray.Length);
            }

            string returnString = string.Empty;
            if (isReturn)
            {
                using (HttpWebResponse response = (HttpWebResponse)myReq.GetResponse())
                {
                    using (Stream resStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            returnString = reader.ReadToEnd();
                        }
                    }
                }
            }
            result = returnString;
            return result;
        }

        /// <summary>
        /// 创建阿里云请求Header
        /// </summary>
        /// <param name="AccessKeyId"></param>
        /// <param name="AccessSecret"></param>
        /// <param name="HttpUrl"></param>
        /// <param name="Body"></param>
        /// <param name="ContentType"></param>
        /// <param name="Accept"></param>
        /// <returns></returns>
        public static WebHeaderCollection CreateAliyunRequestHeader(string AccessKeyId, string AccessSecret, string HttpUrl, string Body, string ContentType = null, string Accept = null)
        {
            WebHeaderCollection header = new WebHeaderCollection();

            ContentType = ContentType ?? TzRequest.ContetntType.Json;
            Accept = Accept ?? TzRequest.ContetntType.Json;

            string date = DateTime.Now.ToUniversalTime().ToString("r");
            header.Add("Content-Type", ContentType);
            header.Add("Date", date);
            header.Add("Accept", Accept);

            string Signature = string.Empty;
            string StringToSign = $"POST\n{Accept}\n{TzMD5.MD5Encrypt(Body, Encoding.UTF8)}\n{ContentType}\n{date}{HttpUrl}";
            Signature = TzMD5.HMACSHA1Text(StringToSign, AccessSecret, Encoding.UTF8);
            header.Add("Authorization", $"Dataplus {AccessKeyId}:{Signature}");

            return header;
        }
    }

}
