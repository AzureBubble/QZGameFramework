using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView : GraphView
{
    private static int index = 1;
    private string BehaviourTree_Save_Path = "Assets/Game Data/ScriptableObject/Behaviour/";
    private BehaviourTree behaviourTree;
    public Action<BTNodeView> OnNodeSelected;
    private VisualElement titleDiv;

    // 暴露当前组件在 UIBuilder 窗口中
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits>
    { }

    public BehaviourTreeView()
    {
        this.Insert(0, new GridBackground()); // 添加背景
        this.AddManipulator(new ContentZoomer()); // 视图缩放
        this.AddManipulator(new ContentDragger()); // 视图拖拽
        this.AddManipulator(new SelectionDragger()); // 视图中物体拖拽
        this.AddManipulator(new RectangleSelector()); // 视图中框选功能

        // 引入 uss 样式
        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeTool/UI Toolkit/UI/BehaviourTreeView.uss"));
        // 添加撤销重做事件添加
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    /// <summary>
    /// 创建右键菜单
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        if (behaviourTree == null)
        {
            evt.menu.AppendAction("Create New BehaviourTree", _ =>
            CreateNewBehaviourTree());
        }
        else
        {
            // 获取所有继承了抽象类 BTBaseNode 的具体实类
            var types = TypeCache.GetTypesDerivedFrom<BTBaseNode>();
            foreach (var type in types)
            {
                if (!type.IsAbstract)
                {
                    // 获取当前鼠标位置
                    Vector2 localMousePos = this.ChangeCoordinatesTo(this, evt.localMousePosition);
                    if (type.IsSubclassOf(typeof(BTSequenceNode)) || type == typeof(BTSequenceNode))
                    {
                        // 遍历 实类结点类型 在右键菜单中添加对应的菜单名
                        evt.menu.AppendAction($"Sequence/{type.Name}", action => CreateNode(type, localMousePos));
                    }
                    else if (type == typeof(BTSelectNode) || type.IsSubclassOf(typeof(BTSelectNode)))
                    {
                        // 遍历 实类结点类型 在右键菜单中添加对应的菜单名
                        evt.menu.AppendAction($"Select/{type.Name}", action => CreateNode(type, localMousePos));
                    }
                    else if (type == typeof(BTConditionNode) || type.IsSubclassOf(typeof(BTConditionNode)))
                    {
                        // 遍历 实类结点类型 在右键菜单中添加对应的菜单名
                        evt.menu.AppendAction($"Condition/{type.Name}", action => CreateNode(type, localMousePos));
                    }
                    else if (type.IsSubclassOf(typeof(BTActionNode)))
                    {
                        // 遍历 实类结点类型 在右键菜单中添加对应的菜单名
                        evt.menu.AppendAction($"Action/{type.Name}", action => CreateNode(type, localMousePos));
                    }
                }
            }
        }

        base.BuildContextualMenu(evt);
    }

    /// <summary>
    /// 创建新的 DialogueTree
    /// </summary>
    private void CreateNewBehaviourTree()
    {
        if (!Directory.Exists(BehaviourTree_Save_Path))
        {
            Directory.CreateDirectory(BehaviourTree_Save_Path);
        }

        behaviourTree = ScriptableObject.CreateInstance<BehaviourTree>();

        string filePath = BehaviourTree_Save_Path + "New BehaviourTree.asset";
        if (File.Exists(filePath))
        {
            index = 1;
            filePath = BehaviourTree_Save_Path + $"New BehaviourTree {index}.asset";
            while (File.Exists(filePath))
            {
                filePath = BehaviourTree_Save_Path + $"New BehaviourTree {++index}.asset";
            }
        }
        else
        {
            index = 1;
        }

        AssetDatabase.CreateAsset(behaviourTree, filePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorGUIUtility.PingObject(behaviourTree);
        PopulateView(behaviourTree);
    }

    public void SetRootNode(BTBaseNode rootNode)
    {
        behaviourTree.rootNode = rootNode;
        behaviourTree.runningNode = rootNode;

        BTBaseNode originalRootNode = behaviourTree.allNodes.Find(node => node.isRootNode == true && node != rootNode);
        if (originalRootNode != null)
        {
            originalRootNode.isRootNode = false;
        }

        PopulateView(behaviourTree);
    }

    /// <summary>
    /// 撤销重做
    /// </summary>
    private void OnUndoRedo()
    {
        PopulateView(behaviourTree);
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 根据类型创建结点视图
    /// </summary>
    /// <param name="type"></param>
    private void CreateNode(Type type, Vector2 mousePos)
    {
        if (behaviourTree == null)
        {
            Debug.LogWarning("No BehaviourTree Seleted");
            return;
        }

        // 创建一个结点
        BTBaseNode node = behaviourTree.CreateNode(type);

        //设置节点位置
        node.position = mousePos;

        if (!behaviourTree.allNodes.Exists(node => node.isRootNode == true))
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
    private void CreateNodeView(BTBaseNode node)
    {
        BTNodeView nodeView = new BTNodeView(node, this); // 创建一个视图结点
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView); // 添加到 nodeTree 视图中添加这个结点视图
    }

    /// <summary>
    /// DialogueNodeTree 视图绘制函数
    /// </summary>
    /// <param name="nodeTree"></param>
    public void PopulateView(BehaviourTree behaviourTree)
    {
        this.behaviourTree = behaviourTree;
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements); // 清除视图渲染内容
        // 视图变更委托
        graphViewChanged += OnGraphViewChanged;

        titleDiv = this.parent.Q<VisualElement>("TitleDiv");
        var nodeTreeName = titleDiv.Q<Label>("nodeTreeName");

        // 如果当前选择的资源不是 DialogueNodeTree 则不继续向下执行
        if (this.behaviourTree == null)
        {
            nodeTreeName.text = "No BehaviourTree Selected";
            return;
        }

        nodeTreeName.text = behaviourTree.name;

        // 重绘NodeTree视图
        // 重绘所有结点
        behaviourTree.allNodes.ForEach(node => CreateNodeView(node));
        behaviourTree.allNodes.ForEach(node =>
        {
            // 得到每一个结点的孩子结点列表
            var children = behaviourTree.GetChildren(node);

            if (children != null)
            {
                // 根据他们的GUID找到对应绘制出来的视图
                // 并根据他们的父子关系进行绘制连接线
                children.ForEach(child =>
                {
                    BTNodeView parentView = FindNodeView(node);
                    BTNodeView childView = FindNodeView(child);
                    // 添加连线
                    Edge edge = parentView.output.ConnectTo(childView.input);
                    this.AddElement(edge);
                });
            }
        });
        // 对结点进行排序
        SortList();
    }

    public void RepaintNodeTreeView()
    {
        PopulateView(behaviourTree);
    }

    /// <summary>
    /// 通过结点的GUID查找到对应的结点视图
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private BTNodeView FindNodeView(BTBaseNode node)
    {
        return GetNodeByGuid(node.guid) as BTNodeView;
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        // 限制端口连接对象 不能为同一类型的端口 同一节点端口之间不能连接
        return ports.ToList().Where(
            endport =>
            endport.direction != startPort.direction
            && endport.node != startPort.node
            && !endport.node.outputContainer.Contains((BTNodeView)startPort.node)).ToList();
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
                BTNodeView nodeView = elem as BTNodeView;
                // 判断该删除的元素是否是 nodeView 元素
                if (nodeView != null)
                {
                    behaviourTree.DeleteNode(nodeView.node);
                }

                // 如果删除的元素是线
                Edge edge = elem as Edge;
                if (edge != null)
                {
                    // 创建父子结点视图 输出结点为父 输入结点为子
                    BTNodeView parentView = edge.output.node as BTNodeView;
                    BTNodeView childView = edge.input.node as BTNodeView;
                    behaviourTree.RemoveChild(parentView.node, childView.node);
                }
            });
        }

        // 如果是新增结点之间的连接线关系 进行的逻辑处理
        if (graphViewChange.edgesToCreate != null)
        {
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                // 创建父子结点视图 输出结点为父 输入结点为子
                BTNodeView parentView = edge.output.node as BTNodeView;
                BTNodeView childView = edge.input.node as BTNodeView;
                behaviourTree.AddChild(parentView.node, childView.node);
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
            BTNodeView nodeView = node as BTNodeView;
            nodeView.SetNodeState();
        });
    }

    public void SortList()
    {
        foreach (var node in behaviourTree.allNodes)
        {
            if (node.GetType().IsSubclassOf(typeof(BTControlNode)))
            {
                (node as BTControlNode).childs.Sort((a, b) =>
                {
                    return a.position.x > b.position.x ? 1 : -1;
                });
            }
        }
    }
}

