using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class ButtonDoubleClickExtend
    {
        [SerializeField] private bool m_isUseDoubleClick;
        [SerializeField, Range(0, 1)] private float m_clickInterval;
        [SerializeField] private float m_lastPointerDownTime;
        [SerializeField] private UnityEvent m_doubleClickedEvent;

        public void OnPointerDown()
        {
            m_lastPointerDownTime = (Time.realtimeSinceStartup - m_lastPointerDownTime) < m_clickInterval ? 0 : Time.realtimeSinceStartup;
            if (m_lastPointerDownTime == 0)
            {
                m_doubleClickedEvent?.Invoke();
            }
            if (m_isUseDoubleClick) { }
        }

        public void AddClickListener(UnityAction callback, float clickInterval)
        {
            m_clickInterval = clickInterval;
            m_isUseDoubleClick = true;
            m_doubleClickedEvent.AddListener(callback);
        }
    }
}