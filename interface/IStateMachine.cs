using System;

interface IStateMachine
{
    void TransitionState(State from, State to);
    void TickPhysics(State currentState, Double delta);
    State GetNextState(State currentState);
}
