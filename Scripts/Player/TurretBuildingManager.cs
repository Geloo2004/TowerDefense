using Godot;
using System;
using System.Collections.Generic;


public partial class TurretBuildingManager : Node
{
    [Export] public Godot.Collections.Array<PackedScene> turretScenes;
    [Export] public Godot.Collections.Array<int> turretPrices;
    public override void _EnterTree()
    {

    }
    public int GetPrice(int selectedTurretID)
    {
        return turretPrices[selectedTurretID];
    }

    public void BuildTurret(TurretSpot turretSpot, int selectedTurretID)
    {
        turretSpot.BuildTurret(turretScenes[selectedTurretID]);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
