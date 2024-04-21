using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.ObjectPoolManager
{
    /// <summary>
    /// 缓存池容器对象
    /// </summary>
    public class PoolData
    {
        private GameObject parentObj; // 缓存池结点
        private Stack<GameObject> dataStack = new Stack<GameObject>(); // 没有使用的对象池
        private List<GameObject> usedList = new List<GameObject>(); // 使用中的对象池
        private int maxNum = 30;
        public int Count => dataStack.Count;
        public int UseCount => usedList.Count;
        public bool NeedCreate => UseCount < maxNum;

        /// <summary>
        /// 初始化对象池最大容量
        /// </summary>
        /// <param name="maxNum">最大容量</param>
        public void InitObjectPoolMaxNum(int maxNum)
        {
            if (this.maxNum == maxNum) return;
            if (this.maxNum > maxNum)
            {
                while (Count > 0 && Count + UseCount > maxNum)
                {
                    GameObject.Destroy(dataStack.Pop());
                }
                while (UseCount > 0 && Count + UseCount > maxNum)
                {
                    GameObject obj = usedList[^1];
                    GameObject.Destroy(obj);
                    usedList.Remove(obj);
                }
            }
            this.maxNum = maxNum;
        }

        /// <summary>
        /// 构造函数 创建缓存池管理者对象结点，预制体缓存池结点
        /// </summary>
        /// <param name="obj">缓存池物体</param>
        public PoolData(GameObject obj)
        {
            // 创建父节点物体
            this.parentObj = new GameObject(obj.name + " Pool");
            GameObject.DontDestroyOnLoad(this.parentObj);
            // 把父节点物体作为缓存池管理对象的子节点
            //this.parentObj.transform.SetParent(poolMgr.transform, false);

            // 把物体压入已使用记录中
            PushUsedList(obj);
        }

        public PoolData(string name, int maxNun)
        {
            // 创建父节点物体
            this.parentObj = new GameObject(name + " Pool");
            GameObject.DontDestroyOnLoad(this.parentObj);
            // 把父节点物体作为缓存池管理对象的子节点
            //this.parentObj.transform.SetParent(poolMgr.transform, false);
            this.maxNum = maxNun;
        }

        /// <summary>
        /// 从缓存池中取出对象
        /// </summary>
        /// <returns></returns>
        public GameObject GetObj()
        {
            GameObject obj = null;

            if (Count > 0)
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
        public void RealeaseObj(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(parentObj.transform, false);

            // 压入栈中
            dataStack.Push(obj);
            // 删除已使用记录
            usedList.Remove(obj);
        }

        public void PushUsedList(GameObject obj)
        {
            usedList.Add(obj);
        }

        /// <summary>
        /// 清空缓存池
        /// </summary>
        public void Clear()
        {
            dataStack.Clear();
            usedList.Clear();
            GameObject.Destroy(parentObj);
            parentObj = null;
        }
    }

    /// <summary>
    /// 缓存池管理器
    /// </summary>
    public class PoolMgr : Singleton<PoolMgr>
    {
        /// <summary>
        /// 缓存池容器 键：某一类物品名字，值：游戏对象
        /// </summary>
        private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

        /// <summary>
        /// 异步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="callback">回调函数</param>
        /// <param name="path">物体存放路径</param>
        public void GetObjAsync(string name, UnityAction<GameObject> callback = null, string path = "Prefabs/")
        {
            // 判断对应的对象池是否存在
            if (!poolDic.ContainsKey(name)
                || (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
            {
                // 异步加载预制体资源
                ResourcesMgr.Instance.LoadResAsync<GameObject>(path + name, (resObj) =>
                {
                    GameObject obj = GameObject.Instantiate(resObj);
                    obj.name = name;
                    callback?.Invoke(obj);

                    if (!poolDic.ContainsKey(name))
                    {
                        poolDic.Add(name, new PoolData(resObj));
                    }
                    else
                    {
                        poolDic[name].PushUsedList(resObj);
                    }
                });
            }
            else
            {
                callback?.Invoke(poolDic[name].GetObj());
            }
        }

        /// <summary>
        /// 同步加载 从缓存池中取物体
        /// </summary>
        /// <param name="name">物体名字</param>
        /// <param name="path">物体存放路径</param>
        public GameObject GetObj(string name, string path = "Prefabs/")
        {
            GameObject obj = null;
            // 判断对应的对象池是否存在
            if (!poolDic.ContainsKey(name)
                || (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
            {
                // 加载预制体资源
                obj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>(path + name));
                obj.name = name;

                if (!poolDic.ContainsKey(name))
                {
                    poolDic.Add(name, new PoolData(obj));
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
        public void RealeaseObj(string name, GameObject obj)
        {
            if (!poolDic.ContainsKey(name))
            {
                poolDic.Add(name, new PoolData(obj));
            }
            poolDic[name].RealeaseObj(obj);
        }

        /// <summary>
        /// 初始化对象池
        /// </summary>
        /// <param name="name">对象池名称</param>
        /// <param name="maxNum">对象池最大容量</param>
        public void CreateObjectPool(string name, int maxNum)
        {
            if (!poolDic.ContainsKey(name))
            {
                poolDic.Add(name, new PoolData(name, maxNum));
            }
            else
            {
                poolDic[name].InitObjectPoolMaxNum(maxNum);
            }
        }

        /// <summary>
        /// 清空所有的缓存池
        /// </summary>
        public void Clear()
        {
            foreach (PoolData pool in poolDic.Values)
            {
                pool.Clear();
            }
            poolDic.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            Clear();
            base.Dispose();
        }
    }
}