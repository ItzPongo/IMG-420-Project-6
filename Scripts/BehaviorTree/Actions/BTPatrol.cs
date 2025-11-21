using Godot;
using System.Collections.Generic;

public partial class BTPatrol : BTNode
{
	[Export] public float PatrolSpeed = 50f;
	[Export] public float WaypointReachDistance = 10f;

	public override BTState Tick(Node agent, Blackboard blackboard)
	{
		if (agent is not CharacterBody2D character)
			return BTState.Failure;
		
		List<Vector2> patrolPoints = blackboard.GetValue<List<Vector2>>("PatrolPoints");
		if (patrolPoints == null || patrolPoints.Count == 0)
			return BTState.Failure;
		
		int currentWaypoint = blackboard.GetValue("CurrentWaypoint", 0);
		Vector2 targetPoint = patrolPoints[currentWaypoint];
		
		float distance = character.GlobalPosition.DistanceTo(targetPoint);
		
		if (distance < WaypointReachDistance)
		{
			currentWaypoint = (currentWaypoint + 1) % patrolPoints.Count;
			blackboard.SetValue("CurrentWaypoint", currentWaypoint);
			targetPoint = patrolPoints[currentWaypoint];
		}
		
		Vector2 direction = (targetPoint - character.GlobalPosition).Normalized();
		character.Velocity = direction * PatrolSpeed;
		character.MoveAndSlide();
		
		blackboard.SetValue("CurrentState", "Patrolling");
		
		return BTState.Running;
	}
}
