using Godot;

public partial class GravitonField : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(Node2D body)
    {
        var charCtrl = body as GravitonCharBody;
        if (charCtrl == null) return;
        charCtrl.IsInGravitonField = true;
    }

    private void OnBodyExited(Node2D body)
    {
        var charCtrl = body as GravitonCharBody;
        if (charCtrl == null) return;
        charCtrl.IsInGravitonField = false;
    }
}