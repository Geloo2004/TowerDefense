using Godot;
using System;

[GlobalClass]
public partial class ProjectileData : Resource
{
    // Do we even need it? Wouldn't the reference weigh more
    [Export] public int Id;
    [Export] public string Name;
    [Export] public float Damage;
    [Export] public PackedScene Scene { get; set; }
}
