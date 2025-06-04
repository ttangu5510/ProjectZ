using UnityEngine;

public class Player_TakeHit : PlayerState
{
    // TODO : 컬러 체인지 테스트
    private float timer;
    private float hitDuration = 1f;
    public Player_TakeHit(Player player) : base(player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        player.isHit = true;
        player.isInvincible = true;
        player.animator.SetBool("IsHit", true);
        player.animator.SetBool("IsInvincible", true);

        if (!player.isFalling)
        {
            player.rig.velocity = Vector3.zero;

            Vector3 hitDirection = -player.playerAvatar.forward + Vector3.up;
            player.rig.AddForce(hitDirection * 2f, ForceMode.Impulse);
            player.isAir = true;
        }
        player.ChangeColor();
    }
    public override void Update()
    {
        timer += Time.deltaTime;
        if (!player.isAir)
        {
            if (timer >= hitDuration)
            {
                HitOver();
            }

        }
    }
    public override void FixedUpdate()
    {

    }
    public override void Exit() { }
    public void HitOver()
    {
        timer = 0;
        player.isHit = false;
        player.isFalling = false;
        player.isInvincible = false;
        player.animator.SetBool("IsHit", false);
        player.animator.SetBool("IsFall", false);
        player.animator.SetBool("IsInvincible", false);
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        player.ChangeColor();
    }
}
