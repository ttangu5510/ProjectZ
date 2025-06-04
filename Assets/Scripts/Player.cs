using Cinemachine;
using System;
using System.Collections.Generic;
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
    [Header("Set References")]
    [SerializeField] public CinemachineVirtualCamera virtualCamera;
    [SerializeField] public Transform aim;
    [SerializeField] public Transform aimCamera;
    [SerializeField] public Transform playerAvatar;
    [SerializeField] public Transform playerUpperAvatar;



    public StateMachine stateMachine;
    public Rigidbody rig;
    public Animator animator;
    public Renderer[] playerRenderers;
    private Dictionary<Renderer, Color> playerColors;
    private Player_OnFall fallState;

    private float canLandAngle = Mathf.Cos(45f * Mathf.Deg2Rad);
    private float canClimbAngle = Mathf.Cos(70f * Mathf.Deg2Rad);
    private float hitToWallAngle = Mathf.Cos(45f * Mathf.Deg2Rad);
    private float defenceAngle = Mathf.Cos(80f * Mathf.Deg2Rad);

    // 충돌체 노말벡터
    public Vector3 colNormal;


    // 인풋 판단
    public bool isAttack { get; set; }
    public bool isAim { get; set; }
    public bool isInteract { get; set; }
    public bool isStart { get; set; }

    // 판단 변수들
    public bool isAir { get; set; }
    public bool isRolling { get; set; }
    public bool isRollToWall { get; set; }
    public bool isInvincible { get; set; }
    public bool isFalling { get; set; }
    public bool isHit { get; set; }
    //public bool isControlActive { get; set; } // 컨트롤 가능 여부
    //public bool isWeaponOut { get; set; }
    //public bool isGrab { get; set; }
    //public bool isNaviOut { get; set; }
    //public bool isBulletLoad { get; set; }




    void Awake()
    {
        animator = GetComponent<Animator>();
        rig = GetComponent<Rigidbody>();
        StateMachineInit();

        attackInputAction = GetComponent<PlayerInput>().actions["Attack"];
        aimInputAction = GetComponent<PlayerInput>().actions["Aim"];
        interactionInputAction = GetComponent<PlayerInput>().actions["Interaction"];
        defenceInputAction = GetComponent<PlayerInput>().actions["Defence"];
        startInputAction = GetComponent<PlayerInput>().actions["Start"];


        playerRenderers = GetComponentsInChildren<Renderer>();
        colNormal = Vector3.zero;

        // 렌더러의 색상들 추가
        playerColors = new();
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i] != null && playerRenderers[i].material != null && playerRenderers[i].material.color != null)
            {
                playerColors.Add(playerRenderers[i], playerRenderers[i].material.color);
            }
        }
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

        defenceInputAction.Enable();

        startInputAction.Enable();
        startInputAction.started += StartInput;
        startInputAction.canceled += StartInput;
        // performed를 쓰는 방법
        // .performed를 추가하면 됨
        // InputAction의 Hold를 쓰는 법임
        // 한개 더 있는데, 그건 밑에 적음
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

        defenceInputAction.Disable();

        startInputAction.Enable();
        startInputAction.started += StartInput;
        startInputAction.canceled += StartInput;
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

            // 공중일 경우
            if (isAir && !isHit)
            {
                // 자동 점프 중에서 착지 -> Idle
                if (collision.gameObject.layer == 8 && landingAngle > canLandAngle)
                {
                    colNormal = contNormal;
                    isAir = false;
                    stateMachine.ChangeState(stateMachine.stateDic[SState.Idle]);
                }
                // 낙하 중 OnCollision 벽 -> OnWall
                if (collision.gameObject.layer == 10 && wallAngle > canClimbAngle)
                {
                    colNormal = contNormal;
                    stateMachine.ChangeState(stateMachine.stateDic[SState.ClimbWall]);
                }
            }
            else if (isHit)
            {
                if (collision.gameObject.layer == 8 && landingAngle > canLandAngle)
                {
                    colNormal = contNormal;
                    isAir = false;
                }
            }

            // 낙하 중일 경우
            if (isFalling && collision.gameObject.layer == 8)
            {
                stateMachine.stateDic.TryGetValue(SState.Fall, out BaseState state);
                fallState = state as Player_OnFall;
                fallState.CheckHitTime();
            }

            // 공중이 아니고 오를 수 있는 벽과 충돌 -> OnWall
            if (!isAir && collision.gameObject.layer == 10 && wallAngle > canClimbAngle)
            {
                colNormal = contNormal;
                stateMachine.ChangeState(stateMachine.stateDic[SState.ClimbWall]);
            }

            // 구르다 벽에 충돌 시
            if (isRolling && collision.gameObject.layer == 8 && wallAngle > hitToWallAngle)
            {
                isRollToWall = true;
            }
        }
    }

    private void Update()
    {
        if (isStart)
        {
            Time.timeScale = 0;
            UIManager.Instance.OpenMenu();
        }
        else
        {
            Debug.Log($"현재 상태 : {stateMachine.curState}");
            stateMachine.curState.Update();
            // TODO : 피격 테스트
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                TakeDamage(1);
            }
        }
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
        stateMachine.stateDic.Add(SState.Fall, new Player_OnFall(this));
        stateMachine.stateDic.Add(SState.Defence, new Player_Defence(this));

        // 초기 상태 설정
        stateMachine.curState = stateMachine.stateDic[SState.Idle];
    }

    // 피격 인터페이스
    public void TakeDamage(int damage)
    {

        if (!isInvincible)
        {
            hp -= damage;
            if (hp <= 0)
            {
                //player.Die();
            }
            stateMachine.ChangeState(stateMachine.stateDic[SState.OnHit]);
        }
    }
    // 피격 오버로딩
    public void TakeDamage(int damage, Transform damageTransform)
    {
        if (stateMachine.curState == stateMachine.stateDic[SState.Defence])
        {
            Vector3 hitDirection = damageTransform.position - transform.position;
            if (Vector3.Dot(playerAvatar.forward, hitDirection) < defenceAngle)
            {
                TakeDamage(damage);
            }
            else
            {
                Debug.Log("방어 성공");
            }
        }
        else
        {
            TakeDamage(damage);
        }
    }

    // 피격 시 색상 변경
    public void ChangeColor()
    {
        foreach (Renderer renderer in playerRenderers)
        {
            if (renderer.material.color == playerColors[renderer])
            {
                renderer.material.color = Color.red;
            }
            else
            {
                renderer.material.color = playerColors[renderer];
            }
        }

    }


    // 인풋 시스템
    public Vector2 InputDirection { get; private set; }
    public Vector2 RotateDirection { get; private set; }

    // 인풋액션
    public InputAction attackInputAction;
    public InputAction aimInputAction;
    public InputAction interactionInputAction;
    public InputAction defenceInputAction;
    public InputAction startInputAction;
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
        //isAttack = attackInputAction.IsPressed();
        // 눌려 있을 때만 true
    }
    public void AimInput(InputAction.CallbackContext ctx)
    {
        isAim = ctx.started;
    }

    public void InteractionInput(InputAction.CallbackContext ctx)
    {
        isInteract = ctx.started;
    }
    public void StartInput(InputAction.CallbackContext ctx)
    {
        isStart = ctx.started;
    }
}

