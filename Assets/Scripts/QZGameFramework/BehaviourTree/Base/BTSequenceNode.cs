using System;

/// <summary>
/// 1.顺序器(And-&):按照设计顺序执行子节点行为直到所有子节点全部完成或者到某一个失败为止
/// </summary>
public class BTSequenceNode : BTControlNode
{
    public BTSequenceNode()
    {
        AddOnEnterEvent(OnEnter);
        AddOnUpdateEvent(OnUpdate);
        AddOnExitEvent(OnExit);
    }

    private void OnEnter()
    {
        nowIndex = 0;
    }

    private E_BT_StateType OnUpdate()
    {
        E_BT_StateType childState = E_BT_StateType.Waiting;
        if (nowIndex < childs.Count)
        {
            childState = childs[nowIndex].Tick();
            switch (childState)
            {
                case E_BT_StateType.Success:
                    {
                        ++nowIndex;
                        if (nowIndex == childs.Count)
                        {
                            nowIndex = 0;
                            state = E_BT_StateType.Success;
                            return E_BT_StateType.Success;
                        }
                        break;
                    }

                case E_BT_StateType.Failure:
                    nowIndex = 0;
                    state = E_BT_StateType.Failure;
                    return E_BT_StateType.Failure;

                default:
                    state = E_BT_StateType.Running;
                    return childState;
            }
        }

        // 否则当前结点还处于执行状态
        state = E_BT_StateType.Running;
        return E_BT_StateType.Running;
    }

    private void OnExit(E_BT_StateType type)
    {
    }
}