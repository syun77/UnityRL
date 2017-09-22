using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 整数値の2次元の値
/// </summary>
public struct Point2D {
  public int x;
  public int y;

  public Point2D(int x = 0, int y = 0) {
    this.x = x;
    this.y = y;
  }

  public void Set(int x = 0, int y = 0) {
    this.x = x;
    this.y = y;
  }

	static public Point2D operator+(Point2D a, Point2D b) {
		return new Point2D (b.x + a.x, b.y + a.y);
	}
	static public Point2D operator-(Point2D a, Point2D b) {
		return new Point2D (b.x - a.x, b.y - a.y);
	}
}
