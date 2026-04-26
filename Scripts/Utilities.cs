using Godot;
using System;
using System.Collections;

public static class Utilities
{
    public static Godot.Collections.Array<Node> FindAllObjectsOfType<T>(Node rootNode) where T : Node
    {
        Godot.Collections.Array<Node> foundObjects = new Godot.Collections.Array<Node>();
        FindAllRecursive<T>(rootNode, foundObjects);
        return foundObjects;
    }

    private static void FindAllRecursive<T>(Node node, Godot.Collections.Array<Node> foundObjects) where T : Node
    {
        if (node is T)
        {
            foundObjects.Add(node);
        }

        foreach (Node child in node.GetChildren())
        {
            FindAllRecursive<T>(child, foundObjects);
        }
    }
    public static Node3D RaycastMousePosition_NoAlloc(Camera3D root, RayCast3D _cachedRayCastNode)
    {
        GD.PrintErr("CHECK UTILITIES RAYCAST");

        Vector2 mousePos = root.GetViewport().GetMousePosition();
        Vector3 origin = root.ProjectRayOrigin(mousePos);
        Vector3 direction = root.ProjectRayNormal(mousePos);

        // Reposition the RayCast3D node
        _cachedRayCastNode.GlobalPosition = origin;
        _cachedRayCastNode.TargetPosition = direction * 1000;
        _cachedRayCastNode.ForceRaycastUpdate();

        if (_cachedRayCastNode.IsColliding())
        {
            return _cachedRayCastNode.GetCollider() as Node3D;
        }
        return null;
    }



    /// <summary>
    /// Gets horizontal rotation (around Y axis) to face a target.
    /// </summary>
    /// <returns>Angle in radians</returns>
    public static Tuple<float, float> GetDirectionToTarget(Vector3 obj, Vector3 target)
    {
        Tuple<float, float> rotation = new Tuple<float, float>(0, 0);
        Vector3 vec = (target - obj).Normalized();
        // yaw = MathF.Atan2(vec.X, vec.Z);
        // pitch = MathF.Atan2(-vec.Y, MathF.Sqrt(vec.X * vec.X + vec.Z * vec.Z));
        return new Tuple<float, float>(GetHorizontalRotation(vec), GetVerticalRotation(vec));
    }

    public static float GetHorizontalRotation(Vector3 obj, Vector3 target)
    {
        return GetHorizontalRotation((target - obj).Normalized());
    }
    public static float GetHorizontalRotation(Vector3 vec)
    {
        return MathF.Atan2(vec.X, vec.Z);
    }

    public static float GetVerticalRotation(Vector3 obj, Vector3 target)
    {
        return GetVerticalRotation((target - obj).Normalized());
    }
    public static float GetVerticalRotation(Vector3 vec)
    {
        return MathF.Atan2(-vec.Y, MathF.Sqrt(vec.X * vec.X + vec.Z * vec.Z));
    }

    public static Godot.Collections.Dictionary GetClickedObject(Camera3D scene, uint collisionMask) 
    {
        Vector2 mousePos = scene.GetViewport().GetMousePosition();

        // Get ray origin and direction
        Vector3 origin = scene.ProjectRayOrigin(mousePos);
        Vector3 direction = scene.ProjectRayNormal(mousePos);
        Vector3 endPos = origin + direction * 1000;

        var spaceState = scene.GetWorld3D().DirectSpaceState;
        var rayQuery = PhysicsRayQueryParameters3D.Create(origin, endPos, collisionMask);
        rayQuery.CollideWithAreas = true;
        rayQuery.CollideWithBodies = true;

        var result = spaceState.IntersectRay(rayQuery);
        return result;
    }

    public static uint GetCollisionMaskValue(uint enabledLayer)
    {
        return (uint)MathF.Pow(enabledLayer,2)-1;
    }
    public static uint GetCollisionMaskValue(BitArray enabledLayers)
    {
        uint value = 0;
        uint powers = 1;
        for (int i = 0; i < enabledLayers.Length; i++)
        {
            if (enabledLayers[i])
            {
                value += powers;
            }
            powers *= 2;
        }
        return value;
    }
}