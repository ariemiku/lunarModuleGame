using UnityEngine;
using System.Collections;

using UnityEngine.UI;

// ステータス
public enum eStatus{
	eTutorial,
	ePlay,
	eGameOver,
	eStageClear,
	eNextStage,
	eNameInput,
	eRanking,
};

class Fire{
	public static readonly float DISTANCE = 0.3f; // 宇宙船との距離

	protected GameObject fire;
	protected Vector2 position;			// 炎の座標
	protected float rotationAngle;		// 炎の回転角度

	// コンストラクタ
	public Fire(){
		fire = GameObject.Find ("fire");
		position = new Vector2 (0.0f, 0.0f);
		rotationAngle = 0.0f;
	}

	// ゲームオブジェクトを取得する関数
	public GameObject GetGameObject(){
		return fire;
	}

	// 宇宙船の裏側に位置をセットする関数
	public void BackSetPosition(Vector2 spaceShipPos){
		position.x = spaceShipPos.x;
		position.y = spaceShipPos.y;
		
		fire.transform.localPosition = new Vector3(position.x,position.y,0);
	}

	// 炎の位置をセットする関数
	public void SetPosition(Vector2 pos){
		fire.transform.localPosition = new Vector3(pos.x,pos.y,0);
	}

	// 炎の角度をセットする関数
	public void SetRotationAngle(float rotationA){
		rotationAngle = rotationA;
		fire.transform.rotation = Quaternion.AngleAxis (rotationAngle%360,Vector3.forward);
	}

	// 炎の位置を宇宙船の下に設定する関数
	public void SetFireUnderSpaceShip(GameObject spaceShip,Vector2 centerPos){
		Vector3 pos;
		Vector3 centerPosition = new Vector3 (centerPos.x, centerPos.y, 0.0f);
		
		// 向いている方向へ移動させる
		pos = (spaceShip.transform.TransformDirection (Vector3.down) * DISTANCE)+centerPosition;
		
		pos.z = 0.0f;
		
		SetPosition (pos);
	}
}

// 宇宙船クラス
public class SpaceShip {
	// 定数
	public static readonly float MAX_VELOCITY = -1.0f;				// 落下の最高速度
	public static readonly int MAX_FUEL = 1000;						// 満タン時の燃料
	public static readonly int BUENOUTFUEL = 1;						// 消費燃料
	public static readonly Vector2 INITIALPOSITION = new Vector2(-3.0f,-1.9f);	// 宇宙船の初期位置
	public static readonly float SPEED = 0.0003f;

	protected Vector2 position;			// 宇宙船の座標
	protected GameObject mySpaceShip;
	protected float rotationAngle;		// 宇宙船の回転角度
	protected float m_horizontalSpeed;
	protected float m_verticalSpeed;

	protected int fuelRemaining;		// 宇宙船の燃料
	
	// コンストラクタ
	public SpaceShip(){
		position.x = 0;
		position.y = 0;
		rotationAngle = 0;
		m_horizontalSpeed = 0;
		m_verticalSpeed = 0;
		fuelRemaining = MAX_FUEL;

		// コンポーネントの取得
		mySpaceShip = GameObject.Find("spaceship");

		SetVerticalSpeed(0.0f);
		SetVerticalSpeed (0.0f);
	}
	
	public void Initialize(){
		rotationAngle = 0.0f;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (0,Vector3.forward);
		m_horizontalSpeed = 0;
		m_verticalSpeed = 0;
		fuelRemaining = MAX_FUEL;
	}

	// ゲームオブジェクトを取得する関数
	public GameObject GetGameObject(){
		return mySpaceShip;
	}

	// 宇宙船の位置をセットする関数
	public void SetPosition(Vector2 pos){
		position.x = pos.x;
		position.y = pos.y;
		
		mySpaceShip.transform.localPosition = new Vector3(position.x,position.y,0);
	}
	
	// 宇宙船の位置を取得する関数
	public Vector2 GetPosition(){
		position.x = mySpaceShip.transform.localPosition.x;
		position.y = mySpaceShip.transform.localPosition.y;
		return position;
	}
	
	// 宇宙船の回転を行う関数
	public void Rotation(float rotationSpeed){
		rotationAngle = (rotationAngle + rotationSpeed) % 360;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (rotationAngle,Vector3.forward);
	}

