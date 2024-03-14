using UnityEngine;

[System.Serializable]
public class MyParmeter
{
    [Header("���˵�״̬")]
    public int health; // ����

    public float moveSpeed; //�ƶ��ٶ�
    public float chaseSpeed; // ׷���ٶ�
    public float idleTime; // ֹͣʱ��
    public Transform[] patrolPoints; // Ѳ�߷�Χ
    public Transform[] chasePoints; // ׷����Χ
    public Animator animator; // ����������
    public Transform target; // ׷��Ŀ��
    public LayerMask targetLayer; // Ŀ��㼶
    public Transform attackPoint; // Բ�ļ��λ��
    public float attackRadius; // Բ�İ뾶
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