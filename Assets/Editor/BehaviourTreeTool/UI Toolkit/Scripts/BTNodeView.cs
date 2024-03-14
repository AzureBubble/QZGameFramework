using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class BTNodeView : UnityEditor.Experimental.GraphView.Node
{
    public BTBaseNode node; // 结点数据
    public Port input; // 输入端口
    public Port output; // 输出端口
    public Action<BTNodeView> OnNodeSelected;
    private BehaviourTreeView behaviourTreeView;

    public BTNodeView(BTBaseNode node, BehaviourTreeView behaviourTreeView) : base("Assets/Editor/BehaviourTreeTool/UI Toolkit/UI/BTNodeView.uxml")
    {
        this.behaviourTreeView = behaviourTreeView;
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;
        style.left = node.position.x;
        style.top = node.position.y;
        // 构造时就创建两个端口
        CreateInputPorts();
        CreateOutputPorts();
        SetNodeViewUssClass();

        Label description = this.Q<Label>("description");
        description.bindingPath = "description";
        description.Bind(new SerializedObject(node));

        //this.tooltip = this.node.description;
    }

    private void SetNodeViewUssClass()
    {
        VisualElement root = this.Q<VisualElement>("root");
        VisualElement input = this.Q<VisualElement>("input");
        VisualElement output = this.Q<VisualElement>("output");
        VisualElement endNode = this.Q<VisualElement>("endNode");

        root.style.display = DisplayStyle.None;
        input.style.display = DisplayStyle.None;
        output.style.display = DisplayStyle.None;
        endNode.style.display = DisplayStyle.None;
        if (node.isRootNode)
        {
            AddToClassList("root");
            root.style.display = DisplayStyle.Flex;
            output.style.display = DisplayStyle.Flex;
        }
        else if (node is BTSequenceNode)
        {
            AddToClassList("sequence");
            input.style.display = DisplayStyle.Flex;
            output.style.display = DisplayStyle.Flex;
        }
        else if (node is BTSelectNode)
        {
            AddToClassList("select");
            input.style.display = DisplayStyle.Flex;
            output.style.display = DisplayStyle.Flex;
        }
        else if (node is BTActionNode)
        {
            AddToClassList("action");
            input.style.display = DisplayStyle.Flex;
            endNode.style.display = DisplayStyle.Flex;
        }
        else if (node is BTConditionNode)
        {
            AddToClassList("condition");
            input.style.display = DisplayStyle.Flex;
            endNode.style.display = DisplayStyle.Flex;
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
        input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        //if (node is SequenceNode)
        //{
        //    input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        //}

        if (node.GetType().IsSubclassOf(typeof(BTActionNode))
            || node.GetType().IsSubclassOf(typeof(BTDecoratorNode)))
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
        }

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
        if (node is BTActionNode || node is BTConditionNode)
        {
            output = null;
        }

        if (node.GetType().IsSubclassOf(typeof(BTDecoratorNode)))
        {
            input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
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
        }
        EditorGUIUtility.PingObject(this.node);
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

        BTNodeView parent = null;
        // 根据结点的坐标改变结点在列表中的位置
        if (input != null && input.connected)
        {
            parent = (BTNodeView)input.connections.First().output.node;
        }

        if (parent != null)
        {
            if (parent.node.GetType().IsSubclassOf(typeof(BTControlNode)))
            {
                BTControlNode tempParent = (BTControlNode)parent.node;

                tempParent.childs.Sort((a, b) =>
                {
                    return a.position.x > b.position.x ? 1 : -1;
                });
            }
        }

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
                case E_BT_StateType.Success:
                    //AddToClassList("success");
                    break;

                case E_BT_StateType.Failure:
                    //AddToClassList("failure");
                    break;

                case E_BT_StateType.Running:
                    AddToClassList("running");
                    OnNodeSelected.Invoke(this);
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
                    var connectedNode = edge.output.node as BTNodeView;
                    if (connectedNode.node is BTControlNode)
                    {
                        (connectedNode.node as BTControlNode).childs.Remove(this.node);
                    }
                }
            }

            behaviourTreeView.SetRootNode(this.node);
        }
        else
        {
            behaviourTreeView.SetRootNode(null);
        }
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (node is BTControlNode)
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
        }

        base.BuildContextualMenu(evt);
    }
}