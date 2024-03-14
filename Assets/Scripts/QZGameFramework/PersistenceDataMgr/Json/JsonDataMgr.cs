using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QZGameFramework.PersistenceDataMgr
{
    public enum JsonType
    {
        /// <summary>
        /// Unity自带的json序列化工具
        /// 不可以序列化字典
        /// </summary>
        JsonUtility,

        /// <summary>
        /// Unity官方引入的第三方 NewtonsoftJson 包
        /// 建议使用此序列化工具 可序列化字典
        /// </summary>
        NewtonsoftJson,
    }

    public class JsonDataMgr : Singleton<JsonDataMgr>
    {
        /// <summary>
        /// Json持久化数据存储路径
        /// </summary>
        private readonly string PERSISTENT_DATA_JSON_PATH = Application.persistentDataPath + "/SAVE DATA/Json Data/";

        /// <summary>
        /// Json可读数据存储路径
        /// </summary>
        private readonly string DATA_JSON_PATH = Application.streamingAssetsPath + "/Json/";

        /// <summary>
        /// 用于存储所有 Excel 表数据的容器 键：表名
        /// </summary>
        private Dictionary<string, object> tableDic = new Dictionary<string, object>();

        /// <summary>
        /// 加载数据配置文件，初始化数据
        /// </summary>
        public void InitData()
        {
        }

        /// <summary>
        /// 加载 Excel 表的Json数据到内存中
        /// </summary>
        /// <typeparam name="T">数据结构体类名</typeparam>
        public void LoadTable<T>()
        {
            // 通过数据结构体类名找到二进制数据文件
            string savePath = DATA_JSON_PATH + typeof(T).Name + ".json";
            // 判断是否存在对应的二进制数据文件
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("未找到对应的数据结构体类的Json数据表");
                return;
            }

            string jsonStr = File.ReadAllText(savePath);

            List<T> dataList = new List<T>();
            dataList = JsonConvert.DeserializeObject<List<T>>(jsonStr);

            // 将容器对象添加到tableDic中，使用容器对象类的名称作为键
            tableDic.Add(typeof(T).Name, dataList as object);
        }

        /// <summary>
        /// 得到一张表的信息
        /// </summary>
        /// <typeparam name="T">数据结构类类名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            // 通过反射数据结构类类名获得表名
            string tableName = typeof(T).GenericTypeArguments[0].Name;
            if (tableDic.ContainsKey(tableName))
            {
                return tableDic[tableName] as T;
            }

            return null;
        }

        /// <summary>
        /// 存储Json数据
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="fileName">文件名</param>
        /// <param name="jsonType">json工具类型</param>
        public void SaveData(object data, string fileName, JsonType jsonType = JsonType.NewtonsoftJson)
        {
            if (!Directory.Exists(PERSISTENT_DATA_JSON_PATH))
            {
                Directory.CreateDirectory(PERSISTENT_DATA_JSON_PATH);
            }
            // 存储路径
            string path = PERSISTENT_DATA_JSON_PATH + fileName + ".json";

            string jsonStr = "";

            switch (jsonType)
            {
                case JsonType.JsonUtility:
                    jsonStr = JsonUtility.ToJson(data);
                    break;

                case JsonType.NewtonsoftJson:
                    jsonStr = JsonConvert.SerializeObject(data);
                    break;
            }

            // 把序列化后的字符串存储到路径文件中
            File.WriteAllText(path, jsonStr);
        }

        /// <summary>
        /// 读取存储路径中的指定文件数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="jsonType"></param>
        /// <returns></returns>
        public T LoadData<T>(string fileName, JsonType jsonType = JsonType.NewtonsoftJson) where T : new()
        {
            string path = PERSISTENT_DATA_JSON_PATH + fileName + ".json";
            // 判断两个文件存储路径是否存在数据
            if (!File.Exists(path))
            {
                path = DATA_JSON_PATH + "/" + fileName + ".json";
                if (!File.Exists(path))
                {
                    return new T();
                }
            }
            // 在路径中读取json字符串
            string jsonStr = File.ReadAllText(path);

            // 根据工具类型反序列化json字符串
            switch (jsonType)
            {
                case JsonType.JsonUtility:
                    return JsonUtility.FromJson<T>(jsonStr);

                case JsonType.NewtonsoftJson:
                    return JsonConvert.DeserializeObject<T>(jsonStr);
            }

            return default(T);
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            tableDic.Clear();
            base.Dispose();
        }
    }
}