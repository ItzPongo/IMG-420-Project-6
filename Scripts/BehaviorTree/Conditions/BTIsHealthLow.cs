using Godot;

public partial class BTIsHealthLow : BTNode
{
	[Export] public float LowThreshold = 50f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float health = blackboard.GetValue<float>("Health");
		float maxHealth = blackboard.GetValue<float>("MaxHealth");
		
		if (maxHealth <= 0) return BTState.Failure;
		
		float healthPercent = (health / maxHealth) * 100f;
		
		return healthPercent < LowThreshold ? BTState.Success : BTState.Failure;
	}
}
