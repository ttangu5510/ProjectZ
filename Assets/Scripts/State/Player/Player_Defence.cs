using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Defence : PlayerState
{
    public Player_Defence(Player player) : base(player) { }

    public override void Enter()
    {
        player.animator.SetBool("IsDefence", true);
    }
    public override void Update()
    {
        if(!player.defenceInputAction.IsPressed())
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        }
        
    }
    public override void Exit() 
    {
        player.animator.SetBool("IsDefence", false);
    }
}
