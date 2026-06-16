using Godot;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

public abstract partial class Building : Node3D
{
    [Export] public int currentHealth { get; protected set; }
    [Export] public int maxHealth { get; protected set; }

    public abstract void TakeDamage(int damage);
}