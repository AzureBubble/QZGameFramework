using Excel;
using System;
using System.Data;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace QZGameFramework.GameTool
{
    /// <summary>
    /// Excel 工具类
    /// 用于读取 Excel 配置表
    /// </summary>
    public class ExcelTool
    {
        private SerializedObject serializedObject;
        private SerializedProperty excelPathProperty; // Excel文件存放路径
        private SerializedProperty dataClassPathProperty; // 数据结构类存储路径
        private SerializedProperty dataContainerPathProperty; // 数据容器类存储路径
        private SerializedProperty dataBinaryPathProperty; // 二进制数据存储路径
        private SerializedProperty dataJsonPathProperty; // Json数据存储路径
        private SerializedProperty scriptableObjectPathProperty; // ScriptableObject生成路径
        private SerializedProperty generateSOCSPathProperty; // ScriptableObject生成路径
        private SerializedProperty generateEnumPathProperty; // 枚举类生成路径

        private StringBuilder tempString;

        /// <summary>
        /// Excel文件存放路径
        /// </summary>
        //private static StringBuilder EXCEL_PATH;

        /// <summary>
        /// 数据结构类存储路径
        /// </summary>
        //private static string DATA_CLASS_PATH = Application.dataPath + "/Scripts/ExcelData/DataClass/";

        /// <summary>
        /// 数据容器类存储路径
        /// </summary>
        //private static string DATA_CONTAINER_PATH = Application.dataPath + "/Scripts/ExcelData/Container/";

        /// <summary>
        /// 二进制数据存储路径
        /// </summary>
        //private static string DATA_BINARY_PATH = Application.streamingAssetsPath + "/Binary/";

        /// <summary>
        /// Json数据存储路径
        /// </summary>
        //private static string DATA_JSON_PATH = Application.streamingAssetsPath + "/Json/";

        /// <summary>
        /// 变量名所在行索引
        /// </summary>
        private const int BEGIN_VARIABLE_NAME_INDEX = 0;

        /// <summary>
        /// 变量类型所在行索引
        /// </summary>
        private const int BEGIN_VARIABLE_TYPE_INDEX = 1;

        /// <summary>
        /// 获取主键所在行索引
        /// </summary>
        private const int BEGIN_KEY_INDEX = 2;

        /// <summary>
        /// 变量描述所在行索引
        /// </summary>
        private const int BEGIN_DESCRIPTION_INDEX = 3;

        /// <summary>
        /// 数据内容开始行号
        /// </summary>
        private static int BEGIN_INDEX = 4;

        private int nowSelectedIndex = 0;
        private string[] targetStrs = new string[] { "Binary", "Json" };
        private StringBuilder dataClassSB = new StringBuilder();
        //[MenuItem("GameTool/ExcelTool")]
        //public static void OpenExcelToolWindow()
        //{
        //    // 获取 ExcelTool 编辑器窗口对象
        //    ExcelTool window = EditorWindow.GetWindowWithRect<ExcelTool>(new Rect(0, 0, 280, 160));
        //    window.autoRepaintOnSceneChange = true;
        //    // 显示窗口
        //    window.Show();
        //}

        private void OnFocus()
        {
            Init();
        }

        private void Init()
        {
            serializedObject?.Dispose();
            serializedObject = new SerializedObject(ExcelToolScriptableObject.Instance);
            excelPathProperty = serializedObject.FindProperty("excelPath");
            dataClassPathProperty = serializedObject.FindProperty("dataClassPath");
            dataContainerPathProperty = serializedObject.FindProperty("dataContainerPath");
            dataBinaryPathProperty = serializedObject.FindProperty("dataBinaryPath");
            dataJsonPathProperty = serializedObject.FindProperty("dataJsonPath");
            scriptableObjectPathProperty = serializedObject.FindProperty("scriptableObjectPath");
            generateSOCSPathProperty = serializedObject.FindProperty("generateSOCSPath");
            generateEnumPathProperty = serializedObject.FindProperty("generateEnumPath");
        }

        public void OnGUI()
        {
            if (serializedObject == null || !serializedObject.targetObject)
            {
                Init();
            }

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("生成目标文件格式选择");
                nowSelectedIndex = GUILayout.Toolbar(nowSelectedIndex, targetStrs);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5f);

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"读表路径:{excelPathProperty.stringValue}");
                    // excelPathProperty.stringValue = GUILayout.TextField(excelPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Excel Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Excel Folder", excelPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            excelPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"数据结构类存储路径:{dataClassPathProperty.stringValue}");
                    // dataClassPathProperty.stringValue = GUILayout.TextField(dataClassPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save DataClass Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save DataClass Folder", dataClassPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            dataClassPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"数据容器类存储路径:{dataContainerPathProperty.stringValue}");
                    // dataContainerPathProperty.stringValue = GUILayout.TextField(dataContainerPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save DataContainer Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save DataContainer Folder", dataContainerPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            dataContainerPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"二进制数据存储路径:{dataBinaryPathProperty.stringValue}");
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save DataBinary Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save DataBinary Folder", dataBinaryPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            dataBinaryPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                    // dataBinaryPathProperty.stringValue = GUILayout.TextField(dataBinaryPathProperty.stringValue, GUILayout.Width(400f));
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"Json数据存储路径:{dataJsonPathProperty.stringValue}");
                    // dataJsonPathProperty.stringValue = GUILayout.TextField(dataJsonPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save DataJson Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save DataJson Folder", dataJsonPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            dataJsonPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"SO资源生成路径:{scriptableObjectPathProperty.stringValue}");
                    // dataJsonPathProperty.stringValue = GUILayout.TextField(dataJsonPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save ScriptableObject Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save ScriptableObject Folder", scriptableObjectPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            scriptableObjectPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"SO脚本生成路径:{generateSOCSPathProperty.stringValue}");
                    // dataJsonPathProperty.stringValue = GUILayout.TextField(dataJsonPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save ScriptableObjectCS Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save ScriptableObjectCS Folder", generateSOCSPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            generateSOCSPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5f);

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label($"枚举类脚本生成路径:{generateEnumPathProperty.stringValue}");
                    // dataJsonPathProperty.stringValue = GUILayout.TextField(dataJsonPathProperty.stringValue, GUILayout.Width(400f));
                    GUILayout.FlexibleSpace(); // 插入弹性空间
                    GUILayout.Label("Select Save EnumCS Folder: ");
                    if (GUILayout.Button("Select Folder", GUILayout.Width(200)))
                    {
                        string newFolderPath = EditorUtility.OpenFolderPanel("Select Save EnumCS Folder", generateEnumPathProperty.stringValue, "");
                        if (!string.IsNullOrEmpty(newFolderPath))
                        {
                            generateEnumPathProperty.stringValue = newFolderPath.Replace(Application.dataPath, "Assets");
                        }
                    }
                }
                GUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ExcelToolScriptableObject.Save();
            }

            GUILayout.Space(5f);

            // 默认路径生成
            if (GUILayout.Button("读取路径中的所有Excel配置数据表", GUILayout.Height(30f)))
            {
                switch (targetStrs[nowSelectedIndex])
                {
                    case "Binary":
                        GenerateExcelToBinaryInfo();
                        break;

                    case "Json":
                        GenerateExcelToJsonInfo();
                        break;
                }
            }

            // 生成枚举类脚本
            if (GUILayout.Button("读取路径中的所有枚举类Excel配置表生成枚举类脚本", GUILayout.Height(30f)))
            {
                GenerateEnumButton();
            }

            // 生成ScriptableObject文件
            if (GUILayout.Button("读取路径中的所有Excel配置表生成ScriptableObject脚本文件", GUILayout.Height(30f)))
            {
                GenerateScriptableObjectCSButton();
            }

            // 生成ScriptableObject文件
            if (GUILayout.Button("读取路径中的所有Excel配置表生成ScriptableObject资源文件", GUILayout.Height(30f)))
            {
                GenerateScriptableObjectButton();
            }

            //GUI.Label(new Rect(10, 10, 250, 15), "生成目标文件格式选择");
            //nowSelectedIndex = GUI.Toolbar(new Rect(10, 30, 250, 25), nowSelectedIndex, targetStrs);

            //GUI.Label(new Rect(10, 60, 250, 15), "读表路径(末尾要加'/'):" + excelPathProperty.stringValue);

            //serializedObject.Update();
            //EditorGUI.BeginChangeCheck();

            //excelPathProperty.stringValue = GUI.TextField(new Rect(10, 80, 250, 15), excelPathProperty.stringValue);

            //if (EditorGUI.EndChangeCheck())
            //{
            //    serializedObject.ApplyModifiedProperties();
            //    ExcelToolScriptableObject.Save();
            //}
        }

        #region 生成ScriptableObject

        private void GenerateScriptableObjectCSButton(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = excelPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/';
            }
            // 创建一个目录对象，如果不存在的话，就创建一个目录
            DirectoryInfo dInfo = Directory.CreateDirectory(filePath);

            // 获取目录中的文件列表
            FileInfo[] files = dInfo.GetFiles();
            // 创建一个 DataTableCollection 以容纳 Excel 数据表
            DataTableCollection tableCollection;
            int count = 0;
            // 遍历文件列表目录中的每个文件
            foreach (FileInfo file in files)
            {
                // 检查文件扩展名，只处理 .xlsx 和 .xls 文件
                if (file.Extension != ".xlsx" && file.Extension != ".xls")
                {
                    continue;
                }

                // 使用 FileStream 打开每一个 Excel 文件以进行数据读取处理
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    // 创建 ExcelDataReader 以读取 Excel 文件
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    // 将 Excel 所有数据表存储到 DataTableCollection 容器中
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历 DataTableCollection 容器中的每个数据表
                foreach (DataTable table in tableCollection)
                {
                    // 生成ScriptableObject数据结构类
                    GenerateScriptableObjectClass(table);
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.LogError("所选文件夹中没有Excel配置表文件:" + excelPathProperty.stringValue);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据 Excel 配置表生成对应的ScriptableObject数据结构类
        /// </summary>
        /// <param name="table">Excel 数据表</param>
        private void GenerateScriptableObjectClass(DataTable table)
        {
            if (table == null) return;
            // 字段名行
            DataRow rowName = GetVariableNameRow(table);
            // 字段类型行
            DataRow rowType = GetVariableTypeRow(table);
            // 字段描述行
            DataRow rowDescription = GetVariableDescriptionRow(table);

            tempString = new StringBuilder(generateSOCSPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/');

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string generateSOCSPath = tempString.ToString();
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(generateSOCSPath))
            {
                Directory.CreateDirectory(generateSOCSPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + table.TableName + " ScriptableObject类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Description: It is an automatically generated ScriptableObject data structure class.");
            sb.AppendLine($"/* 描述: 自动生成的 {table.TableName} ScriptableObject 数据结构类。");
            sb.AppendLine("/* 此文件为自动生成，请尽量不要修改，重新生成将会覆盖原有修改！！！");
            sb.AppendLine("--------------------------------------- */");
            sb.AppendLine();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Sirenix.OdinInspector;");
            sb.AppendLine();

            sb.AppendLine($"[CreateAssetMenu(fileName = \"New {table.TableName}\", menuName = \"ScriptableObject/{table.TableName}\")]");
            //str += "public class " + table.TableName + "\n{\n";
            sb.AppendLine($"public class {table.TableName} : ScriptableObject");
            sb.AppendLine("{");

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (rowDescription[i].ToString() != "")
                {
                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine($"\t/// {rowDescription[i].ToString()}");
                    sb.AppendLine("\t/// </summary>");
                    //str += "\t/// <summary>\n" + "\t/// " + rowDescription[i].ToString() + "\n\t/// </summary>\n";

                    if (rowType[i].ToString() == "enum") // rowType[i].ToString() == "class" ||
                    {
                        sb.AppendLine($"\t[LabelText(\"{rowDescription[i].ToString()}\")]public {rowName[i].ToString()} {rowName[i].ToString()};");
                    }
                    else if (rowType[i].ToString() == "Sprite")
                    {
                        sb.AppendLine($"\t[LabelText(\"{rowDescription[i].ToString()}\"), LabelWidth(0.1f), PreviewField(70, ObjectFieldAlignment.Left), SuffixLabel(\"{rowDescription[i].ToString()}\")]public {rowType[i].ToString()} {rowName[i].ToString()};");
                    }
                    else
                    {
                        sb.AppendLine($"\t[LabelText(\"{rowDescription[i].ToString()}\")]public {rowType[i].ToString()} {rowName[i].ToString()};");
                    }
                }
                else
                {
                    if (rowType[i].ToString() == "enum") // rowType[i].ToString() == "class" ||
                    {
                        sb.AppendLine($"\tpublic {rowName[i].ToString()} {rowName[i].ToString()};");
                    }
                    else if (rowType[i].ToString() == "Sprite")
                    {
                        sb.AppendLine($"\tpublic {rowType[i].ToString()} {rowName[i].ToString()};");
                    }
                    else
                    {
                        sb.AppendLine($"\tpublic {rowType[i].ToString()} {rowName[i].ToString()};");
                    }
                }

                //str += "\tpublic " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
                if (rowDescription[i].ToString() != "") // || rowDescription[i + 1].ToString() != ""
                {
                    if (i < table.Columns.Count - 1)
                    {
                        sb.AppendLine();
                        //str += "\n";
                    }
                }
            }
            sb.AppendLine("}");
            AssetDatabase.Refresh();
            //str += "}";

            File.WriteAllText(Path.Combine(generateSOCSPath, table.TableName + ".cs"), sb.ToString());
            Debug.Log($"已生成 {table.TableName} 的ScriptableObject的资源文件脚本: {generateSOCSPath}");
        }

        private void GenerateScriptableObjectButton(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = excelPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/';
            }
            // 创建一个目录对象，如果不存在的话，就创建一个目录
            DirectoryInfo dInfo = Directory.CreateDirectory(filePath);

            // 获取目录中的文件列表
            FileInfo[] files = dInfo.GetFiles();
            // 创建一个 DataTableCollection 以容纳 Excel 数据表
            DataTableCollection tableCollection;
            int count = 0;
            // 遍历文件列表目录中的每个文件
            foreach (FileInfo file in files)
            {
                // 检查文件扩展名，只处理 .xlsx 和 .xls 文件
                if (file.Extension != ".xlsx" && file.Extension != ".xls")
                {
                    continue;
                }

                // 使用 FileStream 打开每一个 Excel 文件以进行数据读取处理
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    // 创建 ExcelDataReader 以读取 Excel 文件
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    // 将 Excel 所有数据表存储到 DataTableCollection 容器中
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历 DataTableCollection 容器中的每个数据表
                foreach (DataTable table in tableCollection)
                {
                    // 生成ScriptableObject资源文件
                    GenerateScriptableObject(table);
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.LogError("所选文件夹中没有Excel配置表文件:" + excelPathProperty.stringValue);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据 Excel 配置表生成对应的ScriptableObject资源文件
        /// <param name="table">Excel 数据表</param>
        /// </summary>
        private void GenerateScriptableObject(DataTable table)
        {
            if (table == null) return;
            // 字段名行
            DataRow rowName = GetVariableNameRow(table);
            // 字段类型行
            DataRow rowType = GetVariableTypeRow(table);

            tempString = new StringBuilder(scriptableObjectPathProperty.stringValue);

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string soPath = Path.Combine(tempString.ToString(), table.TableName);
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(soPath))
            {
                Directory.CreateDirectory(soPath);
            }
            DataRow row;
            //string str = "[\n";
            int index = 0;

            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                string soName;
                if (index == 0)
                {
                    soName = "New " + table.TableName;
                }
                else
                {
                    soName = "New " + table.TableName + " " + index;
                }
                ++index;
                row = table.Rows[i];

                ScriptableObject scriptableObject = ScriptableObject.CreateInstance(table.TableName);

                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (rowName[j].ToString().ToLower().Contains($"{table.TableName}name".ToLower()))
                    {
                        soName = table.Rows[i][j].ToString();
                    }

                    //TODO:添加对应的类型字段读写规则(ScriptableObject)
                    switch (rowType[j].ToString())
                    {
                        case "int":
                            scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, Convert.ChangeType(table.Rows[i][j], typeof(int)));
                            break;

                        case "float":
                            scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, Convert.ChangeType(table.Rows[i][j], typeof(float)));
                            break;

                        case "bool":
                            scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, Convert.ChangeType(table.Rows[i][j], typeof(bool)));
                            break;

                        case "string":
                            scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, Convert.ChangeType(table.Rows[i][j], typeof(string)));
                            break;

                        case "enum":
                            Type enumType = scriptableObject.GetType().GetField(rowName[j].ToString()).FieldType;
                            object enumValue = Enum.Parse(enumType, table.Rows[i][j].ToString());
                            scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, enumValue);
                            break;

                        case "Sprite":
                            if (!string.IsNullOrEmpty(table.Rows[i][j].ToString()))
                            {
                                string path = table.Rows[i][j].ToString();
                                if (!path.Contains("Assets/"))
                                {
                                    path = Path.Combine("Assets", path);
                                }
                                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                                scriptableObject.GetType().GetField(rowName[j].ToString()).SetValue(scriptableObject, sprite);
                            }
                            break;
                    }
                }
                if (scriptableObject != null)
                {
                    string outputPath = Path.Combine(soPath, $"{soName}.asset");
                    AssetDatabase.CreateAsset(scriptableObject, outputPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            //File.WriteAllText(soPath + table.TableName + ".json", str);

            Debug.Log($"已生成 {table.TableName} 的ScriptableObject资源文件: {soPath}");
        }

        private void GenerateEnumButton(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = excelPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/';
            }
            // 创建一个目录对象，如果不存在的话，就创建一个目录
            DirectoryInfo dInfo = Directory.CreateDirectory(filePath);

            // 获取目录中的文件列表
            FileInfo[] files = dInfo.GetFiles("*", SearchOption.AllDirectories);
            // 创建一个 DataTableCollection 以容纳 Excel 数据表
            DataTableCollection tableCollection;
            int count = 0;
            // 遍历文件列表目录中的每个文件
            foreach (FileInfo file in files)
            {
                // 检查文件扩展名，只处理 .xlsx 和 .xls 文件
                if (file.Extension != ".xlsx" && file.Extension != ".xls")
                {
                    continue;
                }

                // 使用 FileStream 打开每一个 Excel 文件以进行数据读取处理
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    // 创建 ExcelDataReader 以读取 Excel 文件
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    // 将 Excel 所有数据表存储到 DataTableCollection 容器中
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历 DataTableCollection 容器中的每个数据表
                foreach (DataTable table in tableCollection)
                {
                    // 生成枚举类
                    GenerateEnum(table);
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.LogError("所选文件夹中没有Excel配置表文件:" + excelPathProperty.stringValue);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 生成枚举类
        /// </summary>
        private void GenerateEnum(DataTable table)
        {
            if (!table.TableName.Contains("Enum_"))
            {
                return;
            }
            if (table == null) return;
            // 字段名行
            DataRow rowName = GetVariableNameRow(table, 1);
            // 字段描述行
            DataRow rowDescription = GetVariableDescriptionRow(table, 0);

            tempString = new StringBuilder(generateEnumPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/');

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string generateEnumPath = tempString.ToString();
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(generateEnumPath))
            {
                Directory.CreateDirectory(generateEnumPath);
            }

            string enumName = table.TableName.Replace("Enum_", "E_");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + table.TableName + " 枚举类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Description: It is an automatically generated enum class.");
            sb.AppendLine($"/* 描述: 自动生成的 {table.TableName} 枚举类。");
            sb.AppendLine("/* 此文件为自动生成，请尽量不要修改，重新生成将会覆盖原有修改！！！");
            sb.AppendLine("--------------------------------------- */");
            sb.AppendLine();

            sb.AppendLine("using UnityEngine;");
            sb.AppendLine("using Sirenix.OdinInspector;");
            sb.AppendLine();

            //str += "public class " + table.TableName + "\n{\n";
            sb.AppendLine($"public enum {enumName}");
            sb.AppendLine("{");
            {
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(table.Rows[i].ToString()))
                    {
                        return;
                    }

                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine($"\t/// {table.Rows[i][2].ToString()}");
                    sb.AppendLine("\t/// </summary>");
                    sb.AppendLine($"\t[LabelText(\"{table.Rows[i][2].ToString()}\")] {table.Rows[i][1].ToString()} = {table.Rows[i][0].ToString()},");

                    if (i + 1 < table.Rows.Count)
                    {
                        sb.AppendLine();
                    }
                }
            }

            //for (int i = 0; i < table.Columns.Count; i++)
            //{
            //    if (table.Rows[0].ToString() != "")
            //    {
            //        sb.AppendLine("\t/// <summary>");
            //        sb.AppendLine($"\t/// {table.Rows[0][i].ToString()}");
            //        sb.AppendLine("\t/// </summary>");
            //        //str += "\t/// <summary>\n" + "\t/// " + rowDescription[i].ToString() + "\n\t/// </summary>\n";
            //        sb.AppendLine($"\t[LabelText(\"{table.Rows[0][i].ToString()}\")]{table.Rows[1][i].ToString()},");
            //    }
            //    else
            //    {
            //        sb.AppendLine($"\t{table.Rows[1][i].ToString()},");
            //    }

            //    //str += "\tpublic " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
            //    if (table.Rows[0][i].ToString() != "" || table.Columns[i + 1].ToString() != "")
            //    {
            //        if (i < table.Columns.Count - 1)
            //        {
            //            sb.AppendLine();
            //            //str += "\n";
            //        }
            //    }
            //}
            sb.AppendLine("}");
            //str += "}";

            File.WriteAllText(Path.Combine(generateEnumPath, enumName + ".cs"), sb.ToString());
            Debug.Log($"已生成 {table.TableName} 枚举类脚本: {generateEnumPath}");
            AssetDatabase.Refresh();
        }

        #endregion 生成ScriptableObject

        private void OnDisable()
        {
            ExcelToolScriptableObject.Save();
        }

        /// <summary>
        /// 通过Excel配置表生成对应的数据对象结构类和容器类和二进制数据文件
        /// </summary>
        //[MenuItem("GameTool/ExcelTool/GenerateExcelToBinaryInfo")]
        private void GenerateExcelToBinaryInfo(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = excelPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/';
            }
            // 创建一个目录对象，如果不存在的话，就创建一个目录
            DirectoryInfo dInfo = Directory.CreateDirectory(filePath);

            // 获取目录中的文件列表
            FileInfo[] files = dInfo.GetFiles("*", SearchOption.AllDirectories);
            // 创建一个 DataTableCollection 以容纳 Excel 数据表
            DataTableCollection tableCollection;
            int count = 0;
            dataClassSB.Clear();
            // 遍历文件列表目录中的每个文件
            foreach (FileInfo file in files)
            {
                // 检查文件扩展名，只处理 .xlsx 和 .xls 文件
                if (file.Extension != ".xlsx" && file.Extension != ".xls")
                {
                    continue;
                }

                // 使用 FileStream 打开每一个 Excel 文件以进行数据读取处理
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    // 创建 ExcelDataReader 以读取 Excel 文件
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    // 将 Excel 所有数据表存储到 DataTableCollection 容器中
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历 DataTableCollection 容器中的每个数据表
                foreach (DataTable table in tableCollection)
                {
                    // 生成数据结构类
                    GenerateExcelToDataClass(table);
                    // 生成数据容器类
                    GenerateExcelToContainer(table);
                    // 生成二进制数据
                    GenerateExcelToBinary(table);
                    if (!table.TableName.Contains("Enum_"))
                        dataClassSB.Append($"{table.TableName},");
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.LogError("所选文件夹中没有Excel配置表文件:" + excelPathProperty.stringValue);
            }
            else
            {
                SaveAllDataClassName();
            }
            AssetDatabase.Refresh();
        }

        private void SaveAllDataClassName()
        {
            if (dataClassSB.Length <= 0)
            {
                return;
            }

            if (string.Equals(dataClassSB[^1], ','))
            {
                dataClassSB.Remove(dataClassSB.Length - 1, 1);
            }
            string savePath = "";
            savePath = Path.Combine(dataBinaryPathProperty.stringValue, "AllDataClassName.txt");
            File.WriteAllText(savePath, dataClassSB.ToString());
        }

        /// <summary>
        /// 通过Excel配置表生成对应的数据对象结构类和容器类和Json数据文件
        /// </summary>
        //[MenuItem("GameTool/ExcelTool/GenerateExcelToJsonInfo")]
        private void GenerateExcelToJsonInfo(string filePath = null)
        {
            if (filePath == null)
            {
                filePath = excelPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/';
            }
            // 创建一个目录对象，如果不存在的话，就创建一个目录
            DirectoryInfo dInfo = Directory.CreateDirectory(filePath);

            // 获取目录中的文件列表
            FileInfo[] files = dInfo.GetFiles("*", SearchOption.AllDirectories);
            // 创建一个 DataTableCollection 以容纳 Excel 数据表
            DataTableCollection tableCollection;
            int count = 0;
            // 遍历文件列表目录中的每个文件
            foreach (FileInfo file in files)
            {
                // 检查文件扩展名，只处理 .xlsx 和 .xls 文件
                if (file.Extension != ".xlsx" && file.Extension != ".xls")
                {
                    continue;
                }

                // 使用 FileStream 打开每一个 Excel 文件以进行数据读取处理
                using (FileStream fs = file.Open(FileMode.Open, FileAccess.Read))
                {
                    // 创建 ExcelDataReader 以读取 Excel 文件
                    IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                    // 将 Excel 所有数据表存储到 DataTableCollection 容器中
                    tableCollection = excelReader.AsDataSet().Tables;
                    fs.Close();
                }

                // 遍历 DataTableCollection 容器中的每个数据表
                foreach (DataTable table in tableCollection)
                {
                    // 生成数据结构类
                    GenerateExcelToDataClass(table);
                    // 生成数据容器类 默认 Json 不生成数据容器类
                    //GenerateExcelToContainer(table);
                    // 生成Json数据
                    GenerateExcelToJson(table);
                    count++;
                }
            }
            if (count == 0)
            {
                Debug.Log("所选文件夹中没有Excel配置表文件");
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 根据 Excel 配置表生成对应的数据结构类
        /// </summary>
        /// <param name="table">Excel 数据表</param>
        private void GenerateExcelToDataClass(DataTable table)
        {
            if (table.TableName.Contains("Enum_"))
            {
                GenerateEnum(table);
                return;
            }

            // 字段名行
            DataRow rowName = GetVariableNameRow(table);
            // 字段类型行
            DataRow rowType = GetVariableTypeRow(table);
            // 字段描述行
            DataRow rowDescription = GetVariableDescriptionRow(table);

            tempString = new StringBuilder(dataClassPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/');

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string dataClassPath = tempString.ToString();
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(dataClassPath))
            {
                Directory.CreateDirectory(dataClassPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + table.TableName + " 数据结构类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Description: It is an automatically generated data structure class.");
            sb.AppendLine($"/* 描述: 自动生成的 {table.TableName} 数据结构类。");
            sb.AppendLine("/* 此文件为自动生成，请尽量不要修改，重新生成将会覆盖原有修改！！！");
            sb.AppendLine("--------------------------------------- */");
            sb.AppendLine();

            if (targetStrs[nowSelectedIndex] == "Binary")
            {
                sb.AppendLine("[System.Serializable]");
            }

            sb.AppendLine("public class " + table.TableName);
            sb.AppendLine("{");

            //string str = "public class " + table.TableName + "\n{\n";

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (string.IsNullOrEmpty(rowName[i].ToString()))
                {
                    continue;
                }

                if (rowDescription[i].ToString() != "")
                {
                    sb.AppendLine("\t/// <summary>");
                    sb.AppendLine($"\t/// {rowDescription[i].ToString()}");
                    sb.AppendLine("\t/// </summary>");

                    //str += "\t/// <summary>\n" + "\t/// " + rowDescription[i].ToString() + "\n\t/// </summary>\n";
                }
                sb.AppendLine($"\tpublic {rowType[i].ToString()} {rowName[i].ToString()};");
                //str += "\tpublic " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
                if (rowDescription[i].ToString() != "" || rowDescription[i + 1].ToString() != "")
                {
                    if (i < table.Columns.Count - 1)
                    {
                        sb.AppendLine();
                        //str += "\n";
                    }
                }
            }
            sb.AppendLine("}");
            //str += "}";

            File.WriteAllText(dataClassPath + table.TableName + ".cs", sb.ToString());
        }

        /// <summary>
        /// 生成 Excel 表对应的数据容器类
        /// </summary>
        /// <param name="table">数据表</param>
        private void GenerateExcelToContainer(DataTable table)
        {
            if (table.TableName.Contains("Enum_"))
            {
                return;
            }

            // 得到主键索引
            int keyIndex = GetKeyIndex(table);

            // 得到字段类型行
            DataRow rowType = GetVariableTypeRow(table);

            tempString = new StringBuilder(dataContainerPathProperty.stringValue.Replace("Assets", Application.dataPath) + '/');

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string dataContainerPath = tempString.ToString();
            Debug.Log("dataContainerPath:" + dataContainerPath);
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(dataContainerPath))
            {
                Directory.CreateDirectory(dataContainerPath);
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("/* ------------------------------------");
            sb.AppendLine("/* Title: " + table.TableName + " 数据结构容器类");
            sb.AppendLine("/* Creation Time: " + System.DateTime.Now);
            sb.AppendLine("/* Description: It is an automatically generated data structure class.");
            sb.AppendLine($"/* 描述: 自动生成的 {table.TableName} 数据结构容器类。");
            sb.AppendLine("/* 此文件为自动生成，请尽量不要修改，重新生成将会覆盖原有修改！！！");
            sb.AppendLine("--------------------------------------- */");
            sb.AppendLine();

            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine();

            sb.AppendLine($"public class {table.TableName}Container ");
            sb.AppendLine("{");
            {
                sb.AppendLine($"\tprivate Dictionary<{rowType[keyIndex].ToString()}, {table.TableName}> dataDic = new Dictionary<{rowType[keyIndex].ToString()}, {table.TableName}>();");
                sb.AppendLine();
                sb.AppendLine($"\tpublic {table.TableName} GetData({rowType[keyIndex].ToString()} key)");
                sb.AppendLine("\t{");
                {
                    sb.AppendLine($"\t\tif (dataDic.TryGetValue(key, out {table.TableName} data))");
                    sb.AppendLine("\t\t{");
                    {
                        sb.AppendLine("\t\t\treturn data;");
                    }
                    sb.AppendLine("\t\t}");
                    sb.AppendLine("\t\treturn null;");
                }
                sb.AppendLine("\t}");
            }
            sb.AppendLine("}");
            //string str = "using System.Collections.Generic;\n\n";
            //str += "public class " + table.TableName + "Container " + "\n{\n";
            //str += "\tpublic Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">";
            //str += " dataDic = new ();\n";
            //str += "}";

            File.WriteAllText(dataContainerPath + table.TableName + "Container.cs", sb.ToString());
        }

        /// <summary>
        /// 生成 Excel 二进制数据
        /// </summary>
        /// <param name="table">数据表</param>
        private void GenerateExcelToBinary(DataTable table)
        {
            if (table.TableName.Contains("Enum_"))
            {
                return;
            }

            tempString = new StringBuilder(dataBinaryPathProperty.stringValue);

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string dataBinaryPath = tempString.ToString();
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(dataBinaryPath))
            {
                Directory.CreateDirectory(dataBinaryPath);
            }

            // 创建二进制文件
            using (FileStream fs = new FileStream(dataBinaryPath + table.TableName + ".sav", FileMode.OpenOrCreate, FileAccess.Write))
            {
                // 存储数据内容行数
                fs.Write(BitConverter.GetBytes(table.Rows.Count - 4), 0, 4);
                // 存储主键变量名
                string keyName = GetVariableNameRow(table)[GetKeyIndex(table)].ToString();
                byte[] bytes = Encoding.UTF8.GetBytes(keyName);
                // 存储主键字节数组长度
                fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                // 存储主键字节数组
                fs.Write(bytes, 0, bytes.Length);

                DataRow row;
                DataRow rowType = GetVariableTypeRow(table);
                DataRow rowName = GetVariableNameRow(table);
                // 每一行数据
                for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    // 每一列数据
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (string.IsNullOrEmpty(rowName[j].ToString()))
                        {
                            continue;
                        }

                        //TODO:添加对应的类型字段读写规则(Binary)
                        switch (rowType[j].ToString())
                        {
                            case "int":
                                fs.Write(BitConverter.GetBytes(String.IsNullOrEmpty(row[j].ToString()) ? 0 : int.Parse(row[j].ToString())), 0, 4);
                                break;

                            case "float":
                                fs.Write(BitConverter.GetBytes(float.Parse(row[j].ToString())), 0, 4);
                                break;

                            case "bool":
                                fs.Write(BitConverter.GetBytes(bool.Parse(row[j].ToString())), 0, 1);
                                break;

                            case "string":
                                bytes = Encoding.UTF8.GetBytes(row[j].ToString());
                                fs.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                                fs.Write(bytes, 0, bytes.Length);
                                break;
                        }
                    }
                }

                fs.Close();
            }
            Debug.Log($"已生成 {table.TableName} 的Binary数据表和对应的数据结构类以及容器类");
        }

        /// <summary>
        /// 生成 Excel Json数据
        /// </summary>
        /// <param name="table">Excel 数据表</param>
        private void GenerateExcelToJson(DataTable table)
        {
            // 字段名行
            DataRow rowName = GetVariableNameRow(table);
            // 字段类型行
            DataRow rowType = GetVariableTypeRow(table);

            tempString = new StringBuilder(dataJsonPathProperty.stringValue);

            if (tempString[tempString.Length - 1] != '/')
            {
                tempString.Append("/");
            }

            string dataJsonPath = tempString.ToString();
            tempString = null;

            // 判断路径文件夹是否存在，不存在则创建
            if (!Directory.Exists(dataJsonPath))
            {
                Directory.CreateDirectory(dataJsonPath);
            }
            DataRow row;
            string str = "[\n";

            for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
            {
                row = table.Rows[i];
                str += "\t{\n";
                // TODO:添加对应字段读写规则(Json)
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (rowType[j].ToString() == "int")
                    {
                        str += "\t\"" + rowName[j].ToString() + "\": " + (String.IsNullOrEmpty(row[j].ToString()) ? "0" : int.Parse(row[j].ToString())); // row[j].ToString();
                    }
                    else if (rowType[j].ToString() == "float")
                    {
                        str += "\t\"" + rowName[j].ToString() + "\": " + row[j].ToString(); //+ "";
                    }
                    else if (rowType[j].ToString() == "bool")
                    {
                        str += "\t\"" + rowName[j].ToString() + "\": " + row[j].ToString(); //+ "";
                    }
                    else if (rowType[j].ToString() == "string")
                    {
                        str += "\t\"" + rowName[j].ToString() + "\": \"" + row[j].ToString() + "\"";
                    }
                    if (j < table.Columns.Count - 1)
                    {
                        str += ",\n";
                    }
                    else
                    {
                        str += "\n";
                    }
                }
                if (i < table.Rows.Count - 1)
                {
                    str += "\t},\n";
                }
                else
                {
                    str += "\t}\n";
                }
            }

            str += "]";

            File.WriteAllText(dataJsonPath + table.TableName + ".json", str);

            Debug.Log($"已生成 {table.TableName} 的Json数据表和对应的数据结构类");
        }

        /// <summary>
        /// 获取变量名所在行
        /// </summary>
        /// <param name="table">数据表</param>
        /// <param name="index">变量名所在行索引  默认第1行</param>
        /// <returns></returns>
        private DataRow GetVariableNameRow(DataTable table, int index = BEGIN_VARIABLE_NAME_INDEX)
        {
            return table.Rows[index];
        }

        /// <summary>
        /// 获取变量类型所在行
        /// </summary>
        /// <param name="table">数据表</param>
        /// <param name="index">变量类型所在行索引 默认第2行</param>
        /// <returns></returns>
        private DataRow GetVariableTypeRow(DataTable table, int index = BEGIN_VARIABLE_TYPE_INDEX)
        {
            return table.Rows[index];
        }

        /// <summary>
        /// 获取主键所在行索引
        /// </summary>
        /// <param name="table">数据表</param>
        /// <param name="index">主键所在行  默认第3行</param>
        /// <returns></returns>
        private int GetKeyIndex(DataTable table, int index = BEGIN_KEY_INDEX)
        {
            // 主键所在行
            DataRow row = table.Rows[index];

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (row[i].ToString() == "key")
                {
                    return i;
                }
            }

            return 0;
        }

        /// <summary>
        /// 获取变量描述所在行
        /// </summary>
        /// <param name="table">数据表</param>
        /// <param name="index">变量描述所在行索引  默认第4行</param>
        /// <returns></returns>
        private DataRow GetVariableDescriptionRow(DataTable table, int index = BEGIN_DESCRIPTION_INDEX)
        {
            return table.Rows[index];
        }
    }
}