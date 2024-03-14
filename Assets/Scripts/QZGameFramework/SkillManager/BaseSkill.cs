/// <summary>
/// 技能基类
/// </summary>
[System.Serializable]
public abstract class BaseSkill
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public int id;

    /// <summary>
    /// 技能名称
    /// </summary>
    public string name;

    /// <summary>
    /// 技能图标
    /// </summary>
    public string imgRes;

    /// <summary>
    /// 技能消耗
    /// </summary>
    public int cost;

    /// <summary>
    /// 技能冷却时间
    /// </summary>
    public float coolTime;

    /// <summary>
    /// 技能效果
    /// </summary>
    public string effRes;

    /// <summary>
    /// 技能描述
    /// </summary>
    public string tips;

    /// <summary>
    /// 技能具体实现
    /// </summary>
    public abstract void Cast();
}