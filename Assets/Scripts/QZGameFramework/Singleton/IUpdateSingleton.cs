/// <summary>
/// 需要帧更新的单例
/// </summary>
public interface IUpdateSingleton : ISingleton
{
    public int Priority { get; } // 单例优先级

    /// <summary>
    /// 帧更新方法
    /// </summary>
    public abstract void OnUpdate();
}