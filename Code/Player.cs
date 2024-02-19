using System.Linq;
using Godot;
using Godot.Collections;

public partial class Player : GravitonCharBody
{
    [Export] public float Speed = 400f;
    [Export] public float Acceleration = 1700f;
    [Export] public float JumpForce = 450f;
    [Export] public Area2D PickupArea;
    [Export] public RemoteTransform2D PickedUpPivot;
    [Export] public float ThrowForce = 300f;

    private Node _pickedUpColShapeContainer;
    
    private Node2D _pickedUp;
    private CanPickup _pickedUpInfo;


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
        
        if (Input.IsActionJustPressed("hold_crate") ||
            Input.IsActionPressed("hold_crate") && _pickedUp == null)
        {
            (_pickedUp, _pickedUpInfo) = PickupArea.GetOverlappingBodies()
                .Select(x => (x, x.GetChild<CanPickup>()))
                .Where(x => x.Item2 != null)
                .OrderBy(x => (x.x.GlobalPosition - GlobalPosition).LengthSquared())
                .FirstOrDefault();
            
            if (_pickedUp == null) return;

            PickedUpPivot.AssignPathTo(_pickedUp);
            DisablePickedUpCollision();
            _pickedUp.PropagateCall(Events.PICKED_UP);
        }
        
        else if (Input.IsActionJustReleased("hold_crate") && _pickedUp != null)
        {
            PickedUpPivot.RemotePath = new NodePath("");
            EnablePickedUpCollision();
            var throwVelocity = ThrowForce * (GetViewport().GetMousePosition() - GlobalPosition).Normalized() + Velocity;
            _pickedUp.PropagateCall(Events.DROPPED, new Array(new Variant[] {throwVelocity}));
            
            _pickedUp = null;
            _pickedUpInfo = null;
        }
    }


    private void DisablePickedUpCollision()
    {
        foreach (var colShape in _pickedUp.GetChildren<CollisionShape2D>())
            colShape.Reparent(_pickedUpColShapeContainer, false);
    }


    private void EnablePickedUpCollision()
    {
        foreach (var colShape in _pickedUpColShapeContainer.GetChildren().Select(x => (CollisionShape2D) x))
            colShape.Reparent(_pickedUp, false);
    }
}