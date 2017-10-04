using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// 2次元レイヤー
public class Array2D {

	int _width; // 幅
	int _height; // 高さ
	int _outOfRange = -1; // 領域外を指定した時の値
	int[] _values = null; // マップデータ
	/// 幅
	public int Width {
		get { return _width; }
	}
	/// 高さ
	public int Height {
		get { return _height; }
	}

  /// ForEach関数に渡す関数の型
  public delegate void FuncT(int i, int j, int val);
	public delegate void FuncIdxT (int idx, int val);

	/// 作成
	public void Create(int width, int height) {
		_width = width;
		_height = height;
		_values = new int[Width * Height];
	}

  /// <summary>
  /// 配列から作成
  /// </summary>
  public void CreateFromArray(int width, int height, int[] array) {
    _width = width;
    _height = height;
    _values = array;
  }

	/// 座標をインデックスに変換する
	public int ToIdx(int x, int y) {
		return x + (y * Width);
	}

	/// インデックス値をX座標に変換する
	public int IdxToX(int idx) {
		return idx % Width;
	}
	public int IdxToY(int idx) {
		return idx / Width;
	}

	/// 領域外かどうかチェックする
	public bool IsOutOfRange(int x, int y) {
		if(x < 0 || x >= Width) { return true; }
		if(y < 0 || y >= Height) { return true; }

		// 領域内
		return false;
	}
	/// 値の取得
	// @param x X座標
	// @param y Y座標
	// @return 指定の座標の値（領域外を指定したら_outOfRangeを返す）
	public int Get(int x, int y) {
		if(IsOutOfRange(x, y)) {
			return _outOfRange;
		}

		return _values[y * Width + x];
	}

	/// 値の設定
	// @param x X座標
	// @param y Y座標
	// @param v 設定する値
	public void Set(int x, int y, int v) {
		if(IsOutOfRange(x, y)) {
			// 領域外を指定した
			return;
		}

		_values[y * Width + x] = v;
	}

	public void SetFromIdx(int idx, int v) {
		if (idx < 0 || Width * Height <= idx) {
			// 領域外を指定した
			return;
		}

		_values[idx] = v;
	}

  /// 全要素を走査し、その座標の値を関数に渡す
  public void ForEach(FuncT func) {
    for(var j = 0; j < Height; j++) {
      for(var i = 0; i < Width; i++) {
        int val = Get(i, j);
        func(i, j, val);
      }
    }
  }
	public void ForEachIndex(FuncIdxT func) {
		for(var i = 0; i < Width*Height; i++) {
			int val = _values [i];
			func (i, val);
		}
	}

  /// 指定の値が存在する座標を返す
  /// 見つからなかった場合は(-1, -1)を返す
  public Vec2D Search(int val) {
    for(var j = 0; j < Height; j++) {
      for(var i = 0; i < Width; i++) {
        if(Get(i, j) == val)
        {
          return new Vec2D(i, j);
        }
      }
    }
    // 見つからなかった
    return new Vec2D(-1, -1);
  }

	/// 指定の値が存在する座標をランダムで探す
	public Point2D SearchRandom(int val) {
		var list = new List<int> ();
		ForEachIndex((int idx, int v) => {
			if(v == val) {
				list.Add(idx);
			}
		});

		int rnd = Random.Range (0, list.Count);
		int idx2 = list[rnd];
		return new Point2D(IdxToX(idx2), IdxToY(idx2));
	}

	/// 指定の値ですべてを埋める
	public void Fill(int val) {
		ForEachIndex ((int idx, int v) => {
			SetFromIdx(idx, val);
		});
	}
	/// srcの値に一致するものをdstで埋める
	public void FillSearchVal(int src, int dst) {
		ForEachIndex((int idx, int v) => {
			if(v == src) {
				// 一致した
				SetFromIdx(idx, dst);
			}
		});
	}

  /// CSV文字列に変換する
  public string ToCsv() {
    string ret = "";
    ForEach((int i, int j, int v) => {
      if(i != 0 || j != 0) {
        ret += ",";
      }
      ret += v.ToString();
    });

    return ret;
  }

	/// デバッグ出力
	public void Dump() {
		Debug.Log("[Layer2D] (w,h)=("+Width+","+Height+")");
		for(int y = 0; y < Height; y++) {
			string s = "";
			for(int x = 0; x < Width; x++) {
				s += Get(x, y) + ",";
			}
			Debug.Log(s);
		}
	}
}
