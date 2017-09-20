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

  static int _level = 3;
}
