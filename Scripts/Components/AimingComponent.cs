using Godot;
using System;

public partial class AimingComponent : Node
{
	[Export] Vector3.Axis axis;
    Vector3 targetPosition;

	[Export] private Node3D rotatedObject;
	[Export] private Node3D targetObject;
	[Export] int minRotationAngle;
	[Export] int maxRotationAngle;
	[Export] float rotationSpeed;
	[Export] float aimingMargin;

	public bool isAimed { get; private set; }

	public void SetTarget(Node3D newTarget)
	{
		this.targetObject = newTarget;
        isAimed = true;
	}

    public void TrackTarget()
    {
        if (!GodotObject.IsInstanceValid(targetObject)) { return; }
        // aims exactly at the target. No soft aim
        float rotationAngle;
        if (axis == Vector3.Axis.X)
        {
            rotationAngle = Utilities.GetVerticalRotation(rotatedObject.GlobalPosition, targetObject.GlobalPosition);
            rotatedObject.Rotation = new Vector3(rotationAngle,0,0);
        }
        else if (axis == Vector3.Axis.Y)
        {
            rotationAngle = Utilities.GetHorizontalRotation(rotatedObject.GlobalPosition, targetObject.GlobalPosition);
            rotatedObject.Rotation = new Vector3(0, rotationAngle, 0);
        }
    }

	public override void _Process(double delta)
    {
        return;

        if(targetObject == null) { return; }
        else
        { 
            targetPosition = targetObject.GlobalPosition; 
        }

        if (!isAimed)
        {

            float rotationAngle;
            if (axis == Vector3.Axis.X)
            {
                rotationAngle = Utilities.GetVerticalRotation(rotatedObject.GlobalPosition, targetPosition);
                SoftRotate_Ver(rotationAngle, delta);
                isAimed = Mathf.Abs(Mathf.AngleDifference(rotatedObject.Rotation.X, rotationAngle)) < aimingMargin;
            }
            else if (axis == Vector3.Axis.Y)
            {
                rotationAngle = Utilities.GetHorizontalRotation(rotatedObject.GlobalPosition, targetPosition);
                SoftRotate_Hor(rotationAngle, delta);
                isAimed = Mathf.Abs(Mathf.AngleDifference(rotatedObject.Rotation.Y, rotationAngle)) < aimingMargin;
            }
        }
        else
        {
            TrackTarget();
        }
    }

    public void AimToTarget(double delta) 
    {
        TrackTarget();
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
