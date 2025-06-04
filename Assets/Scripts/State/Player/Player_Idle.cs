using UnityEngine;
public class Player_Idle : PlayerState
{
    public Player_Idle(Player player) : base(player) 
    {
        HasPhysics = true;
    }

    public override void Enter() 
    {
        player.rig.velocity = Vector3.zero;
    }

    public override void Update()
    {
        base.Update();
        // if(A Ű �Է� ��)
        // {
        //      switch(���¿� ����)
        //      {
        //          ���� ��� ���� ��(IsGrab)
        //              ��������
        //          ��ȣ �ۿ�
        //              �ߵ� �� -> ����(IsWeaponOut)
        //              ���� �ִϸ��̼�
        //              ���·� �Ѿ
        //          
        //          �ߵ� �� -> ����
        //              �ִϸ��̼� ���
        //          ���� ������ -> ����(IsNaviOut)
        //              ���� �÷��̾� ���� ��Ʈ��
        // }

        if(player.isAttack)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Attack]);
        }

        // if(�ָ� Ű �Է� ��)
        //      �ָ� ���·� ����
        if(player.isAim)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Aim]);
        //      Ȱ �ߵ�
        //      isWeaponOut = true;
        //      Aim ���·� ����
        }
        if(player.InputDirection != Vector2.zero )
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Move]);
        }

        if(player.defenceInputAction.IsPressed())
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Defence]);
        }

    }
    public override void FixedUpdate()
    {
        if (player.RotateDirection != Vector2.zero)
        {
            SetAimRotation();
        }
    }

    public override void Exit() { }
}





