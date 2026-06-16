using Godot;
using System;

public partial class WallGate : Building
{
	[Export] MeshInstance3D gateMesh;
	[Export] Area3D collider;
	[Export] AudioStreamPlayer3D audioSource;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	public override void TakeDamage(int damage)
	{
		if (currentHealth > 0)
        {
            GD.Print("GATE DAMAGED");
            currentHealth -= damage;
			audioSource.Play();
            GD.Print("HEALTH: "+currentHealth);
            if (currentHealth <= 0)
			{
				GD.Print("UPDATE MESH");
				gateMesh?.QueueFree();
				collider?.QueueFree();
				collider = null;
			}
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
