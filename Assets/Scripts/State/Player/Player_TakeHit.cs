using UnityEngine;

public class Player_TakeHit : PlayerState
{
    // TODO : �÷� ü���� �׽�Ʈ
    private float timer;
    private float changeTime = 2f;
    public Player_TakeHit(Player player) : base(player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        player.animator.SetBool("IsHit", true);

        if (player.isFalling)
        {
            // �߶� �ǰ� �ִϸ��̼� ���
            // �ִϸ��̼� ���� �� ���� ��ȯ �Լ� ���� = HitOver
        }
        else
        {
            // �ǰ� �ִϸ��̼� ���
            // �˹�
            player.rig.velocity = Vector3.zero;
            // player.rig.velocity = moveDir

        }

        player.isInvincible = true;
    }
    public override void Update() { }
    public override void FixedUpdate()
    {
        timer += Time.deltaTime;
        if(timer < changeTime)
        {
        player.ChangeColor();
        }
        else
        {
            HitOver();
        }
    }
    public override void Exit() { }
    public void HitOver()
    {
        player.isInvincible = false;
        player.animator.SetBool("IsHit", false);
        player.animator.SetBool("IsFall", false);
        player.isFalling = false;
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
    }
}
