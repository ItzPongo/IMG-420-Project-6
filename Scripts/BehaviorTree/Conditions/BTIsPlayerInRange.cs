using Godot;

public partial class BTIsPlayerInRange : BTNode
{
	[Export] public float Range = 200f;
	[Export] public bool IsAttackRange = false;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		Node2D player = blackboard.GetValue<Node2D>("Player");
		
		if (player == null || !IsInstanceValid(player) || agent is not Node2D agentNode)
			return BTState.Failure;
		
		float distance = agentNode.GlobalPosition.DistanceTo(player.GlobalPosition);
		
		return distance <= Range ? BTState.Success : BTState.Failure;
	}
}
