/// <summary>
/// 需要帧更新的单例
/// </summary>
public interface IUpdateSingleton : ISingleton
{
    int Priority { get; } // 单例优先级

    /// <summary>
    /// 帧更新方法
    /// </summary>
    abstract void OnUpdate();
}