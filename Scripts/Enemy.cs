using Godot;
using System.Collections.Generic;

public partial class Enemy : CharacterBody2D
{
	[ExportGroup("Stats")]
	[Export] public float MaxHealth = 100f;
	[Export] public float DetectionRange = 200f;
	[Export] public float AttackRange = 50f;
	[Export] public float AttackCooldown = 1f;
	[Export] public float SummonCooldown = 5f;
	[Export] public int MaxAllies = 2;
	
	[ExportGroup("References")]
	[Export] public PackedScene AllyScene;
	
	[ExportGroup("Patrol")]
	[Export] public Vector2[] PatrolPointsArray;
	
	private float health;
	private Blackboard blackboard;
	private BTNode behaviorTreeRoot;
	private Label stateLabel;
	private Node2D player;
	private AnimatedSprite2D sprite;
	private string lastDirection = "down";
	
	public override void _Ready()
	{
		health = MaxHealth;
		blackboard = new Blackboard();
		
		InitializeBlackboard();
		
		stateLabel = GetNodeOrNull<Label>("StateLabel");
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		behaviorTreeRoot = GetNodeOrNull<BTNode>("BehaviorTree");
		
		CallDeferred(nameof(FindPlayer));
		
		if (behaviorTreeRoot != null)
		{
			behaviorTreeRoot.Initialize(this, blackboard);
		}
	}
	
	private void FindPlayer()
	{
		player = GetTree().Root.FindChild("Player", true, false) as Node2D;
		if (player == null)
		{
			GD.PrintErr("Enemy: Could not find Player node!");
		}
		blackboard.SetValue("Player", player);
	}
	
	private void InitializeBlackboard()
	{
		blackboard.SetValue("Health", health);
		blackboard.SetValue("MaxHealth", MaxHealth);
		blackboard.SetValue("LastAttackTime", 0f);
		blackboard.SetValue("AttackCooldown", AttackCooldown);
		blackboard.SetValue("LastSummonTime", -999f);
		blackboard.SetValue("SummonCooldown", SummonCooldown);
		blackboard.SetValue("CurrentAllyCount", 0);
		blackboard.SetValue("MaxAllies", MaxAllies);
		blackboard.SetValue("CurrentWaypoint", 0);
		blackboard.SetValue("CurrentState", "Idle");
		
		List<Vector2> patrolPoints = new List<Vector2>();
		if (PatrolPointsArray != null && PatrolPointsArray.Length > 0)
		{
			foreach (var point in PatrolPointsArray)
			{
				patrolPoints.Add(GlobalPosition + point);
			}
		}
		else
		{
			patrolPoints.Add(GlobalPosition + new Vector2(100, 0));
			patrolPoints.Add(GlobalPosition + new Vector2(-100, 0));
		}
		blackboard.SetValue("PatrolPoints", patrolPoints);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		blackboard.SetValue("Health", health);
		
		if (behaviorTreeRoot != null)
		{
			behaviorTreeRoot.Tick(this, blackboard);
		}
		
		UpdateUI();
		UpdateAnimation();
	}
	
	private void UpdateUI()
	{
		if (stateLabel != null)
		{
			string state = blackboard.GetValue<string>("CurrentState", "Unknown");  // ← Add <string>
			stateLabel.Text = $"{state}\nHP: {health:F0}/{MaxHealth:F0}";
		}
	}

	private void UpdateAnimation()
	{
		if (sprite == null || sprite.SpriteFrames == null) return;

		if (Velocity.Length() > 0)
		{
			if (Mathf.Abs(Velocity.Y) > Mathf.Abs(Velocity.X))
			{
				lastDirection = Velocity.Y > 0 ? "down" : "up";
				sprite.FlipH = false;
			}
			else if (Velocity.X != 0)
			{
				lastDirection = "right";
				sprite.FlipH = Velocity.X < 0;
			}

			if (sprite.SpriteFrames.HasAnimation(lastDirection))
				sprite.Play(lastDirection);
		}
		else
		{
			if (sprite.SpriteFrames.HasAnimation(lastDirection))
			{
				sprite.Animation = lastDirection;
				sprite.Frame = 0;
			}
		}
	}
	
	public void TakeDamage(float damage)
	{
		health -= damage;
		health = Mathf.Max(0, health);
		
		GD.Print($"Enemy took {damage} damage. Health: {health}/{MaxHealth}");
		
		HealthBar healthBar = GetNode<HealthBar>("HealthBar");
		
		healthBar.UpdateHealth(health, MaxHealth);
		
		if (health <= 0)
		{
			GD.Print("Enemy died!");
			QueueFree();
		}
	}
	
	public override void _Draw()
	{
		if (Engine.IsEditorHint() || OS.IsDebugBuild())
		{
			DrawCircle(Vector2.Zero, DetectionRange, new Color(1, 0, 0, 0.1f));
			DrawCircle(Vector2.Zero, AttackRange, new Color(1, 1, 0, 0.2f));
		}
	}
}
