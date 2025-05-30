using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable
{
    [field: SerializeField] public int hp { get; set; }
    private Coroutine TakeHitRoutine { get; set; }
    private Renderer rend;
    private Color originColor;
    [SerializeField] Color hitColor = Color.red;
    [SerializeField] float changeTime;
    public StateMachine stateMachine { get; set; }
    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();
        originColor = rend.material.color;
    }
    void Start()
    {
        StateMachineInit();
    }

    void Update()
    {
        stateMachine.curState.Update();
    }
    void FixedUpdate()
    {
        if(stateMachine.curState.HasPhysics)
        {
            stateMachine.curState.FixedUpdate();
        }
    }
    private void StateMachineInit()
    {
        stateMachine = new StateMachine();
        stateMachine.stateDic.Add(SState.Idle, new Enemy_Idle());
        
        stateMachine.curState =  stateMachine.stateDic[SState.Idle];
    }


    public void TakeDamage(int damage)
    {
        // 체력 줄이기
        // 적의 상태 변경 -> Enemy_Hit : 애니메이션 재생용
            Debug.Log($"데미지 입음{damage}, 현재 체력 {hp}");

        if(TakeHitRoutine==null)
        {
            TakeHitRoutine = StartCoroutine(HitChangeColor());
        }
    }
    IEnumerator HitChangeColor()
    {
        rend.material.color = hitColor;
        yield return new WaitForSecondsRealtime(changeTime);
        rend.material.color = originColor;
        TakeHitRoutine = null;

    }
}
