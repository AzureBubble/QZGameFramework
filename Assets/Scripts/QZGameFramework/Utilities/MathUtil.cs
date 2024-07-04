using System;
using UnityEngine;
using UnityEngine.Events;

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

        #region 射线检测相关

        #region 3D

        /// <summary>
        /// 射线检测 获取一个对象 指定距离 指定层级的
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCast3D(Ray ray, float maxDistance, int layerMask, UnityAction<RaycastHit> callBack = null)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            {
                callBack?.Invoke(hitInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 射线检测 获取一个对象 指定距离 指定层级的
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCast3D<T>(Ray ray, float maxDistance, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, maxDistance, layerMask))
            {
                Type type = typeof(T);
                if (type == typeof(GameObject))
                {
                    callBack?.Invoke(hitInfo.collider.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 射线检测 获取到多个对象 指定距离 指定层级
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数 每一个对象都会调用一次</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCastAll3D(Ray ray, float maxDistance, int layerMask, UnityAction<RaycastHit> callBack = null)
        {
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
            if (hitInfos.Length > 0)
            {
                for (int i = 0; i < hitInfos.Length; i++)
                {
                    callBack?.Invoke(hitInfos[i]);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 射线检测 获取到多个对象 指定距离 指定层级
        /// </summary>
        /// <param name="ray">射线</param>
        /// <param name="callBack">回调函数 每一个对象都会调用一次</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCastAll3D<T>(Ray ray, float maxDistance, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            RaycastHit[] hitInfos = Physics.RaycastAll(ray, maxDistance, layerMask);
            Type type = typeof(T);
            if (hitInfos.Length > 0)
            {
                if (type == typeof(GameObject))
                {
                    for (int i = 0; i < hitInfos.Length; i++)
                    {
                        callBack?.Invoke(hitInfos[i].collider.gameObject as T);
                    }
                }
                else
                {
                    for (int i = 0; i < hitInfos.Length; i++)
                    {
                        callBack?.Invoke(hitInfos[i].collider.gameObject.GetComponent<T>());
                    }
                }
                return true;
            }

            return false;
        }

        #endregion

        #region 2D

        /// <summary>
        /// 射线检测 获取一个对象 指定距离 指定层级的
        /// </summary>
        /// <param name="origin">起始点</param>
        /// <param name="direction">方向</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCast2D(Vector2 origin, Vector2 direction, float maxDistance, int layerMask, UnityAction<RaycastHit2D> callBack = null)
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, maxDistance, layerMask);
            if (hitInfo)
            {
                callBack?.Invoke(hitInfo);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 射线检测 获取一个对象 指定距离 指定层级的
        /// </summary>
        /// <param name="origin">起始点</param>
        /// <param name="direction">方向</param>
        /// <param name="callBack">回调函数</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCast2D<T>(Vector2 origin, Vector2 direction, float maxDistance, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            RaycastHit2D hitInfo = Physics2D.Raycast(origin, direction, maxDistance, layerMask);
            if (hitInfo)
            {
                Type type = typeof(T);
                if (type == typeof(GameObject))
                {
                    callBack?.Invoke(hitInfo.collider.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(hitInfo.collider.gameObject.GetComponent<T>());
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 射线检测 获取到多个对象 指定距离 指定层级
        /// </summary>
        /// <param name="origin">起始点</param>
        /// <param name="direction">方向</param>
        /// <param name="callBack">回调函数 每一个对象都会调用一次</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCastAll2D(Vector2 origin, Vector2 direction, float maxDistance, int layerMask, UnityAction<RaycastHit2D> callBack = null)
        {
            RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, direction, maxDistance, layerMask);
            if (hitInfos.Length > 0)
            {
                for (int i = 0; i < hitInfos.Length; i++)
                {
                    callBack?.Invoke(hitInfos[i]);
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 射线检测 获取到多个对象 指定距离 指定层级
        /// </summary>
        /// <param name="origin">起始点</param>
        /// <param name="direction">方向</param>
        /// <param name="callBack">回调函数 每一个对象都会调用一次</param>
        /// <param name="maxDistance">最大距离</param>
        /// <param name="layerMask">层级筛选</param>
        public static bool RayCastAll2D<T>(Vector2 origin, Vector2 direction, float maxDistance, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            RaycastHit2D[] hitInfos = Physics2D.RaycastAll(origin, direction, maxDistance, layerMask);
            Type type = typeof(T);
            if (hitInfos.Length > 0)
            {
                if (type == typeof(GameObject))
                {
                    for (int i = 0; i < hitInfos.Length; i++)
                    {
                        callBack?.Invoke(hitInfos[i].collider.gameObject as T);
                    }
                }
                else
                {
                    for (int i = 0; i < hitInfos.Length; i++)
                    {
                        callBack?.Invoke(hitInfos[i].collider.gameObject.GetComponent<T>());
                    }
                }
                return true;
            }

            return false;
        }

        #endregion

        #endregion

        #region 范围检测

        #region 3D

        /// <summary>
        /// 进行盒装范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">盒装中心点</param>
        /// <param name="rotation">盒子的角度</param>
        /// <param name="halfExtents">长宽高的一半</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数 </param>
        public static bool OverlapBox3D<T>(Vector3 center, Quaternion rotation, Vector3 halfExtents, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider[] colliders = Physics.OverlapBox(center, halfExtents, rotation, layerMask, QueryTriggerInteraction.Collide);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colliders[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colliders[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colliders[i].gameObject.GetComponent<T>());
                    }
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行球体范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">球体的中心点</param>
        /// <param name="radius">球体的半径</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数</param>
        public static bool OverlapSphere3D<T>(Vector3 center, float radius, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider[] colliders = Physics.OverlapSphere(center, radius, layerMask, QueryTriggerInteraction.Collide);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colliders[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colliders[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colliders[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行椭圆体范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="point0">第一个点</param>
        /// <param name="point1">第二个点</param>
        /// <param name="radius">半径</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数</param>
        public static bool OverlapCapsule3D<T>(Vector3 point0, Vector3 point1, float radius, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider[] colliders = Physics.OverlapCapsule(point0, point1, radius, layerMask, QueryTriggerInteraction.Collide);
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colliders[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colliders[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colliders[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        #region 2D

        /// <summary>
        /// 进行盒装范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">盒装中心点</param>
        /// <param name="rotation">盒子的角度</param>
        /// <param name="halfExtents">长宽高的一半</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数 </param>
        public static bool OverlapBox2D<T>(Vector2 center, Vector2 size, float angle, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D coll = Physics2D.OverlapBox(center, size, angle, layerMask);
            if (coll)
            {
                if (type == typeof(Collider))
                {
                    callBack?.Invoke(coll as T);
                }
                else if (type == typeof(GameObject))
                {
                    callBack?.Invoke(coll.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(coll.gameObject.GetComponent<T>());
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行盒装范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">盒装中心点</param>
        /// <param name="size">盒子大小<param>
        /// <param name="angle">盒子角度<param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数 </param>
        public static bool OverlapBoxAll2D<T>(Vector2 center, Vector2 size, float angle, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D[] colls = Physics2D.OverlapBoxAll(center, size, angle, layerMask);
            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colls[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colls[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colls[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行球体范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">球体的中心点</param>
        /// <param name="radius">球体的半径</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数</param>
        public static bool OverlapSphere2D<T>(Vector2 center, float radius, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D coll = Physics2D.OverlapCircle(center, radius, layerMask);
            if (coll)
            {
                if (type == typeof(Collider))
                {
                    callBack?.Invoke(coll as T);
                }
                else if (type == typeof(GameObject))
                {
                    callBack?.Invoke(coll.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(coll.gameObject.GetComponent<T>());
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行球体范围检测
        /// </summary>
        /// <typeparam name="T">想要获取的信息类型 可以填写 Collider GameObject 以及对象上依附的组件类型</typeparam>
        /// <param name="center">球体的中心点</param>
        /// <param name="radius">球体的半径</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack">回调函数</param>
        public static bool OverlapSphereAll2D<T>(Vector2 center, float radius, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D[] colls = Physics2D.OverlapCircleAll(center, radius, layerMask);
            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colls[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colls[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colls[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行区域范围检测
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point0">左上角的点</param>
        /// <param name="point1">右下角的点</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool OverlapArea2D<T>(Vector2 point0, Vector2 point1, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D coll = Physics2D.OverlapArea(point0, point1, layerMask);
            if (coll)
            {
                if (type == typeof(Collider))
                {
                    callBack?.Invoke(coll as T);
                }
                else if (type == typeof(GameObject))
                {
                    callBack?.Invoke(coll.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(coll.gameObject.GetComponent<T>());
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行区域范围检测
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point0">左上角的点</param>
        /// <param name="point1">右下角的点</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool OverlapBoxAll2D<T>(Vector2 point0, Vector2 point1, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D[] colls = Physics2D.OverlapAreaAll(point0, point1, layerMask);
            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colls[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colls[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colls[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行点检测
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point0">点</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool OverlapArea2D<T>(Vector2 point0, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D coll = Physics2D.OverlapPoint(point0, layerMask);
            if (coll)
            {
                if (type == typeof(Collider))
                {
                    callBack?.Invoke(coll as T);
                }
                else if (type == typeof(GameObject))
                {
                    callBack?.Invoke(coll.gameObject as T);
                }
                else
                {
                    callBack?.Invoke(coll.gameObject.GetComponent<T>());
                }

                return true;
            }
            return false;
        }

        /// <summary>
        /// 进行点检测
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="point0">点</param>
        /// <param name="layerMask">层级筛选</param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        public static bool OverlapBoxAll2D<T>(Vector2 point0, int layerMask, UnityAction<T> callBack = null) where T : class
        {
            Type type = typeof(T);
            Collider2D[] colls = Physics2D.OverlapPointAll(point0, layerMask);
            if (colls.Length > 0)
            {
                for (int i = 0; i < colls.Length; i++)
                {
                    if (type == typeof(Collider))
                    {
                        callBack?.Invoke(colls[i] as T);
                    }
                    else if (type == typeof(GameObject))
                    {
                        callBack?.Invoke(colls[i].gameObject as T);
                    }
                    else
                    {
                        callBack?.Invoke(colls[i].gameObject.GetComponent<T>());
                    }
                }
                return true;
            }
            return false;
        }

        #endregion

        #endregion

        #region 颜色转换

        public static Color UintToColor(uint color)
        {
            uint r = (color >> 16) & 255;
            uint g = (color >> 8) & 255;
            uint b = (color >> 0) & 255;

            return new Color32((byte)r, (byte)g, (byte)b, 255);
        }

        public static Color UintToColorWithAlpha(uint color)
        {
            uint r = (color >> 24) & 255;
            uint g = (color >> 16) & 255;
            uint b = (color >> 8) & 255;
            uint a = (color >> 0) & 255;

            return new Color32((byte)r, (byte)g, (byte)b, (byte)a);
        }

        #endregion
    }
}
