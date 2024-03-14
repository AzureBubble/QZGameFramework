using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using QZGameFramework.PackageMgr.ResourcesManager;

namespace QZGameFramework.GFUIManager
{
    public enum E_UI_Panel_Layer
    {
        Bot, Mid, Top, System
    }

    /// <summary>
    /// UI 面板容器管理者
    /// 负责 游戏里所有UI面板的管理 包括显隐
    /// 享元模式
    /// </summary>
    public class UIManager : Singleton<UIManager>
    {
        /// <summary>
        /// 正在显示的面板的容器
        /// key —— 面板名字
        /// value —— 面板类型
        /// </summary>
        private Dictionary<string, BasePanel> panelDic;

        public RectTransform canvas; // UI 父级对象
        private Transform bot; // 低级面板父节点 (显示层级由低到高)
        private Transform mid; // 中级面板父节点
        private Transform top; // 高级面板父节点
        private Transform system; // 系统级面板父节点

        private PlayerInputAction inputAction;

        public override void Initialize()
        {
            // 构造函数时，加载 UI 面板，并设置不销毁
            GameObject obj = GameObject.Instantiate(ResourcesMgr.Instance.LoadRes<GameObject>("UI/Canvas"));
            obj.name = "Canvas";
            canvas = obj.GetComponent<RectTransform>();
            GameObject.DontDestroyOnLoad(obj);
            // 找到所有的层级结点
            bot = canvas.Find("Bot");
            mid = canvas.Find("Mid");
            top = canvas.Find("Top");
            system = canvas.Find("System");

            // 初始化容器
            panelDic = new Dictionary<string, BasePanel>();

            // 获得全局唯一的键盘映射
            //inputAction = GameManager.Instance.playerInputAction;
        }

        #region 面板功能(获取面板，显隐面板，面板控件自定义事件增加)

