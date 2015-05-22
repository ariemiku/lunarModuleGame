using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;

public class Ranking : MonoBehaviour {
	const int rankingMaxNum = 10;

	private static Ranking s_instance;
	int[] m_score = new int[rankingMaxNum];
	string[] m_name = new string[rankingMaxNum];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// インスタンス取得.
	public static Ranking GetInstance () {
		if (s_instance == null) {
			GameObject gameObject = new GameObject ("Ranking");
			s_instance = gameObject.AddComponent<Ranking> ();
		}

		return s_instance;
	}

	public void SetRankingDate (string fileName) {
		FileStream fs = new FileStream (
			Application.dataPath + "/RankingText/" + fileName + ".txt",
			FileMode.Open,
			FileAccess.Read
			);
		StreamReader sr = new StreamReader (fs);
		
		// ランキング情報取得.
		while (sr.ReadLine () != 's'.ToString ()) {
			
		}
		for (int i = 0; i < rankingMaxNum; i++) {
			m_score[i] = int.Parse (sr.ReadLine ());
		}
		while (sr.ReadLine () != 'n'.ToString ()) {
			
		}
		for (int i = 0; i < rankingMaxNum; i++) {
			m_name[i] = sr.ReadLine ();
		}
		
		// 閉じる.
		sr.Close ();
		fs.Close ();
	}

	// ランキングを書き込む.
	public void WriteRanking (int score, string fileName, string userName) {
		SetRankingDate (fileName);

		for (int i = 0; i < rankingMaxNum; i++) {
			// ランクイン.
			if(m_score[i] <= score) {
				for (int j = rankingMaxNum - 1; j > i; j--) {
					m_score[j] = m_score[j-1];
					m_name[j] = m_name[j-1];
				}
				m_score[i] = score;
				m_name[i] = userName;

				// 書き込み.
				FileStream fs = new FileStream (
					Application.dataPath + "/RankingText/" + fileName + ".txt",
					FileMode.Create,
					FileAccess.Write
				);
				Encoding utf8 = Encoding.GetEncoding ("UTF-8");
				StreamWriter sw = new StreamWriter (fs, utf8);

				sw.WriteLine ('s');
				for (int j = 0; j < rankingMaxNum; j++) {
					sw.WriteLine (m_score[j]);
				}
				sw.WriteLine ("\nn");
				for(int j = 0; j < rankingMaxNum; j++) {
					sw.WriteLine (m_name[j]);
				}
				
				// 閉じる.
				sw.Close ();
				fs.Close ();

				return;
			}
		}
	}

	// ランキング取得.
	public int[] GetScore () {
		return m_score;
	}

	public string[] GetName () {
		return m_name;
	}

	// ランキング上限数取得.
	public int GetRankingMaxNum () {
		return rankingMaxNum;
	}
}
