using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// enterキーでシーンを切り替える
		if(Input.GetKeyDown(KeyCode.Return))
		{
			Application.LoadLevel("LunarModuleGame");
		}
	
	}
}
