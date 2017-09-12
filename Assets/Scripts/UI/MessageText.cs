using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageText : MonoBehaviour {

  // テキストキューの最大数
  public int MAX_TEXT_QUEUE;
  // テキスト表示時間
  public int TIMER_TEXT;

  Queue<string> _Pool = null;
  int _TimerText = 0;

  /// <summary>
  /// テキスト追加
  /// </summary>
  /// <param name="str">String.</param>
  public void AddText(string str) {
    if (_Pool.Count >= MAX_TEXT_QUEUE) {
      // キューの最大を超えていたら取り除く
      _Pool.Dequeue();
    }

    _Pool.Enqueue (str);
    _TimerText = TIMER_TEXT;
  }

	// Use this for initialization
	void Start () {
        _Pool = new Queue<string> (MAX_TEXT_QUEUE);
	}
	
	// Update is called once per frame
	void Update () {

    if (_Pool.Count > 0) {
      _TimerText--;
      if (_TimerText == 0) {
        // キューから取り出し
        _Pool.Dequeue();
        _TimerText = TIMER_TEXT / 4;
      }
    }

    var text = GetComponent<Text> ();
    var str = "";
    foreach (string s in _Pool) {
      str += s + "\n";
    }
    text.text = str;
	}
}
