using Newtonsoft.Json;
using publiclib.Cache.Redis;
using publiclib.Cache.Redis.Service;
using StackExchange.Redis;

namespace publiclib.Cache
{
    /// <summary>
    /// Redis操作
    /// </summary>
    public class RedisHelper : RedisConnectionStrings
    {
        #region 属性字段
        /// <summary>
        /// 网站Redis 系统自定义Key前缀
        /// </summary>
        protected string CustomKey = RedisSysCustomKey;
        /// <summary>
        /// 网站Redis 链接字符串
        /// </summary>
        protected readonly ConnectionMultiplexer _conn;
        /// <summary>
        /// Redis操作对象
        /// </summary>
        protected readonly IDatabase redis = null;
        #endregion

        #region 构造函数
        /// <summary>
        /// 初始化Redis操作方法基础类
        /// </summary>
        /// <param name="dbNum">操作的数据库索引0-64(需要在conf文件中配置)</param>
        protected RedisHelper(int? dbNum = null) : this(dbNum, null) { }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="dbNum">dbindex</param>
        /// <param name="redisHostAddress">可选，为空时返回默认库</param>
        protected RedisHelper(int? dbNum, string redisHostAddress)
        {
            _conn = string.IsNullOrEmpty(redisHostAddress) ?
                RedisManager.GetConnectionMultiplexer(DynamicCacheConnection) :
                RedisManager.GetConnectionMultiplexer(redisHostAddress);
            if (_conn != null)
            {
                redis = _conn.GetDatabase(dbNum ?? RedisDataBaseIndex);   //如果要使用异步， 要为Task.AsyncState 指定一个值
            }
            else
            {
                throw new ArgumentNullException("Redis连接初始化失败");
            }
        }


        #endregion 构造函数

        #region 外部调用静态方法

        /// <summary>
        /// 获取Redis的String数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisStringService StringService(int? dbNum = null) { return new RedisStringService(dbNum); }
        public static RedisStringService StringService(int? dbNum, string redisHostAddress) { return new RedisStringService(dbNum, redisHostAddress); }

        /// <summary>
        /// 获取Redis的String数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisStringService Service(string customkey = null, int? dbNum = null, string redisHostAddress = null) { return new RedisStringService(customkey, dbNum, redisHostAddress); }

        /// <summary>
        /// 获取Redis的Hash数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisHashService HashService(int? dbNum = null) { return new RedisHashService(dbNum); }
        public static RedisHashService HashService(int? dbNum, string redisHostAddress) { return new RedisHashService(dbNum, redisHostAddress); }

        /// <summary>
        /// 获取Redis的List数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisListService ListService(int? dbNum = null) { return new RedisListService(dbNum); }
        public static RedisListService ListService(int? dbNum, string redisHostAddress) { return new RedisListService(dbNum, redisHostAddress); }

        /// <summary>
        /// 获取Redis的Set无序集合数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisSetService SetService(int? dbNum = null) { return new RedisSetService(dbNum); }
        public static RedisSetService SetService(int? dbNum, string redisHostAddress) { return new RedisSetService(dbNum, redisHostAddress); }

        /// <summary>
        /// 获取Redis的SortedSet(ZSet)有序集合数据类型操作辅助方法类
        /// </summary>
        /// <returns></returns>
        public static RedisSortedSetService SortedSetService(int? dbNum = null) { return new RedisSortedSetService(dbNum); }
        public static RedisSortedSetService SortedSetService(int? dbNum, string redisHostAddress) { return new RedisSortedSetService(dbNum, redisHostAddress); }


        /// <summary>
        /// redis锁
        /// </summary>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public static RedisLockServer LockServer(int? dbNum = null) { return new RedisLockServer(dbNum); }
        public static RedisLockServer LockServer(int? dbNum, string redisHostAddress) { return new RedisLockServer(dbNum, redisHostAddress); }

        /// <summary>
        /// 发布与订阅
        /// </summary>
        /// <param name="dbNum"></param>
        /// <returns></returns>
        public static ISubscriber RedisPubSubService(int? dbNum = null) { return new RedisPubSubService(dbNum).GetSubscriber(); }
        public static ISubscriber RedisPubSubService(int? dbNum, string redisHostAddress) { return new RedisPubSubService(dbNum, redisHostAddress).GetSubscriber(); }

        #endregion

        #region 公共操作方法

        #region 不建议公开这些方法，如果项目中用不到，建议注释或者删除

        /// <summary>
        /// 获取Redis事务对象
        /// </summary>
        /// <returns></returns>
        public ITransaction CreateTransaction()
        {
            return redis.CreateTransaction();
        }

        /// <summary>
        /// 获取Redis服务和常用操作对象
        /// </summary>
        /// <returns></returns>
        public IDatabase GetDatabase() { return redis; }

        /// <summary>
        /// 获取Redis服务
        /// </summary>
        /// <param name="hostAndPort"></param>
        /// <returns></returns>
        public IServer GetServer(string hostAndPort)
        {
            return _conn.GetServer(hostAndPort);
        }

