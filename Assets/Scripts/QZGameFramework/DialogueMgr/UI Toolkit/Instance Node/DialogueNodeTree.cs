using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("DialogueTree/DialogueNodeTree"), fileName = ("New DialogueNodeTree"))]
public class DialogueNodeTree : ScriptableObject
{
    public BaseNode rootNode;
    public BaseNode runningNode;
    public E_NodeState treeState = E_NodeState.Waiting;
    public List<BaseNode> allNodes = new List<BaseNode>();

    public virtual void OnUpdate()
    {
        if (treeState == E_NodeState.Running && runningNode.state == E_NodeState.Running)
        {
            runningNode = runningNode.OnUpdate();
        }
    }

    public virtual void OnTreeStart()
    {
        runningNode = rootNode;
        treeState = E_NodeState.Running;
        runningNode.state = E_NodeState.Running;
    }

    public virtual void OnTreeStop()
    {
        treeState = E_NodeState.Waiting;
        runningNode.state = E_NodeState.Waiting;
    }

#if UNITY_EDITOR

    /// <summary>
    /// 创建结点的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BaseNode CreateNode(Type type)
    {
        // 撤销前 记录操作
        Undo.RecordObject(this, "NodeTree (CreateNode)");
        BaseNode node = ScriptableObject.CreateInstance(type) as BaseNode;
        node.name = type.Name; // 对节点重命名
        node.guid = GUID.Generate().ToString(); // 获取唯一标识
        if (!Application.isPlaying)
        {
            AssetDatabase.AddObjectToAsset(node, this);
            allNodes.Add(node); // 加入结点列表中存储
        }
        else
        {
            Debug.LogWarning("Please Edit In The Editor State.Otherwise It Will Not Be Saved");
        }
        // 把新增的结点状态也添加到 Undo中
        Undo.RegisterCreatedObjectUndo(node, "NodeTree (CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    /// <summary>
    /// 删除结点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public BaseNode DeleteNode(BaseNode node)
    {
        // 撤销前 记录操作
        Undo.RecordObject(this, "NodeTree (DeleteNode)");
        if (allNodes.Contains(node))
        {
            allNodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
        }

        AssetDatabase.SaveAssets();
        return node;
    }

    /// <summary>
    /// 删除结点的父子关系
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    public void RemoveChild(BaseNode parent, BaseNode child)
    {
        // 撤销前 记录操作
        Undo.RecordObject(parent, "NodeTree (RemoveChild)");

        if (parent.GetType() == typeof(SequenceNode))
        {
            (parent as SequenceNode).child = null;
        }
        else if (parent.GetType() == typeof(SelectNode))
        {
            (parent as SelectNode).childs.Remove(child);
        }

        EditorUtility.SetDirty(parent);
    }

    /// <summary>
    /// 添加父子关系
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    public void AddChild(BaseNode parent, BaseNode child)
    {
        // 撤销前 记录操作
        Undo.RecordObject(parent, "NodeTree (AddChild)");

        if (parent.GetType() == typeof(SequenceNode))
        {
            (parent as SequenceNode).child = child;
        }
        else if (parent.GetType() == typeof(SelectNode))
        {
            (parent as SelectNode).childs.Add(child);
        }

        EditorUtility.SetDirty(parent);
    }

    public List<BaseNode> GetChildren(BaseNode parent)
    {
        if (parent.GetType() == typeof(SequenceNode))
        {
            SequenceNode tempParent = parent as SequenceNode;
            if (tempParent.child != null)
            {
                return new List<BaseNode> { tempParent.child };
            }
        }
        else if (parent.GetType() == typeof(SelectNode))
        {
            SelectNode tempParent = parent as SelectNode;
            if (tempParent.childs.Count > 0)
            {
                return (parent as SelectNode).childs;
            }
        }

        return null;
    }

#endif
}