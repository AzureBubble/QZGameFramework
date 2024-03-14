using System;

/// <summary>
/// 普通单例
/// </summary>
public interface ISingleton : IDisposable
{
    /// <summary>
    ///  标记单例是否被销毁
    /// </summary>
    public bool IsDisposed { get; set; }

    /// <summary>
    /// 单例初始化方法
    /// </summary>
    public void Initialize();
}