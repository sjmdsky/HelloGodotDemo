using Godot;
using static Godot.GD;
using System;

public partial class StatusPanel : HBoxContainer
{
    [Export]
    States states;
    TextureProgressBar HealthBar;
    public override void _Ready()
    {
        base._Ready();
        HealthBar = GetNode<TextureProgressBar>("HealthBar");
        states.HealthChanged += _updateHealth;
        _updateHealth();
    }
    private void _updateHealth()
    {
        var percentage = (float)states.Health / states.MaxHealth;
        HealthBar.Value = percentage;
    }
}