	// 宇宙船の傾きを取得する関数
	public float GetRotation(){
		return rotationAngle;
	}

	// ロケット噴射を行う関数
	public void Jet(){
		Vector2 pos;
		// 現在の位置を取得
		pos.x = mySpaceShip.transform.localPosition.x;
		pos.y = mySpaceShip.transform.localPosition.y;
		
		// スピード更新.
		if (mySpaceShip.transform.up.x > 8.742278E-08 || 
		    mySpaceShip.transform.up.x < -8.742278E-08) {
			m_horizontalSpeed -= mySpaceShip.transform.up.x * SPEED;
		}
		m_verticalSpeed -= mySpaceShip.transform.up.y * SPEED;
		pos.x -= m_horizontalSpeed;
		pos.y -= m_verticalSpeed;

		SetPosition (pos);

		// 燃料を消費する
		UseFuel ();
	}

	// 慣性の法則に従った動きをする関数
	public void MoveXByInertia(){
		Vector2 pos;
		// 現在の位置を取得
		pos.x = mySpaceShip.transform.localPosition.x;
		pos.y = mySpaceShip.transform.localPosition.y;

		// 移動.
		pos.x -= m_horizontalSpeed;
		pos.y -= m_verticalSpeed;

		// 位置を設定する
		SetPosition (pos);
	}
	
	// 落下速度を取得する関数
	public float GetVerticalSpeed(){
		return m_verticalSpeed;
	}

	// 落下速度を設定する関数
	public void SetVerticalSpeed (float verticalSpeed) {
		m_verticalSpeed = verticalSpeed;
	}

	// 落下速度が上限を超えないようにする関数
	public void SetChangeVertical(){
		// 落下速度に上限を設ける
		float verticalSpeed = GetVerticalSpeed();
		
		if (verticalSpeed < MAX_VELOCITY) {
			verticalSpeed = MAX_VELOCITY;
			SetVerticalSpeed(verticalSpeed);
		}
	}

	// 着地が成功かどうか判定を行い成功の場合trueを返す関数
	public bool Landing(){
		// 傾き.
		if ((rotationAngle > 4.0f && rotationAngle < 356.0f) || 
		    (rotationAngle < -4.0f && rotationAngle > -356.0f)) {
			return false;
		}
	
		// 水平移動速度チェック.
		if (m_horizontalSpeed > 0.005 || m_horizontalSpeed < -0.005) {
			return false;
		}

		// 現在の落下速度を取得
		float verticalSpeed = -GetVerticalSpeed();
		// 落下速度チェック.
 		if (verticalSpeed < -0.005){
			return false;
		}

		return true;
	}

	// 残りの燃料を取得する関数
	public int GetFuelRemaining(){
		return fuelRemaining;
	}

	// 燃料を消費する関数
	public void UseFuel(){
		fuelRemaining -= BUENOUTFUEL;
	}

	// 残りの燃料をパーセントで返す関数
	public int GetPercentFuelRemaining(){
		float percent;
		percent = (float)fuelRemaining / (float)MAX_FUEL;
		return  (int)(percent * 100);
	}

	// 平行スピード取得.
	public float GetHorizontalSpeed () {
		return m_horizontalSpeed;
	}

	// 傾き取得.
	public float GetRotationAngle () {
		return rotationAngle;
	}

	// 補給.
	public void Supply () {
		fuelRemaining += 600;
	}
}

public class Game : MonoBehaviour {
	// 定数
	public static readonly float ROTATION_SPEED = 2.0f;				// 回転速度
	public static readonly float GRAVITYMOON = -0.5f;				// 重力
	public static readonly float CORRECTIONTOLOOKVEROCITY = -10000;	// 速度を表示するにあたって補正する値
	public static readonly float CURSOR_Y_DIFFERENCE = 0.2f;		// 燃料ゲージカーソルのy座標の差
	public static readonly float CURSORSTICK_MINPOS_X = 1.525f;		// 0%の時のカーソルのx座標
	public static readonly Vector2 CURSORSTICK_INITIALPOS = new Vector2 (3.006f,-2.59f);	// カーソルの初期位置（100％）

	private eStatus m_Status;

	// テキスト
	public GUIText tutorialText;
	public GUIText gameEndText;
	public GUIText angleText;
	public GUIText landingVelocityText;
	public GUIText horizontalSpeedText;
	public GUIText courseTimeText;
	public GUIText stageNumText;
	public GUIText scoreNumText;
	public GUIText rankingText;
	public GUIText myScoreText;

