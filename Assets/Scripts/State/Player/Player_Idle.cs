using UnityEngine;
public class Player_Idle : PlayerState
{
    public Player_Idle(Player player) : base(player) 
    {
        HasPhysics = true;
    }

    public override void Enter()
    {

    }

    public override void Update()
    {

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

        // if(���� Ű Ȧ��)
        // {
        //      ���� �ߵ�
        //      isWeaponOut = true;
        //      Aim ���·� ����
        // }

        if(player.InputDirection != Vector2.zero )
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Move]);
            player.animator.SetBool("IsMove", true);
        }

        // if(�ָ� Ű �Է� ��)
        //      �ָ� ���·� ����

        // if(��� Ű �Է� ��)
        //      ��� ���·� ����

        // if(���� Ű �Է� ��)
        //      ���� ���·� ����
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





