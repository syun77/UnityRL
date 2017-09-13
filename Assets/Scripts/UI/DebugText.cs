using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// デバッグ用テキスト
/// </summary>
public class DebugText : MonoBehaviour {

  // 表示時間
  const float TIMER_DISPLAY = 1.0f;

  float _Timer = 0;

  /// <summary>
  /// テキストを設定
  /// </summary>
  /// <param name="text">Text.</param>
  public void SetText(string msg) {
    var text = GetComponent<Text> ();
    text.text = msg;
    var c = text.color;
    c.a = 1;
    text.color = c;
    _Timer = TIMER_DISPLAY;
  }

	// Use this for initialization
	void Start () {
    SetText ("");
	}
	
	// Update is called once per frame
	void Update () {
    // テキスト更新
    _Timer -= Time.deltaTime;
    if (_Timer < 0) {
      var text = GetComponent<Text> ();
      var c = text.color;
      c.a *= 0.9f;
      text.color = c;
    }
	}
}
