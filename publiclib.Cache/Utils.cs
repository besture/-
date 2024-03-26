using System.Net;
using System.Text;

namespace publiclib.Cache
{
    public class CacheUtils
    {
        public static string PostUrl(string url, string postDataStr)
        {
            string result = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                request.Timeout = 5000;

                byte[] data = Encoding.UTF8.GetBytes(postDataStr);
                using (Stream reqStream = request.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream resStream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(resStream, Encoding.UTF8))
                        {
                            result = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"无法读取配置：{url}," + ex.ToString());
            }
            return result;
        }
    }
}
