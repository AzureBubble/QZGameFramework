using UnityEngine;

namespace QZGameFramework.Utilities
{
    public class MathUtil
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
    }
}