using Cinemachine.Utility;
using UnityEngine;

public class PlayerState : BaseState
{
    // �÷��̾� ����
    protected Player player;

    protected Vector3 moveDir;
    // �����ڷ� �÷��̾� ������ ����
    public PlayerState(Player player)
    {
        this.player = player;
        moveDir = new Vector3();
    }

    public override void Enter()
    {

    }


    public override void Update()
    {

    }
    public override void Exit() { }

    // ������ ���� �Լ�
    public Vector3 SetMove(float moveSpeed)
    {
        Vector3 moveDirection = player.rig.velocity;
        moveDirection.x = player.InputDirection.x * moveSpeed;
        moveDirection.z = player.InputDirection.y * moveSpeed;

        player.rig.velocity = moveDirection;
        return moveDirection.normalized;
    }
    public void SetPlayerRotation(Vector3 moveDirection)
    {
        if (moveDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        player.transform.rotation = Quaternion.Lerp(player.transform.rotation, targetRotation, player.rotSpeed * Time.deltaTime);
    }
    public void SetAimRotation(float rotSpeed)
    {
        player.currentRotation.x += player.RotateDirection.x;
        player.currentRotation.y = Mathf.Clamp(player.currentRotation.y + player.RotateDirection.y, -90, 90);
    }
}