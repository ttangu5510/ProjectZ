using UnityEngine;

public class PlayerState : BaseState
{
    // 플레이어 참조
    protected Player player;

    // 생성자로 플레이어 참조를 받음
    public PlayerState(Player player)
    {
        this.player = player;
    }

    public override void Enter()
    {

    }
    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        // 추가 조건 붙여야함
        if (player.isAim)
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Aim]);
    }
    public override void Exit() { }


    // 움직임 관련 함수
    public Vector3 SetMove(float moveSpeed)
    {
        Vector3 moveDirection = GetMoveDirection();
        Vector3 curVelocity = player.rig.velocity;
        curVelocity.x = moveDirection.x * moveSpeed;
        curVelocity.z = moveDirection.z * moveSpeed;

        player.rig.velocity = curVelocity*player.InputDirection.magnitude;
        return moveDirection;
    }
    public void SetPlayerRotation(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        player.playerAvatar.rotation = Quaternion.Lerp(player.playerAvatar.rotation, targetRotation, player.rotSpeed * Time.deltaTime);
    }
    public Vector3 SetAimRotation()
    {
        player.currentRotation.x += player.RotateDirection.x;
        player.currentRotation.y = Mathf.Clamp(player.currentRotation.y + player.RotateDirection.y, -50, 50);
        player.transform.rotation = Quaternion.Euler(0, player.currentRotation.x, 0);

        Vector3 currentEuler = player.aimCamera.localEulerAngles;
        player.aim.localEulerAngles = new Vector3(player.currentRotation.y, currentEuler.y, currentEuler.z);

        Vector3 rotateDirVec = player.transform.forward;
        rotateDirVec.y = 0;
        return rotateDirVec.normalized;
    }
    public Vector3 GetMoveDirection()
    {
        Vector3 direction = (player.transform.right * player.InputDirection.x) + (player.transform.forward * player.InputDirection.y);
        return direction.normalized;
    }
}