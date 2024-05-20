using QZGameFramework.Utilities;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public static class ExtensionUtil
{
    //public static void RayCastAll(this Ray ray, float maxDistance, int layerMask, UnityAction<GameObject> action)
    //{
    //    MathUtil.RayCastAll(ray, action, maxDistance, layerMask);
    //}

    /// <summary>
    /// UI物体跟随世界物体移动
    /// </summary>
    /// <param name="uiObj">UI物体</param>
    /// <param name="canvas">UI物体的父级Canvas</param>
    /// <param name="targetObjPos">跟随的世界物体坐标值</param>
    /// <param name="uiCamera">UI相机</param>
    /// <param name="originalDistance">世界物体相对于主相机的初始距离</param>
    /// <param name="offset">偏移位置<param>
    /// <param name="followZoom">近大远小开关</param>
    /// <returns>UI物体的缩放值</returns>
    public static float FollowWorldGameObject(this RectTransform rectTransform, RectTransform canvas, Vector3 targetObjPos, Camera uiCamera, float originalDistance, Vector3 offset = default(Vector3), bool followZoom = false)
    {
        return UGUIUtil.UIObjectFollowWorldObject(rectTransform, canvas, targetObjPos, uiCamera, originalDistance, offset, followZoom);
    }

    /// <summary>
    /// 添加自定义事件监听器到指定控件的 EventTrigger 上。
    /// </summary>
    /// <param name="control">要添加监听器的控件</param>
    /// <param name="type">事件类型</param>
    /// <param name="action">回调函数</param>
    public static void AddCustomEventListener(this UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        UGUIUtil.AddCustomEventListener(control, type, action);
    }

    /// <summary>
    /// 移除自定义事件监听器到指定控件的 EventTrigger 上。
    /// </summary>
    /// <param name="control">要添加监听器的控件</param>
    /// <param name="type">事件类型</param>
    /// <param name="action">回调函数</param>
    public static void RemoveCustomEventListener(this UIBehaviour control, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        UGUIUtil.RemoveCustomEventListener(control, type, action);
    }
}