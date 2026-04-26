using Godot;
using System;

public partial class AimingComponent : Node
{
	[Export] Vector3.Axis axis;
	[Export] private Node3D rotatedObject;
	[Export] private Node3D targetObject;
	[Export] int minRotationAngle;
	[Export] int maxRotationAngle;
	[Export] float rotationSpeed;
	[Export] float aimingMargin;
	private bool _isAimed;
	public bool IsAimed 
	{
		get { return _isAimed; }
		private set { _isAimed = value; } 
	}

	public void SetTarget(Node3D newTarget)
	{
		this.targetObject = newTarget;
	}

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        if(targetObject == null) { return; }
        float rotationAngle;
        if (axis == Vector3.Axis.X)
        {
            rotationAngle = Utilities.GetVerticalRotation(rotatedObject.GlobalPosition, targetObject.GlobalPosition);
            SoftRotate_Ver(rotationAngle, delta);
            IsAimed = Mathf.Abs(Mathf.AngleDifference(rotatedObject.Rotation.X, rotationAngle)) < aimingMargin;
        }
        else if (axis == Vector3.Axis.Y)
        {
            rotationAngle = Utilities.GetHorizontalRotation(rotatedObject.GlobalPosition, targetObject.GlobalPosition);
            SoftRotate_Hor(rotationAngle, delta);
            IsAimed = Mathf.Abs(Mathf.AngleDifference(rotatedObject.Rotation.Y, rotationAngle)) < aimingMargin;
        }
    }

    public void SoftRotate_Ver(float targetVerRot, double delta)
    {

        // --- BARREL (X axis) ---
        float currentX = rotatedObject.Rotation.X;

        float clampedTargetX = Mathf.Clamp(
            targetVerRot,
            Mathf.DegToRad(minRotationAngle),
            Mathf.DegToRad(maxRotationAngle)
        );

        float newX = Mathf.LerpAngle(
            currentX,
            clampedTargetX,
            rotationSpeed * (float)delta
        );

        rotatedObject.Rotation = new Vector3(newX, 0, 0);
    }
    public void SoftRotate_Hor(float targetHorRot, double delta)
    {
        // --- BASE (Y axis) ---
        float currentY = rotatedObject.Rotation.Y;

        float clampedTargetY = Mathf.Clamp(
            targetHorRot,
            Mathf.DegToRad(minRotationAngle),
            Mathf.DegToRad(maxRotationAngle)
        );

        float newY = Mathf.LerpAngle(
            currentY,
            clampedTargetY,
            rotationSpeed * (float)delta
        );

        rotatedObject.Rotation = new Vector3(0, newY, 0);
    }

}
