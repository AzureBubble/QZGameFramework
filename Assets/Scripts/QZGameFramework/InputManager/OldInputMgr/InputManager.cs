using QZGameFramework.MonoManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    [Obsolete("建议使用命令模式的InputManager")]
    public interface IInputManager
    {
        void PushStack();

        void PopStack();

        void AddKeyDownListener(KeyCode keyCode, UnityAction action);

        void AddKeyStayListener(KeyCode keyCode, UnityAction action);

        void AddKeyUpListener(KeyCode keyCode, UnityAction action);

        void RemoveKeyDownListener(KeyCode keyCode, UnityAction action);

        void RemoveKeyStayListener(KeyCode keyCode, UnityAction action);

        void RemoveKeyUpListener(KeyCode keyCode, UnityAction action);
    }

    /// <summary>
    /// 旧输入系统总管理者
    /// </summary>
    [Obsolete("建议使用命令模式的InputManager")]
    public class InputManager : Singleton<InputManager>, IInputManager, IUpdateSingleton
    {
        public int Priority => 1;

        // 按键控制栈  如场景一用一套按键，场景二用一套按键
        public Stack<InputController> InputStacks { get; private set; }

        public InputManager()
        {
            InputStacks = new Stack<InputController>();
        }

        public void OnUpdate()
        {
            // 如果没有键盘控制映射，则不执行 Update
            if (InputStacks.Count == 0) return;

            // 创建三个处理不同按键状态的协程
            IEnumerator[] enumerators = {
                KeyDownCoroutine(InputStacks.Peek().KeyDownDic),
                KeyStayCoroutine(InputStacks.Peek().KeyStayDic),
                KeyUpCoroutine(InputStacks.Peek().KeyUpDic),
                };

            // 循环开启协程
            foreach (var enumerator in enumerators)
            {
                SingletonManager.StartCoroutine(enumerator);
            }
        }

        private IEnumerator KeyDownCoroutine(Dictionary<KeyCode, UnityAction> keyDownDic)
        {
            // 检测是否有按键按下
            if (Input.anyKeyDown)
            {
                // 循环字典中的所有键，找到对应的键，执行它的回调函数
                foreach (var keyCode in keyDownDic.Keys)
                {
                    if (Input.GetKeyDown(keyCode))
                    {
                        keyDownDic[keyCode]?.Invoke();
                    }
                }
            }
            yield break;
        }

        private IEnumerator KeyStayCoroutine(Dictionary<KeyCode, UnityAction> keyStayDic)
        {
            // 检测是否有按键按住
            if (Input.anyKey)
            {
                // 循环字典中的所有键，找到对应的键，执行它的回调函数
                foreach (var keyCode in keyStayDic.Keys)
                {
                    if (Input.GetKey(keyCode))
                    {
                        keyStayDic[keyCode]?.Invoke();
                    }
                }
            }
            yield break;
        }

        private IEnumerator KeyUpCoroutine(Dictionary<KeyCode, UnityAction> keyUpDic)
        {
            // 检测是否有按键抬起
            if (!Input.anyKeyDown)
            {
                // 循环字典中的所有键，找到对应的键，执行它的回调函数
                foreach (var keyCode in keyUpDic.Keys)
                {
                    if (Input.GetKeyUp(keyCode))
                    {
                        keyUpDic[keyCode]?.Invoke();
                    }
                }
            }
            yield break;
        }

        /// <summary>
        /// 压入新的输入控制器
        /// </summary>
        public void PushStack()
        {
            InputStacks.Push(new InputController());
        }

        /// <summary>
        /// 弹出输入控制器
        /// </summary>
        public void PopStack()
        {
            if (InputStacks.Count > 0)
            {
                InputStacks.Pop();
            }
        }

        /// <summary>
        /// 根据按键的状态向栈中当前启用控制器添加对应的按键回调函数
        /// </summary>
        /// <param name="keyCode">按键</param>
        /// <param name="action">回调函数</param>
        /// <param name="type">按键状态</param>
        private void AddKeyDownListenerFromLayer(KeyCode keyCode, UnityAction action, E_Press_Type type)
        {
            if (InputStacks.Count > 0)
            {
                InputStacks.Peek().AddKeyCodeListener(keyCode, action, type);
            }
        }

        /// <summary>
        /// 根据按键的状态向栈中当前启用控制器移除对应的按键回调函数
        /// </summary>
        /// <param name="keyCode">按键</param>
        /// <param name="action">回调函数</param>
        /// <param name="type">按键状态</param>
        private void RemoveKeyDownListenerFromLayer(KeyCode keyCode, UnityAction action, E_Press_Type type)
        {
            if (InputStacks.Count > 0)
            {
                InputStacks.Peek().RemoveKeyCodeListener(keyCode, action, type);
            }
        }

        public void AddKeyDownListener(KeyCode keyCode, UnityAction action)
        {
            AddKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Down);
        }

        public void AddKeyStayListener(KeyCode keyCode, UnityAction action)
        {
            AddKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Stay);
        }

        public void AddKeyUpListener(KeyCode keyCode, UnityAction action)
        {
            AddKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Up);
        }

        public void RemoveKeyDownListener(KeyCode keyCode, UnityAction action)
        {
            RemoveKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Down);
        }

        public void RemoveKeyStayListener(KeyCode keyCode, UnityAction action)
        {
            RemoveKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Stay);
        }

        public void RemoveKeyUpListener(KeyCode keyCode, UnityAction action)
        {
            RemoveKeyDownListenerFromLayer(keyCode, action, E_Press_Type.Up);
        }
    }
}