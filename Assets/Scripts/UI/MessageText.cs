using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageText : MonoBehaviour {

  Queue<string> _Pool = null;

  /// <summary>
  /// テキスト追加
  /// </summary>
  /// <param name="str">String.</param>
  public void AddText(string str) {
    _Pool.Enqueue (str);
  }

	// Use this for initialization
	void Start () {
    _Pool = new Queue<string> (4);
	}
	
	// Update is called once per frame
	void Update () {
    var text = GetComponent<Text> ();
    var str = "";
    foreach (string s in _Pool) {
      str += s + "\n";
    }
    text.text = str;
	}
}
