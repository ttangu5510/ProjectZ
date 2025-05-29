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

        // if( 이동 키 입력 시)
        //      주목 애니메이션과 같음
        //      이동 로직 수행

        // if( R 스틱 조작 시)
        //      카메라 움직임

        // if(Y키 홀드)
        // {
        //     새총 땡김(IsBulletLoad) = true;
        // }
        // else if(Y키 홀드 놓음)
        // {
        //     새총 발사(IsBulletLoad) = false;
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
