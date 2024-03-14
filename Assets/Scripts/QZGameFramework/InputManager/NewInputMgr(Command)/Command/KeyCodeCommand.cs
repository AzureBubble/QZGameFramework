using UnityEngine.Events;
using UnityEngine;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 键盘按键命令类
    /// </summary>
    public class KeyCodeCommand : ICommand
    {
        private E_KeyCode_Command_Type type; // 按键状态 Down/Stay/Up
        private KeyCode keyCode; // 某个按键
        private UnityAction action; // 按下触发事件

        public KeyCodeCommand(KeyCode keyCode, E_KeyCode_Command_Type type, UnityAction action)
        {
            this.keyCode = keyCode;
            this.type = type;
            this.action = action;
        }

        public virtual void Execute()
        {
            switch (type)
            {
                case E_KeyCode_Command_Type.Down:
                    if (Input.GetKeyDown(keyCode))
                    {
                        action?.Invoke();
                    }
                    break;

                case E_KeyCode_Command_Type.Stay:
                    if (Input.GetKey(keyCode))
                    {
                        action?.Invoke();
                    }
                    break;

                case E_KeyCode_Command_Type.Up:
                    if (Input.GetKeyUp(keyCode))
                    {
                        action?.Invoke();
                    }
                    break;
            }
        }

        public bool AddListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            if (this.keyCode == keyCode && this.type == type)
            {
                if (this.action == null)
                {
                    this.action = action;
                }
                else
                {
                    this.action += action;
                }

                return true;
            }
            return false;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            if (this.keyCode == keyCode && this.type == type)
            {
                if (this.action != null)
                {
                    this.action -= action;
                }

                if (this.action == null)
                {
                    return this;
                }
            }
            return null;
        }

        public bool RebindingKeyCode(KeyCode oldKey, KeyCode newKey)
        {
            if (this.keyCode == oldKey)
            {
                this.keyCode = newKey;
                return true;
            }
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            return false;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            return null;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            return null;
        }
    }
}