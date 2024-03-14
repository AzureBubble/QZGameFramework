using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace QZGameFramework.PackageMgr.AddressablesManager
{
    public class AddressablesInfo
    {
        //记录 异步操作句柄
        public AsyncOperationHandle handle;

        //记录 引用计数
        public uint count;

        public AddressablesInfo(AsyncOperationHandle handle)
        {
            this.handle = handle;
            count += 1;
        }
    }

    /// <summary>
    /// Addressables 包管理器
    /// </summary>
    public class AddressablesMgr : Singleton<AddressablesMgr>
    {
        /// <summary>
        /// 存储异步加载的返回值
        /// </summary>
        private Dictionary<string, AddressablesInfo> resDict = new Dictionary<string, AddressablesInfo>();

        #region 异步加载资源的方法

        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名</param>
        /// <param name="callback">加载结束回调函数</param>
        public void LoadAssetAsync<T>(string resName, Action<AsyncOperationHandle<T>> callback)
        {
            // 区分同名 但不同类型资源的 key = 资源名_资源类型
            string key = resName + "_" + typeof(T).Name;
            AsyncOperationHandle<T> handle;

            // 如果加载过此资源
            if (resDict.ContainsKey(key))
            {
                // 获取异步加载返回的句柄
                handle = resDict[key].handle.Convert<T>();
                // 使用资源 则引用计数 + 1
                resDict[key].count++;
                // 异步加载是否结束
                if (handle.IsDone)
                {
                    // 如果成功 则执行回调函数 把句柄返回
                    callback(handle);
                }
                else
                {
                    // 未结束加载 则在加载添加加载结束事件
                    handle.Completed += (operation) =>
                    {
                        if (operation.Status == AsyncOperationStatus.Succeeded)
                        {
                            callback(operation);
                        }
                    };
                }
                return;
            }

            // 如果没加载过此资源
            // 直接进行异步加载 并且存入字典中
            handle = Addressables.LoadAssetAsync<T>(resName);
            // 资源加载完成事件
            handle.Completed += (operation) =>
            {
                // 如果加载成功
                if (operation.Status == AsyncOperationStatus.Succeeded)
                {
                    callback(operation);
                }
                else
                {
                    // 资源加载失败 则移除字典中的资源
                    Debug.LogWarning(key + "资源加载失败");
                    if (resDict.ContainsKey(key))
                    {
                        resDict.Remove(key);
                    }
                }
            };

            // 第一次加载资源 则存储到字典中
            resDict.Add(key, new AddressablesInfo(handle));
        }

        public void LoadAssetAsync<T>(Addressables.MergeMode mode, Action<T> callback, params string[] keys)
        {
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="resName">资源名</param>
        public void Release<T>(string resName)
        {
            string key = resName + "_" + typeof(T).Name;

            if (resDict.ContainsKey(key))
            {
                // 释放资源 先把引用计数 - 1
                resDict[key].count--;
                // 如果引用计数为0 则才进行资源释放
                if (resDict[key].count <= 0)
                {
                    // 取出对象 移除资源 字典中删除
                    AsyncOperationHandle<T> handle = resDict[key].handle.Convert<T>();
                    Addressables.Release(handle);
                    resDict.Remove(key);
                }
            }
        }

        #endregion

        /// <summary>
        /// 清空资源 释放资源
        /// </summary>
        public void Clear()
        {
            foreach (AddressablesInfo info in resDict.Values)
            {
                Addressables.Release(info.handle);
            }
            resDict.Clear();
            AssetBundle.UnloadAllAssetBundles(true);
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
    }
}