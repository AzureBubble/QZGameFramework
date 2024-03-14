using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 如果不想使用配置表 也可直接使用SO文件在Untiy编辑器中直接配置
/// 使用 ScriptableObject 实现的对话系统
/// </summary>
[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData_SO : ScriptableObject
{
    public List<DialoguePiece> dialoguePieces = new List<DialoguePiece>();
    public Dictionary<string, DialoguePiece> pieces = new Dictionary<string, DialoguePiece>();

#if UNITY_EDITOR

    /// <summary>
    /// 在Unity窗口中编辑数据后 就把数据加载到字典中 确保数据与List列表一一对应
    /// </summary>
    private void OnValidate()
    {
        pieces.Clear();
        foreach (DialoguePiece piece in dialoguePieces)
        {
            if (!pieces.ContainsKey(piece.pieceId))
            {
                pieces.Add(piece.pieceId, piece);
            }
        }
    }

#endif

    /// <summary>
    /// 得到对应的对话内容
    /// </summary>
    /// <param name="pieceId">对话的Id</param>
    /// <returns></returns>
    public DialoguePiece GetPiece(string pieceId)
    {
        if (pieces.ContainsKey(pieceId))
        {
            return pieces[pieceId];
        }

        return null;
    }

    /// <summary>
    /// 添加对话片段
    /// </summary>
    /// <param name="piece">对话片段</param>
    public void AddPiece(DialoguePiece piece)
    {
        if (!pieces.ContainsKey(piece.pieceId))
        {
            pieces.Add(piece.pieceId, piece);
        }
    }

    /// <summary>
    /// 删除对话片段
    /// </summary>
    /// <param name="pieceId"></param>
    public void RemovePiece(string pieceId)
    {
        if (pieces.ContainsKey(pieceId))
        {
            pieces.Remove(pieceId);
        }
    }

    /// <summary>
    /// 对话片段是否存在
    /// </summary>
    /// <param name="pieceId"></param>
    /// <returns></returns>
    public bool PieceExist(string pieceId)
    {
        if (pieces.ContainsKey(pieceId))
            return true;

        return false;
    }

    /// <summary>
    /// 对话片段是否存在可选选项
    /// </summary>
    /// <param name="pieceId"></param>
    /// <returns></returns>
    public bool OptionExist(string pieceId)
    {
        if (pieces.ContainsKey(pieceId))
            if (pieces[pieceId].options.Count > 0)
                return true;

        return false;
    }
}