using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace publiclib.Utils
{
    public class TzSign
    {

        /// <summary>
        /// 将xml转化为Dictionary
        /// </summary>
        /// <param name="xml">标准键值xml</param>
        /// <param name="rootName">根节点名</param>
        public static Dictionary<string, object> ToDictionary(string xml, string rootName = "xml")
        {
            Dictionary<string, object> item = new Dictionary<string, object>();
            XmlDocument xd = new XmlDocument();
            xd.LoadXml(xml);
            XmlNode xroot = xd.SelectSingleNode(rootName);
            XmlNodeList xnList = xroot.ChildNodes;
            foreach (XmlNode xn in xnList)
            {
                item.Add(xn.Name, xn.InnerText);
            }
            return item;
        }

        /// <summary>
        /// 将Dictionary转化为字典序的QueryString,字符串最后会多一个&   key=value&....
        /// </summary>
        /// <param name="item"></param>
        /// <param name="notNull">是否包含null值---true:不能包含null值，false:需要包含null</param>
        /// <returns></returns>
        public static string ToQuery(Dictionary<string, object> item, bool isNull, string filter = "key")
        {
            string result = string.Empty;
            ArrayList akeys = new ArrayList(item.Keys);
            akeys.Sort(); //按字母顺序进行排序
            foreach (string skey in akeys)
            {
                if (skey.ToLower().Equals(filter) || skey.ToLower().Equals("sign")) continue;
                if (isNull)
                {
                    if (item[skey] != null && !string.IsNullOrEmpty(item[skey].ToString()))
                        result += string.Format("{0}={1}&", skey, item[skey]);
                }
                else
                {
                    result += string.Format("{0}={1}&", skey, item[skey]);
                }
            }
            return result;
        }

        public static string ToSort(dynamic param)
        {
            string result = string.Empty;
            string pjson = JsonConvert.SerializeObject(param);
            var j = JObject.Parse(pjson);
            var target = TzJson.DynamicSort(j);
            return JsonConvert.SerializeObject(target);
        }

        /// <summary>
        /// 将xml转化为字典序的QueryString,字符串最后会多一个&    key=value&....
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="notNull">是否包含null值---true:不能包含null值，false:需要包含null</param>
        /// <returns></returns>
        public static string ToQuery(string xml, bool isNull)
        {
            string result = string.Empty;
            Dictionary<string, object> item = ToDictionary(xml);
            result = ToQuery(item, isNull, "key");
            return result;
        }

        /// <summary>
        /// 字典序的QueryString,字符串最后会多一个&
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="toLower">属性名是否转为小写</param>
        /// <returns></returns>
        public static string ToQuery<T>(T model, bool toLower = false)
        {
            string result = string.Empty;
            Hashtable htb = new Hashtable();
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            foreach (PropertyInfo item in PropertyList)
            {
                htb.Add(item.Name, item.GetValue(model, null));
            }
            ArrayList akeys = new ArrayList(htb.Keys);
            akeys.Sort(); //按字母顺序进行排序
            foreach (string skey in akeys)
            {
                if (skey.ToLower().Equals("key") || skey.ToLower().Equals("sign")) continue;
                var nskey = toLower ? skey.ToLower() : skey;
                if (htb[skey] != null)
                    result += string.Format("{0}={1}&", nskey, htb[skey]);
            }
            return result;
        }

        public static string ToSign<T>(T model, string key, bool toLower = false, int type = 0)
        {
            string query = ToQuery<T>(model, toLower);
            return ToSign(query, key, type);
        }

        public static string ToSign(Dictionary<string, object> item, string key, bool isNull = false, int type = 0)
        {
            string query = ToQuery(item, isNull, "key");
            return ToSign(query, key, type);
        }

        public static string ToSign(string xml, string key, bool isNull = false, int type = 0)
        {
            string query = ToQuery(xml, isNull);
            return ToSign(query, key, type);
        }

        public static string AuthSign(dynamic param)
        {
            string sort = ToSort(param);
            return TzMD5.MD5Encrypt(sort, Encoding.UTF8);
        }

        /// <summary>
        /// 返回签名字符串
        /// </summary>
        /// <param name="query"></param>
        /// <param name="key">加密密钥</param>
        /// <param name="type">0:MD5,1:SHA256</param>
        /// <returns></returns>
        public static string ToSign(string query, string key, int type = 0)
        {
            if (!string.IsNullOrEmpty(key))
                query = query + "key=" + key;
            if (type == 0)
                return TzMD5.MD5Encrypt(query, Encoding.UTF8);
            else if (type == 1)
                return TzMD5.SHA256(query, Encoding.UTF8);
            return "";
        }

        public static string ToSign(Dictionary<string, object> item, string charset, string key, bool isNull = false, int type = 0)
        {
            string query = ToQuery(item, isNull, "key");
            return ToSign(query, charset, key, type);
        }

        /// <summary>
        /// 返回签名字符串
        /// </summary>
        /// <param name="query"></param>
        /// <param name="key">加密密钥</param>
        /// <param name="type">0:MD5,1:SHA256</param>
        /// <returns></returns>
        public static string ToSign(string query, string charset, string key, int type = 0)
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Encoding encoding = Encoding.GetEncoding(charset.ToUpper());
            if (!string.IsNullOrEmpty(key))
                query = query + "key=" + key;
            if (type == 0)
                return TzMD5.MD5Encrypt(query, encoding);
            else if (type == 1)
                return TzMD5.SHA256(query, encoding);
            return "";
        }

    }
}
