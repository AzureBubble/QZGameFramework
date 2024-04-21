using UnityEngine;

namespace QZGameFramework.Utilities
{
    public class MathUtility
    {
        #region 角度和弧度互转

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="deg">角度</param>
        /// <returns></returns>
        public static float Deg2Rad(float deg)
        {
            return deg * Mathf.Deg2Rad;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="deg">弧度</param>
        /// <returns></returns>
        public static float Rad2Deg(float rad)
        {
            return rad * Mathf.Rad2Deg;
        }

        #endregion

        #region 距离判断

        /// <summary>
        /// 获取 XZ 平面上 两点的距离
        /// </summary>
        /// <param name="a">a点</param>
        /// <param name="b">b点</param>
        /// <returns></returns>
        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            a.y = 0;
            b.y = 0;
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// 判断 XZ 平面上的两点距离 是否小于等于目标距离
        /// </summary>
        /// <param name="a">a点</param>
        /// <param name="b">b点</param>
        /// <param name="distance">目标距离</param>
        /// <returns></returns>
        public static bool CheckDistanceXZ(Vector3 a, Vector3 b, float distance)
        {
            return DistanceXZ(a, b) <= distance;
        }

        /// <summary>
        /// 获取 XY 平面上 两点的距离
        /// </summary>
        /// <param name="a">a点</param>
        /// <param name="b">b点</param>
        /// <returns></returns>
        public static float DistanceXY(Vector3 a, Vector3 b)
        {
            a.z = 0;
            b.z = 0;
            return Vector3.Distance(a, b);
        }

        /// <summary>
        /// 判断 XY 平面上的两点距离 是否小于等于目标距离
        /// </summary>
        /// <param name="a">a点</param>
        /// <param name="b">b点</param>
        /// <param name="distance">目标距离</param>
        /// <returns></returns>
        public static bool CheckDistanceXY(Vector3 a, Vector3 b, float distance)
        {
            return DistanceXY(a, b) <= distance;
        }

        #endregion

        #region 位置判断

        /// <summary>
        /// 判断世界坐标的某个点 是否在屏幕可见范围外
        /// </summary>
        /// <param name="pos">世界坐标的点</param>
        /// <returns>可见范围内-false 可见范围外-true</returns>
        public static bool CheckWorldPosOutOfScreen(Vector3 pos)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(pos);

            if (screenPos.x >= 0 && screenPos.x <= Screen.width &&
                screenPos.y >= 0 && screenPos.y <= Screen.height)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判断世界物体是否超出相机的远近平面
        /// </summary>
        /// <param name="worldPos">世界物体坐标</param>
        /// <param name="camera">相对相机</param>
        /// <returns></returns>
        public static bool CheckWorldObjectOutOfCameraView(Vector3 worldPos, Camera camera)
        {
            float nearClip = camera.nearClipPlane;
            float farClip = camera.farClipPlane;

            float distance = Vector3.Distance(worldPos, camera.transform.position);
            if (distance < nearClip || distance > farClip)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 判断世界物体是否在相机的视锥体内
        /// </summary>
        /// <param name="objTransform">世界物体</param>
        /// <param name="camera">相对相机</param>
        /// <returns></returns>
        public static bool CheckWorldObjectInCameraFrustum(Transform objTransform, Camera camera)
        {
            // 获取相机的视锥体平面
            Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(camera);

            // 获取物体的边界框
            // 使用 Renderer 的边界框
            Bounds objBounds = objTransform.GetComponent<Renderer>().bounds;
            // 或者使用 Collider 的边界框
            // Bounds objBounds = objTransform.GetComponent<Collider>().bounds;

            // 判断物体的边界框是否在相机的视锥体内部
            return GeometryUtility.TestPlanesAABB(frustumPlanes, objBounds);
        }

        /// <summary>
        /// 判断点 B 是否在点 A 的扇形范围内 XZ 平面
        /// </summary>
        /// <param name="a">a点</param>
        /// <param name="forward">a点面朝向</param>
        /// <param name="b">b点</param>
        /// <param name="radius">扇形半径</param>
        /// <param name="angle">扇形角度</param>
        /// <returns>范围内-true 范围外-false</returns>
        public static bool CheckBIsInSectorRangeOfAAboutXZ(Vector3 a, Vector3 forward, Vector3 b, float radius, float angle)
        {
            a.y = 0;
            forward.y = 0;
            b.y = 0;

            return CheckDistanceXZ(a, b, radius) && Vector3.Angle(forward, b - a) <= angle / 2f;
        }

        #endregion

        #region 世界坐标转UI坐标

        /// <summary>
        /// 世界坐标转UI窗口相对坐标
        /// </summary>
        /// <param name="canvas">UI父物体</param>
        /// <param name="worldPos">世界坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <param name="offset">偏移位置</param>
        /// <returns>相对于UI父物体的坐标</returns>
        public static Vector3 WorldPointToUILocalPoint(RectTransform canvas, Vector3 worldPos, Camera uiCamera, Vector3 offset = default(Vector3))
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            Vector2 uiLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPoint, uiCamera, out uiLocalPoint);
            return new Vector3(uiLocalPoint.x, uiLocalPoint.y, 0) + offset;
        }

