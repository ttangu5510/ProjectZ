using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    // �� ����
    [field: Header("Set Value")]
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpPower { get; set; }
    [field: SerializeField] public int attackPower { get; set; }

    // �ν��Ͻ� �� ������Ʈ ����
    [SerializeField] public CinemachineVirtualCamera virtualCamera;

    public StateMachine stateMachine;
    public Rigidbody2D rig;
    public Animator animator;

    // �Ǵ� ������
    public bool isControlActive { get; set; } // ��ȣ�ۿ� ������ ��
    public bool isGrab { get; set; }
    public bool isWeaponOut { get; set; }
    public bool isNaviOut { get; set; }
    public bool isBulletLoad { get; set; }
    public bool isRolling { get; set; }
    public bool isRollToWall { get; set; }
    
    void OnCollisionEnter(Collision collision)
    {
        if(isRolling)
        {
            isRollToWall = true;
        }
    }

    // SM ���� �� ��ųʸ� �߰�
    void StateMachineInit()
    {
        stateMachine = new StateMachine();
        // ���⼭ this�� Player Ŭ������ �ν��Ͻ���
        stateMachine.stateDic.Add(SState.Idle, new Player_Idle(this));
        stateMachine.stateDic.Add(SState.Aim, new Player_Aim(this));
        stateMachine.stateDic.Add(SState.Move, new Player_Move(this));
        stateMachine.stateDic.Add(SState.Attack, new Player_Attack(this));

        // �ʱ� ���� ����
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }
    public void TakeDamage(int damage)
    {

    }
}
