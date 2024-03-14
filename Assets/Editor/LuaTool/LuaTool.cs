using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace QZGameFramework.GameTool
{
    /// <summary>
    /// Lua 工具类
    /// 用于自动生成 Lua .txt文件 方便AB包打包
    /// </summary>
    public class LuaTool
    {
        private SerializedObject serializedObject;
        private SerializedProperty luaDirNameProperty; // Lua原文件夹名
        private SerializedProperty luaNewDirNameProperty; // Lua转存文件夹名
        private SerializedProperty abNameProperty; // Lua的AB包名

        //private string luaDirName = "Lua"; // Lua原文件夹名
        //private string luaNewDirName = "LuaTxt"; // Lua转存文件夹名
        //private string abName = "lua"; // Lua的AB包名

        //[MenuItem("GameTool/LuaTool")]
        //public static void OpenLuaToolWindow()
        //{
        //    LuaTool window = EditorWindow.GetWindowWithRect<LuaTool>(new Rect(0, 0, 250, 130));
        //    window.autoRepaintOnSceneChange = true;
        //    window.Show();
        //}

        private void Init()
        {
            serializedObject?.Dispose();
            serializedObject = new SerializedObject(LuaToolScriptableObject.Instance);
            luaDirNameProperty = serializedObject.FindProperty("luaDirName");
            luaNewDirNameProperty = serializedObject.FindProperty("luaNewDirName");
            abNameProperty = serializedObject.FindProperty("abName");
        }

        public void OnGUI()
        {
            if (serializedObject == null || !serializedObject.targetObject)
            {
                Init();
            }

            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Lua原文件夹名");
                    luaDirNameProperty.stringValue = GUILayout.TextField(luaDirNameProperty.stringValue, GUILayout.Width(400f));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Lua转存文件夹名");
                    luaNewDirNameProperty.stringValue = GUILayout.TextField(luaNewDirNameProperty.stringValue, GUILayout.Width(400f));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Lua的AB包名");
                    abNameProperty.stringValue = GUILayout.TextField(abNameProperty.stringValue, GUILayout.Width(400f));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                LuaToolScriptableObject.Save();
            }

            if (GUILayout.Button("Copy Lua To Txt", GUILayout.Height(30f)))
            {
                CopyLuaToTxt(luaDirNameProperty.stringValue, luaNewDirNameProperty.stringValue, abNameProperty.stringValue);
            }

            //GUI.Label(new Rect(10, 10, 100, 20), "Lua原文件夹名");
            //luaDirNameProperty.stringValue = GUI.TextField(new Rect(120, 10, 100, 20), luaDirNameProperty.stringValue);
            //GUI.Label(new Rect(10, 35, 100, 20), "Lua转存文件夹名");
            //luaNewDirNameProperty.stringValue = GUI.TextField(new Rect(120, 35, 100, 20), luaNewDirNameProperty.stringValue);
            //GUI.Label(new Rect(10, 60, 100, 20), "Lua的AB包名");
            //abNameProperty.stringValue = GUI.TextField(new Rect(120, 60, 100, 20), abNameProperty.stringValue);

            // 把 lua 文件增加 .txt后缀 并移动到指定路径
        }

        /// <summary>
        /// 把 lua 文件增加 .txt后缀 并移动到指定路径存放
        /// </summary>
        private void CopyLuaToTxt(string dirName, string newDirName, string abName)
        {
            // lua 文件的存放路径
            string path = Application.dataPath + $"/{dirName}/";

            if (!Directory.Exists(path))
            {
                return;
            }

            // 移动到的新存储路径
            string newPath = Application.dataPath + $"/{newDirName}/";
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            else
            {
                // 得到路径下的所有 .txt的文件
                string[] oldFileStrs = Directory.GetFiles(newPath, "*.txt");
                foreach (string file in oldFileStrs)
                {
                    // 删除文件
                    File.Delete(file);
                }
            }

            // 找到原路径下所有后缀为.lua的文件
            string[] strs = Directory.GetFiles(path, "*.lua");
            List<string> newFileNames = new List<string>();
            string fileName = null;
            foreach (string file in strs)
            {
                // 拼接文件新的路径且加上.txt后缀
                fileName = newPath + file.Substring(file.LastIndexOf("/") + 1) + ".txt";
                newFileNames.Add(fileName);
                File.Copy(file, fileName);
            }

            AssetDatabase.Refresh();

            // 编辑器界面后 再修改文件的AB包路径
            foreach (string newFileName in newFileNames)
            {
                // 这个API传入的路径必须是 相对于Assets文件夹 Assets/.../...
                AssetImporter import = AssetImporter.GetAtPath(newFileName.Substring(newFileName.IndexOf("Asset")));
                if (import != null)
                {
                    // 修改文件的AB包
                    import.assetBundleName = abName;
                }
            }

            Debug.Log("Lua文件转存成功");
        }
    }
}