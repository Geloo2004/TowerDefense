using Godot;
using System;
using System.Collections.Generic;

public partial class Cannon : Node3D
{
    [Export] float reloadTime = 1;
    Timer reloadTimer;
    Timer peaceTimer;

    [Export] Area3D detectionArea;
    [Export] Godot.Collections.Array<AimingComponent> aimingComponents;
    bool readyToFire = false;

    List<Unit> unitsInRange = new List<Unit>();
    Unit currentTarget = null;
    int currentTargetPriority = 0;

    static int distancePriority = 3;
    static int damagePriority = 20;
    static int speedPriority = 10;  

    static float targetSwitchMargin = 1.5f; // shows how many times greater (in percent) the priority of a new target should be for the cannon to switch to it instead of the current one


   // [Export] Resources.CannonData _data;
    [Export] AudioStreamPlayer3D audioSource;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        reloadTimer = new Timer();
        reloadTimer.Autostart = false;
        reloadTimer.OneShot = true;
        AddChild(reloadTimer);
        reloadTimer.Timeout += () =>
        {
            readyToFire = true;
            GD.Print("LOADED!");
        };

      //  detectionArea.AreaEntered += UpdateTarget;

        reloadTimer.Start(reloadTime);

        detectionArea.AreaEntered += (Area3D collision) => 
        {
            var parent = collision.GetParent();
            if(parent is Unit)
            {
                unitsInRange.Add((Unit)parent);
            }
        };
        detectionArea.AreaExited += (Area3D collision) =>
        {
            var parent = collision.GetParent();
            if (parent is Unit)
            {
                unitsInRange.Remove((Unit)parent);                
            }
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
    {
        // SCRAPPED. TOO DIFFICULT

        // find target distance
        // find TIME to reach target now
        // guess position after TIME
        // find angles
        // aim

        // ADD RANDOM ROTATION IF NO ENEMIES?


        // choose target
        // deal damage
        // start reloading
        if (unitsInRange.Count > 0)
        {
            SelectTarget();
            bool isAimed = AimCannon(delta);
            if (readyToFire && isAimed)
            {
                Fire();
            }
        }
    }

    private void SelectTarget()
    {
        Unit highestPriorityUnit = null;
        int highestPriority = 0;

        foreach (var unit in unitsInRange)
        {
            float distance = (unit.GlobalPosition - this.GlobalPosition).LengthSquared();
            int priority = (int)(distancePriority * distance);

            if(priority > highestPriority)
            {
                highestPriority = priority;
                highestPriorityUnit = unit;
            }
        }

        if (highestPriority > currentTargetPriority)
        {
            currentTarget = highestPriorityUnit;
            currentTargetPriority = (int)(highestPriority * targetSwitchMargin);
            UpdateAimingComponentsTarget(highestPriorityUnit);
        }

        currentTarget = highestPriorityUnit;
    }

    private void Fire()
    {
        if(currentTarget == null) { return; }

        readyToFire = false;
        if (aimingComponents[0].isAimed)
        {
            GD.Print("TakeDamage");
            currentTarget.TakeDamage(1);
            if (currentTarget.currentHealth <= 0)
            {
                UpdateAimingComponentsTarget(null);
            }
        }

        reloadTimer.Start(reloadTime);
    }

    private bool AimCannon(double delta)
    {
        bool isAimed = true;
        foreach (var aimingComponent in aimingComponents)
        {
            aimingComponent.AimToTarget(delta);
            if (!aimingComponent.isAimed) { isAimed = false; }
        }
        GD.Print(isAimed);
        return isAimed;
    }

    private void UpdateAimingComponentsTarget(Node3D newTarget)
    {
        foreach (var aimingComponent in aimingComponents)
        {
            aimingComponent.SetTarget(newTarget);
        }
    }
}
