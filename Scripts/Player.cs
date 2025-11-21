using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200f;
	[Export] public float MaxHealth = 100f;
	[Export] public float AttackDamage = 15f;
	[Export] public float AttackRange = 60f;
	[Export] public float AttackCooldown = 0.5f;
	
	private float health;
	private float lastAttackTime = 0f;
	private AnimatedSprite2D sprite;
	private Label healthLabel;
	private string lastDirection = "walk_down"; // track last movement direction
	
	public override void _Ready()
	{
		health = MaxHealth;
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		healthLabel = GetNodeOrNull<Label>("HealthBar/Label");
	}
	
	public override void _PhysicsProcess(double delta)
	{
		HandleMovement();
		HandleAttack();
		UpdateAnimation();
		UpdateHealthDisplay();
	}
	
	private void HandleMovement()
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;
		MoveAndSlide();
	}
	
	private void HandleAttack()
	{
		if (Input.IsActionJustPressed("attack"))
		{
			float currentTime = (float)Time.GetTicksMsec() / 1000f;
			if (currentTime - lastAttackTime >= AttackCooldown)
			{
				PerformAttack();
				lastAttackTime = currentTime;
			}
		}
	}
	
	private void PerformAttack()
	{
		var spaceState = GetWorld2D().DirectSpaceState;
		var query = new PhysicsShapeQueryParameters2D();
		var shape = new CircleShape2D();
		shape.Radius = AttackRange;
		query.Shape = shape;
		query.Transform = new Transform2D(0, GlobalPosition);
		query.CollideWithAreas = false;
		query.CollideWithBodies = true;

		var results = spaceState.IntersectShape(query);

		foreach (var result in results)
		{
			if (result.ContainsKey("collider"))
			{
				Node collider = result["collider"].As<Node>();
				if (collider != this && collider.HasMethod("TakeDamage"))
				{
					collider.Call("TakeDamage", AttackDamage);
					GD.Print($"Player attacked {collider.Name}!");
				}
			}
		}
	}
	
	private void UpdateAnimation()
	{
		if (sprite == null || sprite.SpriteFrames == null) return;

		if (Velocity.Length() > 0)
		{
			// Determine movement direction
			if (Mathf.Abs(Velocity.Y) > Mathf.Abs(Velocity.X))
			{
				lastDirection = Velocity.Y > 0 ? "walk_down" : "walk_up";
			}
			else if (Velocity.X != 0)
			{
				lastDirection = "walk_right";
				sprite.FlipH = Velocity.X < 0;
			}

			if (sprite.SpriteFrames.HasAnimation(lastDirection))
				sprite.Play(lastDirection);
		}
		else
		{
			// Player stopped: show first frame of lastDirection
			if (sprite.SpriteFrames.HasAnimation(lastDirection))
			{
				sprite.Animation = lastDirection;
				sprite.Frame = 0;
			}
		}
	}

	private void UpdateHealthDisplay()
	{
		if (healthLabel != null)
		{
			healthLabel.Text = $"HP: {health:F0}/{MaxHealth:F0}";
		}
	}
	
	public void TakeDamage(float damage)
	{
		health -= damage;
		health = Mathf.Max(0, health);
		
		GD.Print($"Player took {damage} damage. Health: {health}/{MaxHealth}");
		
		HealthBar healthBar = GetNode<HealthBar>("HealthBar");
		
		healthBar.UpdateHealth(health, MaxHealth);
		
		if (health <= 0)
		{
			GD.Print("Player died! Restarting...");
			GetTree().ReloadCurrentScene();
		}
	}
	
	public override void _Draw()
	{
		if (Engine.IsEditorHint() || OS.IsDebugBuild())
		{
			DrawCircle(Vector2.Zero, AttackRange, new Color(0, 1, 0, 0.2f));
		}
	}
}
