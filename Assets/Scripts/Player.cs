using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamagable
{
    // 값 설정
    [field: Header("Set Value")]
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpPower { get; set; }
    [field: SerializeField] public int attackPower { get; set; }
    [field: SerializeField] public int hp { get; set; }
    [field: Range(0.01f, 2f)][field: SerializeField] public float rotSensitivity { get; set; }
    [field: Range(10f, 360f)][field: SerializeField] public float rotSpeed { get; set; }
    public Vector2 currentRotation;

    // 인스턴스 및 컴포넌트 참조
    [SerializeField] public CinemachineVirtualCamera virtualCamera;
    [SerializeField] public Transform aim;
    [SerializeField] public Transform aimCamera;
    [SerializeField] public Transform playerAvatar;
    [SerializeField] public Transform playerUpperAvatar;


    public StateMachine stateMachine;
    public Rigidbody rig;
    public Animator animator;

    private float canLandAngle = Mathf.Cos(45f * Mathf.Deg2Rad);
    private float canClimbAngle = Mathf.Cos(70f * Mathf.Deg2Rad);

    // 판단 변수들
    public bool isControlActive { get; set; } // 컨트롤 가능 여부
    public bool isAir { get; set; }
    public bool isAttack { get; set; }
    public bool isAim { get; set; }
    public bool isGrab { get; set; }
    public bool isWeaponOut { get; set; }
    public bool isNaviOut { get; set; }
    public bool isBulletLoad { get; set; }
    public bool isInteract { get; set; }
    public bool isRolling { get; set; }
    public bool isRollToWall { get; set; }
    public bool isInvincible { get; set; }

    // 인풋액션
    public InputAction attackInputAction;
    public InputAction aimInputAction;
    public InputAction interactionInputAction;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        StateMachineInit();

        attackInputAction = GetComponent<PlayerInput>().actions["Attack"];
        aimInputAction = GetComponent<PlayerInput>().actions["Aim"];
        interactionInputAction = GetComponent<PlayerInput>().actions["Interaction"];
    }
    private void OnEnable()
    {
        currentRotation = new();

        attackInputAction.Enable();
        attackInputAction.started += AttackInput;
        attackInputAction.canceled += AttackInput;

        aimInputAction.Enable();
        aimInputAction.started += AimInput;
        aimInputAction.canceled += AimInput;

        interactionInputAction.Enable();
        interactionInputAction.started += InteractionInput;
        interactionInputAction.canceled += InteractionInput;
        // performed를 쓰는 방법
        // .performed를 추가하면 됨

    }
    private void OnDisable()
    {
        attackInputAction.Disable();
        attackInputAction.started -= AttackInput;
        attackInputAction.canceled -= AttackInput;

        aimInputAction.Disable();
        aimInputAction.started -= AimInput;
        aimInputAction.canceled -= AimInput;

        interactionInputAction.Disable();
        interactionInputAction.started -= InteractionInput;
        interactionInputAction.canceled -= InteractionInput;
    }
    void OnCollisionEnter(Collision collision)
    {
        int contactCount = collision.contactCount;

        for (int i = 0; i < contactCount; i++)
        {
            ContactPoint contact = collision.GetContact(i);
            Vector3 contNormal = contact.normal;
            float landingAngle = Vector3.Dot(transform.up, contNormal);
            float wallAngle = Vector3.Dot(playerAvatar.forward, -contNormal);

            if (collision.gameObject.layer == 10 && wallAngle > canClimbAngle)
            {
                stateMachine.ChangeState(stateMachine.stateDic[SState.ClimbWall]);
            }
            if (isAir)
            {
                // 자동 점프 후 착지
                if (collision.gameObject.layer == 8 && landingAngle > canLandAngle)
                {
                    isAir = false;
                    stateMachine.ChangeState(stateMachine.stateDic[SState.Idle]);
                }
                // 낙하 중 OnCollision 벽
                if (collision.gameObject.layer == 10 && wallAngle > canClimbAngle)
                {
                    isAir = false;
                    stateMachine.ChangeState(stateMachine.stateDic[SState.ClimbWall]);
                }
            }
        }
        if (isRolling && collision.gameObject.layer == 9)
        {
            isRollToWall = true;
        }



    }
    private void OnTriggerEnter(Collider other)
    {
        // if(콜라이더가 플랫폼 엣지면)
        //  isAir = true;
        //  stateMachine.ChangeState(stateMachine.stateDic[SState.Jump]);
    }

    private void Update()
    {
        Debug.Log($"현제 스테이트 : {stateMachine.curState}");
        stateMachine.curState.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine.curState.HasPhysics)
        {
            stateMachine.curState.FixedUpdate();
        }
    }

    // SM 생성 및 딕셔너리 추가
    void StateMachineInit()
    {
        stateMachine = new StateMachine();
        // 여기서 this는 Player 클래스의 인스턴스다
        stateMachine.stateDic.Add(SState.Idle, new Player_Idle(this));
        stateMachine.stateDic.Add(SState.Aim, new Player_Aim(this));
        stateMachine.stateDic.Add(SState.Attack, new Player_Attack(this));
        stateMachine.stateDic.Add(SState.OnHit, new Player_TakeHit(this));
        stateMachine.stateDic.Add(SState.Move, new Player_Move(this));
        stateMachine.stateDic.Add(SState.Roll, new Player_OnRoll(this));
        stateMachine.stateDic.Add(SState.ClimbWall, new Player_OnWall(this));
        stateMachine.stateDic.Add(SState.OnJump, new Player_OnJump(this));

        // 초기 상태 설정
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            stateMachine.ChangeState(stateMachine.stateDic[SState.OnHit]);
            hp -= damage;
        }
    }

    // 인풋 시스템
    public Vector2 InputDirection { get; private set; }
    public Vector2 RotateDirection { get; private set; }
    public void OnMove(InputValue value)
    {
        InputDirection = value.Get<Vector2>();
    }
    public void OnRotate(InputValue value)
    {
        Vector2 rotDir = value.Get<Vector2>();
        rotDir.y *= -1;
        RotateDirection = rotDir * rotSensitivity;
    }
    public void AttackInput(InputAction.CallbackContext ctx)
    {
        isAttack = ctx.started;
    }
    public void AimInput(InputAction.CallbackContext ctx)
    {
        isAim = ctx.started;
    }

    public void InteractionInput(InputAction.CallbackContext ctx)
    {
        isInteract = ctx.started;
    }
}

