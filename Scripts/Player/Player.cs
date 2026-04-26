using Godot;
using Godot.Collections;
using System;
using System.Data;

public partial class Player : Node
{

	public uint selectedBuildingId = 0;
    private int selectedBuildingPrice = 0;

    // BUILDING
    [Export] CameraControl camera;
    [Export] BuildingManager buildingManager;
    [Export] UIManager uiManager;
    private bool isImprovingTower = false;

    public bool CanBuild(uint buildingID)
    {
        selectedBuildingPrice = buildingManager.GetPrice(buildingID);
        return selectedBuildingPrice <= coins;
    }

    private bool isBuilding = false;
    public bool IsBuilding 
    { 
        get { return isBuilding; }
        set
        {
            buildingManager.DestroyBuildingOutline();
            if (value)
            {
                buildingManager.CreateBuildingOutline(selectedBuildingId);
                collisionMask = 3;
            }
            else
            {
                collisionMask = 2;
            }
            buildingOutlinePosition = Vector3I.MinValue;
            isBuilding = value;
        }
    }
    Vector3I startTile;
    Vector3I endTile;
    Vector3I buildingOutlinePosition;
    bool outOfBounds = false;
    [Export] public uint collisionMask = 2;
    Node clickedObject = null;
    /*
        1 - hitboxes
        2 - buildings
        3 - tiles
    */


    // MOUSE CONTROL
    float lmbHoldDuration = 0;
	float rmbHoldDuration = 0;
    [Export] float holdDuration = 0.15f;

    // GAME
    [Export] public int coins = 100;
    SceneManager sceneManager;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        uiManager.UpdateCoins(coins);
        var sceneManager = Utilities.FindAllObjectsOfType<SceneManager>(GetTree().Root);
        if (sceneManager.Count > 0)
        {
            this.sceneManager = (SceneManager)sceneManager[0];
        }
    }

    public void UpdateBuildingOutline()
    {

        var cellUnderMouse = camera.GetObjectUnderMouse(collisionMask);
        if (cellUnderMouse == null) { return; }

        if (cellUnderMouse is GridCell)
        {
            Vector3I newPosition = ((GridCell)cellUnderMouse).IntPosition;
            if (buildingOutlinePosition!= newPosition)
            {
                buildingOutlinePosition = newPosition;
                buildingManager.UpdateBuildingOutline(buildingOutlinePosition, selectedBuildingId);
            }
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        uiManager.UpdateStatus(isBuilding, isImprovingTower);
        var rmbInput = Input.GetActionStrength("RMBPress");
        var lmbInput = Input.GetActionStrength("LMBPress");

        if (isBuilding)
        {
            UpdateBuildingOutline();
        }

        if (lmbInput > 0)
		{
            if(lmbHoldDuration == 0)
            {
                // On first press
                HandleLMBPress(delta);
            }
            else
            {
                OnLMBHold(delta);
            }
        }
        else if (lmbInput == 0)
        {
            if (lmbHoldDuration > holdDuration)
            {
                // On long hold and release
                OnLMBRelease();
            }
            else if(lmbHoldDuration>0)
            {
                // On quick hold and release
                HandleLMBClick();
            }
        }


        if (rmbInput > 0)
        {
            if (rmbHoldDuration == 0)
            {
                // On first press
                HandleRMBPress(delta);
            }
            else
            {
                OnRMBHold(delta);
            }
        }
        else if (rmbInput == 0)
        {
            if (rmbHoldDuration > holdDuration)
            {
                OnRMBRelease();
            }
            else if (rmbHoldDuration > 0)
            {
                // On quick hold and release
                HandleRMBClick();
            }
        }
        uiManager.UpdateStatus(isBuilding, isImprovingTower);
    }
    private void HandleRMBPress(double delta)
    {
        rmbHoldDuration += (float)delta;
        // On first press
    }
    public void HandleRMBClick()
    {
        GD.Print("HandleRMBClick");
        rmbHoldDuration = 0;
        // On quick hold and release
        var clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null) { return; }
        if(clickedObject is GridCell)
        {

        }
    }

    public void OnRMBHold(double delta) { rmbHoldDuration += (float)delta; }
    public void OnRMBRelease() { rmbHoldDuration = 0; }
    private void HandleLMBPress(double delta)
    {
        GD.Print("HandleLMBPress");
        lmbHoldDuration += (float)delta;
        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null) { return; }

        if (clickedObject is GridCell)
        {
            startTile = ((GridCell)clickedObject).IntPosition;
            outOfBounds = false;
        }
        else
        {
            outOfBounds = true;
        }
    }
    private void OnLMBRelease()
    {
        GD.Print("OnLMBRelease");
        lmbHoldDuration = 0;
        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null) { return; }

        if (outOfBounds) { return; }
        if (clickedObject is GridCell)
        {
            endTile = ((GridCell)clickedObject).IntPosition;
        }

        buildingManager.BuildWall(startTile, endTile);
    }

    private void OnLMBHold(double delta)
    {
        GD.Print("OnLMBHold");
        lmbHoldDuration += (float)delta;

        // add functionality for showing building outline, when the position of end cell changes
    }

    private void HandleLMBClick()
    {
        GD.Print("HandleLMBClick");
        lmbHoldDuration = 0;
        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null)
        {
            if (lmbHoldDuration > 0)
            {
                if (isImprovingTower)
                {
                    DisableTowerImprovement();
                }

                if (isBuilding)
                {
                    isBuilding = false;
                }
            }
            return;
        }

        if (isBuilding)
        {
            GD.Print("isBuilding");
            if (clickedObject is GridCell)
            {
                var clickedTile = ((GridCell)clickedObject).IntPosition;
                if (buildingManager.IsAreaFree(clickedTile, selectedBuildingId))
                {
                    coins-=selectedBuildingPrice;
                    buildingManager.Build(clickedTile, 0, selectedBuildingId);
                    outOfBounds = false;
                    uiManager.UpdateCoins(coins);
                }
            }
            else
            {
                GD.Print("outOfBounds");
                outOfBounds = true;
            }
        }
        else
        {
            GD.Print("is not Building");
            if (clickedObject is TurretSpot)
            {
                GD.Print("is TurretSpot");
                if (isImprovingTower)
                {
                    uiManager.HideTurretSelectionMenu();
                }
                isImprovingTower = true;
                collisionMask = 2;
                uiManager.ShowTurretSelectionMenu((TurretSpot)clickedObject);

            }
            else if (isImprovingTower)
            {
                GD.Print("isImprovingTower");
                uiManager.HideTurretSelectionMenu();
            }
        }
    }
    private void EnableIdle()
    {
        collisionMask = 2;
    }

    private void DisableTowerImprovement()
    {
        isImprovingTower = false;
        collisionMask = 2;
        uiManager.HideTurretSelectionMenu();
    }
    private void EnableTowerImprovement(TurretSpot turretSpot)
    {
        isImprovingTower = true;
        collisionMask = 2;
        uiManager.ShowTurretSelectionMenu(turretSpot);
    }
}