using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.ObjectPoolManager
{
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
            PoolObjCountAttribute poolObjAttr = null;
            foreach (var script in scripts)
            {
                poolObjAttr = (PoolObjCountAttribute)System.Attribute.GetCustomAttribute(script.GetType(), typeof(PoolObjCountAttribute));
                if (poolObjAttr != null)
                {
                    maxNum = poolObjAttr.MaxNum;
                    break;
                }
            }

            if (poolObjAttr == null)
            {
                Debug.LogWarning($"Object pool objects must have PoolObjCountAttribute to Set MaxNum of GameObject, Otherwise, the default value: {this.maxNum} will be used. Please check GameObject: {obj.name}.");
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
}