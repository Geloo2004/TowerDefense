using Godot;
using System;

public partial class Projectile : Area3D
{
	[Export] public string DataPath { get; set; } // Path to the .tres file


	// Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
