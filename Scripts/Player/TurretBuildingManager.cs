using Godot;
using System;
using System.Collections.Generic;

public partial class TurretBuildingManager : Node
{
    [Export] public Godot.Collections.Dictionary<int, string> turretsDataPaths;	// id - path
    private Dictionary<int, Resources.CannonData> tData = new Dictionary<int, Resources.CannonData>();
    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {
        foreach (var path in turretsDataPaths)
        {
            var turretsData = ResourceLoader.Load<Resources.CannonData>(path.Value);
            tData[path.Key] = turretsData;
        }


    }
    public int GetPrice(int selectedTurretID)
    {
        return tData[selectedTurretID].Price;
    }

    public void BuildTurret(TurretSpot turretSpot, int selectedTurretID)
    {
        turretSpot.BuildTurret(tData[selectedTurretID].Scene);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
