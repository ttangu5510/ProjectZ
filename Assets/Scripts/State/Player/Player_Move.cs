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
        //      if(�̵� Ű ������ ���� ��)
        //          Idle�� ����
        //      if(�̵� Ű �Է� ��)
        //          �̵� �ִϸ��̼� ���(����)
        //      if( A Ű �Է½�)
        //          ������ ����(isRolling) = true;
        // }

    }
    public override void FixedUpdate()
    {
        // �̵� ���� ����
        // if(isRolling)
        //      rig.velocity = moveSpeed *(1+inputDir.normalized)
        //      
        //
    }
    public override void Exit() { }

}
