using Godot;

public partial class BTAttack : BTNode
{
	[Export] public float AttackDamage = 10f;
	[Export] public float AttackRange = 50f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		Node2D player = blackboard.GetValue<Node2D>("Player");
		
		if (player == null || !IsInstanceValid(player) || agent is not Node2D agentNode)
			return BTState.Failure;
		
		float distance = agentNode.GlobalPosition.DistanceTo(player.GlobalPosition);
		
		if (distance <= AttackRange)
		{
			if (player.HasMethod("TakeDamage"))
			{
				player.Call("TakeDamage", AttackDamage);
			}
			
			float currentTime = (float)Time.GetTicksMsec() / 1000f;
			blackboard.SetValue("LastAttackTime", currentTime);
			blackboard.SetValue("CurrentState", "Attacking");
			
			GD.Print($"Enemy attacked player for {AttackDamage} damage!");
			
			return BTState.Success;
		}
		
		return BTState.Failure;
	}
}
