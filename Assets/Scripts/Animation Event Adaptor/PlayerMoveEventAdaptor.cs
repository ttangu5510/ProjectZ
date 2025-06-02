using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveEventAdaptor : MonoBehaviour
{
    [Header("Set Timer")]
    [Range(0.5f,2f)][SerializeField] private float animationTimer = 1f;

    private Player player;
    public Player_OnWall playerClimbState;
    void Start()
    {
        player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("플레이어 컴포넌트가 없음");
        }
        if(player.stateMachine.stateDic.TryGetValue(SState.ClimbWall, out BaseState state))
        {
            playerClimbState = state as Player_OnWall;
            if(playerClimbState == null)
            {
                Debug.LogError("Player_OnWall 이라는 스크립트가 없음");
            }
        }
        else if(state == null)
        {
            Debug.LogError("벽 오르는 상태가 딕셔너리에 없음");
        }
    }

    void Update() { }
    public void ClimbDone()
    {
        playerClimbState.ClimbAnimeDone();
    }
}
