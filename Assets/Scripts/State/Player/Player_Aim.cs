using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Aim : PlayerState
{
    public Player_Aim(Player player) : base(player) { }
    public override void Enter()
    {
        // �̵��ӵ� ����
    }

    public override void Update()
    {
        base.Update();

        // if(���� Ű Ȧ�� ���� ��)
        //      Idle ���·� ����

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
    public override void Exit()
    {
        // �̵��ӵ� ����
    }
}
