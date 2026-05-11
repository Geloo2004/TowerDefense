using Godot;
using Godot.Collections;
using System;
using System.Data;

public partial class Player : Node
{

	private int selectedBuildingId = 0;
    private int selectedBuildingPrice = 0;
    private int selectedTurretId = 0;
    // BUILDING
    [Export] CameraControl camera;

   // [Export] BuildingManager buildingManager;

    [Export] TurretBuildingManager turretBuildingManager;
    [Export] UIManager uiManager;

    private bool isBuilding = false;
    private bool isBuildingTurret = false;
    private bool isMouseOverUI = false;

    Vector3I startTile;
    Vector3I endTile;
    Vector3I buildingOutlinePosition;
    bool outOfBounds = false;
    [Export] public uint collisionMask = 2;
    Node clickedObject = null;
    /*
        1 - buildings hitboxes
        2 - buildings clickboxes
        3 - tiles

        4 - unit hitboxes

    */


    // MOUSE CONTROL
    float lmbHoldDuration = 0;
	float rmbHoldDuration = 0;
    [Export] float holdDuration = 0.15f;

    // GAME
    [Export] public int coins = 100;
    SceneManager sceneManager;

    private void RestartLevel()
    {
        this.GetTree().ReloadCurrentScene();
    }




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Palace palace = (Palace)Utilities.FindAllObjectsOfType<Palace>(this.GetTree().Root)[0];
        palace.OnPalaceDestruction += RestartLevel;

        uiManager.UpdateCoins(coins);
        uiManager.OnBuildingSelected += StartBuilding;
        uiManager.OnTurretSelected += StartBuildingTurrets;

        var sceneManager = Utilities.FindAllObjectsOfType<SceneManager>(GetTree().Root);
        if (sceneManager.Count > 0)
        {
            this.sceneManager = (SceneManager)sceneManager[0];
        }

        uiManager.OnMouseHoverUI += (bool isMouseOverUI) => { this.isMouseOverUI = isMouseOverUI; };
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        uiManager.UpdateStatus(isBuilding, isBuildingTurret, selectedBuildingId);
        var rmbInput = Input.GetActionStrength("RMBPress");
        var lmbInput = Input.GetActionStrength("LMBPress");

        if (isBuilding)
        {
           //buildingManager.UpdateBuildingOutline(camera.GetObjectUnderMouse(collisionMask),selectedBuildingId);
        }
        if (isBuildingTurret) 
        {
            
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
            StopBuilding();
            StopBuildingTurrets();
        }

        uiManager.UpdateStatus(isBuilding, isBuildingTurret,selectedBuildingId);
    }
    private void HandleRMBPress(double delta)
    {
        rmbHoldDuration += (float)delta;
        // On first press
    }
    public void HandleRMBClick() { }
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
        lmbHoldDuration = 0;
        return;

        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null) { return; }

        if (outOfBounds) { return; }
        if (clickedObject is GridCell)
        {
            endTile = ((GridCell)clickedObject).IntPosition;
        }

       // buildingManager.BuildWall(startTile, endTile);
    }

    private void OnLMBHold(double delta)
    {
        lmbHoldDuration += (float)delta;
    }

    private void HandleLMBClick()
    {
        lmbHoldDuration = 0;

        if (isMouseOverUI)
        {
            return;
        }

        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);

        if (clickedObject == null)
        {
            if (lmbHoldDuration > 0)
            {
                if (isBuildingTurret)
                {
                    isBuildingTurret = false;
                    StopBuildingTurrets();
                }

                if (isBuilding)
                {
                    StopBuilding();
                }
            }
            return;
        }
        else if (isBuilding)
        {
            /*
            GD.Print($"selectedBuildingId = {selectedBuildingId}");
            if(buildingManager.GetPrice(selectedBuildingId) > coins) 
            {
                uiManager.ShowMessage("NOT ENOUGH COINS");
                return;
            }
            if (clickedObject is GridCell)
            {
                var clickedTile = ((GridCell)clickedObject).IntPosition;

                if (buildingManager.IsAreaFree(clickedTile, selectedBuildingId))
                {
                    coins -= selectedBuildingPrice;
                    buildingManager.Build(clickedTile, 0, selectedBuildingId);
                    outOfBounds = false;
                    uiManager.UpdateCoins(coins);
                }
                else
                {
                    GD.Print("AREA NOT FREE");
                }
            }
            else
            {
                StopBuilding();
            }
            */
        }
        else if (isBuildingTurret)
        {
            GD.Print("isBuildingTurret");
            if (clickedObject is TurretSpot)
            {
                var clickedTurretSpot = (TurretSpot)clickedObject;

                if (turretBuildingManager.GetPrice(selectedTurretId) > coins) { return; }

                GD.Print("isBuildingTurret and price is ok");
                turretBuildingManager.BuildTurret(clickedTurretSpot, selectedTurretId);
            }
            else
            {
                StopBuildingTurrets();
            }
        }
        else
        {
            /*
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
            */
        }
    }
    private void EnableIdle()
    {
        collisionMask = 2;
    }

    private void StopBuildingTurrets()
    {
        isBuildingTurret = false;

        uiManager.ShowBuildingMenus();
        uiManager.UpdateTooltip("");
    }
    private void StartBuildingTurrets(int selectedTurretId)
    {
        isBuildingTurret = true;
        collisionMask = 2;

        this.selectedTurretId = selectedTurretId;

        uiManager.HideBuildingMenus();
        uiManager.UpdateTooltip("[ PRESS RMB TO CANCEL ]");
    }
    public void StartBuilding(int selectedBuildingId)
    {
        

        isBuilding = true;
        collisionMask = 3;

        this.selectedBuildingId = selectedBuildingId;
        /*
        buildingManager.DestroyBuildingOutline();
        buildingManager.CreateBuildingOutline(selectedBuildingId);
        buildingOutlinePosition = Vector3I.MinValue;
        */
        uiManager.HideBuildingMenus();
        uiManager.UpdateTooltip("[ PRESS RMB TO CANCEL ]");
        
    }

    public void StopBuilding()
    {
        isBuilding = false;

       // buildingManager.DestroyBuildingOutline();

        uiManager.ShowBuildingMenus();
        uiManager.UpdateTooltip("");
    }
}