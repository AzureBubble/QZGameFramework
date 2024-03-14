using System.Collections.Generic;

public abstract class BTControlNode : BTBaseNode
{
    // 子结点容器
    public List<BTBaseNode> childs = new List<BTBaseNode>();

    protected int nowIndex;

    /// <summary>
    /// 添加子结点
    /// </summary>
    /// <param name="nodes"></param>
    public void AddChild(params BTBaseNode[] nodes)
    {
        foreach (var node in nodes)
        {
            if (!childs.Contains(node))
            {
                childs.Add(node);
            }
        }
    }

    /// <summary>
    /// 删除子结点
    /// </summary>
    /// <param name="nodes"></param>
    public void RemoveChild(params BTBaseNode[] nodes)
    {
        foreach (var node in nodes)
        {
            if (childs.Contains(node))
            {
                childs.Remove(node);
            }
        }
    }

    public int GetChildCount()
    {
        return childs.Count;
    }
}