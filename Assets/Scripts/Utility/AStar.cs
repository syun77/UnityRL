using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A-star algorithm.
/// </summary>
public class AStar {

	/// <summary>
	/// デバッグ出力を行うかどうか
	/// </summary>
	public static bool DEBUG_DUMP = false;

	/// <summary>
	/// 移動可能かどうか
	/// </summary>
	/// <returns><c>true</c> if is movable the specified tile; otherwise, <c>false</c>.</returns>
	/// <param name="tile">Tile.</param>
	public static bool IsMovable(int tile) {
		return tile != (int)FieldManager.eTile.Wall;
	}

	/// A-starノード.
	class ANode {
		enum eStatus {
			None,
			Open,
			Closed,
		}
		/// ステータス
		eStatus _status = eStatus.None;
		/// 実コスト
		int _cost = 0;
		/// ヒューリスティック・コスト
		int _heuristic = 0;
		/// 親ノード
		ANode _parent = null;
		/// 座標
		int _x = 0;
		int _y = 0;
		public int X {
			get { return _x; }
		}
		public int Y {
			get { return _y; }
		}
		public int Cost {
			get { return _cost; }
		}

		/// コンストラクタ.
		public ANode(int x, int y) {
			_x = x;
			_y = y;
		}
		/// スコアを計算する.
		public int GetScore() {
			return _cost + _heuristic;
		}
		/// ヒューリスティック・コストの計算.
		public void CalcHeuristic(bool allowdiag, int xgoal, int ygoal) {

			if(allowdiag) {
				// 斜め移動あり
				var dx = (int)Mathf.Abs (xgoal - X);
				var dy = (int)Mathf.Abs (ygoal - Y);
				// 大きい方をコストにする
				_heuristic =  dx > dy ? dx : dy;
			}
			else {
				// 縦横移動のみ
				var dx = Mathf.Abs (xgoal - X);
				var dy = Mathf.Abs (ygoal - Y);
				_heuristic = (int)(dx + dy);
			}

			if (DEBUG_DUMP) {
				Dump();
			}
		}
		/// ステータスがNoneかどうか.
		public bool IsNone() {
			return _status == eStatus.None;
		}
		/// ステータスをOpenにする.
		public void Open(ANode parent, int cost) {
			if (DEBUG_DUMP) {
				Debug.Log (string.Format ("Open: ({0},{1})", X, Y));
			}
			_status = eStatus.Open;
			_cost   = cost;
			_parent = parent;
		}
		/// ステータスをClosedにする.
		public void Close() {
			if (DEBUG_DUMP) {
				Debug.Log (string.Format ("Closed: ({0},{1})", X, Y));
			}
			_status = eStatus.Closed;
		}
		/// パスを取得する
		public void GetPath(List<Point2D> pList) {
			pList.Add(new Point2D(X, Y));
			if(_parent != null) {
				_parent.GetPath(pList);
			}
		}
		public void Dump() {
			Debug.Log (string.Format("({0},{1})[{2}] cost={3} heuris={4} score={5}", X, Y, _status, _cost, _heuristic, GetScore()));
		}
		public void DumpRecursive() {
			Dump ();
			if(_parent != null) {
				// 再帰的にダンプする.
				_parent.DumpRecursive ();
			}
		}
	}

	/// A-starノード管理.
	class ANodeMgr {
		/// 地形レイヤー.
		Array2D _layer;
		/// 斜め移動を許可するかどうか.
		bool _allowdiag = true;
		/// オープンリスト.
		List<ANode> _openList = null;
		/// ノードインスタンス管理.
		Dictionary<int,ANode> _pool = null;
		/// ゴール座標.
		int _xgoal = 0;
		int _ygoal = 0;
		public ANodeMgr(Array2D layer, int xgoal, int ygoal, bool allowdiag=true) {
			_layer = layer;
			_allowdiag = allowdiag;
			_openList = new List<ANode>();
			_pool = new Dictionary<int, ANode>();
			_xgoal = xgoal;
			_ygoal = ygoal;
		}
		/// ノード生成する.
		public ANode GetNode(int x, int y) {
			var idx = _layer.ToIdx(x, y);
			if(_pool.ContainsKey(idx)) {
				// 既に存在しているのでプーリングから取得.
				return _pool[idx];
			}

			// ないので新規作成.
			var node = new ANode(x, y);
			_pool[idx] = node;
			// ヒューリスティック・コストを計算する.
			node.CalcHeuristic(_allowdiag, _xgoal, _ygoal);
			return node;
		}
		/// ノードをオープンリストに追加する.
		public void AddOpenList(ANode node) {
			_openList.Add(node);
		}
		/// ノードをオープンリストから削除する.
		public void RemoveOpenList(ANode node) {
			_openList.Remove(node);
		}
		/// 指定の座標にあるノードをオープンする.
		public ANode OpenNode(int x, int y, int cost, ANode parent) {
			// 座標をチェック.
			if(_layer.IsOutOfRange(x, y)) {
				// 領域外.
				return null;
			}
			if(AStar.IsMovable(_layer.Get(x, y)) == false) {
				// 通過できない.
				return null;
			}
			// ノードを取得する.
			var node = GetNode(x, y);
			if(node.IsNone() == false) {
				// 既にOpenしているので何もしない
				return null;
			}

			// Openする.
			node.Open(parent, cost);
			AddOpenList(node);

			return node;
		}

