using Godot;
using System;

[GlobalClass]
public partial class States : Node
{
    [Export]
    public int MaxHealth = 3;
    private int _health;
    public override void _Ready()
    {
        base._Ready();
        _health = MaxHealth;
    }

    public int OnHit(int hurt)
    {
        _health -= hurt;
        return _health;
    }
}
