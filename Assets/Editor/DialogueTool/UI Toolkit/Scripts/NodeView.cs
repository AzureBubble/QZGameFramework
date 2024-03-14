using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView : UnityEditor.Experimental.GraphView.Node
{
    public BaseNode node; // 结点数据
    public Port input; // 输入端口
    public Port output; // 输出端口
    public Action<NodeView> OnNodeSelected;
    private NodeTreeView nodeTreeView;

    public NodeView(BaseNode node, NodeTreeView nodeTreeView) : base("Assets/Editor/DialogueTool/UI Toolkit/UI/NodeView.uxml")
    {
        this.nodeTreeView = nodeTreeView;
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;
        // 构造时就创建两个端口
        CreateInputPorts();
        CreateOutputPorts();
        SetNodeViewUssClass();

        Label speakerName = this.Q<Label>("speakerName");
        speakerName.bindingPath = "speakerName";
        speakerName.Bind(new SerializedObject(node));

        this.tooltip = this.node.description;
    }

    private void SetNodeViewUssClass()
    {
        VisualElement root = this.Q<VisualElement>("root");
        VisualElement input = this.Q<VisualElement>("input");
        root.style.display = DisplayStyle.None;
        input.style.display = DisplayStyle.None;
        if (node.isRootNode)
        {
            AddToClassList("root");
            root.style.display = DisplayStyle.Flex;
        }
        else if (node is SequenceNode)
        {
            AddToClassList("sequence");
            input.style.display = DisplayStyle.Flex;
        }
        else if (node is SelectNode)
        {
            AddToClassList("select");
            input.style.display = DisplayStyle.Flex;
        }
    }

    /// <summary>
    /// 创建输入端口
    /// </summary>
    private void CreateInputPorts()
    {
        // 将节点入口设置为
        // 端口链接方向 竖向Orientation.Vertical  横向Orientation.Horizontal
        // 端口类型 Direction.Input/Output
        // 端口可链接数量 Port.Capacity.Single/Multi 单个/多个
        // 端口数据类型 typeof(bool)
        // 默认所有节点为多入口类型
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
        //if (node is SequenceNode)
        //{
        //    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        //}

        if (node.isRootNode)
        {
            input = null;
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            // 加入输入端口容器中
            inputContainer.Add(input);
        }
    }

    /// <summary>
    /// 创建输出端口
    /// </summary>
    private void CreateOutputPorts()
    {
        output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
        if (node is SequenceNode)
        {
            output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
        }
        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            // 加入输出端口容器中
            outputContainer.Add(output);
        }
    }

    /// <summary>
    /// 结点视图被选中时调用
    /// </summary>
    public override void OnSelected()
    {
        base.OnSelected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(this);

            EditorGUIUtility.PingObject(this.node);
        }
    }

    public override void OnUnselected()
    {
        base.OnUnselected();
        if (OnNodeSelected != null)
        {
            OnNodeSelected.Invoke(null);
        }
    }

    /// <summary>
    /// 设置视图结点的位置
    /// </summary>
    /// <param name="newPos"></param>
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        // 撤回记录
        Undo.RecordObject(node, "Node Tree(Set Position)");

        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    /// <summary>
    /// 运行时 根据结点状态改变 结点视图的uss样式
    /// </summary>
    public void SetNodeState()
    {
        RemoveFromClassList("running");

        if (Application.isPlaying)
        {
            switch (node.state)
            {
                case E_NodeState.Running:
                    if (node.isExcute)
                    {
                        AddToClassList("running");

                        OnNodeSelected.Invoke(this);
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// 设置根节点
    /// </summary>
    public void SetRootNode(bool rootNode)
    {
        this.node.isRootNode = rootNode;

        if (rootNode)
        {
            if (input.connections != null)
            {
                foreach (Edge edge in input.connections)
                {
                    var connectedNode = edge.output.node as NodeView;
                    if (connectedNode.node.GetType() == typeof(SequenceNode))
                    {
                        (connectedNode.node as SequenceNode).child = null;
                    }
                    else if (connectedNode.node.GetType() == typeof(SelectNode))
                    {
                        (connectedNode.node as SelectNode).childs.Remove(this.node);
                    }
                }
            }
            nodeTreeView.SetRootNode(this.node);
        }
        else
        {
            nodeTreeView.SetRootNode(null);
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (node != null && !node.isRootNode)
        {
            evt.menu.AppendAction("Set RootNode", _ =>
            {
                SetRootNode(true);
            });
        }
        else if (node != null && node.isRootNode)
        {
            evt.menu.AppendAction("Set NormalNode", _ =>
            {
                SetRootNode(false);
            });
        }

        base.BuildContextualMenu(evt);
    }
}