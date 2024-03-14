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
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                ExcelToolScriptableObject.Save();
            }

            GUILayout.Space(5f);

            // 默认路径生成
            if (GUILayout.Button("读取路径中的所有Excel配置表", GUILayout.Height(30f)))
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
                    // 生成数据结构类
                    GenerateExcelToDataClass(table);
                    // 生成数据容器类
                    GenerateExcelToContainer(table);
                    // 生成二进制数据
                    GenerateExcelToBinary(table);
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

            string str = "public class " + table.TableName + "\n{\n";

            for (int i = 0; i < table.Columns.Count; i++)
            {
                if (rowDescription[i].ToString() != "")
                {
                    str += "\t/// <summary>\n" + "\t/// " + rowDescription[i].ToString() + "\n\t/// </summary>\n";
                }
                str += "\tpublic " + rowType[i].ToString() + " " + rowName[i].ToString() + ";\n";
                if (rowDescription[i].ToString() != "" || rowDescription[i + 1].ToString() != "")
                {
                    if (i < table.Columns.Count - 1)
                    {
                        str += "\n";
                    }
                }
            }

            str += "}";

            File.WriteAllText(dataClassPath + table.TableName + ".cs", str);
        }

        /// <summary>
        /// 生成 Excel 表对应的数据容器类
        /// </summary>
        /// <param name="table">数据表</param>
        private void GenerateExcelToContainer(DataTable table)
        {
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

            string str = "using System.Collections.Generic;\n\n";
            str += "public class " + table.TableName + "Container " + "\n{\n";
            str += "\tpublic Dictionary<" + rowType[keyIndex].ToString() + ", " + table.TableName + ">";
            str += " dataDic = new ();\n";
            str += "}";

            File.WriteAllText(dataContainerPath + table.TableName + "Container.cs", str);
        }

        /// <summary>
        /// 生成 Excel 二进制数据
        /// </summary>
        /// <param name="table">数据表</param>
        private void GenerateExcelToBinary(DataTable table)
        {
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
                // 每一行数据
                for (int i = BEGIN_INDEX; i < table.Rows.Count; i++)
                {
                    row = table.Rows[i];
                    // 每一列数据
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        //TODO:添加对应的类型字段读写规则(Binary)
                        switch (rowType[j].ToString())
                        {
                            case "int":
                                fs.Write(BitConverter.GetBytes(int.Parse(row[j].ToString())), 0, 4);
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
                        str += "\t\"" + rowName[j].ToString() + "\": " + row[j].ToString();
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