using UnityEngine;

public class Player_Aim : PlayerState
{
    public Player_Aim(Player player) : base(player) { }
    public override void Enter()
    {
        HasPhysics = true;
        player.aimCamera.gameObject.SetActive(true);
        player.animator.SetBool("IsAim", true);
    }

    public override void Update()
    {
        base.Update();

        if (!player.isAim)
        {
            player.stateMachine.ChangeState(player.stateMachine.stateDic[SState.Idle]);
        }
        else if (player.isAim)
        {
            Vector3 aimDir = SetAimRotation();
            SetPlayerRotation(aimDir);
        }

        // if(Y≈∞ »¶µÂ)
        // {
        //     ªı√— ∂Ø±Ë(IsBulletLoad) = true;
        // }
        // else if(Y≈∞ »¶µÂ ≥ı¿Ω)
        // {
        //     ªı√— πﬂªÁ(IsBulletLoad) = false;
        // }
    }
    public override void FixedUpdate()
    {
        if (player.isAim)
        {
            if (player.InputDirection != Vector2.zero)
            {
                SetMove(player.moveSpeed);
                player.animator.SetFloat("MoveSpeed", player.rig.velocity.magnitude);
            }
        }
    }
    public override void Exit()
    {
        player.aimCamera.gameObject.SetActive(false);
        player.animator.SetBool("IsAim", false);
    }
}
