using Godot;

/// <summary>Note: Calls MoveAndSlide on end of _PhysicsProcess</summary>
public partial class GravitonCharBody : CharacterBody2D
{
    public Transform2D ToFloorTrs => new(UpDirection.Rot90CCW(), UpDirection, Vector2.Zero);


    [Export] public float GravScale = 1f;
    public bool IsInGravitonField;


    public bool IsOnFloorJust() => !_prevIsOnFloor && IsOnFloor();
    public bool IsOnWallJust() => !_prevIsOnWall && IsOnWall();
    private bool _prevIsOnFloor;
    private bool _prevIsOnWall;
    private Vector2 _prevVelocity;
    

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!IsOnFloor()) Velocity -= UpDirection * GravScale * ProjectSettings.GetSetting("physics/2d/default_gravity").As<float>() * (float) delta;

        if (IsInGravitonField && IsOnFloor() && IsOnWallJust())
        {
            SetUpDirKeepMomentum(GetWallNormal());
        }
        
        // TODO: Bounce of crates that land on the players head
        _prevVelocity = Velocity;
        _prevIsOnFloor = IsOnFloor();
        _prevIsOnWall = IsOnWall();
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


    public void SetUpDirKeepMomentum(Vector2 newUp)
    {
        // _prevVelocity is used because Velocity can be reduced due to running into a wall
        var floorVelo = ToFloorTrs * _prevVelocity;
        UpDirection = newUp;
        Velocity = floorVelo * ToFloorTrs;
        Transform = new Transform2D(UpDirection.Rot90CCW(), -UpDirection, Position);
    }
}