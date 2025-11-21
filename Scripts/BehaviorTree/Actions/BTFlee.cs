using Godot;

public partial class BTFlee : BTNode
{
	[Export] public float FleeSpeed = 150f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
			
		Node2D player = blackboard.GetValue<Node2D>("Player");
		if (player == null || !IsInstanceValid(player))
			return BTState.Failure;
		
		Vector2 fleeDirection = (character.GlobalPosition - player.GlobalPosition).Normalized();
		character.Velocity = fleeDirection * FleeSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Fleeing");
		
		return BTState.Running;
	}
}
