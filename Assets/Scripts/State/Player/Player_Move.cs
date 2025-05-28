using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Move : PlayerState
{
    public Player_Move(Player player) : base(player)
    {
        Debug.Log("������� ����");
        HasPhysics = true;
    }

    public override void Enter() { }
    public override void Update()
    {
        // ���� ĳ��Ʈ ���� ���� : ������ ������, ������, �������, ��ٸ����� �� �Ǵ� �ʿ�

        if(player.InputDirection == Vector2.zero)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        }
        // ������ ���� �ƴϸ�
        // if(!isRolling)
        // {
        if(player.InputDirection != Vector2.zero)
        {
            Debug.Log("���� ���� ������Ʈ");
            moveDir = SetMove(player.moveSpeed);
            SetPlayerRotation(moveDir);
        }
        //          if(isClimbWall && ����)
        //              ��Ÿ�� ���� ����
        //          else if(isPlatform && ����)
        //              ���� ���� ����
        //              �̵� �ִϸ��̼� ���(����)
        //          
        //      if( A Ű �Է½�)
        //          ������ ����(isRolling) = true;
        //          ���� (isInvincible) = true;
        // }

        // ������ ���̸�
        // else if(isRolling)
        // {
        //      �ִϸ��̼� ���
        // }


    }
    public override void FixedUpdate()
    {
        Debug.Log("������� �Ƚ��������Ʈ");
        // �̵� ���� ����
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
        // if(A Ű �Է� ��)
        //      ���� ���·� ����
    }
    public override void FixedUpdate()
    {
        // if(�̵� Ű �Է� ��)
        //      �� ���� �����¿� �̵�
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
        // �ִϸ��̼� ���
        // ���޽� ���� ���
        
    }
    
    public override void FixedUpdate()
    {
        // �̵�
    }

}