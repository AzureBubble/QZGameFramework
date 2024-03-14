using QZGameFramework.MonoManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 命令模式的输入管理器
    /// </summary>
    public class InputMgr : Singleton<InputMgr>, IUpdateSingleton
    {
        public int Priority => 0;
        private bool canInput; // 是否可输入
        private List<ICommand> commands; // 命令容器
        private Dictionary<KeyCode, List<E_KeyCode_Command_Type>> keyCodes; // 存储已经注册按键
        private Dictionary<int, List<E_KeyCode_Command_Type>> mouseButtons; // 存储已经注册按键
        private Dictionary<string, List<E_KeyCode_Command_Type>> hotKeys; // 存储已经注册按键

        public override void Initialize()
        {
            // 初始化容器
            commands = new List<ICommand>(16);
            keyCodes = new Dictionary<KeyCode, List<E_KeyCode_Command_Type>>(16);
            mouseButtons = new Dictionary<int, List<E_KeyCode_Command_Type>>();
            hotKeys = new Dictionary<string, List<E_KeyCode_Command_Type>>();
        }

        public void OnUpdate()
        {
            // 如果不监听输入 则直接返回 不再监听键盘输入
            if (!canInput) return;

            // 循环遍历列表中的命令 并执行命令的 Excute 方法
            foreach (ICommand command in commands)
            {
                command?.Execute();
            }
        }

        /// <summary>
        /// 注册命令 如果使用了这个方法注册命令 则不要用下面的三个参数进行注册
        /// </summary>
        /// <param name="command"></param>
        public void RegisterCommand(ICommand command)
        {
            // 将命令添加到执行列表中
            commands.Add(command);
        }

        /// <summary>
        /// 取消注册命令 与上面的注册方法配套使用
        /// </summary>
        /// <param name="command"></param>
        public void RemoveCommand(ICommand command)
        {
            if (commands.Contains(command))
            {
                // 从执行列表中移除命令
                commands.Remove(command);
            }
        }

        /// <summary>
        /// 注册键盘按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="keyCode">按键</param>
        /// <param name="action">按键事件</param>
        public void RegisterCommand(KeyCode keyCode, E_KeyCode_Command_Type type, UnityAction action)
        {
            // 是否已经注册对应状态的按键事件
            if (keyCodes.ContainsKey(keyCode) && keyCodes[keyCode].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(type, keyCode, action))
                    {
                        break;
                    }
                }
            }
            else
            {
                // 存在按键 但不存在对应的按键状态
                if (keyCodes.ContainsKey(keyCode))
                {
                    // 加对应的状态
                    keyCodes[keyCode].Add(type);
                }
                else
                {
                    // 不存在按键
                    keyCodes.Add(keyCode, new List<E_KeyCode_Command_Type>(3) { type });
                }

                // 把按键命令注册到列表容器中
                commands.Add(new KeyCodeCommand(keyCode, type, action));
            }
        }

        /// <summary>
        /// 注册鼠标按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="mouseButton">按键</param>
        /// <param name="action">按键事件</param>
        public void RegisterCommand(int mouseButton, E_KeyCode_Command_Type type, UnityAction action)
        {
            // 是否已经注册对应状态的按键事件
            if (mouseButtons.ContainsKey(mouseButton) && mouseButtons[mouseButton].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(type, mouseButton, action))
                    {
                        break;
                    }
                }
            }
            else
            {
                // 存在按键 但不存在对应的按键状态
                if (mouseButtons.ContainsKey(mouseButton))
                {
                    // 加对应的状态
                    mouseButtons[mouseButton].Add(type);
                }
                else
                {
                    // 不存在按键
                    mouseButtons.Add(mouseButton, new List<E_KeyCode_Command_Type>(3) { type });
                }

                // 把按键命令注册到列表容器中
                commands.Add(new MouseButtonCommand(mouseButton, type, action));
            }
        }

        /// <summary>
        /// 注册热键命令
        /// </summary>
        /// <param name="type">热键类型</param>
        /// <param name="keyName">按键</param>
        /// <param name="action">按键事件</param>
        public void RegisterCommand(string keyName, E_KeyCode_Command_Type type, UnityAction<float> action)
        {
            // 是否已经注册对应状态的按键事件
            if (hotKeys.ContainsKey(keyName) && hotKeys[keyName].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(type, keyName, action))
                    {
                        break;
                    }
                }
            }
            else
            {
                // 存在按键 但不存在对应的按键状态
                if (hotKeys.ContainsKey(keyName))
                {
                    // 加对应的状态
                    hotKeys[keyName].Add(type);
                }
                else
                {
                    // 不存在按键
                    hotKeys.Add(keyName, new List<E_KeyCode_Command_Type>(3) { type });
                }

                // 把按键命令注册到列表容器中
                commands.Add(new HotKeyCommand(keyName, type, action));
            }
        }

        /// <summary>
        /// 取消注册键盘按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="keyCode">按键</param>
        /// <param name="action">按键事件</param>
        /// <param name="isRemove">是否删除容器中空事件的按键命令</param>
        public void RemoveCommand(KeyCode keyCode, E_KeyCode_Command_Type type, UnityAction action, bool isRemove = true)
        {
            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (keyCodes.ContainsKey(keyCode) && keyCodes[keyCode].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(type, keyCode, action);

                    if (removeCommand != null)
                    {
                        break;
                    }
                }

                // 如果对应的按键事件已经为 null
                if (isRemove && removeCommand != null)
                {
                    // 删除对应的命令
                    commands.Remove(removeCommand);

                    // 删除字典中对应的按键状态
                    if (keyCodes[keyCode].Count > 1)
                    {
                        keyCodes[keyCode].Remove(type);
                    }
                    else
                    {
                        keyCodes.Remove(keyCode);
                    }
                }
            }
        }

        /// <summary>
        /// 取消注册鼠标按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="mouseButton">按键</param>
        /// <param name="action">按键事件</param>
        /// <param name="isRemove">是否删除容器中空事件的按键命令</param>
        public void RemoveCommand(int mouseButton, E_KeyCode_Command_Type type, UnityAction action, bool isRemove = true)
        {
            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (mouseButtons.ContainsKey(mouseButton) && mouseButtons[key: mouseButton].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(type, mouseButton, action);

                    if (removeCommand != null)
                    {
                        break;
                    }
                }

                // 如果对应的按键事件已经为 null
                if (isRemove && removeCommand != null)
                {
                    // 删除对应的命令
                    commands.Remove(removeCommand);

                    // 删除字典中对应的按键状态
                    if (mouseButtons[mouseButton].Count > 1)
                    {
                        mouseButtons[mouseButton].Remove(type);
                    }
                    else
                    {
                        mouseButtons.Remove(mouseButton);
                    }
                }
            }
        }

        /// <summary>
        /// 取消注册热键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="keyName">按键</param>
        /// <param name="action">按键事件</param>
        /// <param name="isRemove">是否删除容器中空事件的按键命令</param>
        public void RemoveCommand(string keyName, E_KeyCode_Command_Type type, UnityAction<float> action, bool isRemove = true)
        {
            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (hotKeys.ContainsKey(keyName) && hotKeys[key: keyName].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(type, keyName, action);

                    if (removeCommand != null)
                    {
                        break;
                    }
                }

                // 如果对应的按键事件已经为 null
                if (isRemove && removeCommand != null)
                {
                    // 删除对应的命令
                    commands.Remove(removeCommand);

                    // 删除字典中对应的按键状态
                    if (hotKeys[keyName].Count > 1)
                    {
                        hotKeys[keyName].Remove(type);
                    }
                    else
                    {
                        hotKeys.Remove(keyName);
                    }
                }
            }
        }

        /// <summary>
        /// 重新绑定键盘按键事件监听
        /// </summary>
        /// <param name="oldKey">旧的按键</param>
        /// <param name="newKey">新的按键</param>
        public void RebindingCommandKeyCode(KeyCode oldKey, KeyCode newKey)
        {
            foreach (ICommand command in commands)
            {
                if (command.RebindingKeyCode(oldKey, newKey))
                {
                    break;
                }
            }
        }

        /// <summary>
        /// 启动键盘监听
        /// </summary>
        public void Enable()
        {
            canInput = true;
        }

        /// <summary>
        /// 取消键盘监听
        /// </summary>
        public void Disable()
        {
            canInput = false;
        }

        /// <summary>
        /// 清空容器中的命令
        /// </summary>
        private void Clear()
        {
            commands.Clear();
            keyCodes.Clear();
            mouseButtons.Clear();
            hotKeys.Clear();
            canInput = false;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            Clear();
            base.Dispose();
        }
    }
}