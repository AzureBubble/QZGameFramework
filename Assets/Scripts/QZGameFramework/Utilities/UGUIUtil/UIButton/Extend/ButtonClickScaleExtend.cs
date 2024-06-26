using UnityEngine;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class ButtonClickScaleExtend
    {
        [SerializeField] private bool m_isUseClickScale = true;
        public bool UseClickScale => m_isUseClickScale;
        [SerializeField] private Vector3 m_normalScale = Vector3.one;
        [SerializeField] private Vector3 m_clickScale = new Vector3(0.9f, 0.9f, 0.9f);

        public void OnPointerDown(Transform transf, bool interactable)
        {
            if (m_isUseClickScale && interactable)
            {
                transf.localScale = m_clickScale;
            }
        }

        public void OnPointerUp(Transform transf, bool interactable)
        {
            if (m_isUseClickScale && interactable)
            {
                transf.localScale = m_normalScale;
            }
        }
    }
}