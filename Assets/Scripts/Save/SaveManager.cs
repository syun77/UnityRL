using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// セーブデータ管理
/// </summary>
public class SaveManager : MonoBehaviour {

  const string SAVE_FILE_PATH = "save.txt";

  // デバッグメッセージ表示用
  public DebugText debugText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKeyDown (KeyCode.S)) {
      // セーブ実行
      var data = new SaveData ();
      data.Set ();
      // JSONにシリアライズ
      var json = JsonUtility.ToJson (data);
      debugText.GetComponent<DebugText> ().SetText (json);
      // Assetsフォルダに保存する
      var path = Application.dataPath + "/" + SAVE_FILE_PATH;
      var writer = new StreamWriter (path, false);
      writer.WriteLine (json);
      writer.Flush ();
      writer.Close ();
    } else if (Input.GetKeyDown (KeyCode.L)) {
      // ロード実行
      // Assetsフォルダからロード
      var info = new FileInfo(Application.dataPath + "/" + SAVE_FILE_PATH);
      var reader = new StreamReader (info.OpenRead ());
      var json = reader.ReadToEnd ();
      debugText.GetComponent<DebugText> ().SetText (json);
      var data = new SaveData ();
      data.Load (json);
    }
	}
}
