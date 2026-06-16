using Godot;
using System;
using System.Collections.Generic;

public partial class Cannon : Node3D
{
    [Export] private float reloadTime = 1f;
    [Export] private sbyte damage = 1;

    [Export] CpuParticles3D fireParticles;
    [Export] CpuParticles3D smokeParticles;

    [Export] private Area3D detectionArea;
    [Export] private Godot.Collections.Array<AimingComponent> aimingComponents;
    [Export] private AudioStreamPlayer3D audioSource;

    private readonly HashSet<Unit> unitsInRange = new();

    private Unit currentTarget;

    private Timer reloadTimer;
    private bool readyToFire;

    private bool isDead = false;

    private static readonly Random random = new();

    public void Die()
    {
        detectionArea.QueueFree();
        reloadTimer.Stop();
        reloadTimer.QueueFree();
        currentTarget = null;
        isDead = true;
    }

    public override void _Ready()
    {
        reloadTimer = new Timer();
        reloadTimer.OneShot = true;
        reloadTimer.Timeout += OnReloadFinished;

        AddChild(reloadTimer);

        detectionArea.AreaEntered += OnAreaEntered;
        detectionArea.AreaExited += OnAreaExited;

        StartReload();
    }

    public override void _Process(double delta)
    {
        if (isDead) { return; }
        ValidateCurrentTarget();

        if (currentTarget == null)
            AcquireTarget();

        if (currentTarget == null)
            return;

        bool aimed = AimAtTarget(delta);

        if (readyToFire && aimed)
            Fire();
    }

    private void OnReloadFinished()
    {
        readyToFire = true;
    }

    private void StartReload()
    {
        readyToFire = false;
        reloadTimer.Start(reloadTime);
    }

    private void OnAreaEntered(Area3D area)
    {
        if (area.GetParent() is Unit unit)
            unitsInRange.Add(unit);
    }

    private void OnAreaExited(Area3D area)
    {
        if (area.GetParent() is Unit unit)
        {
            unitsInRange.Remove(unit);

            if (unit == currentTarget)
                ClearTarget();
        }
    }

    private void ValidateCurrentTarget()
    {
        if (currentTarget == null)
            return;

        if (!GodotObject.IsInstanceValid(currentTarget))
        {
            ClearTarget();
            return;
        }

        if (currentTarget.IsDead)
        {
            ClearTarget();
            return;
        }

        if (!unitsInRange.Contains(currentTarget))
        {
            ClearTarget();
            return;
        }
    }

    private void AcquireTarget()
    {
        Unit bestTarget = null;
        float bestScore = float.MinValue;

        foreach (var unit in unitsInRange)
        {
            if (!GodotObject.IsInstanceValid(unit))
                continue;

            if (unit.IsDead)
                continue;

            float distance =
                GlobalPosition.DistanceSquaredTo(unit.GlobalPosition);

            float score =
                unit.threatLevel * 1000f - distance;

            if (score > bestScore)
            {
                bestScore = score;
                bestTarget = unit;
            }
        }

        if (bestTarget != null)
            SetTarget(bestTarget);
    }

    private void SetTarget(Unit target)
    {
        currentTarget = target;

        foreach (var aimer in aimingComponents)
            aimer.SetTarget(target);
    }

    private void ClearTarget()
    {
        currentTarget = null;

        foreach (var aimer in aimingComponents)
            aimer.SetTarget(null);
    }

    private bool AimAtTarget(double delta)
    {
        bool allAimed = true;

        foreach (var aimer in aimingComponents)
        {
            // Let each aimer run its own soft-rotate logic via _Process,
            // but we still need to know if all are aimed
            if (!aimer.isAimed)
                allAimed = false;
        }

        return allAimed;
    }
    private void Fire()
    {
        if (currentTarget == null)
            return;

        currentTarget.TakeDamage(damage);

        audioSource.PitchScale =
            1f + random.Next(0, 50) / 100f;

        audioSource.Play();

        if (currentTarget.IsDead)
            ClearTarget();

        fireParticles.Emitting = true;
        smokeParticles.Emitting = true;

        StartReload();
    }
}