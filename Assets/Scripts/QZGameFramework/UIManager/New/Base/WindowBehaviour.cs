using System;
using UnityEngine;

namespace QZGameFramework.AutoUIManager
{
    /// <summary>
    /// UI 的生命周期基类
    /// </summary>
    public class WindowBehaviour
    {
        /// <summary>
        /// 当前窗口物体 GameObject
        /// </summary>
        public GameObject gameObject { get; set; }

        /// <summary>
        /// 代表自己 Transform
        /// </summary>
        public Transform transform { get; set; }

        /// <summary>
        /// 当前窗口的 Canvas
        /// </summary>
        public Canvas Canvas { get; set; }

        /// <summary>
        /// 当前窗口的名字
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 当前窗口是否可见
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// 是否是通过堆栈系统弹出的弹窗
        /// </summary>
        public bool PopStack { get; set; }

        /// <summary>
        /// 弹栈事件监听
        /// </summary>
        public Action<WindowBase> PopStackListener { get; set; }

        /// <summary>
        /// 只会在物体创建时执行一次 ，与Mono Awake调用时机和次数保持一致
        /// </summary>
        public virtual void OnAwake()
        { }

        /// <summary>
        /// 在物体显示时执行一次，与Mono OnEnable一致
        /// </summary>
        public virtual void OnShow()
        { }

        /// <summary>
        /// 更新函数 与Mono Update一致
        /// </summary>
        public virtual void OnUpdate()
        { }

        /// <summary>
        /// 在物体隐藏时执行一次，与Mono OnDisable 一致
        /// </summary>
        public virtual void OnHide()
        { }

        /// <summary>
        /// 在当前界面被销毁时调用一次
        /// </summary>
        public virtual void OnDestroy()
        { }

        /// <summary>
        /// 设置界面的可见性
        /// </summary>
        /// <param name="isVisble">是否可见</param>
        public virtual void SetVisible(bool isVisble)
        { }
    }
}