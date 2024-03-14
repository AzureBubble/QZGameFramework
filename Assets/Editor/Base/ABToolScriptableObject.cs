using UnityEngine;

[ScriptableObjectPath("ProjectSettings/ABTool.asset")]
public class ABToolScriptableObject : ScriptableObjectSingleton<ABToolScriptableObject>
{
    [Header("远程服务器地址")]
    public string serverIP = "ftp://192.168.168.128/";

    [Header("用户名")]
    public string userName = "root";

    [Header("密码")]
    public string password = "";
}