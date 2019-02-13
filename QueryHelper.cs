using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NugetLibs.HelpTool
{
    /// <summary>
    /// 把具体对象与查询字符串的相互转换帮助类
    /// </summary>
    public static class QueryHelper
    {
        /// <summary>
        /// 把具体对象转成键值对的查询字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="model">具体对象</param>
        /// <returns>键值对的查询字符串</returns>
        public static string ToQueryString<T>(T model)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (model == null) return queryBuilder.ToString();
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object obj = pi.GetValue(model, null);
                if (obj == null || string.IsNullOrEmpty(obj.ToString())) continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat("&{0}={1}", pi.Name, obj.ToString());
                else
                    queryBuilder.Append($"{pi.Name}={obj.ToString()}");
            }
            return queryBuilder.ToString();
        }

        /// <summary>
        /// 把键值对的查询字符串转成具体对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="queryString">查询字符串</param>
        /// <returns>具体对象</returns>
        public static T ConvertToModel<T>(string queryString) where T : new()
        {
            if (string.IsNullOrWhiteSpace(queryString)) return default(T);
            if (queryString.EndsWith("?")) return default(T);
            int startIndex = queryString.IndexOf("?");
            if (startIndex != -1)
                queryString = queryString.Substring(startIndex + 1);
            string[] paras = queryString.Split('&');
            T model = new T();
            // 获得此模型的公共属性   
            PropertyInfo[] propertys = model.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                // 检查paras是否包含此列  
                string paraInfo = paras.Where(para => para.StartsWith(pi.Name + "=")).FirstOrDefault();
                if (paraInfo != null)
                {
                    string paraValue = string.Empty;
                    if (!paraInfo.EndsWith("="))
                    {
                        int valueStart = paraInfo.IndexOf("=");
                        paraValue = paraInfo.Substring(valueStart + 1);
                    }
                    pi.SetValue(model, paraValue, null);
                }
            }
            return model;
        }
    }
}
