using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace QZGameFramework.Utilities
{
    public static class StringConvert
    {
        /// <summary>
        /// 正则表达式
        /// </summary>
        private static readonly Regex REGEX = new Regex(@"\{[-+]?[0-9]+\.?[0-9]*\}", RegexOptions.IgnoreCase);

        /// <summary>
        /// string 转换为 Bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns>不等于0则为真 反之为假</returns>
        public static bool StringToBool(string value)
        {
            return (int)Convert.ChangeType(value, typeof(bool)) != 0;
        }

        /// <summary>
        /// string 转换成为数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T StringToValue<T>(string value)
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// string 转换成为数值列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static List<T> StringToValue<T>(string value, char separator)
        {
            List<T> list = new List<T>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] splits = value.Split(separator);
                foreach (string split in splits)
                {
                    list.Add((T)Convert.ChangeType(split, typeof(T)));
                }
            }
            return list;
        }

        /// <summary>
        /// string 转换成为字符串列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static List<string> StringToValue(string value, char separator)
        {
            List<string> list = new List<string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] splits = value.Split(separator);
                foreach (string split in splits)
                {
                    list.Add((string)Convert.ChangeType(split, typeof(string)));
                }
            }
            return list;
        }

        /// <summary>
        /// /// 转换为枚举
		/// 枚举索引转换为枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T IndexToEnum<T>(string index) where T : IConvertible
        {
            int enumIndex = (int)Convert.ChangeType(index, typeof(int));
            return IndexToEnum<T>(enumIndex);
        }

        /// <summary>
        /// /// 转换为枚举
		/// 枚举索引转换为枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T IndexToEnum<T>(int index) where T : IConvertible
        {
            if (!Enum.IsDefined(typeof(T), index))
            {
                throw new ArgumentException($"Enum {typeof(T)} is not defined index {index}");
            }
            return (T)Enum.ToObject(typeof(T), index);
        }

        /// <summary>
        /// /// 转换为枚举
		/// 枚举名称转换为枚举类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">枚举名</param>
        /// <returns></returns>
		public static T NameToEnum<T>(string name)
        {
            if (Enum.IsDefined(typeof(T), name) == false)
            {
                throw new ArgumentException($"Enum {typeof(T)} is not defined name {name}");
            }
            return (T)Enum.Parse(typeof(T), name);
        }

        /// <summary>
		/// 字符串转换为参数列表
		/// </summary>
		public static List<float> StringToParams(string str)
        {
            List<float> result = new List<float>();
            MatchCollection matches = REGEX.Matches(str);
            for (int i = 0; i < matches.Count; i++)
            {
                string value = matches[i].Value.Trim('{', '}');
                result.Add(StringToValue<float>(value));
            }
            return result;
        }
    }
}