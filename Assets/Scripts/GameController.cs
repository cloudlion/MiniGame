using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {

	public GameObject player;
	public GameObject explosionPrefab;
	public Rect scoreRec;
	public Rect healthRec;
	public Rect restartRec;


	private static GameController _instance;
	private GameObject _explosion;
	private int score = 0;
	private GameObject _deadEnemy;
	private Health playerHealth;

	public static GameController Instance{
		get{
			return _instance;
		}
	}

	// Use this for initialization
	void Awake () {
		if (_instance != null) {
			Debug.LogError( "Duplicate game controller" );
		}

		_instance = this;
	}

	void Start()
	{
		playerHealth = player.GetComponent<Health>( );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDeath( GameObject go)
	{
		if (go == player) {
			player.SetActive(false);
			Time.timeScale = 0;
			return;
		}

		_explosion = Instantiate (explosionPrefab) as GameObject;
		_explosion.transform.position = go.transform.position;

		score++;
		if (_deadEnemy != null)
			_deadEnemy.SetActive (true);
		_deadEnemy = go;
		go.SetActive (false);
	}

	void OnDamage( Health health )
	{

	}

	void OnGUI()
	{
		GUI.Box (scoreRec, "Kill: " + score);

		GUI.Box (healthRec, "Health: " + playerHealth.health);

		if (!player.activeSelf) {
			GUI.Box( new Rect(0, 0, Screen.width, Screen.height), "" );

			if( GUI.Button(restartRec, "Restart") )
			{
				Time.timeScale = 1f;
				Application.LoadLevel(0);
			}
		}
	}
}
