using Godot;
using System;
using System.Reflection;

public partial class UnitSpawner : Node
{
	static Random random;
	FlowfieldNode parentNode;
	[Export] float spawnInterval = 2;
    [Export] float spawnIntervalOffset = 1;
    [Export] bool enabled = true;

	[Export] Godot.Collections.Array<PackedScene> startSpawnedUnits;
	private PackedScene[] spawnedUnits;

    int pointer = 0;

	[Export] bool isInfinite = false;

	[Export] int maxSpawnedUnitsCount = 1;
    int spawnedUnitCount = 0;
	bool readyToSpawn;
    Timer spawnTimer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (random == null) 
		{
			random = new Random();
		}

		parentNode = (FlowfieldNode)GetParent();

		if (enabled)
		{
			spawnTimer = new Timer();
			this.AddChild(spawnTimer);
			spawnTimer.OneShot = true;
			spawnTimer.Timeout += () => { readyToSpawn = true; };
			spawnTimer.Start(spawnInterval + ( 1 - random.NextDouble()) * spawnIntervalOffset*2 );
		}
	}

	public void SpawnUnit()
    {
        if (!parentNode.IsFree) { return; }

        readyToSpawn = false;
        spawnedUnitCount++; 
		pointer++;

        Unit newUnit = spawnedUnits[pointer].Instantiate<Unit>();
		this.AddChild(newUnit);
        newUnit.Init(parentNode);
        newUnit.GlobalPosition = parentNode.GlobalPosition;
		if (spawnedUnitCount < maxSpawnedUnitsCount)
        {
            spawnTimer.Start(spawnInterval + (1 - random.NextDouble()) * spawnIntervalOffset * 2);
        }				
	}

	public void UpdateSpawnerSettings(PackedScene[] newUnits, float interval, float intervalRandom, bool isInfinite) { }

	public void SetSpawnedUnits(PackedScene[] newUnits)
	{
		spawnedUnits = newUnits;

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (readyToSpawn) { SpawnUnit(); }
	}
}