public class RightClickMenu : ScriptableObject, ISearchWindowProvider
{
    public delegate bool SelectEntryDelegate(SearchTreeEntry searchTreeEntry
        , SearchWindowContext context);

    public SelectEntryDelegate OnSelectEntryHandler;

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var entries = new List<SearchTreeEntry>();
        entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));
        entries = AddNodeType<BTControlNode>(entries, "组合节点");
        entries = AddNodeType<BTSelectNode>(entries, "条件节点");
        entries = AddNodeType<BTActionNode>(entries, "行为节点");
        return entries;
    }

    /// <summary>
    /// 通过反射获取对应的菜单数据
    /// </summary>
    public List<SearchTreeEntry> AddNodeType<T>(List<SearchTreeEntry> entries, string pathName)
    {
        entries.Add(new SearchTreeGroupEntry(new GUIContent(pathName)) { level = 1 });
        var types = TypeCache.GetTypesDerivedFrom<BTBaseNode>();
        foreach (var rootType in types)
        {
            string menuName = rootType.Name;
            //if (rootType.GetCustomAttribute(typeof(NodeLabelAttribute)) is NodeLabelAttribute nodeLabel)
            //{
            //    menuName = nodeLabel.MenuName;
            //    if (nodeLabel.MenuName == "")
            //    {
            //        menuName = rootType.Name;
            //    }
            //}
            entries.Add(new SearchTreeEntry(new GUIContent(menuName)) { level = 2, userData = rootType });
        }
        return entries;
    }

    public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
    {
        if (OnSelectEntryHandler == null)
        {
            return false;
        }
        return OnSelectEntryHandler(SearchTreeEntry, context);
    }
}