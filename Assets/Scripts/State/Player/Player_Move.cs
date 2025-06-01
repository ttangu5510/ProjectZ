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
        // ���� ĳ��Ʈ ���� ���� : ������ ������, ������, �������, ��ٸ����� �� �Ǵ� �ʿ�
        // ���� ���� ���ǵ�
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

        // ĳ���� ���� �Ʒ��� ���� ���� ���� ����
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

        // ������ ���� �ƴϸ�
        // if(!isRolling)
        // {

        //          if(isClimbWall && ����)
        //              ��Ÿ�� ���� ����
        //          else if(isPlatform && ����)
        //              ���� ���� ����
        //              �̵� �ִϸ��̼� ���(����)
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

// ������
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

    // �Լ���
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


// ��Ÿ��
public class Player_OnWall : Player_Move
{
    private bool canUpdate;
    public Player_OnWall(Player player) : base(player)
    {
        HasPhysics = true;
        canUpdate = false;
    }
    public override void Enter()
    {
        // ���� �߿� ���� ���̸�, ��ŸƮ �ִϸ��̼� ����
        if (player.isAir)
        {
            player.isAir = false;
        }

        if (player.isStartClimbUp)
        {
            // ������ �����ϴ� �ִϸ��̼� ���
            // ����� ������ canUpdate = true;
            canUpdate = true;
        }
        else if (player.isStartClimbDown)
        {
            // �������� �����ϴ� �ִϸ��̼� ���
            // ����� ������ canUpdate = true;
        }
        player.isOnWall = true;
        // �� Ÿ�� �߿��� �߷��� ������ �ڵ����� �� ������
        player.rig.useGravity = false;
    }
    public override void Update()
    {
        // ���� �ִϸ��̼��� ����Ǿ�� ������Ʈ�� ����
        if (canUpdate)
        {
            // A Ű ������ ���ϻ��·� ��ȯ
            if (player.isInteract)
            {
                player.transform.Translate(-player.playerAvatar.transform.forward*0.5f, Space.World);
                player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Fall]);
            }
            if (Physics.Raycast(player.transform.position + Vector3.up * 1.5f, player.playerAvatar.forward, 0.6f))
            {
                Debug.Log("�� �� �ö��");
                // canUpdate = false;
                // �� �� �ö� �ִϸ��̼� ���
                // ��� ���� ��
                // ���� ���� -> Idle;
            }
        }

    }
    public override void FixedUpdate()
    {
        if (!canUpdate)
        {
            SetPlayerRotation(-player.colNormal);
        }
        if (canUpdate)
        {
            SetAimRotation();
            SetPlayerQuickRotation(-player.colNormal);
            SetClimbMove();
        }
        // if �����ɽ�Ʈ �� ��ġ���� �ؼ� None �� ��
        // ���ִϸ��̼�
        // �� �ڵ����� ��ġ�� �������� ���� = �̰� ��������
        // ���� ResetState�� ���� ����

        // else if OnCollisionEnter Ground �̸� Idle�� ��ȯ

        // else if ���ε� OnCollisionExit ���̸� ����

        // if(�̵� Ű �Է� ��)
        //      �� ���� �����¿� �̵� ���� normal ���͸� ��� �޾ƿ;� ��

    }
    public override void Exit()
    {
        // player.isClimbWall = false;
        // canUpdate =false;
        player.rig.useGravity = true;
        player.isOnWall = false;
    }
    private void SetClimbMove()
    {
        Vector3 inputDir = GetAvatarMoveDirection();

        player.rig.velocity = inputDir * player.moveSpeed*0.5f;

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
        player.isAir = true;
        player.rig.velocity = Vector3.zero;
        player.rig.AddForce((player.playerAvatar.forward + moveDir + Vector3.up) * player.moveSpeed * 1.3f, ForceMode.Impulse);
        player.animator.SetTrigger("IsJump");
        player.animator.SetBool("IsAir", true);
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {
        SetAimRotation();
    }
    public override void Exit()
    {
        player.rig.velocity = Vector3.zero;
        player.animator.SetBool("IsAir", false);
    }
}

// ����
public class Player_OnFall : Player_Move
{
    public Player_OnFall(Player player) : base(player) { }

    public override void Enter()
    {
        player.isAir = true;
        // ���� �ִϸ��̼� ���
    }
    public override void Update()
    {

    }
    public override void FixedUpdate()
    {
        // ���� �� �̵� ����(�̼� ����)
        // ���� �� ī�޶� ���� ����
    }
    public override void Exit()
    {
        player.isAir = false;
    }
}
