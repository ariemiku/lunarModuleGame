using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {
	public enum eStage {
		eStage1,
		eStage2,
	};

	private static StageManager s_instance;
	eStage m_stage;

	public SpaceShip m_spaceShip;
	public GameObject m_stage1;
	public GameObject m_stage2;

	void Awake () {
		Game game = GameObject.Find ("spaceship").GetComponent<Game> ();
		m_spaceShip = game.GetMySpaceShip ();
		m_stage1 = GameObject.Find ("lunarSurface1");
		m_stage2 = GameObject.Find ("lunarSurface2");
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		switch (m_stage) {
		case eStage.eStage1:
			break;
		case eStage.eStage2:
			break;
		}
	}

	public static StageManager GetInstance () {
		if (s_instance == null) {
			GameObject gameObject = new GameObject ("gameObject");
			s_instance = gameObject.AddComponent<StageManager> ();
		}

		return s_instance;
	}

	public void Transit (eStage nextStage) {
		switch (nextStage) {
		case eStage.eStage1:
			StartStage1 ();
			m_spaceShip.InitializeStage1 ();
			break;
		case eStage.eStage2:
			StartStage2 ();
			m_spaceShip.InitializeStage2 ();
			break;
		}
		m_stage = nextStage;
	}

	void StartStage1 () {
		m_stage1.transform.position = new Vector2 (0, 0);
		m_stage2.transform.position = new Vector2 (100, 100);
	}

	void StartStage2 () {
		m_stage2.transform.position = new Vector2 (0, 0);
		m_stage1.transform.position = new Vector2 (100, 100);
	}

	public void SetNextStage () {
		switch (m_stage) {
		case eStage.eStage1:
			Transit (eStage.eStage2);
			break;
		case eStage.eStage2:
			Transit (eStage.eStage1);
			break;
		}
	}
}
