using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Aim : PlayerState
{
    public Player_Aim(Player player) : base(player) { }
    public override void Enter()
    {
        HasPhysics = true;
        player.aimCamera.gameObject.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        if(!player.isAim)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        }

        // if( �̵� Ű �Է� ��)
        //      �ָ� �ִϸ��̼ǰ� ����
        //      �̵� ���� ����

        // if( R ��ƽ ���� ��)
        //      ī�޶� ������

        // if(YŰ Ȧ��)
        // {
        //     ���� ����(IsBulletLoad) = true;
        // }
        // else if(YŰ Ȧ�� ����)
        // {
        //     ���� �߻�(IsBulletLoad) = false;
        // }
    }
    public override void FixedUpdate()
    {
        if (player.isAim)
        {
            if(player.InputDirection!= Vector2.zero)
            {
                SetMove(player.moveSpeed);
                Vector3 aimDir = SetAimRotation();
                SetPlayerRotation(aimDir);
                player.animator.SetFloat("MoveSpeed", player.rig.velocity.magnitude);
            }
        }
    }
    public override void Exit()
    {
        player.aimCamera.gameObject.SetActive(false);
    }
}
