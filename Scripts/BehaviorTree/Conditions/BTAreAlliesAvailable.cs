using Godot;

public partial class BTAreAlliesAvailable : BTNode
{
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		int currentAllies = blackboard.GetValue<int>("CurrentAllyCount", 0);
		int maxAllies = blackboard.GetValue<int>("MaxAllies", 2);
		
		float lastSummonTime = blackboard.GetValue<float>("LastSummonTime", 0f);
		float summonCooldown = blackboard.GetValue<float>("SummonCooldown", 5f);
		float currentTime = (float)Time.GetTicksMsec() / 1000f;
		
		bool cooldownReady = (currentTime - lastSummonTime) >= summonCooldown;
		bool canSummon = currentAllies < maxAllies && cooldownReady;
		
		GD.Print($"Allies Check - Current: {currentAllies}, Max: {maxAllies}, Cooldown: {cooldownReady}, CanSummon: {canSummon}");
		
		return canSummon ? BTState.Success : BTState.Failure;
	}
}
