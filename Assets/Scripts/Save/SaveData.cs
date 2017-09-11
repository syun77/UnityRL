using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // UnityJsonを使う場合に必要

[Serializable]
public struct _Player {
  public int x;
  public int y;
  public eDir dir;
}

[Serializable]
public class SaveData {
  public _Player player;

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public SaveData() {
  }

  /// <summary>
  /// セーブする値を設定する
  /// </summary>
  public void Set() {
    _SetPlayer ();
  }

  /// <summary>
  /// Player情報を設定する
  /// </summary>
  void _SetPlayer() {
    player = new _Player ();
    {
      var p = Player.GetInstance ();
      player.x = p.GridX;
      player.y = p.GridY;
      player.dir = p.Dir;
    }
  }

  /// <summary>
  /// Json文字列をロードする
  /// </summary>
  /// <param name="json">Json.</param>
  public void Load(string json) {
    var data = JsonUtility.FromJson<SaveData> (json);
    data._Load ();
  }

  /// <summary>
  /// 実際のロード処理
  /// </summary>
  void _Load() {
    // プレイヤー
    var p = Player.GetInstance ();
    p.Warp (player.x, player.y, player.dir);
  }
}
