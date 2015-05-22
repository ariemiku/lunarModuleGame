using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {
	public enum eStage {
		eStage1,
		eStage2,
		eStage3,
		eStage4,
	};

	readonly Vector2 stage1InitializePosition = new Vector2 (-3.0f,-1.9f);
	readonly Vector2 stage2InitializePosition = new Vector2 (-0.1f,-1.5f);
	readonly Vector2 stage3InitializePosition = new Vector2 (-2.3f,-2.2f);
	readonly Vector2 stage4InitializePosition = new Vector2 (-2.3f,-2.2f);

	private static StageManager s_instance;
	eStage m_stage;

	Game m_game;
	SpaceShip m_spaceShip;
	GameObject m_stage1;
	GameObject m_stage2;

	bool m_lastStage = false;

	void Awake () {
		m_game = GameObject.Find ("spaceship").GetComponent<Game> ();
		m_spaceShip = m_game.GetMySpaceShip ();
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
		case eStage.eStage3:
			break;
		case eStage.eStage4:
			break;
		}
	}

	public static StageManager GetInstance () {
		if (s_instance == null) {
			GameObject gameObject = new GameObject ("StageManager");
			s_instance = gameObject.AddComponent<StageManager> ();
		}

		return s_instance;
	}

	public void Transit (eStage nextStage) {
		switch (nextStage) {
		case eStage.eStage1:
			StartStage1 ();
			break;
		case eStage.eStage2:
			StartStage2 ();
			break;
		case eStage.eStage3:
			StartStage3 ();
			break;
		case eStage.eStage4:
			StartStage4 ();
			break;
		}
		m_stage = nextStage;
	}

	void StartStage1 () {
		m_stage1.transform.position = new Vector2 (0, 0);
		m_stage2.transform.position = new Vector2 (100, 100);
		m_game.SetFuel (new Vector2 (0.2f, 0));
		m_spaceShip.Initialize ();
		m_spaceShip.SetPosition (stage1InitializePosition);
	}

	void StartStage2 () {
		m_game.SetFuel (new Vector2 (-3, 2));
		m_spaceShip.Initialize ();
		m_spaceShip.SetPosition (stage2InitializePosition);
	}

	void StartStage3 () {
		m_stage2.transform.position = new Vector2 (0, 0);
		m_stage1.transform.position = new Vector2 (100, 100);
		m_game.SetFuel (new Vector2 (-3, 2));
		m_spaceShip.Initialize ();
		m_spaceShip.SetPosition (stage3InitializePosition);
	}

	void StartStage4 () {
		m_game.SetFuel (new Vector2 (-0.06f, 0.14f));
		m_spaceShip.Initialize ();
		m_spaceShip.SetPosition (stage4InitializePosition);
		m_lastStage = true;
	}

	public void SetNextStage () {
		switch (m_stage) {
		case eStage.eStage1:
			Transit (eStage.eStage2);
			break;
		case eStage.eStage2:
			Transit (eStage.eStage3);
			break;
		case eStage.eStage3:
			Transit (eStage.eStage4);
			break;
		case eStage.eStage4:
			break;
		}
	}

	public bool CheckLastStage () {
		return m_lastStage;
	}
}
