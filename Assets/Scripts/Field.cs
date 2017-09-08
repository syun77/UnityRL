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
		return min.x + ToWorldDX (x) + ToWorldDX(1) * 0.5f;
	}

	/// <summary>
	/// グリッド座標をワールド座標に変換する (Y)
	/// </summary>
	/// <returns>The world y.</returns>
	/// <param name="y">The y coordinate.</param>
	public static float ToWorldY(int y) {
		// カメラビューの右上の座標を取得する
		Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));
		// Unityでは上下逆になるので、逆さにして変換
		return max.y + ToWorldDY(y) + ToWorldDY(1) * 0.5f;
	}

	public static float ToWorldDX(int dx) {
		if (CHIP_WIDTH == 0) {
			var spr = GetChipSprite();
			CHIP_WIDTH = spr.bounds.size.x;
		}

		return (CHIP_WIDTH * dx);
	}
	public static float ToWorldDY(int dy) {
		if (CHIP_HEIGHT == 0) {
			var spr = GetChipSprite();
			CHIP_HEIGHT = spr.bounds.size.y;
		}

		// Unityでは上下逆になるので、逆さにして変換
		return -(CHIP_HEIGHT * dy);
	}

	public static float CHIP_WIDTH = 0;
	public static float CHIP_HEIGHT = 0;
}
