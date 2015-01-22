﻿using UnityEngine;
using System.Collections;

public class MoveController : MonoBehaviour {

//	public Transform target;
	private NavMeshAgent nav;	
	// Use this for initialization
	void Awake () {
	
		nav = GetComponent<NavMeshAgent> ();
//		nav.SetDestination (target.position);
	}
	
	// Update is called once per frame
	void Update () {
	

	}
	
	public void MoveTo(Vector3 position)
	{
		nav.destination = position;
	}

	public void Stop()
	{
		nav.Stop ();
	}

	public bool IsStopped
	{
		get{
			return ( transform.position - nav.destination).magnitude < nav.stoppingDistance;
		}
	}
}
