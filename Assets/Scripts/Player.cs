using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    // 값 설정
    [field: Header("Set Value")]
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpPower { get; set; }
    [field: SerializeField] public int attackPower { get; set; }

    // 인스턴스 및 컴포넌트 참조
    [SerializeField] public CinemachineVirtualCamera virtualCamera;

    public StateMachine stateMachine;
    public Rigidbody2D rig;
    public Animator animator;

    // 판단 변수들
    public bool isControlActive { get; set; } // 상호작용 상태일 때
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

    // SM 생성 및 딕셔너리 추가
    void StateMachineInit()
    {
        stateMachine = new StateMachine();
        // 여기서 this는 Player 클래스의 인스턴스다
        stateMachine.stateDic.Add(SState.Idle, new Player_Idle(this));
        stateMachine.stateDic.Add(SState.Aim, new Player_Aim(this));
        stateMachine.stateDic.Add(SState.Move, new Player_Move(this));
        stateMachine.stateDic.Add(SState.Attack, new Player_Attack(this));

        // 초기 상태 설정
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }
    public void TakeDamage(int damage)
    {

    }
}
