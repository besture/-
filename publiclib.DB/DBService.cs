using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using publiclib.Config;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace cloudpark.DAL
{
    public class DbService
    {

        public static IConfiguration configuration;

        #region 数据库连接
        static string ReadDBConnectionString { get { return AppSettingConfig.ReadDBConnectionString; } }
        static string WriteDBConnectionString { get { return AppSettingConfig.WriteDBConnectionString; } }

        /// <summary>
        /// 基础全局__连接读库
        /// </summary>
        public IDbConnection ReadConnection { get { return new MySqlConnection(ReadDBConnectionString); } }
        /// <summary>
        /// 基础全局__连接写库
        /// </summary>
        public IDbConnection WriteConnection { get { return new MySqlConnection(WriteDBConnectionString); } }

        #endregion

        #region 通用分页数据读取函数
        ///// <summary>
        ///// 通用分页数据读取函数-指定列    
        ///// </summary>
        ///// <param name="T">需要返回的Model</param>  
        ///// <param name="tableName">查询的表/表联合</param>  
        ///// <param name="showFields">字段名</param>  
        ///// <param name="selectWhere">过滤条件</param>
        ///// <param name="orderFields">排序字段,如：CreateTime</param>
        ///// <param name="sortType">排序顺序0降序1升序</param>
        ///// <param name="idKey">主键</param>
        ///// <param name="pageSize">每页数据大小</param>
        ///// <param name="page">当前第几页</param>  
        ///// <param name="pageCount">总页数</param>
        ///// <param name="totalRecord">记录总数</param>
        //public static IEnumerable<T> GetPageList<T>(string tableName, string showFields, string selectWhere, string orderFields, int sortType, string idKey, int pageSize, int page, out int pageCount, out int totalRecord)
        //{
        //    pageCount = 0; totalRecord = 0;
        //    if (string.IsNullOrEmpty(showFields))
        //    {
        //        showFields = " * ";
        //    }

        //    using (var db = new DbService().ReadOrderConnection)
        //    {
        //        string totalCmdText = string.Format(@"select count({0}) from {1} where 1=1 {2}", idKey, tableName, selectWhere);
        //        string strSum = db.ExecuteScalar<string>(totalCmdText);

        //        if (!string.IsNullOrEmpty(strSum))
        //        {
        //            totalRecord = Convert.ToInt32(strSum);
        //            if (totalRecord % pageSize > 0)
        //                pageCount = totalRecord / pageSize + 1;
        //            else
        //                pageCount = totalRecord / pageSize;

        //            string strSort = "desc";
        //            if (sortType == 1)
        //                strSort = "asc";

        //            if (page == 1 || page > pageCount || page < 1)
        //            {
        //                string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5}", showFields, tableName, selectWhere, orderFields, strSort, pageSize);
        //                return db.Query<T>(cmdText);
        //            }
        //            else
        //            {
        //                string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5},{6}", showFields, tableName, selectWhere, orderFields, strSort, ((page - 1) * pageSize), pageSize);
        //                return db.Query<T>(cmdText);
        //            }

        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// 特殊的通用分页数据读取函数-
        /// </summary>
        /// <param name="T">需要返回的Model</param>  
        /// <param name="sqlCount">查询总数的sql语句 eg：select count(*) from a where 1=1</param>
        /// <param name="sqlSelect">查询页内容 eg：select * from a where 1=1 order by a_id desc {0}</param>
        /// <param name="pageSize">每页数据大小</param>
        /// <param name="page">当前第几页</param>  
        /// <param name="pageCount">总页数</param>
        /// <param name="totalRecord">记录总数</param>
        public static IEnumerable<T> GetSpecialPageList<T>(string sqlCount, string sqlSelect, int pageSize, int page, out int pageCount, out int totalRecord)
        {
            pageCount = 0; totalRecord = 0;

            using (var db = new DbService().ReadConnection)
            {
                string strSum = db.ExecuteScalar<string>(sqlCount);

                if (!string.IsNullOrEmpty(strSum))
                {
                    totalRecord = Convert.ToInt32(strSum);
                    if (totalRecord % pageSize > 0)
                        pageCount = totalRecord / pageSize + 1;
                    else
                        pageCount = totalRecord / pageSize;

                    if (page == 1 || page > pageCount || page < 1)
                    {
                        string cmdText = string.Format(sqlSelect, "limit " + pageSize);
                        return db.Query<T>(cmdText);
                    }
                    else
                    {
                        string cmdText = string.Format(sqlSelect, "limit " + ((page - 1) * pageSize) + "," + pageSize);
                        return db.Query<T>(cmdText);
                    }

                }
                return null;
            }
        }

        /// <summary>
        /// 通用分页数据读取函数-指定列    (传数据库DbConnection连接)
        /// </summary>
        /// <param name="T">需要返回的Model</param>  
        /// <param name="tableName">查询的表/表联合</param>  
        /// <param name="showFields">字段名</param>  
        /// <param name="selectWhere">过滤条件</param>
        /// <param name="orderFields">排序字段,如：CreateTime</param>
        /// <param name="sortType">排序顺序0降序1升序</param>
        /// <param name="idKey">主键</param>
        /// <param name="pageSize">每页数据大小</param>
        /// <param name="page">当前第几页</param>  
        /// <param name="pageCount">总页数</param>
        /// <param name="totalRecord">记录总数</param>
        public static IEnumerable<T> GetPageList<T>(IDbConnection dbConn, string tableName, string showFields, string selectWhere, string orderFields, int sortType, string idKey, int pageSize, int page, out int pageCount, out int totalRecord)
        {
            pageCount = 0; totalRecord = 0;
            if (string.IsNullOrEmpty(showFields))
            {
                showFields = " * ";
            }

            using (var db = dbConn)
            {
                string totalCmdText = string.Format(@"select count({0}) from {1} where 1=1 {2}", idKey, tableName, selectWhere);
                string strSum = db.ExecuteScalar<string>(totalCmdText);

                if (!string.IsNullOrEmpty(strSum))
                {
                    totalRecord = Convert.ToInt32(strSum);
                    if (totalRecord % pageSize > 0)
                        pageCount = totalRecord / pageSize + 1;
                    else
                        pageCount = totalRecord / pageSize;

                    string strSort = "desc";
                    if (sortType == 1)
                        strSort = "asc";

                    if (page == 1 || page > pageCount || page < 1)
                    {
                        string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5}", showFields, tableName, selectWhere, orderFields, strSort, pageSize);
                        return db.Query<T>(cmdText);
                    }
                    else
                    {
                        string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5},{6}", showFields, tableName, selectWhere, orderFields, strSort, ((page - 1) * pageSize), pageSize);
                        return db.Query<T>(cmdText);
                    }

                }
                return null;
            }
        }

        /// <summary>
        /// 通用分页数据读取函数-指定列    
        /// </summary>
        /// <param name="T">需要返回的Model</param>  
        /// <param name="tableName">查询的表/表联合</param>  
        /// <param name="showFields">字段名</param>  
        /// <param name="selectWhere">过滤条件</param>
        /// <param name="orderFields">排序字段,如：CreateTime</param>
        /// <param name="sortType">排序顺序0降序1升序</param>
        /// <param name="idKey">主键</param>
        /// <param name="pageSize">每页数据大小</param>
        /// <param name="page">当前第几页</param>  
        /// <param name="pageCount">总页数</param>
        /// <param name="totalRecord">记录总数</param>
        public static IEnumerable<T> GetPageList<T>(IDbConnection dbConn, object param, string tableName, string showFields, string selectWhere, string orderFields, int sortType, string idKey, int pageSize, int page, out int pageCount, out int totalRecord)
        {
            pageCount = 0; totalRecord = 0;
            if (string.IsNullOrEmpty(showFields))
            {
                showFields = " * ";
            }

            using (var db = dbConn)
            {
                string totalCmdText = string.Format(@"select count({0}) from {1} where 1=1 {2}", idKey, tableName, selectWhere);
                string strSum = db.ExecuteScalar<string>(totalCmdText, param);

                if (!string.IsNullOrEmpty(strSum))
                {
                    totalRecord = Convert.ToInt32(strSum);
                    if (totalRecord % pageSize > 0)
                        pageCount = totalRecord / pageSize + 1;
                    else
                        pageCount = totalRecord / pageSize;

                    string strSort = "desc";
                    if (sortType == 1)
                        strSort = "asc";

                    if (page == 1 || page > pageCount || page < 1)
                    {
                        string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5}", showFields, tableName, selectWhere, orderFields, strSort, pageSize);
                        return db.Query<T>(cmdText, param);
                    }
                    else
                    {
                        string cmdText = string.Format(@"select {0} from {1} where 1=1 {2} order by {3} {4} limit {5},{6}", showFields, tableName, selectWhere, orderFields, strSort, ((page - 1) * pageSize), pageSize);
                        return db.Query<T>(cmdText, param);
                    }

                }
                return null;
            }
        }
        #endregion

        #region 根据模型自动生成sql更新语句和参数
        /// <summary>
        /// 根据模型自动生成sql更新语句和参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="commandText">输出sql语句字段</param>
        /// <param name="paramArray">输出sql参数</param>
        public static void GetUpdateCommandText<T>(T t, out string commandText, out MySqlParameter[] paramArray)
        {
            StringBuilder tStr = new StringBuilder();

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            string name;
            object value;

            int conut = 0;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(t, null);
                if (value != null && name != t.GetType().Name + "_ID")
                {
                    conut++;
                    tStr.AppendFormat("{0}=?{1},", name, name);
                }
            }
            commandText = tStr.ToString(0, tStr.Length - 1);

            MySqlParameter[] arrParam = new MySqlParameter[conut];
            var index = 0;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(t, null);
                if (value != null && name.ToLower() != $"{t.GetType().Name}_ID".ToLower())
                {

                    MySqlDbType sqlDbType = ConverDbType.TypeToSqlDbType(item.PropertyType);

                    arrParam[index] = new MySqlParameter("?" + name, sqlDbType);
                    arrParam[index].Value = value;

                    index++;
                }
            }

            paramArray = arrParam;
        }

        /// <summary>
        /// 根据模型自动生成sql更新语句和参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="commandText">输出sql语句字段</param>
        /// <param name="paramArray">输出sql参数</param>
        public static void GetUpdateCommandText<T>(T t, out string commandText, out MySqlParameter[] paramArray, string primarykey = "")
        {
            string tStr = string.Empty;
            if (string.IsNullOrEmpty(primarykey))
            {
                primarykey = $"{t.GetType().Name}_ID";
            }

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            string name;
            object value;

            int conut = 0;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(t, null);
                if (value != null && name != primarykey)
                {
                    conut++;
                    tStr += string.Format("{0}=?{1},", name, name);
                }
            }
            commandText = tStr.Remove(tStr.Length - 1, 1);

            MySqlParameter[] arrParam = new MySqlParameter[conut];
            var index = 0;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(t, null);
                if (value != null && name != primarykey)
                {

                    MySqlDbType sqlDbType = ConverDbType.TypeToSqlDbType(item.PropertyType);

                    arrParam[index] = new MySqlParameter("?" + name, sqlDbType);
                    arrParam[index].Value = value;

                    index++;
                }
            }

            paramArray = arrParam;
        }

        #endregion
        
        /// <summary>
        /// 批量执行SQL
        /// </summary>
        public int ExecuteTrans(List<string> SqlList, out string errmsg, IDbConnection Connection = null)
        {
            errmsg = string.Empty;
            try
            {
                Connection = Connection ?? WriteConnection;

                System.Collections.ArrayList sqlArrayList = new System.Collections.ArrayList();
                List<string> list = new List<string>();
                for (int i = 0; i < SqlList.Count; i++)
                {
                    list.Add(SqlList[i].Trim().TrimEnd(';'));

                    if (list.Count > 50)
                    {
                        sqlArrayList.Add(string.Join(";", list));
                        list.Clear();
                    }
                    else
                    {
                        if (i == SqlList.Count - 1)
                        {
                            sqlArrayList.Add(string.Join(";", list));
                        }
                    }
                }

                int result = 0;
                //using (var db = Connection)
                //using (var db = new MySqlConnection(Connection.ConnectionString))
                using (var db = new MySqlConnection(Connection.ConnectionString))
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var tran = db.BeginTransaction();

                    try
                    {
                        for (int i = 0; i < sqlArrayList.Count; i++)
                        {
                            if (tran.Connection.State == ConnectionState.Closed)
                            {
                                //Console.WriteLine($"DBService/ExecuteTrans事务连接已关闭重新打开");
                                db.Open();
                            }
                            result += db.Execute(sqlArrayList[i].ToString(), null, tran, null, null);
                        }
                        tran.Commit();
                        //db.Close();
                    }
                    catch (Exception btEx)
                    {
                        if (tran.Connection.State != ConnectionState.Closed)
                        {
                            tran.Rollback();
                            tran.Dispose();
                        }
                        throw btEx;
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
                return -1;
            }
        }

        #region 根据模型自动生成sql查询语句和参数
        /// <summary>
        /// 根据模型自动生成sql更新语句和参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="commandText">输出sql语句字段</param>
        /// <param name="paramArray">输出sql参数</param>
        public static string GetSelectCommandText<T>(T t)
        {
            //string tStr = string.Empty;
            StringBuilder tStr = new StringBuilder();

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            //string name;

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                //name = item.Name;
                tStr.AppendFormat("{0},", item.Name);
            }
            //commandText = tStr.Remove(tStr.Length - 1, 1);
            return tStr.ToString(0, tStr.Length - 1);
        }

        #endregion

        #region 自动转换为实体类属性字符串
        /// <summary>
        /// 满足条件的查询字符串（*）自动转换为实体类属性字符串
        /// </summary>
        /// <param name="replaceStr">原字符串*</param>
        /// <param name="beReplaceObj">属性字符串实体类Model.Park</param>
        /// <param name="condition">满足条件</param>
        /// <returns>查询字符串</returns>
        public string ConditionsToReplace(string replaceStr, object beReplaceObj, string condition = "*")
        {
            if (string.IsNullOrEmpty(replaceStr) || replaceStr.Trim() == condition)
            {
                return GetSelectCommandText(beReplaceObj);
            }
            return replaceStr;
        }
        #endregion

        public static string GetValue<T>(T t)
        {
            if (t == null)
            {
                if (typeof(T) == typeof(int?) || typeof(T) == typeof(int))
                {
                    return "'0'";
                }
                else
                {
                    return "null";
                }
            }
            else
            {
                if (typeof(T) == typeof(DateTime?) || typeof(T) == typeof(DateTime))
                {
                    return "'" + Convert.ToDateTime(t).ToString("yyyy-MM-dd HH:mm:ss") + "'";
                }
                else
                {
                    return "'" + Convert.ToString(t) + "'";
                }
            }


        }

        /// <summary>
        /// 获取事务Sql列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reflectionModel">映射实体</param>
        /// <param name="model">数据列表</param>
        /// <param name="sqlCount">单次执行更新数据量</param>
        /// <returns></returns>
        public static List<string> GetTransSql<T>(T reflectionModel, List<T> model, int sqlCount = 100)
        {
            List<string> sqlLst = new List<string>();
            if (model == null) return sqlLst;
            System.Reflection.PropertyInfo[] properties = reflectionModel.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            string tableName = reflectionModel.GetType().Name;
            string name;
            object value;
            string sql = "insert into {0} ({1}) values {2} ON DUPLICATE KEY UPDATE {3}";
            string field1 = string.Empty;
            string field2 = string.Empty;
            string field3 = string.Empty;
            List<System.Reflection.PropertyInfo> items = new List<System.Reflection.PropertyInfo>();
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(reflectionModel, null);

                if (value != null && name.ToLower() != (tableName + "_ID").ToLower())
                {
                    field1 += $"{name},";
                    items.Add(item);
                    field3 += $"{name}=VALUES({name}),";
                }
            }

            if (string.IsNullOrEmpty(field1)) return sqlLst;

            StringBuilder vlSql = new StringBuilder();
            int i = 0;
            foreach (var m in model)
            {
                if (i > 0 && i % sqlCount == 0)
                {
                    string transSql = string.Format(sql, tableName, field1.Trim(','), vlSql.ToString().Trim(','), field3.Trim(','));
                    sqlLst.Add(transSql);
                    vlSql = new StringBuilder();
                }
                string vl = string.Empty;
                vl = "(";
                foreach (var item in items)
                {
                    value = item.GetValue(m, null);
                    if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
                    {
                        DateTime? dt = (DateTime?)value;
                        if (dt == null)
                        {
                            vl += $"NULL,";
                        }
                        else
                        {
                            value = dt.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            vl += $"'{value}',";
                        }
                    }
                    else
                    {
                        vl += $"'{value}',";
                    }
                }
                vlSql.Append(vl.Trim(',') + "),");
                i++;
            }
            if (!string.IsNullOrEmpty(vlSql.ToString()))
            {
                sql = string.Format(sql, tableName, field1.Trim(','), vlSql.ToString().Trim(','), field3.Trim(','));
                sqlLst.Add(sql);
            }

            return sqlLst;
        }

        public static string GetAddSql<T>(T model)
        {
            return GetAddSql<T>(model, null);
        }

        public static string GetAddSql<T>(T model, string tableName)
        {
            if (model == null) return "";
            System.Reflection.PropertyInfo[] properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            if (tableName == null)
                tableName = model.GetType().Name;

            string name;
            object value;
            //string sql = "insert into {0} ({1}) values ({2}) ;SELECT LAST_INSERT_ID();";
            //string field1 = string.Empty;
            //string field2 = string.Empty;

            StringBuilder field1 = new StringBuilder();
            StringBuilder field2 = new StringBuilder();

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(model, null);

                if (value != null && name.ToLower() != (tableName + "_ID").ToLower())
                {
                    if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
                    {
                        DateTime dt = (DateTime)value;
                        value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    //field1 += $"{name},";
                    //field2 += $"'{value}',";

                    field1.AppendFormat("{0},", name);
                    field2.AppendFormat("'{0}',", value);
                }
            }

            if (string.IsNullOrEmpty(field1.ToString())) return "";

            //sql = string.Format(sql, tableName, field1.Trim(','), field2.Trim(','));
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into {0} ({1}) values ({2}) ;SELECT LAST_INSERT_ID();",
                tableName, field1.ToString().Trim(','), field2.ToString().Trim(','));

            return sql.ToString();
        }

        //public static string GetAddSql<T>(T model,string tableName)
        //{
        //    if (model == null) return "";
        //    System.Reflection.PropertyInfo[] properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
        //    string name;
        //    object value;
        //    //string sql = "insert into {0} ({1}) values ({2}) ;SELECT LAST_INSERT_ID();";

        //    StringBuilder field1 = new StringBuilder();
        //    StringBuilder field2 = new StringBuilder();
        //    //string field1 = string.Empty;
        //    //string field2 = string.Empty;
        //    foreach (System.Reflection.PropertyInfo item in properties)
        //    {
        //        name = item.Name;
        //        value = item.GetValue(model, null);

        //        if (value != null && name != tableName + "_ID")
        //        {
        //            if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
        //            {
        //                DateTime dt = (DateTime)value;
        //                value = dt.ToString("yyyy-MM-dd HH:mm:ss");
        //            }

        //            field1.AppendFormat("{0},", name);
        //            field2.AppendFormat("'{0}',", value);

        //            //field1 += $"{name},";
        //            //field2 += $"'{value}',";
        //        }
        //    }

        //    if (string.IsNullOrEmpty(field1.ToString())) return "";

        //    StringBuilder sql = new StringBuilder();
        //    sql.AppendFormat("insert into {0} ({1}) values ({2}) ;SELECT LAST_INSERT_ID();",
        //        tableName, field1.ToString().Trim(','), field2.ToString().Trim(','));

        //    //sql = string.Format(sql, tableName, field1.Trim(','), field2.Trim(','));
        //    return sql.ToString();
        //}

        /// <summary>
        /// 转换实体SQL（支持分表）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="subTableName">指定分表名称</param>
        /// <returns></returns>
        public static string GetAddOrUpdateSql<T>(T model, string asTableName = null)
        {
            if (model == null) return "";
            System.Reflection.PropertyInfo[] properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            string tableName = model.GetType().Name;
            string name;
            object value;

            //string sql = "insert into {0} ({1}) values ({2}) ON DUPLICATE KEY UPDATE {3};SELECT LAST_INSERT_ID();";

            StringBuilder field1 = new StringBuilder();
            StringBuilder field2 = new StringBuilder();
            StringBuilder field3 = new StringBuilder();

            //string field1 = string.Empty;
            //string field2 = string.Empty;
            //string field3 = string.Empty;

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(model, null);

                if (value != null && name.ToLower() != $"{tableName}_ID".ToLower())
                {
                    if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
                    {
                        DateTime dt = (DateTime)value;
                        value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    field1.AppendFormat("{0},", name);
                    field2.AppendFormat("'{0}',", value);
                    field3.AppendFormat("{0}=VALUES({1}),", name, name);

                    //field1 += $"{name},";
                    //field2 += $"'{value}',";
                    //field3 += $"{name}=VALUES({name}),";
                }
            }

            if (string.IsNullOrEmpty(field1.ToString())) return "";

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("insert into {0} ({1}) values ({2}) ON DUPLICATE KEY UPDATE {3};SELECT LAST_INSERT_ID();",
                asTableName ?? tableName, field1.ToString().Trim(','), field2.ToString().Trim(','), field3.ToString().Trim(','));

            //sql = string.Format(sql, asTableName ?? tableName, field1.Trim(','), field2.Trim(','), field3.Trim(','));

            return sql.ToString();
        }

        /// <summary>
        /// 转换实体SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model">实体</param>
        /// <param name="convertNullField">是否处理Null的字段：true-处理，false-不处理</param>
        /// <param name="subTableName">指定分表名称</param>
        /// <returns></returns>
        public static string GetAddOrUpdateSql<T>(T model, bool convertNullField)
        {
            if (model == null) return "";
            System.Reflection.PropertyInfo[] properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            string tableName = model.GetType().Name;
            string name;
            object value;
            string sql = "insert into {0} ({1}) values ({2}) ON DUPLICATE KEY UPDATE {3};SELECT LAST_INSERT_ID();";
            string field1 = string.Empty;
            string field2 = string.Empty;
            string field3 = string.Empty;
            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(model, null);

                if (value != null)
                {
                    if (name.ToLower() != (tableName + "_ID").ToLower())
                    {
                        if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
                        {
                            DateTime dt = (DateTime)value;
                            value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                        }

                        field1 += $"{name},";
                        field2 += $"'{value}',";
                        field3 += $"{name}=VALUES({name}),";
                    }
                }
                else
                {
                    if (convertNullField)
                    {
                        if (name.ToLower() != (tableName + "_ID").ToLower())
                        {
                            value = "null";
                            field1 += $"{name},";
                            field2 += $"{value},";
                            field3 += $"{name}=VALUES({name}),";
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(field1)) return "";

            sql = string.Format(sql, tableName, field1.Trim(','), field2.Trim(','), field3.Trim(','));

            return sql;
        }

        /// <summary>
        /// 分表用
        /// </summary>
        public static string GetAddOrUpdateSql<T>(T model, string table, bool isOnlyUpdate = false, string updatewhere = "")
        {
            if (model == null) return "";
            System.Reflection.PropertyInfo[] properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            string tableName = model.GetType().Name;
            string name;
            string id = string.Empty;
            object value;

            string sql = "insert into {0} ({1}) values ({2}) ON DUPLICATE KEY UPDATE {3};SELECT LAST_INSERT_ID();";

            StringBuilder field1 = new StringBuilder();
            StringBuilder field2 = new StringBuilder();
            StringBuilder field3 = new StringBuilder();
            StringBuilder field4 = new StringBuilder();

            foreach (System.Reflection.PropertyInfo item in properties)
            {
                name = item.Name;
                value = item.GetValue(model, null);
                if (name.ToLower() == (tableName + "_ID").ToLower() && isOnlyUpdate)
                {
                    id = value?.ToString();
                }

                if (value != null && name.ToLower() != (tableName + "_ID").ToLower())
                {
                    if (item.PropertyType.FullName.Contains(typeof(System.DateTime).FullName))
                    {
                        DateTime dt = (DateTime)value;
                        value = dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    field1.AppendFormat("{0},", name);
                    field2.AppendFormat("'{0}',", value);
                    field3.AppendFormat("{0}=VALUES({1}),", name, name);
                    field4.AppendFormat("{0}='{1}',", name, value);
                }
            }

            if (string.IsNullOrEmpty(field1.ToString())) return "";

            if (!isOnlyUpdate)
            {
                sql = string.Format(sql, table, field1.ToString().Trim(','), field2.ToString().Trim(','), field3.ToString().Trim(','));
            }
            else if (!string.IsNullOrWhiteSpace(updatewhere))
            {
                sql = $"UPDATE {table} SET {field4.ToString().Trim(',')} WHERE {updatewhere}";
            }
            else
            {
                sql = $"UPDATE {table} SET {field4.ToString().Trim(',')} WHERE {tableName}_ID ='{id}'";
            }

            return sql;
        }
    }

    public class ConverDbType
    {
        // C#数据类型转换为DbType
        public static DbType TypeToDbType(Type t)
        {
            DbType dbt;
            try
            {
                dbt = (DbType)Enum.Parse(typeof(DbType), t.Name);
            }
            catch
            {
                dbt = DbType.Object;
            }
            return dbt;
        }

        // C#数据类型转换为SqlDbType
        public static MySqlDbType TypeToSqlDbType(Type t)
        {

            //转换可空类型，因为可空类型 的TypeCode是 TypeCode.Object，可空类型包含 int? ,Decimal?,DateTime? 等

            //判断convertsionType类型是否为泛型，因为nullable是泛型类,
            if (t.IsGenericType &&
                //判断convertsionType是否为nullable泛型类
                t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {

                //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                NullableConverter nullableConverter = new NullableConverter(t);
                //将convertsionType转换为nullable对的基础基元类型
                t = nullableConverter.UnderlyingType;
            }

            TypeCode tc = Type.GetTypeCode(t);

            switch (tc)
            {
                case TypeCode.Boolean:
                    return MySqlDbType.Bit;
                case TypeCode.Byte:
                    return MySqlDbType.Byte;
                case TypeCode.DateTime:
                    return MySqlDbType.DateTime;
                case TypeCode.Decimal:
                    return MySqlDbType.Decimal;
                case TypeCode.Double:
                    return MySqlDbType.Float;
                case TypeCode.Int16:
                    return MySqlDbType.Int16;
                case TypeCode.Int32:
                    return MySqlDbType.Int32;
                case TypeCode.Int64:
                    return MySqlDbType.Int64;
                case TypeCode.SByte:
                    return MySqlDbType.UByte;
                case TypeCode.Single:
                    return MySqlDbType.Float;
                case TypeCode.String:
                    return MySqlDbType.VarChar;
                case TypeCode.UInt16:
                    return MySqlDbType.UInt16;
                case TypeCode.UInt32:
                    return MySqlDbType.UInt32;
                case TypeCode.UInt64:
                    return MySqlDbType.UInt64;
                //case TypeCode.Object:
                //    return MySqlDbType.JSON;

                default:
                    if (t == typeof(byte[]))
                    {
                        return MySqlDbType.Binary;
                    }
                    return MySqlDbType.Guid;
            }

        }

    }


}
