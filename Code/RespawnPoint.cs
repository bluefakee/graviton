using Godot;

public partial class RespawnPoint : Area2D
{
    [Export] public Vector2 UpDirection = Vector2.Up;
    [Export]  public bool IsBeginner;

    public static RespawnPoint Current { get; private set; }


    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
        if (IsBeginner)
        {
            if (Current != null) GD.PrintErr("2 RespawnPoints are Beginner!");
            Current = this;
        }
    }

    private void OnBodyEntered(Node2D body)
    {
        var player = body as Player;
        if (player == null) return;
        Current = this;
    }


    public void Respawn()
    {
        Player.Instance.GlobalPosition = GlobalPosition;
        Player.Instance.SetUpDir(UpDirection);
        Player.Instance.Velocity = Vector2.Zero;
    }
}