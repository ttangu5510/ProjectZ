using System.Collections.Generic;

public class StateMachine
{
    public Dictionary<SState, BaseState> stateDic;

    public BaseState curState;

    public StateMachine()
    {
        stateDic = new();
    }
    public void ChangeState(BaseState nextState)
    {
        if (curState == nextState) return;

        curState.Exit();
        curState = nextState;
        curState.Enter();
    }

    public void Update() => curState.Update();

    public void FixedUpdate()
    {
        if (curState.HasPhysics) curState.FixedUpdate();
    }
}
