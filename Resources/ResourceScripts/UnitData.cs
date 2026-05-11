using Godot;
using System;

namespace Resources
{
    [GlobalClass]
    public partial class UnitData : Resource
    {
        [Export] public int Id;
        [Export] public string Name;

        [Export] public sbyte Health { get; private set; } = 3;
        [Export] public sbyte Damage { get; private set; } = 1;
        [Export] public float Speed { get; private set; } = 2;
        [Export] public float AttackDuration { get; private set; } = 1;

        [Export] public byte CoinDrop { get; private set; } = 1;


        [Export] public PackedScene Scene { get; set; }
    }
}
