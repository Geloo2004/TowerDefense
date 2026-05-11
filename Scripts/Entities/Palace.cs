using Godot;
using System;

public partial class Palace : Building
{
	public delegate void PalaceDestruction();
	public event PalaceDestruction OnPalaceDestruction;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
    }
    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            OnPalaceDestruction.Invoke();
        }
    }
}
