using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

namespace QZGameFramework.UIManager
{
    /// <summary>
    /// 如果有需要进行帧更新的需求 可以在子类中继承IUpdateWindow接口
    /// </summary>
    public class WindowBase : WindowBehaviour
    {
        /// <summary>
        /// 所有 Button 列表
        /// </summary>
        private List<Button> allButtonList = new List<Button>();

        /// <summary>
        /// 所有的 Toggle 列表
        /// </summary>
        private List<Toggle> allToggleList = new List<Toggle>();

        /// <summary>
        /// 所有的输入框列表
        /// </summary>
        private List<InputField> allInputList = new List<InputField>();

        private CanvasGroup mUIModelMask;
        private UIModelMask mModelMask;
        private CanvasGroup mCanvasGroup;
        protected Transform mUIContent;

        /// <summary>
        /// 禁用动画
        /// </summary>
        protected bool mDisableAnim = false;

        /// <summary>
        /// 初始化基类组件
        /// </summary>
        private void InitializeBaseComponent()
        {
            mCanvasGroup = transform.GetComponent<CanvasGroup>();
            mUIModelMask = transform.Find("UIModelMask").GetComponent<CanvasGroup>();
            mModelMask = mUIModelMask.gameObject.GetComponent<UIModelMask>();
            mModelMask.OnUIWindowEnable(this);
            mUIContent = transform.Find("UIContent").transform;
        }

        #region 重写生命周期

        public override void OnAwake()
        {
            base.OnAwake();
            InitializeBaseComponent();
        }

        public override void OnShow()
        {
            base.OnShow();
            ShowAnimation();
        }

        public override void OnHide()
        {
            base.OnHide();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemoveAllButtonListener();
            RemoveAllToggleListener();
            RemoveAllInputListener();
            mModelMask.OnUIWindowDisable();
            allButtonList.Clear();
            allToggleList.Clear();
            allInputList.Clear();
        }

        #endregion

        #region 动画管理

        public void ShowAnimation()
        {
            //基础弹窗不需要动画
            if (Canvas.sortingOrder > 90 && mDisableAnim == false && Time.timeScale > 0)
            {
                //Mask动画
                //mUIMask.alpha = 0;
                //mUIMask.DOFade(1, 0.2f);
                //缩放动画
                mUIContent.localScale = Vector3.one * 0.8f;
                mUIContent.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
            }
        }

        public void HideAnimation()
        {
            if (Canvas.sortingOrder > 90 && mDisableAnim == false && Time.timeScale > 0)
            {
                mUIContent.DOScale(Vector3.one * 1.1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    UIManager.Instance.HideWindow(Name);
                });
            }
            else
            {
                UIManager.Instance.HideWindow(Name);
            }
        }

        #endregion

        public void HideWindow()
        {
            HideAnimation();
            //UIManager.Instance.HideWindow(Name);
        }

        public override void SetVisible(bool isVisble)
        {
            Visible = isVisble;
            //gameObject.SetActive(isVisble);
            if (mCanvasGroup == null) return;
            mCanvasGroup.alpha = isVisble ? 1 : 0;
            mCanvasGroup.blocksRaycasts = isVisble;
        }

        /// <summary>
        /// 设置遮罩的显隐
        /// </summary>
        /// <param name="isVisble"></param>
        public void SetMaskVisible(bool isVisble)
        {
            // 单遮和叠遮模式处理
            if (!UIManager.Instance.SingMaskSystem)
            {
                return;
            }
            if (mUIModelMask == null) return;
            mUIModelMask.alpha = isVisble ? 1 : 0;
        }

        #region 组件事件管理

        /// <summary>
        /// 添加 Button 组件事件监听
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="action"></param>
        public void AddButtonClickListener(Button btn, UnityAction action)
        {
            if (btn != null)
            {
                if (!allButtonList.Contains(btn))
                {
                    // 添加 btn 组件到列表中统一管理
                    allButtonList.Add(btn);
                }
                // 先把 btn 所有事件移除 再添加新的事件
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
        }

        /// <summary>
        /// 添加 Toggle 组件事件监听
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="action"></param>
        public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
        {
            if (toggle != null)
            {
                if (!allToggleList.Contains(toggle))
                {
                    // 添加 Toggle 组件到列表中统一管理
                    allToggleList.Add(toggle);
                }
                // 先把 Toggle 所有事件移除 再添加新的事件
                toggle.onValueChanged.RemoveAllListeners();
                toggle.onValueChanged.AddListener((isOn) =>
                {
                    // 把 Toggle 自身和他的 isOn值一起传递出去
                    action?.Invoke(isOn, toggle);
                });
            }
        }

        /// <summary>
        /// 添加输入框事件监听
        /// </summary>
        /// <param name="input"></param>
        /// <param name="onChangeAction">输入修改事件</param>
        /// <param name="endAction">输入完成事件</param>
        public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction, UnityAction<string> endAction)
        {
            if (input != null)
            {
                if (!allInputList.Contains(input))
                {
                    // 添加 InputField 组件到列表中统一管理
                    allInputList.Add(input);
                }
                // 先移除事件 再监听新事件
                input.onValueChanged.RemoveAllListeners();
                input.onEndEdit.RemoveAllListeners();
                input.onValueChanged.AddListener(onChangeAction);
                input.onEndEdit.AddListener(endAction);
            }
        }

        /// <summary>
        /// 移除 Button 事件监听
        /// </summary>
        public void RemoveAllButtonListener()
        {
            foreach (var item in allButtonList)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 移除 Toggle 事件监听
        /// </summary>
        public void RemoveAllToggleListener()
        {
            foreach (var item in allToggleList)
            {
                item.onValueChanged.RemoveAllListeners();
            }
        }

        /// <summary>
        /// 移除 InputField 事件监听
        /// </summary>
        public void RemoveAllInputListener()
        {
            foreach (var item in allInputList)
            {
                item.onValueChanged.RemoveAllListeners();
                item.onEndEdit.RemoveAllListeners();
            }
        }

        #endregion
    }
}