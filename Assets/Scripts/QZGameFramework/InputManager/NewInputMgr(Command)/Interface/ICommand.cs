using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public abstract class ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// 重新绑定按键事件
        /// </summary>
        /// <param name="oldKey">旧的按键</param>
        /// <param name="newKey">新的按键</param>
        public virtual bool RebindingKeyCode(KeyCode oldKey, KeyCode newKey) => false;

        public virtual bool AddListener(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action) => false;

        public virtual ICommand RemoveListener(KeyCode keyCode, KeyPressType type, UnityAction<KeyCode> action) => null;

        public virtual bool AddListener(int mouseButton, KeyPressType type, UnityAction action) => false;

        public virtual ICommand RemoveListener(int mouseButton, KeyPressType type, UnityAction action) => null;

        public virtual bool AddListener(string keyName, KeyPressType type, UnityAction<float> action) => false;

        public virtual ICommand RemoveListener(string keyName, KeyPressType type, UnityAction<float> action) => null;
    }
}