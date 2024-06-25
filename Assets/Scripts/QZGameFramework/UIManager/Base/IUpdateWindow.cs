namespace QZGameFramework.UIManager
{
    /// <summary>
    /// 窗口类帧更新接口
    /// </summary>
    public interface IUpdateWindow
    {
        /// <summary>
        /// 更新函数 与Mono Update一致
        /// </summary>
        void OnUpdate();
    }
}