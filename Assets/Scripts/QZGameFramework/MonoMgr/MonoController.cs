using UnityEngine;
using UnityEngine.Events;

namespace QZGameFramework.MonoManager
{
    public class MonoController : MonoBehaviour
    {
        private bool openControlUpdateTime; // 是否开启帧更新模式
        public static long LogicFrameid; // 逻辑帧id 自增
        public static float LogicFrameInterval = 0.066f; // 一秒15帧
        public static int LogicFrameIntervalms = 66;
        private float _acclogicRuntime; // 逻辑帧累计运行时间
        private float _nextLogicTime; // 下一个逻辑帧时间
        public static float DeltaTime; // 动画缓动时间

        // 生命周期 Update 函数监听
        private event UnityAction updateEvent;

        // 生命周期 FixedUpdate 函数监听
        private event UnityAction fixedUpdateEvent;

        // 生命周期 LateUpdate 函数监听
        private event UnityAction lateUpdateEvent;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke();
        }

        private void Update()
        {
            if (openControlUpdateTime)
            {
                // 逻辑帧运行时间累加
                _acclogicRuntime += Time.deltaTime;
                // 当前逻辑帧累计时间 大于下一个逻辑帧开始时间 就需要更新逻辑帧
                // 控制帧数 保证所有设备的逻辑帧帧数的一致性 并进行追帧操作
                while (_acclogicRuntime > _nextLogicTime)
                {
                    // 更新逻辑帧
                    updateEvent?.Invoke();

                    // 计算下一个逻辑帧的时间
                    _nextLogicTime += LogicFrameInterval;
                    LogicFrameid++;
                }
                // _acclogicRuntime = 0.01 LogicFrameInterval = 0.066 _nextLogicTime = 0.066 / LogicFrameInterval 0.066
                // (0.01 + 0.066 - 0.066) / 0.066 = 0.01 / 0.066 = 当前值 / 最大值 得到结果 0~1 比率 与血条的计算是一样的
                DeltaTime = (_acclogicRuntime + LogicFrameInterval - _nextLogicTime) / LogicFrameInterval;
            }
            else
            {
                // 更新逻辑帧
                updateEvent?.Invoke();
            }
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }

        public void AddUpdateListener(UnityAction action)
        {
            updateEvent += action;
        }

        public void RemoveUpdateListener(UnityAction action)
        {
            updateEvent -= action;
        }

        public void AddFixedUpdateListener(UnityAction action)
        {
            fixedUpdateEvent += action;
        }

        public void RemoveFixedUpdateListener(UnityAction action)
        {
            fixedUpdateEvent -= action;
        }

        public void AddLateUpdateListener(UnityAction action)
        {
            lateUpdateEvent += action;
        }

        public void RemoveLateUpdateListener(UnityAction action)
        {
            lateUpdateEvent -= action;
        }
    }
}