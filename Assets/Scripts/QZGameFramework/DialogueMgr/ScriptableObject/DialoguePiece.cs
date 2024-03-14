using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialoguePiece
{
    /// <summary>
    /// 片段 pieceId
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
    [TextArea]
    public string text;

    /// <summary>
    /// 是否可以堆叠
    /// </summary>
    [HideInInspector] public bool canExpand;

    /// <summary>
    /// 选项数组
    /// </summary>
    public List<DialogueOption> options = new List<DialogueOption>();
}