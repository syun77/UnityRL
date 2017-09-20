using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 敵
/// </summary>
public class Enemy : Actor {

  public static Player target = null;

  /// <summary>
  /// 移動要求をする
  /// </summary>
  public void RequestMove() {
    var p = new Point2D (GridX, GridY);
    var dir = _aiMoveDir ();
    p = DirUtil.Add (p, dir);

    if (target.ExistsGrid (p.x, p.y)) {
      // 移動先にプレイヤーがいるときは攻撃する
      //_Change(eState.ActBegin);
			_Change(eState.TurnEnd); // TODO: ひとまずターン終了
      return;
    }

    if (FieldManager.IsMovabledTile (p.x, p.y)) {
      // 移動可能
      _NextX = p.x;
      _NextY = p.y;
      _Change (eState.MoveBegin);
    } else {
      // 移動できないのでターン終了
      _Change (eState.TurnEnd);
    }

  }

  /// <summary>
  /// 開始
  /// </summary>
  override protected void _Start() {
    Create (1, eDir.Down);
  }

	// Update is called once per frame
	override public void Proc () {
    switch (State) {
    case eState.MoveExec:
      // 移動実行
      if (_ProcMove ()) {
        _Change (eState.TurnEnd);
      }
      break;
    }
  }

  /// <summary>
  /// 移動方向を決める
  /// </summary>
  /// <returns>The move dir.</returns>
  eDir _aiMoveDir() {
    var dir = eDir.None;

    // 頭の悪い方法で移動する
    dir = _AiMoveDirStupid ();

    return dir;
  }

  /// <summary>
  /// 頭の悪い方法で移動する
  /// </summary>
  /// <returns>The move dir stupid.</returns>
  eDir _AiMoveDirStupid() {
    var target = Player.GetInstance ();
    var dx = target.GridX - GridX;
    var dy = target.GridY - GridY;
    Func<eDir> func = () => {
      // 水平方向に移動するかどうか
      var bHorizon = Mathf.Abs (dx) > Mathf.Abs (dy);
      if (bHorizon) {
        // 水平方向へ移動
        return (dx < 0) ? eDir.Left : eDir.Right;
      } else {
        // 垂直方向へ移動
        return (dy < 0) ? eDir.Up : eDir.Down;
      }
    };

    return func ();
  }

  override protected void _UpdateAnimation() {
    var idx = 2 * (ID - 1);
    idx += _GetAnimationIdx ();
    var ofs = _GetAnimationOfs ();
    _ChangeSprite (idx + ofs);
  }

  int _GetAnimationIdx() {
    switch (_Dir) {
    case eDir.Left:
      return 0;
    case eDir.Up:
      return 0;
    case eDir.Right:
      return 0;
    case eDir.Down:
      return 0;
    default:
      return 0;
    }
  }

  int _GetAnimationOfs() {
    int t = (int)(_AnimTimer * 100) % 100;
    return (t / 50);
  }

  void _ChangeSprite(int idx) {
    Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/enemy");
    string name = "enemy_" + idx;
    Sprite spr = System.Array.Find<Sprite> (sprites, (sprite) => sprite.name.Equals (name));

    SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
    renderer.sprite = spr;
  }
}
