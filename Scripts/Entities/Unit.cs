using Godot;
using System;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

public enum UnitState
{
	Run,
	Attack
}

public partial class Unit : Node3D
{
    //pathfininding
    [Export] FlowfieldAgent pathfindingAgent;

    private FlowfieldNode originNode;

    private Timer disapperanceTimer;

	// data management
	[Export] string DataPath;

    // Unit variables
    public sbyte currentHealth { get; private set; }
    public float currentSpeed { get; private set; }

    public bool IsDead { get { return currentHealth <= 0; } }

	
	// AI
    UnitState currentState = UnitState.Run;
	Building target;
	[Export] RayCast3D attackRay;
	[Export] float attackRange = 0.5f;

	// SFX
	[Export] AudioStreamPlayer3D audioSource;
	// write audio controller for random sounds

	// Animation
	[Export] UnitAnimationController animationController;
	// write a random animation selector for each state
	Timer attackTimer;
	Node attackTarget;


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

	private void LoadData()
	{
		currentHealth = _data.Health;
		currentSpeed = _data.Speed;
	}

    public void Init(FlowfieldNode originNode)
    {
        this.originNode = originNode;

        if (pathfindingAgent == null)
        {
            GD.PrintErr("PathfindingAgent missing");
            return;
        }

        pathfindingAgent.Init(originNode);
    }

    public override void _EnterTree()
	{
		if (Data == null)
		{
			GD.PrintErr($"Unit {Name} has no Data! Path: {DataPath}");
			return;
		}
		LoadData();
    }

	public bool CheckBuildingInfront()
	{
        Rid collision = attackRay.GetColliderRid();
        if (!collision.IsValid)
        {
			return false;
        }
        else
        {
            // if enemy detected -> make it TARGET
            Area3D area = (Area3D)(attackRay.GetCollider());
            Building building = area.GetParent<Building>();

            if (building != null)
            {
                target = building;
            }
        }
        return true;
    }
    public override void _Ready()
	{
		attackRay.TargetPosition = new Vector3(0,-attackRange,0);

        attackTimer = new Timer();
        attackTimer.Autostart = false;
        attackTimer.OneShot = true;
        AddChild(attackTimer);
        attackTimer.Timeout += () =>
        {
			readyToAttack = true;
        };

        disapperanceTimer = new Timer();
        disapperanceTimer.Autostart = false;
        disapperanceTimer.OneShot = true;
        AddChild(disapperanceTimer);
        disapperanceTimer.Timeout += () => { QueueFree(); };

        currentState = UnitState.Run;
        animationController.PlayAnimation(currentState);

     //   pathfindingAgent.Enable(originNode);
    }

	public void PlayAttack()
	{
        readyToAttack = false;
        attackTimer.Start();
        animationController.PlayAnimation(UnitState.Attack);
		target.TakeDamage(Data.Damage);
		if(target.currentHealth <= 0) 
		{
            pathfindingAgent.StartWaiting();
            PlayRun();
        }
    }

	private void PlayRun()
    {
        target = null;
        animationController.PlayAnimation(UnitState.Run);
       // pathfindingAgent.MoveToNextNode();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
		if (target!=null && readyToAttack) { PlayAttack(); }
	}

	public void TakeDamage(sbyte damage)
	{
		currentHealth-=damage;
        if (currentHealth <= 0)
		{
			pathfindingAgent.ClearNodes();
            pathfindingAgent.StopMoving();
             disapperanceTimer.Start(5);
            this.GlobalPosition += new Vector3(0, -10, 0);
        }
	}
}
