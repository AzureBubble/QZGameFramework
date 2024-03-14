using UnityEngine;

public abstract class BaseNode : ScriptableObject
{
    [ReadOnly] public string guid; // 唯一标识
    [ReadOnly] public Vector2 position; // 结点位置坐标
    [ReadOnly] public bool isExcute = false; // 是否执行
    [ReadOnly] public bool isRootNode;

    public E_NodeState state = E_NodeState.Waiting;// 结点初始状态为等待执行
    [TextArea] public string description;
    public Sprite headIcon; // 头像
    public string speakerName; // 名字

    public BaseNode OnUpdate()
    {
        if (!isExcute)
        {
            OnEnter();
            isExcute = true;
        }
        BaseNode node = Excute();
        if (state != E_NodeState.Running)
        {
            OnExit();
            isExcute = false;
        }
        return node;
    }

    /// <summary>
    /// 结点进入
    /// </summary>
    public abstract void OnEnter();

    /// <summary>
    /// 执行逻辑的方法
    /// </summary>
    /// <returns></returns>
    public abstract BaseNode Excute();

    /// <summary>
    /// 结点退出
    /// </summary>
    public abstract void OnExit();
}