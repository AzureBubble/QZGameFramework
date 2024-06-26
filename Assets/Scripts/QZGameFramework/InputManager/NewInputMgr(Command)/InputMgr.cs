using Cysharp.Threading.Tasks;
using QZGameFramework.PersistenceDataMgr;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    public enum KeyType
    {
        Key, Mouse
    }

    public enum HotKeyNameType
    {
        Horizontal, Vertical
    }

    public class KeyMap
    {
        public KeyType Type { get; set; }
        public int MouseButton { get; set; }
        public KeyCode Key { get; set; }
        public KeyPressType PressType { get; set; }
    }

    /// <summary>
    /// 命令模式的输入管理器
    /// </summary>
    public class InputMgr : Singleton<InputMgr>, IUpdateSingleton
    {
        public int Priority => 0;
        private bool canInput; // 是否可输入
        private List<ICommand> commands; // 命令容器

        private Dictionary<KeyCode, List<KeyPressType>> keyCodes; // 存储已经注册按键
        private Dictionary<int, List<KeyPressType>> mouseButtons; // 存储已经注册按键
        private Dictionary<string, List<KeyPressType>> hotKeys; // 存储已经注册按键
        private Dictionary<string, KeyMap> keyMaps = new Dictionary<string, KeyMap>(); // 键盘映射

        public override void Initialize()
        {
            // 初始化容器
            keyMaps = LoadKeyMap();

            commands = new List<ICommand>(16);
            keyCodes = new Dictionary<KeyCode, List<KeyPressType>>(16);
            mouseButtons = new Dictionary<int, List<KeyPressType>>(4);
            hotKeys = new Dictionary<string, List<KeyPressType>>(16);
        }

        public void OnUpdate()
        {
            // 如果不监听输入 则直接返回 不再监听键盘输入
            if (!canInput || commands.Count <= 0) return;

            // 循环遍历列表中的命令 并执行命令的 Excute 方法
            for (int i = 0; i < commands.Count; i++)
            {
                commands[i]?.Execute();
            }
        }

        /// <summary>
        /// 注册键盘按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="keyCode">按键</param>
        /// <param name="action">按键事件</param>
        public void RegisterCommand(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action)
        {
            RegisterCommandTask(keyCode, type, action).Forget();
        }

        private async UniTaskVoid RegisterCommandTask(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action)
        {
            await UniTask.Yield();

            string actionMethodName = action.Method.Name;
            if (keyMaps.Count > 0 && keyMaps.ContainsKey(actionMethodName))
            {
                if (keyMaps[actionMethodName].Type == KeyType.Key)
                {
                    keyCode = keyMaps[actionMethodName].Key;
                    type = keyMaps[actionMethodName].PressType;
                }
            }

            // 是否已经注册对应状态的按键事件
            if (keyCodes.ContainsKey(keyCode) && keyCodes[keyCode].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(keyCode, type, action))
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
                    keyCodes.Add(keyCode, new List<KeyPressType>(3) { type });
                }

                // 把按键命令注册到列表容器中
                commands.Add(new KeyCodeCommand(keyCode, type, action));
            }

            keyMaps[actionMethodName] = new KeyMap()
            {
                Key = keyCode,
                PressType = type,
                Type = KeyType.Key,
            };
        }

        /// <summary>
        /// 注册鼠标按键命令
        /// </summary>
        /// <param name="type">按键的状态</param>
        /// <param name="mouseButton">按键</param>
        /// <param name="action">按键事件</param>
        public void RegisterCommand(int mouseButton, KeyPressType type, UnityAction action)
        {
            RegisterCommandTask(mouseButton, type, action).Forget();
        }

        private async UniTaskVoid RegisterCommandTask(int mouseButton, KeyPressType type, UnityAction action)
        {
            await UniTask.Yield();

            RegisterCommandTask(mouseButton, type, action).Forget();
            // 是否已经注册对应状态的按键事件
            if (mouseButtons.ContainsKey(mouseButton) && mouseButtons[mouseButton].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(mouseButton, type, action))
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
                    mouseButtons.Add(mouseButton, new List<KeyPressType>(3) { type });
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
        public void RegisterCommand(HotKeyNameType keyNameType, KeyPressType type, UnityAction<float> action)
        {
            RegisterCommandTask(keyNameType, type, action).Forget();
        }

        private async UniTaskVoid RegisterCommandTask(HotKeyNameType keyNameType, KeyPressType type, UnityAction<float> action)
        {
            await UniTask.Yield();

            string keyName = keyNameType.ToString();

            // 是否已经注册对应状态的按键事件
            if (hotKeys.ContainsKey(keyName) && hotKeys[keyName].Contains(type))
            {
                foreach (ICommand command in commands)
                {
                    // 已存在 则添加事件
                    if (command.AddListener(keyName, type, action))
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
                    hotKeys.Add(keyName, new List<KeyPressType>(3) { type });
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
        public void RemoveCommand(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action, bool isRemove = true)
        {
            RemoveCommandTask(keyCode, type, action, isRemove).Forget();
        }

        private async UniTaskVoid RemoveCommandTask(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action, bool isRemove = true)
        {
            await UniTask.Yield();

            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (keyCodes.ContainsKey(keyCode) && keyCodes[keyCode].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(keyCode, type, action);

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
        public void RemoveCommand(int mouseButton, KeyPressType type, UnityAction action, bool isRemove = true)
        {
            RemoveCommandTask(mouseButton, type, action, isRemove).Forget();
        }

        private async UniTaskVoid RemoveCommandTask(int mouseButton, KeyPressType type, UnityAction action, bool isRemove = true)
        {
            await UniTask.Yield();

            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (mouseButtons.ContainsKey(mouseButton) && mouseButtons[mouseButton].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(mouseButton, type, action);

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
        public void RemoveCommand(HotKeyNameType keyNameType, KeyPressType type, UnityAction<float> action, bool isRemove = true)
        {
            RemoveCommandTask(keyNameType, type, action, isRemove).Forget();
        }

        private async UniTaskVoid RemoveCommandTask(HotKeyNameType keyNameType, KeyPressType type, UnityAction<float> action, bool isRemove = true)
        {
            await UniTask.Yield();

            string keyName = keyNameType.ToString();

            // 存储要删除的命令
            ICommand removeCommand = null;

            // 存在这个按键状态类型 才进行移除监听事件操作
            if (hotKeys.ContainsKey(keyName) && hotKeys[keyName].Contains(type))
            {
                // 循环遍历
                foreach (ICommand command in commands)
                {
                    // 移除对应的按键事件监听
                    removeCommand = command.RemoveListener(keyName, type, action);

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

        public void SaveKeyMap()
        {
            JsonDataMgr.Instance.SaveData(keyMaps, "KeyMaps");
        }

        public Dictionary<string, KeyMap> LoadKeyMap()
        {
            return JsonDataMgr.Instance.LoadData<Dictionary<string, KeyMap>>("KeyMaps");
        }

        /// <summary>
        /// 重新绑定键盘按键事件监听
        /// </summary>
        /// <param name="oldKey">旧的按键</param>
        /// <param name="newKey">新的按键</param>
        public void RebindingCommandKeyCode(KeyCode oldKey, KeyCode newKey)
        {
            RebindingCommandKeyCodeTask(oldKey, newKey).Forget();
        }

        private async UniTaskVoid RebindingCommandKeyCodeTask(KeyCode oldKey, KeyCode newKey)
        {
            await UniTask.Yield();

            foreach (ICommand command in commands)
            {
                if (command.RebindingKeyCode(oldKey, newKey))
                {
                    KeyCodeCommand keyCodeCommand = command as KeyCodeCommand;
                    keyMaps[keyCodeCommand.action.Method.Name] = new KeyMap()
                    {
                        Type = KeyType.Key,
                        Key = newKey,
                        PressType = keyCodeCommand.type
                    };
                    Debug.Log($"Rebinding CommandKeyCode Successfully. KeyCode: {oldKey} ---> KeyCode: {newKey}");
                    break;
                }
            }

            SaveKeyMap();
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
            SaveKeyMap();
            Clear();
            base.Dispose();
        }
    }
}