using UnityEngine;
public class Player_Idle : PlayerState
{
    public Player_Idle(Player player) : base(player) { }

    public override void Enter()
    {
        Debug.Log(" ���̵� ���� ����");
    }

    public override void Update()
    {
        Debug.Log("���̵� ���� ������Ʈ");
        if(player.RotateDirection!= Vector2.zero)
        {
            SetAimRotation();
        }
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
            Debug.Log("���� ���·� ����");
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Move]);
        }

        // if(�ָ� Ű �Է� ��)
        //      �ָ� ���·� ����

        // if(��� Ű �Է� ��)
        //      ��� ���·� ����

        // if(���� Ű �Է� ��)
        //      ���� ���·� ����
    }

    public override void Exit() { }
}





