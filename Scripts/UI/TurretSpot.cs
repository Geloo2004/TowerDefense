using Godot;
using System;
using System.Collections.Generic;

public partial class TurretSpot : Node3D
{
	[Export] public Godot.Collections.Array<uint> allowedTurrets {  get; private set; }
    private Cannon currentCannon;
    public override void _Ready()
    {
        base._Ready();
    }
	public void BuildTurret(PackedScene cannon) 
    {
        Cannon newCannon = cannon.Instantiate<Cannon>();
        this.AddChild(newCannon);
        newCannon.GlobalPosition = this.GlobalPosition;
        currentCannon = newCannon;
        GD.Print("CANNON BUILT");
    }
}
