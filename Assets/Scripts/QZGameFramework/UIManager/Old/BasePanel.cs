using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace QZGameFramework.GFUIManager
{
    /// <summary>
    /// 抽象面板基类
    /// 负责找到自己面板下所有的控件 并管理他们
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class BasePanel : MonoBehaviour
    {
        /// <summary>
        /// 存储自身面板下所有控件的容器
        /// key —— 控件的名字
        /// value —— 对应名字关联的所有控件的集合(比如Button，既有Image，也有Button)
        /// </summary>
        private Dictionary<string, List<UIBehaviour>> controlDic = new Dictionary<string, List<UIBehaviour>>();

        // 控制面板淡入淡出
        private CanvasGroup canvasGroup;

        private float alphaSpeed = 10.0f; // 淡入淡出速度
        private UnityAction showCallBack;
        private UnityAction hideCallBack;
        private bool isShow = false;

        /// <summary>
        /// Awake 的时候就获取自身身上所有的控件存入容器中
        /// </summary>
        protected virtual void Awake()
        {
            if (!this.TryGetComponent<CanvasGroup>(out canvasGroup))
            {
                canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
            }

            FindChildrenControl<Button>();
            FindChildrenControl<Image>();
            FindChildrenControl<Text>();
            FindChildrenControl<TMP_Text>();
            FindChildrenControl<Toggle>();
            FindChildrenControl<InputField>();
            FindChildrenControl<Slider>();
            FindChildrenControl<ScrollRect>();
        }

        protected virtual void Start()
        {
            InitalizePanel();
        }

        protected virtual void Update()
        {
        }

        /// <summary>
        /// 面板初始化函数
        /// </summary>
        protected abstract void InitalizePanel();

        /// <summary>
        /// 根据控件类型，找到所有相关联的控件，存入容器中统一管理
        /// </summary>
        /// <typeparam controlName="T">控件类型</typeparam>
        private void FindChildrenControl<T>() where T : UIBehaviour
        {
            // 找到对应的所有子控件
            T[] controls = GetComponentsInChildren<T>();

            foreach (T control in controls)
            {
                // 判断是否已经存在对应名字控件的容器
                if (controlDic.ContainsKey(control.name))
                {
                    // 如果已经存在，则把控件存入对应的容器列表中
                    controlDic[control.name].Add(control);
                }
                else
                {
                    // 如果不存在，则新建一个容器存储
                    controlDic.Add(control.name, new List<UIBehaviour>() { control });
                }

                // 加入容器后，根据控件类型添加对应的响应事件
                if (control is Button)
                {
                    (control as Button).onClick.AddListener(() =>
                    {
                        OnButtonClick(control.name);
                    });
                }
                else if (control is Toggle)
                {
                    (control as Toggle).onValueChanged.AddListener((value) =>
                    {
                        OnToggleValueChanged(control.name, value);
                    });
                }
                else if (control is InputField)
                {
                    (control as InputField).onValueChanged.AddListener((value) =>
                    {
                        OnInputFieldValueChanged(control.name, value);
                    });
                }
                else if (control is Slider)
                {
                    (control as Slider).onValueChanged.AddListener((value) =>
                    {
                        OnSliderValueChanged(control.name, value);
                    });
                }
            }
        }

        /// <summary>
        /// Slider 事件监听
        /// </summary>
        /// <param name="sliderName">控件名字</param>
        /// <param name="value"></param>
        protected virtual void OnSliderValueChanged(string sliderName, float value)
        {
        }

        /// <summary>
        /// InputField 事件监听
        /// </summary>
        /// <param name="inputName">控件名字</param>
        /// <param name="value"></param>
        protected virtual void OnInputFieldValueChanged(string inputName, string value)
        {
        }

        /// <summary>
        /// Toggle 事件监听
        /// </summary>
        /// <param name="toggleName">控件名字</param>
        /// <param name="value"></param>
        protected virtual void OnToggleValueChanged(string toggleName, bool value)
        {
        }

        /// <summary>
        /// Button 事件监听
        /// </summary>
        /// <param name="btnName">控件名字</param>
        /// <param name="value"></param>
        protected virtual void OnButtonClick(string btnName)
        {
        }

        /// <summary>
        /// 通过控件名字在容器中找到对应的控件
        /// </summary>
        /// <typeparam controlName="T">控件类型</typeparam>
        /// <param controlName="controlName">控件名字</param>
        /// <returns></returns>
        protected T GetControl<T>(string controlName) where T : UIBehaviour
        {
            // 根据传入的控件名字找到对应的控件容器
            if (controlDic.ContainsKey(controlName))
            {
                // 遍历容器中的所有控件，找到与名字相对应的控件
                foreach (UIBehaviour control in controlDic[controlName])
                {
                    if (control.name == controlName)
                    {
                        // 返回出去
                        return control as T;
                    }
                }
            }

            // 没找到则返回空
            return null;
        }

        /// <summary>
        /// 显示自己
        /// </summary>
        public virtual void ShowMe(UnityAction callback = null, float targetAlpha = 1f)
        {
            this.gameObject.SetActive(true);
            isShow = true;
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            showCallBack = callback;
            FadeIn(targetAlpha);
        }

        /// <summary>
        /// 隐藏自己
        /// </summary>
        public virtual void HideMe(UnityAction callback = null, float targetAlpha = 0f)
        {
            isShow = false;
            canvasGroup.alpha = 1.0f;
            canvasGroup.interactable = false;
            hideCallBack = callback;
            FadeOut(targetAlpha);
        }

        /// <summary>
        /// 画布淡入
        /// </summary>
        public virtual void FadeIn(float targetAlpha)
        {
            StartCoroutine(FadeInAndOutAsync(targetAlpha));
        }

        /// <summary>
        /// 画布淡出
        /// </summary>
        public virtual void FadeOut(float targetAlpha)
        {
            StartCoroutine(FadeInAndOutAsync(targetAlpha));
        }

        /// <summary>
        /// 异步执行画布淡入淡出
        /// </summary>
        /// <param name="targetAlpha"></param>
        /// <returns></returns>
        private IEnumerator FadeInAndOutAsync(float targetAlpha)
        {
            while (Mathf.Abs(canvasGroup.alpha - targetAlpha) > 0.05f)
            {
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, alphaSpeed * Time.deltaTime);
                yield return null;
            }
            // 画布淡入淡出后，想要执行的事件
            if (isShow)
            {
                showCallBack?.Invoke();
            }
            else
            {
                hideCallBack?.Invoke();
            }
            canvasGroup.interactable = true;
            canvasGroup.alpha = targetAlpha;
        }
    }
}