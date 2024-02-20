using Godot;

public partial class CameraRotator : Camera2D
{
    public override void _Ready()
    {
        base._Ready();
        Player.Instance.OnUpDirChange += PlayerOnUpDirChange;
    }

    private void PlayerOnUpDirChange(GravitonCharBody obj)
    {
        var playerTrs = obj.Transform;
        Transform = new Transform2D(playerTrs.X, playerTrs.Y, Vector2.Zero);
    }
}
