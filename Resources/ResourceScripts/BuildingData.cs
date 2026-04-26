using Godot;

namespace Resources
{
    [GlobalClass]
    public partial class BuildingData : Resource
    {
        [Export] public uint Id { get; set; }
        [Export] public string BuildingName { get; set; }
        [Export] public Vector3I Size { get; set; } = new Vector3I(1, 0, 1);
        [Export] public int Health { get; set; } = 3;
        [Export] public PackedScene Scene { get; set; }
        [Export] public PackedScene OutlineScene { get; set; }
        [Export] public int Cost { get; set; }
        // [Export] public Texture2D Icon { get; set; }
    }
}