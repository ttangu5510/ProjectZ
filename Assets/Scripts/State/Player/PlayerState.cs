using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : BaseState
{
    // 플레이어 참조
    protected Player player;
    // 생성자로 플레이어 참조를 받음
    public PlayerState(Player player)
    {
        this.player = player;
    }

    public override void Enter()
    {

    }


    public override void Update()
    {
        // 여기서 인풋 받음
    }
    public override void Exit() { }
}
