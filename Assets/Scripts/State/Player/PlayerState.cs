using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : BaseState
{
    // �÷��̾� ����
    protected Player player;
    // �����ڷ� �÷��̾� ������ ����
    public PlayerState(Player player)
    {
        this.player = player;
    }

    public override void Enter()
    {

    }


    public override void Update()
    {
        // ���⼭ ��ǲ ����
    }
    public override void Exit() { }
}
