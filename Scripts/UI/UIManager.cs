using Godot;
using System;

public partial class UIManager : Node
{
    [Export] Label coinsLabel;
    [Export] ItemList TurretsButtonsList;
    [Export] ItemList BuildingButtonsList;
    [Export] Label statusLabel;
    [Export] Label messageLabel;
    [Export] Label tooltipLabel;
    [Export] Panel buildingInfoPanel;   // move this thing with cursor. Show when hovers over a clickbox


    [Export] float messageDuration = 3;
    Timer messageTimer;

    Vector2I buildingInfoPanelOffset = new Vector2I(10,10);

    public delegate void MouseHover(bool isMouseOverUI);
    public event MouseHover OnMouseHoverUI;

    public delegate void SelectBuilding(int selectedBuildingId);
    public event SelectBuilding OnBuildingSelected;

    public delegate void SelectTurret(int selectedTurretId);
    public event SelectTurret OnTurretSelected;

    string[] buildings = new string[]
    {
        "HOUSE",
        "TOWER",
        "CASTLE",
    };

    string[] turrets = new string[]
    {
        "MEDIUM",
        "HEAVY",
        "SNIPER",
        "MORTAR",
        "WALL BATTERY"
    };

    private void UpdateLayout()
    {
        float SelectionListHeightRatio = 0.2f;
        float SelectionListVOffsetRatio = 0.7f;

        float SelectionListWidthRatio = 0.1f;
        float SelectionListGapRatio = 0.025f;

        Vector2I screenSize = GetWindow().Size;
        messageLabel.Position = Vector2I.Zero;
        messageLabel.Size = screenSize;

        int buttonListHeight = (int)(SelectionListHeightRatio * screenSize.Y);
        int buttonListVOffset = (int)(SelectionListVOffsetRatio * screenSize.Y);

        int buttonListWidth = (int)(SelectionListWidthRatio * screenSize.X);
        int buttonListMargin = (int)(SelectionListGapRatio * screenSize.X);

        TurretsButtonsList.OffsetTop = buttonListVOffset;
        TurretsButtonsList.OffsetBottom = buttonListVOffset + buttonListHeight;
        TurretsButtonsList.OffsetRight = buttonListWidth * 2 + buttonListMargin * 2; // position of the right border
        TurretsButtonsList.OffsetLeft = buttonListWidth + buttonListMargin * 2;   // position of the left border


        BuildingButtonsList.OffsetTop = buttonListVOffset;
        BuildingButtonsList.OffsetBottom = buttonListVOffset + buttonListHeight;
        BuildingButtonsList.OffsetRight = buttonListWidth + buttonListMargin; // position of the right border
        BuildingButtonsList.OffsetLeft = buttonListMargin;

        GD.Print("WIDTH: " + buttonListWidth);
        GD.Print("MARGIN: " + buttonListMargin);
        GD.Print("HEIGHT: " + buttonListHeight);

    }

    public override void _Process(double delta)
    {
        buildingInfoPanel.GlobalPosition = GetWindow().GetMousePosition() + buildingInfoPanelOffset;
    }

    public override void _Ready()
    {
        UpdateLayout();

        messageTimer = new Timer();
        this.AddChild(messageTimer);
        messageTimer.OneShot = true;
        messageTimer.Autostart = false;
        messageTimer.Timeout += () => { messageLabel.Text = ""; };

        BuildingButtonsList.Clear();
        for (int i = 0; i < buildings.Length; i++) 
        {
            BuildingButtonsList.AddItem(buildings[i],null,true);
        }
        BuildingButtonsList.ItemClicked += HandleBuildingClick;

        TurretsButtonsList.Clear();
        for (int i = 0; i < turrets.Length; i++)
        {
            TurretsButtonsList.AddItem(turrets[i], null, true);
        }
        TurretsButtonsList.ItemClicked += HandleTurretClick;


        BuildingButtonsList.MouseEntered += () => { OnMouseHoverUI.Invoke(true); };
        BuildingButtonsList.MouseExited += () => { OnMouseHoverUI.Invoke(false); };

        TurretsButtonsList.MouseEntered += () => { OnMouseHoverUI.Invoke(true); };
        TurretsButtonsList.MouseExited += () => { OnMouseHoverUI.Invoke(false); };
    }

    public void HandleBuildingClick(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        OnBuildingSelected.Invoke((int)index + 1);
    }
    public void HandleTurretClick(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        OnTurretSelected.Invoke((int)index + 1);
    }

    public void UpdateStatus(bool isBuilding, bool isImprovingTower, int selectedBuildingID) 
    {
        statusLabel.Text = $"isBuilding={isBuilding}; isImprovingTower={isImprovingTower}; selectedBuildingID={selectedBuildingID}";
    }
    public void UpdateCoins(int coins)
    {
        coinsLabel.Text = $"COINS: {coins}";
    }


    public void UpdateTooltip(string message)
    {
        tooltipLabel.Text = message;
    }

    public void ShowBuildingMenus()
    {
        BuildingButtonsList.Show();
        TurretsButtonsList.Show();
    }
    public void HideBuildingMenus()
    {
        BuildingButtonsList.Hide();
        TurretsButtonsList.Hide();
    }
    public void ShowUI()
    {
        BuildingButtonsList.Show();
        TurretsButtonsList.Show();
        messageLabel.Show();
        tooltipLabel.Show();
    }

    public void HideUI()
    {
        BuildingButtonsList.Hide();
        TurretsButtonsList.Hide();
        messageLabel.Hide();
        tooltipLabel.Hide();
    }

    public void ShowMessage(string message)
    {
        messageLabel.Text = message;
        if (!messageTimer.IsStopped())
        {
            messageTimer.Stop();
        }
        messageTimer.Start(messageDuration);
    }
}
