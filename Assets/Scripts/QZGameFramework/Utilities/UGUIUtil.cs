using QZGameFramework.Utilities;
using UnityEngine;

namespace QZGameFramework.Utilities
{
    public class UGUIUtil
    {
        #region 世界坐标转UI坐标

        /// <summary>
        /// 世界坐标转UI窗口相对坐标
        /// </summary>
        /// <param name="canvas">UI父物体</param>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <param name="offset">偏移位置</param>
        /// <returns>相对于UI父物体的坐标</returns>
        public static Vector3 WorldPointToUILocalPoint(RectTransform fatherCanvas, Vector3 worldPos, Camera uiCamera, Vector3 offset = default(Vector3))
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos + offset);
            Vector2 uiLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fatherCanvas, screenPoint, uiCamera, out uiLocalPoint);
            return new Vector3(uiLocalPoint.x, uiLocalPoint.y, 0);
        }

        /// <summary>
        /// 获得UI物体跟随世界物体移动的UI坐标
        /// </summary>
        /// <param name="canvas">UI物体的父级Canvas</param>
        /// <param name="worldPos">世界物体坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <param name="offset">世界物体和UI物体的初始偏移值</param>
        /// <returns>世界物体相对UI窗口的坐标</returns>
        public static Vector3 UIObjectFollowWorldObject(RectTransform canvas, Vector3 worldPos, Camera uiCamera, Vector3 offset = default(Vector3))
        {
            return WorldPointToUILocalPoint(canvas, worldPos, uiCamera, offset);
        }

        /// <summary>
        /// 获得UI物体跟随世界物体移动的近大远小缩放值
        /// </summary>
        /// <param name="worldPos">世界物体坐标</param>
        /// <param name="originalDistance">世界物体相对于主相机的初始距离</param>
        /// <returns>近大远小的Scale缩放值</returns>
        public static float UIObjectFollowWorldObjectFactor(Vector3 worldPos, float originalDistance, Camera camera = null)
        {
            if (camera == null)
            {
                return originalDistance / MathUtil.DistanceXZ(Camera.main.transform.position, worldPos);
            }
            else
            {
                return originalDistance / MathUtil.DistanceXZ(camera.transform.position, worldPos);
            }

            //Vector3.Distance(Camera.main.transform.position, worldPos);
        }

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
        public static float UIObjectFollowWorldObject(RectTransform uiObj, RectTransform canvas, Vector3 targetObjPos, Camera uiCamera, float originalDistance, Vector3 offset = default(Vector3), bool followZoom = false)
        {
            if (uiObj != null)
            {
                if (followZoom)
                {
                    float zoomFactor = UIObjectFollowWorldObjectFactor(targetObjPos, originalDistance);
                    // TODO:当缩放值 > 10 || <= 0.1f 时 不再对UI物体进行赋值
                    if (zoomFactor > 10 || zoomFactor <= 0.1f)
                    {
                        return zoomFactor;
                    }
                    uiObj.localPosition = UIObjectFollowWorldObject(canvas, targetObjPos, uiCamera, offset);
                    //uiObjTrans.localPosition = WorldPointToUILocalPoint(canvas, targetObj.transform.position, uiCamera, offset * zoomFactor);
                    uiObj.localScale = (Vector3.one * zoomFactor);
                    return zoomFactor;
                }
                else
                {
                    uiObj.localPosition = WorldPointToUILocalPoint(canvas, targetObjPos, uiCamera, offset);
                    return 1f;
                }
            }

            return 0f;
        }

        #endregion
    }
}