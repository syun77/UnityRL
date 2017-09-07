using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールド情報管理
/// </summary>
public class Field {

	/// チップサイズの基準となるスプライトを取得する
	static Sprite GetChipSprite()
	{
		return Util.GetSprite("Levels/tileset", "tileset_0");
	}

	/// <summary>
	/// グリッド座標をワールド座標に変換する (X)
	/// </summary>
	/// <returns>The world x.</returns>
	/// <param name="x">The x coordinate.</param>
	public static float ToWorldX(int x) {
		// カメラビューの左下の座標を取得
		Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		var spr = GetChipSprite();
		var sprW = spr.bounds.size.x;

		return min.x + (sprW * x) + sprW/2;

	}

	/// <summary>
	/// グリッド座標をワールド座標に変換する (Y)
	/// </summary>
	/// <returns>The world y.</returns>
	/// <param name="y">The y coordinate.</param>
	public static float ToWorldY(int y) {
		// カメラビューの右上の座標を取得する
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		var spr = GetChipSprite();
		var sprH = spr.bounds.size.y;

		// Unityでは上下逆になるので、逆さにして変換
		return max.y - (sprH * y) - sprH/2;

	}
}
