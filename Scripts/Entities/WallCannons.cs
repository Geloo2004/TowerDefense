using Godot;
using System;
using System.Collections.Generic;

public partial class WallCannons : Node
{
	[Export] Area3D DetectionArea;
	List<Unit> unitsInRange = new List<Unit>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		DetectionArea.AreaEntered += (Area3D collision) => 
		{
			GD.Print("TARGET ACQUIRED");
		};

        DetectionArea.AreaExited += (Area3D collision) =>
        {
            GD.Print("TARGET DEQUIRED");

        };
    }

	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
