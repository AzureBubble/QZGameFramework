using QZGameFramework.Utilities;
using QZGameFramework.Utilities.EncryptionTool;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace QZGameFramework.PersistenceDataMgr
{
    /// <summary>
    /// 数据加密类型
    /// </summary>
    public enum Encryption_Type
    {
        /// <summary>
        /// 无加密
        /// </summary>
        None,

        /// <summary>
        /// AES 加密
        /// </summary>
        AES,

        /// <summary>
        /// 异或加密
        /// </summary>
        XOR,
    }

    public class BinaryDataMgr : Singleton<BinaryDataMgr>
    {
        /// <summary>
        /// 默认存储路径
        /// </summary>
        private readonly string PERSISTENT_DATA_BINARY_PATH = Application.persistentDataPath + "/SAVE DATA/Binary Data/";

        /// <summary>
        /// 异或加密键
        /// </summary>
        private readonly byte key = 121;

        private readonly string aesKey = "sztu";

        /// <summary>
        /// 二进制数据存储路径
        /// </summary>
        private readonly string DATA_BINARY_PATH = Application.streamingAssetsPath + "/Binary/";

        /// <summary>
        /// 用于存储所有 Excel 表数据的容器 键：表名
        /// </summary>
        private Dictionary<string, TableContainer> tableDic = new Dictionary<string, TableContainer>();

        private bool IsInit = false;

        public override void Initialize()
        {
            base.Initialize();
            InitData();
        }

        /// <summary>
        /// 加载数据配置文件，初始化数据
        /// </summary>
        private void InitData()
        {
            // 避免重复初始化数据
            if (IsInit)
            {
                return;
            }

            LoadAllTable();

            //TODO: 需要在这里对游戏数据进行初始化

            IsInit = true;
        }

        /// <summary>
        /// 加载所有的数据表
        /// </summary>
        public void LoadAllTable()
        {
            List<string> dataClassNames = StringConvert.StringToValue(File.ReadAllText(DATA_BINARY_PATH + "AllDataClassName.txt"), ',');
            for (int i = 0; i < dataClassNames.Count; i++)
            {
                Type dataClass = Type.GetType(dataClassNames[i]);
                Type tableContainer = Type.GetType($"{dataClassNames[i]}Container");
                typeof(BinaryDataMgr).GetMethod("LoadTable").MakeGenericMethod(dataClass, tableContainer).Invoke(Instance, null);
            }
        }

        /// <summary>
        /// 加载 Excel 表的二进制数据到内存中
        /// </summary>
        /// <typeparam name="T">数据结构体类名</typeparam>
        /// <typeparam name="K">容器类类名</typeparam>
        public void LoadTable<T, K>()
        {
            // 通过数据结构体类名找到二进制数据文件
            string savePath = DATA_BINARY_PATH + typeof(T).Name + ".sav";
            // 判断是否存在对应的二进制数据文件
            if (!File.Exists(savePath))
            {
                Debug.LogWarning("未找到对应的数据结构体类的二进制数据表");
                return;
            }

            byte[] bytes; // 数据缓存字节数组
            int index = 0; // 读取数据记录指针

            // 使用FileStream打开二进制数据文件以进行读取
            using (FileStream fs = File.Open(savePath, FileMode.Open, FileAccess.Read))
            {
                // 把数据文件一次性读取成字节数组
                bytes = new byte[fs.Length];
                fs.Read(bytes, 0, bytes.Length);
                fs.Close();
            }

            // 读取记录总数
            int count = BitConverter.ToInt32(bytes, index);
            index += 4;
            // 读取主键名称的长度
            int keyNameLength = BitConverter.ToInt32(bytes, index);
            index += 4;
            // 读取主键名称
            string keyName = Encoding.UTF8.GetString(bytes, index, keyNameLength);
            index += keyNameLength;

            // 获取数据结构类的Type
            Type classType = typeof(T);
            // 获取数据结构类的所有字段信息
            FieldInfo[] infos = classType.GetFields();

            // 使用反射创建容器类对象
            Type containerType = typeof(K);
            // 使用反射实例化一个容器类对象
            object containerObj = Activator.CreateInstance(containerType);

            // 逐行读取数据
            for (int i = 0; i < count; i++)
            {
                // 使用反射实例化一个数据结构类对象
                object dataObj = Activator.CreateInstance(classType);
                foreach (FieldInfo info in infos)
                {
                    var value = ReadFieldValue(bytes, ref index, info.FieldType);
                    info.SetValue(dataObj, value);
                }

                // 获取容器类中的dataDic字段
                object dataDicObj = containerType.GetField("dataDic", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(containerObj);

                // 获取dataDic的Add方法
                MethodInfo addInfo = dataDicObj.GetType().GetMethod("Add");

                // 获取数据对象的主键值
                object keyValue = classType.GetField(keyName).GetValue(dataObj);
                // 调用Add方法将数据对象添加到dataDic中
                addInfo.Invoke(dataDicObj, new object[] { keyValue, dataObj });
            }
            // 将容器对象添加到tableDic中，使用容器对象类的名称作为键
            tableDic.Add(typeof(K).Name, new TableContainer<K>(containerObj));
        }

        private object ReadFieldValue(byte[] bytes, ref int index, Type fieldType)
        {
            //TODO:添加对应的类型字段读写规则(Binary)
            if (fieldType == typeof(int))
            {
                // 读取并设置int类型字段的值
                var value = BitConverter.ToInt32(bytes, index);
                index += 4;
                return value;
            }
            else if (fieldType == typeof(float))
            {
                // 读取并设置float类型字段的值
                var value = BitConverter.ToSingle(bytes, index);
                index += 4;
                return value;
            }
            else if (fieldType == typeof(bool))
            {
                // 读取并设置bool类型字段的值
                var value = BitConverter.ToBoolean(bytes, index);
                index += 1;
                return value;
            }
            else if (fieldType == typeof(string))
            {
                // 读取并设置string类型字段的值
                int length = BitConverter.ToInt32(bytes, index);
                index += 4;
                var value = Encoding.UTF8.GetString(bytes, index, length);
                index += length;
                return value;
            }

            throw new InvalidOperationException("Unsupported field type: " + fieldType);
        }

        /// <summary>
        /// 得到一张表的信息
        /// </summary>
        /// <typeparam name="T">容器类类名</typeparam>
        /// <returns></returns>
        public T GetTable<T>() where T : class
        {
            // 通过反射容器类类名获得表名
            string tableName = typeof(T).Name;
            if (tableDic.ContainsKey(tableName))
            {
                TableContainer<T> table = tableDic[tableName] as TableContainer<T>;
                return table.Data;
            }

            return null;
        }

        /// <summary>
        /// 保存二进制数据
        /// </summary>
        /// <param name="data">数据对象</param>
        /// <param name="fileName">文件名</param>
        /// <param name="type">加密类型</param>
        public void SaveData(object data, string fileName, Encryption_Type type = Encryption_Type.None)
        {
            // 判断是否存在这个文件夹，不存在则创建
            if (!Directory.Exists(PERSISTENT_DATA_BINARY_PATH))
            {
                Directory.CreateDirectory(PERSISTENT_DATA_BINARY_PATH);
            }
            string savePath = PERSISTENT_DATA_BINARY_PATH + "/" + fileName + ".sav";

            switch (type)
            {
                case Encryption_Type.None:
                    // 创建一个文件流
                    using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        // 创建一个二进制格式化器，用于将数据序列化为二进制格式并写入文件
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器将数据对象 data 序列化并写入文件流 fs
                        bf.Serialize(fs, data);
                        fs.Close();
                    }
                    break;

                case Encryption_Type.AES:
                    // 创建一个内存流，用于将数据序列化到内存中
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // 创建一个二进制格式化器，用于将数据序列化到内存流中
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器将数据序列化到内存流中
                        bf.Serialize(ms, data);
                        // 获取序列化后的字节数组
                        byte[] bytes = ms.GetBuffer();

                        ms.Close();

                        bytes = AES.AESEncrypt(bytes, aesKey);

                        // 将加密后的字节数组写入指定的文件
                        File.WriteAllBytes(savePath, bytes);
                    }
                    break;

                case Encryption_Type.XOR:
                    // 创建一个内存流，用于将数据序列化到内存中
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // 创建一个二进制格式化器，用于将数据序列化到内存流中
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器将数据序列化到内存流中
                        bf.Serialize(ms, data);
                        // 获取序列化后的字节数组
                        byte[] bytes = ms.GetBuffer();

                        ms.Close();

                        // 对字节数组中的每个字节执行异或操作，以加密数据
                        for (int i = 0; i < bytes.Length; i++)
                        {
                            bytes[i] ^= key;
                        }
                        // 将加密后的字节数组写入指定的文件
                        File.WriteAllBytes(savePath, bytes);
                    }
                    break;
            }
        }

        /// <summary>
        /// 读取二进制数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="fileName">文件名</param>
        /// <param name="type">加密类型</param>
        /// <returns></returns>
        public T LoadData<T>(string fileName, Encryption_Type type = Encryption_Type.None) where T : class
        {
            T data = default(T);

            string savePath = PERSISTENT_DATA_BINARY_PATH + "/" + fileName + ".sav";

            if (!File.Exists(savePath))
            {
                savePath = Application.streamingAssetsPath + "/" + fileName + ".sav";
                if (!File.Exists(savePath))
                {
                    return data;
                }
            }

            switch (type)
            {
                case Encryption_Type.None:
                    // 创建一个文件流
                    using (FileStream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
                    {
                        // 创建一个二进制格式化器，用于从文件中反序列化数据
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器从文件流 fs 中反序列化数据并将其转换为类型 T
                        data = bf.Deserialize(fs) as T;
                        fs.Close();
                    }
                    break;

                case Encryption_Type.AES:
                    // 从指定的文件中读取加密后的字节数组
                    byte[] aesBytes = File.ReadAllBytes(savePath);
                    aesBytes = AES.AESDecrypt(aesBytes, aesKey);
                    // 创建一个内存流，并将解密后的字节数组写入其中
                    using (MemoryStream ms = new MemoryStream(aesBytes))
                    {
                        // 创建一个二进制格式化器，用于将数据从内存流中反序列化
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器从内存流中反序列化数据，并将其转换为类型 T
                        data = bf.Deserialize(ms) as T;
                        ms.Close();
                    }
                    break;

                case Encryption_Type.XOR:
                    // 从指定的文件中读取加密后的字节数组
                    byte[] bytes = File.ReadAllBytes(savePath);
                    // 对字节数组中的每个字节执行异或操作，以解密数据
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        bytes[i] ^= key;
                    }
                    // 创建一个内存流，并将解密后的字节数组写入其中
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        // 创建一个二进制格式化器，用于将数据从内存流中反序列化
                        BinaryFormatter bf = new BinaryFormatter();
                        // 使用二进制格式化器从内存流中反序列化数据，并将其转换为类型 T
                        data = bf.Deserialize(ms) as T;
                        ms.Close();
                    }
                    break;
            }

            return data;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            tableDic.Clear();
            IsInit = false;
            base.Dispose();
        }
    }

    public class TableContainer
    {
    }

    public class TableContainer<T> : TableContainer
    {
        private T m_data;
        public T Data => m_data;

        public TableContainer(object data)
        {
            m_data = (T)Convert.ChangeType(data, typeof(T));
        }
    }
}