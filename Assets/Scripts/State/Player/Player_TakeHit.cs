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
        // ����
        //     player.isInvincible = true;

    }
    public override void FixedUpdate()
    {
        // �ǰ� �ִϸ��̼� ���
        // �˹�
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
        // ������� �޾ƿ���
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void FixedUpdate()
    {
        // ƨ�ܳ����� �ִϸ��̼�
        // �˹�
        // �÷��̾� �������� �ݴ�� �̵�
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
