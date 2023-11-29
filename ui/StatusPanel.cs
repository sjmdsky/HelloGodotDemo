using Godot;

public partial class StatusPanel : HBoxContainer
{
    [Export]
    States states;
    [Export]
    public bool IsShowAvatar = true;
    TextureProgressBar HealthBar;
    TextureProgressBar EasedHealthBar;
    PanelContainer AvatarBox;
    public override void _Ready()
    {
        base._Ready();
        HealthBar = GetNode<TextureProgressBar>("HealthBar");
        EasedHealthBar = GetNode<TextureProgressBar>("HealthBar/EasedHealthBar");
        AvatarBox = GetNode<PanelContainer>("AvatarBox");
        AvatarBox.Visible = IsShowAvatar;
        states.HealthChanged += _updateHealth;
        _updateHealth();
    }
    private void _updateHealth()
    {
        var percentage = (float)states.Health / states.MaxHealth;
        HealthBar.Value = percentage;
        CreateTween().TweenProperty(EasedHealthBar, "value", percentage, 0.5);
    }
}
