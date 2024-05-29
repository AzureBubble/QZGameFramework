using QZGameFramework.ObjectPoolManager;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.Utilities.UGUIUtil
{
    /// <summary>
    /// 虚拟列表排列方向的类型
    /// </summary>
    public enum VirtualListType
    {
        /// <summary>
        /// 横向
        /// </summary>
        Horzontal,

        /// <summary>
        /// 竖向
        /// </summary>
        Vertical,
    }

    /// <summary>
    /// 格子类必须继承此接口实现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBaseItemInfo<T>
    {
        /// <summary>
        /// 初始化背包格子数据
        /// </summary>
        /// <param name="itemInfo"></param>
        void InitInfo(T itemInfo);
    }

    /// <summary>
    /// UGUI 中的 ScrollView 实现虚拟列表效果
    /// </summary>
    /// <typeparam name="T">虚拟列表的数据来源类</typeparam>
    /// <typeparam name="K">格子类</typeparam>
    public class VirtualList<T, K> where K : IBaseItemInfo<T>
    {
        /// <summary>
        /// ScrollView 中的 Content 对象
        /// </summary>
        private RectTransform _content;

        private VirtualListType _virtualListType;
        private int _itemMaxCount;
        private List<T> _items;

        /// <summary>
        /// 预制体资源路径
        /// </summary>
        private string _itemResPath;

        /// <summary>
        /// 格子预制体名字
        /// </summary>
        private string _itemPrefabName = "Item";

        /// <summary>
        /// 可是范围的高度
        /// </summary>
        private int _viewPortHeight;

        /// <summary>
        /// 可是范围的宽度
        /// </summary>
        private int _viewPortWidth;

        /// <summary>
        /// 当前正在显示中的格子对象
        /// </summary>
        private Dictionary<int, GameObject> _displayItems = new Dictionary<int, GameObject>();

        // 记录上一次显示的索引范围
        private int _oldMinIndex = -1;

        private int _oldMaxIndex = -1;

        // 记录最新显示的索引范围
        private int _minIndex = 0;

        private int _maxIndex = 0;

        /// <summary>
        /// 格子的高度
        /// </summary>
        private int _itemHeight;

        /// <summary>
        /// 格子的宽度
        /// </summary>
        private int _itemWidth;

        /// <summary>
        /// 最大行数的格子数量
        /// </summary>
        private int _rowItemMaxCount;

        /// <summary>
        /// 最大列数的格子数量
        /// </summary>
        private int _colItemMaxCount;

        /// <summary>
        /// 初始化 ScrollView 的 content 对象和可视范围的高度
        /// </summary>
        /// <param name="content">content对象</param>
        /// <param name="ViewPortLength">viewPort的高度/宽度</param>
        /// <param name="maxCount">一行/一列格子最大数量</param>
        /// <param name="itemSize">格子的大小</param>
        /// <param name="itemResPath">格子预制体资源路径</param>
        /// <param name="type">虚拟列表的排列类型</param>
        /// <param name="itemName">格子预制体名字</param>
        public VirtualList(RectTransform content, int ViewPortLength, int maxCount, Vector2Int itemSize, string itemResPath = "UI/Prefabs", VirtualListType type = VirtualListType.Vertical, string itemName = "Item")
        {
            _content = content;
            _virtualListType = type;
            _itemResPath = itemResPath;
            _itemPrefabName = itemName;
            switch (_virtualListType)
            {
                case VirtualListType.Horzontal:
                    _viewPortWidth = ViewPortLength;
                    _rowItemMaxCount = maxCount;
                    break;

                case VirtualListType.Vertical:
                    _viewPortHeight = ViewPortLength;
                    _colItemMaxCount = maxCount;
                    break;
            }
            _itemWidth = itemSize.x;
            _itemHeight = itemSize.y;
        }

        /// <summary>
        /// 设置虚拟列表的数据来源
        /// </summary>
        /// <param name="items">虚拟列表的数据</param>
        /// <param name="interval">格子间的间隔</param>
        public void InitContentInfo(List<T> items, Vector2Int interval = default)
        {
            _itemWidth += interval.x;
            _itemHeight += interval.y;
            _items = items;
            _itemMaxCount = items.Count;

            int maxNum = 0;
            PoolObjCountAttribute poolObjAttr = (PoolObjCountAttribute)System.Attribute.GetCustomAttribute(typeof(K), typeof(PoolObjCountAttribute));
            switch (_virtualListType)
            {
                case VirtualListType.Horzontal:
                    _content.sizeDelta = new Vector2(Mathf.CeilToInt(_itemMaxCount / _rowItemMaxCount) * _itemWidth, 0);

                    if (poolObjAttr != null)
                    {
                        _minIndex = (int)(-_content.anchoredPosition.x / _itemHeight) * _rowItemMaxCount;
                        _maxIndex = (int)((-_content.anchoredPosition.x + _viewPortWidth) / _itemWidth) * _rowItemMaxCount + _rowItemMaxCount - 1;
                        maxNum = _maxIndex - _minIndex + 1 + _rowItemMaxCount;
                    }
                    break;

                case VirtualListType.Vertical:
                    _content.sizeDelta = new Vector2(0, Mathf.CeilToInt(_itemMaxCount / _colItemMaxCount) * _itemHeight);

                    if (poolObjAttr != null)
                    {
                        _minIndex = (int)(_content.anchoredPosition.y / _itemHeight) * _colItemMaxCount;
                        _maxIndex = (int)((_content.anchoredPosition.y + _viewPortHeight) / _itemHeight) * _colItemMaxCount + _colItemMaxCount - 1;
                        maxNum = _maxIndex - _minIndex + 1 + _colItemMaxCount;
                    }
                    break;
            }

            if (maxNum > poolObjAttr.MaxNum)
            {
                Debug.Log("虚拟列表: " + typeof(T).Name + " 所需要的最小对象池数量为: " + maxNum);
            }
        }

        /// <summary>
        /// 更新虚拟列表内容
        /// </summary>
        public void UpdateContent()
        {
            //检测哪些格子应该显示出来
            switch (_virtualListType)
            {
                case VirtualListType.Horzontal:
                    _minIndex = (int)(-_content.anchoredPosition.x / _itemHeight) * _rowItemMaxCount;
                    _maxIndex = (int)((-_content.anchoredPosition.x + _viewPortWidth) / _itemWidth) * _rowItemMaxCount + _rowItemMaxCount - 1;
                    break;

                case VirtualListType.Vertical:
                    _minIndex = (int)(_content.anchoredPosition.y / _itemHeight) * _colItemMaxCount;
                    _maxIndex = (int)((_content.anchoredPosition.y + _viewPortHeight) / _itemHeight) * _colItemMaxCount + _colItemMaxCount - 1;
                    break;
            }

            // 优化少许 Update 性能
            if (_oldMinIndex == _minIndex || _oldMaxIndex == _maxIndex)
            {
                return;
            }

            //最小值判断
            if (_minIndex < 0)
                _minIndex = 0;

            //超出道具最大数量
            if (_maxIndex >= _itemMaxCount)
                _maxIndex = _itemMaxCount - 1;

            if (_minIndex != _oldMinIndex ||
                _maxIndex != _oldMaxIndex)
            {
                //在记录当前索引之前 要做一些事儿
                //根据上一次索引和这一次新算出来的索引 用来判断 哪些该移除
                //删除上一节溢出
                for (int i = _oldMinIndex; i < _minIndex; ++i)
                {
                    if (_displayItems.ContainsKey(i))
                    {
                        if (_displayItems[i] != null)
                            PoolMgr.Instance.ReleaseQueueObj(_itemPrefabName, _displayItems[i]);
                        _displayItems.Remove(i);
                    }
                }
                //删除下一节溢出
                for (int i = _maxIndex + 1; i <= _oldMaxIndex; ++i)
                {
                    if (_displayItems.ContainsKey(i))
                    {
                        if (_displayItems[i] != null)
                            PoolMgr.Instance.ReleaseQueueObj(_itemPrefabName, _displayItems[i]);
                        _displayItems.Remove(i);
                    }
                }
            }

            _oldMinIndex = _minIndex;
            _oldMaxIndex = _maxIndex;

            //创建指定索引范围内的格子
            for (int i = _minIndex; i <= _maxIndex; ++i)
            {
                if (_displayItems.ContainsKey(i))
                    continue;
                else
                {
                    //根据这个关键索引 用来设置位置 初始化道具信息
                    _displayItems.Add(i, null);
                    GameObject obj = PoolMgr.Instance.GetQueueObj(_itemPrefabName, _itemResPath);

                    //当格子创建出来后我们要做什么
                    //设置它的父对象
                    obj.transform.SetParent(_content);
                    //重置相对缩放大小
                    obj.transform.localScale = Vector3.one;
                    //重置位置
                    switch (_virtualListType)
                    {
                        case VirtualListType.Horzontal:
                            obj.transform.localPosition = new Vector3(i / _rowItemMaxCount * _itemWidth, -(i % _rowItemMaxCount) * _itemHeight, 0);
                            break;

                        case VirtualListType.Vertical:
                            obj.transform.localPosition = new Vector3((i % _colItemMaxCount) * _itemWidth, -i / _colItemMaxCount * _itemHeight, 0);
                            break;
                    }
                    //更新格子信息
                    if (obj.TryGetComponent<K>(out K component))
                    {
                        component?.InitInfo(_items[i]);
                    }
                    else
                    {
                        Debug.LogError("虚拟列表中的格子类对象必须继承 IBaseItemInfo 接口");
                    }

                    //判断有没有这个坑
                    if (_displayItems.ContainsKey(i))
                        _displayItems[i] = obj;
                    else
                        PoolMgr.Instance.ReleaseQueueObj(_itemPrefabName, obj);
                }
            }
        }
    }
}