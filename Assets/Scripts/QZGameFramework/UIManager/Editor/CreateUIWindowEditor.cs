using QZGameFramework.Utilities.UGUIUtil;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.UIManager
{
    public class CreateUIWindowEditor : EditorWindow
    {
        private static WindowType windowType = WindowType.FirstLevel;
        private string windowTitle = "WindowName"; // 窗口标题
        private string windowName = "New"; // 默认窗口名
        private static string WindowPrefabSavePath = "Assets/Resources/UI/Window";
        private static string UIRootPrefabSavePath = "Assets/Resources/UI";
        private const string UI_CONFIG_PATH = "UI/UIConfig/UIConfig";

        [MenuItem("GameObject/UI/创建UIRoot根物体", priority = 0)]
        private static void CreateUIRoot()
        {
            CreateUIRootGameObject(true);
        }

        [MenuItem("GameObject/UI/创建一级UI窗口模板", priority = 1)]
        private static void CreateFirstLevelWindow()
        {
            // 创建窗口实例并设定窗口标题
            CreateUIWindowEditor window = (CreateUIWindowEditor)EditorWindow.GetWindow(typeof(CreateUIWindowEditor));
            window.titleContent = new GUIContent("创建一级UI窗口模板");
            window.maxSize = new Vector2(300, 70);
            window.minSize = new Vector2(200, 70); // 设置窗口最小尺寸
            windowType = WindowType.FirstLevel;
            window.Show();
        }

        [MenuItem("GameObject/UI/创建二级UI窗口模板", priority = 2)]
        private static void CreateSecondLevelWindow()
        {
            // 创建窗口实例并设定窗口标题
            CreateUIWindowEditor window = (CreateUIWindowEditor)EditorWindow.GetWindow(typeof(CreateUIWindowEditor));
            window.titleContent = new GUIContent("创建二级UI窗口模板");
            window.maxSize = new Vector2(300, 70);
            window.minSize = new Vector2(200, 70); // 设置窗口最小尺寸
            windowType = WindowType.SecondLevel;
            window.Show();
        }

        [MenuItem("GameObject/UI/创建三级UI窗口模板", priority = 3)]
        private static void CreateThreeLevelWindow()
        {
            // 创建窗口实例并设定窗口标题
            CreateUIWindowEditor window = (CreateUIWindowEditor)EditorWindow.GetWindow(typeof(CreateUIWindowEditor));
            window.titleContent = new GUIContent("创建三级UI窗口模板");
            window.maxSize = new Vector2(300, 70);
            window.minSize = new Vector2(200, 70); // 设置窗口最小尺寸
            windowType = WindowType.ThreeLevel;
            window.Show();
        }

        [MenuItem("GameObject/UI/创建四级UI窗口模板", priority = 4)]
        private static void CreateFourLevelWindow()
        {
            // 创建窗口实例并设定窗口标题
            CreateUIWindowEditor window = (CreateUIWindowEditor)EditorWindow.GetWindow(typeof(CreateUIWindowEditor));
            window.titleContent = new GUIContent("创建四级UI窗口模板");
            window.maxSize = new Vector2(300, 70);
            window.minSize = new Vector2(200, 70); // 设置窗口最小尺寸
            windowType = WindowType.FourLevel;
            window.Show();
        }

        [MenuItem("GameObject/UI/创建五级UI窗口模板", priority = 5)]
        private static void CreateFiveLevelWindow()
        {
            // 创建窗口实例并设定窗口标题
            CreateUIWindowEditor window = (CreateUIWindowEditor)EditorWindow.GetWindow(typeof(CreateUIWindowEditor));
            window.titleContent = new GUIContent("创建五级UI窗口模板");
            window.maxSize = new Vector2(300, 70);
            window.minSize = new Vector2(200, 70); // 设置窗口最小尺寸
            windowType = WindowType.FiveLevel;
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            windowName = EditorGUILayout.TextField(windowName);
            EditorGUILayout.EndHorizontal();
            windowTitle = $"窗口名: {windowName}Window";
            GUILayout.Label(windowTitle, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            if (GUILayout.Button("Create Window", GUILayout.ExpandWidth(true)))
            {
                // 创建窗口的逻辑
                CreateNewWindow(windowName + "Window", windowType);
            }
        }

        private void CreateNewWindow(string windowName, WindowType windowType)
        {
            GameObject uiRoot = CreateUIRootGameObject(false);

            if (uiRoot == null)
            {
                Debug.LogError("UIRoot创建失败，请检查代码和环境");
                return;
            }

            // 创建一个空的游戏对象并添加组件
            GameObject newWindow = new GameObject(windowName);
            newWindow.layer |= LayerMask.NameToLayer("UI");
            newWindow.transform.SetParent(uiRoot.transform, false);
            Canvas canvas = newWindow.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = GetWindowSortingOrder(windowType);
            CanvasScaler canvasScaler = newWindow.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            newWindow.AddComponent<GraphicRaycaster>();
            newWindow.AddComponent<CanvasGroup>();

            // 创建 UIMask 遮罩层
            GameObject uiModelMask = new GameObject("UIModelMask");
            uiModelMask.layer |= LayerMask.NameToLayer("UI");
            uiModelMask.transform.SetParent(newWindow.transform, false);
            RectTransform maskRectTransform = uiModelMask.AddComponent<RectTransform>();
            maskRectTransform.sizeDelta = new Vector2(3840, 2160);
            RawImage maskImg = uiModelMask.AddComponent<RawImage>();
            maskImg.color = new Color(0, 0, 0, 0.4f);
            uiModelMask.AddComponent<UIModelMask>();
            Button modelMaskBtn = uiModelMask.AddComponent<UIButton>();
            //Navigation navigation = modelMaskBtn.navigation;
            //navigation.mode = Navigation.Mode.None;
            //modelMaskBtn.navigation = navigation;
            modelMaskBtn.transition = Selectable.Transition.None;
            uiModelMask.AddComponent<CanvasGroup>();

            // 创建 UIContent UI组件父物体
            GameObject uiContent = new GameObject("UIContent");
            uiContent.layer |= LayerMask.NameToLayer("UI");
            RectTransform uiContentRectTrans = uiContent.AddComponent<RectTransform>();
            uiContentRectTrans.anchorMin = Vector2.zero; // 设置锚点最小值（左下角）
            uiContentRectTrans.anchorMax = Vector2.one;  // 设置锚点最大值（右上角）
            uiContentRectTrans.offsetMin = Vector2.zero; // 设置偏移的最小值（通常为0，0）
            uiContentRectTrans.offsetMax = Vector2.zero; // 设置偏移的最大值（通常为0，0）
            uiContent.transform.SetParent(newWindow.transform, false);

            // 创建UI窗口的背景RawImage
            GameObject background = new GameObject("Background");
            background.layer |= LayerMask.NameToLayer("UI");
            RectTransform backRectTrans = background.AddComponent<RectTransform>();
            RawImage backRawImg = background.AddComponent<RawImage>();
            backRectTrans.sizeDelta = GetWindowBackgroundsSize(windowType);
            if (windowType == WindowType.FirstLevel)
            {
                backRectTrans.anchorMin = Vector2.zero; // 设置锚点最小值（左下角）
                backRectTrans.anchorMax = Vector2.one;  // 设置锚点最大值（右上角）
                backRectTrans.offsetMin = Vector2.zero; // 设置偏移的最小值（通常为0，0）
                backRectTrans.offsetMax = Vector2.zero; // 设置偏移的最大值（通常为0，0）
            }
            background.transform.SetParent(uiContent.transform, false);

            // 创建UI窗口的关闭按钮
            GameObject closeBtn = new GameObject("[Button]Close");
            closeBtn.layer |= LayerMask.NameToLayer("UI");
            RectTransform closeBtnRectTrans = closeBtn.AddComponent<RectTransform>();
            closeBtnRectTrans.anchorMin = Vector2.one; // 设置锚点最小值（左下角）
            closeBtnRectTrans.anchorMax = Vector2.one;  // 设置锚点最大值（右上角）
            closeBtnRectTrans.offsetMin = Vector2.zero; // 设置偏移的最小值（通常为0，0）
            closeBtnRectTrans.offsetMax = Vector2.zero; // 设置偏移的最大值（通常为0，0）
            Image closeBtnImg = closeBtn.AddComponent<Image>();
            Sprite defaultSprite = Resources.Load<Sprite>("UI/Sprite/UIInputFieldBackground");
            if (defaultSprite != null)
            {
                closeBtnImg.sprite = defaultSprite;
            }
            Button closeButton = closeBtn.AddComponent<UIButton>();
            //navigation = closeButton.navigation;
            //navigation.mode = Navigation.Mode.None;
            //closeButton.navigation = navigation;
            closeBtnRectTrans.sizeDelta = new Vector2(80, 80);
            closeBtnRectTrans.localPosition = GetWindowCloseButtonPosition(windowType);
            closeBtn.transform.SetParent(uiContent.transform, false);

            // 创建关闭按钮的提示文本
            GameObject btnTextObj = new GameObject("Text (Legacy)");
            btnTextObj.layer |= LayerMask.NameToLayer("UI");
            RectTransform btnTextRectTrans = btnTextObj.AddComponent<RectTransform>();
            btnTextRectTrans.anchorMin = Vector2.zero; // 设置锚点最小值（左下角）
            btnTextRectTrans.anchorMax = Vector2.one;  // 设置锚点最大值（右上角）
            btnTextRectTrans.offsetMin = Vector2.zero; // 设置偏移的最小值（通常为0，0）
            btnTextRectTrans.offsetMax = Vector2.zero; // 设置偏移的最大值（通常为0，0）
            Text btnText = btnTextObj.AddComponent<Text>();
            btnText.text = "X";
            btnText.color = Color.black;
            btnText.fontSize = 60;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.raycastTarget = false;
            btnText.transform.SetParent(closeBtn.transform, false);

            // 创建UI窗口的提示文本
            string windowTipStr = GetNewWindowTips(windowType);
            GameObject windowTip = new GameObject(windowTipStr);
            windowTip.layer |= LayerMask.NameToLayer("UI");
            RectTransform windowTipRectTrans = windowTip.AddComponent<RectTransform>();
            windowTipRectTrans.sizeDelta = new Vector2(700, 150);
            Text windowTipTxt = windowTip.AddComponent<Text>();
            windowTipTxt.color = Color.black;
            windowTipTxt.fontSize = 100;
            windowTipTxt.text = windowTipStr;
            windowTipTxt.alignment = TextAnchor.MiddleCenter;
            windowTip.transform.SetParent(uiContent.transform, false);

            if (!Directory.Exists(WindowPrefabSavePath))
            {
                Directory.CreateDirectory(WindowPrefabSavePath);
            }

            // 保存为预制体
            string prefabPath = Path.Combine(WindowPrefabSavePath, windowName + ".prefab");
            int index = 1;
            while (File.Exists(prefabPath))
            {
                prefabPath = Path.Combine(WindowPrefabSavePath, windowName + $" ({index}).prefab");
                index++;
            }
            PrefabUtility.SaveAsPrefabAsset(newWindow, prefabPath);

            GameObject.DestroyImmediate(newWindow);
            newWindow = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            newWindow = PrefabUtility.InstantiatePrefab(newWindow) as GameObject;
            newWindow.transform.SetParent(uiRoot.transform, false);
            // 选择新创建的窗口
            Selection.activeGameObject = newWindow;

            // 关闭编辑器窗口
            Close();
        }

        private static GameObject CreateUIRootGameObject(bool isMenuItem)
        {
            if (File.Exists("Assets/Resources/" + UI_CONFIG_PATH + ".asset"))
            {
                UIConfig uiConfig = Resources.Load<UIConfig>(UI_CONFIG_PATH);
                if (uiConfig != null)
                {
                    WindowPrefabSavePath = uiConfig.UIWindowPrefabsSavePath;
                    UIRootPrefabSavePath = uiConfig.UIRootPrefabsSavePath;
                }
            }

            GameObject uiRoot = GameObject.Find("UIRoot");

            if (uiRoot == null)
            {
                if (!Directory.Exists(UIRootPrefabSavePath))
                {
                    Directory.CreateDirectory(UIRootPrefabSavePath);
                }
                string uiRootPath = Path.Combine(UIRootPrefabSavePath, "UIRoot.prefab");
                if (File.Exists(uiRootPath))
                {
                    uiRoot = AssetDatabase.LoadAssetAtPath<GameObject>(uiRootPath);
                    if (uiRoot != null)
                    {
                        uiRoot = PrefabUtility.InstantiatePrefab(uiRoot) as GameObject;
                    }
                }
            }
            if (isMenuItem)
            {
                if (uiRoot != null)
                {
                    Debug.LogWarning("UIRoot预制体已经存在，无需再次创建: " + UIRootPrefabSavePath + "/UIRoot.prefab");
                    return uiRoot;
                }
            }
            else
            {
                if (uiRoot != null)
                {
                    return uiRoot;
                }
            }

            uiRoot = new GameObject("UIRoot");
            uiRoot.layer |= LayerMask.NameToLayer("UI");
            uiRoot.AddComponent<RectTransform>();

            GameObject uiCamera = new GameObject("UICamera");
            uiCamera.transform.SetParent(uiRoot.transform, false);
            var camera = uiCamera.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask &= 0;
            camera.cullingMask |= 1 << LayerMask.NameToLayer("UI");

            if (!Transform.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            {
                // 创建 EventSystem 组件物体
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                eventSystemObj.transform.SetParent(uiRoot.transform, false);
            }

            // 保存为预制体
            string prefabPath = Path.Combine(UIRootPrefabSavePath, "UIRoot.prefab");
            PrefabUtility.SaveAsPrefabAsset(uiRoot, prefabPath);
            GameObject.DestroyImmediate(uiRoot);
            uiRoot = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            uiRoot = PrefabUtility.InstantiatePrefab(uiRoot) as GameObject;

            return uiRoot;
        }

        private int GetWindowSortingOrder(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.FirstLevel:
                    return 0;

                case WindowType.SecondLevel:
                    return 100;

                case WindowType.ThreeLevel:
                    return 200;

                case WindowType.FourLevel:
                    return 300;

                case WindowType.FiveLevel:
                    return 400;
            }
            return 0;
        }

        /// <summary>
        /// 获取Window临时名字
        /// </summary>
        /// <param name="windowType"></param>
        /// <returns></returns>
        private string GetNewWindowTips(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.FirstLevel:
                    return "一级窗口模板";

                case WindowType.SecondLevel:
                    return "二级窗口模板";

                case WindowType.ThreeLevel:
                    return "三级窗口模板";

                case WindowType.FourLevel:
                    return "四级窗口模板";

                case WindowType.FiveLevel:
                    return "五级窗口模板";
            }
            return "";
        }

        private int GetWindowTipsSize(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.FirstLevel:
                case WindowType.SecondLevel:
                case WindowType.ThreeLevel:
                case WindowType.FourLevel:
                    return 100;

                case WindowType.FiveLevel:
                    return 50;
            }
            return 24;
        }

        private Vector2 GetWindowBackgroundsSize(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.FirstLevel:
                    return new Vector2(1920, 1080);

                case WindowType.SecondLevel:
                    return new Vector2(1728, 972);

                case WindowType.ThreeLevel:
                    return new Vector2(1536, 864);

                case WindowType.FourLevel:
                    return new Vector2(1334, 756);

                case WindowType.FiveLevel:
                    return new Vector2(1152, 648);
            }
            return new Vector2(1920, 1080);
        }

        private Vector2 GetWindowCloseButtonPosition(WindowType windowType)
        {
            switch (windowType)
            {
                case WindowType.FirstLevel:
                    return new Vector2(-40, -40);

                case WindowType.SecondLevel:
                    return new Vector2(-136, -94);

                case WindowType.ThreeLevel:
                    return new Vector2(-232, -148);

                case WindowType.FourLevel:
                    return new Vector2(-333, -202);

                case WindowType.FiveLevel:
                    return new Vector2(-424, -256);
            }
            return new Vector2(-40, -40);
        }

        public enum WindowType
        {
            /// <summary>
            /// 一级窗口模板 sortingOrder:0~99
            /// </summary>
            FirstLevel,

            /// <summary>
            /// 二级窗口模板 sortingOrder:100~199
            /// </summary>
            SecondLevel,

            /// <summary>
            /// 三级窗口模板 ortingOrder:200~299
            /// </summary>
            ThreeLevel,

            /// <summary>
            /// 四级窗口模板 ortingOrder:300~399
            /// </summary>
            FourLevel,

            /// <summary>
            /// 五级窗口模板 ortingOrder:400~499
            /// </summary>
            FiveLevel,
        }
    }
}