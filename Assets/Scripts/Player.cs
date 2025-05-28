using Cinemachine;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    // 값 설정
    [field: Header("Set Value")]
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpPower { get; set; }
    [field: SerializeField] public int attackPower { get; set; }
    [field: SerializeField] public int hp { get; set; }

    // 인스턴스 및 컴포넌트 참조
    [SerializeField] public CinemachineVirtualCamera virtualCamera;

    public StateMachine stateMachine;
    public Rigidbody rig;
    public Animator animator;

    // 판단 변수들
    public bool isControlActive { get; set; } // 컨트롤 가능 여부
    public bool isAir {  get; set; }
    public bool isGrab { get; set; }
    public bool isWeaponOut { get; set; }
    public bool isNaviOut { get; set; }
    public bool isBulletLoad { get; set; }
    public bool isRolling { get; set; }
    public bool isRollToWall { get; set; }
    public bool isInvincible { get; set; }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();


        StateMachineInit();
    }
    void OnCollisionEnter(Collision collision)
    {
        // 구르는 중 벽에 부딪히면
        if(isRolling && collision.gameObject.layer == 6)
        {
            isRollToWall = true;
        }
        // 낙하 중 OnCollision 벽
        if(isAir&&collision.gameObject.layer == 9)
        {
            stateMachine.ChangeState(stateMachine.stateDic[SState.OnWall]);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // if(콜라이더가 플랫폼 엣지면)
        //  isAir = true;
        //  stateMachine.ChangeState(stateMachine.stateDic[SState.Jump]);
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
        stateMachine.stateDic.Add(SState.OnWall, new Player_OnWall(this));
        stateMachine.stateDic.Add(SState.OnHit, new Player_TakeHit(this));
        stateMachine.stateDic.Add(SState.OnJump, new Player_OnJump(this));

        // 초기 상태 설정
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        stateMachine.ChangeState(stateMachine.stateDic[SState.OnHit]);
    }
}
