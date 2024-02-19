using Godot;

public partial class FrictionGravitonCharBody : GravitonCharBody
{
    [Export] public float FloorFriction;


    public override void _PhysicsProcess(double delta)
    {
        if (IsOnFloor()) Velocity *= 1 - FloorFriction;
        base._PhysicsProcess(delta);
    }
}