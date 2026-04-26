using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class Building : Node3D
{
    [Export] public string DataPath { get; set; } // Path to the .tres file
    [Export] protected Area3D clickBox;
    [Export] protected AudioStreamPlayer3D audioSource;
    protected Resources.BuildingData _data;
    protected int currentHealth;

    public Resources.BuildingData Data
    {
        get
        {
            if (_data == null && !string.IsNullOrEmpty(DataPath))
            {
                _data = ResourceLoader.Load<Resources.BuildingData>(DataPath);
                if (_data == null)
                {
                    GD.PrintErr($"Failed to load BuildingData from: {DataPath}");
                }
            }
            return _data;
        }
        set
        {
            _data = value;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) 
        {
            
        }
    }

    public uint GetID()
    {
        return Data.Id;
    }

    public override void _Ready()
    {
        if (clickBox != null)
        {
            clickBox.MouseEntered += OnClick;
        }
    }

    public void OnClick()
    {
        GD.Print($"{this.Name} clicked");
    }

    public SceneManager sceneGrid;
    private Vector3I intPosition;

    public Vector3I IntPosition
    {
        get { return intPosition; }
        private set { intPosition = value; }
    }

    // Convenience properties
    public Vector3I size { get { if (Data == null)
            {
                GD.PrintErr($"Failed to load BuildingData from: {DataPath}");
                return Vector3I.Zero;
            }
            else { return Data.Size; }
        }
        set { }
    }
    public int maxHealth => Data?.Health ?? 3;

    public override void _EnterTree()
    {
        if (Data == null)
        {
            GD.PrintErr($"Building {Name} has no Data! Path: {DataPath}");
            return;
        }

        currentHealth = Data.Health;

        intPosition.X = (int)Math.Round(Position.X, 0);
        intPosition.Y = (int)Math.Round(Position.Y, 0);
        intPosition.Z = (int)Math.Round(Position.Z, 0);
    }
}