using Godot;
using System;
using System.Collections.Generic;

public partial class DetectionManager:Node
{
    [Export] GridMap detectableLand;
    Dictionary<Vector2I, List<Unit>> detectedPositions;

    public override void _EnterTree()
    {

    }
    /*
    private bool HasCellsAbove(ref Vector3I coordinates, ref Godot.Collections.Array<Vector3I> cells)
    {
        GD.Print((int)CellSize.Y);

        for (int i = 1; i < 10; i++)
        {
            if (cells.Contains(new Vector3I(coordinates.X, coordinates.Y + i, coordinates.Z)))
            {
                return true;
            }
        }
        return false;
    }*/
}
