using Godot;

public partial class RespawnTrigger : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body) => (body as IRespawnable)?.Respawn();
}