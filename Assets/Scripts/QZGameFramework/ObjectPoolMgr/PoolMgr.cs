using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QZGameFramework.ObjectPoolManager
{
    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolMgr : Singleton<PoolMgr>
    {
        /// <summary>
        /// 缓存池容器 键：某一类物品名字，值：游戏对象
        /// </summary>
        private Dictionary<string, BasePoolData> poolDic = new Dictionary<string, BasePoolData>();

        /// <summary>
        /// 用于存储数据结构类 逻辑类对象的池子容器
        /// </summary>
        private Dictionary<string, BasePoolObject> classPoolDic = new Dictionary<string, BasePoolObject>();

        #region Stack GameObjectPool

        /// <summary>
        /// 异步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="callback">回调函数</param>
        /// <param name="path">物体存放路径</param>
        //public void GetStackObjAsync(string name, UnityAction<GameObject> callback = null, string path = "Prefabs/")
        //{
        //    // 判断对应的对象池是否存在
        //    if (!poolDic.ContainsKey(name)
        //        || (poolDic[name].Count() == 0 && poolDic[name].NeedCreate()))
        //    {
        //        // 异步加载预制体资源
        //        ResourcesMgr.Instance.LoadResAsync<GameObject>(Path.Combine(path, name), (resObj) =>
        //        {
        //            GameObject obj = GameObject.Instantiate(resObj);
        //            obj.name = name;
        //            callback?.Invoke(obj);

        //            if (!poolDic.ContainsKey(name))
        //            {
        //                //poolDic.Add(name, new PoolData(resObj));
        //                poolDic.Add(name, new StackPoolData(resObj));
        //            }
        //            else
        //            {
        //                if (!poolDic[name].Contains(resObj))
        //                {
        //                    poolDic[name].PushUsedList(resObj);
        //                }
        //            }
        //        });
        //    }
        //    else
        //    {
        //        callback?.Invoke(poolDic[name].GetObj());
        //    }
        //}

        /// <summary>
        /// 同步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="path">物体存放路径</param>
        public GameObject GetStackObj(string name, string path = "Prefabs/")
        {
            GameObject obj = null;
            // 判断对应的对象池是否存在
            if (!poolDic.ContainsKey(name)
                || (poolDic[name].Count() == 0 && poolDic[name].NeedCreate()))
            {
                // 加载预制体资源
                obj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>(Path.Combine(path, name)));
                obj.name = name;

                if (!poolDic.ContainsKey(name))
                {
                    poolDic.Add(name, new StackPoolData(obj));
                }
                else
                {
                    poolDic[name].PushUsedList(obj);
                }
            }
            else
            {
                obj = poolDic[name].GetObj();
            }

            return obj;
        }

        /// <summary>
        /// 把物体放回对象池中
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="obj">归还的物体对象</param>
        public void ReleaseStackObj(GameObject obj)
        {
            if (!poolDic.ContainsKey(obj.name))
            {
                poolDic.Add(obj.name, new StackPoolData(obj));
            }
            poolDic[obj.name].ReleaseObj(obj);
        }

        #endregion

        #region Queue GameObjectPool

        /// <summary>
        /// 异步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="callback">回调函数</param>
        /// <param name="path">物体存放路径</param>
        //public void GetQueueObjAsync(string name, UnityAction<GameObject> callback = null, string path = "Prefabs/")
        //{
        //    // 判断对应的对象池是否存在
        //    if (!poolDic.ContainsKey(name)
        //        || (poolDic[name].Count() == 0 && poolDic[name].NeedCreate()))
        //    {
        //        // 异步加载预制体资源
        //        ResourcesMgr.Instance.LoadResAsync<GameObject>(Path.Combine(path, name), (resObj) =>
        //        {
        //            GameObject obj = GameObject.Instantiate(resObj);
        //            obj.name = name;
        //            callback?.Invoke(obj);

        //            if (!poolDic.ContainsKey(name))
        //            {
        //                //poolDic.Add(name, new PoolData(resObj));
        //                poolDic.Add(name, new QueuePoolData(resObj));
        //            }
        //            else
        //            {
        //                if (!poolDic[name].Contains(resObj))
        //                {
        //                    poolDic[name].PushUsedList(resObj);
        //                }
        //            }
        //        });
        //    }
        //    else
        //    {
        //        callback?.Invoke(poolDic[name].GetObj());
        //    }
        //}

        /// <summary>
        /// 同步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="path">物体存放路径</param>
        public GameObject GetQueueObj(string name, string path = "Prefabs/")
        {
            GameObject obj = null;
            // 判断对应的对象池是否存在
            if (!poolDic.ContainsKey(name)
                || (poolDic[name].Count() == 0 && poolDic[name].NeedCreate()))
            {
                // 加载预制体资源
                obj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>(Path.Combine(path, name)));
                obj.name = name;

                if (!poolDic.ContainsKey(name))
                {
                    poolDic.Add(name, new QueuePoolData(obj));
                }
                else
                {
                    poolDic[name].PushUsedList(obj);
                }
            }
            else
            {
                obj = poolDic[name].GetObj();
            }

            return obj;
        }

        /// <summary>
        /// 把物体放回对象池中
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="obj">归还的物体对象</param>
        public void ReleaseQueueObj(string name, GameObject obj)
        {
            if (!poolDic.ContainsKey(name))
            {
                poolDic.Add(name, new QueuePoolData(obj));
            }
            poolDic[name].ReleaseObj(obj);
        }

        #endregion

        #region 数据结构类和逻辑类 对象池

        /// <summary>
        /// 同步获取自定义数据结构类和逻辑类对象
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <returns></returns>
        public T GetObj<T>(string nameSpace = "") where T : class, IClassPoolObject, new()
        {
            string poolName = nameSpace + "_" + typeof(T).Name;

            if (classPoolDic.ContainsKey(poolName))
            {
                //ClassPoolObject<T> pool = classPoolDic[poolName] as ClassPoolObject<T>;

                //if (pool.classQueue.Count > 0)
                //{
                //    return pool.classQueue.Dequeue();
                //}
                return classPoolDic[poolName].GetObj() as T;
            }

            return new T();
        }

        /// <summary>
        /// 把自定义数据结构类和逻辑类对象放回对象池中
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        public void ReleaseObj<T>(T obj, string nameSpace = "") where T : class, IClassPoolObject, new()
        {
            if (obj == null)
            {
                Debug.LogError("The object cannot be empty. ObjType: " + typeof(T));
                return;
            }

            string poolName = nameSpace + "_" + typeof(T).Name;
            obj.Rest();

            if (!classPoolDic.ContainsKey(poolName))
            {
                classPoolDic.Add(poolName, new ClassPoolObject<T>());
            }
            classPoolDic[poolName].ReleaseObj(obj);
        }

        #endregion

        /// <summary>
        /// 清空所有的缓存池
        /// </summary>
        public void Clear()
        {
            foreach (BasePoolData pool in poolDic.Values)
            {
                pool.Clear();
            }

            foreach (BasePoolObject item in classPoolDic.Values)
            {
                item.Clear();
            }

            poolDic.Clear();
            classPoolDic.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            Clear();
            base.Dispose();
        }
    }
}