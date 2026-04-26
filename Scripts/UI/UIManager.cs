using Godot;
using System;

public partial class UIManager : Node
{
    [Export] Label coinsLabel;
    [Export] ItemList TowerImprovementsList;
    [Export] ItemList BuildingButtonsList;
    [Export] Label status;

    public override void _Ready()
    {
        BuildingButtonsList.ItemClicked += Ebuchka;
    }

    public void Ebuchka(long index, Vector2 atPosition, long mouseButtonIndex) 
    {
        if(BuildingButtonsList.GetItemText((int)index) == "house")
        {
            GD.Print("HOUSE");
        }
    }

    public void UpdateStatus(bool isBuilding, bool isImprovingTower) 
    {
        status.Text = $"isBuilding={isBuilding}; isImprovingTower={isImprovingTower}";
    }
    public void UpdateCoins(int coins)
    {
        coinsLabel.Text = $"COINS: {coins}";
    }

    public void ShowTurretSelectionMenu(TurretSpot turretSpot)
    {
        BuildingButtonsList.Hide();
        TowerImprovementsList.AddItem("turret test");
        TowerImprovementsList.Show();
    }

    public void HideTurretSelectionMenu()
    {
        TowerImprovementsList.Clear();
        TowerImprovementsList.Hide();
        BuildingButtonsList.Show();
    }
}
