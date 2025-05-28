using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_TakeHit : PlayerState
{
    public Player_TakeHit(Player player) : base(player) 
    {
        HasPhysics = true;
    }

    public override void Enter() 
    {
        // 무적
        //     player.isInvincible = true;

    }
    public override void FixedUpdate()
    {
        // 피격 애니메이션 재생
        // 넉백
        // player.rig.velocity = moveDir
    }
    public override void Exit()
    {
        //      player.isInvincible = false;
    }
}
public class Player_HitToWall : Player_TakeHit
{
    public Player_HitToWall(Player player) : base(player)
    {
        HasPhysics = true;
        // 진행방향 받아오기
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void FixedUpdate()
    {
        // 튕겨나오는 애니메이션
        // 넉백
        // 플레이어 진행방향과 반대로 이동
    }
    public override void Exit()
    {
        base.Exit();
    }


}

public class Player_FallHit : Player_TakeHit
{
    public Player_FallHit(Player player) : base(player)
    {

    }


}
