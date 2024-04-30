using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace QZGameFramework.PackageMgr.UnityWebRequestMgr
{
    public class WebResMgr : Singleton<WebResMgr>
    {
        public void LoadResAsync<T>(string path, UnityAction<T> successCallback = null, UnityAction failedCallback = null) where T : class
        {
            if (!path.Contains("file://"))
            {
                path = "file://" + path;
            }
            LoadResTask<T>(path, successCallback, failedCallback).Forget();
        }

        private async UniTaskVoid LoadResTask<T>(string path, UnityAction<T> successCallback, UnityAction failedCallback) where T : class
        {
            Type type = typeof(T);
            UnityWebRequest req = null;
            if (type == typeof(string) || type == typeof(byte[]))
            {
                req = UnityWebRequest.Get(path);
            }
            else if (type == typeof(Texture))
            {
                req = UnityWebRequestTexture.GetTexture(path);
            }
            else if (type == typeof(AssetBundle))
            {
                req = UnityWebRequestAssetBundle.GetAssetBundle(path);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("UnityWebRequest loading types are not supported. Type: " + type);
#endif
                failedCallback?.Invoke();
                return;
            }

            await req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.Success)
            {
                if (type == typeof(string))
                {
                    successCallback?.Invoke(req.downloadHandler.text as T);
                }
                else if (type == typeof(byte[]))
                {
                    successCallback?.Invoke(req.downloadHandler.data as T);
                }
                else if (type == typeof(Texture))
                {
                    successCallback?.Invoke(DownloadHandlerTexture.GetContent(req) as T);
                }
                else if (type == typeof(AssetBundle))
                {
                    successCallback?.Invoke(DownloadHandlerAssetBundle.GetContent(req) as T);
                }
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogError("UnityWebRequest resource loading failure. Type: " + type);
#endif
                failedCallback?.Invoke();
            }

            req.Dispose();
        }

        public void LoadResThread<T>(string path, UnityAction<T> successCallback = null, UnityAction failedCallback = null) where T : class
        {
            if (!path.Contains("file://"))
            {
                path = "file://" + path;
            }
            LoadResUniTaskThread<T>(path, successCallback, failedCallback).Forget();
        }

        private async UniTaskVoid LoadResUniTaskThread<T>(string path, UnityAction<T> successCallback, UnityAction failedCallback) where T : class
        {
            await UniTask.RunOnThreadPool(async () =>
            {
                Type type = typeof(T);
                UnityWebRequest req = null;

                await UniTask.Yield();
                if (type == typeof(string) || type == typeof(byte[]))
                {
                    req = UnityWebRequest.Get(path);
                }
                else if (type == typeof(Texture))
                {
                    req = UnityWebRequestTexture.GetTexture(path);
                }
                else if (type == typeof(AssetBundle))
                {
                    req = UnityWebRequestAssetBundle.GetAssetBundle(path);
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("UnityWebRequest loading types are not supported. Type: " + type);
#endif
                    failedCallback?.Invoke();
                    return;
                }
                await req.SendWebRequest();
                if (req.result == UnityWebRequest.Result.Success)
                {
                    if (type == typeof(string))
                    {
                        successCallback?.Invoke(req.downloadHandler.text as T);
                    }
                    else if (type == typeof(byte[]))
                    {
                        successCallback?.Invoke(req.downloadHandler.data as T);
                    }
                    else if (type == typeof(Texture))
                    {
                        successCallback?.Invoke(DownloadHandlerTexture.GetContent(req) as T);
                    }
                    else if (type == typeof(AssetBundle))
                    {
                        successCallback?.Invoke(DownloadHandlerAssetBundle.GetContent(req) as T);
                    }
                }
                else
                {
#if UNITY_EDITOR
                    Debug.LogError("UnityWebRequest resource loading failure. Type: " + type);
#endif
                    failedCallback?.Invoke();
                }

                req.Dispose();
            });
        }
    }
}