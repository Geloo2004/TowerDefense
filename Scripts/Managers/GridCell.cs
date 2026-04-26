using Godot;
using System;

public partial class GridCell : Node3D
{
    static BuildingManager buildingManager;
    Vector3I intPosition;
    Player player;
    public Vector3I IntPosition
    {
        get { return intPosition; }
        private set { }
    }

    public static void SetBuildingManager(BuildingManager manager)
    {
        buildingManager = manager;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        intPosition.X = (int)Position.X;
        intPosition.Y = (int)Position.Y;
        intPosition.Z = (int)Position.Z;
        // GD.Print($"Cell Position V3: {Position}");
        // GD.Print($"Cell Position V3I: {intPosition}");

        var a = GetChild<Area3D>(0);
		if (a != null)
		{
		//	GD.Print(a);
		}
		else
        {
          //  GD.Print("b");
        }

        var area3D = GetChild<Area3D>(0);
        area3D.MouseEntered += OnMouseEntered;
        area3D.MouseExited += OnMouseExited;


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

    // This will automatically connect if the signal exists
    private void OnMouseEntered()
    {
        return;
       // GD.Print("Mouse entered!");
        Position += new Vector3(0,0.2f,0);
        if (buildingManager != null)
        {
            GD.Print(IntPosition);
          //  sceneGrid.CheckSpot(IntPosition);
            if(player == null) { return; }
           // player.startTile = IntPosition;
            //player.overTile = true;
        }
    }
    private void OnMouseExited()
    {
        return;
     //   GD.Print("Mouse exited!");
        Position -= new Vector3(0, 0.2f, 0);
       // player.overTile = false;
    }
}