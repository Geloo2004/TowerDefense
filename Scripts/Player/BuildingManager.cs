using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public partial class BuildingManager : Node
{
    [Export] public Godot.Collections.Dictionary<uint, string> buildingDataPaths;	// id - path
    private Dictionary<uint, Resources.BuildingData> bData = new Dictionary<uint, Resources.BuildingData>();
    Dictionary<Vector3I, uint> builtBuildings;
    private BuildingOutline buildingOutline;
    /*
    1 = house
    2 = tower
    3 = castle
    4 = wall
    5 = gate
    6 = 
    */
    [Export] public GridMap buildableGrid;
    [Export] MeshLibrary tileMeshLib;

    // Towers
    // Walls

    /*
		Instantiate GridColliders (area3d cubes) for every cell.
		Register onMouseEnter to reference here
		Update onMouseEnter if player is in building mode
		Update GridColliders colors based on freeness and selected building size
	 */

    Node3D buildingsParent;
    Dictionary<Vector3I, GridCell> cells;

    // emitted when player attempts to build
    [Signal] public delegate void MouseClickEventHandler();

    // emitted when a building is destroyed by enemy
    [Signal] public delegate void BuildingDestructionEventHandler();

    // Called when the node enters the scene tree for the first time.

    #region BuildingRequirements

    public int GetPrice(uint buildingID)
    {
        return (bData[buildingID].Cost);
    }

    public bool IsAreaFree(Vector3I start, uint buildingID)
    {
        foreach(var item in builtBuildings)
        {
            GD.Print(item);
        }

        var size = bData[buildingID].Size;

        for (int x = 0; x < size.X; x++)
        {
            for (int z = 0; z < size.Z; z++)
            {
                Vector3I checkPos = new Vector3I(
                    start.X + (x * (int)buildableGrid.CellSize.X),
                    start.Y,
                    start.Z + (z * (int)buildableGrid.CellSize.Z)
                );

                GD.Print($"Checking: {x} {z} : {checkPos}");
                if (builtBuildings.TryGetValue(checkPos, out uint buildingIndex))
                {
                    GD.Print($"INDEX: {buildingIndex}");
                    if (buildingIndex != 0)
                    {
                        GD.Print($"{start} is NOT FREE!");
                        return false;
                    }
                }
                else
                {
                    builtBuildings.Add(checkPos, 0);
                    GD.Print($"{start} is FREE!");
                    return true;
                }
            }
        }
        GD.Print($"{start} is FREE!");
        return true;
    }

    #endregion
    public void CreateBuildingOutline(uint buildingID)
    {
        GD.Print("CreateBuildingOutline");
        buildingOutline = bData[buildingID].OutlineScene.Instantiate<BuildingOutline>();
        buildingOutline.Visible = false;
        this.AddChild(buildingOutline);
    }

    public void UpdateBuildingOutline(Vector3I gridPosition, uint buildingID)
    {
        buildingOutline.Visible = true;
        GD.Print("UpdateBuildingOutline");
        buildingOutline.Position = gridPosition;

        GD.Print($"CHECKING POSITION {gridPosition}");
        buildingOutline.SetColor(IsAreaFree(gridPosition, buildingID));

        GD.Print($"buildingOutline in {gridPosition}");
    }

    public void DestroyBuildingOutline()
    {
        GD.Print("DestroyBuildingOutline");
        if (buildingOutline != null) {
            if (buildingOutline.Visible)
            {
                buildingOutline.QueueFree();
            }
        }
    }

    public bool CanBuild(Vector3I position, uint buildingID)
    {
        // determines whether a building can be built in a position with a set of requirements

        if (IsAreaFree(position, buildingID))
        {

            switch (buildingID)
            {
                case 1:
                    return true;
                case 2:
                    return true;
                case 3:
                    return true;
                case 4:
                    return true;
                case 5:
                    return true;


            }
            return false;
        }
        else 
        {
            return false; 
        }
    }

    public void BuildWall(Vector3I startTile, Vector3I endTile)
    {
        // Get direction
        // Check longest direction
        // No other type of building must interfere
        // Make cost calculations
        // Build
        GD.Print($"BUILDING A WALL {startTile} -> {endTile}");

        Vector3I direction = endTile - startTile;
        GD.Print(direction);
        int magnitude;
        float rotation;
        if (direction.X > direction.Z) 
        {
            magnitude = Mathf.Abs(direction.X);
            rotation = 0;
            direction = new Vector3I(1, 0, 0);
        }
        else 
        {
            magnitude = Mathf.Abs(direction.Z);
            rotation = 90;
            direction = new Vector3I(0, 0, 1);
        }

        GD.Print(magnitude);

        Build(startTile, 0, 2);
        for (int i = (int)buildableGrid.CellSize.X; i < magnitude; i+= (int)buildableGrid.CellSize.X)
        {
            Build(startTile + direction * i, rotation, 4);
        }
        Build(startTile + direction * magnitude, 0, 2);
    }
    

    public void Build(Vector3I gridPosition, float rotation, uint buildingID)
    {

        // Instantiate building
        Building building = bData[buildingID].Scene.Instantiate<Building>();

        building.Position = gridPosition;
        building.RotationDegrees = new Vector3(0,rotation,0);
        // Add to buildings parent
        buildingsParent.AddChild(building);

        builtBuildings[gridPosition] = buildingID;

        // Update grid spaces
        UpdateCells(gridPosition, bData[buildingID].Size, buildingID);
    }

    private void UpdateCells(Vector3I start, Vector3I size, uint newBuildingID)
    {
        for (int x = 0; x < size.X; x++)
        {
            for (int z = 0; z < size.Z; z++)
            {
                Vector3I occupiedPos = new Vector3I(
                    start.X + (x * (int)buildableGrid.CellSize.X),
                    start.Y,
                    start.Z + (z * (int)buildableGrid.CellSize.Z)
                );
                builtBuildings[occupiedPos] = newBuildingID;
                GD.Print($"{occupiedPos} updated : {builtBuildings[occupiedPos]}");
            }
        }
    }

    #region Preparation

    private GridCell CreateColliderTemplate()
    {
        GridCell template = new GridCell();
        template.Name = "GridCellCollider_Template";
        // GD.Print("empty template created");

        Area3D area3d = new Area3D();
        area3d.InputRayPickable = true;
        // GD.Print("area3d created");
        template.AddChild(area3d);
        // GD.Print("area3d added");

        CollisionShape3D collisionShape = new CollisionShape3D();
        BoxShape3D boxShape = new BoxShape3D();
        boxShape.Size = buildableGrid.CellSize;
        collisionShape.Shape = boxShape;
        area3d.AddChild(collisionShape);
        area3d.CollisionLayer = Utilities.GetCollisionMaskValue(3);
        // GD.Print("collision3d added");

        return template;
    }
    private bool HasCellsAbove(ref Vector3I coordinates, ref Godot.Collections.Array<Vector3I> cells)
    {
        GD.Print((int)buildableGrid.CellSize.Y);

        for (int i = 1; i < 10; i++)
        {
            if (cells.Contains(new Vector3I(coordinates.X, coordinates.Y + i, coordinates.Z)))
            {
                return true;
            }
        }
        return false;
    }


    public override void _EnterTree()
    {
        // load building data
        foreach (var path in buildingDataPaths)
        {
            var buildingData = ResourceLoader.Load<Resources.BuildingData>(path.Value);
            bData[path.Key] = buildingData;
        }

        // GD.Print("Enter tree");
        cells = new Dictionary<Vector3I, GridCell>();
        builtBuildings = new Dictionary<Vector3I, uint>();

        // Create template with your GridCell script
        GridCell.SetBuildingManager(this);
        GridCell cellTemplate = CreateColliderTemplate();
        // GD.Print("template created");

        var usedCells = buildableGrid.GetUsedCells();
        foreach (var cell in usedCells)
        {
            Vector3I coordinates = new Vector3I(cell.X, cell.Y, cell.Z);
            string cellName = buildableGrid.GetCellItem(coordinates).ToString();

            if (tileMeshLib.GetItemName(Convert.ToInt32(cellName)) != "GROUND_FLAT")
            {
                //  GD.Print($"CELL: {tileMeshLib.GetItemName(Convert.ToInt32(cellName))}");
                continue;
            }

            if (HasCellsAbove(ref coordinates, ref usedCells)) { continue; }
            // Duplicate the template (includes the script!)
            GridCell cellInstance = (GridCell)cellTemplate.Duplicate();

            // Position the cell
            Vector3 worldPos = buildableGrid.MapToLocal(coordinates);
            cellInstance.Position = worldPos;
            cellInstance.Name = $"GridCell_{cell.X}_{cell.Y}_{cell.Z}";

            // Add to scene
            AddChild(cellInstance);
            // GD.Print("child added");

            // Store reference
            cells.Add(coordinates, cellInstance);
            //  GD.Print("reference stored");
        }

    }
    public override void _Ready()
    {
        buildingsParent = GetNodeOrNull<Node3D>("/root/Node/Buildings");
        if (buildingsParent == null)
        {
            buildingsParent = new Node3D();
            buildingsParent.Name = "Buildings";
            GetTree().Root.AddChild(buildingsParent);
            GD.Print("Created Buildings parent");
        }


        GD.Print($"Searching for buildings");
        Godot.Collections.Array<Node> existingBuildings = Utilities.FindAllObjectsOfType<Building>(GetTree().Root);
        GD.Print($"Found: {existingBuildings.Count}");

        foreach (Node buildingNode in existingBuildings)
        {
            Building b = buildingNode as Building;
            if (b != null)
            {
                UpdateCells(b.IntPosition, b.size, ((Building)buildingNode).GetID());
            }
        }

        foreach (var item in builtBuildings)
        {
            GD.Print(item);
        }
    }

    #endregion

    public override void _Process(double delta)
    {

    }
}