using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤー制御
/// </summary>
public class Player : MonoBehaviour {

[SerializeField]
	float MOVE_SPEED = 0.1f;

	// Use this for initialization
	void Start () {
		
	}
	
	/// <summary>
	/// 更新
	/// </summary>
	void Update () {
		// 移動
		UpdateMove ();
	}

	/// <summary>
	/// 更新・移動
	/// </summary>
	void UpdateMove() {
		Vector3 p = transform.position;
		if (Input.GetKey (KeyCode.UpArrow)) {
			// 上キーを押している
			p.y += MOVE_SPEED;
		} else if (Input.GetKey (KeyCode.LeftArrow)) {
			// 左キーをしている
			p.x -= MOVE_SPEED;
		} else if (Input.GetKey (KeyCode.DownArrow)) {
			// 下キーをしている
			p.y -= MOVE_SPEED;
		} else if (Input.GetKey (KeyCode.RightArrow)) {
			// 右キーをしている
			p.x += MOVE_SPEED;
		}

		// 移動量反映
		transform.position = p;

		ChangeSprite (1);
	}

	/// <summary>
	/// スプライトの番号を変更する
	/// </summary>
	/// <param name="idx">スプライト番号</param>
	void ChangeSprite(int idx) {

		// スプライトを取得する
		Sprite[] sprites = Resources.LoadAll<Sprite> ("Sprites/chara");
		string name = "chara_" + idx;
		Sprite spr = System.Array.Find<Sprite> (sprites, (sprite) => sprite.name.Equals (name));

		// スプライトを設定
		SpriteRenderer renderer = GetComponent<SpriteRenderer> ();
		renderer.sprite = spr;
	}
}
