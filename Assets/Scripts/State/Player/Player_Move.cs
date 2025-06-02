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

        // 캐릭터 앞의 아래로 빛을 쏴서 정보 읽음
        Vector3 frontPos = player.playerAvatar.transform.position + (player.transform.forward) * 0.1f + Vector3.up * 0.2f;
        if (Physics.Raycast(frontPos, Vector3.down, out RaycastHit hitInfo, 0.8f))
        {

        }
        else if (hitInfo.Equals(default(RaycastHit)))
        {
            if (!player.isAir && player.InputDirection.magnitude > 0.5f)
            {
                player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.OnJump]);
            }
        }

        // 구르는 중이 아니면
        // if(!isRolling)
        // {

        //          if(isClimbWall && 전진)
        //              벽타기 상태 전이
        //          else if(isPlatform && 전진)
        //              점프 상태 전이
        //              이동 애니메이션 재생(블렌드)
        // }
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
        player.isInvincible = false;
    }

    // 함수들
    private void ResetState()
    {
        timer = 0;
        rollState = RollState.Roll;
        player.animator.SetBool("IsRoll", false);
        player.isRollToWall = false;
        player.isRolling = false;
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
public class Player_OnWall : Player_Move
{
    private bool canUpdate;
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
    }

    public override void Update()
    {
        if (canUpdate)
        {
            // A 키 누르면 낙하상태로 전환
            if (player.isInteract)
            {
                ChangeToFall();

            }
            // 플레이어 전방에 레이캐스트. 맞은 정보가 없으면 다 올라온 것으로 판단
            Physics.Raycast(player.transform.position + Vector3.up * 1.1f, player.playerAvatar.forward, out RaycastHit hitInfo, 0.6f);
            if (hitInfo.Equals(default(RaycastHit)))
            {
                SetToGround();
            }
            // 좌, 우 오를 수 없는 벽쪽으로 이동할 경우
            else if(hitInfo.collider != null)
            {
                if(hitInfo.collider.gameObject.layer == 8)
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
        }
    }
    public override void FixedUpdate()
    {
        if (!canUpdate)
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
        // if 레이케스트 얼굴 위치에서 해서 None 될 때
        // 업애니메이션
        // 및 자동으로 위치를 지상으로 조정 = 이걸 어케하지
        // 이후 ResetState로 상태 변경

        // else if OnCollisionEnter Ground 이면 Idle로 전환

        // else if 벽인데 OnCollisionExit 벽이면 낙하

        // if(이동 키 입력 시)
        //      벽 기준 상하좌우 이동 벽의 normal 벡터를 계속 받아와야 함
    }
    public override void Exit()
    {
        // player.isClimbWall = false;
        // canUpdate =false;
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
        player.animator.SetBool("IsClimb", false);
        // canUpdate = false;
        // 벽 다 올라간 애니메이션 재생
        // 재생 종료 후
        // 상태 변경 -> Idle;
    }
    public void ClimbAnimeDone()
    {
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
        player.animator.SetBool("IsAir",true);
    }
    public override void Update()
    {
        // 시간 지나면 IsFall -> 트루로 바꾸기
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
    }
}

// 낙하
public class Player_OnFall : Player_Move
{
    private float timer;
    private float hitTime = 1f;
    public Player_OnFall(Player player) : base(player) { }

    public override void Enter()
    {
        player.isAir = true;
        player.isFalling = true;
        player.animator.SetBool("IsFall", true);
        // 낙하 애니메이션 재생
    }
    public override void Update()
    {
        timer += Time.deltaTime;
    }
    public override void FixedUpdate()
    {
        // 낙하 중 이동 가능(이속 절반)
        if(timer > 1)
        {
            SetMove(player.moveSpeed*0.1f);
        }
        // 낙하 중 카메라 조절 가능
        SetAimRotation();
    }
    public override void Exit()
    {
    }
    public void CheckHitTime()
    {
        player.isAir = false;
        if(timer>hitTime)
        {
            // 히트 상태에서 추락으로 인한 애니메이션 재생
            // 이후 IsFall = false
            player.TakeDamage(1);
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
