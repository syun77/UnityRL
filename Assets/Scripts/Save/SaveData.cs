using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // UnityJsonを使う場合に必要

[Serializable]
public struct _Player {
  public int x;
  public int y;
}

[Serializable]
public class SaveData {
  public _Player player;

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public SaveData() {
    player = new _Player ();
    {
      var p = Player.GetInstance ();
      player.x = p.GridX;
      player.y = p.GridY;
    }
  }
}
