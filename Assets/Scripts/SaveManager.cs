using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// セーブデータ管理
/// </summary>
public class SaveManager : MonoBehaviour {

  // デバッグメッセージ表示用
  public DebugText debugText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown (KeyCode.S)) {
      // セーブ実行
      debugText.GetComponent<DebugText>().SetText("Save Done.");
    }
	}
}
