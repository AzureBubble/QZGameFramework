using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.PackageMgr.ResourcesManager
{
    public abstract class ResInfoBase
    {
        /// <summary>
        /// 引用计数
        /// </summary>
        public int RefCount { get; protected set; }
    }

    public class ResInfo<T> : ResInfoBase
    {
        /// <summary>
        /// 资源
        /// </summary>
        public T asset;

        /// <summary>
        /// 异步加载资源结束后 传递资源给外部的回调函数
        /// </summary>
        public UnityAction<T> callback;

        /// <summary>
        /// UniTask 取消标记
        /// </summary>
        public CancellationTokenSource loadCancelTokenSource = new CancellationTokenSource();

        public Coroutine coroutine;

        /// <summary>
        /// 引用计数为0时 控制资源的释放时机
        /// </summary>
        public bool isDelImmediate;

        public void AddRefCount()
        {
            ++RefCount;
        }

        public void SubRefCount()
        {
            --RefCount;
            if (RefCount < 0)
            {
                Debug.LogError("The RefCount is less than 0, please check whether resource usage and uninstallation are paired.");
            }
        }
    }

    public class ResourcesMgr : Singleton<ResourcesMgr>
    {
        // 已加载资源存储字典 key-资源类型_资源路径 value-ResInfo<T>
        private Dictionary<string, ResInfoBase> resDic = new Dictionary<string, ResInfoBase>();

        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <returns></returns>
        public T LoadRes<T>(string path) where T : Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResInfo<T> resInfo;
            if (resDic.ContainsKey(keyName))
            {
                resInfo = resDic[keyName] as ResInfo<T>;
                resInfo.AddRefCount();
                if (resInfo.asset != null)
                {
                    return resInfo.asset;
                }
                else
                {
                    resInfo.loadCancelTokenSource.Cancel();
                    //SingletonManager.StopCoroutine(resInfo.coroutine);
                    resInfo.asset = Resources.Load<T>(path);
                    resInfo.callback?.Invoke(resInfo.asset);
                    resInfo.callback = null;
                    resInfo.coroutine = null;
                    return resInfo.asset;
                }
            }
            else
            {
                resInfo = new ResInfo<T>();
                resInfo.AddRefCount();
                resInfo.asset = Resources.Load<T>(path);
                resDic.Add(keyName, resInfo);
                return resInfo.asset;
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        public void LoadResAsync<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResInfo<T> resInfo;
            if (!resDic.ContainsKey(keyName))
            {
                // 如果没加载过该资源 则开启协程加载
                resInfo = new ResInfo<T>();
                // 引用计数
                resInfo.AddRefCount();
                // 记录到字典中
                resDic.Add(keyName, resInfo);
                // 记录回调函数
                resInfo.callback += callback;
                // 异步加载资源
                //resInfo.coroutine = SingletonManager.StartCoroutine(LoadResAsyncCoroutine<T>(path));
                LoadResAsyncByUniTask<T>(path, resInfo.loadCancelTokenSource).Forget();
            }
            else
            {
                // 如果已经开启了加载协程
                resInfo = resDic[keyName] as ResInfo<T>;
                // 引用计数
                resInfo.AddRefCount();
                if (resInfo.asset != null)
                {
                    // 异步加载已经完成 直接调用回调函数
                    callback?.Invoke(resInfo.asset);
                }
                else
                {
                    // 未完成 则记录回调函数
                    resInfo.callback += callback;
                }
            }
        }

        private IEnumerator LoadResAsyncCoroutine<T>(string path) where T : UnityEngine.Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            // 等待异步加载结束
            yield return rr;
            // 根据资源类型的不同进行不同处理
            if (resDic.ContainsKey(keyName))
            {
                ResInfo<T> resInfo = resDic[keyName] as ResInfo<T>;
                resInfo.asset = rr.asset as T;

                if (resInfo.RefCount == 0)
                {
                    UnloadAsset<T>(path, isSubResCount: false, isDelImmediate: resInfo.isDelImmediate);
                    yield break;
                }
                // 等待异步加载完成 进行回调函数
                resInfo.callback?.Invoke(resInfo.asset);
                // 加载完毕 清空回调函数和协程引用
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }

        /// <summary>
        /// 使用UniTask实现异步操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        private async UniTask LoadResAsyncByUniTask<T>(string path, CancellationTokenSource cancellationSource) where T : UnityEngine.Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResourceRequest rr = Resources.LoadAsync<T>(path);
            CancellationToken cancellationToken = cancellationSource.Token;
            // 等待异步加载结束
            await UniTask.WaitUntil(() => rr.isDone || cancellationToken.IsCancellationRequested);

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // 根据资源类型的不同进行不同处理
            if (resDic.ContainsKey(keyName))
            {
                ResInfo<T> resInfo = resDic[keyName] as ResInfo<T>;
                resInfo.asset = rr.asset as T;

                if (resInfo.RefCount == 0)
                {
                    UnloadAsset<T>(path, isSubResCount: false, isDelImmediate: resInfo.isDelImmediate);
                    return;
                }
                // 等待异步加载完成 进行回调函数
                resInfo.callback?.Invoke(resInfo.asset);
                // 加载完毕 清空回调函数和协程引用
                resInfo.callback = null;
                resInfo.coroutine = null;
            }
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">删除资源后的回调函数</param>
        /// <param name="isSubResCount">引用计数是否进行减少</param>
        /// <param name="isDelImmediate">是否立即删除资源</param>
        public void UnloadAsset<T>(string path, UnityAction<T> callback = null, bool isSubResCount = true, bool isDelImmediate = true) where T : Object
        {
            string keyName = typeof(T).Name + "_" + path;
            if (resDic.ContainsKey(keyName))
            {
                ResInfo<T> resInfo = resDic[keyName] as ResInfo<T>;
                if (isSubResCount)
                {
                    resInfo.SubRefCount();
                }
                resInfo.isDelImmediate = isDelImmediate;
                if (resInfo.asset != null && resInfo.RefCount == 0 && isDelImmediate)
                {
                    resDic.Remove(keyName);
                    Resources.UnloadAsset(resInfo.asset);
                }
                else if (resInfo.asset == null)
                {
                    if (callback != null)
                    {
                        resInfo.callback -= callback;
                    }
                }
            }
        }

        public T[] LoadAllAssets<T>(string path) where T : Object
        {
            string keyName = typeof(T).Name + "_";
            T[] assets = Resources.LoadAll<T>(path);
            List<T> delAssets = new List<T>();

            for (int i = 0; i < assets.Length; i++)
            {
                ResInfo<T> resInfo = new ResInfo<T>();
                keyName += Path.Combine(path, assets[i].name);
                if (resDic.ContainsKey(keyName))
                {
                    resInfo = resDic[keyName] as ResInfo<T>;
                    if (resInfo.asset != null)
                    {
                        delAssets.Add(assets[i]);
                        assets[i] = resInfo.asset;
                        resInfo.AddRefCount();
                    }
                    else
                    {
                        resInfo.loadCancelTokenSource.Cancel();
                        //SingletonManager.StopCoroutine(resInfo.coroutine);
                        resInfo.asset = assets[i];
                        resInfo.callback?.Invoke(resInfo.asset);
                        resInfo.callback = null;
                        resInfo.coroutine = null;
                    }
                }
                else
                {
                    resInfo = new ResInfo<T>();
                    resInfo.asset = assets[i];
                    resInfo.AddRefCount();
                    resDic.Add(keyName, resInfo);
                }
            }

            UnLoadExistAssets<T>(delAssets).Forget();

            return assets;
        }

        private async UniTaskVoid UnLoadExistAssets<T>(List<T> assets) where T : Object
        {
            if (assets.Count <= 0) return;
            for (int i = assets.Count - 1; i >= 0; i--)
            {
                Resources.UnloadAsset(assets[i]);
                await UniTask.Yield();
            }
        }

        /// <summary>
        /// 异步移除所有不再使用的资源
        /// </summary>
        /// <param name="callback"></param>
        public void UnLoadAllUnusedAssets(UnityAction callback = null)
        {
            UnLoadUnusedAssetsAsync(callback).Forget();
        }

        private async UniTaskVoid UnLoadUnusedAssetsAsync(UnityAction callback)
        {
            List<string> removeResInfoList = new List<string>();

            foreach (var resInfoKey in resDic.Keys)
            {
                if (resDic[resInfoKey].RefCount == 0)
                {
                    removeResInfoList.Add(resInfoKey);
                }
            }
            for (int i = 0; i < removeResInfoList.Count; i++)
            {
                resDic.Remove(removeResInfoList[i]);
            }

            await Resources.UnloadUnusedAssets();

            callback?.Invoke();
        }

        /// <summary>
        /// 获取某个资源的引用计数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public int GetRefCount<T>(string path)
        {
            string keyName = typeof(T).Name + "_" + path;
            if (resDic.TryGetValue(keyName, out ResInfoBase resInfoBase))
            {
                return (resInfoBase as ResInfo<T>).RefCount;
            }

            return 0;
        }
    }
}