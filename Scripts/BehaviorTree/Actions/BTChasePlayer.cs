using Godot;

public partial class BTChasePlayer : BTNode
{
	[Export] public float ChaseSpeed = 100f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
			
		Node2D player = blackboard.GetValue<Node2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		Vector2 direction = (player.GlobalPosition - character.GlobalPosition).Normalized();
		character.Velocity = direction * ChaseSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Chasing");
		
		return BTState.Running;
	}
}
