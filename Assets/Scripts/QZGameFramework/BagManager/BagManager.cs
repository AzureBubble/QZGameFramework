using QZGameFramework.PersistenceDataMgr;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具背包
/// </summary>
public class BagManager : Singleton<BagManager>
{
    /// <summary>
    /// 道具存储列表
    /// </summary>
    public List<BaseItemInfo> items = new List<BaseItemInfo>();

    public override void Initialize()
    {
        base.Initialize();
    }

    /// <summary>
    /// 添加道具
    /// </summary>
    /// <param name="item"></param>
    public bool AddItem(BaseItemInfo item)
    {
        if (items.Contains(item) && item.canStack)
        {
            foreach (BaseItemInfo baseItem in items)
            {
                if (baseItem.name == item.name)
                {
                    ++baseItem.num;
                    return true;
                }
            }
        }
        else
        {
            items.Add(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 删除道具
    /// </summary>
    /// <param name="item"></param>
    public bool RemoveItem(BaseItemInfo item)
    {
        if (items.Contains(item) && item.canStack)
        {
            if (item.num == 1)
            {
                item.num = 0;
                items.Remove(item);
                return true;
            }
            else
            {
                foreach (BaseItemInfo baseItem in items)
                {
                    if (baseItem.name == item.name)
                    {
                        --baseItem.num;
                        return true;
                    }
                }
            }
        }
        else if (items.Contains(item))
        {
            items.Remove(item);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 根据道具名称 获取背包中的道具信息
    /// </summary>
    /// <param name="itemName">道具名字</param>
    /// <returns></returns>
    public BaseItemInfo GetItem(string itemName)
    {
        for (int i = items.Count - 1; i >= 0; --i)
        {
            if (items[i].name == itemName)
            {
                return items[i];
            }
        }

        Debug.LogError("背包中不存在道具: " + itemName);
        return null;
    }

    /// <summary>
    /// 根据道具的 id 获取背包中的道具信息
    /// </summary>
    /// <param name="id">道具的id</param>
    /// <returns></returns>
    public BaseItemInfo GetItem(int id)
    {
        for (int i = items.Count - 1; i >= 0; --i)
        {
            if (items[i].id == id)
            {
                return items[i];
            }
        }

        Debug.LogError("背包中不存在道具id: " + id);
        return null;
    }

    /// <summary>
    /// 加载本地背包数据
    /// </summary>
    public void LoadInventory()
    {
        items = BinaryDataMgr.Instance.LoadData<List<BaseItemInfo>>("Inventory");
    }

    /// <summary>
    /// 保存本地背包数据
    /// </summary>
    public void SaveInventory()
    {
        BinaryDataMgr.Instance.SaveData(items, "Inventory");
    }
}