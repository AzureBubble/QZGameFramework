using QZGameFramework.Utilities;
using UnityEngine;

namespace QZGameFramework.Utilities
{
    public class UGUIUtil
    {
        #region ��������תUI����

        /// <summary>
        /// ��������תUI�����������
        /// </summary>
        /// <param name="canvas">UI������</param>
        /// <param name="worldPos">��������</param>
        /// <param name="uiCamera">UI���</param>
        /// <param name="offset">ƫ��λ��</param>
        /// <returns>�����UI�����������</returns>
        public static Vector3 WorldPointToUILocalPoint(RectTransform fatherCanvas, Vector3 worldPos, Camera uiCamera, Vector3 offset = default(Vector3))
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos + offset);
            Vector2 uiLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fatherCanvas, screenPoint, uiCamera, out uiLocalPoint);
            return new Vector3(uiLocalPoint.x, uiLocalPoint.y, 0);
        }

        /// <summary>
        /// ���UI����������������ƶ���UI����
        /// </summary>
        /// <param name="canvas">UI����ĸ���Canvas</param>
        /// <param name="worldPos">������������</param>
        /// <param name="uiCamera">UI���</param>
        /// <param name="offset">���������UI����ĳ�ʼƫ��ֵ</param>
        /// <returns>�����������UI���ڵ�����</returns>
        public static Vector3 UIObjectFollowWorldObject(RectTransform canvas, Vector3 worldPos, Camera uiCamera, Vector3 offset = default(Vector3))
        {
            return WorldPointToUILocalPoint(canvas, worldPos, uiCamera, offset);
        }

        /// <summary>
        /// ���UI����������������ƶ��Ľ���ԶС����ֵ
        /// </summary>
        /// <param name="worldPos">������������</param>
        /// <param name="originalDistance">�������������������ĳ�ʼ����</param>
        /// <returns>����ԶС��Scale����ֵ</returns>
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
        /// UI����������������ƶ�
        /// </summary>
        /// <param name="uiObj">UI����</param>
        /// <param name="canvas">UI����ĸ���Canvas</param>
        /// <param name="targetObjPos">�����������������ֵ</param>
        /// <param name="uiCamera">UI���</param>
        /// <param name="originalDistance">�������������������ĳ�ʼ����</param>
        /// <param name="offset">ƫ��λ��<param>
        /// <param name="followZoom">����ԶС����</param>
        /// <returns>UI���������ֵ</returns>
        public static float UIObjectFollowWorldObject(RectTransform uiObj, RectTransform canvas, Vector3 targetObjPos, Camera uiCamera, float originalDistance, Vector3 offset = default(Vector3), bool followZoom = false)
        {
            if (uiObj != null)
            {
                if (followZoom)
                {
                    float zoomFactor = UIObjectFollowWorldObjectFactor(targetObjPos, originalDistance);
                    // TODO:������ֵ > 10 || <= 0.1f ʱ ���ٶ�UI������и�ֵ
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