public abstract class BaseState
{
    // FixedUpdate 사용 시
    public bool HasPhysics;

    // 상태 입장 시
    public abstract void Enter();
    // 핵심 로직
    public abstract void Update();
    // 물리 연산
    public virtual void FixedUpdate() { }
    // 상태 나갈 때
    public abstract void Exit();
    // HFSM 중, 부모 상태

}
public enum SState { Idle, Move, Attack, Defence, Look, Fall, Water, OnHit, OnInteract, Aim }