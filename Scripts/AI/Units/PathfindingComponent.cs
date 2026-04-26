using Godot;
using System;
using System.Threading.Tasks;

public partial class PathfindingComponent : NavigationAgent3D
{
    private Unit parent;

    public override void _Ready()
    {
        //  GetTree().PhysicsFrame += () => { SetTarget(target); };
        parent = (Unit)GetParent();
    }

    public void SetTarget(Vector3 target)
    {
        this.TargetPosition = target;
    }

    public Vector3 GetDirection()
    {
        Vector3 currentPos = parent.GlobalPosition;
        Vector3 nextPos = this.GetNextPathPosition();

        return currentPos.DirectionTo(nextPos);
    }
}
