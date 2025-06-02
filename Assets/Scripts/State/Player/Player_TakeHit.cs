using UnityEngine;

public class Player_TakeHit : PlayerState
{
    // TODO : 컬러 체인지 테스트
    private float timer;
    private float changeTime = 1f;
    public Player_TakeHit(Player player) : base(player)
    {
        HasPhysics = true;
    }

    public override void Enter()
    {
        player.animator.SetBool("IsHit", true);

        if (!player.isFalling)
        { 
            player.rig.velocity = Vector3.zero;

            Vector3 hitDirection = -player.playerAvatar.forward+Vector3.up;
            player.rig.AddForce(hitDirection*3f, ForceMode.Impulse);
        }
        player.ChangeColor();
        player.isInvincible = true;
    }
    public override void Update() 
    {
        timer += Time.deltaTime;
        if (timer < changeTime)
        {
        }
        else
        {
            HitOver();
        }
    }
    public override void FixedUpdate()
    {

    }
    public override void Exit() { }
    public void HitOver()
    {
        timer = 0;
        player.isInvincible = false;
        player.animator.SetBool("IsHit", false);
        player.animator.SetBool("IsFall", false);
        player.isFalling = false;
        player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        player.ChangeColor();
    }
}
