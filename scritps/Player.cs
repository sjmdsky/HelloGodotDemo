using Godot;
using static Godot.Mathf;
using static Godot.GD;
using System.Linq;
using System;

public partial class Player : CharacterBody2D, IStateMachine
{
    public static readonly State[] GroundState = [State.Idle, State.Running, State.Landing, State.Attack1, State.Attack2, State.Attack3];
    public const float RunSpeed = 160;
    public const float JumpVelocity = -320;

    float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    Node2D graphics;
    StateMachine stateMachine;
    // Sprite2D sprite2D;
    AnimationPlayer animationPlayer;
    Timer jumpRequestTimer;
    Timer coyoteTimer;

    RayCast2D handChecker;
    RayCast2D footChecker;

    private bool _isJumped = false;
    private bool _isFirstTick = false;
    bool isComboRequested = false;
    [Export]
    bool canCombo = false;
    public override void _Ready()
    {
        // sprite2D = GetNode<Sprite2D>("Sprite2D");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        jumpRequestTimer = GetNode<Timer>("JumpRequestTimer");
        coyoteTimer = GetNode<Timer>("CoyoteTimer");
        graphics = GetNode<Node2D>("Graphics");
        handChecker = GetNode<RayCast2D>("Graphics/HandChecker");
        footChecker = GetNode<RayCast2D>("Graphics/FootChecker");
        stateMachine = GetNode<StateMachine>("StateMachine");
        // velocity = Velocity;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            Print("_UnhandledInput：IsActionPressed -> jump");
            jumpRequestTimer.Start();
        }

