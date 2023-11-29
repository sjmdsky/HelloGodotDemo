using System;
using Godot;
using static Godot.GD;
using static Godot.Mathf;
public partial class Boar : Enemy, IStateMachine
{
    const float KockBack = 200;
    RayCast2D wallChecker;
    RayCast2D floorChecker;
    RayCast2D playerChecker;
    Timer calmDownTimer;
    States states;

    public override void _Ready()
    {
        base._Ready();
        wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
        calmDownTimer = GetNode<Timer>("CalmDownTimer");
        states = GetNode<States>("States");
    }
    public State GetNextState(State currentState)
    {
        if (states.Health <= 0) return State.Dying;
        if (states.IsOnHit)
        {
            states.IsOnHit = false;
            return State.Hurt;
        }
        switch (currentState)
        {
            case State.Idle:
                if (playerChecker.GetCollider() is Player) return State.Running;
                if (StateMachine.stateTime > 2) return State.Walk;
                break;
            case State.Running:
                if (calmDownTimer.IsStopped() && playerChecker.GetCollider() is not Player) return State.Walk;
                break;
            case State.Walk:
                if (playerChecker.GetCollider() is Player) return State.Running;
                if (wallChecker.IsColliding() || !floorChecker.IsColliding()) return State.Idle;
                break;
            case State.Hurt:
                calmDownTimer.Start();
                if (!animationPlayer.IsPlaying()) return State.Running;
                break;
        }
        return currentState;
    }

    public void TickPhysics(State currentState, double delta)
    {
        Velocity = HandleVelocity(currentState, Velocity, delta);
        MoveAndSlide();
    }

    public void TransitionState(State fromState, State toState)
    {
        var frame = Engine.GetPhysicsFrames();
        Print($"[{frame}] Boar -> from: {fromState} to: {toState}");
        switch (toState)
        {
            case State.Idle:
                animationPlayer.Play("idle");
                if (wallChecker.IsColliding()) Direction = (Direction)((int)Direction * -1);
                break;
            case State.Running:
                animationPlayer.Play("run");
                break;
            case State.Walk:
                animationPlayer.Play("walk");
                if (!floorChecker.IsColliding()) Direction = (Direction)((int)Direction * -1);
                break;
            case State.Hurt:
                animationPlayer.Play("hit");
                break;
            case State.Dying:
                animationPlayer.Play("die");
                break;
        }
    }

    private Vector2 HandleVelocity(State currentState, Vector2 velocity, double delta)
    {
        switch (currentState)
        {
            case State.Idle:
                return move(0, delta);
            case State.Running:
                if (wallChecker.IsColliding() || !floorChecker.IsColliding()) Direction = (Direction)((int)Direction * -1);
                if (playerChecker.GetCollider() is Player) calmDownTimer.Start();
                return move(MaxSpeed, delta);
            case State.Walk:
                return move(MaxSpeed / 3, delta);
            case State.Hurt:
                Player player = (Player)states.DamageSource;
                Direction = (Direction)(player.graphics.Scale.X * -1);
                velocity.X = player.graphics.Scale.X * KockBack;
                break;
            case State.Dying:
                if (!animationPlayer.IsPlaying()) QueueFree();
                break;

        }
        velocity.Y += defaultGravity * (float)delta;
        return velocity;
    }
    Vector2 move(double speed, double delta)
    {
        var velocity = new Vector2();
        velocity.X = (float)MoveToward((double)Velocity.X, speed * (float)Direction, Acceleration * delta);
        velocity.Y = defaultGravity * (float)delta;
        return velocity;
    }
}