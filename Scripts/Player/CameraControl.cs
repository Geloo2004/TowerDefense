using Godot;
using System;

public partial class CameraControl : Camera3D
{
    Vector2I resolution;
    [Export] float scrollSpeed = 30f;
    [Export] float maxVertical;
    [Export] float maxHorizontal;
    [Export] Input.MouseModeEnum mouseMode;
    [Export] AudioStreamPlayer3D audioSource;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.MouseMode = mouseMode;
        resolution = GetViewport().GetWindow().Size;
    }

    private void HandlePerspectiveEdgeScrolling(double delta)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        float edgeSize = 20.0f;

        Vector3 moveDirection = Vector3.Zero;

        // Edge detection
        if (mousePos.X <= edgeSize)
            moveDirection.X = -1;  // Left
        else if (mousePos.X >= resolution.X - edgeSize)
            moveDirection.X = 1;   // Right

        if (mousePos.Y <= edgeSize)
            moveDirection.Z = -1;  // Forward
        else if (mousePos.Y >= resolution.Y - edgeSize)
            moveDirection.Z = 1;   // Backward

        if (moveDirection != Vector3.Zero)
        {
            moveDirection = moveDirection.Normalized();

            // Convert local direction to global and move
            Vector3 globalMovement = Transform.Basis * moveDirection;
            globalMovement.Y = 0;  // Lock Y axis to keep same height
            Position += globalMovement * scrollSpeed * (float)delta;
        }
    }
    private void HandleOrthographicEdgeScrolling(double delta)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();
        float edgeSize = 20.0f;

        Vector3 moveDirection = Vector3.Zero;

        // Edge detection
        if (mousePos.X <= edgeSize)
            moveDirection.X = -1;  // Left
        else if (mousePos.X >= resolution.X - edgeSize)
            moveDirection.X = 1;   // Right

        if (mousePos.Y <= edgeSize)
            moveDirection.Y = 1;  // Up (forward in local space)
        else if (mousePos.Y >= resolution.Y - edgeSize)
            moveDirection.Y = -1;   // Down (backward in local space)

        if (moveDirection != Vector3.Zero)
        {
            moveDirection = moveDirection.Normalized();

            // Convert local direction to global and move
            Vector3 globalMovement = Transform.Basis * moveDirection;
            Position += globalMovement * scrollSpeed * (float)delta;
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if(Input.MouseMode != Input.MouseModeEnum.Confined) { return; }
        if (this.Projection == ProjectionType.Orthogonal)
        {
            HandleOrthographicEdgeScrolling(delta);
        }
        else if (this.Projection == ProjectionType.Perspective)
        {
            HandlePerspectiveEdgeScrolling(delta);
        }
    }
    public Node GetObjectUnderMouse(uint collisionMask)
    {
        Vector2 mousePos = GetViewport().GetMousePosition();

        // Get ray origin and direction
        Vector3 origin = ProjectRayOrigin(mousePos);
        Vector3 direction = ProjectRayNormal(mousePos);
        Vector3 endPos = origin + direction * 1000;

        var spaceState = GetWorld3D().DirectSpaceState;
        var rayQuery = PhysicsRayQueryParameters3D.Create(origin, endPos, Utilities.GetCollisionMaskValue(collisionMask));
        rayQuery.CollideWithAreas = true;
        rayQuery.CollideWithBodies = true;

        var result = spaceState.IntersectRay(rayQuery);

        if (result.Count > 0)
        {
            // Correct way to get the collider in Godot 4
            Variant colliderVariant = result["collider"];
            Node3D hitNode = colliderVariant.As<Node3D>();

            if (hitNode != null)
            {
                Vector3 hitPoint = result["position"].As<Vector3>();

                var parent = hitNode.GetParent();
                return parent;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
