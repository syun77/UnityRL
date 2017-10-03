using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusText : MonoBehaviour {

	Text _text = null;

	public void SetText(string msg) {
		if (_text == null) {
			_text = GetComponent<Text> ();
		}
		_text.text = msg;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		var floor = Global.level;
		var hp = Global.hp;
		var msg = string.Format ("Floor: {0}\nHp: {1}", floor, hp);
		SetText (msg);
	}
}
