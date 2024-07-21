using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.ObjectPoolManager
{
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
}