using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace QZGameFramework.PersistenceDataMgr
{
    /// <summary>
    /// 重写XML可序列化的Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [Obsolete("建议使用Binary进行数据持久化")]
    public class SerizlizerDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            // 键值对的xml翻译器
            XmlSerializer kSer = new XmlSerializer(typeof(TKey));
            XmlSerializer vSer = new XmlSerializer(typeof(TValue));

            // 跳过根结点
            reader.Read();
            // 当前结点不为结束结点,对键值对进行反序列化
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                TKey key = (TKey)kSer.Deserialize(reader);
                TValue value = (TValue)vSer.Deserialize(reader);
                // 存入字典中
                this.Add(key, value);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            // 键值对的xml翻译器
            XmlSerializer kSer = new XmlSerializer(typeof(TKey));
            XmlSerializer vSer = new XmlSerializer(typeof(TValue));
            // 遍历本字典中的所有键值对进行序列化
            foreach (KeyValuePair<TKey, TValue> kv in this)
            {
                kSer.Serialize(writer, kv.Key);
                vSer.Serialize(writer, kv.Value);
            }
        }
    }
}