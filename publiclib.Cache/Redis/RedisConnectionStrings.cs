using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using publiclib.Config;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace publiclib.Cache.Redis
{
    public class RedisConnectionStrings
    {

        private static Dictionary<string, string> _ConnectionStrings;

        ////此处可以用构造函数初始化ConnectionStrings
        //public static Dictionary<string, string> ConnectionStrings
        //{
        //    get
        //    {
        //        if (_ConnectionStrings == null)
        //        {
        //            string json = CacheUtils.PostUrl(System.Configuration.ConfigurationManager.AppSettings["ConnectionStrings"], "");
        //            if (!string.IsNullOrEmpty(json))
        //            {
        //                _ConnectionStrings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
        //            }
        //        }
        //        return _ConnectionStrings;
        //    }
        //}

        public static IConfiguration configuration;

        #region 云车场基本信息缓存
        private static string _RedisSysCustomKey;

        /// <summary>
        /// Redis保存的Key前缀，会自动添加到指定的Key名称前
        /// </summary>
        public static string RedisSysCustomKey
        {
            get
            {
                if (string.IsNullOrEmpty(_RedisSysCustomKey))
                    _RedisSysCustomKey = AppSettingConfig.RedisKey;
                return _RedisSysCustomKey;
            }
        }
        /// <summary>
        /// 当前连接的Redis中的DataBase索引，默认0-16，可以在service.conf配置，最高64
        /// </summary>
        private static int? _RedisDataBaseIndex;
        public static int RedisDataBaseIndex
        {
            get
            {
                if (_RedisDataBaseIndex == null)
                    _RedisDataBaseIndex = Convert.ToInt32(AppSettingConfig.RedisDataBaseIndex);
                return Convert.ToInt32(_RedisDataBaseIndex);
            }
        }

        //连接格式为：127.0.0.1:6379,allowadmin=true,password=pwd

        /// <summary>
        /// 当前连接的Redis中连接字符串：（此库为默认库，存储订单）
        /// </summary>
        private static string _DynamicCacheConnection;
        public static string DynamicCacheConnection
        {
            get
            {
                if (string.IsNullOrEmpty(_DynamicCacheConnection))
                    _DynamicCacheConnection = AppSettingConfig.RedisHostConnection;
                return _DynamicCacheConnection;
            }
        }
        #endregion

    }
}
