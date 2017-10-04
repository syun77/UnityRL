using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グローバルデータ
/// </summary>
public class Global {

  static public int level {
    get { return _level; }
  }

	static public int hp {
		get { return _hp; }
	}

  static int _level = 3;
	static int _hp = 10;

	/// <summary>
	/// 次のレベルに進む
	/// </summary>
	public static void NextLevel() {
		// TODO:
		_level++;
	}
}
