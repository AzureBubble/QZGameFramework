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

        public KeyPressType type; // 热键输入类型：Axis/AxisRaw

        public UnityAction<float> action; // 热键输入的回调函数

        public HotKeyCommand(string keyName, KeyPressType type, UnityAction<float> action)
        {
            this.keyName = keyName;
            this.type = type;
            this.action = action;
        }

        public override void Execute()
        {
            switch (type)
            {
                case KeyPressType.Axis:
                    action?.Invoke(Input.GetAxis(keyName));
                    break;

                case KeyPressType.AxisRaw:
                    action?.Invoke(Input.GetAxisRaw(keyName));
                    break;
            }
        }

        public override bool AddListener(string keyName, KeyPressType type, UnityAction<float> action)
        {
            if (this.keyName == keyName && this.type == type)
            {
                this.action += action;
                return true;
            }

            return false;
        }

        public override ICommand RemoveListener(string keyName, KeyPressType type, UnityAction<float> action)
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
    }
}