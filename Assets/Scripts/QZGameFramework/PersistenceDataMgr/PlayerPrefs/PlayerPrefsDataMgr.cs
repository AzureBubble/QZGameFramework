using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace QZGameFramework.PersistenceDataMgr
{
    public class PlayerPrefsDataMgr : Singleton<PlayerPrefsDataMgr>
    {
        /// <summary>
        /// 存储数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="keyName">存储键</param>
        public void SaveData(object data, string keyName)
        {
            // 得到传入数据的类型
            Type dataType = data.GetType();
            // 得到传入数据的所有字段
            FieldInfo[] fieldInfos = dataType.GetFields();

            // 自定义存储键的规则进行存储
            string saveKeyName = "";
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                // keyName_类型名_字段类型名_字段名
                saveKeyName = keyName + "_" + dataType.Name + "_" + fieldInfo.FieldType.Name + "_" + fieldInfo.Name;

                // 反射获取字段的值，调用存储方法
                SaveValue(fieldInfo.GetValue(data), saveKeyName);
            }

            // 把数据存储到硬盘
            PlayerPrefs.Save();
        }

        /// <summary>
        /// 存储字段值
        /// </summary>
        /// <param name="field"></param>
        /// <param name="keyName"></param>
        private void SaveValue(object field, string keyName)
        {
            // 获取字段的类型
            Type fieldType = field.GetType();
            // 根据字段的不同类型，调用不同的API进行字段的数据存储
            if (fieldType == typeof(int))
            {
                PlayerPrefs.SetInt(keyName, (int)field);
            }
            else if (fieldType == typeof(float))
            {
                PlayerPrefs.SetFloat(keyName, (float)field);
            }
            else if (fieldType == typeof(string))
            {
                PlayerPrefs.SetString(keyName, field.ToString());
            }
            else if (fieldType == typeof(bool))
            {
                PlayerPrefs.SetInt(keyName, (bool)field ? 1 : 0);
            }
            // 判断List数据，并进行存储
            // 里氏替换原则
            else if (typeof(IList).IsAssignableFrom(fieldType))
            {
                // 把传入数据用父类装载
                IList list = field as IList;
                // 存储List数量
                PlayerPrefs.SetInt(keyName, list.Count);
                int index = 0;
                // 存储具体的List里的值
                foreach (object obj in list)
                {
                    SaveValue(obj, keyName + index);
                    ++index;
                }
            }
            // 处理字典
            else if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                // 把传入数据用父类装载
                IDictionary dic = field as IDictionary;
                // 存储Dic数量
                PlayerPrefs.SetInt(keyName, dic.Count);
                int index = 0;
                // 存储具体的Dic里的值
                foreach (object key in dic.Keys)
                {
                    SaveValue(key, keyName + "_key_" + index);
                    SaveValue(dic[key], keyName + "_value_" + index);
                    ++index;
                }
            }
            // 不是基础数据类型处理
            else
            {
                SaveData(field, keyName);
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        /// <param name="type"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public object LoadData(Type type, string keyName)
        {
            // 根据传入类型 创建对象实例
            object data = Activator.CreateInstance(type);
            // 获取对象中的所有字段
            FieldInfo[] infos = type.GetFields();

            string loadKeyName = "";
            // 读取数据值
            foreach (FieldInfo info in infos)
            {
                loadKeyName = keyName + "_" + type.Name + "_" + info.FieldType.Name + "_" + info.Name;

                info.SetValue(data, LoadValue(info.FieldType, loadKeyName));
            }

            return data;
        }

        /// <summary>
        /// 读取单个字段的值
        /// </summary>
        /// <param name="fieldType"></param>
        /// <param name="loadKeyName"></param>
        /// <returns></returns>
        private object LoadValue(Type fieldType, string loadKeyName)
        {
            if (fieldType == typeof(int))
            {
                return PlayerPrefs.GetInt(loadKeyName, 0);
            }
            else if (fieldType == typeof(float))
            {
                return PlayerPrefs.GetFloat(loadKeyName, 0);
            }
            else if (fieldType == typeof(string))
            {
                return PlayerPrefs.GetString(loadKeyName, "");
            }
            else if (fieldType == typeof(bool))
            {
                return PlayerPrefs.GetInt(loadKeyName, 0) == 1 ? true : false;
            }
            else if (typeof(IList).IsAssignableFrom(fieldType))
            {
                int count = PlayerPrefs.GetInt(loadKeyName, 0);
                IList list = Activator.CreateInstance(fieldType) as IList;
                for (int i = 0; i < count; i++)
                {
                    list.Add(LoadValue(fieldType.GetGenericArguments()[0], loadKeyName + i));
                }
                return list;
            }
            else if (typeof(IDictionary).IsAssignableFrom(fieldType))
            {
                int count = PlayerPrefs.GetInt(loadKeyName, 0);
                IDictionary dic = Activator.CreateInstance(fieldType) as IDictionary;
                for (int i = 0; i < count; i++)
                {
                    dic.Add(LoadValue(fieldType.GetGenericArguments()[0], loadKeyName + "_key_" + i),
                        LoadValue(fieldType.GetGenericArguments()[1], loadKeyName + "_value_" + i));
                }
                return dic;
            }
            else
            {
                return LoadData(fieldType, loadKeyName);
            }
        }
    }
}