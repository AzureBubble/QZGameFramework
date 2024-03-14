public class BTMonitorNode : BTParallelNode
{
    public BTMonitorNode(int SuccessPolicyCount, int FailurePolicyCount) : base(SuccessPolicyCount, FailurePolicyCount)
    {
    }

    public void AddCondition(BTBaseNode condition)
    {
        childs.Insert(0, condition);
    }

    public void AddAction(BTBaseNode action)
    {
        childs.Add(action);
    }
}