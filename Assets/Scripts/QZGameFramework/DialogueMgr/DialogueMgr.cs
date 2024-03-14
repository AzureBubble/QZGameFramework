using System.Collections.Generic;

/// <summary>
/// 对话系统管理器
/// </summary>
public class DialogueMgr : Singleton<DialogueMgr>
{
    /// <summary>
    /// 存储所有对话的字典 key:pieceId value:BasePiece
    /// </summary>
    private Dictionary<string, BasePiece> pieces = new Dictionary<string, BasePiece>();

    private bool isInit;

    public override void Initialize()
    {
        if (isInit) return;

        isInit = true;

        // 在这里加载配置表对话数据到字典中
        //pieces = BinaryDataMgr.Instance.GetTable<BasePiece>();
    }

    /// <summary>
    /// 得到对应的对话内容
    /// </summary>
    /// <param name="pieceId">对话的Id</param>
    /// <returns></returns>
    public BasePiece GetPiece(string pieceId)
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
    public void AddPiece(BasePiece piece)
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
            if (pieces[pieceId].options.Length > 0)
                return true;

        return false;
    }

    public override void Dispose()
    {
        if (IsDisposed) return;
        pieces.Clear();
        isInit = false;
        base.Dispose();
    }
}