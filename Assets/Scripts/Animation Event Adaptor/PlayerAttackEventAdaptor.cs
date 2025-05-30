using UnityEngine;

public class PlayerAttackEventAdaptor : MonoBehaviour
{
    public Player_Attack playerAtkState;

    public float normalAttackRadius = 1.5f;
    public float spinAttackRadius = 1.5f;
    public float jumpAttackRadius = 1.5f;
    public LayerMask enemyLayer = 1 << 7;

    private void Start()
    {
        Player player = GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("Player ������Ʈ�� ����");
            return;
        }

        if (player.stateMachine.stateDic.TryGetValue(SState.Attack, out BaseState state))
        {
            playerAtkState = state as Player_Attack;
            if (playerAtkState == null)
            {
                Debug.LogError("�ִϸ��̼� �̺�Ʈ ����Ϳ��� ���������� ���°� Player_Attack��  �ƴ�");
            }
        }
        else
        {
            Debug.LogError("stateMachine�� SState.Attack�� ����");
        }
    }

    public void OnNormalAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, normalAttackRadius, enemyLayer);
        if (playerAtkState != null)
        {
            Debug.Log("�÷��̾� ������ �Լ��� �ֱ�");
            playerAtkState.NormalAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("���� �ִϸ��̼��� ����Ϳ� ����������");
        }
    }
    public void OnSpinAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, spinAttackRadius, enemyLayer);

        if (playerAtkState != null)
        {
            Debug.Log("�÷��̾� ������ �Լ��� �ֱ�");
            playerAtkState.SpinAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("���� �ִϸ��̼��� ����Ϳ� ����������");
        }
    }
    public void OnJumpAttackEvent()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, jumpAttackRadius, enemyLayer);

        if (playerAtkState != null)
        {
            Debug.Log("�÷��̾� ������ �Լ��� �ֱ�");
     //       playerAtkState.FirstAttackDamage(hitEnemies);
        }
        else
        {
            Debug.LogError("���� �ִϸ��̼��� ����Ϳ� ����������");
        }
    }
}
