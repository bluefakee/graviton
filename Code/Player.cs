using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public partial class Player : GravitonCharBody, IRespawnable
{
    [Export] public float Speed = 200f;
    [Export] public float FloorAcceleration = 1500f;
    [Export] public float AirAcceleration = 1500f;
    [Export] public float JumpForce = 325f;
    [Export] public Area2D PickupArea;
    [Export] public RemoteTransform2D PickedUpPivot;
    [Export] public Vector2 ThrowForce = new(450f, 20f);

    public static Player Instance { get; private set; }
    
    private List<CollisionShape2D> _pickedUpColShapes = new();
    
    private Node2D _pickedUp;
    private CanPickup _pickedUpInfo;
    private float _lastMoveDir = 1f;
    

    public Player() : base()
    {
        Instance = this;
    }


    public override void _PhysicsProcess(double delta)
    {
        var floorVelo = ToFloorTrs * Velocity;

        // Movement
        var horAxis = Input.GetAxis("move_left", "move_right");
        floorVelo.X = Mathf.MoveToward(floorVelo.X, horAxis * Speed,
            (IsOnFloor() ? FloorAcceleration : AirAcceleration) * (float) delta);

        // Jumping
        if (IsOnFloor() && Input.IsActionPressed("move_up")) floorVelo.Y = JumpForce;

        if (horAxis != 0) _lastMoveDir = horAxis;
        
        Velocity = floorVelo * ToFloorTrs;
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

            if (_pickedUp != null)
            {
                PickedUpPivot.AssignPathTo(_pickedUp);
                DisablePickedUpCollision();
                _pickedUp.PropagateCall(Events.PICKED_UP);
            }
        }
        
        if (Input.IsActionJustReleased("hold_crate") && _pickedUp != null)
        {
            PickedUpPivot.RemotePath = new NodePath("");
            EnablePickedUpCollision();
            var throwVelocity = _lastMoveDir > 0 ?
                ToFloorTrs * ThrowForce :
                ToFloorTrs * new Vector2(-ThrowForce.X, ThrowForce.Y);
            _pickedUp.PropagateCall(Events.DROPPED, new Array(new Variant[] {throwVelocity}));
            
            _pickedUp = null;
            _pickedUpInfo = null;
        }
        
        if (Input.IsActionJustPressed("reset")) RespawnPoint.Current.Respawn();
    }


    private void DisablePickedUpCollision()
    {
        foreach (var colShape in _pickedUp.GetChildren<CollisionShape2D>())
        {
            colShape.Reparent(this);
            _pickedUpColShapes.Add(colShape);
        }
    }


    private void EnablePickedUpCollision()
    {
        foreach (var colShape in _pickedUpColShapes)
            colShape.Reparent(_pickedUp);
        _pickedUpColShapes.Clear();
    }

    public void Respawn() => RespawnPoint.Current.Respawn();
}