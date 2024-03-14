using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeTreeView : GraphView
{
    private static int index = 1;
    private string DialogueTree_Save_Path = "Assets/Game Data/ScriptableObject/Dialogue/";
    private DialogueNodeTree nodeTree;
    public Action<NodeView> OnNodeSelected;
    private VisualElement titleDiv;

    // 暴露当前组件在 UIBuilder 窗口中
    public new class UxmlFactory : UxmlFactory<NodeTreeView, GraphView.UxmlTraits>
    { }

    public NodeTreeView()
    {
        this.Insert(0, new GridBackground()); // 添加背景
        this.AddManipulator(new ContentZoomer()); // 视图缩放
        this.AddManipulator(new ContentDragger()); // 视图拖拽
        this.AddManipulator(new SelectionDragger()); // 视图中物体拖拽
        this.AddManipulator(new RectangleSelector()); // 视图中框选功能

        // 引入 uss 样式
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogueTool/UI Toolkit/UI/NodeTreeView.uss"));
        // 添加撤销重做事件添加
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    /// <summary>
    /// 创建右键菜单
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (nodeTree == null)
        {
            evt.menu.AppendAction("Create New DialogueTree", _ =>
            CreateNewDialogueTree());
        }
        else
        {
            // 获取所有继承了抽象类 BaseNode 的具体实类
            var types = TypeCache.GetTypesDerivedFrom<BaseNode>();
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                {
                    // 获取当前鼠标位置
                    Vector2 localMousePos = this.ChangeCoordinatesTo(this, evt.localMousePosition);
                    // 遍历 实类结点类型 在右键菜单中添加对应的菜单名
                    evt.menu.AppendAction($"{type.Name}", action => CreateNode(type, localMousePos));
                }
            }
        }

        base.BuildContextualMenu(evt);
    }

    /// <summary>
    /// 创建新的 DialogueTree
    /// </summary>
    private void CreateNewDialogueTree()
    {
        if (!Directory.Exists(DialogueTree_Save_Path))
        {
            Directory.CreateDirectory(DialogueTree_Save_Path);
        }

        nodeTree = ScriptableObject.CreateInstance<DialogueNodeTree>();

        string filePath = DialogueTree_Save_Path + "New DialogueNodeTree.asset";
        if (File.Exists(filePath))
        {
            index = 1;
            filePath = DialogueTree_Save_Path + $"New DialogueNodeTree {index}.asset";
            while (File.Exists(filePath))
            {
                filePath = DialogueTree_Save_Path + $"New DialogueNodeTree {++index}.asset";
            }
        }
        else
        {
            index = 1;
        }

        AssetDatabase.CreateAsset(nodeTree, filePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(nodeTree);
        PopulateView(nodeTree);
    }

    public void SetRootNode(BaseNode rootNode)
    {
        nodeTree.rootNode = rootNode;
        nodeTree.runningNode = rootNode;

        BaseNode originalRootNode = nodeTree.allNodes.Find(node => node.isRootNode == true && node != rootNode);
        if (originalRootNode != null)
        {
            originalRootNode.isRootNode = false;
        }

        PopulateView(nodeTree);
    }

    /// <summary>
    /// 撤销重做
    /// </summary>
    private void OnUndoRedo()
    {
        PopulateView(nodeTree);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 根据类型创建结点视图
    /// </summary>
    /// <param name="type"></param>
    private void CreateNode(Type type, Vector2 mousePos)
    {
        if (nodeTree == null)
        {
            Debug.LogWarning("No NodeTree Seleted");
            return;
        }

        // 创建一个结点
        BaseNode node = nodeTree.CreateNode(type);

        // 设置节点位置
        node.position = mousePos;

        if (!nodeTree.allNodes.Exists(node => node.isRootNode == true))
        {
            Debug.LogError("Must Create A Root Node");
        }

        if (node != null)
        {
            // 并创建这个结点的视图
            CreateNodeView(node);
        }
    }

    /// <summary>
    /// 创建 BaseNode 结点视图
    /// </summary>
    /// <param name="node"></param>
    private void CreateNodeView(BaseNode node)
    {
        NodeView nodeView = new NodeView(node, this); // 创建一个视图结点
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView); // 添加到 nodeTree 视图中添加这个结点视图
    }

    /// <summary>
    /// DialogueNodeTree 视图绘制函数
    /// </summary>
    /// <param name="nodeTree"></param>
    public void PopulateView(DialogueNodeTree nodeTree)
    {
        this.nodeTree = nodeTree;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements); // 清除视图渲染内容
        // 视图变更委托
        graphViewChanged += OnGraphViewChanged;

        titleDiv = this.parent.Q<VisualElement>("TitleDiv");
        var nodeTreeName = titleDiv.Q<Label>("nodeTreeName");

        // 如果当前选择的资源不是 DialogueNodeTree 则不继续向下执行
        if (this.nodeTree == null)
        {
            nodeTreeName.text = "No DialogueTree Selected";
            return;
        }

        nodeTreeName.text = nodeTree.name;

        // 重绘NodeTree视图
        // 重绘所有结点
        nodeTree.allNodes.ForEach(node => CreateNodeView(node));
        nodeTree.allNodes.ForEach(node =>
        {
            // 得到每一个结点的孩子结点列表
            var children = nodeTree.GetChildren(node);

            if (children != null)
            {
                // 根据他们的GUID找到对应绘制出来的视图
                // 并根据他们的父子关系进行绘制连接线
                children.ForEach(child =>
                {
                    NodeView parentView = FindNodeView(node);
                    NodeView childView = FindNodeView(child);
                    // 添加连线
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    this.AddElement(edge);
                });
            }
        });
    }

    public void RepaintNodeTreeView()
    {
        PopulateView(nodeTree);
    }

    /// <summary>
    /// 通过结点的GUID查找到对应的结点视图
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private NodeView FindNodeView(BaseNode node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        // 限制端口连接对象 不能为同一类型的端口 同一节点端口之间不能连接
        return ports.ToList().Where(
            endport =>
            endport.direction != startPort.direction
            && endport.node != startPort.node
            && !endport.node.outputContainer.Contains((NodeView)startPort.node)).ToList();
    }

    /// <summary>
    /// DialogueNodeTree 中的元素发生改变时 执行的函数
    /// </summary>
    /// <param name="graphViewChange"></param>
    /// <returns></returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null)
        {
            // 如果视图中有元素被删除
            graphViewChange.elementsToRemove.ForEach(elem =>
            {
                NodeView nodeView = elem as NodeView;
                // 判断该删除的元素是否是 nodeView 元素
                if (nodeView != null)
                {
                    nodeTree.DeleteNode(nodeView.node);
                }

                // 如果删除的元素是线
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    // 创建父子结点视图 输出结点为父 输入结点为子
                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;
                    nodeTree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        // 如果是新增结点之间的连接线关系 进行的逻辑处理
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                // 创建父子结点视图 输出结点为父 输入结点为子
                NodeView parentView = edge.output.node as NodeView;
                NodeView childView = edge.input.node as NodeView;
                nodeTree.AddChild(parentView.node, childView.node);
            });
        }

        return graphViewChange;
    }

    /// <summary>
    /// 循环所有的结点 对其状态进行更新 以便更改 uss 样式
    /// </summary>
    public void UpdateNodeState()
    {
        nodes.ForEach(node =>
        {
            NodeView nodeView = node as NodeView;
            nodeView.SetNodeState();
        });
    }
}