	public GameObject m_exclamation;
	public GameObject m_black;
	public GameObject m_newRecord;

	public GameObject m_fuel1;

	// 燃料ゲージ
	public GameObject m_gauge;
	public GameObject m_gaugeOver;
	public GameObject m_cursorStick;
	public GameObject m_cursor;
	public Vector2 m_cursorPos;
	
	SpaceShip mySpaceShip;
	Fire fire;

	GameObject explode;

	bool checkPoint;
	bool m_start;
	bool m_rankIn;

	float m_courseTime;
	int m_stageNum;
	int m_score;
	int m_setFuelCount;
	string m_playerName = "";
	
	// Use this for initialization
	void Start () {
		m_Status = eStatus.eTutorial;
		Transit (m_Status);
	}
	
	// Update is called once per frame
	void Update () {
		switch (m_Status) {
		case eStatus.eTutorial:
			UpdateTutorial();
			break;
		case eStatus.ePlay:
			UpdatePlay();
			break;
		case eStatus.eGameOver:
			UpdateGameover();
			break;
		case eStatus.eStageClear:
			UpdateStageClear ();
			break;
		case eStatus.eRanking:
			UpdateRanking ();
			break;
		case eStatus.eNextStage:
			m_Status = eStatus.ePlay;
			break;
		case eStatus.eNameInput:
			UpdateNameInput ();
			break;
		}
	}
	
	// シーンを代える
	void Transit(eStatus NextStatus){
		switch (NextStatus) {
		case eStatus.eTutorial:
			m_Status = NextStatus;
			StartTutorial(m_Status);
			break;
		case eStatus.ePlay:
			m_Status = NextStatus;
			StartPlay(m_Status);
			break;
		case eStatus.eGameOver:
			m_Status = NextStatus;
			StartGameover(m_Status);
			break;
		case eStatus.eStageClear:
			m_Status = NextStatus;
			StartStageClear ();
			break;
		case eStatus.eRanking:
			m_Status = NextStatus;
			StartRanking ();
			break;
		case eStatus.eNextStage:
			m_Status = NextStatus;
			StartNextStage ();
			break;
		case eStatus.eNameInput:
			m_Status = NextStatus;
			StartNameInput ();
			break;
		}
	}

	// tutorial状態の開始関数
	void StartTutorial(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);

