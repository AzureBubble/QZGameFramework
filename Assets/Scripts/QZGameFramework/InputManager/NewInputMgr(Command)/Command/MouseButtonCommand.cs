using UnityEngine.Events;
using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 鼠标按键命令类
    /// </summary>
    public class MouseButtonCommand : ICommand
    {
        /// <summary>
        /// 0->鼠标左键 1->鼠标右键 2->鼠标中键/滚轮
        /// </summary>
        private int mouseButton;

        private E_KeyCode_Command_Type type; // 按键状态
        private UnityAction action; // 按下触发事件

        public MouseButtonCommand(int mouseButton, E_KeyCode_Command_Type type, UnityAction action)
        {
            this.mouseButton = mouseButton;
            this.type = type;
            this.action = action;
        }

        public virtual void Execute()
        {
            switch (type)
            {
                case E_KeyCode_Command_Type.Down:
                    if (Input.GetMouseButtonDown(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;

                case E_KeyCode_Command_Type.Stay:
                    if (Input.GetMouseButton(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;

                case E_KeyCode_Command_Type.Up:
                    if (Input.GetMouseButtonUp(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;
            }
        }

        public bool AddListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            if (this.mouseButton == mouseButton && this.type == type)
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

        public ICommand RemoveListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action)
        {
            if (this.mouseButton == mouseButton && this.type == type)
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
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            return false;
        }

        public bool AddListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            return false;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action)
        {
            return null;
        }

        public ICommand RemoveListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action)
        {
            return null;
        }
    }
}