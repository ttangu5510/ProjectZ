using UnityEngine;

public class PlayerAttackEventAdaptor : MonoBehaviour
{
    public Player_Attack playerAtkState;

    [Header("Set Attack Radius")]
    [Range(0,2)][SerializeField]public float normalAttackRadius = 1.5f;
    [Range(0, 2)][SerializeField] public float spinAttackRadius = 1.5f;
    [Range(0, 2)][SerializeField] public float jumpAttackRadius = 1.5f;

    [Header("Set Target Layer")]
    [SerializeField] public LayerMask enemyLayer = 1 << 7;

    private void Start()
    {
        Player player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player 컴포넌트가 없음");
            return;
        }

        if (player.stateMachine.stateDic.TryGetValue(SState.Attack, out BaseState state))
        {
            playerAtkState = state as Player_Attack;
            if (playerAtkState == null)
            {
                Debug.LogError("애니메이션 이벤트 어댑터에서 가져오려는 상태가 Player_Attack이  아님");
            }
        }
        else
        {
            Debug.LogError("stateMachine에 SState.Attack이 없음");
        }
    }

    public void OnNormalAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, normalAttackRadius, enemyLayer);
        if (playerAtkState != null)
        {
            playerAtkState.NormalAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("어택 애니메이션을 어댑터에 참조안했음");
        }
    }
    public void OnSpinAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, spinAttackRadius, enemyLayer);

        if (playerAtkState != null)
        {
            playerAtkState.SpinAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("어택 애니메이션을 어댑터에 참조안했음");
        }
    }
    public void OnJumpAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, jumpAttackRadius, enemyLayer);

        if (playerAtkState != null)
        {
     //       playerAtkState.FirstAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("어택 애니메이션을 어댑터에 참조안했음");
        }
    }
}
