using UnityEngine;
using System.Collections;

using UnityEngine.UI;

// ステータス
enum eStatus{
	ePlay,
	eGameOver,
};

class Fire{
	public static readonly float DIFFERENCE_POSX = 0.05f;	// 宇宙船のX座標との差
	public static readonly float DIFFERENCE_POSY = -0.31f;	// 宇宙船のY座標との差

	protected GameObject fire;
	protected Vector2 position;
	protected float rotationAngle;		// 回転角度

	// コンストラクタ
	public Fire(){
		fire = GameObject.Find ("fire");
		position = new Vector2 (0.0f, 0.0f);
		rotationAngle = 0.0f;
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

	// テストを行う関数
	public void Test(){

	}

	// 炎の位置を宇宙船の下に設定する関数
	public void SetFireUnderSpaceShip(GameObject spaceShip,Vector2 centerPos){
		Vector3 pos;
		Vector3 centerPosition = new Vector3 (centerPos.x, centerPos.y, 0.0f);
		
		// 向いている方向へ移動させる
		pos = (spaceShip.transform.TransformDirection (Vector3.down) * 0.3f)+centerPosition;
		
		pos.z = 0.0f;
		
		SetPosition (pos);
	}
}

// 宇宙船クラス
class SpaceShip{
	// 定数
	public static readonly float MAX_XSPEED = 0.03f;				// 左右移動の最高速度
	public static readonly float MAX_VELOCITY = -1.0f;				// 落下の最高速度
	public static readonly int MAX_FUEL = 500;						// 満タン時の燃料
	public static readonly int BUENOUTFUEL = 1;						// 消費燃料
	public static readonly Vector2 INITIALPOSITION = new Vector2(-3.0f,-1.9f);	// 宇宙船の初期位置

	protected Vector2 position;			// 宇宙船の座標
	protected GameObject mySpaceShip;
	protected float rotationAngle;		// 宇宙船の回転角度
	protected float moveX;

	protected int fuelRemaining;		// 宇宙船の燃料

	protected Rigidbody2D rigidbody2D;	// 落下速度
	
	// コンストラクタ
	public SpaceShip(){
		position.x = 0;
		position.y = 0;
		rotationAngle = 0;
		moveX = 0.0f;
		fuelRemaining = MAX_FUEL;

		// コンポーネントの取得
		mySpaceShip = GameObject.Find("spaceship");

		rigidbody2D = mySpaceShip.GetComponent<Rigidbody2D> ();
		SetVelocity(new Vector3 (0.0f, 0.0f, 0.0f));
	}
	
	public void Initialize(){
		// 初期位置に宇宙船をセット
		position=INITIALPOSITION;
		mySpaceShip.transform.localPosition = new Vector3(position.x,position.y,0);

		rotationAngle = 0.0f;
		mySpaceShip.transform.rotation = Quaternion.AngleAxis (0,Vector3.forward);
		moveX = 0.0f;
	}

	public GameObject GetGameObject(){
		return mySpaceShip;
	}

	// 宇宙船の位置をセットする関数
	public void SetPosition(float x, float y){
		position.x = x;
		position.y = y;
		
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
		Vector3 pos;
		// 現在の位置を取得
		pos.x = mySpaceShip.transform.localPosition.x;
		pos.y = mySpaceShip.transform.localPosition.y;
		pos.z = 0.0f;
		
		// 向いている方向へ移動させる
		pos += mySpaceShip.transform.TransformDirection (Vector3.up) * MAX_XSPEED;

		// Xの移動量を記憶しておく
		moveX = pos.x - mySpaceShip.transform.localPosition.x;

		SetPosition (pos.x,pos.y);

		// 燃料を消費する
		UseFuel ();
	}

	// テスト関数
	public void Test(){
	}

	// 慣性の法則に従って動き続けるx座標の処理
	public void MoveXByInertia(){
		Vector3 pos;
		// 現在の位置を取得
		pos.x = mySpaceShip.transform.localPosition.x;
		pos.y = mySpaceShip.transform.localPosition.y;
		pos.z = 0.0f;
		
		// ロケット噴射をした時に記憶しておいたXの移動値を加算する
		pos.x += moveX;

		// 位置を設定する
		SetPosition (pos.x,pos.y);
	}
	
	// 落下速度を取得する関数
	public Vector3 GetVelocity(){
		rigidbody2D = mySpaceShip.GetComponent<Rigidbody2D> ();
		return rigidbody2D.velocity;
	}

	// 着陸時の最高落下速度の絶対値を取得する関数
	public float GetLandingMaxVelocity(){
		return (-MAX_VELOCITY)/2;
	}
	
	// 落下速度を設定する関数
	public void SetVelocity(Vector3 velocity){
		rigidbody2D.velocity = velocity;
	}

	// 落下速度が上限を超えないようにする関数
	public void SetChangeVelocity(){
		// 落下速度に上限を設ける
		Vector3 nowSpeed = GetVelocity();
		
		if (nowSpeed.y < MAX_VELOCITY) {
			nowSpeed.y = MAX_VELOCITY;
			SetVelocity(nowSpeed);
		}
	}

