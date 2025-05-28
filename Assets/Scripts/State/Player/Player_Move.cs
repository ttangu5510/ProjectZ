using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : PlayerState
{
    public Player_Move(Player player) : base(player)
    {
        HasPhysics = true;
    }

    public override void Enter() { }
    public override void Update()
    {
        base.Update();
        // if(!isRolling)
        // {
        //      if(이동 키 조작이 없을 시)
        //          Idle로 전이
        //      if(이동 키 입력 시)
        //          이동 애니메이션 재생(블렌드)
        //      if( A 키 입력시)
        //          구르기 상태(isRolling) = true;
        // }

    }
    public override void FixedUpdate()
    {
        // 이동 로직 수행
        // if(isRolling)
        //      rig.velocity = moveSpeed *(1+inputDir.normalized)
        //      
        //
    }
    public override void Exit() { }

}
