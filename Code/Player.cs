using System.Linq;
using Godot;

public partial class Player : GravitonCharBody
{
    [Export] public float Speed = 400f;
    [Export] public float Acceleration = 1700f;
    [Export] public float JumpForce = 450f;
    [Export] public Area2D PickupArea;
    [Export] public RemoteTransform2D PickedUpPivot;

    private Node _pickedUpColShapeContainer;
    
    private Node2D _pickedUp;


    public override void _Ready()
    {
        base._Ready();
        _pickedUpColShapeContainer = new Node();
    }


    public override void _PhysicsProcess(double delta)
    {
        var floorVelo = ToFloorSpaceTrs * Velocity;

        // Movement
        floorVelo.X = Mathf.MoveToward(floorVelo.X, Input.GetAxis("move_left", "move_right") * Speed,
            Acceleration * (float) delta);

        // Jumping
        if (IsOnFloor() && Input.IsActionPressed("move_up")) floorVelo.Y = JumpForce;
        
        Velocity = floorVelo * ToFloorSpaceTrs;
        base._PhysicsProcess(delta);
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
        
        if (Input.IsActionJustPressed("hold_crate") && PickupArea.HasOverlappingBodies())
        {
            (_pickedUp, CanPickup pickedUpInfo) = PickupArea.GetOverlappingBodies()
                .Select(x => (x, x.GetChild<CanPickup>()))
                .Where(x => x.Item2 != null)
                .MinBy(x => (x.x.GlobalPosition - GlobalPosition).LengthSquared());
            
            if (_pickedUp == null) return;

            PickedUpPivot.AssignPathTo(_pickedUp);
            DisablePickedUpCollision();
        }
    }


    private void DisablePickedUpCollision()
    {
        foreach (var colShape in _pickedUp.GetChildren<CollisionShape2D>())
            colShape.Reparent(_pickedUpColShapeContainer);
    }
}