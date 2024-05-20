using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace QZGameFramework.UIManager
{
    public class GenerateWindowTool : Editor
    {
        private static Dictionary<string, string> methodDic = new Dictionary<string, string>();

        [MenuItem("GameObject/GenerateWindowScript(Shift+V) #V", false, 0)]
        private static void CreateFindComponentScripts()
        {
            // 获取当前选择的物体
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null)
            {
                Debug.LogError("Need select a GameObject");
                return;
            }

            //设置脚本生成路径
            if (!Directory.Exists(GenerateConfig.WindowGeneratePath))
            {
                Directory.CreateDirectory(GenerateConfig.WindowGeneratePath);
            }

            //生成CS脚本
            string csContnet = CreateWindowCS(obj.name);

            //Debug.Log("CsConent:\n" + csContnet);
            string cspath = GenerateConfig.WindowGeneratePath + "/" + obj.name + ".cs";
            UIWindowEditor.ShowScriptWindow(csContnet, cspath, methodDic);
        }

        /// <summary>
        /// 生成Window脚本
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string CreateWindowCS(string name)
        {
            // 储存字段名称
            string datalistJson = PlayerPrefs.GetString(GenerateConfig.OBJDATALIST_KEY);
            List<EditorObjectData> objDatalist = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistJson);
            methodDic.Clear();
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + name + "类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Author: [" + GenerateConfig.AUTHOR_NAME + "]");
            sb.AppendLine("/* Description: This is the class used to bind the Window prefab.");
            sb.AppendLine("/* 描述: 这是用于绑定 Window 预制体的类。");
            sb.AppendLine("/* 注意: 如需重新生成此文件，请务必对此 Window 的预制体进行新的修改后，并重新生成对应的 DataComponent 类后再重新生成此文件，否则现有编写的代码将会被全部覆盖为默认代码！！！");
            sb.AppendLine("--------------------------------------- */");
            sb.AppendLine();

            // 添加引用
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");
            sb.AppendLine("using QZGameFramework.UIManager;");
            sb.AppendLine();

            // 生成类名
            sb.AppendLine($"public class {name} : WindowBase");
            sb.AppendLine("{");

            sb.AppendLine($"\t// UI面板的组件类");
            // 生成字段
            sb.AppendLine($"\tprivate {name}DataComponent dataCompt;");

            // 生成生命周期函数
            sb.AppendLine();

            sb.AppendLine($"\t#region 生命周期函数");
            sb.AppendLine();
            // OnAwake
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// 在物体显示时执行一次，与Mono Awake一致");
            sb.AppendLine($"\t/// </summary>");
            sb.AppendLine("\tpublic override void OnAwake()");
            sb.AppendLine("\t{");
            sb.AppendLine($"\t\tdataCompt=gameObject.GetComponent<{name}DataComponent>();");
            sb.AppendLine($"\t\tdataCompt.InitUIComponent(this);");
            sb.AppendLine("\t\tbase.OnAwake();");
            sb.AppendLine("\t}");

            sb.AppendLine();

            // OnShow
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// 在物体显示时执行一次，与Mono OnEnable一致");
            sb.AppendLine($"\t/// </summary>");
            sb.AppendLine("\tpublic override void OnShow()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tbase.OnShow();");
            sb.AppendLine("\t}");

            sb.AppendLine();

            // OnHide
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t/// 在物体隐藏时执行一次，与Mono OnDisable 一致");
            sb.AppendLine($"\t/// </summary>");
            sb.AppendLine("\tpublic override void OnHide()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tbase.OnHide();");
            sb.AppendLine("\t}");

            sb.AppendLine();

            // OnDestroy
            sb.AppendLine($"\t/// <summary>");
            sb.AppendLine($"\t///  在物体销毁时执行一次，与Mono OnDestroy一致");
            sb.AppendLine($"\t/// </summary>");
            sb.AppendLine("\tpublic override void OnDestroy()");
            sb.AppendLine("\t{");
            sb.AppendLine("\t\tbase.OnDestroy();");
            sb.AppendLine("\t}");
            sb.AppendLine();
            sb.AppendLine($"\t#endregion");

            sb.AppendLine();

            // Custom API Function
            sb.AppendLine($"\t#region Custom API Function");
            sb.AppendLine();
            sb.AppendLine($"\t#endregion");
            sb.AppendLine();

            // UI组件事件生成
            sb.AppendLine($"\t#region UI组件事件");
            sb.AppendLine();
            foreach (var item in objDatalist)
            {
                string type = item.fieldType;
                string methodName = "On" + item.fieldName;
                string suffix = "";
                if (type.Contains("Button"))
                {
                    suffix = "ButtonClick";
                    CreateMethod(sb, ref methodDic, methodName + suffix);
                }
                else if (type.Contains("InputField"))
                {
                    suffix = "InputChange";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                    suffix = "InputEnd";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "string text");
                }
                else if (type.Contains("Toggle"))
                {
                    suffix = "ToggleChange";
                    CreateMethod(sb, ref methodDic, methodName + suffix, "bool state,Toggle toggle");
                }
            }
            //sb.AppendLine();
            sb.AppendLine($"\t#endregion");

            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// 生成UI事件方法
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="methodDic"></param>
        /// <param name="methodName"></param>
        /// <param name="param"></param>
        private static void CreateMethod(StringBuilder sb, ref Dictionary<string, string> methodDic, string methodName, string param = "")
        {
            //声明UI组件事件
            sb.AppendLine($"\tpublic void {methodName}({param})");
            sb.AppendLine("\t{");
            if (methodName == "OnCloseButtonClick")
            {
                sb.AppendLine("\t\tHideWindow();");
            }
            sb.AppendLine("\t}");
            sb.AppendLine();

            //存储UI组件事件 提供给后续新增代码使用
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"public void {methodName}({param})");
            builder.AppendLine("\t{");
            builder.AppendLine("\t}");
            builder.AppendLine();
            methodDic.Add(methodName, builder.ToString());
        }
    }
}