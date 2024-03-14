using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    private Editor editor;

    // 暴露当前组件在 UIBuilder 窗口中
    public new class UxmlFactory : UxmlFactory<InspectorView, GraphView.UxmlTraits>
    { }

    /// <summary>
    /// 选中结点视图的时候 更新Inpector窗口的信息
    /// </summary>
    /// <param name="nodeView"></param>
    public void UpdateSelection(NodeView nodeView)
    {
        Clear();

        // 销毁上一次创建的 editor
        // 创建一个类似 Unity Inspector的editor编辑器实例 用于绘制 BaseNode
        Object.DestroyImmediate(editor);
        if (nodeView != null)
        {
            editor = Editor.CreateEditor(nodeView.node);

            IMGUIContainer container = new IMGUIContainer(() =>
            {
                // 选择的 BaseNode 不为空才进行绘制
                if (editor.target != null)
                {
                    editor.OnInspectorGUI();
                }
            });

            this.Add(container);
        }
    }

    public void CreateNewDialogueTree(System.Action callback)
    {
    }
}