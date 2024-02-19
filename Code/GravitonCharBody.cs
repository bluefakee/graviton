using Godot;

/// <summary>Note: Calls MoveAndSlide on end of _PhysicsProcess</summary>
public partial class GravitonCharBody : CharacterBody2D
{
    public Transform2D ToFloorSpaceTrs => new(UpDirection.Rot90CCW(), UpDirection, Vector2.Zero);


    [Export] public float GravScale = 1f;


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        Velocity -= UpDirection * GravScale * ProjectSettings.GetSetting("physics/2d/default_gravity").As<float>() * (float) delta;
        // TODO: Bounce of crates that land on the players head
        MoveAndSlide();
    }


    public void _PickedUp()
    {
        SetPhysicsProcess(false);
    }

    public void _Dropped(Vector2 throwVelocity)
    {
        SetPhysicsProcess(true);
        Velocity = throwVelocity;
    }
}