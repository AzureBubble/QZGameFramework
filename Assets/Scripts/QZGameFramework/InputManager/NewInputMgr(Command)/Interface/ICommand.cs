using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.GFInputManager
{
    /// <summary>
    /// 命令接口
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        void Execute();

        /// <summary>
        /// 重新绑定按键事件
        /// </summary>
        /// <param name="oldKey">旧的按键</param>
        /// <param name="newKey">新的按键</param>
        bool RebindingKeyCode(KeyCode oldKey, KeyCode newKey);

        bool AddListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action);

        bool AddListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action);

        bool AddListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action);

        ICommand RemoveListener(E_KeyCode_Command_Type type, KeyCode keyCode, UnityAction action);

        ICommand RemoveListener(E_KeyCode_Command_Type type, int mouseButton, UnityAction action);

        ICommand RemoveListener(E_KeyCode_Command_Type type, string keyName, UnityAction<float> action);
    }
}