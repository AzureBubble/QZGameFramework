using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.ObjectPoolManager
{
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
            if (obj.TryGetComponent<RectTransform>(out RectTransform rectTransform))
            {
                obj.transform.SetParent(null, false);
            }
            else
            {
                obj.transform.SetParent(null);
            }
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
}