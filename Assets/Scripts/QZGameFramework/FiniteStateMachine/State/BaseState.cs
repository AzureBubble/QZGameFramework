using UnityEngine;

namespace QZGameFramework.StateMachine
{
    /// <summary>
    /// 状态抽象基类
    /// 如果需要成员属性 成员变量 可使用状态基类
    /// </summary>
    public abstract class BaseState : IState
    {
        protected GameObject gameObject; // 状态持有对象 GameObject
        protected Transform transform; // Transform 组件
        protected Animator animator; // Animator组件
        protected string animationName; // 状态对应的 Animation 动画名称
        protected float animationTransitionTime = 0.1f; // 动画切换过渡时间
        protected BaseFsm fsm; // 状态机对象
        private int stateHash; // 状态对应的动画哈希值

        protected float CurrentInfoTime => animator.GetCurrentAnimatorStateInfo(0).normalizedTime; // 当前动画状态的归一化时间

        protected float CurrentAnimationTime // 当前动画状态的播放时间
        {
            get
            {
                AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                float animationLength = clipInfo.Length > 0 ? clipInfo[0].clip.length : 0f;
                return animationLength * CurrentInfoTime;
            }
        }

        protected bool IsAnimationFinished => CurrentAnimationTime >= animator.GetCurrentAnimatorStateInfo(0).length; // 当前动画是否播放结束

        /// <summary>
        /// 初始化状态方法
        /// </summary>
        public abstract void Init();

        public void OnCreate(BaseFsm fsm)
        {
            this.fsm = fsm;
            this.gameObject = this.fsm.Entity as GameObject;
            this.transform = this.gameObject.transform;
            this.animator = this.gameObject.GetComponent<Animator>();

            if (this.animator == null)
            {
                this.animator = this.gameObject.GetComponentInChildren<Animator>();
            }
#if UNITY_EDITOR
            if (this.animator == null)
            {
                Debug.LogError($"GameObject {this.gameObject.name}'s  animator is not found. Please Check.");
            }
#endif

            Init();
            // 通过状态对应的动画名字获得其在动画系统中的哈希值
            stateHash = Animator.StringToHash(animationName);
        }

        public virtual void OnEnter()
        {
            // 过度切换动画(动画哈希值，动画融合时间)
            animator.CrossFade(stateHash, animationTransitionTime);
        }

        public abstract void OnUpdate();

        public abstract void OnExit();
    }
}