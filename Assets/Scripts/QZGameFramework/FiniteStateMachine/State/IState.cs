namespace QZGameFramework.StateMachine
{
    /// <summary>
    /// 状态接口
    /// 如果不需要成员属性 成员变量 可使用接口
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// 状态创建的时候执行的方法
        /// </summary>
        /// <param name="fsm">状态机</param>
        void OnCreate(BaseFsm fsm);

        /// <summary>
        /// 状态进入
        /// </summary>
        void OnEnter();

        /// <summary>
        /// 状态更新
        /// </summary>
        void OnUpdate();

        /// <summary>
        /// 物理帧更新更新
        /// </summary>
        void OnFixedUpdate();

        /// <summary>
        /// 状态退出
        /// </summary>
        void OnExit();
    }
}