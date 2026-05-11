using Godot;
using System;

enum AgentState
{
    Moving,
    Waiting,
    Fighting,
    Disabled
}

public partial class FlowfieldAgent : Node
{
    [Export] Unit unit;
    [Export] AimingComponent aimingComponent;
    [Export] public int lookAhead = 3;

    AgentState currentState = AgentState.Disabled;

    // we only need 2 nodes - current and target
    //
    [Export] public FlowfieldNode CurrentNode;
    private FlowfieldNode TargetNode;

    Vector3 direction;


    public override void _Ready()
    {
        if (CurrentNode != null)
        {
            CurrentNode.IsFree = false;
            unit.GlobalPosition = CurrentNode.GlobalPosition;
            SelectTarget();
        }
    }

    public void ClearNodes()
    {
        if (currentState == AgentState.Moving)
        {
            TargetNode.IsReserved = false;
        }
        if (currentState == AgentState.Waiting)
        {
            CurrentNode.IsFree = true;
        }
        if (currentState == AgentState.Fighting)
        {
            CurrentNode.IsFree = true;
        }
    }

    public void Init(FlowfieldNode originNode)
    {
        if (originNode == null)
        {
            GD.PrintErr("Init received null originNode");
            return;
        }

        CurrentNode = originNode;
        currentState = AgentState.Waiting;

        CurrentNode.IsFree = false;

        unit.GlobalPosition = CurrentNode.GlobalPosition;

        SelectTarget();
    }
    public void StartMoving()
    {
        // this is supposed to update the agent's state and reset the node it's coming from
        currentState = AgentState.Moving;
        CurrentNode.IsFree = true;
    }

    public void StartWaiting()
    {
        currentState = AgentState.Waiting;
    }

    public void StopMoving()
    {
        // this is supposed to update the agent's state and reset the node it's coming from
        currentState = AgentState.Disabled;
    }

    public void Move(double delta)
    {
        // this is supposed to update the agent's position
        // if it reaches target node it makes a check for buildings
        // if buildings were found it awaits until the building is destroyed
        // , otherwise it selects new target and keeps moving
        float distance = unit.GlobalPosition.DistanceTo(TargetNode.GlobalPosition);

        if (distance < 0.01f)
        {
            Arrive();
        }
        else
        {
            if(CurrentNode == null) { GD.PrintErr("CurrentNode = null in Move"); }
            float currentSpeed = unit.currentSpeed * CurrentNode.speedModifier;
            float step = currentSpeed * (float)delta;

            if (step >= distance)
            {
                Arrive();
            }
            else
            {
                unit.GlobalPosition += direction * step;
            }
        }
    }

    private void Arrive()
    {
        unit.GlobalPosition = TargetNode.GlobalPosition;
        CurrentNode = TargetNode;
        TargetNode.IsFree = false;
        TargetNode.IsReserved = false;
        TargetNode = null;

        // check for buildings from Unit
        if (unit.CheckBuildingInfront())
        {
            // found building -> update state
            currentState = AgentState.Fighting;
        }
        else
        {
            SelectTarget();
        }
    }

    public override void _Process(double delta)
    {
        if (currentState == AgentState.Disabled) { return; }

        if (currentState == AgentState.Fighting) { return; }
        if(currentState == AgentState.Moving)
        {
            Move(delta);
        }
        if (currentState == AgentState.Waiting)
        {
            SelectTarget();
        }
    }

    /// <summary>
    /// Assign this agent to a specific entry node
    /// </summary>
    public void SetStartingNode(FlowfieldNode node)
    {
        CurrentNode = node;
    }
    public void SelectTarget()
    {
        // this thing is called to choose the next node for the agent to move to.
        // it must reserve it as to not allow other agent to move there
        // if target is found it starts moving there, otherwise awaits until one is found
        TargetNode = GetClosestNode();
        if (TargetNode != null)
        {
            float rotationAngle = Utilities.GetHorizontalRotation(unit.GlobalPosition, TargetNode.GlobalPosition);
            unit.GlobalRotation = new Vector3(0, rotationAngle, 0);
            direction = (TargetNode.GlobalPosition - unit.GlobalPosition).Normalized();
            TargetNode.IsReserved = true;

            StartMoving();
        }
        else
        {
            currentState = AgentState.Waiting;
        }
    }

    private FlowfieldNode GetClosestNode()
    {
        FlowfieldNode ClosestNode = null;
        float minDistance = float.MaxValue;
        foreach (var node in CurrentNode.NextNodes)
        {
            if (node.IsFree && !node.IsReserved)
            {
                float nodeDistance = CurrentNode.NodeDistances[node];
                if (minDistance > nodeDistance)
                {
                    ClosestNode = node;
                    minDistance = nodeDistance;
                }
            }
        }
        return ClosestNode;
    }
}