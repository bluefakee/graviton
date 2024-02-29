using Godot;

public partial class RespawnPoint : Area2D
{
    [Export] public Vector2 UpDirection = Vector2.Up;

    public static RespawnPoint Current { get; private set; }


    public RespawnPoint() : base()
    {
        Current = this;
    }


    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
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
        
        foreach (var crateSpawner in CrateSpawner.AllSpawners) crateSpawner.Respawn();
    }
}