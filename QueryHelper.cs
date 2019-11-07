using Newtonsoft.Json;
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
        /// 获取对象的值字符串内容
        /// </summary>
        /// <param name="obj">值对象</param>
        /// <returns>值字符串内容</returns>
        private static string GetValStr(object obj)
        {
            if (obj == null) return null;
            if (obj is string)
                return obj.ToString();
            try
            {
                return JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 把具体对象转成键值对的查询字符串
        /// </summary>
        /// <param name="modelType">对象类型</param>
        /// <param name="model">具体对象</param>
        /// <returns>键值对的查询字符串</returns>
        public static string ToQueryString(object model,Type modelType)
        {
            StringBuilder queryBuilder = new StringBuilder();
            if (model == null) return queryBuilder.ToString();
            //获取公有字段的值
            FieldInfo[] fields = modelType.GetFields();
            foreach (FieldInfo field in fields)
            {
                object obj = field.GetValue(model);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat("&{0}={1}", field.Name, value);
                else
                    queryBuilder.Append($"{field.Name}={value}");
            }
            //获取公有属性的值
            PropertyInfo[] props = modelType.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object obj = pi.GetValue(model, null);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat("&{0}={1}", pi.Name, value);
                else
                    queryBuilder.Append($"{pi.Name}={value}");
            }
            return queryBuilder.ToString();
        }

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
            //获取公有字段的值
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object obj = field.GetValue(model);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat("&{0}={1}", field.Name, value);
                else
                    queryBuilder.Append($"{field.Name}={value}");
            }
            //获取公有属性的值
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object obj = pi.GetValue(model, null);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                if (queryBuilder.Length > 0)
                    queryBuilder.AppendFormat("&{0}={1}", pi.Name, value);
                else
                    queryBuilder.Append($"{pi.Name}={value}");
            }
            return queryBuilder.ToString();
        }

        /// <summary>
        /// 把具体对象转成键值对字典
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="model">具体对象</param>
        /// <returns>键值对的字典</returns>
        public static Dictionary<string, string> ToDictionary<T>(T model)
        {
            Dictionary<string, string> queryDic = new Dictionary<string, string>();
            if (model == null) return queryDic;
            //获取公有字段的值
            FieldInfo[] fields = typeof(T).GetFields();
            foreach (FieldInfo field in fields)
            {
                object obj = field.GetValue(model);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                queryDic.Add(field.Name, value);
            }
            //获取公有属性的值
            PropertyInfo[] props = typeof(T).GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object obj = pi.GetValue(model, null);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                queryDic.Add(pi.Name, value);
            }
            return queryDic;
        }

        /// <summary>
        /// 把具体对象转成键值对字典
        /// </summary>
        /// <param name="modelType">对象类型</param>
        /// <param name="model">具体对象</param>
        /// <returns>键值对的字典</returns>
        public static Dictionary<string,string> ToDictionary(object model, Type modelType)
        {
            Dictionary<string,string> queryDic = new Dictionary<string, string>();
            if (model == null) return queryDic;
            //获取公有字段的值
            FieldInfo[] fields = modelType.GetFields();
            foreach (FieldInfo field in fields)
            {
                object obj = field.GetValue(model);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                queryDic.Add(field.Name, value);
            }
            //获取公有属性的值
            PropertyInfo[] props = modelType.GetProperties();
            foreach (PropertyInfo pi in props)
            {
                object obj = pi.GetValue(model, null);
                string value = GetValStr(obj);
                if (string.IsNullOrEmpty(value)) continue;
                queryDic.Add(pi.Name, value);
            }
            return queryDic;
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
            string[] paras = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
            T model = new T();
            // 获得此模型的公共字段  
            FieldInfo[] fieldInfos = model.GetType().GetFields();
            foreach (FieldInfo field in fieldInfos)
            {
                // 检查fieldInfos是否包含此列  
                string paraInfo = paras.Where(para => para.StartsWith(field.Name + "=")).FirstOrDefault();
                if (paraInfo != null)
                {
                    string paraValue = string.Empty;
                    if (!paraInfo.EndsWith("="))
                    {
                        int valueStart = paraInfo.IndexOf("=");
                        paraValue = paraInfo.Substring(valueStart + 1);
                    }
                    field.SetValue(model, paraValue);
                }
            }
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

        /// <summary>
        /// 对查询字符串进行参数Key的升序排序
        /// </summary>
        /// <param name="queryString">原需要排序的查询字符串</param>
        /// <returns>排序后的查询字符串</returns>
        public static string QueryStringSort(string queryString)
        {
            if (string.IsNullOrWhiteSpace(queryString)) return queryString;
            if (queryString.EndsWith("?")) return queryString;
            int startIndex = queryString.IndexOf("?");
            if (startIndex != -1)
                queryString = queryString.Substring(startIndex + 1);
            string[] paras = queryString.Split('&', StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string,KeyValuePair<string, string>> paraDic = paras.Where(para => para.IndexOf('=') > 0).Select(para =>
            {
                string[] paraKeyVal = para.Split('=');
                return new KeyValuePair<string,string>(paraKeyVal[0], paraKeyVal[1]);
            }).ToDictionary(keyValPair => keyValPair.Key);
            SortedDictionary<string, KeyValuePair<string, string>> sortDic = new SortedDictionary<string, KeyValuePair<string, string>>(paraDic);
            StringBuilder sortQueryBuilder = new StringBuilder();
            foreach(var keyValuePair in sortDic)
            {
                var para = keyValuePair.Value;
                if (sortQueryBuilder.Length > 0)
                    sortQueryBuilder.AppendFormat("&{0}={1}", para.Key, para.Value);
                else
                    sortQueryBuilder.Append($"{para.Key}={para.Value}");
            }
            return sortQueryBuilder.ToString();
        }
    }
}
