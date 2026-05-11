using Godot;

namespace Resources
{
    [GlobalClass]
    public partial class CannonData : Resource
    {
        [Export] public uint Id { get; set; }
        [Export] public string Name { get; set; }
        [Export] public int Price { get; set; }


        [Export] public PackedScene Scene { get; set; }
    }
}
