using Godot;
using System;

namespace Resources
{
    [GlobalClass]
    public partial class UnitData : Resource
    {
        [Export] public int Id;
        [Export] public string Name;
        [Export] public int Damage;
        [Export] public int CoinDrop;
//[Export] item drop
        [Export] public sbyte Health { get; set; } = 3;
        [Export] public PackedScene Scene { get; set; }
    }
}
