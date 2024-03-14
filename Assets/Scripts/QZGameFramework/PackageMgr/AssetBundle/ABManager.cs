using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.PackageMgr.AssetBundleMgr
{
    /// <summary>
    /// AB包管理器
    /// 方便外部加载资源
    /// </summary>
    public class ABManager : SingletonAutoMono<ABManager>
    {
        // AB包存放路径
        private string AB_PATH;

        // 目标平台
        private readonly string PLATFORM =
#if UNITY_IOS
            "ISO/"
#elif UNITY_ANDROID
             "ANDROID/"
#else
                "PC/";

#endif

        // 已加载AB包容器 key —— abName  value —— AssetBundle
        private Dictionary<string, AssetBundle> abDic = new Dictionary<string, AssetBundle>();

        private AssetBundle mainAB; // 主包
        private AssetBundleManifest manifest; // 依赖配置文件
        private bool IsLoadingMainAB;

        private string MainABName // 主包名
        {
            get
            {
#if UNITY_IOS
            return "ISO"
#elif UNITY_ANDROID
            return "ANDROID"
#else
                return "PC";
#endif
            }
        }

        private void Awake()
        {
            AB_PATH = Application.persistentDataPath + "/AB/";
        }

        #region 同步加载

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public Object LoadABRes(string abName, string resName)
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            //if (!File.Exists(path))
            //{
            //    path = Application.streamingAssetsPath + PLATFORM + abName;
            //    if (!File.Exists(path))
            //    {
            //        Debug.Log("不存在对应的AB包文件");
            //        return null;
            //    }
            //}

            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                Debug.Log("不存在对应的AB包文件");
                return null;
            }

            // 加载主包和依赖配置
            LoadMainABAndDependence(abName, path);

            // 加载对应资源返回
            Object obj = abDic[abName].LoadAsset(resName);
            return obj is GameObject ? Instantiate(obj) : obj;
        }

        /// <summary>
        /// 通过Type同步加载资源
        /// </summary>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <param name="type">资源类型</param>
        /// <returns></returns>
        public Object LoadABRes(string abName, string resName, System.Type type)
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                Debug.Log("不存在对应的AB包文件");
                return null;
            }

            // 加载主包和依赖配置
            LoadMainABAndDependence(abName, path);

            // 加载对应资源返回
            Object obj = abDic[abName].LoadAsset(resName, type);
            return obj is GameObject ? Instantiate(obj) : obj;
        }

        /// <summary>
        /// 通过泛型同步加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="abName">ab包名</param>
        /// <param name="resName">资源名</param>
        /// <returns></returns>
        public T LoadABRes<T>(string abName, string resName) where T : Object
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                Debug.Log("不存在对应的AB包文件");
                return null;
            }

            // 加载主包和依赖配置
            LoadMainABAndDependence(abName, path);

            // 加载对应资源返回
            T obj = abDic[abName].LoadAsset<T>(resName);
            return obj is GameObject ? Instantiate(obj) : obj;
        }

        /// <summary>
        /// 同步加载主包和相依赖配置文件
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="abPath">AB包路径</param>
        private void LoadMainABAndDependence(string abName, string abPath)
        {
            string path = AB_PATH + PLATFORM + MainABName;
            // 判断是否存在主包文件
            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/AB/" + PLATFORM + MainABName;
                if (!File.Exists(path))
                {
                    Debug.Log("不存在对应的AB主包文件");
                    return;
                }
            }
            // 加载主包和依赖配置文件
            if (mainAB == null)
            {
                mainAB = AssetBundle.LoadFromFile(path);
                manifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            }

            // 得到所有依赖的包名
            string[] strs = manifest.GetAllDependencies(abName);
            foreach (string str in strs)
            {
                // 判断是否存在AB包文件
                path = AB_PATH + PLATFORM + str;
                if (!File.Exists(path))
                {
                    path = Application.streamingAssetsPath + PLATFORM + str;
                    if (!File.Exists(path))
                    {
                        Debug.Log("不存在对应的AB包文件");
                        return;
                    }
                }
                // 判断是否加载过依赖包
                if (!abDic.ContainsKey(str))
                {
                    abDic.Add(str, AssetBundle.LoadFromFile(path));
                }
            }

            if (!abDic.ContainsKey(abName))
            {
                abDic.Add(abName, AssetBundle.LoadFromFile(abPath));
            }
        }

        #endregion

        #region 异步加载AB包资源

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        public void LoadABResAsync(string abName, string resName, UnityAction<Object> callback)
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                return;
            }

            // 异步加载资源
            StartCoroutine(LoadABResCoroutineAsync(abName, resName, path, callback));
        }

        private IEnumerator LoadABResCoroutineAsync(string abName, string resName, string abPath, UnityAction<Object> callback)
        {
            // 异步开启加载主包协程
            yield return StartCoroutine(LoadMainABAndDependenceCoroutineAsync(abName, abPath));

            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName);
            yield return abr;
            Object obj = abr.asset;
            callback?.Invoke(obj is GameObject ? Instantiate(obj) : obj);
        }

        /// <summary>
        /// 通过Type异步加载资源
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        public void LoadABResAsync(string abName, string resName, System.Type type, UnityAction<Object> callback)
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                return;
            }

            // 异步加载资源
            StartCoroutine(LoadABResCoroutineAsync(abName, resName, path, type, callback));
        }

        private IEnumerator LoadABResCoroutineAsync(string abName, string resName, string abPath, System.Type type, UnityAction<Object> callback)
        {
            // 异步开启加载主包协程
            yield return StartCoroutine(LoadMainABAndDependenceCoroutineAsync(abName, abPath));

            AssetBundleRequest abr = abDic[abName].LoadAssetAsync(resName, type);
            yield return abr;
            Object obj = abr.asset;
            callback?.Invoke(obj is GameObject ? Instantiate(obj) : obj);
        }

        /// <summary>
        /// 通过泛型异步加载资源
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="resName">资源名</param>
        public void LoadABResAsync<T>(string abName, string resName, UnityAction<T> callback) where T : Object
        {
            string path = AB_PATH + PLATFORM + abName;
            // 判断是否存在AB包文件
            if (!CheckExistsAB(abName, ref path))
            {
                return;
            }

            // 异步加载资源
            StartCoroutine(LoadABResCoroutineAsync<T>(abName, resName, path, callback));
        }

        private IEnumerator LoadABResCoroutineAsync<T>(string abName, string resName, string abPath, UnityAction<T> callback) where T : Object
        {
            // 异步开启加载主包协程
            yield return StartCoroutine(LoadMainABAndDependenceCoroutineAsync(abName, abPath));

            AssetBundleRequest abr = abDic[abName].LoadAssetAsync<T>(resName);
            yield return abr;
            T obj = abr.asset as T;
            callback?.Invoke(obj is GameObject ? Instantiate(obj) : obj);
        }

        /// <summary>
        /// 异步加载主包和相依赖配置文件
        /// </summary>
        /// <param name="abName">AB包名</param>
        /// <param name="abPath">AB包路径</param>
        private IEnumerator LoadMainABAndDependenceCoroutineAsync(string abName, string abPath)
        {
            string path = AB_PATH + PLATFORM + MainABName;
            // 判断是否存在主包文件
            if (!CheckExistsAB(MainABName, ref path))
            {
                yield break;
            }

            //if (!File.Exists(path))
            //{
            //    path = Application.streamingAssetsPath + "/AB/" + PLATFORM + MainABName;
            //    if (!File.Exists(path))
            //    {
            //        Debug.Log($"不存在{MainABName}的AB包文件");
            //        yield break;
            //    }
            //}

            // 加锁等待上一次异步操作完成，防止异步多次执行加载同一包操作
            while (IsLoadingMainAB)
            {
                yield return null;
            }

            IsLoadingMainAB = true; // 修改加载标识

            // 加载主包和依赖配置文件
            if (mainAB == null)
            {
                AssetBundleCreateRequest abcr = AssetBundle.LoadFromFileAsync(path);
                yield return abcr;
                // 异步加载主包
                mainAB = abcr.assetBundle;
                // 异步加载依赖配置
                AssetBundleRequest abr = mainAB.LoadAssetAsync<AssetBundleManifest>("AssetBundleManifest");
                yield return abr;
                manifest = abr.asset as AssetBundleManifest;
            }

            // 得到所有依赖的包名
            string[] strs = manifest.GetAllDependencies(abName);
            AssetBundle ab = null;
            foreach (string str in strs)
            {
                // 判断是否存在AB包文件
                path = AB_PATH + PLATFORM + str;
                if (!CheckExistsAB(str, ref path))
                {
                    yield break;
                }
                //if (!File.Exists(path))
                //{
                //    path = Application.streamingAssetsPath + PLATFORM + str;
                //    if (!File.Exists(path))
                //    {
                //        Debug.Log($"不存在{str}的AB包文件");
                //        yield break;
                //    }
                //}

                // 判断是否加载过依赖包
                if (!abDic.ContainsKey(str))
                {
                    ab = AssetBundle.LoadFromFileAsync(path).assetBundle;
                    yield return ab;
                    abDic.Add(str, ab);
                }
            }

            if (!abDic.ContainsKey(abName))
            {
                ab = AssetBundle.LoadFromFileAsync(abPath).assetBundle;
                yield return abDic;
                abDic.Add(abName, ab);
            }

            IsLoadingMainAB = false; // 修改加载标识
        }

        #endregion

        /// <summary>
        /// 判断存储路径是否存在AB包资源
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckExistsAB(string abName, ref string path)
        {
            if (!File.Exists(path))
            {
                path = Application.streamingAssetsPath + "/AB/" + PLATFORM + abName;
                if (!File.Exists(path))
                {
                    Debug.Log($"不存在{abName}的AB包文件");
                    return false;
                }
            }
            return true;
        }

        #region 同步卸载AB包资源

        /// <summary>
        /// 卸载单个AB包
        /// </summary>
        /// <param name="abName"></param>
        public void UnLoadABRes(string abName)
        {
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                ab.Unload(false);
                abDic.Remove(abName);
            }
        }

        /// <summary>
        /// 卸载所有AB包资源，进行GC操作
        /// </summary>
        public void ClearABRes()
        {
            AssetBundle.UnloadAllAssetBundles(false);
            Resources.UnloadUnusedAssets();
            abDic.Clear();
            mainAB = null;
            manifest = null;
            //GC.Collect();
        }

        #endregion

        #region 异步卸载AB包资源

        /// <summary>
        /// 异步卸载单个AB包
        /// </summary>
        /// <param name="abName"></param>
        public void UnLoadABResAsync(string abName)
        {
            StartCoroutine(UnLoadABResCoroutineAsync(abName));
        }

        private IEnumerator UnLoadABResCoroutineAsync(string abName)
        {
            if (abDic.TryGetValue(abName, out AssetBundle ab))
            {
                yield return ab.UnloadAsync(false);
                abDic.Remove(abName);
            }
        }

        #endregion
    }
}