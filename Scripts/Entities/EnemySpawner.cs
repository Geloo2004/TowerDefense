using Godot;
using System;

public partial class EnemySpawner : Node3D
{
	[Export] bool enabled = false;
	[Export] PackedScene enemy;
	[Export] float time;

    [Export] Path3D path;
	
	Timer timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timer = new Timer();
		this.AddChild(timer);
		timer.Autostart = true;
		timer.Timeout += () => { SpawnEnemy(); };
		timer.Start(time);
    }

	public void SpawnEnemy()
	{
		if (!enabled) { return; }
		GD.Print("Spawned");
		var newEnemy = enemy.Instantiate<Unit>();
        path.AddChild(newEnemy);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
