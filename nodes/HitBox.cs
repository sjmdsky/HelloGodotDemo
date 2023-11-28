using Godot;
using static Godot.GD;
using System;
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
        Print($"[{Engine.GetPhysicsFrames()}][Hit] {Owner.Name} -> {body.Name} {Owner is Enemy}");

        // if (body is PhysicsBody2D)
        // {
        //     Print($"[{Engine.GetPhysicsFrames()}][Hit] {Owner.Name} -> {body.Name} {Owner is Enemy}");
        // }

    }


}
