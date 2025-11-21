using Godot;

public partial class BTCanAttack : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		float lastAttackTime = blackboard.GetValue<float>("LastAttackTime");
		float attackCooldown = blackboard.GetValue<float>("AttackCooldown", 1f);
		float currentTime = (float)Time.GetTicksMsec() / 1000f;

		bool canAttack = (currentTime - lastAttackTime) >= attackCooldown;

		return canAttack ? BTState.Success : BTState.Failure;
	}
}
