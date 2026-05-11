using Godot;
using System;

public partial class UnitSpawner : Node
{
	FlowfieldNode parentNode;
	[Export] float spawnInterval = 2;
	[Export] bool enabled = true;
	[Export] PackedScene spawnedUnits;

	[Export] int maxSpawnedUnitsCount = 1;
    int spawnedUnitCount = 0;
	bool readyToSpawn;
    Timer spawnTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		parentNode = (FlowfieldNode)GetParent();

		if (enabled)
		{
			spawnTimer = new Timer();
			this.AddChild(spawnTimer);
			spawnTimer.OneShot = true;
			spawnTimer.Timeout += () => { readyToSpawn = true; };
			spawnTimer.Start(spawnInterval);
		}
	}

	public void SpawnUnit()
    {
        if (!parentNode.IsFree) { return; }

        readyToSpawn = false;
        spawnedUnitCount++;
		
        Unit newUnit = spawnedUnits.Instantiate<Unit>();
		this.AddChild(newUnit);
        newUnit.Init(parentNode);
        newUnit.GlobalPosition = parentNode.GlobalPosition;
		if (spawnedUnitCount < maxSpawnedUnitsCount)
        {
            spawnTimer.Start(spawnInterval);
        }				
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (readyToSpawn) { SpawnUnit(); }
	}
}
