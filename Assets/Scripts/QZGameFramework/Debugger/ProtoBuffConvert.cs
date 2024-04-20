using Newtonsoft.Json;
using QZGameFramework.DebuggerSystem;

/// <summary>
/// ProtoBuff 转为字符串 并打印输出日志
/// </summary>
public class ProtoBuffConvert
{
    /// <summary>
    /// ProtoBuff 转为字符串 并打印输出日志
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="proto"></param>
    public static void ToJson<T>(T proto)
    {
        Debugger.Log(JsonConvert.SerializeObject(proto));
    }
}