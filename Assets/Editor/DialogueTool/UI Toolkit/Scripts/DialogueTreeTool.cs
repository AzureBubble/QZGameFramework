using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueTreeTool : EditorWindow
{
    public static NodeTreeView nodeTreeView;
    public InspectorView inspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("GameTool/OpenDialogueTreeWindow")]
    public static void ShowExample()
    {
        DialogueTreeTool wnd = GetWindow<DialogueTreeTool>();
        wnd.titleContent = new GUIContent("DialogueTreeTool");
    }

    [OnOpenAsset]
    public static bool OnDialogueAsset(int instanceId, int line)
    {
        if (Selection.activeObject is DialogueNodeTree)
        {
            ShowExample();

            DialogueNodeTree tree = null;
            if (Selection.activeObject is DialogueNodeTree)
            {
                tree = Selection.activeObject as DialogueNodeTree;
            }
            else
            {
                tree = null;
            }

            nodeTreeView.PopulateView(tree);
            return true;
        }
        return false;
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        //VisualElement label = new Label("Hello World! From C#");
        //root.Add(label);

        // Instantiate UXML
        //VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        m_VisualTreeAsset.CloneTree(root);

        // 找到编辑器中的NodeTreeView 将nodeTreeView结点树视图添加到编辑器视图中
        nodeTreeView = root.Q<NodeTreeView>();
        inspectorView = root.Q<InspectorView>();
        nodeTreeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    /// <summary>
    /// 结点视图变换时
    /// </summary>
    /// <param name="view"></param>
    private void OnNodeSelectionChanged(NodeView view)
    {
        inspectorView?.UpdateSelection(view);
    }

    private DialogueNodeTree tree;

    /// <summary>
    /// Project窗口中选择项变换时执行的操作
    /// </summary>
    private void OnSelectionChange()
    {
        if (Selection.activeObject is DialogueNodeTree)
        {
            tree = Selection.activeObject as DialogueNodeTree;
        }

        if (Selection.activeObject is not DialogueNodeTree && !Application.isPlaying)
        {
            tree = null;
        }

        nodeTreeView?.PopulateView(tree);
    }

    private void OnInspectorUpdate()
    {
        // 更新 nodeTreeView 中的结点状态更新方法
        nodeTreeView?.UpdateNodeState();
    }

    private void OnDestroy()
    {
        nodeTreeView = null;
    }
}