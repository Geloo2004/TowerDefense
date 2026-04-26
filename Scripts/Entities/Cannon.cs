using Godot;
using System;
using System.Collections.Generic;

public partial class Cannon : Node3D
{
    [Export] float reloadTime = 1;
    Timer reloadTimer;
    Timer peaceTimer;

    [Export] Vector2I detectionArea;

    [Export] Unit target;
    [Export] Godot.Collections.Array<AimingComponent> aimingComponents;
    bool readyToFire = false;

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
    }

    public void Fire()
    {
        if (!readyToFire) { return; }

        readyToFire = false;
        GD.Print("BAM!!!");
        target.TakeDamage();
        reloadTimer.Start(reloadTime);

        audioSource?.Play();
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

        if (target == null) { return; }
        if(target.health <= 0) { target = null; return; }

        if (readyToFire) 
        {
            Fire();
        }
    }

    public void UpdateTarget(Node3D target)
    {
        this.target = (Unit)(target.GetParent());
        foreach(var aimingComponent in aimingComponents)
        {
            aimingComponent.SetTarget(target);
        }
        GD.Print($"New target: {target.Name}");
    }
}
