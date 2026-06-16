using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class FlowfieldNode : Node3D
{
    [Export] public Godot.Collections.Array<FlowfieldNode> NextNodes;
    public bool IsFree = true;
    public bool IsReserved = false;

    [Export] MeshInstance3D reserved;
    [Export] MeshInstance3D free;

    public Dictionary<FlowfieldNode, float> NodeDistances { get; private set; }

    public override void _Ready()
    {
        // Auto-populate NextNodes from child FlowfieldNodes
        if (NextNodes == null || NextNodes.Count == 0)
        {
            NextNodes = new Godot.Collections.Array<FlowfieldNode>();

            foreach (Node child in GetChildren())
            {
                if (child is FlowfieldNode flowNode)
                {
                    NextNodes.Add(flowNode);
                }
            }
        }

        // Build distance cache
        NodeDistances = new Dictionary<FlowfieldNode, float>();

        foreach (var node in NextNodes)
        {
            if (node == null)
                continue;

            NodeDistances[node] =
                (node.GlobalPosition - GlobalPosition).Length();

            GD.Print(NodeDistances[node]);
        }
    }
    public override void _Process(double delta)
    {
        free.Visible = IsFree;
        reserved.Visible = IsReserved;
    }
}