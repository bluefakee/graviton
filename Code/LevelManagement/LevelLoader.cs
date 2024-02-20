using Godot;

public partial class LevelLoader : Node
{
    public const string LEVELS_PREFIX = "res://Scenes/Level/level_";
    public const string LEVELS_POSTFIX = ".tscn";
    
    
    public static LevelLoader Instance { get; private set; }
    private int _curLvl = 1;


    public override void _Ready()
    {
        base._Ready();
        Instance = this;
        RespawnPoint.Current.Respawn();
    }


    public void NextLevel()
    {
        GetChild(0).QueueFree();
        var newLevel = GD.Load<PackedScene>($"{LEVELS_PREFIX}{++_curLvl}{LEVELS_POSTFIX}").Instantiate();
        CallDeferred(Node.MethodName.AddChild, newLevel);
        RespawnPoint.Current.Respawn();
    }


#if DEBUG
    private bool _prevPressed;
    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsKeyPressed(Key.P) && !_prevPressed) NextLevel();
        _prevPressed = Input.IsKeyPressed(Key.P);
    }
#endif
}