        /// <summary>
        /// UI物体跟随世界物体移动
        /// </summary>
        /// <param name="uiObj">UI物体</param>
        /// <param name="canvas">UI物体的父级Canvas</param>
        /// <param name="targetObj">跟随的世界物体</param>
        /// <param name="uiCamera">UI相机</param>
        /// <param name="originTargetPos">世界物体初始位置</param>
        /// <param name="originalDistance">世界物体相对于主相机的初始距离</param>
        /// <param name="offset">世界物体和UI物体的初始偏移值</param>
        /// <param name="followZoom">近大远小开关</param>
        public static void UIObjectFollowWorldObject(GameObject uiObj, RectTransform canvas, GameObject targetObj, Camera uiCamera, Vector3 originTargetPos, float originalDistance, Vector3 offset, bool followZoom = false)
        {
            if (originTargetPos != targetObj.transform.position)
            {
                if (uiObj.TryGetComponent<RectTransform>(out RectTransform uiObjTrans))
                {
                    if (!CheckWorldObjectInCameraFrustum(targetObj.transform, Camera.main) || CheckWorldObjectOutOfCameraView(targetObj.transform.position, Camera.main))
                    {
                        uiObj.SetActive(false);
                        return;
                    }
                    else
                    {
                        uiObj.SetActive(true);
                    }
                    if (followZoom)
                    {
                        float zoomFactor = originalDistance / Vector3.Distance(Camera.main.transform.position, targetObj.transform.position);
                        uiObjTrans.localPosition = WorldPointToUILocalPoint(canvas, targetObj.transform.position, uiCamera, offset * zoomFactor);
                        uiObjTrans.localScale = Vector3.one * zoomFactor;
                    }
                    else
                    {
                        uiObjTrans.localPosition = WorldPointToUILocalPoint(canvas, targetObj.transform.position, uiCamera, offset);
                    }
                }
            }
        }

        /// <summary>
        /// 获得UI物体跟随世界物体移动的UI坐标和近大远小缩放值
        /// </summary>
        /// <param name="canvas">UI物体的父级Canvas</param>
        /// <param name="worldPos">世界物体坐标</param>
        /// <param name="uiCamera">UI相机</param>
        /// <param name="originalDistance">世界物体相对于主相机的初始距离</param>
        /// <param name="offset">世界物体和UI物体的初始偏移值</param>
        /// <returns>Vector3:世界物体相对UI窗口的坐标  float:近大远小的缩放值</returns>
        public static (Vector3, float) UIObjectFollowWorldObject(RectTransform canvas, Vector3 worldPos, Camera uiCamera, float originalDistance, Vector3 offset)
        {
            float zoomFactor = originalDistance / Vector3.Distance(Camera.main.transform.position, worldPos);

            return (WorldPointToUILocalPoint(canvas, worldPos, uiCamera, offset * zoomFactor), zoomFactor);
        }

        #endregion
    }
}