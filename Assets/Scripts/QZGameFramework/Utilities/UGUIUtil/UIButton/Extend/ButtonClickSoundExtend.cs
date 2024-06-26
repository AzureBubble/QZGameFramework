using QZGameFramework.MusicManager;
using UnityEngine;

namespace QZGameFramework.Utilities.UGUIUtil
{
    [System.Serializable]
    public class ButtonClickSoundExtend
    {
        [SerializeField] private bool m_isUseClickSound;

        //[SerializeField] private int m_clickSoundId = 0;

        [Tooltip("默认加载路径是Resources下，如需更改加载方式请自行修改源码"), SerializeField] private string m_clickSoundPath = "ButtonClick";

        /// <summary>
        /// 按下
        /// </summary>
        /// <param name="transf"></param>
        public void OnPointerDown(Transform transf)
        {
        }

        /// <summary>
        /// 抬起
        /// </summary>
        /// <param name="transf"></param>
        public void OnPointerUp(BaseUIButton uiButton)
        {
        }

        /// <summary>
        /// 按钮点击事件
        /// </summary>
        /// <returns></returns>
        public void OnButtonClick()
        {
            if (m_isUseClickSound)
            {
                //TODO: 自定义播放音效的方式
                AudioSource clickSound = MusicMgr.Instance.PlaySoundMusic(m_clickSoundPath, false, "");
                MusicMgr.Instance.StopSoundMusicWaitUntilClipOver(clickSound).Forget();
            }
        }
    }
}