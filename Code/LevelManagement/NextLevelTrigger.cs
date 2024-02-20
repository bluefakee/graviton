using Godot;

public partial class NextLevelTrigger : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node2D body)
    {
        if (body is not Player) return;
        LevelLoader.Instance.NextLevel();
    }
}