using Godot;

public partial class World : Node2D
{
	TileMap tileMap;
	Camera2D camera2D;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tileMap = GetNode<TileMap>("TileMap");
		camera2D = GetNode<Camera2D>("Player/Camera2D");
		var used = tileMap.GetUsedRect().Grow(-1);
		var tileSize = tileMap.TileSet.TileSize;
		camera2D.LimitTop = used.Position.Y * tileSize.Y;
		camera2D.LimitRight = used.End.X * tileSize.X;
		camera2D.LimitBottom = used.End.Y * tileSize.Y;
		camera2D.LimitLeft = used.Position.X * tileSize.X;
		camera2D.ResetSmoothing();
	}
}
