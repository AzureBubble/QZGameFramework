using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class ButtonClickProtectExtend
    {
        [SerializeField] private bool m_isUseClickProtect;
        [SerializeField, Range(0, 5)] private float m_protectTime;
        private float m_lastClickTime;
        private bool m_canClick = true;
        public bool CanClick => m_canClick;
        public bool IsUseClickProtect => m_isUseClickProtect;

        public void OnPointerDown()
        {
            if (m_isUseClickProtect)
            {
                m_lastClickTime = Time.realtimeSinceStartup;
                m_canClick = false;
            }
        }

        public void OnPointerUp()
        {
            if (Time.realtimeSinceStartup - m_lastClickTime >= m_protectTime)
            {
                m_canClick = true;
            }
        }
    }
}