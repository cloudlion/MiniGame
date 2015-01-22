using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour {
	public GameObject bulletPrefab ;
	public Transform spawnPoint ;
	public float frequency = 10f;
	public float coneAngle = 1.5f;
	public float hitSoundVolume = 0.5f;

	private bool firing = false;
	private AudioSource _asource ;
//	GameObject muzzleFlashFront : ;
	
	private float lastFireTime = -1;
	
	void Awake () {

		_asource = GetComponent<AudioSource>();
//		muzzleFlashFront.SetActive (false);
		if (spawnPoint == null)
			spawnPoint = transform;
	}
	

	
	void Update ()
	{
		if (firing) {
			
			if (Time.time > lastFireTime + 1 / frequency) {
				var coneRandomRotation = Quaternion.Euler (Random.Range (-coneAngle, coneAngle), Random.Range (-coneAngle, coneAngle), 0);
				GameObject go  = Spawner.Spawn (bulletPrefab, spawnPoint.position, spawnPoint.rotation * coneRandomRotation) as GameObject;
				Bullet bullet  = go.GetComponent<Bullet> ();	
				lastFireTime = Time.time;
				bullet.dist = 1000;
			}
		}
	}
	
	void OnStartFire () {
		if (Time.timeScale == 0)
			return;
		
		firing = true;
		
//		muzzleFlashFront.SetActive (true);
		
		if (audio)
			audio.Play ();
	}
	
	void OnStopFire () {
		firing = false;
		
//		muzzleFlashFront.SetActive (false);
		if (audio)
			audio.Stop ();
	}
}
