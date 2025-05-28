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
        // 레이 캐스트
        // if(이동 키입력 없음)
        // {
        //     player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle];
        // }
        //
        // 구르는 중이 아니면
        // if(!isRolling)
        // {
        //      if(이동 키 조작이 없을 시)
        //          Idle로 전이
        //      if(이동 키 입력 시)
        //          if(isClimbWall && 전진)
        //              벽타기 상태 전이
        //          else if(isPlatform && 전진)
        //              점프 상태 전이
        //              이동 애니메이션 재생(블렌드)
        //          
        //      if( A 키 입력시)
        //          구르기 상태(isRolling) = true;
        //          무적 (isInvincible) = true;
        // }

        // 구르는 중이면
        // else if(isRolling)
        // {
        //      애니메이션 재생
        // }


    }
    public override void FixedUpdate()
    {
        // 이동 로직 수행
        // if(isRolling)
        // {
        //      rig.velocity = moveSpeed *(1+inputDir.normalized)
        // }
        // else
        // {
        //      rig.velocity = moveSpeed * inputDir;
        // }
    }
    public override void Exit() { }

}

public class Player_OnWall:Player_Move
{
    public Player_OnWall(Player player) : base(player) 
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        
    }
    public override void Update()
    {
        base.Update();
        // if(A 키 입력 시)
        //      낙하 상태로 전이
    }
    public override void FixedUpdate()
    {
        // if(이동 키 입력 시)
        //      벽 기준 상하좌우 이동
    }
    public override void Exit() 
    {
        // player.isClimbWall = false;
    }
}

public class Player_OnJump :Player_Move
{
    private float timer;
    public Player_OnJump(Player player) : base(player)
    {
        HasPhysics = true;
    }
    public override void Enter()
    {
        // 애니메이션 재생
        // 임펄스 위로 쏘기
        
    }
    
    public override void FixedUpdate()
    {
        // 이동
    }

}