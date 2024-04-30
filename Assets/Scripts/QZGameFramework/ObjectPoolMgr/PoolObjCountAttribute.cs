using System;

/// <summary>
/// 用于设置对象池中的 GameObject 的最大数量
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class PoolObjCountAttribute : Attribute
{
    /// <summary>
    /// 对象池对象的数量上限
    /// </summary>
    public int MaxNum { get; private set; }

    public PoolObjCountAttribute(int maxNum)
    {
        MaxNum = maxNum;
    }
}