using QZGameFramework.GFEventCenter;
using QZGameFramework.NetManager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

/// <summary>
/// AB包更新管理器
/// </summary>
public class ABUpdateMgr : MonoBehaviour
{
    private readonly string USER_NAME = "root"; // 用户名
    private readonly string PASSWORD = "root"; //密码
    private readonly string SERVER_IP = "ftp://192.168.168.128/AB/"; // 服务端IP地址

    private static ABUpdateMgr instance;

    public static ABUpdateMgr Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("ABUpdateMgr");
                instance = obj.AddComponent<ABUpdateMgr>();
            }
            return instance;
        }
    }

    private string localPath; // 本地存放路径

    private string TargetPlatform // 目标平台
    {
        get
        {
#if UNITY_ANDROID
            return "Android";
#elif UNITY_IOS
            return "IOS";
#else
            return "PC";
#endif
        }
    }

    // 存储远端AB包信息字典 用于与本地AB包进行对比更新
    private Dictionary<string, ABInfo> remoteABDic;

    // 存储本地AB包信息字典 用于与远端AB包信息对比
    private Dictionary<string, ABInfo> localABDic;

    // 存储待从服务端下载的AB包名
    private List<string> downLoadABList;

    private void Awake()
    {
        // 初始化存放路径
        localPath = Application.persistentDataPath + "/AB/";
        remoteABDic = new Dictionary<string, ABInfo>();
        localABDic = new Dictionary<string, ABInfo>();
        downLoadABList = new List<string>();
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void CheckUpdateABFile()
    {
        // 清楚缓存
        remoteABDic.Clear();
        localABDic.Clear();
        downLoadABList.Clear();

        // 加载远端AB包对比文件
        EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "开始下载AB包对比文件");
        DownLoadABCompareFile((isDownLoadABCompareFileOver) =>
        {
            // 远端对比文件下载成功后 继续向下执行
            if (isDownLoadABCompareFileOver)
            {
                EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "下载AB包对比文件完成,开始解析");
                // 解析远端AB包对比文件
                // 读取下载的AB包对比文件
                string remoteABCompareInfo = File.ReadAllText(localPath + "ABCompareInfo_TMP.txt");
                AnalysisABCompareFileInfo(remoteABCompareInfo, remoteABDic);
                EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "解析远端对比文件完成");

                EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "开始解析本地对比文件");

                // 异步协程解析本地AB包对比文件
                AnalysisLocalABCompareFileInfoAsync((isAnalysisLocalOver) =>
                {
                    if (isAnalysisLocalOver)
                    {
                        long downLoadSize = 0;
                        EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "开始对比AB包文件");
                        foreach (string abName in remoteABDic.Keys)
                        {
                            // 如果本地不存在AB包，则加入待下载列表中
                            if (!localABDic.ContainsKey(abName))
                            {
                                downLoadABList.Add(abName);
                                downLoadSize += remoteABDic[abName].size;
                            }
                            else
                            {
                                // 存在，则判断两个AB包的MD5码是否一样，不一样则代表是新包
                                if (!localABDic[abName].Equals(remoteABDic[abName]))
                                {
                                    // 加入待下载列表
                                    downLoadABList.Add(abName);
                                    downLoadSize += remoteABDic[abName].size;
                                }
                                // 检测完待下载包后 从本地字典中移除 最后字典中剩下的包则为垃圾
                                localABDic.Remove(abName);
                            }
                        }
                        // 把需要更新的AB包文件大小通过事件中心传出外部
                        EventCenter.Instance.EventTrigger<long>(E_EventType.FtpDownLoadFileSize, downLoadSize);
                        EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "对比AB包文件完成");
                        EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "删除无用AB包，释放空间");
                        // 删除多余的AB包文件，释放内存空间
                        foreach (string abName in localABDic.Keys)
                        {
                            if (File.Exists(localPath + abName))
                            {
                                File.Delete(localPath + abName);
                            }
                        }
                        EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "开始下载更新AB包文件");
                        // 下载更新AB包文件
                        DownLoadABFile((isDownLoadAbFileOver) =>
                        {
                            if (isDownLoadAbFileOver)
                            {
                                EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "更新本地AB包对比文件");

                                // 把最新AB包对比文件写入本地
                                File.WriteAllText(localPath + "ABCompareInfo.txt", remoteABCompareInfo);
                            }
                            // 通知外部AB包更新状态
                            EventCenter.Instance.EventTrigger<bool>(E_EventType.UpdateStatus, isDownLoadAbFileOver);
                        });
                    }
                    else
                    {
                        // 解析本地AB包文件失败
                        EventCenter.Instance.EventTrigger<bool>(E_EventType.UpdateStatus, false);
                    }
                });
            }
            else
            {
                // 远端AB包对比文件下载失败
                EventCenter.Instance.EventTrigger<bool>(E_EventType.UpdateStatus, false);
            }
        });
    }

    /// <summary>
    /// 下载更新AB包文件
    /// </summary>
    /// <param name="callback">回调函数</param>
    private async void DownLoadABFile(UnityAction<bool> callback)
    {
        // 文件存放路径
        string path = Application.persistentDataPath + "/AB/";
        // 服务端IP地址
        string serverIP = SERVER_IP + TargetPlatform + "/";
        // 存储下载成功的AB包列表
        List<string> tempList = new List<string>();

        // 开始下载更新AB包文件
        foreach (string abName in downLoadABList)
        {
            await FtpManager.Instance.DownLoadFileAsync(abName, path + abName, serverIP, USER_NAME, PASSWORD, (result) =>
            {
                // 下载成功 才加入成功列表中
                if (result)
                {
                    tempList.Add(abName);
                }
            });
        }

        // 循环删除已经下载的AB包文件
        foreach (string abName in tempList)
        {
            downLoadABList.Remove(abName);
        }

        callback(downLoadABList.Count == 0);
    }

    /// <summary>
    /// 解析远端AB包对比文件，获取远端AB包信息
    /// </summary>
    private void AnalysisABCompareFileInfo(string compareInfo, Dictionary<string, ABInfo> abInfoDic)
    {
        string[] strs = compareInfo.Split("|");
        string[] infos = null;
        foreach (string str in strs)
        {
            infos = str.Split(" ");
            // 把远端AB包信息存储到字典中
            abInfoDic.Add(infos[0], new ABInfo(infos[0], infos[1], infos[2]));
        }
    }

    /// <summary>
    /// 异步协程解析本地AB包对比文件
    /// </summary>
    /// <param name="callback">解析完成回调函数</param>
    private void AnalysisLocalABCompareFileInfoAsync(UnityAction<bool> callback)
    {
        string path =
#if UNITY_EDITOR
                localPath + "ABCompareInfo.txt";
#else
                "file:///" + localPath + "/ABCompareInfo.txt";
#endif

        if (!File.Exists(path))
        {
            path =
#if UNITY_ANDROID
                Application.streamingAssetsPath+"/AB/"+TargetPlatform+"/ABCompareInfo.txt";
#elif UNITY_EDITOR
                Application.streamingAssetsPath + "/AB/" + TargetPlatform + "/ABCompareInfo.txt";
#else
                "file:///" + Application.streamingAssetsPath + "/AB/" + TargetPlatform + "/ABCompareInfo.txt";
#endif
            if (!File.Exists(path))
            {
                callback?.Invoke(true);
            }
        }

        StartCoroutine(AnalysisLocalABCompareFileInfoAsync(path, callback));
    }

    /// <summary>
    /// 异步解析本地AB包对比文件的协程
    /// </summary>
    /// <param name="callback">回调函数</param>
    /// <returns></returns>
    private IEnumerator AnalysisLocalABCompareFileInfoAsync(string filePath, UnityAction<bool> callback)
    {
        // 加载本地文件
        UnityWebRequest req = UnityWebRequest.Get(filePath);
        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
        {
            // 解析本地AB包对比文件
            AnalysisABCompareFileInfo(req.downloadHandler.text, localABDic);
            // 解析成功
            callback?.Invoke(true);
            EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "解析本地对比文件成功");
        }
        else
        {
            // 解析失败
            callback?.Invoke(false);
            EventCenter.Instance.EventTrigger<string>(E_EventType.DownLoadABCompare, "解析本地对比文件失败");
        }
    }

    /// <summary>
    /// 从服务端下载AB包对比文件
    /// </summary>
    /// <param name="callback">下载完成回调函数</param>
    public async void DownLoadABCompareFile(UnityAction<bool> callback)
    {
        // 判断本地文件夹是否存在，不存在则创建
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }
        print(Application.persistentDataPath);

        // 文件存放路径
        string path = localPath + "ABCompareInfo_TMP.txt";
        // 服务端IP地址
        string serverIP = SERVER_IP + TargetPlatform + "/";

        await FtpManager.Instance.DownLoadFileAsync("ABCompareInfo.txt", path, serverIP, USER_NAME, PASSWORD, callback);
    }

    /// <summary>
    /// AB包信息类，用于检查AB包是否需要更新
    /// </summary>
    public class ABInfo
    {
        public string abName; // AB包名
        public long size; // AB包大小
        public string md5; // AB包的MD5码

        public ABInfo(string abName, string size, string md5)
        {
            this.abName = abName;
            this.size = long.Parse(size);
            this.md5 = md5;
        }

        /// <summary>
        /// 重写Equals方法，对比AB包的md5码
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            ABInfo other = obj as ABInfo;
            if (other is null) return false;
            return this.md5 == other.md5;
        }

        public override int GetHashCode()
        {
            return md5.GetHashCode();
        }
    }
}