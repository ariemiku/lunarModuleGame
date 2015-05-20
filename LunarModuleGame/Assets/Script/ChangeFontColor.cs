using UnityEngine;
using System.Collections;

public class ChangeFontColor : MonoBehaviour {
	// フォントのカラー.
	enum eFontColor {
		eGreen,
		eYellow,
		eRed,
		eWhite,
	};

	public Game m_game;
	SpaceShip m_spaceship;
	public GUIText m_text;
	public eTextType m_textType;
	eFontColor m_fontColor;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		ChangeColor ();
	}

	// フォントの色変え.
	void ChangeColor () {
		if (m_spaceship == null) {

			if(m_game == null) {
				return;
			}
			m_spaceship = m_game.GetMySpaceShip ();
			return;
		}

		// テキストの種類の現状によって色を決める.
		switch (m_textType) {
		case eTextType.eTextFuelRemaining:
			ChangeColorTextFuelRemaining ();
			break;
		case eTextType.eTextAngle:
			ChangeColorTextAngle ();
			break;
		case eTextType.eTextLandingVelocity:
			ChangeColorTextLandingVelocity ();
			break;
		case eTextType.eTextCheckLanding:
			ChangeColorTextCheckLanding ();
			break;
		case eTextType.eTextHorizontalSpeed:
			ChangeColorTextHorizontalSpeed ();
			break;
		default:
			m_fontColor = eFontColor.eWhite;
			break;
		}

		// フォントの色を変える.
		switch (m_fontColor) {
		case eFontColor.eGreen:
			m_text.color = Color.green;
			break;
		case eFontColor.eYellow:
			m_text.color = Color.yellow;
			break;
		case eFontColor.eRed:
			m_text.color = Color.red;
			break;
		case eFontColor.eWhite:
			m_text.color = Color.white;
			break;
		}
	}

	// 燃料の色変え.
	void ChangeColorTextFuelRemaining () {
		int percentFuelRemaining = m_spaceship.GetPercentFuelRemaining ();
		if (percentFuelRemaining > 50) {
			m_fontColor = eFontColor.eGreen;
		}
		else if (percentFuelRemaining > 10) {
			m_fontColor = eFontColor.eYellow;
		}
		else {
			m_fontColor = eFontColor.eRed;
		}
	}

	// 角度の色変え.
	void ChangeColorTextAngle () {
		int rotation = (int)m_spaceship.GetRotation ();
		if ((rotation > 4.0f && rotation < 356.0f) || 
		    (rotation < -4.0f && rotation > -356.0f)) {
			m_fontColor = eFontColor.eRed;
		}
		else {
			m_fontColor = eFontColor.eGreen;
		}
	}

	// 落下速度の色変え.
	void ChangeColorTextLandingVelocity () {
		if (m_game.GetStatus () == eStatus.eGameOver) {
			return;
		}
		float verticalSpeed = -0.005f;
		if (-m_spaceship.GetVerticalSpeed () < verticalSpeed) {
			m_fontColor = eFontColor.eRed;
		}
		else {
			m_fontColor = eFontColor.eGreen;
		}
	}

	// 平行速度の色変え.
	void ChangeColorTextHorizontalSpeed () {
		float horizontalSpeed = m_spaceship.GetHorizontalSpeed ();
		if(horizontalSpeed > 0.005 || horizontalSpeed < -0.005) {
			m_fontColor = eFontColor.eRed;
		}
		else {
			m_fontColor = eFontColor.eGreen;
		}
	}

	// 着陸できるかの色変え.
	void ChangeColorTextCheckLanding () {

	}
}
