using System;

public class BTParallelNode : BTControlNode
{
    public int successPolicyCount;
    public int failurePolicyCount;
    protected int successCounter;
    protected int failureCounter;

    public BTParallelNode(int SuccessPolicyCount, int FailurePolicyCount)
    {
        failurePolicyCount = FailurePolicyCount;
        successPolicyCount = SuccessPolicyCount;

        AddOnEnterEvent(OnEnter);
        AddOnUpdateEvent(OnUpdate);
        AddOnExitEvent(OnExit);
    }

    private void OnEnter()
    {
    }

    public virtual E_BT_StateType OnUpdate()
    {
        for (int i = 0; i < childs.Count; i++)
        {
            if (!childs[i].IsExit())
            {
                childs[i].Tick();
            }

            //优先处理失败比较保险
            if (childs[i].GetStatus() == E_BT_StateType.Failure)
            {
                ++failureCounter;
                if (failureCounter >= failurePolicyCount)
                {
                    return E_BT_StateType.Failure;
                }
            }

            if (childs[i].GetStatus() == E_BT_StateType.Success)
            {
                ++successCounter;
                if (successCounter >= successPolicyCount)
                {
                    return E_BT_StateType.Success;
                }
            }
        }

        return E_BT_StateType.Running;
    }

    public virtual void OnExit(E_BT_StateType status) //终止所有运行中的子节点
    {
        for (int i = 0; i < childs.Count; i++)
        {
            BTBaseNode node = childs[i];
            if (node.IsRunning())
            {
                node.Abort();
            }
        }
    }
}