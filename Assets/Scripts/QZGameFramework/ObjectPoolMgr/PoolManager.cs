using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace QZGameFramework.ObjectPoolManager
{
    /// <summary>
    /// Unity 的 ObjectPool 对象池管理器
    /// </summary>
    public class PoolManager : Singleton<PoolManager>
    {
        /// <summary>
        /// 对象池总容器 键：某一类物品名字，值：对应该类物体的对象池容器
        /// </summary>
        private Dictionary<string, ObjectPool<GameObject>> poolDic = new Dictionary<string, ObjectPool<GameObject>>();

        private List<GameObject> parentObj = new List<GameObject>();

        /// <summary>
        /// 创建对象池
        /// </summary>
        /// <param name="objName">游戏物体名字</param>
        private void CreatePool(string objName, string path = "Prefabs/")
        {
            // 缓存池游戏对象
            Transform parent = new GameObject(objName + " Pool").transform;
            parentObj.Add(parent.gameObject);
            // 设置为缓存池管理器子对象
            //parent.SetParent(this.transform, false);
            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
                // 游戏对象创建时候的方法
                () =>
                            {
                                GameObject obj = null;
                                obj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>(path + objName));
                                obj.name = objName;
                                obj.transform.SetParent(null, false);
                                return obj;
                            },
      e =>
                            {
                                // 取对象时候的方法
                                // e:对象池中的每一个元素
                                e.SetActive(true);
                                e.transform.SetParent(null, false);
                            },
      e =>
                            {
                                // 对象放回对象池时候的方法
                                // e:对象池中的每一个元素
                                e.SetActive(false);
                                e.transform.SetParent(parent, false);
                            },
    e =>
                            {
                                // 对象销毁时候的方法
                                // e:对象池中的每一个元素
                                GameObject.Destroy(e.gameObject);
                            }); // 是否自动扩展， 默认容量：10，最大容量：20

            // 把对象池加入到List中
            poolDic.Add(objName, newPool);
        }

        /// <summary>
        /// 从总容器中找到对应游戏物体的对象池取出
        /// </summary>
        /// <param name="objName">想要取出物体的名字</param>
        /// <returns></returns>
        public GameObject GetObj(string objName)
        {
            GameObject obj = null;
            if (!poolDic.ContainsKey(objName))
            {
                CreatePool(objName);
            }
            obj = poolDic[objName].Get();
            return obj;
        }

        /// <summary>
        /// 返还游戏物体到对象池
        /// </summary>
        /// <param name="objName">游戏物体名字</param>
        /// <param name="obj">游戏物体</param>
        public void RealeaseObj(string objName, GameObject obj)
        {
            if (poolDic.ContainsKey(objName))
            {
                poolDic[objName].Release(obj);
            }
        }

        public void Clear()
        {
            foreach (var pool in poolDic.Values)
            {
                pool.Clear();
            }
            poolDic.Clear();
            foreach (var obj in parentObj)
            {
                GameObject.Destroy(obj);
            }
            parentObj.Clear();
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            Clear();
            base.Dispose();
        }
    }
}