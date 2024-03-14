using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// Unity热键监测输入类
    /// </summary>
    public class HotKeyCommand : ICommand
    {
        /// <summary>
        /// 热键名字
        /// Horizontal->左右箭头/AD键 Vertical->上下箭头/WS键 Mouse X/Y->鼠标水平/垂直方向
        /// </summary>
        private string keyName;

        private E_KeyCode_Command_Type type; // 热键输入类型：Axis/AxisRaw
        private UnityAction<float> action; // 热键输入的回调函数

        public HotKeyCommand(string keyName, E_KeyCode_Command_Type type, UnityAction<float> action)
        {
            this.keyName = keyName;
            this.type = type;
            this.action = action;
        }

        public void Execute()
        {
            switch (type)
            {
                case E_KeyCode_Command_Type.Axis:
                    action?.Invoke(Input.GetAxis(keyName));
                    break;

                case E_KeyCode_Command_Type.AxisRaw:
                    action?.Invoke(Input.GetAxisRaw(keyName));
                    break;
            }
        }

        public bool AddListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            if (this.keyName == keyName && this.type == type)
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

        public ICommand RemoveListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            if (this.keyName == keyName && this.type == type)
            {
                if (this.action == null)
                {
                    return this;
                }

                this.action -= action;
            }
            return null;
        }

        public bool RebindingKeyCode(KeyCode oldKey, KeyCode newKey)
        {
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            return false;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            return null;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            return null;
        }
    }
}