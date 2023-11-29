using Godot;
using static Godot.GD;

[GlobalClass]
public partial class HitBox : Area2D
{
    // BodyEnteredEventHandler
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += _onBodyEntered;
    }

    private void _onBodyEntered(Node2D body)
    {
        Print($"[{Engine.GetPhysicsFrames()}][Hit] {Owner.Name} -> {body.Name}");

        States states = body.GetNode<States>("States");
        if (Owner is Player)
        {
            StateMachine stateMachine = Owner.GetNode<StateMachine>("StateMachine");
            switch (stateMachine.currentState)
            {
                case State.Attack1:
                    states.OnHit(1, Owner);
                    return;
                case State.Attack2:
                    states.OnHit(2, Owner);
                    return;
                case State.Attack3:
                    states.OnHit(5, Owner);
                    return;
            }
            // if (body is PhysicsBody2D)
            // {
            //     Print($"[{Engine.GetPhysicsFrames()}][Hit] {Owner.Name} -> {body.Name} {Owner is Enemy}");
            // }

        }
        states.OnHit(1, Owner);
    }
}