/// <summary>
/// 道具工厂类
/// 工厂模式
/// </summary>
public class ItemFactory
{
    /// <summary>
    /// 根据传入道具类型 创建对应的道具
    /// </summary>
    /// <param name="type">道具类型</param>
    /// <returns></returns>
    public static BaseItemInfo CreateItem(BagItemType type)
    {
        switch (type)
        {
            default: return null;
        }
    }
}