using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.ObjectPoolManager
{
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
}