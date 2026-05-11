using Godot;
using System;
using System.Collections.Generic;

public partial class UnitAnimationController : Node
{
    [Export] Godot.Collections.Dictionary<UnitState, string> Animations;
    [Export] AnimationPlayer animController;

    static Random Random = new Random();
	public void PlayAnimation(UnitState currentState)
	{
		List<string> possibleAnimations = new List<string>();
		foreach (var animation in Animations) 
		{
			if (animation.Key == currentState)
			{
				possibleAnimations.Add(animation.Value);
			}
		}

        // play random animation
        animController.Play(possibleAnimations[Random.Next(possibleAnimations.Count)]);
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
