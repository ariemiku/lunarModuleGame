using UnityEngine;
using System.Collections;

using UnityEngine.UI;

enum eStatus{
	ePlay,
	eGameOver,
};

public class Game : MonoBehaviour {
	private eStatus m_Status;

	private GameObject mySpaceShip;

	public Text gameoverText;
	
	// Use this for initialization
	void Start () {
		m_Status = eStatus.ePlay;
		Transit (m_Status);
	}
	
	// Update is called once per frame
	void Update () {
		switch (m_Status) {
		case eStatus.ePlay:
			UpdatePlay();
			break;
		case eStatus.eGameOver:
			UpdateGameover();
			break;
		}
	}

	// シーンを代える
	void Transit(eStatus NextStatus){
		switch (NextStatus) {
		case eStatus.ePlay:
			StartPlay(m_Status);
			break;
		case eStatus.eGameOver:
			StartGameover(m_Status);
			break;
		}
		m_Status = NextStatus;
	}


	// play状態の開始関数
	void StartPlay(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		gameoverText = GameObject.Find ("Canvas/TextGameover").GetComponent<Text> ();
		gameoverText.text = "";

		// 宇宙船の初期位置設定
		mySpaceShip = GameObject.Find("SpaceShip");
		mySpaceShip.transform.Translate (1,1,0);
	}

	// Game状態の開始関数
	void StartGameover(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		gameoverText.text = "GAMEOVER push space";
	}

	// Play状態の更新関数
	void UpdatePlay(){
		// RightShiftキーでゲームオーバーに切り替える（仮）
		if(Input.GetKeyDown(KeyCode.RightShift))
		{
			Transit (eStatus.eGameOver);
		}

	}

	// Game状態の更新関数
	void UpdateGameover(){
		// spaceキーでTitleに切り替える
		if(Input.GetKeyDown(KeyCode.Space))
		{
			Application.LoadLevel("Title");
		}
	}



}
