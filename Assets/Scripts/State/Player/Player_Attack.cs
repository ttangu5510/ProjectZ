using UnityEngine;

public class Player_Attack : PlayerState
{
    private float chainTime = 0.2f;
    private float FirstAttackDuration = 0.5f;
    private float SecondAttackDuration = 0.5f;
    private float ThirdAttackDuration = 1f;

    private float chargeThreshold = 1.5f;
    private float chargeAttackDuration = 1f;

    private bool canAttack = true;
    private int attackOrder = 0;
    private float attackTimer = 0;
    private float holdTime = 0;
    public Player_Attack(Player player) : base(player)
    {
        HasPhysics = true;
    }

    // �ڻ��� 30���� �� Deg2Rad : 30���� ���� ������ ������. �� 0.86
    private float normalAtkAngle = Mathf.Cos(50f * Mathf.Deg2Rad);

    public override void Enter()
    {

    }

    public override void Update()
    {

        // ���� ���¿� ���� ���ǹ� switch
        if (attackOrder > 0)
        {
            attackTimer += Time.deltaTime;
            switch (attackOrder)
            {
                case 1:
                    if (attackTimer < FirstAttackDuration)
                    {
                        break;
                    }
                    else if (attackTimer <= FirstAttackDuration + chainTime && !player.isAttack)
                    {
                        canAttack = true;
                    }
                    else if (attackTimer > FirstAttackDuration + chainTime && !player.isAttack && holdTime < chargeThreshold)
                    {
                        ResetState();
                    }
                    // Ȧ�� Ÿ���� ȸ������ �����ð��� �Ѿ�� ���ǹ��� �ش� �ȵǼ� �Ѿ
                    break;
                case 2:
                    if (attackTimer < SecondAttackDuration)
                    {
                        break;
                    }
                    else if (attackTimer <= SecondAttackDuration + chainTime && !player.isAttack)
                    {
                        canAttack = true;
                    }
                    else if (attackTimer > SecondAttackDuration + chainTime && !player.isAttack && holdTime < chargeThreshold)
                    {
                        ResetState();
                    }
                    break;
                case 3:
                    if (attackTimer < ThirdAttackDuration)
                    {
                        break;
                    }
                    else if (attackTimer <= ThirdAttackDuration + chainTime && !player.isAttack)
                    {
                        canAttack = true;
                    }
                    else if (attackTimer > ThirdAttackDuration + chainTime && !player.isAttack && holdTime < chargeThreshold)
                    {
                        ResetState();
                    }
                    break;
                case 4:
                    if (attackTimer > chargeAttackDuration)
                    {
                        ResetState();
                    }
                    break;
            }
        }

        // ���� Ű ���� ��
        if (player.isAttack)
        {
            if (canAttack)
            {
                switch (attackOrder)
                {
                    case 0:
                        FirstAttack();
                        break;
                    case 1:
                        SecondAttack();
                        break;
                    case 2:
                        ThirdAttack();
                        break;

                }
            }
            else if (!canAttack && attackOrder > 0)
            {
                holdTime += Time.deltaTime;
                if (holdTime >= 1 && holdTime < chargeThreshold)
                {
                    // Ȧ�� �ִϸ��̼� ���
                    player.animator.SetBool("IsCharge", true);
                }
                else if (holdTime >= chargeThreshold)
                {
                    // ���� �Ϸ� ���
                }
            }
        }

        // ���� Ű �� ��
        else if (!player.isAttack)
        {
            player.animator.SetBool("IsCharge", false);
            if (holdTime >= chargeThreshold && attackOrder > 0)
            {
                Debug.Log($"{attackTimer}  {attackOrder}");
                SpinAttack();
            }
        }
    }
    public override void FixedUpdate()
    {
        SetAimRotation();
    }

    public override void Exit()
    {
        player.animator.SetBool("IsAttack", false);
    }
    private void FirstAttack()
    {
        // �ִϸ��̼� ���
        Debug.Log("ù ��° ����");
        player.animator.SetBool("IsAttack", true);
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }
    private void SecondAttack()
    {
        Debug.Log("�� ��° ����");
        // �ִϸ��̼� ���
        player.animator.SetTrigger("IsAttack2");
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }
    private void ThirdAttack()
    {
        // �ִϸ��̼� ���
        Debug.Log("�� ��° ����");
        player.animator.SetTrigger("IsAttack3");
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }

    public void NormalAttackDamage(Collider[] enemies)
    {
        foreach (Collider enemy in enemies)
        {
            // TODO: ��������Ʈ ���ϴ� ����ȭ ����( �ʹ� ���� ���� �ƴϸ� �̰͵� ������)
            if (enemy.TryGetComponent<IDamagable>(out IDamagable t))
            {
                Vector3 dirToEnemy = (enemy.transform.position - player.transform.position).normalized;

                // dot�� ���� -1~1. 1�̸� ����, -1�̸� �ĸ�. �� ������ dot product��
                float dot = Vector3.Dot(player.playerAvatar.transform.forward, dirToEnemy);

                // dot�� 30�� �̳���� ����, ���� �÷��̾� ������ ���� 30�� �̳���� ��
                if (dot > normalAtkAngle)
                {
                    Debug.Log(" ������ ������ �ֱ�");
                    t.TakeDamage(player.attackPower);
                }
            }

        }
    }

    private void SpinAttack()
    {
        // �ִϸ��̼� ���
        player.animator.SetTrigger("IsSpinAttack");
        canAttack = false;
        attackOrder = 4;
        attackTimer = 0;
        holdTime = 0;
    }

    public void SpinAttackDamage(Collider[] enemies)
    {
        foreach (Collider enemy in enemies)
        {
            // TODO: ��������Ʈ ���ϴ� ����ȭ ����( �ʹ� ���� ���� �ƴϸ� �̰͵� ������)
            if (enemy.TryGetComponent<IDamagable>(out IDamagable t))
            {
                Debug.Log(" ���ɾ��� ������ �ֱ�");
                t.TakeDamage(player.attackPower*2);
            }
        }
    }

    private void ResetState()
    {
        attackOrder = 0;
        attackTimer = 0;
        holdTime = 0;
        canAttack = true;
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
    }

}