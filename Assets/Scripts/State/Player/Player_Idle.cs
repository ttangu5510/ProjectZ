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
        // if(A 키 입력 시)
        // {
        //      switch(상태에 따라)
        //      {
        //          무언가 들고 있을 시(IsGrab)
        //              내려놓기
        //          상호 작용
        //              발도 중 -> 납도(IsWeaponOut)
        //              납도 애니메이션
        //              상태로 넘어감
        //          
        //          발도 중 -> 납도
        //              애니메이션 재생
        //          나비가 들어가있음 -> 나옴(IsNaviOut)
        //              나비가 플레이어 주위 패트롤
        // }

        if(player.isAttack)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Attack]);
        }

        // if(주목 키 입력 시)
        //      주목 상태로 전이
        if(player.isAim)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Aim]);
        //      활 발도
        //      isWeaponOut = true;
        //      Aim 상태로 전이
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





