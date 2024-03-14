using UnityEngine;

[System.Serializable]
public class MyParmeter
{
    [Header("敌人的状态")]
    public int health; // 生命

    public float moveSpeed; //移动速度
    public float chaseSpeed; // 追击速度
    public float idleTime; // 停止时间
    public Transform[] patrolPoints; // 巡逻范围
    public Transform[] chasePoints; // 追击范围
    public Animator animator; // 动画控制器
    public Transform target; // 追击目标
    public LayerMask targetLayer; // 目标层级
    public Transform attackPoint; // 圆心检测位置
    public float attackRadius; // 圆的半径
    public bool getHit;
    public Vector2 direction;
    public float hitSpeed;
    public Rigidbody2D rb;
    public Animator hitAnimation;
}

public class BehaviourTreeDriver : MonoBehaviour
{
    public BehaviourTree behaviourTree;
    public MyParmeter myParmeter;

    private void Awake()
    {
        behaviourTree.OnTreeStart(this.gameObject);
        StartCoroutine(behaviourTree.InitNode());
    }

    private void Update()
    {
        behaviourTree.OnUpdate();
    }
}