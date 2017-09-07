using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー制御
/// </summary>
public class Player : MonoBehaviour {

	[SerializeField]
	float _MOVE_SPEED = 0.1f; // 移動速度

	[SerializeField]
	eDir _Dir; // 移動方向

	/// <summary>
	/// 状態
	/// </summary>
	enum eAnimState {
		Standby, // 待機アニメ
		Walk,    // 歩きアニメ
	}

	// ------------------------------------------
	// ■メンバ変数

	// アニメーション状態
	[SerializeField]
	eAnimState _AnimState = eAnimState.Standby;

	// グリッド座標
	[SerializeField]
	int xgrid, ygrid;

	// アニメーションタイマー
	float _AnimTimer = 0;

	int _WaitTimer = 0;

	/// <summary>
	/// 初期化
	/// </summary>
	void Start () {
		_Dir = eDir.Down;
		_AnimState = eAnimState.Standby;
		_AnimTimer = 0;
		xgrid = 6;
		ygrid = 6;
	}
	
	/// <summary>
	/// 更新
	/// </summary>
	void Update () {
		// 移動
		_UpdateMove ();
		// アニメーションの更新
		_UpdateAnimation();
	}

	/// <summary>
	/// 更新・移動
	/// </summary>
	void _UpdateMove() {

		if (_WaitTimer > 0) {
			_WaitTimer--;
			return;
		}

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
			_Dir = Dir;
			_AnimState = eAnimState.Walk;
			_WaitTimer = 30;
		}

		// 移動量を求める
		Vector3 p = transform.position;
		Vector2 v = DirUtil.ToVecWorld (Dir);
		xgrid += (int)v.x;
		ygrid += (int)v.y;
		p.x = Field.ToWorldX (xgrid);
		p.y = Field.ToWorldY (ygrid);

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
