using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー制御
/// </summary>
public class Player : MonoBehaviour {

	[SerializeField]
	int _TIMER_WALK = 10; // 移動速度

	/// <summary>
	/// 状態
	/// </summary>
	enum eState {
		KeyInput, // 入力待ち
		Walk,     // 移動中
	}

	/// <summary>
	/// アニメーション状態
	/// </summary>
	enum eAnimState {
		Standby, // 待機アニメ
		Walk,    // 歩きアニメ
	}

  // ------------------------------------------
  // ■static
  public static Player GetInstance() {
    var obj = GameObject.Find ("Player");
    return obj.GetComponent<Player> ();
  }

	// ------------------------------------------
	// ■メンバ変数

	[SerializeField]
	eDir _Dir; // 移動方向

	// 状態
	[SerializeField]
	eState _State = eState.KeyInput;

	// アニメーション状態
	[SerializeField]
	eAnimState _AnimState = eAnimState.Standby;

	// グリッド座標
	[SerializeField]
	int _GridX, _GridY;
  public int GridX {
    get { return _GridX; }
  }
  public int GridY {
    get { return _GridY; }
  }

	// 移動先の座標
	[SerializeField]
	int _NextX, _NextY;

	// アニメーションタイマー
	float _AnimTimer = 0;

	// 汎用タイマー
	int _Timer = 0;

  /// <summary>
  /// 指定座標にワープする
  /// </summary>
  /// <param name="i">The index.</param>
  /// <param name="j">J.</param>
  public void Warp(int i, int j) {
    _GridX = i;
    _GridY = j;
    _NextX = i;
    _NextY = j;
  }

  // =======================================================
  // ■ここから private 関数

	/// <summary>
	/// 初期化
	/// </summary>
	void Start () {
		_Dir = eDir.Down;
		_AnimState = eAnimState.Standby;
		_AnimTimer = 0;
	}
	
	/// <summary>
	/// 更新
	/// </summary>
	void Update () {

		switch (_State) {
		case eState.KeyInput:
			// 入力待ち
			_UpdateKeyInput ();
			break;

		case eState.Walk:
			// 移動
			_UpdateWalk ();
			break;
		}

		// 座標の更新
		if (_State == eState.Walk) {
			// 補間あり
			float Ratio = 1.0f *  _Timer / _TIMER_WALK;
			_UpdatePosition (Ratio);
		} else {
			_UpdatePosition(0);
		}

		// アニメーションの更新
		_UpdateAnimation();
	}

	/// <summary>
	/// 更新・キー入力待ち
	/// </summary>
	void _UpdateKeyInput() {
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
        _State = eState.Walk;
      }
		}
	}

	/// <summary>
	/// 更新・移動
	/// </summary>
	void _UpdateWalk() {
		_Timer++;
		if (_Timer >= _TIMER_WALK) {
			// 移動完了
			_GridX = _NextX;
			_GridY = _NextY;
			_State = eState.KeyInput;
			_AnimState = eAnimState.Standby;
		}
	}

	/// <summary>
	/// 座標の更新
	/// </summary>
	/// <param name="bInterpolated">補間を有効にするかどうか<c>true</c> b interpolated.</param>
	/// <param name="Ratio">補間値</param>
	void _UpdatePosition(float Ratio) {
		// 移動量を求める
		Vector3 p = transform.position;
		float px = Field.ToWorldX (_GridX);
		float py = Field.ToWorldY (_GridY);
		if (Ratio > 0) {
			// 補間あり
			px += Field.ToWorldDX (_NextX - _GridX) * Ratio;
			py += Field.ToWorldDY (_NextY - _GridY) * Ratio;
		}
		p.x = px;
		p.y = py;

		// 移動量反映
		transform.position = p;
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
	/// アニメーションの更新
	/// </summary>
	void _UpdateAnimation() {
		_AnimTimer += Time.deltaTime;
		int idx = _GetAnimationIdx ();
		int ofs = _GetAnimationOfs ();
		idx *= 4;
		_ChangeSprite (idx + ofs);
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
