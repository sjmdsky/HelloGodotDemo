using System;

interface IStateMachine
{
    void TransitionState(State fromStae, State toState);
    void TickPhysics(State currentState, Double delta);
    State GetNextState(State currentState);
}
