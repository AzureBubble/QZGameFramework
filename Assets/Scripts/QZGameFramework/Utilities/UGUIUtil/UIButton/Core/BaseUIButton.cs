using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class BaseUIButton : Button, IUpdateSelectedHandler
    {
        [SerializeField] private ButtonClickProtectExtend m_buttonClickProtectExtend = new ButtonClickProtectExtend();
        [SerializeField] private ButtonDoubleClickExtend m_buttonDoubleClickExtend = new ButtonDoubleClickExtend();
        [SerializeField] private ButtonLongPressExtend m_buttonLongPressExtend = new ButtonLongPressExtend();
        [SerializeField] private ButtonClickScaleExtend m_buttonClickScaleExtend = new ButtonClickScaleExtend();
        [SerializeField] private ButtonClickSoundExtend m_buttonClickSoundExtend = new ButtonClickSoundExtend();
        [SerializeField] private UnityEvent m_buttonClickEvent = new UnityEvent();

        private Vector2 m_PressPos; // 点击的坐标
        private bool m_isPress; // 是否按下
        private bool m_isFirstProtectClick;
        private PointerEventData m_pointerEventData;
        public Action OnPointerUpListener;

        public void OnUpdateSelected(BaseEventData eventData)
        {
            m_buttonLongPressExtend?.OnUpdateSelected();
        }

        /// <summary>
        /// 同一元素按下并抬起事件
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClick();
        }

        public void OnPointerClick()
        {
            if (m_buttonClickProtectExtend.IsUseClickProtect && !m_buttonClickProtectExtend.CanClick)
            {
                return;
            }
            if (interactable)
            {
                m_buttonClickSoundExtend?.OnButtonClick();
                onClick?.Invoke();
            }
        }

        /// <summary>
        /// 按下事件
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!m_buttonClickProtectExtend.CanClick)
                return;

            base.OnPointerDown(eventData);
            m_PressPos = eventData.position;
            m_isPress = true;
            m_pointerEventData = eventData;
            m_isFirstProtectClick = true;

            m_buttonClickProtectExtend?.OnPointerDown();
            m_buttonDoubleClickExtend?.OnPointerDown();
            m_buttonLongPressExtend?.OnPointerDown();
            m_buttonClickScaleExtend?.OnPointerDown(transform, interactable);
            m_buttonClickSoundExtend?.OnPointerDown(transform);
        }

        /// <summary>
        /// 抬起事件
        /// </summary>
        /// <param name="eventData"></param>
        public override void OnPointerUp(PointerEventData eventData)
        {
            m_buttonClickProtectExtend?.OnPointerUp();

            if (!m_isFirstProtectClick && !m_buttonClickProtectExtend.CanClick)
            {
                return;
            }
            if (m_isFirstProtectClick)
            {
                m_isFirstProtectClick = false;
            }

            base.OnPointerUp(eventData);
            m_isPress = false;
            m_pointerEventData = null;

            if (interactable && Mathf.Abs(Vector2.Distance(m_PressPos, eventData.position)) < 10f)
            {
                m_buttonClickEvent?.Invoke();
                m_buttonClickSoundExtend.OnPointerUp(this);
            }

            m_buttonLongPressExtend?.OnPointerUp();
            OnPointerUpListener?.Invoke();
            m_buttonClickScaleExtend?.OnPointerUp(transform, interactable);
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// 添加按钮长按时间
        /// </summary>
        /// <param name="callback">长按后回调</param>
        /// <param name="duration">长按持续时间</param>
        public void AddButtonLongPressListener(UnityAction callback, float duration)
        {
            m_buttonLongPressExtend.AddListener(callback, duration);
        }

        /// <summary>
        /// 添加按钮双击时间
        /// </summary>
        /// <param name="callback">双击后回调</param>
        /// <param name="clickInterval">双击时间间隔</param>
        public void AddButtonDoubleClickListener(UnityAction callback, float clickInterval)
        {
            m_buttonDoubleClickExtend.AddClickListener(callback, clickInterval);
        }

        public void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (m_isPress && m_pointerEventData != null)
                {
                    OnPointerUp(m_pointerEventData);
                }
            }
        }

        public void AddButtonClick(UnityAction callback)
        {
            m_buttonClickEvent.AddListener(callback);
        }

        public void RemoveButtonClick(UnityAction callback)
        {
            m_buttonClickEvent.RemoveListener(callback);
        }

#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            if (m_buttonClickScaleExtend.UseClickScale)
            {
                transition = Transition.None;
            }
            //Navigation tempNavigation = navigation;
            //tempNavigation.mode = Navigation.Mode.None;
            //navigation = tempNavigation;
        }

#endif
    }
}