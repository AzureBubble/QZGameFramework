namespace QZGameFramework.GFEventCenter
{
    /// <summary>
    /// 事件中心 事件枚举
    /// </summary>
    public enum E_EventType
    {
        /// <summary>
        /// 更新成就事件 - string
        /// </summary>
        UpdateAchievement,

        /// <summary>
        /// 下载AB包数据对比文件 - string
        /// </summary>
        DownLoadABCompare,

        /// <summary>
        /// FTP下载文件大小 - long
        /// </summary>
        FtpDownLoadFileSize,

        /// <summary>
        /// AB包更新状态 - bool
        /// </summary>
        UpdateStatus,
    }
}