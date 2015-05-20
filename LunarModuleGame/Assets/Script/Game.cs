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
		//moveX = 0.0f;
		m_horizontalSpeed = 0;
		m_verticalSpeed = 0;
		fuelRemaining = MAX_FUEL;

		// コンポーネントの取得
		mySpaceShip = GameObject.Find("spaceship");

		//rigidbody2D = mySpaceShip.GetComponent<Rigidbody2D> ();
		SetVerticalSpeed(0.0f);
		SetVerticalSpeed (0.0f);
	}
	
	public void Initialize(){
		// 初期位置に宇宙船をセット
		//position=INITIALPOSITION;
		//mySpaceShip.transform.localPosition = new Vector3(position.x,position.y,0);

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
		rotationAngle = rotationAngle + rotationSpeed;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (rotationAngle%360,Vector3.forward);
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
		
		// 向いている方向へ移動させる

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

	// 慣性の法則に従って動き続けるx座標の処理
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

	private eStatus m_Status;

	// テキスト
	public GUIText tutorialText;
	public GUIText gameEndText;
	public GUIText fuelRemainingText;
	public GUIText angleText;
	public GUIText landingVelocityText;
	public GUIText checkLandingText;
	public GUIText horizontalSpeedText;
	public GUIText courseTimeText;
	public GUIText stageNumText;
	public GUIText scoreNumText;

	public GameObject m_exclamation;

	public GameObject m_fuel1;

	SpaceShip mySpaceShip;
	Fire fire;

	GameObject explode;

	bool checkPoint;
	bool m_start;

	float m_courseTime;
	int m_stageNum;
	float m_score;
	int m_setFuelCount;
	
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
		case eStatus.eNextStage:
			m_Status = eStatus.ePlay;
			break;
		}
	}
	
	// シーンを代える
	void Transit(eStatus NextStatus){
		switch (NextStatus) {
		case eStatus.eTutorial:
			StartTutorial(m_Status);
			break;
		case eStatus.ePlay:
			StartPlay(m_Status);
			break;
		case eStatus.eGameOver:
			StartGameover(m_Status);
			break;
		case eStatus.eStageClear:
			StartStageClear ();
			break;
		case eStatus.eNextStage:
			StartNextStage ();
			break;
		}
		m_Status = NextStatus;
	}

	// tutorial状態の開始関数
	void StartTutorial(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);

		// テキスト
		gameEndText = GameObject.Find ("Canvas/TextGameEnd").GetComponent<GUIText> ();
		gameEndText.text = "";
		tutorialText = GameObject.Find ("Canvas/TextTutorial").GetComponent<GUIText> ();
		checkLandingText = GameObject.Find ("Canvas/TextCheckLanding").GetComponent<GUIText> ();
		checkLandingText.text = "";
	}
	
	// play状態の開始関数
	void StartPlay(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする
		checkPoint = false;

		// 宇宙船の初期位置設定
		mySpaceShip = new SpaceShip ();
		//mySpaceShip.InitializeStage1 ();
		StageManager.GetInstance ();

		// テキスト
		fuelRemainingText = GameObject.Find ("Canvas/TextFuelRemaining").GetComponent<GUIText> ();
		fuelRemainingText.text = "残りの燃料："+mySpaceShip.GetPercentFuelRemaining();
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
		int minute = (int)(m_courseTime / 60);
		int second1 = (int)(m_courseTime % 60 % 10);
		int second2 = (int)(m_courseTime % 60 / 10);
		courseTimeText = GameObject.Find ("Canvas/TextCourseTime").GetComponent<GUIText> ();
		courseTimeText.text = minute + "：" + second2 + second1;
		stageNumText = GameObject.Find ("Canvas/TextStageNum").GetComponent<GUIText> ();
		stageNumText.text = "ステージ：" + m_stageNum;
		scoreNumText = GameObject.Find ("Canvas/TextScore").GetComponent<GUIText> ();
		scoreNumText.text = "スコア：" + m_score;

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

	// 次のステージの準備.
	void StartNextStage () {
		StageManager.GetInstance ().SetNextStage ();
		m_start = false;
		gameEndText.text = "";
		m_courseTime = 0;
		m_stageNum++;
		m_setFuelCount = 0;
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

		// 燃料がなくなった時に重力によって落下させる
		if (mySpaceShip.GetPercentFuelRemaining () <= 0) {
			Physics2D.gravity=new Vector3(0.0f, GRAVITYMOON, 0.0f);
		}

		// ステータスのテキスト更新
		fuelRemainingText.text = "残りの燃料："+mySpaceShip.GetPercentFuelRemaining()+"%";
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
		int minute = (int)(m_courseTime / 60);
		int second1 = (int)(m_courseTime % 60 % 10);
		int second2 = (int)(m_courseTime % 60 / 10);
		courseTimeText.text = minute + "：" + second2 + second1;
		stageNumText.text = "ステージ：" + m_stageNum;
		scoreNumText.text = "スコア：" + m_score;

		// チェックポイントを過ぎたかつ着陸条件を満たしている場合着陸可能かテキストを出す
		// どちらも満たしていない場合表記しない
		if (!checkPoint) {
			checkLandingText.text = "";
			m_exclamation.transform.position = new Vector2 (100, 100);
		}
		else if (mySpaceShip.Landing ()) {
			checkLandingText.text = "着陸可能";
			m_exclamation.transform.position = new Vector2 (100, 100);
		}
		else {
			checkLandingText.text = "着陸不可";
			Vector2 exclamationPosition = mySpaceShip.GetPosition ();
			m_exclamation.transform.position = new Vector2 (exclamationPosition.x, exclamationPosition.y + 0.5f);
		}
	
	}
	
	// Game状態の更新関数
	void UpdateGameover(){
		// enterキーでTitleに切り替える
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Application.LoadLevel("Title");
		}
	}

	void UpdateStageClear () {
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Transit (eStatus.eNextStage);
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
	
	// 壁や床にぶつかった時に呼び出される
	void OnTriggerEnter2D (Collider2D c){
		// チェックポイントブロックを通過したかの判定を行う
		if (c.gameObject.tag == "CheckPoint") {
			checkPoint = true;
			return;
		}

		if (c.gameObject.tag == "Fuel") {
			mySpaceShip.Supply ();
			c.gameObject.transform.position = new Vector2 (100, 100);
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

		// 重力と落下速度を調節する
		/*Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);
		mySpaceShip.SetVerticalSpeed (0.0f);*/
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
}


