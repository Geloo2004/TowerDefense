using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using static Godot.TabContainer;

public partial class SceneManager : Node
{
    /*
		Instantiate GridColliders (area3d cubes) for every cell.
		Register onMouseEnter to reference here
		Update onMouseEnter if player is in building mode
		Update GridColliders colors based on freeness and selected building size
	 */

    Player player;

    /*
    // emitted when player attempts to build
    [Signal] public delegate void MouseClickEventHandler();

    // emitted when a building is destroyed by enemy
    [Signal] public delegate void BuildingDestructionEventHandler();
    */

    // Called when the node enters the scene tree for the first time.
    public override void _EnterTree()
    {


    }

    public override void _Ready()
    {

        // assign player
        var players = Utilities.FindAllObjectsOfType<Player>(GetTree().Root);
        if (players.Count > 0)
        {
            player = (Player)players[0];
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
