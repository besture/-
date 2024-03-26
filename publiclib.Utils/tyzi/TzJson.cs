using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace publiclib.Utils
{
    public class TzJson
    {
        public class LowercaseContractResolver : Newtonsoft.Json.Serialization.DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
        public static string ToString(object obj, bool isNull = false)
        {
            JsonSerializerSettings jsSet = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
            if (isNull)
                return JsonConvert.SerializeObject(obj);
            else
                return JsonConvert.SerializeObject(obj, jsSet);
        }
        public static string ToStringLower(object obj, bool isNull = false)
        {
            JsonSerializerSettings jsSet = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include, ContractResolver = new LowercaseContractResolver() };
            if (isNull)
                return JsonConvert.SerializeObject(obj);
            else
                return JsonConvert.SerializeObject(obj, jsSet);
        }
        public static T ToObject<T>(string json)
        {
            try
            {
                T model = JsonConvert.DeserializeObject<T>(json);
                return model;
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static T1 ToObject<T1, T2>(T2 t2)
        {
            try
            {
                string js = JsonConvert.SerializeObject(t2);
                T1 model = JsonConvert.DeserializeObject<T1>(js);
                return model;
            }
            catch (Exception ex)
            {
                return default(T1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T2"></typeparam>
        /// <param name="t2"></param>
        /// <param name="isNull">是否包含null值：true-包含,false-null值字段不显示</param>
        /// <returns></returns>
        public static T1 ToObject<T1, T2>(T2 t2, bool isNull)
        {
            try
            {
                string js = ToString(t2, isNull);
                var model = JsonConvert.DeserializeObject<T1>(js);
                return model;
            }
            catch (Exception ex)
            {
                return default(T1);
            }
        }

        public static string ToDtString(DataTable dt)
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> item = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    item.Add(col.ColumnName, row[col.ColumnName]);
                }
                lst.Add(item);
            }
            return ToString(lst);
        }

        public static DataTable ToDataTable<T>(IEnumerable<T> array)
        {
            DataTable result = new DataTable();
            foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
            {
                if (pd.PropertyType.IsGenericType && pd.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    result.Columns.Add(pd.Name, Nullable.GetUnderlyingType(pd.PropertyType));
                else
                    result.Columns.Add(pd.Name, pd.PropertyType);
            }
            foreach (T item in array)
            {
                DataRow row = result.NewRow();
                foreach (PropertyDescriptor pd in TypeDescriptor.GetProperties(typeof(T)))
                    row[pd.Name] = pd.GetValue(item) ?? DBNull.Value;
                result.Rows.Add(row);
            }
            return result;
        }

        public static T ToModel<T>(Dictionary<string, object> item)
        {
            string json = ToString(item, true);
            return ToModel<T>(json);
        }

        public static T ToModel<T>(string json)
        {
            return ToObject<T>(json);
        }

        public static SortedDictionary<string, object> DynamicSort(JObject obj)
        {
            var res = new SortedDictionary<string, object>();
            foreach (var x in obj)
            {
                if (x.Value is JValue) res.Add(x.Key, x.Value);
                else if (x.Value is JObject) res.Add(x.Key, DynamicSort((JObject)x.Value));
                else if (x.Value is JArray)
                {
                    var tmp = new SortedDictionary<string, object>[x.Value.Count()];
                    for (var i = 0; i < x.Value.Count(); i++)
                    {
                        tmp[i] = DynamicSort((JObject)x.Value[i]);
                    }
                    res.Add(x.Key, tmp);
                }
            }
            return res;
        }

        /// <summary>
        /// 序列化json
        /// </summary>
        /// <typeparam name="T">model</typeparam>
        /// <param name="obj">数据</param>
        /// <param name="code">字段处理: 0-不处理,1-转小写,2-转大写</param>
        /// <returns></returns>
        public static string ToString<T>(T obj, int code)
        {
            var o = ToString(obj);
            var d = ToObject<Dictionary<string, object>>(o);

            Dictionary<string, object> data = new Dictionary<string, object>();
            switch (code)
            {
                case 1:
                    foreach (var item in d)
                    {
                        data.Add(item.Key.ToLower().Replace("_", ""), item.Value);
                    }
                    break;
                case 2:
                    foreach (var item in d)
                    {
                        data.Add(item.Key.ToUpper().Replace("_", ""), item.Value);
                    }
                    break;
                default: break;
            }

            return ToString(data);
        }
    }
}
