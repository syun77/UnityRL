using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // UnityJsonを使う場合に必要

/// <summary>
/// プレイヤー情報
/// </summary>
[Serializable]
public struct _Player {
  public int x;
  public int y;
  public eDir dir;

  /// <summary>
  /// ゲームデータから情報をコピーする
  /// </summary>
  public void Save() {
      var player = Player.GetInstance ();
      x = player.GridX;
      y = player.GridY;
      dir = player.Dir;
  }

  /// <summary>
  /// データをゲームに反映させる
  /// </summary>
  public void Load() {
    var p = Player.GetInstance ();
    p.Warp (x, y, dir);
  }
}

[Serializable]
public struct _Enemy {
  public int x;
  public int y;
  public eDir dir;

  /// <summary>
  /// ゲームデータから情報をコピーする
  /// </summary>
  public void Save(Enemy e) {
      x = e.GridX;
      y = e.GridY;
      dir = e.Dir;
  }

  /// <summary>
  /// データをゲームに反映させる
  /// </summary>
  public void Load() {
    EnemyManager.Add (1, x, y);
  }
}

/// <summary>
/// マップ情報
/// </summary>
[Serializable]
public struct _Map {
  public string data;
  public int width;
  public int height;

  public void Save() {
    var layer = FieldManager.Layer;
    data = layer.ToCsv ();
    width = layer.Width;
    height = layer.Height;
  }

  public void Load() {
    var layer = FieldManager.Layer;
    var strArray = data.Split (","[0]);
    var array = Array.ConvertAll (strArray, int.Parse);
    layer.CreateFromArray (width, height, array);
    // 背景描画
    FieldManager.RenderBack();
  }
}

[Serializable]
public class SaveData {
  public _Player player = new _Player();
  public _Enemy[] enemies = null;
  public _Map    map    = new _Map ();

  /// <summary>
  /// コンストラクタ
  /// </summary>
  public SaveData() {
  }

  /// <summary>
  /// セーブする値を設定する
  /// </summary>
  public void Save() {
    player.Save ();
    map.Save ();
    enemies = new _Enemy[EnemyManager.Count ()];
    int idx = 0;
    EnemyManager.ForEachExists (((Enemy e) => {
      var enemy = new _Enemy();
//      var enemy = enemies[idx];
      enemy.Save(e);
      enemies[idx] = enemy;
      idx++;
    }));
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
    player.Load();
    map.Load ();

    // 敵をいったん全て消す
    EnemyManager.KillAll();
    foreach (var e in enemies) {
      e.Load ();
    }
  }
}