        if (@event.IsActionReleased("jump")) jumpRequestTimer.Stop();
        if (@event.IsActionPressed("attack") && canCombo) isComboRequested = true;

    }


    private Vector2 HandleVelocity(State currentState, Vector2 velocity, double delta)
    {
        // Print("HandleVelocity: " + currentState);
        var direction = Input.GetAxis("move_left", "move_right");
        // velocity.X = direction * RunSpeed;
        velocity.X = (float)MoveToward((double)Velocity.X, RunSpeed * direction, 2000 * delta);

        var canJump = IsOnFloor() || coyoteTimer.TimeLeft > 0;
        var shouldJump = canJump && jumpRequestTimer.TimeLeft > 0;

        // if (!IsZeroApprox(direction)) sprite2D.FlipH = direction < 0;
        if (!IsZeroApprox(direction)) graphics.Scale = direction < 0 ? new Vector2(-1, 1) : new Vector2(1, 1);
        switch (currentState)
        {
            case State.Idle:
                break;
            case State.Running:
                break;
            case State.Jump:
                if (shouldJump)
                {
                    Print("Jump -> start");
                    velocity.Y = JumpVelocity;
                    jumpRequestTimer.Stop();
                    _isJumped = true;
                    return velocity;
                }
                break;
            case State.Fall:
                break;
            case State.Landing:
                break;
            case State.WallSliding:
                velocity.Y += (gravity / 5) * (float)delta;
                return velocity;
            case State.WallJump:
                if (IsOnWallOnly() && jumpRequestTimer.TimeLeft > 0)
                {
                    Print("WallJump -> shouldJump");
                    velocity.Y = JumpVelocity;
                    // MoveToward(velocity.X, 1000 * GetWallNormal().X, delta * 1000);
                    velocity.X = RunSpeed * 2 * GetWallNormal().X;
                    jumpRequestTimer.Stop();
                    _isJumped = true;
                    return velocity;
                }
                // else if (stateMachine.stateTime < 0.1)
                else
                {

                    {
                        Print("WallJump -> stateTime");
                        velocity.Y += gravity * (float)delta;
                        velocity.X = 160 * GetWallNormal().X;
                        graphics.Scale = GetWallNormal().X < 0 ? new Vector2(-1, 1) : new Vector2(1, 1);
                        return velocity;
                        // return move(RunSpeed, delta);

                    }
                }
            case State.Attack1 or State.Attack2 or State.Attack3:
                velocity.X = (float)Decimal.Zero;
                break;


        }
        velocity.Y += gravity * (float)delta;
        return velocity;
    }

    public void TransitionState(State fromState, State toState)
    {
        var frame = Engine.GetPhysicsFrames();
        Print($"[{frame}] Player -> from: {fromState} to: {toState}");
        if (!GroundState.Contains(fromState) && !GroundState.Contains(toState)) coyoteTimer.Stop();
        switch (toState)
        {
            case State.Idle:
                animationPlayer.Play("idle");
                break;
            case State.Running:
                animationPlayer.Play("running");
                break;
            case State.Jump:
                animationPlayer.Play("jump");
                break;
            case State.Fall:
                animationPlayer.Play("fall");
                if (GroundState.Contains(fromState)) coyoteTimer.Start();
                break;
            case State.Landing:
                animationPlayer.Play("landing");
                break;
            case State.WallSliding:
                animationPlayer.Play("wall_sliding");
                break;
            case State.WallJump:
                animationPlayer.Play("jump");
                break;
            case State.Attack1:
                animationPlayer.Play("attack_1");
                isComboRequested = false;
                break;
            case State.Attack2:
                animationPlayer.Play("attack_2");
                isComboRequested = false;
                break;
            case State.Attack3:
                animationPlayer.Play("attack_3");
                isComboRequested = false;
                break;

        }
        // Engine.TimeScale = (toState == State.WallJump) ? 0.5 : 1.0;
        _isFirstTick = true;
    }

    public void TickPhysics(State currentState, double delta)
    {
        Velocity = HandleVelocity(currentState, Velocity, delta);
        var wasOnFloor = IsOnFloor();

        MoveAndSlide();
        if (IsOnFloor() ^ wasOnFloor)
        {
            if (wasOnFloor)
            {
                if (!_isJumped)
                {
                    Print("CoyoteTimer -> Start()");
                    coyoteTimer.Start();
                }
            }
            else if (_isJumped)
            {
                _isJumped = false;
                Print("Jump -> completed");
            }
        }
        _isFirstTick = false;
    }

    public State GetNextState(State currentState)
    {
        // Print("GetNextState -> " + currentState);
        var velocity = Velocity;
        var direction = Input.GetAxis("move_left", "move_right");
        var isStill = IsZeroApprox(direction);

        var canJump = IsOnFloor() || coyoteTimer.TimeLeft > 0;
        var shouldJump = canJump && jumpRequestTimer.TimeLeft > 0;
        if (GroundState.Contains(currentState) && !IsOnFloor()) return State.Fall;
        if (shouldJump) return State.Jump;
        // if (shouldJump && (currentState != State.WallSliding) && (currentState != State.WallJump)) return State.Jump;

        switch (currentState)
        {
            case State.Idle:
                // if (!IsOnFloor()) return State.Fall;
                if (Input.IsActionJustPressed("attack")) return State.Attack1;
                if (!isStill) return State.Running;
                // if (shouldJump) return State.Jump;
                break;
            case State.Running:
                // if (!IsOnFloor()) return State.Fall;
                if (Input.IsActionJustPressed("attack")) return State.Attack1;
                if (isStill) currentState = State.Idle;
                // if (shouldJump) currentState = State.Jump;
                break;
            case State.Jump:
                if (velocity.Y >= 0) currentState = State.Fall;
                break;
            case State.Fall:
                if (IsOnFloor()) currentState = isStill ? State.Landing : State.Running;
                if (IsOnWall() && handChecker.IsColliding() && footChecker.IsColliding()) currentState = State.WallSliding;
                break;
            case State.Landing:
                // if (!IsOnFloor()) return State.Fall;
                if (Input.IsActionJustPressed("attack")) return State.Attack1;

                if (!animationPlayer.IsPlaying()) currentState = State.Idle;
                // if (shouldJump) currentState = State.Jump;
                break;
            case State.WallSliding:
                if (jumpRequestTimer.TimeLeft > 0) currentState = State.WallJump;
                if (IsOnFloor() || !IsOnWall()) currentState = State.Idle;
                break;
            case State.WallJump:
                if (IsOnWall() && !_isFirstTick) currentState = State.WallSliding;
                if (velocity.Y >= 0) currentState = State.Fall;
                break;
            case State.Attack1:
                if (!animationPlayer.IsPlaying())
                    return isComboRequested ? State.Attack2 : State.Idle;
                // if (isComboRequested) return State.Attack2; else return State.Idle;
                break;
            case State.Attack2:
                if (!animationPlayer.IsPlaying())
                    return isComboRequested ? State.Attack3 : State.Idle;
                break;
            case State.Attack3:
                if (!animationPlayer.IsPlaying())
                    return State.Idle;
                break;

            default:
                return currentState;
        }
        return currentState;
    }

}