        /// <summary>
        /// 外部获取面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <returns>没有则返回空</returns>
        public T GetPanel<T>() where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName))
            {
                return panelDic[panelName] as T;
            }
            return null;
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        /// <typeparam name="T">面板类型</typeparam>
        /// <param name="layer">显示层级</param>
        /// <param name="callback">回调函数</param>
        public void ShowPanel<T>(E_UI_Panel_Layer layer = E_UI_Panel_Layer.Mid, UnityAction<T> callback = null) where T : BasePanel
        {
            // 通过反射获得面板的名字
            string panelName = typeof(T).Name;

            if (panelDic.ContainsKey(panelName))
            {
                if (!panelDic[panelName].gameObject.activeSelf)
                {
                    panelDic[panelName].ShowMe();
                }
                callback?.Invoke(panelDic[panelName] as T);
                return;
            }

            ResourcesMgr.Instance.LoadResAsync<GameObject>("UI/Panel/" + panelName, (resObj) =>
            {
                Transform parent = GetPanelParent(layer);

                GameObject panelObj = GameObject.Instantiate(resObj, parent);
                panelObj.name = panelName;

                // 设置面板的位置，防止变形错位
                panelObj.transform.localPosition = Vector3.zero;
                panelObj.transform.localScale = Vector3.one;
                (panelObj.transform as RectTransform).offsetMax = Vector2.zero;
                (panelObj.transform as RectTransform).offsetMin = Vector2.zero;

                // 把面板设置显示，并通过回调函数返回出去
                // 再把面板加入到已经在场景显示的字典中
                T panel = panelObj.GetComponent<T>();
                callback?.Invoke(panel);
                panel.ShowMe();
                panelDic.Add(panelName, panel);
            });
        }

        /// <summary>
        /// 获得面板显示层级的父节点Transform
        /// </summary>
        /// <param name="layer">传入面板层级</param>
        /// <returns>默认返回Mid</returns>
        public Transform GetPanelParent(E_UI_Panel_Layer layer)
        {
            switch (layer)
            {
                case E_UI_Panel_Layer.Bot:
                    return bot;

                case E_UI_Panel_Layer.Mid:
                    return mid;

                case E_UI_Panel_Layer.Top:
                    return top;

                case E_UI_Panel_Layer.System:
                    return system;

                default:
                    return null;
            }
        }

        /// <summary>
        /// 隐藏面板
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isFade">是否淡入淡出</param>
        /// <param name="isDestory">是否销毁面板</param>
        public void HidePanel<T>(bool isFade = true, bool isDestory = true) where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelDic.ContainsKey(panelName) && panelDic[panelName].gameObject.activeSelf)
            {
                if (isFade)
                {
                    panelDic[panelName].HideMe(() =>
                    {
                        if (isDestory)
                        {
                            GameObject.Destroy(panelDic[panelName].gameObject);
                            panelDic.Remove(panelName);
                        }
                        else
                        {
                            panelDic[panelName].gameObject.SetActive(false);
                        }
                    });
                }
                else
                {
                    if (isDestory)
                    {
                        GameObject.Destroy(panelDic[panelName].gameObject);
                        panelDic.Remove(panelName);
                    }
                    else
                    {
                        panelDic[panelName].gameObject.SetActive(false);
                    }
                }
            }
        }

        /// <summary>
        /// 添加自定义事件监听器到指定控件的 EventTrigger 上。
        /// </summary>
        /// <param name="control">要添加监听器的控件</param>
        /// <param name="type">事件类型</param>
        /// <param name="callback">回调函数</param>
        public static void AddCustomEventListener(UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> action)
        {
            // 找到控件身上的EventTrigger组件
            EventTrigger trigger = control.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                // 如果没有则添加一个
                trigger = control.AddComponent<EventTrigger>();
            }
            // 创建一个 EventTrigger.Entry 条目并设置事件类型和回调函数
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(action);
            // 将条目添加到 EventTrigger 的触发器列表中
            trigger.triggers.Add(entry);
        }

        #endregion

        public override void Dispose()
        {
            if (IsDisposed) return;
            if (canvas != null)
            {
                GameObject.Destroy(canvas.gameObject);
            }
            panelDic = null;

            base.Dispose();
        }

        #region 新输入系统改键功能

        public void RebindKeyAction(string actionName, int controlIndex, int keyIndex, UnityAction callback, bool isExclude = true)
        {
            if (!CheckHaveBindingActionByActionName(actionName)) return;
            // 临时存储
            InputAction tempAction = inputAction.FindAction(actionName);
            // 得到对应的键的索引
            int tempIndex = tempAction.GetBindingIndexForControl(tempAction.controls[controlIndex]) + keyIndex;

            // 改建前一定要先禁用键盘监听
            tempAction.Disable();

            if (isExclude)
            {
                // 执行互动重绑定，为指定的键创建重绑定操作
                tempAction.PerformInteractiveRebinding(tempIndex)
                .WithControlsExcluding("Mouse") // 排除掉名为 "Mouse" 的控件
                .OnMatchWaitForAnother(.1f) // 如果有匹配的控件，等待另一个键的输入（最多 0.1 秒）来进行绑定
                .OnComplete(operation => // 当重绑定操作完成时，执行以下操作：
                {
                    // 改建完成后保存到本地
                    string actionMapJson = inputAction.asset.SaveBindingOverridesAsJson();
                    PlayerPrefs.SetString("ActionMap", actionMapJson);

                    callback?.Invoke();
                    tempAction.Enable();
                    operation.Dispose();
                }).Start();
            }
            else
            {
                tempAction.PerformInteractiveRebinding(tempIndex)
                .OnMatchWaitForAnother(.1f)
                .OnComplete(operation =>
                {
                    // 改建完成后保存到本地
                    string actionMapJson = inputAction.asset.SaveBindingOverridesAsJson();
                    PlayerPrefs.SetString("ActionMap", actionMapJson);

                    callback?.Invoke();
                    tempAction.Enable();
                    operation.Dispose(); // 释放操作对象，以确保资源被正确释放
                }).Start(); // 启动互动重绑定操作，此时开始等待用户的输入
            }
        }

        /// <summary>
        /// 得到对应映射对应的绑定按键的名字
        /// </summary>
        /// <param name="actionName">键盘映射名</param>
        /// <param name="index">当前映射的按键索引</param>
        /// <returns></returns>
        public string GetBindingNameOfAction(string actionName, int controlIndex, int keyIndex)
        {
            if (!CheckHaveBindingActionByActionName(actionName)) return null;
            // 临时存储
            InputAction tempAction = inputAction.FindAction(actionName);
            // 得到对应的键的索引
            int tempIndex = tempAction.GetBindingIndexForControl(tempAction.controls[controlIndex]) + keyIndex;
            // 通过索引找到对应的键，并把路径改成人类易读字符返回
            return InputControlPath.ToHumanReadableString(
                tempAction.bindings[tempIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice);
        }

        /// <summary>
        /// 查找键盘映射中是否存在对应名字的事件
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        private bool CheckHaveBindingActionByActionName(string actionName)
        {
            if (inputAction.asset[actionName].bindings.Count > 0)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}