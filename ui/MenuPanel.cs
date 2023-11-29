using Godot;
using System;

public partial class MenuPanel : HBoxContainer
{
    Button Button;
    public override void _Ready()
    {
        base._Ready();
        Button = GetNode<Button>("Button");
        Button.Pressed += _restart;
    }

    private void _restart()
    {
        GetTree().ReloadCurrentScene();
    }

}
