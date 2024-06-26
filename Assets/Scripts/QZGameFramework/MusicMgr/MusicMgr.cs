using Cysharp.Threading.Tasks;
using QZGameFramework.ObjectPoolManager;
using QZGameFramework.PackageMgr.ResourcesManager;
using System.Collections.Generic;
using System.IO;
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
        private bool gameMusicIsMute = false;

        // 游戏环境音
        private AudioSource ambientMusic = null;

        private float ambientMusicVolume = 1f;
        private bool ambientMusicIsMute = false;

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
            ResourcesMgr.Instance.LoadResAsync<AudioClip>(Path.Combine(path, name), (clip) =>
            {
                gameMusic.clip = clip;
                gameMusic.loop = true;
                gameMusic.volume = gameMusicVolume;
                gameMusic.mute = gameMusicIsMute;
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
            AudioClip clip = ResourcesMgr.Instance.LoadRes<AudioClip>(Path.Combine(path, name));
            gameMusic.clip = clip;
            gameMusic.loop = true;
            gameMusic.volume = gameMusicVolume;
            gameMusic.mute = gameMusicIsMute;
            gameMusic.Play();
        }

        /// <summary>
        /// 暂停播放背景音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void PlayOrPauseGameMusic(string name, bool isPause)
        {
            if (isPause)
            {
                gameMusic?.Pause();
            }
            else
            {
                gameMusic?.Play();
            }
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
            gameMusicIsMute = isMute;
            if (gameMusic != null)
            {
                gameMusic.mute = gameMusicIsMute;
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
            ResourcesMgr.Instance.LoadResAsync<AudioClip>(Path.Combine(path, name), (clip) =>
            {
                ambientMusic.clip = clip;
                ambientMusic.loop = true;
                ambientMusic.volume = ambientMusicVolume;
                ambientMusic.mute = ambientMusicIsMute;
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
            AudioClip clip = ResourcesMgr.Instance.LoadRes<AudioClip>(Path.Combine(path, name));
            ambientMusic.clip = clip;
            ambientMusic.loop = true;
            ambientMusic.volume = ambientMusicVolume;
            ambientMusic.mute = ambientMusicIsMute;
            ambientMusic.Play();
        }

        /// <summary>
        /// 暂停播放环境音乐
        /// </summary>
        /// <param name="name">音乐名字</param>
        public void PlayOrPauseAmbientMusic(string name, bool isPause)
        {
            if (isPause)
            {
                ambientMusic?.Pause();
            }
            else
            {
                ambientMusic?.Play();
            }
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
            ambientMusicIsMute = isMute;
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
        /// <param name="soundObjName">AudioSource预制体名</param>
        /// <param name="soundObjPath">AudioSource预制体路径</param>
        //public void PlaySoundMusicAsync(string name, bool isLoop = false, UnityAction<AudioSource> callback = null, string path = "Music/Sound/", string soundObjName = "Sound", string soundObjPath = "Prefabs/SFX")
        //{
        //    // 先加载音效资源
        //    ResourcesMgr.Instance.LoadResAsync<AudioClip>(Path.Combine(path, name), (soundClip) =>
        //    {
        //        // 然后通过对象池管理音效播放组件
        //        PoolMgr.Instance.GetStackObjAsync(soundObjName, (soundObj) =>
        //        {
        //            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
        //            audioSource.loop = isLoop;
        //            audioSource.volume = soundMusicVolume;
        //            audioSource.mute = soundMusicIsMute;
        //            audioSource.PlayOneShot(soundClip);
        //            soundList.Add(audioSource);
        //            callback?.Invoke(audioSource);
        //        }, soundObjPath);
        //    });
        //}

        /// <summary>
        /// 同步 播放游戏音效
        /// </summary>
        /// <param name="name">音效名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="path">资源路径</param>
        /// <param name="soundObjName">AudioSource预制体名</param>
        /// <param name="soundObjPath">AudioSource预制体路径</param>
        public AudioSource PlaySoundMusic(string name, bool isLoop = false, string path = "Music/Sound/", string soundObjName = "Sound", string soundObjPath = "Prefabs/SFX")
        {
            // 先加载音效资源
            AudioClip soundClip = ResourcesMgr.Instance.LoadRes<AudioClip>(Path.Combine(path, name));

            // 然后通过对象池管理音效播放组件
            GameObject soundObj = PoolMgr.Instance.GetStackObj(soundObjName, soundObjPath);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.loop = isLoop;
            audioSource.volume = soundMusicVolume;
            audioSource.mute = soundMusicIsMute;
            audioSource.PlayOneShot(soundClip);
            if (!soundList.Contains(audioSource))
            {
                soundList.Add(audioSource);
            }

            RemoveOrReleaseSoundListNullElement().Forget();

            return audioSource;
        }

        /// <summary>
        /// 播放随机音效
        /// </summary>
        /// <param name="path">音效路径</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="soundObjName">AudioSource预制体名</param>
        /// <param name="soundObjPath">AudioSource预制体路径</param>
        /// <returns></returns>
        public AudioSource PlayRandomSoundMusic(string path = "Music/Sound/", bool isLoop = false, string soundObjName = "Sound", string soundObjPath = "Prefabs/SFX")
        {
            // 先加载音效资源
            AudioClip[] soundClips = ResourcesMgr.Instance.LoadAllAssets<AudioClip>(path);

            if (soundClips.Length <= 0) return null;

            int randomIndex = Random.Range(0, soundClips.Length);

            // 然后通过对象池管理音效播放组件
            GameObject soundObj = PoolMgr.Instance.GetStackObj(soundObjName, soundObjPath);

            AudioSource audioSource = soundObj.GetComponent<AudioSource>();
            audioSource.loop = isLoop;
            audioSource.volume = soundMusicVolume;
            audioSource.mute = soundMusicIsMute;
            audioSource.PlayOneShot(soundClips[randomIndex]);
            if (!soundList.Contains(audioSource))
            {
                soundList.Add(audioSource);
            }

            RemoveOrReleaseSoundListNullElement().Forget();

            return audioSource;
        }

        /// <summary>
        /// 停止播放游戏音效
        /// </summary>
        /// <param name="audioSource">音效</param>
        public void StopSoundMusic(AudioSource audioSource)
        {
            if (audioSource == null) return;

            if (soundList.Contains(audioSource))
            {
                // 音效停止播放，并放回对象池
                audioSource.Stop();
                audioSource.clip = null;
                PoolMgr.Instance.ReleaseStackObj(audioSource.gameObject);
                soundList.Remove(audioSource);
            }
        }

        /// <summary>
        /// 等待音效播放结束 返回对象池
        /// </summary>
        /// <param name="audioSource">音效</param>
        public async UniTaskVoid StopSoundMusicWaitUntilClipOver(AudioSource audioSource)
        {
            if (audioSource == null) return;
            await UniTask.WaitUntil(() => !audioSource.isPlaying);
            StopSoundMusic(audioSource);
        }

        public void ChangeSoundMusicVolume(float volume)
        {
            soundMusicVolume = volume;
            bool isDel = false;
            if (soundList.Count > 0)
            {
                for (int i = soundList.Count - 1; i >= 0; --i)
                {
                    if (soundList[i] == null)
                    {
                        isDel = true;
                        continue;
                    }

                    soundList[i].volume = volume;
                }
            }
            if (isDel)
            {
                RemoveOrReleaseSoundListNullElement().Forget();
            }
        }

        /// <summary>
        /// 这是游戏音效是否静音
        /// </summary>
        /// <param name="isMute"></param>
        public void SetSoundMusicMute(bool isMute)
        {
            soundMusicIsMute = isMute;
            bool isDel = false;
            if (soundList.Count > 0)
            {
                for (int i = soundList.Count - 1; i >= 0; --i)
                {
                    if (soundList[i] == null)
                    {
                        isDel = true;
                        continue;
                    }

                    soundList[i].mute = isMute;
                }
            }

            if (isDel)
            {
                RemoveOrReleaseSoundListNullElement().Forget();
            }
        }

        private async UniTaskVoid RemoveOrReleaseSoundListNullElement()
        {
            await UniTask.Yield();
            for (int i = soundList.Count - 1; i >= 0; --i)
            {
                if (soundList[i] == null)
                {
                    soundList.RemoveAt(i);
                    continue;
                }

                if (!soundList[i].isPlaying)
                {
                    StopSoundMusic(soundList[i]);
                }
            }
        }

        /// <summary>
        /// 继续播放或者暂停所有音效
        /// </summary>
        /// <param name="isPlay">是否是继续播放 true为播放 false为暂停</param>
        public void PlayOrPauseSound(bool isPlay)
        {
            if (isPlay)
            {
                for (int i = 0; i < soundList.Count; i++)
                    soundList[i]?.Play();
            }
            else
            {
                for (int i = 0; i < soundList.Count; i++)
                    soundList[i]?.Pause();
            }
        }

        #endregion

        /// <summary>
        /// 清空所有音效
        /// </summary>
        public void ClearSoundList()
        {
            foreach (AudioSource sound in soundList)
            {
                if (sound == null) continue;
                sound.Stop();
                sound.clip = null;
                PoolMgr.Instance.ReleaseStackObj(sound.gameObject);
            }
            soundList.Clear();
            soundList = null;
        }

        public override void Dispose()
        {
            if (IsDisposed) return;
            if (musicMgrObj != null)
            {
                GameObject.Destroy(musicMgrObj);
            }
            ClearSoundList();

            base.Dispose();
        }
    }
}