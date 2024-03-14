using UnityEngine;

/// <summary>
/// 抽象基类 对话片段
/// </summary>
[System.Serializable]
public abstract class BasePiece
{
    /// <summary>
    /// 片段 pieceId
    /// </summary>
    public int id;

    /// <summary>
    /// 片段 ID 用于寻找
    /// </summary>
    public string pieceId;

    /// <summary>
    /// 头像路径
    /// </summary>
    public string imgRes;

    /// <summary>
    /// 归属名字
    /// </summary>
    public string name;

    /// <summary>
    /// 片段内容
    /// </summary>
    public string text;

    /// <summary>
    /// 选项数组
    /// </summary>
    public BaseOption[] options;
}