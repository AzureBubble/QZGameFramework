//using GameFramework.PackageMgr.ResourcesManager;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Audio;

//public class AudioMixerMgr : Singleton<AudioMixerMgr>
//{
//    private GameObject audioMixerMgr;
//    private AudioMixer gameAudioMixer;

//    private AudioMixerSnapshot normal;
//    private AudioMixerSnapshot mute;
//    private AudioMixerSnapshot ambientOnly;

//    // 游戏背景音乐
//    private AudioSource gameMusic = null;

//    private float gameMusicVolume = 1f;
//    //private bool gameMusicIsMute = false;

//    // 游戏环境音
//    private AudioSource ambientMusic = null;

//    private float ambientMusicVolume = 1f;
//    //private bool ambientMusicIsMute = false;

//    // 游戏音效
//    private List<AudioSource> soundList;

//    private float soundMusicVolume = 1f;
//    private bool soundMusicIsMute = false;

//    public override void Initialize()
//    {
//        GameObject obj = new GameObject("AudioMixer Manager");
//        GameObject.DontDestroyOnLoad(obj);
//        gameAudioMixer = ResourcesMgr.Instance.LoadRes<AudioMixer>("AduioMixer/GameAudioMixer");
//        normal = gameAudioMixer.FindSnapshot("Normal");
//        mute = gameAudioMixer.FindSnapshot("Mute");
//        ambientOnly = gameAudioMixer.FindSnapshot("AmbientOnly");
//        AudioMixerGroup[] groups = gameAudioMixer.FindMatchingGroups("");
//        groups[0].
//        //audioMixerMgr = obj;
//        soundList = new List<AudioSource>(10);
//    }

//    #region 游戏背景音乐

//    /// <summary>
//    /// 异步 播放背景音乐
//    /// </summary>
//    /// <param name="name">音乐名字</param>
//    /// <param name="path">资源路径</param>
//    public void PlayGameMusicAsync(string name, string path = "AduioMixer/Music/BK/")
//    {
//        if (gameMusic == null)
//        {
//            GameObject gameMusicObj = new GameObject("Game Music");
//            gameMusic = gameMusicObj.AddComponent<AudioSource>();
//            gameMusic.outputAudioMixerGroup = gameAudioMixer.FindMatchingGroups("");
//            gameMusicObj.transform.SetParent(audioMixerMgr.transform, false);
//        }
//        ResourcesMgr.Instance.LoadResAsync<AudioClip>(path + name, (clip) =>
//        {
//            gameMusic.clip = clip;
//            gameMusic.loop = true;
//            gameMusic.volume = gameMusicVolume;
//            gameMusic.Play();
//        });
//    }

//    /// <summary>
//    /// 同步 播放背景音乐
//    /// </summary>
//    /// <param name="name">音乐名字</param>
//    /// <param name="path">资源路径</param>
//    public void PlayGameMusic(string name, string path = "Music/BK/")
//    {
//        if (gameMusic == null)
//        {
//            GameObject gameMusicObj = new GameObject("Game Music");
//            gameMusic = gameMusicObj.AddComponent<AudioSource>();
//            gameMusicObj.transform.SetParent(musicMgrObj.transform, false);
//        }
//        AudioClip clip = ResourcesMgr.Instance.LoadRes<AudioClip>(path + name);
//        gameMusic.clip = clip;
//        gameMusic.loop = true;
//        gameMusic.volume = gameMusicVolume;
//        gameMusic.Play();
//    }

//    /// <summary>
//    /// 暂停播放背景音乐
//    /// </summary>
//    /// <param name="name">音乐名字</param>
//    public void PauseGameMusic(string name)
//    {
//        gameMusic?.Pause();
//    }

//    /// <summary>
//    /// 停止播放背景音乐
//    /// </summary>
//    /// <param name="name">音乐名字</param>
//    public void StopGameMusic(string name)
//    {
//        gameMusic?.Stop();
//    }

//    public void ChangeGameMusicVolume(float volume)
//    {
//        gameMusicVolume = volume;
//        if (gameMusic != null)
//        {
//            gameMusic.volume = gameMusicVolume;
//        }
//    }

//    /// <summary>
//    /// 这是背景音乐是否静音
//    /// </summary>
//    /// <param name="isMute"></param>
//    public void SetGameMusicMute(bool isMute)
//    {
//        if (gameMusic != null)
//        {
//            gameMusic.mute = isMute;
//        }
//    }

//    #endregion

//    /// <summary>
//    /// 获取音量转换为AudioMixer的值
//    /// </summary>
//    /// <param name="volume"></param>
//    /// <returns></returns>
//    private float ConvertVolume(float volume)
//    {
//        return volume * 100 - 80;
//    }

//    public override void Dispose()
//    {
//        if (IsDisposed) return;
//        base.Dispose();
//    }
//}