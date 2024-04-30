using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.ObjectPoolManager
{
    #region GameObjectPool

    /// <summary>
    /// 缓存池容器对象
    /// </summary>
    public class BasePoolData
    {
        protected GameObject parentObj; // 缓存池结点
        protected int maxNum = 20;
        //public int Count { get; protected set; }

        //public int UseCount { get; protected set; }
        //public bool NeedCreate => UseCount < maxNum;

        public virtual int Count()
        { return 0; }

        public virtual int UseCount()
        { return 0; }

        public virtual bool NeedCreate()
        { return UseCount() < maxNum; }

        /// <summary>
        /// 构造函数 创建缓存池管理者对象结点，预制体缓存池结点
        /// </summary>
        /// <param name="obj">缓存池物体</param>
        public BasePoolData(GameObject obj)
        {
            // 创建父节点物体
            this.parentObj = new GameObject(obj.name + " Pool");
            GameObject.DontDestroyOnLoad(this.parentObj);
            // 把父节点物体作为缓存池管理对象的子节点
            //this.parentObj.transform.SetParent(poolMgr.transform, false);

            // 把物体压入已使用记录中
            PushUsedList(obj);

            MonoBehaviour[] scripts = obj.GetComponents<MonoBehaviour>();
            PoolObjAttribute poolObjAttr = null;
            foreach (var script in scripts)
            {
                poolObjAttr = (PoolObjAttribute)System.Attribute.GetCustomAttribute(script.GetType(), typeof(PoolObjAttribute));
                if (poolObjAttr != null)
                {
                    maxNum = poolObjAttr.MaxNum;
                    break;
                }
            }

            if (poolObjAttr == null)
            {
                Debug.LogError($"Object pool objects must have PoolObjAttribute to Set MaxNum of GameObject, Otherwise, the default value: {this.maxNum} will be used. Please check GameObject: {obj.name}.");
            }
        }

        /// <summary>
        /// 从缓存池中取出对象
        /// </summary>
        /// <returns></returns>
        public virtual GameObject GetObj()
        { return null; }

        /// <summary>
        /// 把物体压入缓存池
        /// </summary>
        /// <param name="obj"></param>
        public virtual void ReleaseObj(GameObject obj)
        { }

        public virtual void PushUsedList(GameObject obj)
        { }

        public virtual bool Contains(GameObject obj)
        { return false; }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public virtual void Clear()
        {
            GameObject.Destroy(parentObj);
            parentObj = null;
        }
    }

    /// <summary>
    /// Stack 缓存池容器对象
    /// </summary>
    public class StackPoolData : BasePoolData
    {
        //private GameObject parentObj; // 缓存池结点
        private Stack<GameObject> dataStack = new Stack<GameObject>(); // 没有使用的对象池

        private List<GameObject> usedList = new List<GameObject>(); // 使用中的对象池

        //private int maxNum = 30;
        //public new int Count => dataStack.Count;

        //public new int UseCount => usedList.Count;
        //public bool NeedCreate => UseCount < maxNum;

        public override int Count()
        {
            return dataStack.Count;
        }

        public override int UseCount()
        {
            return usedList.Count;
        }

        /// <summary>
        /// 构造函数 创建缓存池管理者对象结点，预制体缓存池结点
        /// </summary>
        /// <param name="obj">缓存池物体</param>
        public StackPoolData(GameObject obj) : base(obj)
        {
            //// 创建父节点物体
            //this.parentObj = new GameObject(obj.name + " Pool");
            //GameObject.DontDestroyOnLoad(this.parentObj);
            //// 把父节点物体作为缓存池管理对象的子节点
            ////this.parentObj.transform.SetParent(poolMgr.transform, false);

            //// 把物体压入已使用记录中
            //PushUsedList(obj);
        }

        /// <summary>
        /// 从缓存池中取出对象
        /// </summary>
        /// <returns></returns>
        public override GameObject GetObj()
        {
            GameObject obj = null;

            if (Count() > 0)
            {
                // 取出缓存池中一个对象
                obj = dataStack.Pop();
                // 在已经使用容器中记录这个对象
                usedList.Add(obj);
            }
            else
            {
                // 从已经使用的队列中取出最久没有使用的物体
                obj = usedList[0];
                usedList.RemoveAt(0);
                // 再次入队
                usedList.Add(obj);
            }

            // 激活对象
            obj.SetActive(true);
            // 断开物体和缓存池的父子关系
            obj.transform.parent = null;

            return obj;
        }

        /// <summary>
        /// 把物体压入缓存池
        /// </summary>
        /// <param name="obj"></param>
        public override void ReleaseObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parentObj.transform, false);

            // 压入栈中
            dataStack.Push(obj);

            // 删除已使用记录
            usedList.Remove(obj);
        }

        public override void PushUsedList(GameObject obj)
        {
            usedList.Add(obj);
        }

        public override bool Contains(GameObject obj)
        {
            return dataStack.Contains(obj);
        }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public override void Clear()
        {
            dataStack.Clear();
            usedList.Clear();
            GameObject.Destroy(parentObj);
            parentObj = null;
        }
    }

    /// <summary>
    /// Queue 缓存池容器对象
    /// </summary>
    public class QueuePoolData : BasePoolData
    {
        //private GameObject parentObj; // 缓存池结点
        private Queue<GameObject> dataQueue = new Queue<GameObject>(); // 没有使用的对象池

        private List<GameObject> usedList = new List<GameObject>(); // 使用中的对象池
        //private int maxNum = 30;
        //public int Count => dataStack.Count;
        //public int UseCount => usedList.Count;
        //public bool NeedCreate => UseCount < maxNum;

        public override int Count()
        {
            return dataQueue.Count;
        }

        public override int UseCount()
        {
            return usedList.Count;
        }

        /// <summary>
        /// 构造函数 创建缓存池管理者对象结点，预制体缓存池结点
        /// </summary>
        /// <param name="obj">缓存池物体</param>
        public QueuePoolData(GameObject obj) : base(obj)
        {
            //// 创建父节点物体
            //this.parentObj = new GameObject(obj.name + " Pool");
            //GameObject.DontDestroyOnLoad(this.parentObj);
            //// 把父节点物体作为缓存池管理对象的子节点
            ////this.parentObj.transform.SetParent(poolMgr.transform, false);

            //// 把物体压入已使用记录中
            //PushUsedList(obj);
        }

        /// <summary>
        /// 从缓存池中取出对象
        /// </summary>
        /// <returns></returns>
        public override GameObject GetObj()
        {
            GameObject obj = null;

            if (Count() > 0)
            {
                // 取出缓存池中一个对象
                obj = dataQueue.Dequeue();
                // 在已经使用容器中记录这个对象
                usedList.Add(obj);
            }
            else
            {
                // 从已经使用的队列中取出最久没有使用的物体
                obj = usedList[0];
                usedList.RemoveAt(0);
                // 再次入队
                usedList.Add(obj);
            }

            // 激活对象
            obj.SetActive(true);
            // 断开物体和缓存池的父子关系
            obj.transform.parent = null;
            Debug.LogError(maxNum);
            return obj;
        }

        /// <summary>
        /// 把物体压入缓存池
        /// </summary>
        /// <param name="obj"></param>
        public override void ReleaseObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parentObj.transform, false);

            // 压入栈中
            dataQueue.Enqueue(obj);
            // 删除已使用记录
            usedList.Remove(obj);
        }

        public override void PushUsedList(GameObject obj)
        {
            usedList.Add(obj);
        }

        public override bool Contains(GameObject obj)
        {
            return dataQueue.Contains(obj);
        }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public override void Clear()
        {
            dataQueue.Clear();
            usedList.Clear();
            GameObject.Destroy(parentObj);
            parentObj = null;
        }
    }

    #endregion

    #region 数据结构类和逻辑类对象池

    public abstract class BasePoolObject
    {
        public abstract object GetObj();

        public abstract void ReleaseObj(object obj);

        public abstract void Clear();
    }

    public interface IClassPoolObject
    {
        /// <summary>
        /// 重置对象数据
        /// </summary>
        void Rest();
    }

    /// <summary>
    /// 用于存储数据结构类和逻辑类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ClassPoolObject<T> : BasePoolObject where T : class, new()
    {
        private Queue<T> classQueue = new Queue<T>();

        public int Count => classQueue.Count;

        public override object GetObj()
        {
            if (classQueue.Count > 0)
            {
                return classQueue.Dequeue();
            }
            return new T();
        }

        public override void ReleaseObj(object obj)
        {
            classQueue.Enqueue(obj as T);
        }

        public override void Clear()
        {
            classQueue.Clear();
            classQueue = null;
        }
    }

    #endregion

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