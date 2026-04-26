using Godot;
using System;

public partial class BuildingOutline : Node3D
{
    [Export] Material good;
    [Export] Material bad;
    [Export] MeshInstance3D model;
    public void SetColor(bool canBeBuilt)
    {
        if (canBeBuilt)
        {
            model.SetSurfaceOverrideMaterial(0, good);
        }
        else
        {
            model.SetSurfaceOverrideMaterial(0, bad);
        }
    }
}
