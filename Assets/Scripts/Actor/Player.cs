using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー制御
/// </summary>
public class Player : Actor {

  // ------------------------------------------
  // ■static
  public static Player GetInstance() {
    var obj = GameObject.Find ("Player");
    return obj.GetComponent<Player> ();
  }

	// ------------------------------------------
	// ■メンバ変数

  // ------------------------------------------
  // ■ public関数

  // =======================================================
  // ■ここから private 関数

	/// <summary>
	/// 更新
	/// </summary>
	public void Proc() {

		switch (_Action) {
		case eAct.KeyInput:
			// 入力待ち
			_ProcKeyInput ();
			break;

		case eAct.Move:
			// 移動
			_ProcWalk ();
			break;
		}
	}

	/// <summary>
	/// 更新・キー入力待ち
	/// </summary>
	void _ProcKeyInput() {
		// 移動方向を判定する
		eDir Dir = eDir.None;
		if (Input.GetKey(KeyCode.UpArrow)) {
			// 上キーを押している
			Dir = eDir.Up;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			// 左キーを押している
			Dir = eDir.Left;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			// 下キーを押している
			Dir = eDir.Down;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			// 右キーを押している
			Dir = eDir.Right;
		}
		_AnimState = eAnimState.Standby;
		if (Dir != eDir.None) {
			// 移動する
			Vector2 v = DirUtil.ToVec(Dir);
      int NextX = _GridX + (int)v.x;
			int NextY = _GridY + (int)v.y;
			_Dir = Dir;

      if (FieldManager.IsMovabledTile(NextX, NextY)) {
        // 移動可能
        _NextX = NextX;
        _NextY = NextY;
        _AnimState = eAnimState.Walk;
        _Timer = 0;
        _Action = eAct.Move;
      }
		}
	}

	/// <summary>
	/// 更新・移動
	/// </summary>
	void _ProcWalk() {
		_Timer++;
		if (_Timer >= _TIMER_WALK) {
			// 移動完了
			_GridX = _NextX;
			_GridY = _NextY;
			_Action = eAct.KeyInput;
			_AnimState = eAnimState.Standby;
      MessageWindowManager.AddMessage("x = " + _GridX + ", y = " + _GridY + "に移動");
		}
	}

  /// <summary>
  /// アニメーションの更新
  /// </summary>
  override protected void _UpdateAnimation() {
    _AnimTimer += Time.deltaTime;
    int idx = _GetAnimationIdx ();
    int ofs = _GetAnimationOfs ();
    idx *= 4;
    _ChangeSprite (idx + ofs);
  }

  /// <summary>
  /// アニメーションの番号を取得する
  /// </summary>
  /// <returns>アニメーション番号</returns>
  int _GetAnimationIdx() {
    switch (_Dir) {
    case eDir.Left:
      return 0;
    case eDir.Up:
      return 1;
    case eDir.Right:
      return 2;
    case eDir.Down:
      return 3;
    default:
      return 3;
    }
  }

  /// <summary>
  /// アニメーションのオフセット番号を取得する
  /// </summary>
  /// <returns>The animation offset.</returns>
  int _GetAnimationOfs() {
    int t = (int)(_AnimTimer * 40);
    switch (_AnimState) {
    case eAnimState.Standby:
      if (t%24 < 12) {
        return 0;
      } else {
        return 1;
      }
    case eAnimState.Walk:
      int v = t % 32;
      if (v < 8) {
        return 0;
      } else if (v < 16) {
        return 1;
      } else if (v < 24) {
        return 2;
      } else {
        return 3;
      }
    }

    return 0;
  }

	/// <summary>
	/// スプライトの番号を変更する
	/// </summary>
	/// <param name="idx">スプライト番号</param>
	void _ChangeSprite(int idx) {

		// スプライトを取得する
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/chara");
		string name = "chara_" + idx;
		Sprite spr = System.Array.Find<Sprite> (sprites, (sprite) => sprite.name.Equals (name));

		// スプライトを設定
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = spr;
	}
}
