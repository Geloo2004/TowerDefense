using Godot;
using System;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public enum UnitState
{
	Run,
	Attack
}

public partial class Unit : CharacterBody3D
{
	[Export] string DataPath;
	[Export] Node3D target;
	[Export] float speed;
	[Export] float attackRange0;
    UnitState currentState = UnitState.Run;
	[Export] public sbyte health = 3;
	[Export] Area3D attackRange;
	[Export] AudioStreamPlayer3D audioSource;
	// write audio controller for random sounds
	[Export] float attackDuration = 1;
	[Export] AnimationPlayer animController;
	// write a random animation selector for each state
	Timer attackTimer;
	Node attackTarget;
	int currentHealth;

	[Export] NavigationAgent3D navAgent;

	bool readyToAttack = true;

    Resources.UnitData _data;
    Resources.UnitData Data
    {
        get
        {
            if (_data == null && !string.IsNullOrEmpty(DataPath))
            {
                _data = ResourceLoader.Load<Resources.UnitData>(DataPath);
                if (_data == null)
                {
                    GD.PrintErr($"Failed to load BuildingData from: {DataPath}");
                }
            }
            return _data;
        }
        set
        {
            _data = value;
        }
    }

	public override void _EnterTree()
	{
		if (Data == null)
		{
			GD.PrintErr($"Unit {Name} has no Data! Path: {DataPath}");
			return;
		}
		else
		{

			currentHealth = Data.Health;
		}
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        attackTimer = new Timer();
        attackTimer.Autostart = false;
        attackTimer.OneShot = true;
        AddChild(attackTimer);
        attackTimer.Timeout += () =>
        {
			readyToAttack = true;
        };
        animController.Play("RUN");

        navAgent.TargetPosition = target.GlobalPosition;
		navAgent.VelocityComputed += UpdateVelocity;
    }

	private void UpdateVelocity(Vector3 safeVelocity) 
	{
		Velocity = safeVelocity;
	}

	public void Die()
	{
		// Disappear;
		// Drop items
		// Drop coins
		GD.Print($"{this.Name} dies");
	}

	public void PlayAttack()
	{
		if (currentState == UnitState.Attack)
        {
            readyToAttack = false;
            animController.Play("FIGHT");

			audioSource.Play();
			((Building)attackTarget).TakeDamage(Data.Damage);
			attackTimer.Start(attackDuration);
		}
    }
	public void SwitchState(UnitState state)
    {
		if(currentState == state) { return; }

        currentState = state;
        if (state == UnitState.Attack)
		{

		}
        else if (state == UnitState.Run)
        {
            animController.Play("RUN");
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
    {
		if (currentState == UnitState.Run)
		{
			if (navAgent.IsNavigationFinished()) { return; }

			Vector3 nextPos = navAgent.GetNextPathPosition();
			Vector3 direction = GlobalPosition.DirectionTo(nextPos);
            Vector3 newVelocity = direction * speed * (float)delta;

			if (navAgent.AvoidanceEnabled)
			{
				navAgent.Velocity = newVelocity;
            }
            else
            {
                UpdateVelocity(newVelocity);
            }

            LookAt(GlobalPosition + Velocity);

			if (!navAgent.IsTargetReachable())
			{
				GD.Print("TARGET UNREACHABLE");
				animController.Stop();
			}

			MoveAndSlide();
        }
	}

	public void TakeDamage()
	{
		health--;
		if(health <= 0)
		{
			QueueFree();
		}
	}
}
