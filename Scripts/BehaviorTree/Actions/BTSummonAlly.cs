using Godot;

public partial class BTSummonAlly : BTNode
{
	[Export] public PackedScene AllyScene;
	[Export] public float SummonDistance = 100f;
	
	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (AllyScene == null || agent is not Node2D agentNode)
		{
			GD.PrintErr("BTSummonAlly: AllyScene is null or agent is not Node2D!");
			return BTState.Failure;
		}
		
		Node2D ally = AllyScene.Instantiate<Node2D>();
		Vector2 spawnOffset = new Vector2(
			(float)GD.RandRange(-SummonDistance, SummonDistance),
			(float)GD.RandRange(-SummonDistance, SummonDistance)
		);
		ally.GlobalPosition = agentNode.GlobalPosition + spawnOffset;
		
		agentNode.GetTree().Root.CallDeferred("add_child", ally);
		
		int currentAllies = blackboard.GetValue<int>("CurrentAllyCount", 0);
		blackboard.SetValue("CurrentAllyCount", currentAllies + 1);
		
		float currentTime = (float)Time.GetTicksMsec() / 1000f;
		blackboard.SetValue("LastSummonTime", currentTime);
		blackboard.SetValue("CurrentState", "Summoning");
		
		GD.Print($"Enemy summoned an ally! Total allies: {currentAllies + 1}");
		
		return BTState.Success;
	}
}
