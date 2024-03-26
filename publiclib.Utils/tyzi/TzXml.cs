using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;

namespace publiclib.Utils
{
    public class TzXml
    {
        public static T ToModel<T>(string xml)
        {
            StringReader xmlReader = new StringReader(xml);
            XmlSerializer xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(xmlReader);
        }

        public static string ToXml<T>(T model)
        {
            MemoryStream stream = new MemoryStream();
            XmlSerializer xmlSer = new XmlSerializer(typeof(T));
            xmlSer.Serialize(stream, model);

            stream.Position = 0;
            StreamReader sr = new StreamReader(stream);
            return sr.ReadToEnd();
        }

        public static string ToXml(Dictionary<string, object> model)
        {
            string xml = "<xml>";
            foreach (KeyValuePair<string, object> item in model)
            {
                xml += $"<{item.Key}>[DATA[{item.Value}]]<{item.Key}>";
            }
            return xml;
        }

        public static DataTable ToTable(string xml)
        {
            StringReader xmlReader = new StringReader(xml);
            DataSet ds = new DataSet();
            ds.ReadXml(xmlReader);
            return ds.Tables[0];
        }

        public static string XmlAnalysis(string stringRoot, string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc.DocumentElement.SelectSingleNode(stringRoot).InnerXml.Trim();
        }

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

    }

}
