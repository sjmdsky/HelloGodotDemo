using Godot;
using System;
using static System.Decimal;
using static Godot.GD;
using static Godot.Mathf;

[GlobalClass]
public partial class States : Node
{

    [Signal]
    public delegate void HealthChangedEventHandler();
    [Export]
    public int MaxHealth = 3;
    [Export]
    private int _health;

    public Node DamageSource;
    public bool IsOnHit = false;
    public int Health
    {
        set
        {
            _health = Clamp(value, (int)Zero, MaxHealth);
            EmitSignal(SignalName.HealthChanged);
        }
        get => _health;
    }
    public override void _Ready()
    {
        base._Ready();
        Health = MaxHealth;
    }

    public int OnHit(int damage, Node damageSource)
    {
        DamageSource = damageSource;
        if (Owner is Player)
        {
            Player player = Owner as Player;
            if (player.InvincibleTimer.TimeLeft > 0)
            {

                Print($"[{Engine.GetPhysicsFrames()}][{Owner.Name}] Invincible time");
                return Health;

            }
        }
        if (Health > 0)
        {
            Health -= damage;
            Print($"[{Engine.GetPhysicsFrames()}][{Owner.Name}] Hurt -> {damage}");
            Print($"[{Engine.GetPhysicsFrames()}][{Owner.Name}] Health -> {Health}");
            if (Health <= 0) OnDie();
            IsOnHit = true;
        }
        return Health;
    }
    public void OnDie()
    {
        Print($"[{Engine.GetPhysicsFrames()}][{Owner.Name}] Dead");
        // Owner.QueueFree();
    }
}
