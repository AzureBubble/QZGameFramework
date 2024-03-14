using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.PackageMgr.ResourcesManager
{
    public abstract class ResInfoBase
    { }

    public class ResInfo<T> : ResInfoBase
    {
        public T asset;
        public UnityAction<T> callback;
        public Coroutine coroutine;
        public bool isDel;
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
        /// <param name="objName">物体实例化后名字</param>
        /// <returns></returns>
        public T LoadRes<T>(string path) where T : Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResInfo<T> res;
            if (resDic.ContainsKey(keyName))
            {
                res = resDic[keyName] as ResInfo<T>;
                if (res.asset != null)
                {
                    return res.asset;
                }
                else
                {
                    SingletonManager.StopCoroutine(res.coroutine);
                    res.asset = Resources.Load<T>(path);
                    res.callback?.Invoke(res.asset);
                    res.callback = null;
                    res.coroutine = null;
                    return res.asset;
                }
            }
            else
            {
                res = new ResInfo<T>();
                res.asset = Resources.Load<T>(path);
                resDic.Add(keyName, res);
                return res.asset;
            }
        }

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="path">资源路径</param>
        /// <param name="callback">回调函数</param>
        /// <param name="objName">物体实例化后名字</param>
        public void LoadResAsync<T>(string path, UnityAction<T> callback) where T : UnityEngine.Object
        {
            string keyName = typeof(T).Name + "_" + path;
            ResInfo<T> res;
            if (!resDic.ContainsKey(keyName))
            {
                // 如果没加载过该资源 则开启协程加载
                res = new ResInfo<T>();
                // 记录到字典中
                resDic.Add(keyName, res);
                // 记录回调函数
                res.callback += callback;
                // 协程异步加载资源
                res.coroutine = SingletonManager.StartCoroutine(LoadResAsyncCoroutine<T>(path));
            }
            else
            {
                // 如果已经开启了加载协程
                res = resDic[keyName] as ResInfo<T>;

                if (res.asset != null)
                {
                    // 异步加载已经完成 直接调用回调函数
                    callback?.Invoke(res.asset);
                }
                else
                {
                    // 未完成 则记录回调函数
                    res.callback += callback;
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
                ResInfo<T> res = resDic[keyName] as ResInfo<T>;
                res.asset = rr.asset as T;

                if (res.isDel)
                {
                    UnloadAsset<T>(path);
                    yield break;
                }
                // 等待异步加载完成 进行回调函数
                res.callback?.Invoke(res.asset);
                // 加载完毕 清空回调函数和协程引用
                res.callback = null;
                res.coroutine = null;
            }
        }

        /// <summary>
        /// 卸载指定资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        public void UnloadAsset<T>(string path) where T : Object
        {
            string keyName = typeof(T).Name + "_" + path;
            if (resDic.ContainsKey(keyName))
            {
                ResInfo<T> res = resDic[keyName] as ResInfo<T>;
                if (res.asset != null)
                {
                    resDic.Remove(keyName);
                    Resources.UnloadAsset(res.asset);
                }
                else
                {
                    // 如果资源存在 但还没加载完成 打上删除标记
                    res.isDel = true;
                }
            }
        }
    }
}