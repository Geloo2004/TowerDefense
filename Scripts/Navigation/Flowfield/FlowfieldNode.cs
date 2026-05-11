using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FlowfieldNode : Node3D
{
    [Export] public Godot.Collections.Array<FlowfieldNode> NextNodes;
    [Export] public float speedModifier = 1;
    public bool IsFree = true;
    public bool IsReserved = false;

    [Export] MeshInstance3D reserved;
    [Export] MeshInstance3D free;

    public Dictionary<FlowfieldNode, float> NodeDistances { get; private set; }

    public override void _Ready()
    {
        NodeDistances = new Dictionary<FlowfieldNode, float>();
        foreach (var node in NextNodes)
        {
            NodeDistances[node] = (node.GlobalPosition - this.GlobalPosition).Length();
            GD.Print(NodeDistances[node]);
        }
    }

    public override void _Process(double delta)
    {
        free.Visible = IsFree;
        reserved.Visible = IsReserved;
    }
}