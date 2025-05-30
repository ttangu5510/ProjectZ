using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IDamagable
{
    // �� ����
    [field: Header("Set Value")]
    [field: SerializeField] public float moveSpeed { get; set; }
    [field: SerializeField] public float jumpPower { get; set; }
    [field: SerializeField] public int attackPower { get; set; }
    [field: SerializeField] public int hp { get; set; }
    [field: Range(0.01f, 2f)][field: SerializeField] public float rotSensitivity { get; set; }
    [field: Range(10f, 360f)][field: SerializeField] public float rotSpeed { get; set; }
    public Vector2 currentRotation;

    // �ν��Ͻ� �� ������Ʈ ����
    [SerializeField] public CinemachineVirtualCamera virtualCamera;
    [SerializeField] public Transform aim;
    [SerializeField] public Transform aimCamera;
    [SerializeField] public Transform playerAvatar;
    [SerializeField] public Transform playerUpperAvatar;


    public StateMachine stateMachine;
    public Rigidbody rig;
    public Animator animator;

    // �Ǵ� ������
    public bool isControlActive { get; set; } // ��Ʈ�� ���� ����
    public bool isAir { get; set; }
    public bool isAttack { get; set; }
    public bool isAim { get; set; }
    public bool isGrab { get; set; }
    public bool isWeaponOut { get; set; }
    public bool isNaviOut { get; set; }
    public bool isBulletLoad { get; set; }
    public bool isInteract {  get; set; }
    public bool isRolling { get; set; }
    public bool isRollToWall { get; set; }
    public bool isInvincible { get; set; }

    // ��ǲ�׼�
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
        // performed�� ���� ���
        // .performed�� �߰��ϸ� ��

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
        // if(�÷��̾ isAir && collision = ��)
        // ü����������Ʈ = Idle
        // ������ �� ���� �ε�����
        if (isRolling && collision.gameObject.layer == 9)
        {
            isRollToWall = true;
        }
        // ���� �� OnCollision ��
        if (isAir && collision.gameObject.layer == 9)
        {
            stateMachine.ChangeState(stateMachine.stateDic[SState.ClimbWall]);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // if(�ݶ��̴��� �÷��� ������)
        //  isAir = true;
        //  stateMachine.ChangeState(stateMachine.stateDic[SState.Jump]);
    }

    private void Update()
    {
        Debug.Log($"���� ������Ʈ : {stateMachine.curState}");
        stateMachine.curState.Update();
    }

    private void FixedUpdate()
    {
        if (stateMachine.curState.HasPhysics)
        {
            stateMachine.curState.FixedUpdate();
        }
    }

    // SM ���� �� ��ųʸ� �߰�
    void StateMachineInit()
    {
        stateMachine = new StateMachine();
        // ���⼭ this�� Player Ŭ������ �ν��Ͻ���
        stateMachine.stateDic.Add(SState.Idle, new Player_Idle(this));
        stateMachine.stateDic.Add(SState.Aim, new Player_Aim(this));
        stateMachine.stateDic.Add(SState.Attack, new Player_Attack(this));
        stateMachine.stateDic.Add(SState.OnHit, new Player_TakeHit(this));
        stateMachine.stateDic.Add(SState.Move, new Player_Move(this));
        stateMachine.stateDic.Add(SState.Roll, new Player_OnRoll(this));
        stateMachine.stateDic.Add(SState.ClimbWall, new Player_OnWall(this));
        stateMachine.stateDic.Add(SState.OnJump, new Player_OnJump(this));

        // �ʱ� ���� ����
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        stateMachine.ChangeState(stateMachine.stateDic[SState.OnHit]);
    }

    // ��ǲ �ý���
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

