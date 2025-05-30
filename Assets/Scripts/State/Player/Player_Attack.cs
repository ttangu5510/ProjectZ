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

    // 코사인 30도의 값 Deg2Rad : 30도를 라디안 값으로 변경함. 약 0.86
    private float normalAtkAngle = Mathf.Cos(50f * Mathf.Deg2Rad);

    public override void Enter()
    {

    }

    public override void Update()
    {

        // 공격 상태에 따른 조건문 switch
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
                    // 홀드 타임이 회전베기 충전시간을 넘어가면 조건문에 해당 안되서 넘어감
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

        // 공격 키 누를 시
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
                    // 홀드 애니메이션 재생
                    player.animator.SetBool("IsCharge", true);
                }
                else if (holdTime >= chargeThreshold)
                {
                    // 차지 완료 재생
                }
            }
        }

        // 공격 키 뗄 시
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
        // 애니메이션 재생
        Debug.Log("첫 번째 공격");
        player.animator.SetBool("IsAttack", true);
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }
    private void SecondAttack()
    {
        Debug.Log("두 번째 공격");
        // 애니메이션 재생
        player.animator.SetTrigger("IsAttack2");
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }
    private void ThirdAttack()
    {
        // 애니메이션 재생
        Debug.Log("세 번째 공격");
        player.animator.SetTrigger("IsAttack3");
        attackOrder++;
        canAttack = false;
        attackTimer = 0;
    }

    public void NormalAttackDamage(Collider[] enemies)
    {
        foreach (Collider enemy in enemies)
        {
            // TODO: 겟컴포넌트 피하는 최적화 가능( 너무 많은 적이 아니면 이것도 괜찮음)
            if (enemy.TryGetComponent<IDamagable>(out IDamagable t))
            {
                Vector3 dirToEnemy = (enemy.transform.position - player.transform.position).normalized;

                // dot의 값은 -1~1. 1이면 정면, -1이면 후면. 두 벡터의 dot product다
                float dot = Vector3.Dot(player.playerAvatar.transform.forward, dirToEnemy);

                // dot이 30도 이내라는 것은, 적과 플레이어 정면의 각이 30도 이내라는 뜻
                if (dot > normalAtkAngle)
                {
                    Debug.Log(" 적에게 데미지 주기");
                    t.TakeDamage(player.attackPower);
                }
            }

        }
    }

    private void SpinAttack()
    {
        // 애니메이션 재생
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
            // TODO: 겟컴포넌트 피하는 최적화 가능( 너무 많은 적이 아니면 이것도 괜찮음)
            if (enemy.TryGetComponent<IDamagable>(out IDamagable t))
            {
                Debug.Log(" 스핀어택 데미지 주기");
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