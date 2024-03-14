/// <summary>
/// 抽象基类 对话选项
/// </summary>
[System.Serializable]
public abstract class BaseOption
{
    /// <summary>
    /// 选项ID
    /// </summary>
    [UnityEngine.HideInInspector] public int id;

    /// <summary>
    /// 跳转ID
    /// </summary>
    public string targetID;

    /// <summary>
    /// 选项内容
    /// </summary>
    public string text;

    /// <summary>
    /// 是否接受任务
    /// </summary>
    public bool takeQuest;
}