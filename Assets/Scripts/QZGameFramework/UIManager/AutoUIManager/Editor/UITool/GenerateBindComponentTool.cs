using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QZGameFramework.AutoUIManager
{
    public class GenerateBindComponentTool : Editor
    {
        // 查找对象的数据
        public static List<EditorObjectData> objDataList;

        [MenuItem("GameObject/GenerateUIComponentScript(Shift+B) #B", false, 0)]
        private static void CreateBindComponentScripts()
        {
            // 获取当前选择的物体
            GameObject obj = Selection.objects.First() as GameObject;
            if (obj == null)
            {
                Debug.LogError("Need select a GameObject");
                return;
            }
            objDataList = new List<EditorObjectData>();

            // 设置脚本生成路径
            if (!Directory.Exists(GenerateConfig.BindComponentGeneratePath))
            {
                Directory.CreateDirectory(GenerateConfig.BindComponentGeneratePath);
            }

            AnalysisWindowNodeData(obj.transform, obj.name);

            // 生成C#脚本
            string str = CreateCS(obj.name);

            // 储存字段名称
            string datalistJson = JsonConvert.SerializeObject(objDataList);
            PlayerPrefs.SetString(GenerateConfig.OBJDATALIST_KEY, datalistJson);

            string csPath = GenerateConfig.BindComponentGeneratePath + "/" + obj.name + "DataComponent.cs";
            UIWindowEditor.ShowScriptWindow(str, csPath);
            EditorPrefs.SetString("GeneratorClassName", obj.name + "DataComponent");
        }

        /// <summary>
        /// 解析 UI 面板下的带 [] 的组件
        /// </summary>
        /// <param name="trans">父物体</param>
        /// <param name="windowName">窗口名字</param>
        private static void AnalysisWindowNodeData(Transform trans, string windowName)
        {
            // 遍历孩子节点
            for (int i = 0; i < trans.childCount; i++)
            {
                // 得到孩子节点的 GameObject 对象
                GameObject obj = trans.GetChild(i).gameObject;
                // 获取它的名字
                string name = obj.name;
                // 判断名字中是否包含 [] 中括号元素
                if (name.Contains("[") && name.Contains("]"))
                {
                    // 解析组件名 获得组件类型和名字
                    int index = name.IndexOf("]") + 1;
                    string fieldName = name.Substring(index, name.Length - index); // 获取字段昵称
                    string fieldType = name.Substring(1, index - 2); // 获取字段类型
                    // 保存到容器中
                    objDataList.Add(new EditorObjectData { fieldName = fieldName, fieldType = fieldType, insID = obj.GetInstanceID() });
                }
                // 递归
                AnalysisWindowNodeData(trans.GetChild(i), windowName);
            }
        }

        /// <summary>
        /// 生成C#脚本文件
        /// </summary>
        /// <param name="name">UI窗口名字</param>
        private static string CreateCS(string windowName)
        {
            StringBuilder sb = new StringBuilder();
            string nameSpaceName = "QZGameFramework.AutoUIManager";

            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + windowName + "组件类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Description: It is used to mount the corresponding Window object and automatically obtain UI components.");
            sb.AppendLine("/* 描述: 用于挂载在对应的Window物体上，自动获取UI组件。");
            sb.AppendLine("/* 此文件为自动生成，请尽量不要修改，重新生成将会覆盖原有修改！！！");
            sb.AppendLine("------------------------------------ */");
            sb.AppendLine();

            //添加引用
            sb.AppendLine("using TMPro;");
            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using UnityEngine.UI;");

            sb.AppendLine();

            //生成命名空间
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                sb.AppendLine($"namespace {nameSpaceName}");
                sb.AppendLine("{");
            }
            sb.AppendLine($"\t[DisallowMultipleComponent]");
            sb.AppendLine($"\tpublic class {windowName + "DataComponent : MonoBehaviour"}");
            sb.AppendLine("\t{");

            //根据字段数据列表 声明字段
            foreach (var item in objDataList)
            {
                sb.AppendLine("\t\t[ReadOnly] public " + item.fieldType + " " + item.fieldName + item.fieldType + ";");
            }

            sb.AppendLine();
            //声明初始化组件接口
            sb.AppendLine("\t\tpublic void InitUIComponent(WindowBase target)");
            sb.AppendLine("\t\t{");
            sb.AppendLine("\t\t\t// 组件事件绑定");
            //得到逻辑类 WindowBase => LoginWindow
            sb.AppendLine($"\t\t\t{windowName} mWindow=target as {windowName};");

            //生成UI事件绑定代码
            foreach (var item in objDataList)
            {
                string type = item.fieldType;
                string methodName = item.fieldName;
                string suffix = "";
                if (type.Contains("Button"))
                {
                    suffix = "Click";
                    sb.AppendLine($"\t\t\ttarget.AddButtonClickListener({methodName}{type},mWindow.On{methodName}Button{suffix});");
                }
                if (type.Contains("InputField"))
                {
                    sb.AppendLine($"\t\t\ttarget.AddInputFieldListener({methodName}{type},mWindow.On{methodName}InputChange,mWindow.On{methodName}InputEnd);");
                }
                if (type.Contains("Toggle"))
                {
                    suffix = "Change";
                    sb.AppendLine($"\t\t\ttarget.AddToggleClickListener({methodName}{type},mWindow.On{methodName}Toggle{suffix});");
                }
            }
            sb.AppendLine("\t\t}");
            sb.AppendLine("\t}");
            if (!string.IsNullOrEmpty(nameSpaceName))
            {
                sb.AppendLine("}");
            }
            return sb.ToString();
        }

        private static EditorObjectData GetEditorObjectData(int insID)
        {
            return objDataList.Find(obj =>
            {
                return obj.insID == insID;
            });
        }

        /// <summary>
        /// 编译完成系统自动调用
        /// </summary>
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void AddComponent2Window()
        {
            //如果当前不是生成数据脚本的回调，就不处理
            string className = EditorPrefs.GetString("GeneratorClassName");
            if (string.IsNullOrEmpty(className))
            {
                return;
            }
            //1.通过反射的方式，从程序集中找到这个脚本，把它挂在到当前的物体上
            //获取所有的程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            //找到Csharp程序集
            var cSharpAssembly = assemblies.First(assembly => assembly.GetName().Name == "Assembly-CSharp");
            //获取类所在的程序集路径
            string relClassName = "QZGameFramework.AutoUIManager." + className;
            Type type = cSharpAssembly.GetType(relClassName);
            if (type == null)
            {
                return;
            }

            //获取要挂载的那个物体
            string windowObjName = className.Replace("DataComponent", "");
            GameObject windowObj = GameObject.Find(windowObjName);
            if (windowObj == null)
            {
                windowObj = GameObject.Find("UIRoot/" + windowObjName);
                if (windowObj == null)
                {
                    return;
                }
            }
            //先获取现窗口上有没有挂载该数据组件，如果没挂载在进行挂载
            Component compt = windowObj.GetComponent(type);
            if (compt == null)
            {
                compt = windowObj.AddComponent(type);
            }
            //2.通过反射的方式，遍历数据列表 找到对应的字段，赋值
            //获取对象数据列表
            string datalistJson = PlayerPrefs.GetString(GenerateConfig.OBJDATALIST_KEY);
            List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistJson);
            //获取脚本所有字段
            FieldInfo[] fieldInfoList = type.GetFields();

            foreach (var item in fieldInfoList)
            {
                foreach (var objData in objDataList)
                {
                    if (item.Name == objData.fieldName + objData.fieldType)
                    {
                        //根据Insid找到对应的对象
                        GameObject uiObject = EditorUtility.InstanceIDToObject(objData.insID) as GameObject;
                        //设置该字段所对应的对象
                        if (string.Equals(objData.fieldType, "GameObject"))
                        {
                            item.SetValue(compt, uiObject);
                        }
                        else
                        {
                            item.SetValue(compt, uiObject.GetComponent(objData.fieldType));
                        }
                        break;
                    }
                }
            }

            EditorPrefs.DeleteKey("GeneratorClassName");
        }
    }

    public class EditorObjectData
    {
        public int insID;
        public string fieldName; // 字段名
        public string fieldType; // 字段类型
    }
}