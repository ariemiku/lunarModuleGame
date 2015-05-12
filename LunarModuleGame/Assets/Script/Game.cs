using UnityEngine;
using System.Collections;

using UnityEngine.UI;

// ステータス
enum eStatus{
	ePlay,
	eGameOver,
};

class SpaceShip{
	protected Vector2 position;			// 宇宙船の座標
	protected GameObject mySpaceShip;
	protected float rotationAngle;		// 宇宙船の回転角度

	// コンストラクタ
	public SpaceShip(){
		position.x = 0;
		position.y = 0;
		rotationAngle = 0;

		mySpaceShip = GameObject.Find("SpaceShip");
	}

	public void Initialize(){
		position.x = -2.0f;
		position.y = 2.0f;
		mySpaceShip.transform.localPosition = new Vector3(position.x,position.y,0);
		rotationAngle = 0.0f;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (0,Vector3.forward);
	}

	// 宇宙船の位置をセットする関数
	public void SetPosition(float x, float y){
		position.x = x;
		position.y = y;

		mySpaceShip.transform.localPosition = new Vector3(position.x,position.y,0);
	}

	// 宇宙船の位置を取得する関数
	public Vector2 GetPosition(){
		return position;
	}

	// 宇宙船の回転を行う関数
	public void Rotation(float rotationSpeed){
		rotationAngle = rotationAngle + rotationSpeed;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (rotationAngle%360,Vector3.forward);
	}

	// テスト関数
	public void Test(){
		Debug.Log (mySpaceShip.transform.up);
	}
}

public class Game : MonoBehaviour {
	// 定数
	public static readonly float ROTATION_SPEED = 2.0f;		// 回転速度
	
	private eStatus m_Status;
	public Text gameoverText;

	SpaceShip mySpaceShip;

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
		mySpaceShip = new SpaceShip ();
		mySpaceShip.Initialize();

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

		// leftShiftでブロックを最初の位置に戻す（仮）
		if (Input.GetKeyDown (KeyCode.LeftShift)) {
			mySpaceShip.Initialize();
		}

		// RightArrowで宇宙船を右回転させる
		if (Input.GetKey(KeyCode.RightArrow)) {
			mySpaceShip.Rotation(-ROTATION_SPEED);
		}

		// LeftArrowで宇宙船を左回転させる
		if (Input.GetKey(KeyCode.LeftArrow)) {
			mySpaceShip.Rotation(ROTATION_SPEED);
		}

		// Aでテスト
		if (Input.GetKey(KeyCode.A)) {
			mySpaceShip.Test ();
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


