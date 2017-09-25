using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵管理
/// </summary>
public class EnemyManager : MonoBehaviour {

  /// <summary>
  /// 敵の最大数
  /// </summary>
  public const int MAX_ENEMY = 8;

  /// <summary>
  /// ForEach関数に渡す関数の型
  /// </summary>
  public delegate void FuncT(Enemy e);
	public delegate bool FuncCheckT (Enemy e);

  public static EnemyManager instance = null;

  public static void Create() {
    instance = GetInstance ();
    instance._Create ();
  }

  /// <summary>
  /// 全ての敵を移動させる
  /// </summary>
  public static void MoveAll() {
    ForEachExists (((Enemy e) => {
      if (e.State == Actor.eState.MoveBegin) {
        e.BeginMove ();
      }
    }));
  }

  /// <summary>
  /// 全て更新する
  /// </summary>
  public static void ProcAll() {
    ForEachExists((Enemy e) => e.Proc());
  }

  /// <summary>
  /// 全て消す
  /// </summary>
  public static void KillAll() {
		ForEachExists ((Enemy e) => e.Kill());
  }

  /// <summary>
  /// 生存数を計算する
  /// </summary>
  public static int Count() {
    int cnt = 0;
    ForEachExists ((Enemy e) => cnt++);
    return cnt;
  }

	/// <summary>
	/// 指定座標に敵が存在するかどうか
	/// </summary>
	/// <returns><c>true</c>, if to grip position was existsed, <c>false</c> otherwise.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	public static bool ExistsToGridPosition(int x, int y) {
		return ForEachExistsCheck (((Enemy e) => {
			if(e.ExistsGrid(x, y)) {
				return true;
			}
			return false;
		}));
	}
	public static bool ExistsToGridNextPosition(int x, int y) {
		return ForEachExistsCheck (((Enemy e) => {
			if(e.ExistsNext(x, y)) {
				return true;
			}
			return false;
		}));
	}

  public static void ForEachExists(FuncT func) {
    foreach (var e in instance._pool) {
      if (e.enabled) {
        func (e);
      }
    }
  }
	public static bool ForEachExistsCheck(FuncCheckT func) {
		foreach (var e in instance._pool) {
			if (e.enabled) {
				if (func (e)) {
					return true;
				}
			}
		}

		return false;
	}

  public static Enemy Add(int id, int xgrid, int ygrid) {
    foreach(var e in instance._pool) {
      if (e.enabled == false) {
        e.Create (id, eDir.Down);
        e.Warp (xgrid, ygrid, eDir.Down);
				e.Revive ();
        return e;
      }
    }

    return null;
  }

  static EnemyManager GetInstance() {
    var obj = GameObject.Find ("EnemyManager");
    return obj.GetComponent<EnemyManager> ();
  }

  Enemy[] _pool = null;

  void _Create() {
    var prefab = Util.GetPrefab (null, "Enemy");
    _pool = new Enemy[MAX_ENEMY];
    for(int i = 0; i < _pool.Length; i++) {
      var obj = GameObject.Instantiate (prefab);
      _pool [i] = obj.GetComponent<Enemy> ();
			_pool [i].Kill ();
    }
  }

	// Use this for initialization
	void Start () {
    Create ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
