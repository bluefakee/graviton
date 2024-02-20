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
    [Export] public float ThrowForce = 300f;

    public static Player Instance { get; private set; }
    
    private Node _pickedUpColShapeContainer;
    
    private Node2D _pickedUp;
    private CanPickup _pickedUpInfo;


    public Player() : base()
    {
        Instance = this;
    }


    public override void _Ready()
    {
        base._Ready();
        _pickedUpColShapeContainer = new Node();
    }


    public override void _PhysicsProcess(double delta)
    {
        var floorVelo = ToFloorTrs * Velocity;

        // Movement
        floorVelo.X = Mathf.MoveToward(floorVelo.X, Input.GetAxis("move_left", "move_right") * Speed,
            (IsOnFloor() ? FloorAcceleration : AirAcceleration) * (float) delta);

        // Jumping
        if (IsOnFloor() && Input.IsActionPressed("move_up")) floorVelo.Y = JumpForce;
        
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
            var throwVelocity = ThrowForce * (GetViewport().GetMousePosition() - GlobalPosition).Normalized() + Velocity;
            _pickedUp.PropagateCall(Events.DROPPED, new Array(new Variant[] {throwVelocity}));
            
            _pickedUp = null;
            _pickedUpInfo = null;
        }
        
        if (Input.IsActionJustPressed("reset")) RespawnPoint.Current.Respawn();
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

    public void Respawn() => RespawnPoint.Current.Respawn();
}