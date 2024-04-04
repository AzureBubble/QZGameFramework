using UnityEngine;
using UnityEngine.Events;

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

        private KeyPressType type; // 按键状态
        private UnityAction action; // 按下触发事件

        public MouseButtonCommand(int mouseButton, KeyPressType type, UnityAction action)
        {
            this.mouseButton = mouseButton;
            this.type = type;
            this.action = action;
        }

        public override void Execute()
        {
            switch (type)
            {
                case KeyPressType.Down:
                    if (Input.GetMouseButtonDown(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;

                case KeyPressType.Stay:
                    if (Input.GetMouseButton(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;

                case KeyPressType.Up:
                    if (Input.GetMouseButtonUp(mouseButton))
                    {
                        action?.Invoke();
                    }
                    break;
            }
        }

        public override bool AddListener(int mouseButton, KeyPressType type, UnityAction action)
        {
            if (this.mouseButton == mouseButton && this.type == type)
            {
                this.action += action;
                return true;
            }

            return false;
        }

        public override ICommand RemoveListener(int mouseButton, KeyPressType type, UnityAction action)
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
    }
}