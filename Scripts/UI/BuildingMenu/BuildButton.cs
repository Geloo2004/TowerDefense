using Godot;
using System;

public partial class BuildButton : Button
{
	[Export] uint buildingId;
	Player player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var players = Utilities.FindAllObjectsOfType<Player>(GetTree().Root);
		if (players.Count > 0)
		{
			player = (Player)players[0];
		}

		this.Pressed += OnButtonPressed;
	}

	private void OnButtonPressed()
	{
		GD.PrintErr("I DO NOTHING " + this.Name);
		/*
		if (player.CanBuild(buildingId))
		{
			if (player.IsBuilding)
			{
				if (player.selectedBuildingId != buildingId)
				{
					player.selectedBuildingId = buildingId;
					player.IsBuilding = true;
				}
				else
				{
					player.IsBuilding = false;
				}
			}
			else
			{
				player.selectedBuildingId = buildingId;
				player.IsBuilding = true;
			}
		}*/
	}
}
