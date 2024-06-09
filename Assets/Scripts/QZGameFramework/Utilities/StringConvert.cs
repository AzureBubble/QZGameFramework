using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

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
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            return (int)Convert.ChangeType(value, typeof(int)) != 0;
        }

        /// <summary>
        /// string 转换成为数值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T StringToValue<T>(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// string 转换成为数值数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static T[] StringToValueArray<T>(string value, char separator)
        {
            T[] arr = default(T[]);
            if (!string.IsNullOrEmpty(value))
            {
                string[] splits = value.Split(separator);
                arr = new T[splits.Length];

                for (int i = 0; i < splits.Length; i++)
                {
                    arr[i] = (T)Convert.ChangeType(splits[i], typeof(T));
                }
            }
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }
            return arr;
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
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
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
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }
            return list;
        }

        /// <summary>
        /// string 进行二次切割
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static List<T[]> StringToValueList<T>(string value, char separator1, char separator2)
        {
            List<T[]> result = new List<T[]>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] splites = value.Split(separator2);
                T[] values;
                for (int i = 0; i < splites.Length; i++)
                {
                    values = StringToValueArray<T>(splites[i], separator1);
                    result.Add(values);
                }
            }
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }
            return result;
        }

        /// <summary>
        /// string 进行二次切割
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToValueDictionary(string value, char separator1, char separator2)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] splites = value.Split(separator2);
                string[] values;
                for (int i = 0; i < splites.Length; i++)
                {
                    values = StringToValueArray<string>(splites[i], separator1);
                    result.Add(values[0], values[1]);
                }
            }
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }
            return result;
        }

        /// <summary>
        /// string 进行二次切割
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static Dictionary<T, K> StringToValueDictionary<T, K>(string value, char separator1, char separator2)
        {
            Dictionary<T, K> result = new Dictionary<T, K>();

            if (!string.IsNullOrEmpty(value))
            {
                string[] splites = value.Split(separator2);
                string[] values;
                for (int i = 0; i < splites.Length; i++)
                {
                    values = StringToValueArray<string>(splites[i], separator1);
                    result.Add((T)Convert.ChangeType(values[0], typeof(T)), (K)Convert.ChangeType(values[1], typeof(K)));
                }
            }
            else
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }
            return result;
        }

        #region 转枚举

        /// <summary>
        /// 转换为枚举
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
        /// 转换为枚举
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
        /// 转换为枚举
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

        #endregion

        #region 字符串转坐标

        /// <summary>
        /// string 转换成为二维坐标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static Vector2 StringToValueVector2(string value, char separator)
        {
            float[] arr = StringToValueArray<float>(value, separator);
            return new Vector2(arr[0], arr[1]);
        }

        /// <summary>
        /// string 转换成为二维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static Vector2[] StringToValueVector2Arr(string value, char separator1, char separator2)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            string[] strArr = value.Split(separator2);
            float[] arr;
            Vector2[] result = new Vector2[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                arr = StringToValueArray<float>(strArr[i], separator1);
                result[i] = new Vector2(arr[0], arr[1]);
            }
            return result;
        }

        /// <summary>
        /// string 转换成为二维坐标列表
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static List<Vector2> StringToValueVector2List(string value, char separator1, char separator2)
        {
            Vector2[] vector2s = StringToValueVector2Arr(value, separator1, separator2);
            List<Vector2> result = new List<Vector2>();
            if (vector2s.Length > 0)
            {
                for (int i = 0; i < vector2s.Length; i++)
                {
                    result.Add(vector2s[i]);
                }
            }

            return result;
        }

        /// <summary>
        /// string 转换成为三位坐标
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static Vector3 StringToValueVector3(string value, char separator)
        {
            float[] arr = StringToValueArray<float>(value, separator);
            return new Vector3(arr[0], arr[1], arr[2]);
        }

        /// <summary>
        /// string 转换成为三维坐标数组
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static Vector3[] StringToValueVector3Arr(string value, char separator1, char separator2)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            string[] strArr = value.Split(separator2);
            float[] arr;
            Vector3[] result = new Vector3[strArr.Length];
            for (int i = 0; i < strArr.Length; i++)
            {
                arr = StringToValueArray<float>(strArr[i], separator1);
                result[i] = new Vector3(arr[0], arr[1], arr[2]);
            }
            return result;
        }

        /// <summary>
        /// string 转换成为三维坐标列表
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator1">单个数据里元素分隔符</param>
        /// <param name="separator2">数据之间的分隔符</param>
        /// <returns></returns>
        public static List<Vector3> StringToValueVector3List(string value, char separator1, char separator2)
        {
            Vector3[] vector3s = StringToValueVector3Arr(value, separator1, separator2);
            List<Vector3> result = new List<Vector3>();

            if (vector3s.Length > 0)
            {
                for (int i = 0; i < vector3s.Length; i++)
                {
                    result.Add(vector3s[i]);
                }
            }
            return result;
        }

        #endregion

        #region 秒转时间字符串

        /// <summary>
        /// 秒转时间字符串
        /// </summary>
        /// <param name="totalSeconds">总秒数</param>
        /// <param name="egZero">是否忽略显示0</param>
        /// <param name="separators">时 分 秒 分割符</param>
        /// <returns></returns>
        public static string SecondConvertToTimeString(int totalSeconds, bool egZero = false, params string[] separators)
        {
            if (totalSeconds < 0) totalSeconds = 0;

            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            int seconds = totalSeconds % 60;

            if (!egZero)
            {
                if (separators.Length == 0)
                {
                    return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
                }
                else if (separators.Length == 1)
                {
                    return string.Format("{0:D2}{3}{1:D2}{3}{2:D2}", hours, minutes, seconds, separators[0]);
                }
                else if (separators.Length == 2)
                {
                    return string.Format("{0:D2}{3}{1:D2}{4}{2:D2}", hours, minutes, seconds, separators[0], separators[1]);
                }
                else if (separators.Length == 3)
                {
                    return string.Format("{0:D2}{3}{1:D2}{4}{2:D2}{5}", hours, minutes, seconds, separators[0], separators[1], separators[2]);
                }
            }
            else
            {
                if (separators.Length == 0)
                {
                    return string.Format("{0}{1}{2}", hours == 0 ? "" : $"{hours}:", minutes == 0 ? (hours > 0 ? $"0:" : "") : $"{minutes}:", seconds);
                }
                else if (separators.Length == 1)
                {
                    return string.Format("{0}{1}{2}", hours == 0 ? "" : $"{hours}{separators[0]}", minutes == 0 ? (hours > 0 ? $"0{separators[0]}" : "") : $"{minutes}{separators[0]}", seconds);
                }
                else if (separators.Length == 2)
                {
                    return string.Format("{0}{1}{2}", hours == 0 ? "" : $"{hours}{separators[0]}", minutes == 0 ? (hours > 0 ? $"0{separators[1]}" : "") : $"{minutes}{separators[1]}", seconds);
                }
                else if (separators.Length == 3)
                {
                    return string.Format("{0}{1}{2}", hours == 0 ? "" : $"{hours}{separators[0]}", minutes == 0 ? (hours > 0 ? $"0{separators[1]}" : "") : $"{minutes}{separators[1]}", $"{seconds}{separators[2]}");
                }
            }

            return null;
        }

        #endregion

        #region 数值转字符串

        /// <summary>
        /// 整数转为指定长度的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string NumberToString(int value, int length)
        {
            return value.ToString($"D{length}");
        }

        /// <summary>
        /// 浮点数转为指定保留小数点后几位的字符串返回
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length">字符串长度</param>
        /// <returns></returns>
        public static string FloatToDecimalString(float value, int length)
        {
            return value.ToString($"F{length}");
        }

        /// <summary>
        /// 大数值转换成对应的字符串
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string BigValueConvertToString(int value)
        {
            if (value >= 100000000)
            {
                int tempValue = value % 100000000 / 10000000;
                return string.Format("{0}{1}", $"{value / 100000000}亿", tempValue != 0 ? $"{tempValue}千万" : "");
            }
            else if (value >= 10000)
            {
                int tempValue = value % 10000 / 1000;
                return string.Format("{0}{1}", $"{value / 10000}万", tempValue != 0 ? $"{tempValue}千" : "");
            }

            return value.ToString();
        }

        #endregion

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