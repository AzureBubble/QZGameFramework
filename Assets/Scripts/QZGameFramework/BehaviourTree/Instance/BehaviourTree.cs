using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = ("BehaviourTree"), fileName = ("New BehaviourTree"))]
public class BehaviourTree : ScriptableObject
{
    /// <summary>
    /// 行为树持有者实体对象
    /// </summary>
    public System.Object Entity { private set; get; }

    /// <summary>
    /// 黑板 共享数据信息
    /// </summary>
    private Dictionary<string, System.Object> blackboardDict = new Dictionary<string, System.Object>(50);

    public BTBaseNode rootNode; // 行为树根节点
    public BTBaseNode runningNode;
    public List<BTBaseNode> allNodes = new List<BTBaseNode>(); // 行为树所有节点数据

    /// <summary>
    /// 行为树启动事件
    /// </summary>
    /// <param name="entity">行为树拥有者实体对象</param>
    public virtual void OnTreeStart(System.Object entity)
    {
        this.Entity = entity;
        runningNode = rootNode;
    }

    /// <summary>
    /// 行为树帧更新事件
    /// </summary>
    public virtual void OnUpdate()
    {
        rootNode?.Tick();
    }

    /// <summary>
    /// 行为树终止
    /// </summary>
    public virtual void OnTreeStop()
    {
        foreach (BTBaseNode item in allNodes)
        {
            item.Abort();
        }
    }

    /// <summary>
    /// 异步初始化结点数据
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitNode()
    {
        foreach (BTBaseNode item in allNodes)
        {
            item.OnCreate(this.Entity, this);
            yield return null;
        }
    }

    #region 设置黑板

    /// <summary>
    /// 设置黑板数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public void SetBlackboardValue(string key, System.Object value)
    {
        if (blackboardDict.ContainsKey(key))
        {
            blackboardDict[key] = value;
        }
        else
        {
            blackboardDict.Add(key, value);
        }
    }

    /// <summary>
    /// 获取黑板数据
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public System.Object GetBlackboardValue(string key)
    {
        if (blackboardDict.ContainsKey(key))
        {
            return blackboardDict[key];
        }
        else
        {
            Debug.Log($"Not found blackboard value : {key}");
            return null;
        }
    }

    #endregion

    #region 编辑器模式功能

#if UNITY_EDITOR

    /// <summary>
    /// 创建结点的方法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public BTBaseNode CreateNode(Type type)
    {
        // 撤销前 记录操作
        Undo.RecordObject(this, "BehaviourTree (CreateNode)");
        BTBaseNode node = ScriptableObject.CreateInstance(type) as BTBaseNode;
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
        Undo.RegisterCreatedObjectUndo(node, "BehaviourTree (CreateNode)");
        AssetDatabase.SaveAssets();
        return node;
    }

    /// <summary>
    /// 删除结点
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public BTBaseNode DeleteNode(BTBaseNode node)
    {
        // 撤销前 记录操作
        Undo.RecordObject(this, "BehaviourTree (DeleteNode)");
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
    public void RemoveChild(BTBaseNode parent, BTBaseNode child)
    {
        // 撤销前 记录操作
        Undo.RecordObject(parent, "BehaviourTree (RemoveChild)");

        if (parent is BTControlNode)
        {
            (parent as BTControlNode).childs.Remove(child);
        }

        EditorUtility.SetDirty(parent);
    }

    /// <summary>
    /// 添加父子关系
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="child"></param>
    public void AddChild(BTBaseNode parent, BTBaseNode child)
    {
        // 撤销前 记录操作
        Undo.RecordObject(parent, "BehaviourTree (AddChild)");

        if (parent is BTControlNode)
        {
            (parent as BTControlNode).childs.Add(child);
        }

        EditorUtility.SetDirty(parent);
    }

    public List<BTBaseNode> GetChildren(BTBaseNode parent)
    {
        if (parent is BTControlNode)
        {
            BTControlNode tempParent = parent as BTControlNode;
            if (tempParent.childs.Count > 0)
            {
                return tempParent.childs;
            }
        }

        return null;
    }

#endif

    #endregion
}