using Godot;
using System;
using System.Collections.Generic;

public partial class TurretSpot : Node3D
{
    private Cannon currentCannon;
    public bool hasCannon { get { return currentCannon != null; } }
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

    public void RemoveCannon()
    {
        currentCannon.QueueFree();
        currentCannon = null;
    }

    public void Die()
    {
        // remove/replace cannon
        currentCannon.Die();
        // start playing particles
    }
}
