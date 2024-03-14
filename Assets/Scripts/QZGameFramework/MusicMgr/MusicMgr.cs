using QZGameFramework.ObjectPoolManager;
using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.MusicManager
{
    /// <summary>
    /// 音乐音效管理器
    /// </summary>
    public class MusicMgr : Singleton<MusicMgr>
    {
        private GameObject musicMgrObj;

        // 游戏背景音乐
        private AudioSource gameMusic = null;

        private float gameMusicVolume = 1f;
        //private bool gameMusicIsMute = false;

        // 游戏环境音
        private AudioSource ambientMusic = null;

        private float ambientMusicVolume = 1f;
        //private bool ambientMusicIsMute = false;

        // 游戏音效
        private List<AudioSource> soundList;

        private float soundMusicVolume = 1f;
        private bool soundMusicIsMute = false;

        public override void Initialize()
        {
            GameObject obj = new GameObject("Music Manager");
            GameObject.DontDestroyOnLoad(obj);
            musicMgrObj = obj;
            soundList = new List<AudioSource>(10);
        }

        #region 游戏背景音乐

        /// <summary>
        /// 异步 播放背景音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        /// <param name="path">资源路径</param>
        public void PlayGameMusicAsync(string name, string path = "Music/BK/")
        {
            if (gameMusic == null)
            {
                GameObject gameMusicObj = new GameObject("Game Music");
                gameMusic = gameMusicObj.AddComponent<AudioSource>();
                gameMusicObj.transform.SetParent(musicMgrObj.transform, false);
            }
            ResourcesMgr.Instance.LoadResAsync<AudioClip>(path + name, (clip) =>
            {
                gameMusic.clip = clip;
                gameMusic.loop = true;
                gameMusic.volume = gameMusicVolume;
                gameMusic.Play();
            });
        }

        /// <summary>
        /// 同步 播放背景音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        /// <param name="path">资源路径</param>
        public void PlayGameMusic(string name, string path = "Music/BK/")
        {
            if (gameMusic == null)
            {
                GameObject gameMusicObj = new GameObject("Game Music");
                gameMusic = gameMusicObj.AddComponent<AudioSource>();
                gameMusicObj.transform.SetParent(musicMgrObj.transform, false);
            }
            AudioClip clip = ResourcesMgr.Instance.LoadRes<AudioClip>(path + name);
            gameMusic.clip = clip;
            gameMusic.loop = true;
            gameMusic.volume = gameMusicVolume;
            gameMusic.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void PauseGameMusic(string name)
        {
            gameMusic?.Pause();
        }

        /// <summary>
        /// 停止播放背景音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void StopGameMusic(string name)
        {
            gameMusic?.Stop();
        }

        public void ChangeGameMusicVolume(float volume)
        {
            gameMusicVolume = volume;
            if (gameMusic != null)
            {
                gameMusic.volume = gameMusicVolume;
            }
        }

        /// <summary>
        /// 这是背景音乐是否静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetGameMusicMute(bool isMute)
        {
            if (gameMusic != null)
            {
                gameMusic.mute = isMute;
            }
        }

        #endregion

        #region 游戏环境音乐

        /// <summary>
        /// 异步 播放环境音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        /// <param name="path">资源路径</param>
        public void PlayAmbientMusicAsync(string name, string path = "Music/Ambient/")
        {
            if (ambientMusic == null)
            {
                GameObject ambientMusicObj = new GameObject("Ambient Music");
                ambientMusic = ambientMusicObj.AddComponent<AudioSource>();
                ambientMusicObj.transform.SetParent(musicMgrObj.transform, false);
            }
            ResourcesMgr.Instance.LoadResAsync<AudioClip>(path + name, (clip) =>
            {
                ambientMusic.clip = clip;
                ambientMusic.loop = true;
                ambientMusic.volume = ambientMusicVolume;
                ambientMusic.Play();
            });
        }

        /// <summary>
        /// 同步 播放环境音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        /// <param name="path">资源路径</param>
        public void PlayAmbientMusic(string name, string path = "Music/Ambient/")
        {
            if (ambientMusic == null)
            {
                GameObject ambientMusicObj = new GameObject("Ambient Music");
                ambientMusic = ambientMusicObj.AddComponent<AudioSource>();
                ambientMusicObj.transform.SetParent(musicMgrObj.transform, false);
            }
            AudioClip clip = ResourcesMgr.Instance.LoadRes<AudioClip>(path + name);
            ambientMusic.clip = clip;
            ambientMusic.loop = true;
            ambientMusic.volume = ambientMusicVolume;
            ambientMusic.Play();
        }

        /// <summary>
        /// 暂停播放环境音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void PauseAmbientMusic(string name)
        {
            ambientMusic?.Pause();
        }

        /// <summary>
        /// 停止播放环境音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void StopAmbientMusic(string name)
        {
            ambientMusic?.Stop();
        }

        public void ChangeAmbientMusicVolume(float volume)
        {
            ambientMusicVolume = volume;
            if (ambientMusic != null)
            {
                ambientMusic.volume = ambientMusicVolume;
            }
        }

        /// <summary>
        /// 这是环境音乐是否静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetAmbientMusicMute(bool isMute)
        {
            if (ambientMusic != null)
            {
                ambientMusic.mute = isMute;
            }
        }

        #endregion

        #region 游戏音效

        /// <summary>
        /// 异步 播放游戏音效
        /// </summary>
        /// <param name="name">音效名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">回调函数</param>
        /// <param name="path">资源路径</param>
        public void PlaySoundMusicAsync(string name, bool isLoop = false, UnityAction<AudioSource> callback = null, string path = "Music/Sound/")
        {
            // 先加载音效资源
            ResourcesMgr.Instance.LoadResAsync<AudioClip>(path + name, (soundClip) =>
            {
                // 然后通过对象池管理音效播放组件
                PoolMgr.Instance.GetObjAsync("Sound", (soundObj) =>
                {
                    AudioSource audioSource = soundObj.GetComponent<AudioSource>();
                    audioSource.loop = isLoop;
                    audioSource.volume = soundMusicVolume;
                    audioSource.mute = soundMusicIsMute;
                    audioSource.PlayOneShot(soundClip);
                    soundList.Add(audioSource);
                    callback?.Invoke(audioSource);
                });
            });
        }

        /// <summary>
        /// 同步 播放游戏音效
        /// </summary>
        /// <param name="name">音效名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="path">资源路径</param>
        public AudioSource PlaySoundMusic(string name, bool isLoop = false, string path = "Music/Sound/")
        {
            // 先加载音效资源
            AudioClip soundClip = ResourcesMgr.Instance.LoadRes<AudioClip>(path + name);

            // 然后通过对象池管理音效播放组件
            GameObject soundObj = PoolMgr.Instance.GetObj("Sound");

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.loop = isLoop;
            audioSource.volume = soundMusicVolume;
            audioSource.mute = soundMusicIsMute;
            audioSource.PlayOneShot(soundClip);
            soundList.Add(audioSource);

            return audioSource;
        }

        /// <summary>
        /// 停止播放游戏音效
        /// </summary>
        /// <param name="name">音效名字</param>
        public void StopSoundMusic(AudioSource audioSource)
        {
            if (soundList.Contains(audioSource))
            {
                // 音效停止播放，并放回对象池
                audioSource.Stop();
                PoolMgr.Instance.RealeaseObj(audioSource.name, audioSource.gameObject);
                soundList.Remove(audioSource);
            }
        }

        public void ChangeSoundMusicVolume(float volume)
        {
            soundMusicVolume = volume;
            if (soundList.Count > 0)
            {
                foreach (AudioSource source in soundList)
                {
                    source.volume = volume;
                }
            }
        }

        /// <summary>
        /// 这是游戏音效是否静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetSoundMusicMute(bool isMute)
        {
            soundMusicIsMute = isMute;
            if (soundList.Count > 0)
            {
                foreach (AudioSource source in soundList)
                {
                    source.mute = isMute;
                }
            }
        }

        #endregion

        public override void Dispose()
        {
            if (IsDisposed) return;
            if (musicMgrObj != null)
            {
                GameObject.Destroy(musicMgrObj);
            }
            soundList.Clear();
            soundList = null;
            base.Dispose();
        }
    }
}