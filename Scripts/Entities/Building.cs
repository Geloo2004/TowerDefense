using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class Building : Node3D
{
    [Export] public int health { get; protected set; }
    [Export] public int maxHealth { get; protected set; }

    public virtual void TakeDamage(int damage)
    {

    }
}