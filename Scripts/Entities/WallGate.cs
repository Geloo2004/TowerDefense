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
		if (health > 0)
		{
			health -= damage;
			audioSource.Play();
			if (health <= 0)
			{
				gateMesh?.QueueFree();
				collider?.QueueFree();
			}
		}
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
