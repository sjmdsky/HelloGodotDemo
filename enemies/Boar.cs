using Godot;
using System;

public partial class Boar : Enemy
{
    RayCast2D wallChecker;
    RayCast2D floorChecker;
    RayCast2D playerChecker;
    public override void _Ready()
    {
        wallChecker = GetNode<RayCast2D>("Graphics/WallChecker");
        floorChecker = GetNode<RayCast2D>("Graphics/FloorChecker");
        playerChecker = GetNode<RayCast2D>("Graphics/PlayerChecker");
    }
}
