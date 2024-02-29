using System.Collections.Generic;
using Godot;

public partial class CrateSpawner : Node2D
{
    public static IEnumerable<CrateSpawner> AllSpawners => _allSpawners;
    private static List<CrateSpawner> _allSpawners = new();


    public override void _EnterTree()
    {
        base._Ready();
        _allSpawners.Add(this);
        Respawn();
    }


    public override void _ExitTree()
    {
        base._ExitTree();
        _allSpawners.Remove(this);
    }


    public void Respawn()
    {
        var crate = GetChild<GravitonCharBody>(0);
        crate.Position = Vector2.Zero;
        crate.SetUpDir(-Transform.Y);
    }
}