		/// 周りをOpenする.
		public void OpenAround(ANode parent) {
			var xbase = parent.X; // 基準座標(X).
			var ybase = parent.Y; // 基準座標(Y).
			var cost = parent.Cost; // コスト.
			cost += 1; // 一歩進むので+1する.
			if(_allowdiag) {
				// 8方向を開く.
				for(int j = 0; j < 3; j++) {
					for(int i = 0; i < 3; i++) {
						var x = xbase + i - 1; // -1～1
						var y = ybase + j - 1; // -1～1
						OpenNode(x, y, cost, parent);
					}
				}
			}
			else {
				// 4方向を開く.
				var x = xbase;
				var y = ybase;
				OpenNode (x-1, y,   cost, parent); // 右.
				OpenNode (x,   y-1, cost, parent); // 上.
				OpenNode (x+1, y,   cost, parent); // 左.
				OpenNode (x,   y+1, cost, parent); // 下.
			}
		}

		/// 最小スコアのノードを取得する.
		public ANode SearchMinScoreNodeFromOpenList() {
			// 最小スコア
			int min = 9999;
			// 最小実コスト
			int minCost = 9999;
			ANode minNode = null;
			foreach(ANode node in _openList) {
				int score = node.GetScore();
				if(score > min) {
					// スコアが大きい
					continue;
				}
				if(score == min && node.Cost >= minCost) {
					// スコアが同じときは実コストも比較する
					continue;
				}

				// 最小値更新.
				min = score;
				minCost = node.Cost;
				minNode = node;
			}
			return minNode;
		}
	}

	/// ランダムな座標を取得する.
	Point2D GetRandomPosition(Array2D layer) {
		Point2D p;
		while(true) {
			p.x = Random.Range(0, layer.Width);
			p.y = Random.Range(0, layer.Height);
			if(layer.Get(p.x, p.y) == 1) {
				// 通過可能
				break;
			}
		}
		return p;
	}

	List<Point2D> _pList = null;
	public List<Point2D> GetPathList() {
		return _pList;
	}
	public eDir GetNextDir() {
		if (_pList.Count < 2) {
			return eDir.None;
		}
		Point2D p1 = _pList [0];
		Point2D p2 = _pList [1];
		return DirUtil.ToDir (p1 - p2);
	}

	/// <summary>
	/// Start から Goal までの経路を計算する
	/// </summary>
	/// <param name="layer">Layer.</param>
	/// <param name="StartX">Start x.</param>
	/// <param name="StartY">Start y.</param>
	/// <param name="GoalX">Goal x.</param>
	/// <param name="GoalY">Goal y.</param>
	public bool Calculate(Array2D layer, int StartX, int StartY, int GoalX, int GoalY) {

		_pList = null;
		var pList = new List<Point2D>();
		// A-star実行.
		{
			// スタート地点.
			Point2D pStart = new Point2D(StartX, StartY);
			// ゴール.
			Point2D pGoal = new Point2D(GoalX, GoalY);

			// 斜め移動を許可
			var allowdiag = false;
			var mgr = new ANodeMgr(layer, pGoal.x, pGoal.y, allowdiag);

			// スタート地点のノード取得
			// スタート地点なのでコストは「0」
			ANode node = mgr.OpenNode(pStart.x, pStart.y, 0, null);
			mgr.AddOpenList(node);

			// 試行回数。1000回超えたら強制中断
			int cnt = 0;
			while(cnt < 1000) {
				mgr.RemoveOpenList(node);
				// 周囲を開く
				mgr.OpenAround(node);
				// 最小スコアのノードを探す.
				node = mgr.SearchMinScoreNodeFromOpenList();
				if(node == null) {
					// 袋小路なのでおしまい.
					if (DEBUG_DUMP) {
						Debug.Log ("Not found path.");
					}
					return false;
				}
				if(node.X == pGoal.x && node.Y == pGoal.y) {
					// ゴールにたどり着いた.
					if (DEBUG_DUMP) {
						Debug.Log ("Success.");
					}
					mgr.RemoveOpenList(node);
					if (DEBUG_DUMP) {
						node.DumpRecursive ();
					}
					// パスを取得する
					node.GetPath(pList);
					// 反転する
					pList.Reverse();

					// 探索成功
					_pList = pList;
					return true;
				}
			}
		}

		// 探索失敗
		return false;
	}
}
