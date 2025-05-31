using UnityEngine;

public class Player_Move : PlayerState
{
    protected Vector3 moveDir;
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
        // 이동 로직 수행
        // if(isRolling)
        // {
        //      rig.velocity = moveSpeed *(1+inputDir.normalized)
        // }
        // else
        // {
        //      rig.velocity = moveSpeed * inputDir;
        // }
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
    private enum ClimbType { Wall,Ladder}
    private ClimbType climbType;
    public Player_OnWall(Player player) : base(player)
    {
        HasPhysics = true;
        climbType = ClimbType.Wall;
    }
    public override void Enter()
    {

    }
    public override void Update()
    {
        base.Update();
        // if(A 키 입력 시)
        //      낙하 상태로 전이
    }
    public override void FixedUpdate()
    {
        // if 레이케스트 얼굴 위치에서 해서 None 될 때
        // , 업애니메이션
        // 및 자동으로 위치를 지상으로 조정
        // 이후 ResetState로 탈출

        // if 벽인데 OnCollisionExit 벽이면 낙하
              

        // if(이동 키 입력 시)
        //      벽 기준 상하좌우 이동 벽의 normal 벡터를 계속 받아와야 함
    }
    public override void Exit()
    {
        // player.isClimbWall = false;
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
        player.rig.AddForce((player.playerAvatar.forward + moveDir + Vector3.up) * player.moveSpeed*1.3f, ForceMode.Impulse);
        player.animator.SetTrigger("IsJump");
        player.animator.SetBool("IsAir", true);
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {

    }
    public override void Exit()
    {
        player.rig.velocity = Vector3.zero;
        player.animator.SetBool("IsAir", false);
    }

}

