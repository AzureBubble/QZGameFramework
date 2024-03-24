using System;
using System.Collections.Generic;
using UnityEngine;

namespace QZGameFramework.StateMachine
{
    public class BaseFsm
    {
        /// <summary>
		/// 状态机持有者
		/// </summary>
        public System.Object Entity { private set; get; }

        /// <summary>
        /// 状态机字典
        /// </summary>
        private Dictionary<string, IState> stateDict = new Dictionary<string, IState>(50);

        /// <summary>
        /// 黑板 共享数据信息
        /// </summary>
        private Dictionary<string, System.Object> blackboardDict = new Dictionary<string, System.Object>(50);

        /// <summary>
        /// 当前状态
        /// </summary>
        private IState currentState;

        public string CurrentState => currentState == null ? string.Empty : currentState.GetType().FullName;

        /// <summary>
        /// 上一次状态
        /// </summary>
        private IState preState;

        public string PreState => preState == null ? string.Empty : preState.GetType().FullName;

        //private BaseFsm()
        //{ }

        public BaseFsm(System.Object entity)
        {
            Entity = entity;
        }

        /// <summary>
        /// 状态机帧更新
        /// </summary>
        public void Update()
        {
            // 当前状态更新
            currentState?.OnUpdate();
        }

        #region 启动状态 切换状态

        /// <summary>
        /// 启动状态
        /// </summary>
        public void StateOn<K>() where K : IState
        {
            StateOn(typeof(K).FullName);
        }

        /// <summary>
        /// 启动状态
        /// </summary>
        public void StateOn(Type stateType)
        {
            StateOn(stateType.FullName);
        }

        /// <summary>
        /// 启动状态
        /// </summary>
        public void StateOn(string stateName)
        {
            preState = currentState;
            currentState?.OnExit();

            if (stateDict.TryGetValue(stateName, out currentState))
            {
                currentState?.OnEnter();
            }
            else
            {
                Debug.Log($"No Found State : {stateName}");
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void SwitchState<K>() where K : IState
        {
            StateOn<K>();
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        public void SwitchState(Type stateType)
        {
            StateOn(stateType);
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="stateName">新状态名</param>
        public void SwitchState(string stateName)
        {
            StateOn(stateName);
        }

        #endregion

        #region 添加状态

        /// <summary>
        /// 添加状态结点
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">状态</param>
        public void AddState<K>() where K : IState
        {
            var state = Activator.CreateInstance<K>() as IState;
            AddState(state);
        }

        /// <summary>
        /// 添加状态结点
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <param name="state">状态</param>
        public void AddState(IState state)
        {
            var type = state.GetType();

            if (!stateDict.ContainsKey(type.FullName))
            {
                stateDict.Add(type.FullName, state);
                state.OnCreate(this);
            }
            else
            {
                Debug.LogError($"State has already existed : {type.FullName}");
            }
        }

        #endregion

        #region 设置黑板

        /// <summary>
        /// 设置黑板数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBlackboardValue(string key, System.Object value)
        {
            if (blackboardDict.ContainsKey(key))
            {
                blackboardDict[key] = value;
            }
            else
            {
                blackboardDict.Add(key, value);
            }
        }

        /// <summary>
        /// 获取黑板数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public System.Object GetBlackboardValue(string key)
        {
            if (blackboardDict.ContainsKey(key))
            {
                return blackboardDict[key];
            }
            else
            {
                Debug.Log($"Not found blackboard value : {key}");
                return null;
            }
        }

        #endregion
    }
}