using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移動方向定数
/// </summary>
public enum eDir {
	None, // 無効な方向

	Left, // 左
	Up, // 上
	Right, // 右
	Down, // 下
};

/// <summary>
/// 移動方向のユーティリティ
/// </summary>
public class DirUtil {

	public static Vector2 ToVec(eDir Dir) {
		Vector2 v = new Vector2(0, 0);
		switch (Dir) {
		case eDir.Left:
			v.x = -1;
			break;
		case eDir.Up:
			v.y = -1;
			break;
		case eDir.Right:
			v.x = 1;
			break;
		case eDir.Down:
			v.y = 1;
			break;
		}

		return v;
	}

	public static Point2D ToVecWorld(eDir Dir) {
		var v = new Point2D(0, 0);
		switch (Dir) {
		case eDir.Left:
			v.x = -1;
			break;
		case eDir.Up:
			v.y = 1;
			break;
		case eDir.Right:
			v.x = 1;
			break;
		case eDir.Down:
			v.y = -1;
			break;
		}

		return v;
	}

  /// <summary>
  /// 指定の座標に移動量を足し込む
  /// </summary>
  /// <param name="p">P.</param>
  /// <param name="Dir">Dir.</param>
  public static Point2D Add(Point2D p, eDir Dir) {
    switch (Dir) {
    case eDir.Left:
      p.x -= 1;
      break;
    case eDir.Up:
      p.y -= 1;
      break;
    case eDir.Right:
      p.x += 1;
      break;
    case eDir.Down:
      p.y += 1;
      break;
    }

    return p;
  }
}
