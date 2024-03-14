using System;

/// <summary>
/// 2.选择器(Or-|):依次执行每个子行为直到某个子节点已经成功执行或返回RUNNING状态
/// </summary>
public class BTSelectNode : BTControlNode
{
    public BTSelectNode()
    {
        AddOnEnterEvent(OnEnter);
        AddOnUpdateEvent(OnUpdate);
        AddOnExitEvent(OnExit);
    }

    public virtual void OnEnter()
    {
        nowIndex = 0;
    }

    public E_BT_StateType OnUpdate()
    {
        var childState = E_BT_StateType.Waiting;
        // 遍历子结点，以此执行子结点逻辑
        if (nowIndex < childs.Count)
        {
            childState = childs[nowIndex].Tick();
            switch (childState)
            {
                case E_BT_StateType.Success:
                    nowIndex = 0;
                    state = E_BT_StateType.Success;
                    return E_BT_StateType.Success;

                case E_BT_StateType.Failure:
                    {
                        ++nowIndex;
                        if (nowIndex == childs.Count)
                        {
                            nowIndex = 0;
                            state = E_BT_StateType.Failure;
                            return E_BT_StateType.Failure;
                        }
                        break;
                    }

                default:
                    state = E_BT_StateType.Running;
                    return childState;
            }
        }

        state = E_BT_StateType.Running;
        return E_BT_StateType.Running;
    }

    private void OnExit(E_BT_StateType type)
    {
    }
}