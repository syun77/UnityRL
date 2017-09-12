using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メッセージウィンドウ管理
/// </summary>
public class MessageWindowManager : MonoBehaviour {

  /// <summary>
  /// メッセージ追加
  /// </summary>
  /// <param name="str">String.</param>
  public static void AddMessage(string str) {
    var obj = GameObject.Find ("MessageWindowManager");
    var mgr = obj.GetComponent<MessageWindowManager> ();
    mgr.messageText.AddText(str);
  }

  // メッセージ表示用
  public MessageText messageText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
