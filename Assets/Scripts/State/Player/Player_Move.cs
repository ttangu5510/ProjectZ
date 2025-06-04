using UnityEngine;

public class Player_Move : PlayerState
{
    protected Vector3 moveDir { get; set; }
    public Player_Move(Player player) : base(player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        player.animator.SetBool("IsMove", true);
    }
    public override void Update()
    {
        // 레이 캐스트 정면 근접 : 정면이 벽인지, 문인지, 사람인지, 사다리인지 등 판단 필요
        // 상태 변경 조건들
        if (player.InputDirection == Vector2.zero)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        }
        if (player.isAim)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Aim]);
        }
        if (player.isAttack)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Attack]);
        }
        if (player.isInteract && !player.isRolling)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Roll]);
        }
        if(player.defenceInputAction.IsPressed())
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Defence]);
        }

        // 캐릭터 앞의 아래로 빛을 쏴서 정보 읽음
        Vector3 frontPos = player.playerAvatar.transform.position + (player.transform.forward) * 0.1f + Vector3.up * 0.2f;
        Physics.Raycast(frontPos, Vector3.down, out RaycastHit hitInfo, 0.8f);
        if (hitInfo.Equals(default(RaycastHit)))
        {
            if (!player.isAir && player.InputDirection.magnitude > 0.5f)
            {
                player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.OnJump]);
            }
        }
    }
    public override void FixedUpdate()
    {
        if (player.InputDirection != Vector2.zero)
        {
            moveDir = SetMove(player.moveSpeed);
            SetAimRotation();
            SetPlayerRotation(moveDir);
            player.animator.SetFloat("MoveSpeed", player.rig.velocity.magnitude);


        }
    }
    public override void Exit()
    {
        player.animator.SetBool("IsMove", false);
        player.animator.SetFloat("MoveSpeed", 0f);
    }

}

// 구르기
public class Player_OnRoll : Player_Move
{
    private enum RollState { Roll, CollideToWall }
    private RollState rollState;

    private float rollTime = 0.5f;
    private float rollFail = 1.5f;
    private float timer = 0;
    public Player_OnRoll(Player player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        player.isRolling = true;
        player.animator.SetBool("IsRoll", true);
        player.animator.SetBool("IsInvincible", true);
        moveDir = player.playerAvatar.transform.forward;
        rollState = RollState.Roll;
        player.isInvincible = true;
    }
    public override void Update()
    {
        if (rollState == RollState.Roll)
        {
            timer += Time.deltaTime;
            if (timer > rollTime && !player.isRollToWall)
            {
                ResetState();
            }
            else if (player.isRollToWall)
            {
                HitToWall();
            }
            // 캐릭터 앞의 아래로 빛을 쏴서 정보 읽음
            Vector3 frontPos = player.playerAvatar.transform.position + (player.transform.forward) * 0.1f + Vector3.up * 0.2f;
            Physics.Raycast(frontPos, Vector3.down, out RaycastHit hitInfo, 0.8f);
            if (hitInfo.Equals(default(RaycastHit)))
            {
                if (!player.isAir && player.InputDirection.magnitude > 0.5f)
                {
                    player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.OnJump]);
                }
            }
        }
        else if (rollState == RollState.CollideToWall)
        {
            RollFail();
        }
    }
    public override void FixedUpdate()
    {
        if (!player.isRollToWall)
        {
            SetRollMove();
        }
        SetAimRotation();
    }
    public override void Exit()
    {
        base.Exit();
        timer = 0;
        player.isInvincible = false;
        player.isRolling = false;
        player.animator.SetBool("IsRoll", false);
        player.animator.SetBool("IsInvincible", false);
    }

    // 함수들
    private void ResetState()
    {
        player.isRollToWall = false;
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
    }
    private void SetRollMove()
    {
        Vector3 inputDir = GetMoveDirection();
        float dot = Vector3.Dot(moveDir, inputDir);
        player.rig.velocity = moveDir * Mathf.Clamp(dot * 1.5f + 1.5f, 0, 1.5f) * player.moveSpeed;
    }
    private void HitToWall()
    {
        player.animator.SetTrigger("IsHitToWall");
        rollState = RollState.CollideToWall;
        timer = 0;
        player.rig.velocity = Vector3.zero;
        player.rig.AddForce(-moveDir * 3, ForceMode.Impulse);
    }
    private void RollFail()
    {
        timer += Time.deltaTime;
        if (timer > rollFail)
        {
            ResetState();
        }
    }
}


// 벽타기
// 완료 애니메이션이랑 지상도착이랑 구분 필요
public class Player_OnWall : Player_Move
{
    private bool canUpdate;
    private float timer;
    private float raycastStartTimer = 0.3f;
    public Player_OnWall(Player player) : base(player)
    {
        HasPhysics = true;
        canUpdate = true;
    }
    public override void Enter()
    {

        canUpdate = true;
        player.animator.SetBool("IsClimb", true);
        // 낙하 중에 들어온 벽이면, 스타트 애니메이션 없음
        if (player.isAir)
        {
            player.isAir = false;
        }
        // 벽 타는 중에는 중력을 꺼야지 자동으로 안 내려감
        player.rig.useGravity = false;
        player.transform.Translate(player.transform.up * 0.2f, Space.World);
    }

