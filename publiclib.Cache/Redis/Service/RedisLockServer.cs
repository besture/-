namespace publiclib.Cache.Redis.Service
{
    public class RedisLockServer : RedisHelper
    {
        #region 构造函数

        /// <summary>
        /// 初始化Redis的SortedSet有序数据结构操作
        /// </summary>
        /// <param name="dbNum">操作的数据库索引0-64(需要在conf文件中配置)</param>
        public RedisLockServer(int? dbNum = null) : base(dbNum) { }

        public RedisLockServer(int? dbNum, string redisHostAddress) : base(dbNum, redisHostAddress) { }

        #endregion

    }
}
