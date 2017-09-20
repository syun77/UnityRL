using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 整数値の2次元の値
/// </summary>
public struct Point2D {
  public int x;
  public int y;

  public Point2D(int _x = 0, int _y = 0) {
    x = _x;
    y = _y;
  }

  public void Set(int _x = 0, int _y = 0) {
    x = _x;
    y = _y;
  }
}
