using UnityEngine;
public class Player_Idle : PlayerState
{
    public Player_Idle(Player player) : base(player) { }

    public override void Enter()
    {
        Debug.Log(" 아이들 상태 입장");
    }

    public override void Update()
    {
        Debug.Log("아이들 상태 업데이트");
        if(player.RotateDirection!= Vector2.zero)
        {
            SetAimRotation();
        }
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

        // if(조준 키 홀드)
        // {
        //      새총 발도
        //      isWeaponOut = true;
        //      Aim 상태로 전이
        // }

        if(player.InputDirection != Vector2.zero )
        {
            Debug.Log("무브 상태로 전이");
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Move]);
        }

        // if(주목 키 입력 시)
        //      주목 상태로 전이

        // if(방어 키 입력 시)
        //      방어 상태로 전이

        // if(공격 키 입력 시)
        //      공격 상태로 전이
    }

    public override void Exit() { }
}





