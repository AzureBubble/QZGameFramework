using QZGameFramework.GFEventCenter;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.NetManager
{
    public class FtpManager : Singleton<FtpManager>
    {
        private event UnityAction<float> downLoadAction;

        private event UnityAction<float> upLoadFileAction;

        #region 添加上传下载事件

        /// <summary>
        /// 添加下载文件事件
        /// </summary>
        /// <param name="action"></param>
        public void AddDownloadEvent(UnityAction<float> action)
        {
            if (action != null)
            {
                downLoadAction += action;
            }
        }

        /// <summary>
        /// 删除下载文件事件
        /// </summary>
        /// <param name="action"></param>
        public void RemoveDownloadEvent(UnityAction<float> action)
        {
            if (action != null)
            {
                downLoadAction -= action;
            }
        }

        /// <summary>
        /// 添加上传文件事件
        /// </summary>
        /// <param name="action"></param>
        public void AddUploadFileEvent(UnityAction<float> action)
        {
            if (action != null)
            {
                upLoadFileAction += action;
            }
        }

        /// <summary>
        /// 删除上传文件事件
        /// </summary>
        /// <param name="action"></param>
        public void RemoveUploadFileEvent(UnityAction<float> action)
        {
            if (action != null)
            {
                upLoadFileAction -= action;
            }
        }

        #endregion

        /// <summary>
        /// 异步上传文件到Ftp服务器
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="serverIP">服务器地址</param>
        public async void UpLoadFileAsync(string fileName, string filePath, string serverIP, string userName, string password)
        {
            await Task.Run(() =>
            {
                try
                {
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + fileName)) as FtpWebRequest;
                    req.Credentials = new NetworkCredential(userName, password);
                    req.Proxy = null;
                    req.KeepAlive = false;
                    req.Method = WebRequestMethods.Ftp.UploadFile;
                    Stream stream = req.GetRequestStream();
                    using (FileStream fs = File.OpenRead(filePath))
                    {
                        byte[] bytes = new byte[2048];
                        int contentLength = fs.Read(bytes, 0, bytes.Length);
                        while (contentLength > 0)
                        {
                            stream.Write(bytes, 0, contentLength);
                            upLoadFileAction?.Invoke(contentLength);
                            contentLength = fs.Read(bytes, 0, bytes.Length);
                        }
                        fs.Close();
                        stream.Close();
                    }
                    Debug.Log($"{fileName}上传成功");
                }
                catch (Exception e)
                {
                    Debug.Log($"{fileName}上传失败:" + e.Message);
                }
            });
        }

        /// <summary>
        /// 异步下载文件 从Ftp服务器
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="serverIP">服务器地址</param>
        public async Task DownLoadFileAsync(string fileName, string localPath, string serverIP, string userName, string password, UnityAction<bool> callback)
        {
            bool isDonwLoadOver = false;
            int reDownLoadMaxNum = 5; // 重连服务器最大次数
            int connectCount = 0;
            while (!isDonwLoadOver && connectCount < reDownLoadMaxNum)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + fileName)) as FtpWebRequest;
                        req.Credentials = new NetworkCredential(userName, password);
                        req.Proxy = null;
                        req.Timeout = 2000;
                        // 请求完毕 关闭控制连接
                        req.KeepAlive = false;
                        req.Method = WebRequestMethods.Ftp.DownloadFile;
                        FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                        Stream stream = res.GetResponseStream();
                        using (FileStream fs = File.Create(localPath))
                        {
                            byte[] bytes = new byte[2048];
                            int contentLength = stream.Read(bytes, 0, bytes.Length);
                            while (contentLength > 0)
                            {
                                fs.Write(bytes, 0, contentLength);
                                // 把下载的字节数组长度传出去，以便制作加载进度条
                                if (fileName != "ABCompareInfo.txt")
                                    EventCenter.Instance.EventTrigger<long>(E_EventType.FtpDownLoadFileSize, contentLength);
                                downLoadAction?.Invoke(contentLength);

                                contentLength = stream.Read(bytes, 0, bytes.Length);
                            }
                            fs.Close();
                            stream.Close();
                            res.Close();
                        }
                        Debug.Log($"{fileName}下载成功");
                        // 下载完成的回调函数
                        isDonwLoadOver = true;
                    }
                    catch (Exception e)
                    {
                        isDonwLoadOver = false;
                        ++connectCount;
                        Debug.Log($"{fileName}下载失败:" + e.Message);
                        Debug.Log($"重新连接服务器{connectCount}次");
                    }
                });

                callback?.Invoke(isDonwLoadOver);
            }
        }

        /// <summary>
        /// 查找服务器是否存在文件夹
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="dirName"></param>
        /// <param name="callback"></param>
        public async void CheckFtpDirectorExists(string serverIP, string dirName, string userName, string password, UnityAction<bool> callback)
        {
            try
            {
                await Task.Run(() =>
                {
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP)) as FtpWebRequest;
                    req.Credentials = new NetworkCredential(userName, password);
                    req.Proxy = null;
                    req.KeepAlive = false;
                    // 检查目录方法
                    req.Method = WebRequestMethods.Ftp.ListDirectory;
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    res.Close();
                    callback?.Invoke(true);
                });
            }
            catch (WebException e)
            {
                FtpWebResponse response = (FtpWebResponse)e.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    Debug.Log($"文件夹不存在：" + e.Message);
                    callback?.Invoke(false);
                }
            }
        }

        /// <summary>
        /// 远程创建文件夹
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="dirName"></param>
        /// <param name="callback"></param>
        public async void CreateFtpDirectorAsync(string serverIP, string dirName, string userName, string password, UnityAction callback)
        {
            try
            {
                await Task.Run(() =>
                {
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP)) as FtpWebRequest;
                    req.Credentials = new NetworkCredential(userName, password);
                    req.Proxy = null;
                    req.KeepAlive = false;
                    req.Method = WebRequestMethods.Ftp.MakeDirectory;
                    req.UseBinary = true;
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    res.Close();
                });
            }
            catch (Exception e)
            {
                Debug.Log($"创建文件夹出错：" + e.Message);
            }
            callback?.Invoke();
        }

        /// <summary>
        /// 异步删除文件
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="fileName"></param>
        /// <param name="callback"></param>
        public async void DeleteFileAsync(string serverIP, string fileName, string userName, string password, UnityAction callback)
        {
            await Task.Run(() =>
            {
                try
                {
                    // 通过线程执行这里面的逻辑 那么就不会影响主线程
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + fileName)) as FtpWebRequest;
                    // 代理置空
                    req.Proxy = null;
                    // 通信凭证
                    NetworkCredential n = new NetworkCredential(userName, password);
                    req.Credentials = n;
                    // 是否关闭控制连接
                    req.KeepAlive = false;
                    // 操作类型
                    req.Method = WebRequestMethods.Ftp.DeleteFile;
                    // 传输类型
                    req.UseBinary = true;

                    // 真正删除
                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    res.Close();
                    callback?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.Log("移除文件出错：" + e.Message);
                }
            });
        }

        /// <summary>
        /// 获取远程文件大小
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="fileName"></param>
        /// <param name="callback"></param>
        public async void GetFileSize(string serverIP, string fileName, string userName, string password, UnityAction<long> callback)
        {
            await Task.Run(() =>
            {
                try
                {
                    // 通过线程执行这里面的逻辑 那么就不会影响主线程
                    FtpWebRequest req = FtpWebRequest.Create(new Uri(serverIP + fileName)) as FtpWebRequest;
                    // 代理置空
                    req.Proxy = null;
                    // 通信凭证
                    NetworkCredential n = new NetworkCredential(userName, password);
                    req.Credentials = n;
                    // 是否关闭控制连接
                    req.KeepAlive = false;
                    // 操作类型
                    req.Method = WebRequestMethods.Ftp.GetFileSize;
                    // 传输类型
                    req.UseBinary = true;

                    FtpWebResponse res = req.GetResponse() as FtpWebResponse;
                    callback?.Invoke(res.ContentLength);
                    res.Close();
                }
                catch (Exception e)
                {
                    callback?.Invoke(0);
                    Debug.Log("获取文件大小出错：" + e.Message);
                }
            });
        }
    }
}