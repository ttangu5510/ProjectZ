public abstract class BaseState
{
    // FixedUpdate ��� ��
    public bool HasPhysics;

    // ���� ���� ��
    public abstract void Enter();
    // �ٽ� ����
    public abstract void Update();
    // ���� ����
    public virtual void FixedUpdate() { }
    // ���� ���� ��
    public abstract void Exit();
    // HFSM ��, �θ� ����

}
public enum SState { Idle, Move, Attack, Defence, Look, Fall, Water, OnHit, OnInteract, Aim }