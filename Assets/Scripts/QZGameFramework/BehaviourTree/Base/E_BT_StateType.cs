/// <summary>
/// 行为树枚举
/// </summary>
public enum E_BT_StateType
{
    /// <summary>
    /// 等待
    /// </summary>
    Waiting,

    /// <summary>
    /// 成功
    /// </summary>
    Success,

    /// <summary>
    /// 运行中
    /// </summary>
    Running,

    /// <summary>
    /// 失败
    /// </summary>
    Failure,

    /// <summary>
    /// 终止
    /// </summary>
    Abort,
}