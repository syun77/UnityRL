using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵
/// </summary>
public class Enemy : Actor {

  /// <summary>
  /// 開始
  /// </summary>
  override protected void _Start() {
    Create (1, eDir.Down);
  }

	// Update is called once per frame
	override public void Proc () {
		
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
