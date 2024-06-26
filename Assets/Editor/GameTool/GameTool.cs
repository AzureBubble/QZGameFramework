using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace QZGameFramework.GameTool
{
    public class GameTool : EditorWindow
    {
        //private static DialogueData_SO currentData = null; // 当前对话数据

        [MenuItem("GameTool/OpenGameToolWindow")]
        private static void OpenGameToolWindow()
        {
            GameTool window = EditorWindow.GetWindow<GameTool>("GameTool");
            window.autoRepaintOnSceneChange = true;
            window.minSize = new Vector2(400f, 250f);
            window.maxSize = new Vector2(900f, 650f);
            window.Show();
        }

        private static ToolType type;
        private ExcelTool excelTool;
        private ABTool abTool;

        //private static DialogueTool dialogueTool;

        private void OnEnable()
        {
            Init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void Init()
        {
            type = ToolType.ExcelTool;
            if (excelTool == null)
            {
                excelTool = new ExcelTool();
            }
            if (abTool == null)
            {
                abTool = new ABTool();
            }
            //if (dialogueTool == null)
            //{
            //    dialogueTool = new DialogueTool(currentData);
            //}
        }

        private void OnGUI()
        {
            TypeToggle();

            switch (type)
            {
                case ToolType.ExcelTool:
                    excelTool?.OnGUI();
                    break;

                case ToolType.ABTool:
                    abTool?.OnGUI();
                    break;

                    //case ToolType.DialogueTool:
                    //    dialogueTool?.OnGUI();
                    //    break;
            }
        }

        /// <summary>
        /// 工具页签切换
        /// </summary>
        private void TypeToggle()
        {
            // 绘制顶部工具选择页签
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            float toolbarWidth = position.width - 15 * 4;
            string[] labels = new string[3] { "ExcelTool", "ABTool", "LuaTool" };
            type = (ToolType)GUILayout.Toolbar((int)type, labels, GUILayout.Width(toolbarWidth), GUILayout.Height(30f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(20f);
            //if (type != ToolType.DialogueTool)
            //{
            //    dialogueTool?.OnDisable();
            //}
        }

        private void OnDestroy()
        {
            //currentData = null;
            excelTool = null;
            abTool = null;
            //dialogueTool?.OnDisable();
            //dialogueTool = null;
        }

        /// <summary>
        /// 工具类型
        /// </summary>
        public enum ToolType
        {
            ExcelTool, ABTool,
        }
    }
}