using Godot;

public partial class Player : GravitonCharBody
{
    [Export] public float Speed = 400f;
    [Export] public float Acceleration = 1700f;
    [Export] public float JumpForce = 450f;


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
}