	// 着地が成功かどうか判定を行い成功の場合trueを返す関数
	public bool Landing(){
		// 機体が水平でない場合falseを返す
		if (rotationAngle >= 1.0f || rotationAngle <= -1.0f) {
			return false;
		}
	
		// 着陸の水平移動速度が最高速度の10分の1よりも早い場合falseを返す
		if(moveX > (MAX_XSPEED/10) || moveX < -(MAX_XSPEED/10)){
			return false;
		}

		// 現在の落下速度を取得
		Vector3 nowSpeed = GetVelocity();
		// 着陸の落下速度が最高落下速度の2分の1よりも早い場合falseを返す
		if (nowSpeed.y <= (MAX_VELOCITY/2)){
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
}

public class Game : MonoBehaviour {
	// 定数
	public static readonly float ROTATION_SPEED = 2.0f;				// 回転速度
	public static readonly float GRAVITYMOON = -0.5f;				// 重力
	public static readonly float CORRECTIONTOLOOKVEROCITY = -100;	// 速度を表示するにあたって補正する値

	private eStatus m_Status;
	public Text gameEndText;
	public Text fuelRemainingText;
	public Text angleText;
	public Text landingVelocityText;

	SpaceShip mySpaceShip;

	Fire fire;
	
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
		Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);

		gameEndText = GameObject.Find ("Canvas/TextGameEnd").GetComponent<Text> ();
		gameEndText.text = "";

		// 宇宙船の初期位置設定
		mySpaceShip = new SpaceShip ();
		mySpaceShip.Initialize();

		fuelRemainingText = GameObject.Find ("Canvas/TextFuelRemaining").GetComponent<Text> ();
		fuelRemainingText.text = "残りの燃料："+mySpaceShip.GetPercentFuelRemaining();

		angleText = GameObject.Find ("Canvas/TextAngle").GetComponent<Text> ();
		angleText.text = "機体の傾き：" + mySpaceShip.GetRotation ();

		landingVelocityText = GameObject.Find ("Canvas/TextLandingVelocity").GetComponent<Text> ();
		landingVelocityText.text = "機体の落下速度：" + (mySpaceShip.GetVelocity ().y * CORRECTIONTOLOOKVEROCITY);

		fire = new Fire ();
		fire.BackSetPosition (mySpaceShip.GetPosition ());
	}

	// Game状態の開始関数
	void StartGameover(eStatus PrevStatus){
		// 代わった時に1回しかやらないことをする

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

		// ロケット噴射時に重力を0に設定する
		if (Input.GetKeyDown (KeyCode.Space) && mySpaceShip.GetFuelRemaining() > 0) {
			Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);
			mySpaceShip.SetVelocity(new Vector3 (0.0f, 0.0f, 0.0f));
			mySpaceShip.Test();
		}
		
		// Spaceで噴射
		if (Input.GetKey (KeyCode.Space) && mySpaceShip.GetFuelRemaining() > 0) {
			mySpaceShip.Jet ();

			// 炎の傾きをセットする
			fire.SetRotationAngle (mySpaceShip.GetRotation ());
			// 炎の位置を宇宙船の下に設定する
			fire.SetFireUnderSpaceShip (mySpaceShip.GetGameObject(),mySpaceShip.GetPosition());
		}
		else {
			// 慣性の法則に従ってxを移動させる
			mySpaceShip.MoveXByInertia ();
			mySpaceShip.SetChangeVelocity();

			fire.BackSetPosition(mySpaceShip.GetPosition ());
		}

		// 重力と落下速度を調節する
		if (Input.GetKeyUp (KeyCode.Space) && mySpaceShip.GetFuelRemaining() > 0) {
			Physics2D.gravity=new Vector3(0.0f, GRAVITYMOON, 0.0f);

			mySpaceShip.SetVelocity(new Vector3 (0.0f, 0.0f, 0.0f));
		}
		mySpaceShip.Landing();

		// 残りの燃料の更新
		fuelRemainingText.text = "残りの燃料："+mySpaceShip.GetPercentFuelRemaining()+"%";

		angleText.text = "機体の傾き：" + mySpaceShip.GetRotation ()%360;

		landingVelocityText.text = "機体の落下速度：" + mySpaceShip.GetVelocity ().y * CORRECTIONTOLOOKVEROCITY;
	}
	
	// Game状態の更新関数
	void UpdateGameover(){
		// enterキーでTitleに切り替える
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Application.LoadLevel("Title");
		}
	}

	// スコアの計算
	int ComputeScore(){
		// 着陸速度が小さいほど高得点
		float score = mySpaceShip.GetLandingMaxVelocity() + mySpaceShip.GetVelocity().y;
		score = score * 100;

		// 残りの燃料に伴ってボーナス点を加算
		float percent = (float)mySpaceShip.GetPercentFuelRemaining ();
		score = score * percent;

		return (int)score;
	}

	// 壁や床にぶつかった時に呼び出される
	void OnTriggerEnter2D (Collider2D c){

		// ステージ(壁や地面)にぶつかった場合
		if (c.gameObject.tag == "Stage") {
			gameEndText.text = "GAMEOVER push Enter";
		}

		// 着地ポイントに着地した場合、着地成功かの判定を行う
		if(c.gameObject.tag == "LandPoint") {
			// 着地が成功した場合
			if(mySpaceShip.Landing()){
				// スコアの表示もここで行う
				gameEndText.text = "GameComplete\nscore："+ComputeScore()+"\npush Enter";
			}
			else{
				gameEndText.text = "GAMEOVER push Enter";
			}
		}

		// 重力と落下速度を調節する
		Physics2D.gravity=new Vector3(0.0f, 0.0f, 0.0f);
		mySpaceShip.SetVelocity(new Vector3 (0.0f, 0.0f, 0.0f));

		Transit (eStatus.eGameOver);
	}
}


