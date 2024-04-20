using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using UnityEngine;

namespace QZGameFramework.DebuggerSystem
{
    public class Debugger
    {
        public static LogConfig logConfig;

        [Conditional("OPEN_LOG")]
        public static void InitDebuggerSystem(LogConfig cfg = null)
        {
            if (logConfig == null)
            {
                logConfig = new LogConfig();
            }
            else
            {
                logConfig = cfg;
            }

            if (logConfig.logSave)
            {
                GameObject logObj = new GameObject("UnityLog Helper");
                GameObject.DontDestroyOnLoad(logObj);
                UnityLogHelper unityLogHelper = logObj.AddComponent<UnityLogHelper>();
                unityLogHelper.InitLogFileModule(logConfig.logFileSavePath, logConfig.logFileName);
            }

            if (logConfig.showFPS)
            {
                GameObject fpsObj = new GameObject("FPS");
                GameObject.DontDestroyOnLoad(fpsObj);
                fpsObj.AddComponent<FPS>();
            }
        }

        #region 普通日志

        [Conditional("OPEN_LOG")]
        public static void Log(object obj)
        {
            if (!logConfig.openLog)
            {
                return;
            }

            string log = GenerateLog(obj.ToString());
            UnityEngine.Debug.Log(log);
        }

        [Conditional("OPEN_LOG")]
        public static void Log(object obj, params object[] args)
        {
            if (!logConfig.openLog)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog(obj.ToString() + content);
            UnityEngine.Debug.Log(log);
        }

        [Conditional("OPEN_LOG")]
        public static void LogWarning(object obj)
        {
            if (!logConfig.openLog)
            {
                return;
            }

            string log = GenerateLog(obj.ToString());
            UnityEngine.Debug.LogWarning(log);
        }

        [Conditional("OPEN_LOG")]
        public static void LogWarning(object obj, params object[] args)
        {
            if (!logConfig.openLog)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog(obj.ToString() + content);
            UnityEngine.Debug.LogWarning(log);
        }

        [Conditional("OPEN_LOG")]
        public static void LogError(object obj)
        {
            if (!logConfig.openLog)
            {
                return;
            }

            string log = GenerateLog(obj.ToString());
            UnityEngine.Debug.LogError(log);
        }

        public static void LogError(object obj, params object[] args)
        {
            if (!logConfig.openLog)
            {
                return;
            }
            string content = string.Empty;
            if (args != null)
            {
                foreach (var item in args)
                {
                    content += item;
                }
            }
            string log = GenerateLog(obj.ToString() + content);
            UnityEngine.Debug.LogError(log);
        }

        #endregion

        #region 颜色日志打印

        [Conditional("OPEN_LOG")]
        public static void ColorLog(object obj, LogColor color = LogColor.None)
        {
            if (!logConfig.openLog)
            {
                return;
            }

            string log = GenerateLog(obj.ToString(), color);
            log = GetUnityColor(log, color);
            UnityEngine.Debug.Log(log);
        }

        [Conditional("OPEN_LOG")]
        public static void LogGreen(object obj)
        {
            ColorLog(obj, LogColor.Green);
        }

        [Conditional("OPEN_LOG")]
        public static void LogRed(object obj)
        {
            ColorLog(obj, LogColor.Red);
        }

        [Conditional("OPEN_LOG")]
        public static void LogYellow(object obj)
        {
            ColorLog(obj, LogColor.Yellow);
        }

        [Conditional("OPEN_LOG")]
        public static void LogOrange(object obj)
        {
            ColorLog(obj, LogColor.Orange);
        }

        [Conditional("OPEN_LOG")]
        public static void LogBlue(object obj)
        {
            ColorLog(obj, LogColor.Blue);
        }

        [Conditional("OPEN_LOG")]
        public static void LogMagenta(object obj)
        {
            ColorLog(obj, LogColor.Magenta);
        }

        [Conditional("OPEN_LOG")]
        public static void LogDarkBlue(object obj)
        {
            ColorLog(obj, LogColor.DarkBlue);
        }

        [Conditional("OPEN_LOG")]
        public static void LogCyan(object obj)
        {
            ColorLog(obj, LogColor.Cyan);
        }

        #endregion

        private static string GenerateLog(string log, LogColor color = LogColor.None)
        {
            StringBuilder sb = new StringBuilder(logConfig.logHeadFix, 100);
            if (logConfig.openTime)
            {
                sb.AppendFormat(" [LogTime: {0}]", DateTime.Now.ToString("HH:mm:ss-fff"));
            }
            if (logConfig.showThreadID)
            {
                sb.AppendFormat(" [ThreadID: {0}]", Thread.CurrentThread.ManagedThreadId);
            }
            if (logConfig.showColorName)
            {
                sb.AppendFormat(" [LogColor: {0}]", color.ToString());
            }
            sb.AppendFormat(" [LogMessage: {0}]", log);
            return sb.ToString();
        }

        private static string GetUnityColor(string log, LogColor color)
        {
            switch (color)
            {
                case LogColor.Blue:
                    log = $"<color=#0000FF>{log}</color>";
                    break;

                case LogColor.Cyan:
                    log = $"<color=#00FFFF>{log}</color>";
                    break;

                case LogColor.DarkBlue:
                    log = $"<color=#8FBC8F>{log}</color>";
                    break;

                case LogColor.Green:
                    log = $"<color=#00FF00>{log}</color>";
                    break;

                case LogColor.Orange:
                    log = $"<color=#FFA500>{log}</color>";
                    break;

                case LogColor.Red:
                    log = $"<color=#FF0000>{log}</color>";
                    break;

                case LogColor.Yellow:
                    log = $"<color=#FFFF00>{log}</color>";
                    break;

                case LogColor.Magenta:
                    log = $"<color=#FF00FF>{log}</color>";
                    break;

                case LogColor.Purple:
                case LogColor.None:
                case LogColor.Grey:
                default:
                    break;
            }
            return log;
        }
    }
}