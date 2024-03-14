using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 按键的三种类型状态
    /// </summary>
    public enum E_Press_Type
    {
        /// <summary>
        /// 按键按下
        /// </summary>
        Down,

        /// <summary>
        /// 按键按住
        /// </summary>
        Stay,

        /// <summary>
        /// 按键抬起
        /// </summary>
        Up,
    }

    /// <summary>
    /// 按键管理
    /// 负责按键的注册和回调函数存储
    /// </summary>
    [Obsolete("建议使用命令模式的InputManager")]
    public class InputController
    {
        // 存储按键按下回调函数字典
        public Dictionary<KeyCode, UnityAction> KeyDownDic { get; private set; }

        // 存储按键按住回调函数字典
        public Dictionary<KeyCode, UnityAction> KeyStayDic { get; private set; }

        // 存储按键抬起回调函数字典
        public Dictionary<KeyCode, UnityAction> KeyUpDic { get; private set; }

        public InputController()
        {
            KeyDownDic = new Dictionary<KeyCode, UnityAction>();
            KeyStayDic = new Dictionary<KeyCode, UnityAction>();
            KeyUpDic = new Dictionary<KeyCode, UnityAction>();
        }

        /// <summary>
        /// 根据按键的操作状态添加对应的事件到对应的字典中
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="action">回调函数</param>
        /// <param name="type">类型</param>
        public void AddKeyCodeListener(KeyCode keyCode, UnityAction action, E_Press_Type type)
        {
            Dictionary<KeyCode, UnityAction> dic = GetDicByPressType(type);

            if (dic.ContainsKey(keyCode))
            {
                dic[keyCode] += action;
                return;
            }
            dic.Add(keyCode, action);
        }

        /// <summary>
        /// 根据按键的操作状态移除对应的事件到对应的字典中
        /// </summary>
        /// <param name="keyCode"></param>
        /// <param name="action">回调函数</param>
        /// <param name="type">类型</param>
        public void RemoveKeyCodeListener(KeyCode keyCode, UnityAction action, E_Press_Type type)
        {
            Dictionary<KeyCode, UnityAction> dic = GetDicByPressType(type);

            if (dic.ContainsKey(keyCode))
            {
                dic[keyCode] -= action;
                return;
            }

            if (dic[keyCode] == null)
            {
                dic.Remove(keyCode);
            }
        }

        public Dictionary<KeyCode, UnityAction> GetDicByPressType(E_Press_Type type)
        {
            switch (type)
            {
                case E_Press_Type.Down:
                    return KeyDownDic;

                case E_Press_Type.Stay:
                    return KeyStayDic;

                case E_Press_Type.Up:
                    return KeyUpDic;

                default:
                    return null;
            }
        }
    }
}