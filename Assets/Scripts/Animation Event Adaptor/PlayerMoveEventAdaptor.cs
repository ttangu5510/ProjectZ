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
            Debug.LogError("�÷��̾� ������Ʈ�� ����");
        }
        if(player.stateMachine.stateDic.TryGetValue(SState.ClimbWall, out BaseState state))
        {
            playerClimbState = state as Player_OnWall;
            if(playerClimbState == null)
            {
                Debug.LogError("Player_OnWall �̶�� ��ũ��Ʈ�� ����");
            }
        }
        else if(state == null)
        {
            Debug.LogError("�� ������ ���°� ��ųʸ��� ����");
        }
    }

    void Update() { }
    public void ClimbDone()
    {
        playerClimbState.ClimbAnimeDone();
    }
}
