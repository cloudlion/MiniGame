using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float speed = 10f;
	public float lifeTime  = 0.5f;
	public float dist = 10000;
	
	private float spawnTime  = 0.0f;
	private Transform tr ;
	
	void OnEnable () {
		tr = transform;
		spawnTime = Time.time;
	}
	
	void Update () {
		tr.position += tr.forward * speed * Time.deltaTime;
		dist -= speed * Time.deltaTime;
		if (Time.time > spawnTime + lifeTime || dist < 0) {
			Spawner.Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider other) 
	{
		if (other.tag != "AI_Trigger") 
		{
			Debug.Log ("collision with: " + other.gameObject.name);
			Spawner.Destroy (gameObject);
		}
	}

	void OnTriggerStay(Collider other) 
	{
	}
}
