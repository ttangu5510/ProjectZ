using UnityEngine;

public class Player_TakeHit : PlayerState
{
    // TODO : 컬러 체인지 테스트
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
            // 추락 피격 애니메이션 재생
            // 애니메이션 끝날 때 상태 전환 함수 수행 = HitOver
        }
        else
        {
            // 피격 애니메이션 재생
            // 넉백
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
