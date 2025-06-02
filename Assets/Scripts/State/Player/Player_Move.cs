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
            // ĳ���� ���� �Ʒ��� ���� ���� ���� ����
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
    }

    // �Լ���
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


// ��Ÿ��
// �Ϸ� �ִϸ��̼��̶� �������̶� ���� �ʿ�
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
        // ���� �߿� ���� ���̸�, ��ŸƮ �ִϸ��̼� ����
        if (player.isAir)
        {
            player.isAir = false;
        }
        // �� Ÿ�� �߿��� �߷��� ������ �ڵ����� �� ������
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
            // A Ű ������ ���ϻ��·� ��ȯ
            if (player.isInteract)
            {
                ChangeToFall();
            }

            // �÷��̾� ���濡 ����ĳ��Ʈ. ���� ������ ������ �� �ö�� ������ �Ǵ�
            else
            {
                Physics.Raycast(player.transform.position + Vector3.up * 1.1f, player.playerAvatar.forward, out RaycastHit hitInfo, 0.6f);
                if (!player.isInteract && hitInfo.Equals(default(RaycastHit)))
                {
                    SetToGround();
                }
                // ��, �� ���� �� ���� �������� �̵��� ���
                else if (hitInfo.collider != null)
                {
                    if (hitInfo.collider.gameObject.layer == 8)
                    {
                        Physics.Raycast(player.transform.position + Vector3.up * 0.1f, player.playerAvatar.forward, out RaycastHit secHitInfo, 0.6f);
                        // ����, �� �̸� ����
                        if (secHitInfo.collider != null)
                        {
                            if (secHitInfo.collider.gameObject.layer == 8)
                            {
                                ChangeToFall();
                            }
                        }
                        // ����, �����̸� ����
                        else if (secHitInfo.Equals(default(RaycastHit)))
                        {
                            ChangeToFall();
                        }
                    }
                }
                // �ٴ������� ����ĳ��Ʈ�ؼ� �׶���� Idle�� ��ȯ
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
        player.animator.SetBool("IsClimb", false);
        player.animator.SetTrigger("IsClimbDone");
        // ��Ÿ�� ���� �ִϸ��̼� ���� �� �̺�Ʈ�� ClimbAnimeDone() ȣ���
    }

    // Adaptor���� ȣ��Ǵ� �Լ�
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


// �����ϱ�
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
        // �÷��̾� ��ũ��Ʈ�� OnCollision���� �Ǵ� ��, ���¸� �����ϸ鼭 �����
        player.rig.velocity = Vector3.zero;
        player.animator.SetBool("IsAir", false);
        timer = 0;
    }
}

// ����
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
        // ���� �� �̵� ����(�̼� ����)
        if (timer > 1)
        {
            SetMove(player.moveSpeed * 0.1f);
        }
        // ���� �� ī�޶� ���� ����
        SetAimRotation();
    }
    public override void Exit() { }

    //OnCollisionEnter���� ȣ��
    public void CheckHitTime()
    {
        player.isAir = false;
        if (timer > hitTime)
        {
            // ��Ʈ ���¿��� �߶����� ���� �ִϸ��̼� ���
            // ���� IsFall = false
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
