﻿using UnityEngine;
using System.Collections;

public class ChangeFontSize : MonoBehaviour {
	const int defaultWidth = 800;
	const int defaultHeight = 600;

	const int parameterFontSize = 20;
	const int tutorialFontSize = 24;
	const int gameEndFontSize = 24;
	const int courseTimeFontSize = 50;
	const int titleFontSize = 50;
	const int rankingFontSize = 40;

	public GUIText m_text;
	public eTextType m_textType;

	Vector2 m_nowScreenSize;

	// Use this for initialization
	void Start () {
		m_nowScreenSize = new Vector2 (Screen.width, Screen.height);
		ChengeSize ();
	}

	// Update is called once per frame
	void Update () {
		if (m_text == null) {
			return;
		}
		if (m_nowScreenSize != new Vector2 (Screen.width, Screen.height)) {
			ChengeSize ();
		}
	}

	// フォントサイズ変更.
	public void ChengeSize () {
		int fontSize = parameterFontSize;

		switch (m_textType) {
		case eTextType.eTextGameEnd:
			fontSize = gameEndFontSize;
			break;
		case eTextType.eTextAngle:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextLandingVelocity:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextCheckLanding:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextTutorial:
			fontSize = tutorialFontSize;
			break;
		case eTextType.eTextHorizontalSpeed:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextCourseTime:
			fontSize = courseTimeFontSize;
			break;
		case eTextType.eTextStageNum:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextScore:
			fontSize = parameterFontSize;
			break;
		case eTextType.eTextTitle:
			fontSize = titleFontSize;
			break;
		case eTextType.eTextRanking:
			fontSize = rankingFontSize;
			break;
		case eTextType.eTextMyScore:
			fontSize = rankingFontSize;
			break;
		}

		// スクリーンの大きさによりフォントサイズ変更.
		m_nowScreenSize = new Vector2 (Screen.width, Screen.height);
		float fontSizeWidth = fontSize * (m_nowScreenSize.x / defaultWidth);
		float fontSizeHeight = fontSize * (m_nowScreenSize.y / defaultHeight);
		if (fontSizeWidth > fontSizeHeight) {
			m_text.fontSize = (int)fontSizeHeight;
		}
		else {
			m_text.fontSize = (int)fontSizeWidth;
		}
	}
}
