using System.Collections.Generic;
using UnityEngine.Events;

namespace QZGameFramework.GFEventCenter
{
    /// <summary>
    /// 单个事件接口
    /// </summary>
    public interface IEventInfo
    {
    }

    /// <summary>
    /// 不带参数的事件
    /// </summary>
    public class EventInfo : IEventInfo
    {
        public event UnityAction actions;

        public EventInfo(UnityAction action)
        {
            actions += action;
        }

        /// <summary>
        /// 事件触发
        /// </summary>
        public void EventTrigger()
        {
            actions?.Invoke();
        }
    }

    /// <summary>
    /// 带一个参数的事件
    /// </summary>
    public class EventInfo<T> : IEventInfo
    {
        public event UnityAction<T> actions;

        public EventInfo(UnityAction<T> action)
        {
            actions += action;
        }

        public void EventTrigger(T t)
        {
            actions?.Invoke(t);
        }
    }

    /// <summary>
    /// 带两个参数的事件
    /// </summary>
    public class EventInfo<T1, T2> : IEventInfo
    {
        public event UnityAction<T1, T2> actions;

        public EventInfo(UnityAction<T1, T2> action)
        {
            actions += action;
        }

        public void EventTrigger(T1 t1, T2 t2)
        {
            actions?.Invoke(t1, t2);
        }
    }

    /// <summary>
    /// 带三个参数的事件
    /// </summary>
    public class EventInfo<T1, T2, T3> : IEventInfo
    {
        public event UnityAction<T1, T2, T3> actions;

        public EventInfo(UnityAction<T1, T2, T3> action)
        {
            actions += action;
        }

        public void EventTrigger(T1 t1, T2 t2, T3 t3)
        {
            actions?.Invoke(t1, t2, t3);
        }
    }

    /// <summary>
    /// 带四个参数的事件
    /// </summary>
    public class EventInfo<T1, T2, T3, T4> : IEventInfo
    {
        public event UnityAction<T1, T2, T3, T4> actions;

        public EventInfo(UnityAction<T1, T2, T3, T4> action)
        {
            actions += action;
        }

        public void EventTrigger(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            actions?.Invoke(t1, t2, t3, t4);
        }
    }

    /// <summary>
    /// 事件中心 负责分发管理事件
    /// </summary>
    public class EventCenter : Singleton<EventCenter>
    {
        /// <summary>
        /// 事件容器
        /// key —— 事件的枚举
        /// value —— 对应的是 监听这个事件 对应的委托函数
        /// </summary>
        private Dictionary<E_EventType, IEventInfo> eventDic = new Dictionary<E_EventType, IEventInfo>();

        #region 不带参数的事件监听

        /// <summary>
        /// 添加事件监听
        /// </summary>
        /// <param name="eventType">事件名字</param>
        /// <param name="action">事件</param>
        public void AddEventListener(E_EventType eventType, UnityAction action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo).actions += action;
            }
            else // 否则
            {
                eventDic.Add(eventType, new EventInfo(action));
            }
        }

        /// <summary>
        /// 移除事件监听
        /// </summary>
        /// <param name="eventType">事件名字</param>
        /// <param name="action">事件</param>
        public void RemoveEventListener(E_EventType eventType, UnityAction action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo).actions -= action;
            }
        }

        /// <summary>
        /// 不带参数事件触发
        /// </summary>
        /// <param name="eventType">事件名字</param>
        public void EventTrigger(E_EventType eventType)
        {
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventDic[eventType] as EventInfo).EventTrigger();
            }
        }

        #endregion

        #region 一个参数的事件监听

        public void AddEventListener<T>(E_EventType eventType, UnityAction<T> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T>).actions += action;
            }
            else // 否则
            {
                eventDic.Add(eventType, new EventInfo<T>(action));
            }
        }

        public void RemoveEventListener<T>(E_EventType eventType, UnityAction<T> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T>).actions -= action;
            }
        }

        public void EventTrigger<T>(E_EventType eventType, T parameter)
        {
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventDic[eventType] as EventInfo<T>).EventTrigger(parameter);
            }
        }

        #endregion

        #region 两个参数的事件监听

        public void AddEventListener<T1, T2>(E_EventType eventType, UnityAction<T1, T2> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2>).actions += action;
            }
            else // 否则
            {
                eventDic.Add(eventType, new EventInfo<T1, T2>(action));
            }
        }

        public void RemoveEventListener<T1, T2>(E_EventType eventType, UnityAction<T1, T2> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2>).actions -= action;
            }
        }

        public void EventTrigger<T1, T2>(E_EventType eventType, T1 parameter1, T2 parameter2)
        {
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventDic[eventType] as EventInfo<T1, T2>).EventTrigger(parameter1, parameter2);
            }
        }

        #endregion

        #region 三个参数的事件监听

        public void AddEventListener<T1, T2, T3>(E_EventType eventType, UnityAction<T1, T2, T3> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2, T3>).actions += action;
            }
            else // 否则
            {
                eventDic.Add(eventType, new EventInfo<T1, T2, T3>(action));
            }
        }

        public void RemoveEventListener<T1, T2, T3>(E_EventType eventType, UnityAction<T1, T2, T3> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2, T3>).actions -= action;
            }
        }

        public void EventTrigger<T1, T2, T3>(E_EventType eventType, T1 parameter1, T2 parameter2, T3 parameter3)
        {
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventDic[eventType] as EventInfo<T1, T2, T3>).EventTrigger(parameter1, parameter2, parameter3);
            }
        }

        #endregion

        #region 四个参数的事件监听

        public void AddEventListener<T1, T2, T3, T4>(E_EventType eventType, UnityAction<T1, T2, T3, T4> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2, T3, T4>).actions += action;
            }
            else // 否则
            {
                eventDic.Add(eventType, new EventInfo<T1, T2, T3, T4>(action));
            }
        }

        public void RemoveEventListener<T1, T2, T3, T4>(E_EventType eventType, UnityAction<T1, T2, T3, T4> action)
        {
            // 如果字典中存在该事件
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventInfo as EventInfo<T1, T2, T3, T4>).actions -= action;
            }
        }

        public void EventTrigger<T1, T2, T3, T4>(E_EventType eventType, T1 parameter1, T2 parameter2, T3 parameter3, T4 parameter4)
        {
            if (eventDic.TryGetValue(eventType, out IEventInfo eventInfo))
            {
                (eventDic[eventType] as EventInfo<T1, T2, T3, T4>).EventTrigger(parameter1, parameter2, parameter3, parameter4);
            }
        }

        #endregion

        public override void Dispose()
        {
            if (IsDisposed) return;
            eventDic.Clear();
            base.Dispose();
        }
    }
}