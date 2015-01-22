using UnityEngine;
using System.Collections;

public partial class EnemyAI : StateMachine {

	void PatrollingStart()
	{
		moveController.Stop ();
	}

	void PatrollingUpdate()
	{
		if (scanner.playerInSight)
			SetState ("AIState_Chasing");
	}
}
