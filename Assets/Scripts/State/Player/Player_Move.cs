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
        // ���� ĳ��Ʈ ���� ���� : ������ ������, ������, �������, ��ٸ����� �� �Ǵ� �ʿ�
        base.Update();
        if(player.InputDirection == Vector2.zero)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
            player.animator.SetBool("IsMove", false);
        }

        // ������ ���� �ƴϸ�
        // if(!isRolling)
        // {

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
        if (player.InputDirection != Vector2.zero)
        {
            Vector3 moveDir = SetMove(player.moveSpeed);
            Vector3 aimDir = SetAimRotation();
            SetPlayerRotation(moveDir);
            player.animator.SetFloat("MoveSpeed", player.rig.velocity.magnitude);
        }
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
    public override void Exit() 
    {

    }

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