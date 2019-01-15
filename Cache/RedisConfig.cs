using System;
using System.Collections.Generic;
using System.Text;

namespace NugetLibs.HelpTool.Cache
{
    /// <summary>
    /// Redis的配置信息
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 配置系统RedisKey的前缀（使用:来分割前缀，如Demo:test: 可视化工具查看的时候就比较好按目录区分）
        /// </summary>
        public string SysCustomPrefix { get; set; } = "DefaultPrefix:";
        /// <summary>
        /// Redis的连接字符串
        /// </summary>
        public string RedisConnect { get; set; } = "localhost";
    }
}
