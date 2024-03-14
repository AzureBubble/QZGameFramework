using System;
using UnityEngine;

public abstract class BTBaseNode : ScriptableObject
{
    // 结点持有者
    public System.Object Entity { get; private set; }

    public BehaviourTree EntityTree { get; private set; }

    [ReadOnly] public string guid;
    [ReadOnly] public Vector2 position;
    [ReadOnly, Tooltip("结点状态")] public E_BT_StateType state;
    [ReadOnly, Tooltip("是否是根结点")] public bool isRootNode;

    [Tooltip("结点描述信息")] public string description;

    //行为事件
    private event Action onEnter; // 结点进入事件

    private event Func<E_BT_StateType> onUpdate; // 结点帧更新事件

    private event Action<E_BT_StateType> onExit; // 结点结束事件

    /// <summary>
    /// 结点创建时候 进行结点初始化
    /// </summary>
    /// <param name="owner"></param>
    public void OnCreate(System.Object entity, BehaviourTree tree)
    {
        Entity = entity;
        EntityTree = tree;
        OnStart();
    }

    /// <summary>
    /// 创建结点时候的初始化
    /// </summary>
    public virtual void OnStart()
    {
    }

    #region 添加/删除对应时机的结点事件

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="onEnter">进入时</param>
    /// <param name="onUpdate">帧更新时</param>
    /// <param name="onExit">退出时</param>
    public void AddEvent(Action onEnter, Func<E_BT_StateType> onUpdate, Action<E_BT_StateType> onExit)
    {
        AddOnEnterEvent(onEnter);
        AddOnUpdateEvent(onUpdate);
        AddOnExitEvent(onExit);
    }

    /// <summary>
    /// 添加结点进入时的事件
    /// </summary>
    /// <param name="onEnter"></param>
    public void AddOnEnterEvent(Action onEnter)
    {
        if (onEnter != null)
            this.onEnter += onEnter;
    }

    /// <summary>
    /// 添加结点帧更新时的事件
    /// </summary>
    /// <param name="onUpdate"></param>
    public void AddOnUpdateEvent(Func<E_BT_StateType> onUpdate)
    {
        if (onUpdate != null)
            this.onUpdate += onUpdate;
    }

    /// <summary>
    /// 添加结点退出时的事件
    /// </summary>
    /// <param name="onExit"></param>
    public void AddOnExitEvent(Action<E_BT_StateType> onExit)
    {
        if (onExit != null)
            this.onExit += onExit;
    }

    /// <summary>
    /// 删除结点进入时的事件
    /// </summary>
    /// <param name="onEnter"></param>
    public void RemoveOnEnterEvent(Action onEnter)
    {
        if (onEnter != null)
            this.onEnter -= onEnter;
    }

    /// <summary>
    /// 删除结点帧更新时的事件
    /// </summary>
    /// <param name="onUpdate"></param>
    public void RemoveOnUpdateEvent(Func<E_BT_StateType> onUpdate)
    {
        if (onUpdate != null)
            this.onUpdate -= onUpdate;
    }

    /// <summary>
    /// 删除结点退出时的事件
    /// </summary>
    /// <param name="onExit"></param>
    public void RemoveOnExitEvent(Action<E_BT_StateType> onExit)
    {
        if (onExit != null)
            this.onExit -= onExit;
    }

    #endregion

    /// <summary>
    /// 结点帧更新函数
    /// </summary>
    /// <returns>当前结点的运行状态</returns>
    public E_BT_StateType Tick()
    {
        // 结点状态不为运行状态 且进入函数不为空时候执行进入函数
        if (state != E_BT_StateType.Running && onEnter != null)
            onEnter();

        // 结点帧更新事件
        state = onUpdate();

        // 结点状态不为运行状态 且结束函数不为空时候执行结束函数
        if (state != E_BT_StateType.Running && onExit != null)
            onExit(state);

        return state;
    }

    /// <summary>
    /// 重置结点状态为等待
    /// </summary>
    public virtual void ResetState()
    {
        state = E_BT_StateType.Waiting;
    }

    /// <summary>
    /// 终止执行结点
    /// </summary>
    public void Abort()
    {
        onExit(E_BT_StateType.Abort);
        state = E_BT_StateType.Abort;
    }

    /// <summary>
    /// 结点状态是否处于已经终止
    /// </summary>
    /// <returns></returns>
    public bool IsExit()
    {
        return state == E_BT_StateType.Success || state == E_BT_StateType.Failure;
    }

    /// <summary>
    /// 是否在运行
    /// </summary>
    /// <returns></returns>
    public bool IsRunning()
    {
        return state == E_BT_StateType.Running;
    }

    /// <summary>
    /// 得到当前结点的运行状态
    /// </summary>
    /// <returns></returns>
    public E_BT_StateType GetStatus()
    {
        return state;
    }
}