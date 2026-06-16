using Godot;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;

public partial class UnitAnimationController : Node
{
	Dictionary<UnitState, string[]> animations = new Dictionary<UnitState, string[]> 
	{
		{UnitState.Run, new string[] { "Run" } },
        {UnitState.Attack, new string[] { "Attack0", "Attack0", "Attack0" } },
        {UnitState.Idle, new string[] { "Idle0", "Idle1", "Idle2" } },
		//{UnitState.Dead, new string[] { "Death0", "Death2", "Death3" } }
    };
    [Export] AnimationPlayer animController;
	[Export] CpuParticles3D particles3D;
	[Export] MeshInstance3D mesh;

    static Random Random = new Random();
	public void PlayAnimation(UnitState currentState)
	{
		if (currentState == UnitState.Dead) { PlayDeathAnimation(); }
		else
		{
			string[] possibleAnimations = animations[currentState];

			// play random animation
			animController.Play(possibleAnimations[Random.Next(possibleAnimations.Length)]);
		}
	}

	private void PlayDeathAnimation()
	{
		particles3D.Emitting = true;
		mesh.Visible= false;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
