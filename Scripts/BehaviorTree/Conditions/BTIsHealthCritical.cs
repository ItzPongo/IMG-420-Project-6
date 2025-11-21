using Godot;

public partial class BTIsHealthCritical : BTNode
{
	[Export] public float CriticalThreshold = 20f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float health = blackboard.GetValue<float>("Health");
		float maxHealth = blackboard.GetValue<float>("MaxHealth");

		if (maxHealth <= 0) return BTState.Failure;

		float healthPercent = (health / maxHealth) * 100f;

		return healthPercent < CriticalThreshold ? BTState.Success : BTState.Failure;
	}
}