        /// <summary>
        /// 执行Redis事务
        /// </summary>
        /// <param name="act"></param>
        /// <returns></returns>
        public bool RedisTransaction(Action<ITransaction> act)
        {
            var tran = redis.CreateTransaction();
            act.Invoke(tran);
            bool committed = tran.Execute();
            return committed;
        }
        /// <summary>
        /// Redis锁
        /// </summary>
        /// <param name="act"></param>
        /// <param name="ts">锁住时间</param>
        public void RedisLockTake(Action act, TimeSpan ts)
        {
            RedisValue token = Environment.MachineName;
            string lockKey = "lock_LockTake";
            if (redis.LockTake(lockKey, token, ts))
            {
                try
                {
                    act();
                }
                finally
                {
                    redis.LockRelease(lockKey, token);
                }
            }
        }

        /// <summary>
        /// Redis锁
        /// </summary>
        /// <param name="key">该锁的名称</param>
        /// <param name="token">标识谁拥有该锁并用来释放锁</param>
        /// <param name="ts">该锁的有效时间</param>
        public bool RedisLockTake(string key, string token, TimeSpan ts)
        {
            key = AddSysCustomKey(key);
            return redis.LockTake(key, token, ts);
        }

        /// <summary>
        /// Redis释放锁
        /// </summary>
        /// <param name="key">redis数据库中该锁的名称</param>
        /// <param name="token">标识谁拥有该锁并用来释放锁</param>
        /// <returns></returns>
        public bool RedisLockRelease(string key, string token)
        {
            return redis.LockRelease(key, token);
        }
        #endregion 其他

        #region 常用Key操作

        /// <summary>
        /// 设置前缀
        /// </summary>
        /// <param name="customKey"></param>
        public void SetSysCustomKey(string customKey)
        {
            CustomKey = customKey;
        }

        /// <summary>
        /// 组合缓存Key名称
        /// </summary>
        /// <param name="oldKey"></param>
        /// <returns></returns>
        public string AddSysCustomKey(string oldKey)
        {
            return CustomKey + ":" + oldKey;
        }

