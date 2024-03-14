namespace QZGameFramework.AchieveManager
{
    /// <summary>
    /// 成就基类
    /// </summary>
    public abstract class BaseAchievement
    {
        /// <summary>
        /// 是否获得成就
        /// </summary>
        public bool IsEarn { get; set; } = false;

        /// <summary>
        /// 更新成就信息
        /// </summary>
        public abstract void Update();
    }
}