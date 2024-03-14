using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeTool : EditorWindow
{
    public static BehaviourTreeView behaviourTreeView;
    public BTInspectorView inspectorView;

    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("GameTool/OpenBehaviourTreeTool")]
    public static void ShowExample()
    {
        BehaviourTreeTool wnd = GetWindow<BehaviourTreeTool>();
        wnd.titleContent = new GUIContent("BehaviourTreeTool");
    }

    [OnOpenAsset]
    public static bool OnBehaviourTreeAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            ShowExample();

            BehaviourTree tree = null;
            if (Selection.activeObject is BehaviourTree)
            {
                tree = Selection.activeObject as BehaviourTree;
            }
            else
            {
                tree = null;
            }

            //nodeTreeView.PopulateView(tree);
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

        behaviourTreeView = root.Q<BehaviourTreeView>();
        inspectorView = root.Q<BTInspectorView>();
        behaviourTreeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }

    /// <summary>
    /// 结点视图变换时
    /// </summary>
    /// <param name="view"></param>
    private void OnNodeSelectionChanged(BTNodeView view)
    {
        inspectorView.UpdateSelection(view);
    }

    private BehaviourTree tree;

    /// <summary>
    /// Project窗口中选择项变换时执行的操作
    /// </summary>
    private void OnSelectionChange()
    {
        if (Selection.activeObject is BehaviourTree)
        {
            tree = Selection.activeObject as BehaviourTree;
        }
        if (Selection.activeObject is not BehaviourTree && !Application.isPlaying)
        {
            tree = null;
        }

        behaviourTreeView.PopulateView(tree);
    }

    private void OnInspectorUpdate()
    {
        // 更新 nodeTreeView 中的结点状态更新方法
        behaviourTreeView?.UpdateNodeState();
        //behaviourTreeView?.SortList();
    }

    private void OnDestroy()
    {
        behaviourTreeView = null;
    }
}