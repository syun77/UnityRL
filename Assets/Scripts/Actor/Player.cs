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
	Enemy _Target = null;

  // ------------------------------------------
  // ■ public関数
	override public void BeginAction() {

		switch (State) {
		case eState.ActBegin:
			// 攻撃開始
			base.BeginAction ();
			StartCoroutine(_Attack (_Target, () => {
				// TODO: ひとまず1ターンで倒したことにする
				_Target.Vanish();
				_Change (eState.TurnEnd);
			}));
			break;
		}

	}

  // =======================================================
  // ■ここから private 関数

  /// <summary>
  /// 開始
  /// </summary>
  override protected void _Start() {
    Create (0, eDir.Down);
  }

	/// <summary>
	/// 更新
	/// </summary>
	override public void Proc() {

		switch (State) {
		case eState.KeyInput:
			// 入力待ち
			_ProcKeyInput ();
			break;

    case eState.MoveBegin:
      // 移動開始

    case eState.MoveExec:
			// 移動
      if (_ProcMove ()) {
        // 移動完了
        _Change(eState.TurnEnd);
      }
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
		if (Dir == eDir.None) {
			return;
		}

		// 移動する
		Vector2 v = DirUtil.ToVec(Dir);
    int NextX = _GridX + (int)v.x;
		int NextY = _GridY + (int)v.y;
		_Dir = Dir;

		_Target = null;
		EnemyManager.ForEachExists (((Enemy e) => {
			if(e.ExistsGrid(NextX, NextY)) {
				// 移動先に敵がいた
				_Target = e;
			}
		}));

		if (_Target != null) {
			// 移動先に敵がいる
			_Change (eState.ActBegin);
			return;
		}

    if (FieldManager.IsMovabledTile(NextX, NextY)) {
      // 移動可能
      _NextX = NextX;
      _NextY = NextY;
      _AnimState = eAnimState.Walk;
      _TimerMove = 0;
      _Change(eState.MoveBegin);
    }
	}

  /// <summary>
  /// アニメーションの更新
  /// </summary>
  override protected void _UpdateAnimation() {
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
