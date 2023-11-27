using Godot;
using System;

public enum Direction
{
    Left = -1,
    Right = 1
}


[GlobalClass]
public partial class Enemy : CharacterBody2D
{

    Node2D graphics;
    AnimationPlayer animationPlayer;
    StateMachine stateMachine;
    [Export]
    public float MaxSpeed = 180;
    [Export]
    public float Acceleration = 2000;

    [Export]
    public Direction Direction
    {
        set
        {
            Direction = value;
            graphics.Scale = Direction == Direction.Right ? new Vector2(-1, 1) : new Vector2(1, 1);
        }
        get => Direction;
    }
    float defaultGravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        graphics = GetNode<Node2D>("Graphics");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        stateMachine = GetNode<StateMachine>("StateMachine");
    }
}
