using Godot;
using System;

public partial class Damage : Node
{
	[Export] float damage = 1;
	[Export] DamageType type;
}

public enum DamageType
{
	Melee,
	Ranged,
	Siege
}