		// テキスト
		gameEndText = GameObject.Find ("Canvas/TextGameEnd").GetComponent<GUIText> ();
		gameEndText.text = "";
		tutorialText = GameObject.Find ("Canvas/TextTutorial").GetComponent<GUIText> ();
	}

	// play状態の開始関数
	void StartPlay(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		checkPoint = false;

		// 宇宙船の初期位置設定
		mySpaceShip = new SpaceShip ();
		StageManager.GetInstance ();

		// 燃料ゲージの初期設定
		m_gauge = GameObject.Find ("gauge");
		m_gauge.transform.position = new Vector3 (2.7f,-2.687f);
		m_gaugeOver = GameObject.Find ("gauge_over");
		m_gaugeOver.transform.position = new Vector3 (3.443f,-2.634f);
		m_cursorStick = GameObject.Find ("cursorStick");
		m_cursorPos = CURSORSTICK_INITIALPOS;
		m_cursorStick.transform.position = m_cursorPos;
		m_cursor = GameObject.Find ("cursor");
		m_cursor.transform.position = new Vector2(m_cursorPos.x,m_cursorPos.y+CURSOR_Y_DIFFERENCE);

		// テキスト
		angleText = GameObject.Find ("Canvas/TextAngle").GetComponent<GUIText> ();
		angleText.text = "機体の傾き：" + mySpaceShip.GetRotation ();
		landingVelocityText = GameObject.Find ("Canvas/TextLandingVelocity").GetComponent<GUIText> ();
		landingVelocityText.text = "落下速度：" + (mySpaceShip.GetVerticalSpeed () * CORRECTIONTOLOOKVEROCITY);
		horizontalSpeedText = GameObject.Find ("Canvas/TextHorizontalSpeed").GetComponent<GUIText> ();
		if ((mySpaceShip.GetGameObject ().transform.up.x != 8.742278E-08 || 
		     mySpaceShip.GetGameObject ().transform.up.x != -8.742278E-08) && 
		    mySpaceShip.GetHorizontalSpeed () != 0) {
			horizontalSpeedText.text = "水平速度：" + (mySpaceShip.GetHorizontalSpeed () * CORRECTIONTOLOOKVEROCITY);
		}
		else {
			horizontalSpeedText.text = "水平速度：" + 0;
		}
		// 時間 （単位：秒）小数第2位まで表示
		courseTimeText = GameObject.Find ("Canvas/TextCourseTime").GetComponent<GUIText> ();
		courseTimeText.text = m_courseTime.ToString ("f2");

		stageNumText = GameObject.Find ("Canvas/TextStageNum").GetComponent<GUIText> ();
		stageNumText.text = "ステージ：" + m_stageNum;
		scoreNumText = GameObject.Find ("Canvas/TextScore").GetComponent<GUIText> ();
		scoreNumText.text = "スコア：" + m_score;
		rankingText = GameObject.Find ("Canvas/TextRanking").GetComponent<GUIText> ();
		rankingText.text = "";
		myScoreText = GameObject.Find ("Canvas/TextMyScore").GetComponent<GUIText> ();
		myScoreText.text = "";

		m_fuel1 = GameObject.Find ("Fuel/Fuel1");

		fire = new Fire ();
		fire.BackSetPosition (mySpaceShip.GetPosition ());

		explode = GameObject.Find ("explode");

		m_start = false;
		m_courseTime = 0;
		m_stageNum = 1;
		m_score = 0;
		m_setFuelCount = 0;
		StageManager.GetInstance ().Transit (StageManager.eStage.eStage1);
	}

	// Game状態の開始関数
	void StartGameover(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
	}

	void StartStageClear () {

	}

	void StartNameInput () {
		// 消す.
		gameEndText.text = "";
		angleText.text = "";
		horizontalSpeedText.text = "";
		landingVelocityText.text = "";
		courseTimeText.text = "";
		stageNumText.text = "";
		scoreNumText.text = "";
		m_black.transform.position = new Vector2 (0, 0);

		m_rankIn = false;
		Ranking.GetInstance ().SetRankingDate ("Score_Ranking");

		if (Ranking.GetInstance ().GetScore () [9] <= m_score) {
			m_rankIn = true;
			m_newRecord.transform.position = new Vector2 (0, 0.7f);
		}
		else {
			Transit (eStatus.eRanking);
		}
	}

	void StartRanking () {
		m_newRecord.transform.position = new Vector2 (100, 100);
		string fileName = "Score_Ranking";
		m_rankIn = false;
		Ranking.GetInstance ().WriteRanking (m_score, fileName, m_playerName);
		int rankingMaxNum = Ranking.GetInstance ().GetRankingMaxNum ();
		string[] rankingScore = new string[rankingMaxNum];
		string[] rankingName = new string[rankingMaxNum];
		for (int i = 0; i < rankingMaxNum; i++) {
			rankingScore[i] = Ranking.GetInstance ().GetScore ()[i].ToString ();
			if(int.Parse (rankingScore[i]) < 1000) {
				rankingScore[i] = "  " + rankingScore[i];
			}
			rankingName[i] = Ranking.GetInstance ().GetName ()[i].ToString ();
		}
		string[] rankingNum = new string[rankingMaxNum];
		rankingNum[0] = "　　１位　";
		rankingNum[1] = "　　２位　";
		rankingNum[2] = "　　３位　";
		rankingNum[3] = "　　４位　";
		rankingNum[4] = "　　５位　";
		rankingNum[5] = "　　６位　";
		rankingNum[6] = "　　７位　";
		rankingNum[7] = "　　８位　";
		rankingNum[8] = "　　９位　";
		rankingNum[9] = "　１０位　";

		string[] rankingPoint = new string[rankingMaxNum];
		rankingPoint[0] = "点　";
		rankingPoint[1] = "点　";
		rankingPoint[2] = "点　";
		rankingPoint[3] = "点　";
		rankingPoint[4] = "点　";
		rankingPoint[5] = "点　";
		rankingPoint[6] = "点　";
		rankingPoint[7] = "点　";
		rankingPoint[8] = "点　";
		rankingPoint[9] = "点　";

		// ランクインした自分の順位を赤色で表示
		for (int i = 0; i < rankingMaxNum; i++) {
			if(m_score == Ranking.GetInstance ().GetScore ()[i]) {
				myScoreText.text = rankingNum[i] + rankingScore[i] + "点　" + rankingName[i];
				myScoreText.transform.position = new Vector2 (0.1f, 0.797f - (0.077f * i));
				myScoreText.color = Color.red;
				rankingScore[i] = "";
				rankingPoint[i] = "";
				rankingName[i] = "";
				rankingNum[i] = "";
				break;
			}
		}

		// ランキングを表示
		rankingText.text =
			"　　　　　ランキング\n\n" +
			rankingNum [0] + rankingScore [0] + rankingPoint[0] + rankingName [0] + "\n" +
			rankingNum [1] + rankingScore [1] + rankingPoint[1] + rankingName [1] + "\n" +
			rankingNum [2] + rankingScore [2] + rankingPoint[2] + rankingName [2] + "\n" +
			rankingNum [3] + rankingScore [3] + rankingPoint[3] + rankingName [3] + "\n" +
			rankingNum [4] + rankingScore [4] + rankingPoint[4] + rankingName [4] + "\n" +
			rankingNum [5] + rankingScore [5] + rankingPoint[5] + rankingName [5] + "\n" +
			rankingNum [6] + rankingScore [6] + rankingPoint[6] + rankingName [6] + "\n" +
			rankingNum [7] + rankingScore [7] + rankingPoint[7] + rankingName [7] + "\n" +
			rankingNum [8] + rankingScore [8] + rankingPoint[8] + rankingName [8] + "\n" +
			rankingNum [9] + rankingScore [9] + rankingPoint[9] + rankingName [9] + "\n";
	}

	// 次のステージの準備.
	void StartNextStage () {
		if (StageManager.GetInstance ().CheckLastStage ()) {
			Transit (eStatus.eNameInput);
			return;
		}
		StageManager.GetInstance ().SetNextStage ();
		m_start = false;
		gameEndText.text = "";
		rankingText.text = "";
		myScoreText.text = "";
		m_courseTime = 0;
		m_stageNum++;
		m_setFuelCount = 0;

		// カーソルを100％の位置に戻す
		m_cursorPos = CURSORSTICK_INITIALPOS;
		m_cursorStick.transform.position = m_cursorPos;
		m_cursor.transform.position = new Vector2(m_cursorPos.x,m_cursorPos.y+CURSOR_Y_DIFFERENCE);
		// 100％以上のゲージを暗くしておく
		SetSortingOrder (m_gaugeOver,4);
	}

	// tutorial状態の更新関数
	void UpdateTutorial(){
		// enterキーでゲームをスタートさせる
		if (Input.GetKeyDown (KeyCode.Return)) {
			// 操作説明のテキストを削除する
			Destroy(tutorialText);
			Transit (eStatus.ePlay);
		}
	}

	// Play状態の更新関数
	void UpdatePlay(){
		// RightArrowで宇宙船を右回転させる
		if (Input.GetKey(KeyCode.RightArrow)) {
			mySpaceShip.Rotation(-ROTATION_SPEED);
			fire.SetRotationAngle (mySpaceShip.GetRotation ());
		}
		
		// LeftArrowで宇宙船を左回転させる
		if (Input.GetKey(KeyCode.LeftArrow)) {
			mySpaceShip.Rotation(ROTATION_SPEED);
			fire.SetRotationAngle (mySpaceShip.GetRotation ());
		}
		
		// Spaceで噴射
		if (Input.GetKey (KeyCode.Space) && mySpaceShip.GetFuelRemaining() > 0) {
			mySpaceShip.Jet ();

			// 炎の傾きをセットする
			fire.SetRotationAngle (mySpaceShip.GetRotation ());
			// 炎の位置を宇宙船の下に設定する
			fire.SetFireUnderSpaceShip (mySpaceShip.GetGameObject(),mySpaceShip.GetPosition());

			// 残りの燃料に合わせたカーソルの移動をする
			MoveCursor();

			m_start = true;
		}
		else {
			// 慣性の法則に従ってxを移動させる
			mySpaceShip.MoveXByInertia ();
			mySpaceShip.SetChangeVertical();

			fire.BackSetPosition(mySpaceShip.GetPosition ());
		}

		if (m_start) {
			m_courseTime += Time.deltaTime;
			float verticalSpeed = mySpaceShip.GetVerticalSpeed () + 0.0001f;
			mySpaceShip.SetVerticalSpeed (verticalSpeed);
		}

		// ステータスのテキスト更新
		angleText.text = "機体の傾き：" + mySpaceShip.GetRotation ()%360;
		landingVelocityText.text = "落下速度：" + mySpaceShip.GetVerticalSpeed () * CORRECTIONTOLOOKVEROCITY;
		if ((mySpaceShip.GetGameObject ().transform.up.x != 8.742278E-08 || 
		    mySpaceShip.GetGameObject ().transform.up.x != -8.742278E-08) && 
		    mySpaceShip.GetHorizontalSpeed () != 0) {
			horizontalSpeedText.text = "水平速度：" + (mySpaceShip.GetHorizontalSpeed () * CORRECTIONTOLOOKVEROCITY);
		}
		else {
 			horizontalSpeedText.text = "水平速度：" + 0;
		}
		// 時間 （単位：秒）小数第2位まで表示
		courseTimeText.text = m_courseTime.ToString ("f2");
		stageNumText.text = "ステージ：" + m_stageNum;
		scoreNumText.text = "スコア：" + m_score;

		// チェックポイントを過ぎた場合 着陸条件を1つでも満たせていなければ"!"を頭上に出す
		if (!checkPoint) {
			m_exclamation.transform.position = new Vector2 (100, 100);
		}
		else if (mySpaceShip.Landing ()) {
			m_exclamation.transform.position = new Vector2 (100, 100);
		}
		else {
			Vector2 exclamationPosition = mySpaceShip.GetPosition ();
			m_exclamation.transform.position = new Vector2 (exclamationPosition.x, exclamationPosition.y + 0.5f);
		}
	
	}
	
	// Game状態の更新関数
	void UpdateGameover(){
		// enterキーでTitleに切り替える
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Transit (eStatus.eNameInput);
		}
	}

	void UpdateStageClear () {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Transit (eStatus.eNextStage);
		}
	}

	void UpdateNameInput () {
		if(Input.GetKeyDown(KeyCode.Return) && m_playerName != "")
		{
			Transit (eStatus.eRanking);
		}
	}

	void UpdateRanking () {
		// enterキーでTitleに切り替える
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Application.LoadLevel("Title");
		}
	}

	// スコアの計算
	void ComputeScore(){
		// 着陸速度が小さいほど高得点
		// 水平スピードボーナス.
		int horizontalSpeedBonus = (int)(mySpaceShip.GetHorizontalSpeed () * 10000);
		if (horizontalSpeedBonus < 0) {
			horizontalSpeedBonus *= -1;
		}
		horizontalSpeedBonus = 40 - horizontalSpeedBonus;

		// 垂直スピードボーナス.
		int verticalSpeedBonus = (int)(50 - mySpaceShip.GetVerticalSpeed () * 10000);

		// 傾きボーナス.
		int tiltBonus = (int)(mySpaceShip.GetRotationAngle () * 10);
		if (tiltBonus < 0) {
			tiltBonus *= -1;
		}
		tiltBonus = 40 - tiltBonus;

		// スピードボーナス.
		int speedBonus = 0;
		if (m_courseTime < 60) {
			speedBonus = (int)(120 - (m_courseTime * 2));
		}

		int score = horizontalSpeedBonus + verticalSpeedBonus + tiltBonus + speedBonus;

		// 残りの燃料に伴ってボーナス点を加算
		int percent = mySpaceShip.GetPercentFuelRemaining ();
		score += percent;

		m_score += score;
	}

	// 爆発の位置をセットする関数
	public void SetExplodePos(Vector2 pos){
		explode.transform.localPosition = new Vector3(pos.x,pos.y,0);
	}

	// 描画の順番を変更する関数
	public void SetSortingOrder(GameObject gameObject, int num){
		Renderer renderer = gameObject.GetComponent<Renderer> ();
		renderer.sortingOrder = num;
	}

	// 残りの燃料に合わせたカーソルの移動をする関数
	void MoveCursor(){
		if(mySpaceShip.GetPercentFuelRemaining()%1==0 && m_cursorPos.x > CURSORSTICK_MINPOS_X){
			m_cursorPos.x -= (CURSORSTICK_INITIALPOS.x - CURSORSTICK_MINPOS_X)/1000;
			m_cursorStick.transform.position=m_cursorPos;
			m_cursor.transform.position = new Vector2(m_cursorPos.x,m_cursorPos.y+CURSOR_Y_DIFFERENCE);
		}
	}

	// 壁や床にぶつかった時に呼び出される
	void OnTriggerEnter2D (Collider2D c){
		// チェックポイントブロックを通過したかの判定を行う
		if (c.gameObject.tag == "CheckPoint") {
			checkPoint = true;
			return;
		}

		// 燃料を取得した場合
		if (c.gameObject.tag == "Fuel") {
			mySpaceShip.Supply ();
			c.gameObject.transform.position = new Vector2 (100, 100);


			// 100%を超えた場合のみ暗転させているゲージを裏へ
			if(mySpaceShip.GetPercentFuelRemaining() > 100){
				SetSortingOrder(m_gaugeOver,0);
			}
			// カーソルの位置を計算しなおす
			m_cursorPos.x = CURSORSTICK_MINPOS_X + mySpaceShip.GetPercentFuelRemaining()*
				((CURSORSTICK_INITIALPOS.x - CURSORSTICK_MINPOS_X)/100);
			m_cursorStick.transform.position = m_cursorPos;
			m_cursor.transform.position = new Vector2(m_cursorPos.x,m_cursorPos.y+CURSOR_Y_DIFFERENCE);

		}

		// ステージ(壁や地面)にぶつかった場合
		if (c.gameObject.tag == "Stage") {
			gameEndText.text = "GAMEOVER\nYoreScore\n\nStage：" + m_stageNum + "\nScore：" + m_score + "\n\npush Enter";

			// 宇宙船と炎を裏側にセットする
			SetSortingOrder (mySpaceShip.GetGameObject (), 0);
			SetSortingOrder (fire.GetGameObject (), 0);
			
			// 爆発を宇宙船のあった場所にセット
			SetExplodePos (mySpaceShip.GetPosition ());
			// 爆発が見えるように前に描画
			SetSortingOrder (explode, 3);

			// ゲームオーバーに切り替える
			Transit (eStatus.eGameOver);
		}

		// 着地ポイントに着地した場合、着地成功かの判定を行う
		if(c.gameObject.tag == "LandPoint") {
			// 着地が成功した場合
			if(mySpaceShip.Landing()){
				fire.GetGameObject ().transform.position = new Vector2 (100, 100);
				// スコアの表示もここで行う
				ComputeScore ();
				gameEndText.text = "GameComplete\nscore："+ m_score +"\npush Enter";
				Transit (eStatus.eStageClear);
			}
			else{
				gameEndText.text = "GAMEOVER\n\nYoreScore\nStage：" + m_stageNum + "\nScore：" + m_score + "\n\npush Enter";

				// 宇宙船と炎を裏側にセットする
				SetSortingOrder (mySpaceShip.GetGameObject (), 0);
				SetSortingOrder (fire.GetGameObject (), 0);
				
				// 爆発を宇宙船のあった場所にセット
				SetExplodePos (mySpaceShip.GetPosition ());
				// 爆発が見えるように前に描画
				SetSortingOrder (explode, 3);

				// ゲームオーバーに切り替える
				Transit (eStatus.eGameOver);
			}
		}
	}

	void OnTriggerExit2D (Collider2D c) {
		// チェックポイントブロックを通過したかの判定を行う
		if (c.gameObject.tag == "CheckPoint") {
			checkPoint = false;
			return;
		}
	}

	// スペースシップ取得.
	public SpaceShip GetMySpaceShip () {
		return mySpaceShip;
	}

	// ステータスの取得.
	public eStatus GetStatus () {
		return m_Status;
	}

	// 燃料の設置.
	public void SetFuel (Vector2 position) {
		switch (m_setFuelCount) {
		case 0:
			m_fuel1.transform.position = position;
			break;
		}
	}

	void OnGUI () {
		if (m_rankIn) {
			Vector2 screenSize = new Vector2 (Screen.width, Screen.height);
			float scale = screenSize.x / 800;
			if(scale > screenSize.y / 600) {
				scale = screenSize.y / 600;
			}
			Rect rect = new Rect ((int)297 * scale, (int)400 * scale, (int)205 * scale, (int)50 * scale);
			GUI.skin.textField.fontSize = (int)(40 * scale);
			m_playerName = GUI.TextField (rect, m_playerName, 5);
		}
	}
}


