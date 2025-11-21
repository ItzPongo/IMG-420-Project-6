using Godot;

public partial class Ally : CharacterBody2D
{
	[Export] public float MaxHealth = 50f;
	[Export] public float Speed = 80f;
	[Export] public float AttackRange = 50f;
	[Export] public float AttackDamage = 5f;
	[Export] public float AttackCooldown = 1.5f;
	[Export] public float DetectionRange = 250f;
	
	private float health;
	private Node2D player;
	private float lastAttackTime = 0f;
	private AnimatedSprite2D sprite;
	private string lastDirection = "down";
	
	public override void _Ready()
	{
		health = MaxHealth;
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		CallDeferred(nameof(FindPlayer));
	}
	
	private void FindPlayer()
	{
		player = GetTree().Root.FindChild("Player", true, false) as Node2D;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (player == null || !IsInstanceValid(player))
			return;
		
		float distance = GlobalPosition.DistanceTo(player.GlobalPosition);
		
		if (distance < DetectionRange)
		{
			if (distance > AttackRange)
			{
				Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
				Velocity = direction * Speed;
				MoveAndSlide();
			}
			else
			{
				Velocity = Vector2.Zero;
				TryAttack();
			}
		}
		
		UpdateAnimation();
	}
	
	private void TryAttack()
	{
		float currentTime = (float)Time.GetTicksMsec() / 1000f;
		if (currentTime - lastAttackTime >= AttackCooldown)
		{
			if (player != null && player.HasMethod("TakeDamage"))
			{
				player.Call("TakeDamage", AttackDamage);
				lastAttackTime = currentTime;
				GD.Print("Ally attacked player!");
			}
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
		
		HealthBar healthBar = GetNode<HealthBar>("HealthBar");
		
		healthBar.UpdateHealth(health, MaxHealth);
		
		if (health <= 0)
		{
			QueueFree();
		}
	}
}
