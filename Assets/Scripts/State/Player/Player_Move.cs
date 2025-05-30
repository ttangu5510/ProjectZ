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
        // ���� ĳ��Ʈ ���� ���� : ������ ������, ������, �������, ��ٸ����� �� �Ǵ� �ʿ�
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

        // ������ ���� �ƴϸ�
        // if(!isRolling)
        // {

        //          if(isClimbWall && ����)
        //              ��Ÿ�� ���� ����
        //          else if(isPlatform && ����)
        //              ���� ���� ����
        //              �̵� �ִϸ��̼� ���(����)
        //          
        //      if( A Ű �Է½�)
        //          ������ ����(isRolling) = true;
        //          ���� (isInvincible) = true;
        // }

        // ������ ���̸�
        // else if(isRolling)
        // {
        //      �ִϸ��̼� ���
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
        // �̵� ���� ����
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

// ������
// TODO : ������ ���� �����Է�
// ����ó��
// �浹 �� ��ȯ
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


// ��Ÿ��
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
        // if(A Ű �Է� ��)
        //      ���� ���·� ����
    }
    public override void FixedUpdate()
    {
        // if(�̵� Ű �Է� ��)
        //      �� ���� �����¿� �̵�
    }
    public override void Exit()
    {
        // player.isClimbWall = false;
    }
}


// �����ϱ�
public class Player_OnJump : Player_Move
{
    private float timer;
    public Player_OnJump(Player player) : base(player)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        // �ִϸ��̼� ���
        // ���޽� ���� ���

    }

    public override void FixedUpdate()
    {
        // �̵�
    }

}

