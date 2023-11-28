using Godot;
using static Godot.GD;
using static Godot.Mathf;
public partial class Boar : Enemy, IStateMachine
{
    RayCast2D wallChecker;
    RayCast2D floorChecker;
    RayCast2D playerChecker;
    Timer calmDownTimer;

    public override void _Ready()
    {
        base._Ready();
        wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
        calmDownTimer = GetNode<Timer>("CalmDownTimer");
    }
    public State GetNextState(State currentState)
    {
        if (playerChecker.GetCollider() is Player) return State.Running;
        switch (currentState)
        {
            case State.Idle:
                if (StateMachine.stateTime > 2) return State.Walk;
                break;
            case State.Running:
                if (calmDownTimer.IsStopped()) return State.Walk;
                break;
            case State.Walk:
                if (wallChecker.IsColliding() || !floorChecker.IsColliding()) return State.Idle;
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