using System;

namespace QZGameFramework.Utilities
{
    /// <summary>
    /// 计时器类
    /// </summary>
    public sealed class Timer
    {
        private readonly float intervalTime; // 间隔时间
        private readonly float durationTime; // 持续时间
        private readonly long maxTriggerCount; // 最大触发次数

        private float delayTimer = 0; // 延迟时间计时器
        private float durationTimer = 0; // 持续时间计时器
        private float intervalTimer = 0; // 间隔时间计时器
        private long triggerCount = 0;  // 触发次数计数器

        /// <summary>
        /// 延迟时间
        /// </summary>
        public float DelayTime { get; private set; }

        /// <summary>
        /// 是否已结束
        /// </summary>
        public bool IsOver { get; private set; }

        /// <summary>
        /// 是否已暂停
        /// </summary>
        public bool IsPause { get; private set; }

        /// <summary>
        /// 延迟剩余时间
        /// </summary>
        public float RemainingTime
        {
            get
            {
                if (IsOver) return 0f;
                else return Math.Max(0f, DelayTime - delayTimer);
            }
        }

        /// <summary>
        /// 计时器构造函数
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <param name="durationTime">持续时间</param>
        /// <param name="maxTriggerCount">最大触发次数</param>
        public Timer(float delayTime, float intervalTime, float durationTime, long maxTriggerCount)
        {
            this.DelayTime = delayTime;
            this.intervalTime = intervalTime;
            this.durationTime = durationTime;
            this.maxTriggerCount = maxTriggerCount;
        }

        /// <summary>
        /// 暂停计时器
        /// </summary>
        public void Pause()
        {
            IsPause = true;
        }

        /// <summary>
        /// 恢复计时器
        /// </summary>
        public void Resume()
        {
            IsPause = false;
        }

        /// <summary>
        /// 结束计时器
        /// </summary>
        public void Dispose()
        {
            IsOver = true;
        }

        /// <summary>
        /// 重置计时器
        /// </summary>
        public void Reset()
        {
            delayTimer = 0;
            durationTimer = 0;
            intervalTimer = 0;
            triggerCount = 0;
            IsOver = false;
            IsPause = false;
        }

        /// <summary>
        /// 更新计时器
        /// </summary>
        /// <param name="time">外部传入更新时间间隔</param>
        /// <returns></returns>
        public bool Update(float time)
        {
            // 计时器已经结束 或者 处于暂停中
            if (IsOver || IsPause)
            {
                return false;
            }

            // 增加 延时计时器
            delayTimer += time;
            // 判断延时计时器是否大于设定的延时时间
            if (delayTimer < DelayTime)
            {
                // 小于 则返回false
                return false;
            }

            // 如果间隔时间 大于 0
            if (intervalTime > 0)
            {
                intervalTimer += time;
            }

            // 如果持续时间 大于 0
            if (durationTime > 0)
            {
                durationTimer += time;
            }

            // 不延迟 直接触发
            if (DelayTime == -1)
            {
                DelayTime = 0;
                if (maxTriggerCount > 0)
                    triggerCount++;
                return true;
            }

            // 检测间隔时间执行
            if (intervalTime > 0)
            {
                if (intervalTimer < intervalTime)
                {
                    return false;
                }
                intervalTimer = 0;
            }

            if (durationTime > 0)
            {
                if (durationTimer >= durationTime)
                {
                    Dispose();
                }
            }

            if (maxTriggerCount > 0)
            {
                triggerCount++;
                if (triggerCount >= maxTriggerCount)
                {
                    Dispose();
                }
            }

            return true;
        }

        #region 静态外部调用创建计时器的方法

        /// <summary>
        /// 延迟时间后 触发一次
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <returns></returns>
        public static Timer CreateOnceTimer(float delayTime)
        {
            return new Timer(delayTime, -1, -1, 1);
        }

        /// <summary>
        /// 延迟后 永久间隔时间触发一次
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间</param>
        /// <param name="intervalTime">间隔时间</param>
        /// <returns></returns>
        public static Timer CreateRepeatTimer(float intervalTime, float delayTime = -1)
        {
            return new Timer(delayTime, intervalTime, -1, -1);
        }

        /// <summary>
        /// 延迟后 持续触发一定次数
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间,-1不延迟直接触发</param>
        /// /// <param name="maxTriggerCount">触发次数</param>
        /// <returns></returns>
        public static Timer CreateRepeatTimer(float delayTime, long maxTriggerCount)
        {
            return new Timer(delayTime, -1, -1, maxTriggerCount);
        }

        /// <summary>
        /// 延迟后 只在规定时间内的间隔时间触发
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间,-1不延迟直接触发</param>
        /// <param name="intervalTime">间隔时间</param>
        /// /// <param name="intervalTime">规定时间</param>
        /// <returns></returns>
        public static Timer CreateRepeatTimer(float delayTime, float intervalTime, float durationTime)
        {
            return new Timer(delayTime, intervalTime, durationTime, -1);
        }

        /// <summary>
        /// 延迟后 只在间隔触发一定次数
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间,-1不延迟直接触发</param>
        /// <param name="intervalTime">间隔时间</param>
        /// /// <param name="maxTriggerCount">触发次数</param>
        /// <returns></returns>
        public static Timer CreateRepeatTimer(float delayTime, float intervalTime, long maxTriggerCount)
        {
            return new Timer(delayTime, intervalTime, -1, maxTriggerCount);
        }

        /// <summary>
        /// 延迟后 在一段时间内持续触发
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间,-1不延迟直接触发</param>
        /// <param name="durationTime">持续时间</param>
        /// <returns></returns>
        public static Timer CreateDurationTimer(float delayTime, float durationTime)
        {
            return new Timer(delayTime, -1, durationTime, -1);
        }

        /// <summary>
        /// 延迟后 永久触发
        /// delayTime 默认-1 如果不修改 则直接触发
        /// </summary>
        /// <param name="delayTime">延迟时间,-1不延迟直接触发</param>
        /// <returns></returns>
        public static Timer CreateForeverTimer(float delayTime)
        {
            return new Timer(delayTime, -1, -1, -1);
        }

        #endregion
    }
}