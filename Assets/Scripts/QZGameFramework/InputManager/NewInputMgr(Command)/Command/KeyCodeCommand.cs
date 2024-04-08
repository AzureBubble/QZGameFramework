using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 键盘按键命令类
    /// </summary>
    public class KeyCodeCommand : ICommand
    {
        public KeyPressType type; // 按键状态 Down/Stay/Up
        public KeyCode keyCode; // 某个按键

        public UnityAction<KeyCode> action; // 按下触发事件

        public KeyCodeCommand(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action)
        {
            this.keyCode = keyCode;
            this.type = type;
            this.action = action;
        }

        public override void Execute()
        {
            switch (type)
            {
                case KeyPressType.Down:
                    if (Input.GetKeyDown(keyCode))
                    {
                        action?.Invoke(keyCode);
                    }
                    break;

                case KeyPressType.Stay:
                    if (Input.GetKey(keyCode))
                    {
                        action?.Invoke(keyCode);
                    }
                    break;

                case KeyPressType.Up:
                    if (Input.GetKeyUp(keyCode))
                    {
                        action?.Invoke(keyCode);
                    }
                    break;
            }
        }

        public override bool AddListener(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action)
        {
            if (this.keyCode == keyCode && this.type == type)
            {
                this.action += action;
                return true;
            }
            return false;
        }

        public override ICommand RemoveListener(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action)
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

        public override bool RebindingKeyCode(KeyCode oldKey, KeyCode newKey)
        {
            if (this.keyCode == oldKey)
            {
                this.keyCode = newKey;
                return true;
            }
            return false;
        }
    }
}