using Microsoft.Extensions.Configuration;

namespace publiclib.Config
{
    /// <summary>
    /// 系统公有配置参数
    /// </summary>
    public class AppSettingConfig
    {

        public static IConfiguration Config = AppSettingsManager.GetAppSettings();

        public static string GetAppConfig(string strKey)
        {
            string key = "AppSettings:" + strKey;
            return AppSettingsManager.GetValue(Config, key).Value;
        }

        public static string GetConnectionStrings(string strKey)
        {
            string key = "ConnectionStrings:" + strKey;
            return AppSettingsManager.GetValue(Config, key).Value;
        }

        #region 数据库连接
        private static string _ReadDBConnectionString;
        private static string _WriteDBConnectionString;
        /// <summary>
        /// 读基础库连接参数
        /// </summary>
        public static string ReadDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_ReadDBConnectionString))
                {
                    _ReadDBConnectionString = GetConnectionStrings("ReadDBConnectionString");
                    return _ReadDBConnectionString;
                }
                else { return _ReadDBConnectionString; }
            }
        }

        /// <summary>
        /// 写基础库连接参数
        /// </summary>
        public static string WriteDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_WriteDBConnectionString))
                {
                    _WriteDBConnectionString = GetConnectionStrings("WriteDBConnectionString");
                    return _WriteDBConnectionString;
                }
                else { return _WriteDBConnectionString; }
            }
        }

        #endregion

        #region redis缓存连接参数
        private static string _RedisHostConnection;
        private static string _RedisKey;
        private static string _RedisDataBaseIndex;
        public static string RedisHostConnection
        {
            get
            {
                if (string.IsNullOrEmpty(_RedisHostConnection))
                {
                    _RedisHostConnection = GetConnectionStrings("RedisHostConnection");
                }
                return _RedisHostConnection;
            }
        }
        public static string RedisKey
        {
            get
            {
                if (string.IsNullOrEmpty(_RedisKey))
                {
                    _RedisKey = GetConnectionStrings("RedisKey");
                }
                return _RedisKey;
            }
        }
        public static string RedisDataBaseIndex
        {
            get
            {
                if (string.IsNullOrEmpty(_RedisDataBaseIndex))
                {
                    _RedisDataBaseIndex = GetConnectionStrings("RedisDataBaseIndex");
                }
                return _RedisDataBaseIndex;
            }
        }
        #endregion

    }
}


