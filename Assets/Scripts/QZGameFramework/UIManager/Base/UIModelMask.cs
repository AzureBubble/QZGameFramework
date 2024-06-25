using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.UIManager
{
    public class UIModelMask : MonoBehaviour
    {
        public UIModelMaskType uiModelMaskType = UIModelMaskType.TransparentType;
        private RawImage m_modelMask;
        private Button m_closeBtn;
        private bool m_canClose = false;
        public bool CanClose => m_canClose;

        private void Awake()
        {
            m_closeBtn = GetComponent<Button>();
            m_modelMask = GetComponent<RawImage>();
            if (m_modelMask != null)
            {
                switch (uiModelMaskType)
                {
                    case UIModelMaskType.NormalType:
                        m_modelMask.color = new Color(0, 0, 0, 1f);
                        m_canClose = false;
                        break;

                    case UIModelMaskType.TransparentType:
                        m_modelMask.color = new Color(0, 0, 0, 0.01f);
                        m_canClose = false;
                        break;

                    case UIModelMaskType.UndertintHaveClose:
                        m_modelMask.color = new Color(0, 0, 0, 0.4f);
                        m_canClose = true;
                        break;

                    case UIModelMaskType.NormalHaveClose:
                        m_modelMask.color = new Color(0, 0, 0, 1f);
                        m_canClose = true;
                        break;

                    case UIModelMaskType.TransparentHaveClose:
                        m_modelMask.color = new Color(0, 0, 0, 0.01f);
                        m_canClose = true;
                        break;

                    case UIModelMaskType.NoneType:
                    default:
                        m_modelMask.color = new Color(0, 0, 0, 0);
                        m_canClose = false;
                        break;
                }
            }
        }

        public void OnUIWindowEnable(WindowBase parentWindow)
        {
            if (!m_canClose)
            {
                return;
            }
            m_closeBtn.onClick.AddListener(() =>
            {
                parentWindow.HideWindow();
            });
        }

        public void OnUIWindowDisable()
        {
            if (!m_canClose)
            {
                return;
            }
            m_closeBtn.onClick.RemoveAllListeners();
        }
    }

    public enum UIModelMaskType
    {
        /// <summary> 非模态 </summary>
        NoneType,

        /// <summary> 普通模态 </summary>
        NormalType,

        /// <summary> 透明模态 </summary>
        TransparentType,

        /// <summary> 浅色普通状态且有关闭功能 </summary>
        UndertintHaveClose,

        /// <summary> 普通状态且有关闭功能 </summary>
        NormalHaveClose,

        /// <summary> 透明状态且有关闭功能 </summary>
        TransparentHaveClose,
    }
}