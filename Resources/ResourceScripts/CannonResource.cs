using Godot;

namespace Resources
{
    [GlobalClass]
    public partial class CannonResource : Resource
    {
        [Export] public uint Id { get; set; }
        [Export] public string Name { get; set; }
        [Export] public int Cost { get; set; }

        [Export] public PackedScene Scene { get; set; }
    }
}
