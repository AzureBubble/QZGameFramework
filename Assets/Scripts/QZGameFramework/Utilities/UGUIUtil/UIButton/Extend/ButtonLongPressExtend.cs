using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using static UnityEngine.UI.Button;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class ButtonLongPressExtend
    {
        [SerializeField] private bool m_isUseLongPress;
        [SerializeField, Range(0, 10)] private float m_duration;
        [SerializeField] private bool m_isLoopLongPress;
        [SerializeField, Range(0, 10)] private float m_interval;
        [SerializeField] private UnityEvent m_buttonLongPressEvent;
        private float m_pointerDownTime;
        private bool m_isTriggered;

        public void OnPointerDown()
        {
            m_pointerDownTime = Time.realtimeSinceStartup;
        }

        public void OnPointerUp()
        {
            m_isTriggered = false;
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void OnUpdateSelected()
        {
            if (!m_isTriggered && m_duration >= 0 && (Time.realtimeSinceStartup - m_pointerDownTime) >= m_duration)
            {
                m_buttonLongPressEvent?.Invoke();
                if (!m_isLoopLongPress)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
                m_pointerDownTime = Time.realtimeSinceStartup;
                m_isTriggered = true;
            }

            if (m_isLoopLongPress && m_isTriggered && (Time.realtimeSinceStartup - m_pointerDownTime) >= m_interval)
            {
                m_buttonLongPressEvent?.Invoke();
                m_pointerDownTime = Time.realtimeSinceStartup;
            }
        }

        public void AddListener(UnityAction callback, float duration)
        {
            m_duration = duration;
            m_isUseLongPress = true;
            m_buttonLongPressEvent.AddListener(callback);
            if (m_isUseLongPress) { }
        }
    }
}