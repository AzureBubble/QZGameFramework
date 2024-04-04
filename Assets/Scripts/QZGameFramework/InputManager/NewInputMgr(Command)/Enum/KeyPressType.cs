/// <summary>
/// 按键的三种状态枚举
/// 热键输入状态 Axis AxisRaw
/// </summary>
public enum KeyPressType
{
    /// <summary>
    /// 按键按下
    /// </summary>
    Down,

    /// <summary>
    /// 按键按住
    /// </summary>
    Stay,

    /// <summary>
    /// 按键抬起
    /// </summary>
    Up,

    /// <summary>
    /// 获取 Axis 值: -1 ~ 1
    /// </summary>
    Axis,

    /// <summary>
    /// 获取 AxisRaw 值: -1 0 1
    /// </summary>
    AxisRaw,
}