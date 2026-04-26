using Godot;
using System;
using System.Collections.Generic;

public partial class TurretSpot : Node3D
{
	[Export] public Godot.Collections.Array<uint> allowTurrets;
	public override void _Ready()
	{
	}

	public void SetTurret()
	{

	}

	public override void _Process(double delta)
	{
	}
}
