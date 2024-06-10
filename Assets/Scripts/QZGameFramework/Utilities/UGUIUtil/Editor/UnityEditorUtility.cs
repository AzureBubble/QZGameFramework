using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace QZGameFramework.Utilities.UGUIUtil
{
    public class UnityEditorUtility
    {
        /// <summary>
        /// 设置新创建的UI物体作为 Canvas 的子物体
        /// </summary>
        /// <param name="uiComponent">创建的UI对象</param>
        public static void ResetInCanvasFor(RectTransform uiComponent)
        {
            uiComponent.SetParent(Selection.activeTransform);
            if (!InCanvas(uiComponent))
            {
                // 如果不存在具有 Canvas 组件的父物体 则创建或者查找场景中存在的
                Transform canvasTrans = GetCreateCanvas();
                uiComponent.SetParent(canvasTrans);
            }
            if (!Transform.FindObjectOfType<UnityEngine.EventSystems.EventSystem>())
            {
                // 创建 EventSystem 组件物体
                GameObject eventSystemObj = new GameObject("EventSystem");
                eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
                eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }
            // 重置 UI Text 缩放和位置
            uiComponent.localScale = Vector3.one;
            uiComponent.localPosition = new Vector3(uiComponent.localPosition.x, uiComponent.localPosition.y, 0f);
            Selection.activeGameObject = uiComponent.gameObject;
        }

        /// <summary>
        /// 判断是否存在具有 Canvas 组件的父物体
        /// </summary>
        /// <param name="transf">子物体</param>
        /// <returns></returns>
        public static bool InCanvas(Transform transf)
        {
            // 查是否存在具有 Canvas 组件的父物体
            while (transf.parent)
            {
                transf = transf.parent;
                if (transf.GetComponent<Canvas>())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取或者创建 一个带有 Canvas 组件的物体
        /// </summary>
        /// <returns></returns>
        public static Transform GetCreateCanvas()
        {
            // 查找场景中是否存在 Canvas 组件物体
            Canvas canvas = Object.FindObjectOfType<Canvas>();
            if (canvas)
            {
                return canvas.transform;
            }
            else
            {
                // 不存在则创建一个 Canvas 组件物体
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                return canvasObj.transform;
            }
        }
    }
}