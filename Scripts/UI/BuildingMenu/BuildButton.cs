using Godot;
using System;

public partial class BuildButton : Button
{
	[Export] int buildingId;
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
		player.StartBuildingTurrets(buildingId);
    }
}
