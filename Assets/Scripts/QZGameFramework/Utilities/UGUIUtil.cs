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
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            Vector2 uiLocalPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(fatherCanvas, screenPoint, uiCamera, out uiLocalPoint);
            return new Vector3(uiLocalPoint.x, uiLocalPoint.y, 0) + offset;
        }

        /// <summary>
        /// ���UI����������������ƶ���UI����
        /// </summary>
        /// <param name="canvas">UI����ĸ���Canvas</param>
        /// <param name="worldPos">������������</param>
        /// <param name="uiCamera">UI���</param>
        /// <param name="offset">���������UI����ĳ�ʼƫ��ֵ</param>
        /// <returns>�����������UI���ڵ�����</returns>
        public static Vector3 UIObjectFollowWorldObject(RectTransform canvas, Vector3 worldPos, Camera uiCamera, Vector3 offset)
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
        /// <param name="targetObj">�������������</param>
        /// <param name="uiCamera">UI���</param>
        /// <param name="originalDistance">�������������������ĳ�ʼ����</param>
        /// <param name="offset">���������UI����ĳ�ʼƫ��ֵ</param>
        /// <param name="followZoom">����ԶС����</param>
        /// <returns>UI���������ֵ</returns>
        public static float UIObjectFollowWorldObject(GameObject uiObj, RectTransform canvas, GameObject targetObj, Camera uiCamera, float originalDistance, Vector3 offset, bool followZoom = false)
        {
            if (uiObj.TryGetComponent<RectTransform>(out RectTransform uiObjTrans))
            {
                if (followZoom)
                {
                    float zoomFactor = UIObjectFollowWorldObjectFactor(targetObj.transform.position, originalDistance);//originalDistance / Vector3.Distance(Camera.main.transform.position, targetObj.transform.position);
                    uiObjTrans.localPosition = UIObjectFollowWorldObject(canvas, targetObj.transform.position, uiCamera, offset * zoomFactor);
                    //uiObjTrans.localPosition = WorldPointToUILocalPoint(canvas, targetObj.transform.position, uiCamera, offset * zoomFactor);
                    uiObjTrans.localScale = Vector3.one * zoomFactor;
                    return zoomFactor;
                }
                else
                {
                    uiObjTrans.localPosition = WorldPointToUILocalPoint(canvas, targetObj.transform.position, uiCamera, offset);
                    return 1f;
                }
            }

            return 0f;
        }

        #endregion
    }
}