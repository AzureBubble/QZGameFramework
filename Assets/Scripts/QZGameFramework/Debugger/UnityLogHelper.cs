using Cysharp.Threading.Tasks;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using UnityEngine;

namespace QZGameFramework.DebuggerSystem
{
    public class LogData
    {
        /// <summary>
        /// 日志内容
        /// </summary>
        public string log;

        /// <summary>
        /// 堆栈
        /// </summary>
        public string trace;

        /// <summary>
        /// 日志类型
        /// </summary>
        public LogType type;
    }

    public class UnityLogHelper : MonoBehaviour
    {
        /// <summary>
        /// 文件写入流
        /// </summary>
        private StreamWriter streamWriter;

        private string logFileSavePath;

        /// <summary>
        /// 日志消息队列 子线程安全队列
        /// </summary>
        private readonly ConcurrentQueue<LogData> logDatas = new ConcurrentQueue<LogData>();

        /// <summary>
        /// 工作信号事件
        /// </summary>
        private readonly ManualResetEvent manualResetEvent = new ManualResetEvent(false);

        private string nowTime => DateTime.Now.ToString("yyyy:MM:dd HH:mm:ss");
        private bool threadRunning;

        public void InitLogFileModule(string savePath, string logFileName)
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;

            logFileSavePath = Path.Combine(savePath, logFileName);
            Debug.Log("UnityLogFile SavePath: " + logFileSavePath);
            streamWriter = new StreamWriter(logFileSavePath);
            threadRunning = true;
            // C# 线程
            //Thread fileThread = new Thread(FileLogThread);
            //fileThread.Start();
            // UniTask 异步线程
            LogTask().Forget();
        }

        private void OnEnable()
        {
            Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            if (streamWriter == null && logFileSavePath != null)
            {
                streamWriter = new StreamWriter(logFileSavePath);
            }
            threadRunning = true;
        }

        private async UniTaskVoid LogTask()
        {
            await UniTask.RunOnThreadPool(async () =>
            {
                while (threadRunning)
                {
                    if (streamWriter == null) break;
                    while (logDatas.Count > 0 && logDatas.TryDequeue(out LogData data))
                    {
                        switch (data.type)
                        {
                            case LogType.Log:
                                streamWriter?.Write("Log >>> ");
                                streamWriter?.WriteLine(data.log);
                                streamWriter?.WriteLine(data.trace);
                                break;

                            case LogType.Warning:
                                streamWriter?.Write("Warning >>> ");
                                streamWriter?.WriteLine(data.log);
                                streamWriter?.WriteLine(data.trace);
                                break;

                            case LogType.Error:
                                streamWriter?.Write("Error >>> ");
                                streamWriter?.WriteLine(data.log);
                                streamWriter?.Write('\n');
                                streamWriter?.WriteLine(data.trace);
                                break;
                        }
                        streamWriter?.Write("\r\n");
                    }
                    // 保存当前文件内容 使其生效
                    streamWriter?.Flush();
                    await UniTask.Delay(1000);
                }
            });
        }

        public void FileLogThread()
        {
            while (threadRunning)
            {
                // 让线程进入等待，并进行阻塞
                manualResetEvent.WaitOne();
                if (streamWriter == null)
                {
                    break;
                }

                while (logDatas.Count > 0 && logDatas.TryDequeue(out LogData data))
                {
                    if (data.type == LogType.Log)
                    {
                        streamWriter.Write("Log >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.WriteLine(data.trace);
                    }
                    else if (data.type == LogType.Warning)
                    {
                        streamWriter.Write("Warning >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.WriteLine(data.trace);
                    }
                    else if (data.type == LogType.Error)
                    {
                        streamWriter.Write("Error >>> ");
                        streamWriter.WriteLine(data.log);
                        streamWriter.Write('\n');
                        streamWriter.WriteLine(data.trace);
                    }
                    streamWriter.Write("\r\n");
                }
                // 保存当前文件内容，使其生效
                streamWriter.Flush();
                // 重置信号 表示没有人指定需要工作
                manualResetEvent.Reset();
                Thread.Sleep(1);
            }
        }

        private void OnDisable()
        {
            threadRunning = false;
            // 设置一个信号 表示线程是需要工作的
            manualResetEvent?.Set();
            streamWriter?.Close();
        }

        public void OnApplicationQuit()
        {
            Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
            threadRunning = false;
            // 设置一个信号 表示线程是需要工作的
            manualResetEvent?.Set();
            streamWriter?.Close();
            streamWriter = null;
        }

        private void OnLogMessageReceivedThreaded(string condition, string stackTrace, LogType type)
        {
            logDatas.Enqueue(new LogData()
            {
                log = $"[{nowTime}] {condition}",
                trace = stackTrace,
                type = type
            });
            // 设置一个信号 表示线程是需要工作的
            manualResetEvent.Set();

            // 重置信号 表示没有人指定需要工作
            manualResetEvent.Reset();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                GameObject.Destroy(this.gameObject);
            }
        }
    }
}