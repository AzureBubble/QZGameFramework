using UnityEngine;
using UnityEngine.Serialization;

[ScriptableObjectPath("ProjectSettings/ExcelTool.asset")]
public class ExcelToolScriptableObject : ScriptableObjectSingleton<ExcelToolScriptableObject>
{
    [Header("Excel文件存放路径")]
    public string excelPath = "Assets/ArtRes/Excel/";

    [FormerlySerializedAs("DATA_CLASS_PATH")]
    [Header("数据结构类存储路径")]
    public string dataClassPath = "Assets/Scripts/ExcelData/DataClass/";

    [FormerlySerializedAs("DATA_CONTAINER_PATH")]
    [Header("数据容器类存储路径")]
    public string dataContainerPath = "Assets/Scripts/ExcelData/Container/";

    [FormerlySerializedAs("DATA_BINARY_PATH")]
    [Header("二进制数据存储路径")]
    public string dataBinaryPath = "Assets/StreamingAssets/Binary/";

    [FormerlySerializedAs("DATA_JSON_PATH")]
    [Header("Json数据存储路径")]
    public string dataJsonPath = "Assets/StreamingAssets/Json/";
}