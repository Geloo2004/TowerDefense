using Godot;
using Godot.Collections;
using System;
using System.Data;

public partial class Player : Node
{
    private bool hasLost;

    private int selectedBuildingId = 0;
    private int selectedBuildingPrice = 0;
    private int selectedTurretId = 0;

    [Export] LevelScripter difficultyManager;

    [Export] Resource standardCursor;
    [Export] Resource buildingCursor;


    [Export] PackedScene nextLevel;
    [Export] CameraControl camera;
    [Export] TurretBuildingManager turretBuildingManager;
    [Export] UIUpdater uiManager;

    TownHall selectedTownHall = null;

    private bool isBuildingTurret = false;
    private bool isInteractingWithTownHall = false;
    private bool isMouseOverUI = false;

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

    // PAUSE CONTROL
    bool isPaused = false;
    bool isPausePressed = false;

    SceneManager sceneManager;
    [Export] EconomyManager economyManager;
    private void RestartLevel()
    {
        this.GetTree().ReloadCurrentScene();
    }

    private void LoadNextLevel()
    {
        // load new level when all waves are completed and all enemies are dead
        GetTree().ChangeSceneToPacked(nextLevel);
    }

    private void Lose()
    {
        hasLost = true;
        isBuildingTurret = false;
        uiManager.PlayLossScreen(difficultyManager.GetSurvivedWaves());
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Input.SetCustomMouseCursor(standardCursor);
        uiManager.taxUpgradeBttn.Pressed += UpgradeTownHallTaxes;
        uiManager.healthUpgradeBttn.Pressed += UpgradeTownHallHealth;
        uiManager.restartBttn.Pressed += RestartLevel;
        uiManager.loadNextLevelBttn.Pressed += LoadNextLevel;

        var palaces = Utilities.FindAllObjectsOfType<Palace>(this.GetTree().Root);
        if (palaces.Count > 0)
        {
            Palace palace = (Palace)palaces[0];
            palace.OnPalaceDestruction += Lose;
            GD.PrintErr("PALACE REGISTERED");
        }
        else
        {
            GD.PrintErr("PALACE DOESNT EXIST");
        }

        var sceneManager = Utilities.FindAllObjectsOfType<SceneManager>(GetTree().Root);
        if (sceneManager.Count > 0)
        {
            this.sceneManager = (SceneManager)sceneManager[0];
        }

        uiManager.OnMouseHoverUI += (bool isMouseOverUI) => { this.isMouseOverUI = isMouseOverUI; };
        uiManager.OnContinueButtonPressUI += HandlePausePress;
        economyManager.OnGoldUpdate += UpdateGold;
    }

    private void UpgradeTownHallTaxes()
    {
        GD.Print("UpgradeTownHallTaxes");
        int cost = selectedTownHall.GetTaxUpgradeCost();
        if (cost < economyManager.gold)
        {
            // play sound success
            economyManager.SpendGold(cost);
            selectedTownHall.UpgradeTaxes();
            uiManager.UpdateCardData(selectedTownHall);
        }
        else
        {
            // play sound failure
            // display message

        }
    }

    private void UpgradeTownHallHealth()
    {
        GD.Print("UpgradeTownHallHealth");
        int cost = selectedTownHall.GetHealthUpgradeCost();
        if (cost < economyManager.gold)
        {
            // play sound success
            economyManager.SpendGold(cost);
            selectedTownHall.UpgradeHealth();
            uiManager.UpdateCardData(selectedTownHall);
        }
        else
        {
            // play sound failure
            // display message

        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (hasLost) { return; }

        // OPEN PAUSE MENU
        var pauseInput = Input.GetActionStrength("Pause");
        if (pauseInput > 0)
        {
            if(!isPausePressed)
            {
                isPausePressed = true;
                HandlePausePress();
            }
        }
        else
        {
            OnPauseRelease();
        }

        // READ GAME INPUT
        uiManager.UpdateStatus(isBuildingTurret, selectedBuildingId);
        var lmbInput = Input.GetActionStrength("LMBPress");

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

        uiManager.UpdateStatus(isBuildingTurret,selectedBuildingId);
    }

    private void HandlePausePress()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            uiManager.SetUIPage(UIPage.PauseMenu);
        }
        else
        {
            uiManager.SetUIPage(UIPage.Game);
        }
        UpdateCursor();
    }

    private void UpdateCursor()
    {
        GD.PrintErr("CURSOR IS NOT UPDATING");
        return;

        if (isBuildingTurret)
        {
            Input.SetCustomMouseCursor(buildingCursor);
        }
        else
        {
            Input.SetCustomMouseCursor(standardCursor);
        }
    }
    private void OnPauseRelease()
    {
        isPausePressed = false;
    }

    private void HandleLMBPress(double delta)
    {
        lmbHoldDuration += (float)delta;
    }
    private void OnLMBRelease()
    {
        lmbHoldDuration = 0;
        return;

        Node clickedObject = camera.GetObjectUnderMouse(collisionMask);
        if (clickedObject == null) { return; }

        if (outOfBounds) { return; }

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
            }
            return;
        }
        else if (isBuildingTurret)
        {
            if (clickedObject is TurretSpot)
            {
                StopUpgradingTownHall();
                var clickedTurretSpot = (TurretSpot)clickedObject;

                if (clickedTurretSpot.hasCannon) { uiManager.ShowMessage("ОБЪЕКТ УЖЕ ЗАНЯТ"); }

                if (turretBuildingManager.GetPrice(selectedTurretId) > economyManager.gold)
                {
                    return; 
                }

                turretBuildingManager.BuildTurret(clickedTurretSpot, selectedTurretId);
            }
            else
            {
                StopBuildingTurrets();
            }
        }
        else
        {
            var parent = clickedObject.GetParent();
            if (clickedObject is TownHall)
            {
                selectedTownHall = (TownHall)clickedObject;
                uiManager.ShowTownHallUI(selectedTownHall);
            }
            else
            {
                StopUpgradingTownHall();
                uiManager.ShowTurretsUI();
                selectedTownHall = null;
            }
        }
    }
    private void EnableIdle()
    {
        collisionMask = 2;
    }

    private void StopBuildingTurrets()
    {
        Input.SetCustomMouseCursor(standardCursor);
        isBuildingTurret = false;
        uiManager.UpdateTooltip("");
    }
    public void StartBuildingTurrets(int selectedTurretId)
    {
        Input.SetCustomMouseCursor(buildingCursor);
        isBuildingTurret = true;
        collisionMask = 2;

        this.selectedTurretId = selectedTurretId;
    }

    public void StopUpgradingTownHall()
    {
        selectedTownHall = null;
        uiManager.ShowTurretsUI();
    }

    public void StartUpgradingTownHall(TownHall newSelectedTownHall)
    {
        selectedTownHall = newSelectedTownHall;
        uiManager.ShowTownHallUI(newSelectedTownHall);
    }

    public void UpdateGold(int newGold)
    {
        uiManager.UpdateCoins(newGold);
    }
}