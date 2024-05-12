using System;
using System.IO;
using UnityEngine;

namespace QZGameFramework.DebuggerSystem
{
    public class LogConfig
    {
        /// <summary>
        /// 是否开启日志系统
        /// </summary>
        public bool openLog = true;

        /// <summary>
        /// 日志前缀
        /// </summary>
        public string logHeadFix = "###";

        /// <summary>
        /// 是否显示时间
        /// </summary>
        public bool openTime = true;

        /// <summary>
        /// 显示线程ID
        /// </summary>
        public bool showThreadID = true;

        /// <summary>
        /// 日志文件存储开关
        /// </summary>
        public bool logSave = true;

        /// <summary>
        /// 是否显示颜色名字
        /// </summary>
        public bool showColorName = true;

        /// <summary>
        /// 是否显示FPS
        /// </summary>
        public bool showFPS = true;

        /// <summary>
        /// 是否启用 Unity 自带的日志打印系统
        /// </summary>
        public bool unityLoggerEnabled = true;

        /// <summary>
        /// 日志文件存储路径
        /// </summary>
        public string logFileSavePath => Path.Combine(Application.persistentDataPath, "Log");

        /// <summary>
        /// 日志文件名称
        /// </summary>
        public string logFileName => Application.productName + " " + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + ".log";
    }
}