        #region 同步方法

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">要删除的key</param>
        /// <returns>是否删除成功</returns>
        public bool KeyDelete(string key)
        {
            key = AddSysCustomKey(key);
            var result = redis.KeyDelete(key);
            if (!result)
            {
                result = redis.KeyDelete(key);  //删除失败则再尝试一次
            }
            return result;
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">要删除的key集合</param>
        /// <returns>成功删除的个数</returns>
        public long KeyDelete(params string[] keys)
        {
            RedisKey[] newKeys = keys.Select(o => (RedisKey)AddSysCustomKey(o)).ToArray();
            var result = redis.KeyDelete(newKeys);
            if (result != keys.Count())
            {
                //Log.Warning($"删除Redis异常：keys-{keys}，result-{result}");
            }
            return result;
        }

        /// <summary>
        /// 清空当前DataBase中所有Key
        /// </summary>
        public void KeyFulsh()
        {
            //直接执行清除命令
            redis.Execute("FLUSHDB");
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">要判断的key</param>
        /// <returns></returns>
        public bool KeyExists(string key)
        {
            key = AddSysCustomKey(key);
            return redis.KeyExists(key);
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public bool KeyRename(string key, string newKey)
        {
            key = AddSysCustomKey(key);
            newKey = AddSysCustomKey(newKey);
            return redis.KeyRename(key, newKey);
        }

        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public bool KeyExpire(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return redis.KeyExpire(key, expiry);
        }

        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashExists(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return redis.HashExists(key, dataKey);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool HashSet<T>(string key, string dataKey, T t)
        {
            key = AddSysCustomKey(key);

            string json = ConvertJson(t);
            return redis.HashSet(key, dataKey, json);
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public void HashSet(string key, Dictionary<string, int> datas)
        {
            key = AddSysCustomKey(key);
            var fields = datas.Select(
            pair => new HashEntry(pair.Key, pair.Value)).ToArray();
            redis.HashSet(key, fields);
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string dataKey)
        {
            key = AddSysCustomKey(key);
            return redis.HashDelete(key, dataKey);
        }

        /// <summary>
        /// 移除hash中的多个值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKeys"></param>
        /// <returns></returns>
        public long HashDelete(string key, List<RedisValue> dataKeys)
        {
            key = AddSysCustomKey(key);
            return redis.HashDelete(key, dataKeys.ToArray());
        }

        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public T HashGet<T>(string key, string dataKey)
        {
            key = AddSysCustomKey(key);

            string value = redis.HashGet(key, dataKey);
            return ConvertObj<T>(value);
        }

        /// <summary>
        /// 为数字增长val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>增长后的值</returns>
        public double HashIncrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return redis.HashIncrement(key, dataKey, val);
        }

        /// <summary>
        /// 为数字减少val
        /// </summary>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <param name="val">可以为负</param>
        /// <returns>减少后的值</returns>
        public double HashDecrement(string key, string dataKey, double val = 1)
        {
            key = AddSysCustomKey(key);
            return redis.HashDecrement(key, dataKey, val);
        }

        /// <summary>
        /// 获取hashkey所有Redis key
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> HashKeys<T>(string key)
        {
            key = AddSysCustomKey(key);

            RedisValue[] values = redis.HashKeys(key);
            return ConvetList<T>(values);
        }

        /// <summary>
        /// 获取hashkey所有key与值，必须保证Key内的所有数据类型一致
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            key = AddSysCustomKey(key);
            var query = redis.HashGetAll(key);
            Dictionary<string, T> dic = new Dictionary<string, T>();
            foreach (var item in query)
            {
                dic.Add(item.Name, ConvertObj<T>(item.Value));
            }
            return dic;
        }

        #endregion

        #region 异步方法

        /// <summary>
        /// 删除单个key
        /// </summary>
        /// <param name="key">要删除的key</param>
        /// <returns>是否删除成功</returns>
        public async Task<bool> KeyDeleteAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await redis.KeyDeleteAsync(key);
        }

        /// <summary>
        /// 删除多个key
        /// </summary>
        /// <param name="keys">要删除的key集合</param>
        /// <returns>成功删除的个数</returns>
        public async Task<long> KeyDeleteAsync(params string[] keys)
        {
            RedisKey[] newKeys = keys.Select(o => (RedisKey)AddSysCustomKey(o)).ToArray();
            return await redis.KeyDeleteAsync(newKeys);
        }

        /// <summary>
        /// 清空当前DataBase中所有Key
        /// </summary>
        public async Task KeyFulshAsync()
        {
            //直接执行清除命令
            await redis.ExecuteAsync("FLUSHDB");
        }

        /// <summary>
        /// 判断key是否存在
        /// </summary>
        /// <param name="key">要判断的key</param>
        /// <returns></returns>
        public async Task<bool> KeyExistsAsync(string key)
        {
            key = AddSysCustomKey(key);
            return await redis.KeyExistsAsync(key);
        }

        /// <summary>
        /// 重新命名key
        /// </summary>
        /// <param name="key">就的redis key</param>
        /// <param name="newKey">新的redis key</param>
        /// <returns></returns>
        public async Task<bool> KeyRenameAsync(string key, string newKey)
        {
            key = AddSysCustomKey(key);
            newKey = AddSysCustomKey(newKey);
            return await redis.KeyRenameAsync(key, newKey);
        }

        /// <summary>
        /// 设置Key的过期时间
        /// </summary>
        /// <param name="key">redis key</param>
        /// <param name="expiry">过期时间</param>
        /// <returns></returns>
        public async Task<bool> KeyExpireAsync(string key, TimeSpan? expiry = default(TimeSpan?))
        {
            key = AddSysCustomKey(key);
            return await redis.KeyExpireAsync(key, expiry);
        }
        #endregion

        #endregion 

        #endregion

        #region 辅助方法
        /// <summary>
        /// 将对象转换成string字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string ConvertJson<T>(T value)
        {
            string result = value is string ? value.ToString() :
                JsonConvert.SerializeObject(value, Formatting.None);
            return result;
        }
        /// <summary>
        /// 将值反系列化成对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        protected T ConvertObj<T>(RedisValue value)
        {
            try
            {
                if (typeof(T).Name == "String" || typeof(T).Name == "DateTime")
                    value = JsonConvert.SerializeObject(value);

                return value.IsNullOrEmpty ? default(T) : JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// 将值反系列化成对象集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        protected List<T> ConvetList<T>(RedisValue[] values)
        {
            List<T> result = new List<T>();
            foreach (var item in values)
            {
                var model = ConvertObj<T>(item);
                result.Add(model);
            }
            return result;
        }
        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        protected RedisKey[] ConvertRedisKeys(List<string> redisKeys) { return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray(); }

        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        protected RedisKey[] ConvertRedisKeys(params string[] redisKeys) { return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray(); }

        /// <summary>
        /// 将string类型的Key转换成 <see cref="RedisKey"/> 型的Key，并添加前缀字符串
        /// </summary>
        /// <param name="redisKeys"></param>
        /// <returns></returns>
        protected RedisKey[] ConvertRedisKeysAddSysCustomKey(params string[] redisKeys) { return redisKeys.Select(redisKey => (RedisKey)AddSysCustomKey(redisKey)).ToArray(); }
        /// <summary>
        /// 将值集合转换成RedisValue集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisValues"></param>
        /// <returns></returns>
        protected RedisValue[] ConvertRedisValue<T>(params T[] redisValues) { return redisValues.Select(o => (RedisValue)ConvertJson<T>(o)).ToArray(); }
        #endregion 辅助方法
    }
}
