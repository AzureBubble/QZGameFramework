using QZGameFramework.PersistenceDataMgr;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道具背包
/// </summary>
public class Inventory
{
    /// <summary>
    /// 道具存储列表
    /// </summary>
    private List<BaseItemInfo> items = new List<BaseItemInfo>();

    /// <summary>
    /// 添加道具
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(BaseItemInfo item)
    {
        if (items.Contains(item) && item.canStack)
        {
            foreach (BaseItemInfo baseItem in items)
            {
                if (baseItem.name == item.name)
                {
                    ++baseItem.num;
                    break;
                }
            }
        }
        else
        {
            items.Add(item);
        }
    }

    /// <summary>
    /// 删除道具
    /// </summary>
    /// <param name="item"></param>
    public void RemoveItem(BaseItemInfo item)
    {
        if (items.Contains(item) && item.canStack)
        {
            if (item.num == 1)
            {
                item.num = 0;
                items.Remove(item);
            }
            else
            {
                foreach (BaseItemInfo baseItem in items)
                {
                    if (baseItem.name == item.name)
                    {
                        --baseItem.num;
                        break;
                    }
                }
            }
        }
        else
        {
            items.Remove(item);
        }
    }

    /// <summary>
    /// 根据道具名称 获取背包中的道具信息
    /// </summary>
    /// <param name="itemName">道具名字</param>
    /// <returns></returns>
    public BaseItemInfo GetItem(string itemName)
    {
        foreach (BaseItemInfo item in items)
        {
            if (item.name == itemName)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// 根据列表中的index 获取背包中的道具信息
    /// </summary>
    /// <param name="index">道具在列表中的index</param>
    /// <returns></returns>
    public BaseItemInfo GetItem(int index)
    {
        if (index < 0 && index >= items.Count)
        {
            Debug.Log("道具索引超出列表范围");
            return null;
            //throw new ArgumentOutOfRangeException("index");
        }

        return items[index];
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