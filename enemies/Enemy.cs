using Godot;
using static Godot.GD;
public enum Direction
{
    Left = -1,
    Right = 1
}


[GlobalClass]
public partial class Enemy : CharacterBody2D
{

    public Node2D graphics;
    protected AnimationPlayer animationPlayer;
    protected StateMachine StateMachine;
    [Export]
    public double MaxSpeed = 180;
    [Export]
    public double Acceleration = 2000;
    [Export]
    private Direction _direction = Direction.Left;
    public Direction Direction
    {
        set
        {
            _direction = value;
            // Print($"_direction -> {_direction}");
            // if (_direction != 0)
            // if (IsNodeReady())
            graphics.Scale = _direction == Direction.Right ? new Vector2(-1, 1) : new Vector2(1, 1);
        }
        get { return _direction; }
    }
    protected float defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    async public override void _Ready()
    {
        await ToSignal(Owner, "ready");
        Print($"Enemy {Name} is ready");
        graphics = GetNode<Node2D>("Graphics");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        StateMachine = GetNode<StateMachine>("StateMachine");
        // Direction = Direction.Left;
    }
}
