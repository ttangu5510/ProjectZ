using UnityEngine;

public class Player_Move : PlayerState
{
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
        base.Update();
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

        // 구르는 중이 아니면
        // if(!isRolling)
        // {

        //          if(isClimbWall && 전진)
        //              벽타기 상태 전이
        //          else if(isPlatform && 전진)
        //              점프 상태 전이
        //              이동 애니메이션 재생(블렌드)
        //          
        //      if( A 키 입력시)
        //          구르기 상태(isRolling) = true;
        //          무적 (isInvincible) = true;
        // }

        // 구르는 중이면
        // else if(isRolling)
        // {
        //      애니메이션 재생
        // }


    }
    public override void FixedUpdate()
    {
        if (player.InputDirection != Vector2.zero)
        {
            Vector3 moveDir = SetMove(player.moveSpeed);
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
// TODO : 구르기 전용 방향입력
// 무적처리
// 충돌 시 전환
public class Player_OnRoll : Player_Move
{
    private enum RollState { Roll, Hit }
    private RollState rollState;

    private float rollTime = 0.5f;
    private float rollFail = 1.5f;
    private float timer = 0;
    Vector3 moveDir;
    public Player_OnRoll(Player player) : base(player) { }
    public override void Enter()
    {
        base.Enter();
        player.isRolling = true;
        player.animator.SetBool("IsRoll", true);
        moveDir = player.playerAvatar.transform.forward;
        rollState = RollState.Roll;
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
                player.animator.SetTrigger("IsHitToWall");
                rollState = RollState.Hit;
                timer = 0;
                player.rig.velocity = Vector3.zero;
                player.rig.AddForce(-moveDir * 3, ForceMode.Impulse);
            }
        }
        else if (rollState == RollState.Hit)
        {
            HitToWall();
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
    }
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

        float dot = Vector3.Dot(moveDir, new Vector3(player.InputDirection.x, 0, player.InputDirection.y));
        player.rig.velocity = moveDir * Mathf.Clamp(dot * 1.5f + 1.5f, 0, 1.5f) * player.moveSpeed;
    }
    private void HitToWall()
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
    public Player_OnWall(Player player) : base(player)
    {
        HasPhysics = true;
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
        // if(이동 키 입력 시)
        //      벽 기준 상하좌우 이동
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
        // 애니메이션 재생
        // 임펄스 위로 쏘기

    }

    public override void FixedUpdate()
    {
        // 이동
    }

}

