using StackExchange.Redis;

namespace publiclib.Cache.Redis.Service
{
    public class RedisPubSubService : RedisHelper
    {

        #region 构造函数

        /// <summary>
        /// 初始化Redis的String数据结构操作
        /// </summary>
        /// <param name="dbNum">操作的数据库索引0-64(需要在conf文件中配置)</param>
        public RedisPubSubService(int? dbNum = null) : base(dbNum) { }

        public RedisPubSubService(int? dbNum, string redisHostAddress) : base(dbNum, redisHostAddress) { }
        #endregion

        #region 同步方法

        //订阅与发布
        public ISubscriber GetSubscriber()
        {
            return _conn.GetSubscriber();
        }

        #endregion
    }
}