    public override void Update()
    {
        if (timer < raycastStartTimer)
        {
            timer += Time.deltaTime;
        }
        if (canUpdate && timer >= raycastStartTimer)
        {
            // A 키 누르면 낙하상태로 전환
            if (player.isInteract)
            {
                ChangeToFall();
            }

            // 플레이어 전방에 레이캐스트. 맞은 정보가 없으면 다 올라온 것으로 판단
            else
            {
                Physics.Raycast(player.transform.position + Vector3.up * 1.1f, player.playerAvatar.forward, out RaycastHit hitInfo, 0.6f);
                if (!player.isInteract && hitInfo.Equals(default(RaycastHit)))
                {
                    SetToGround();
                }
                // 좌, 우 오를 수 없는 벽쪽으로 이동할 경우
                else if (hitInfo.collider != null)
                {
                    if (hitInfo.collider.gameObject.layer == 8)
                    {
                        Physics.Raycast(player.transform.position + Vector3.up * 0.1f, player.playerAvatar.forward, out RaycastHit secHitInfo, 0.6f);
                        // 만약, 벽 이면 낙하
                        if (secHitInfo.collider != null)
                        {
                            if (secHitInfo.collider.gameObject.layer == 8)
                            {
                                ChangeToFall();
                            }
                        }
                        // 만약, 공중이면 낙하
                        else if (secHitInfo.Equals(default(RaycastHit)))
                        {
                            ChangeToFall();
                        }
                    }
                }
                // 바닥쪽으로 레이캐스트해서 그라운드면 Idle로 전환
                Physics.Raycast(player.transform.position, -player.transform.up, out RaycastHit feetInfo, 0.1f);
                if (feetInfo.collider != null)
                {
                    if (feetInfo.collider.gameObject.layer == 8)
                    {
                        canUpdate = false;
                        player.rig.velocity = Vector3.zero;
                        player.animator.SetBool("IsClimb", false);
                        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
                    }
                }
            }
        }
    }
    public override void FixedUpdate()
    {
        if (!canUpdate && !player.animator.GetBool("IsClimb"))
        {
            SetPlayerRotation(-player.colNormal);
            Vector3 movePos = player.playerAvatar.transform.position + player.playerAvatar.transform.forward + Vector3.up * 0.5f;
            player.transform.position = Vector3.MoveTowards(player.transform.position, movePos, 1f * Time.deltaTime);
        }
        if (canUpdate)
        {
            SetAimRotation();
            SetPlayerQuickRotation(-player.colNormal);
            SetClimbMove();
        }
    }
    public override void Exit()
    {
        timer = 0;
        player.rig.useGravity = true;
        player.animator.SetBool("IsClimb", false);
    }
    private void SetClimbMove()
    {
        Vector3 inputDir = GetAvatarMoveDirection(out float moveX, out float moveY);
        player.animator.SetFloat("MoveX", moveX);
        player.animator.SetFloat("MoveY", moveY);
        player.rig.velocity = inputDir * player.moveSpeed * 0.5f;

    }
    private void SetToGround()
    {
        canUpdate = false;
        player.rig.velocity = Vector3.zero;
        player.isInvincible = true;
        player.animator.SetBool("IsClimb", false);
        player.animator.SetBool("IsInvincible", true);
        player.animator.SetTrigger("IsClimbDone");
        // 벽타기 종료 애니메이션 수행 중 이벤트로 ClimbAnimeDone() 호출됨
    }

    // Adaptor에서 호출되는 함수
    public void ClimbAnimeDone()
    {
        player.isInvincible = false;
        player.animator.SetBool("IsInvincible", false);
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
    }
    public void ChangeToFall()
    {
        player.transform.Translate(-player.playerAvatar.transform.forward * 0.5f, Space.World);
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Fall]);
    }
}


// 점프하기
public class Player_OnJump : Player_Move
{
    private float timer;
    private float fallTime = 2;
    public Player_OnJump(Player player) : base(player)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        player.isAir = true;
        player.rig.velocity = Vector3.zero;
        player.rig.AddForce((player.playerAvatar.forward + moveDir + Vector3.up) * player.moveSpeed * 1.3f, ForceMode.Impulse);
        player.animator.SetTrigger("IsJump");
        player.animator.SetBool("IsAir", true);
    }
    public override void Update()
    {
        timer += Time.deltaTime;
        if (timer > fallTime)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Fall]);
        }
    }
    public override void FixedUpdate()
    {
        SetAimRotation();
    }
    public override void Exit()
    {
        // 플레이어 스크립트의 OnCollision에서 판단 후, 상태를 변경하면서 수행됨
        player.rig.velocity = Vector3.zero;
        player.animator.SetBool("IsAir", false);
        timer = 0;
    }
}

// 낙하
public class Player_OnFall : Player_Move
{
    private float timer = 0;
    private float idleCheckTimer = 0;
    private float hitTime = 1.5f;
    private int fallDamage = 1;
    public Player_OnFall(Player player) : base(player) { }

    public override void Enter()
    {
        player.isAir = true;
        player.isFalling = true;
        player.animator.SetBool("IsFall", true);
        timer = 0;
    }
    public override void Update()
    {
        timer += Time.deltaTime;
        if (player.rig.velocity.magnitude <= 0.1f)
        {
            idleCheckTimer += Time.deltaTime;
            if (idleCheckTimer > 0.2f)
            {
                player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
                idleCheckTimer = 0;
            }
        }
    }
    public override void FixedUpdate()
    {
        // 낙하 중 이동 가능(이속 감산)
        if (timer > 1)
        {
            SetMove(player.moveSpeed * 0.1f);
        }
        // 낙하 중 카메라 조절 가능
        SetAimRotation();
    }
    public override void Exit() { }

    //OnCollisionEnter에서 호출
    public void CheckHitTime()
    {
        player.isAir = false;
        if (timer > hitTime)
        {
            // 히트 상태에서 추락으로 인한 애니메이션 재생
            // 이후 IsFall = false
            player.TakeDamage(fallDamage);
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.OnHit]);
        }
        else
        {
            player.isFalling = false;
            player.animator.SetBool("IsFall", false);
        }
        timer = 0;
    }
}
