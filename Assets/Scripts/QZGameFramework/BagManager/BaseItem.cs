/// <summary>
/// 抽象道具基类
/// </summary>
[System.Serializable]
public abstract class BaseItemInfo
{
    /// <summary>
    /// 道具ID
    /// </summary>
    public int id;

    /// <summary>
    /// 道具名字
    /// </summary>
    public string name;

    /// <summary>
    /// 道具数量
    /// </summary>
    public int num;

    /// <summary>
    /// 是否可堆叠
    /// </summary>
    public bool canStack;

    /// <summary>
    /// 道具图片路径
    /// </summary>
    public string imgRes;

    /// <summary>
    /// 道具特效路径
    /// </summary>
    public string effRes;

    /// <summary>
    /// 道具描述
    /// </summary>
    public string description;

    /// <summary>
    /// 道具具体使用实现
    /// </summary>
    public abstract void Use();
}