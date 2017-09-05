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

	// Use this for initialization
	void Start () {
		_Dir = eDir.Down;
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
		Vector3 p = transform.position;
		eDir Dir = eDir.None;
		if (Input.GetKey (KeyCode.UpArrow)) {
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
		Vector2 v = DirUtil.ToVecWorld (Dir);
		if (Dir != eDir.None) {
			_Dir = Dir;
		}
		p.x += v.x * _MOVE_SPEED;
		p.y += v.y * _MOVE_SPEED;

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
	/// アニメーションの更新
	/// </summary>
	void _UpdateAnimation() {
		int idx = _GetAnimationIdx ();
		idx *= 4;
		_ChangeSprite (idx);
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
