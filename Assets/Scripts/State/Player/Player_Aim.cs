using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Aim : PlayerState
{
    public Player_Aim(Player player) : base(player) { }
    public override void Enter()
    {
        // 이동속도 절반
    }

    public override void Update()
    {
        base.Update();

        // if(조준 키 홀드 해제 시)
        //      Idle 상태로 전이

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
    public override void Exit()
    {
        // 이동속도 복귀
    }
}
