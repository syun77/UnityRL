using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵管理
/// </summary>
public class EnemyManager : MonoBehaviour {

  const int MAX_ENEMY = 8;
  public static EnemyManager instance = null;

  public static void Create() {
    instance = GetInstance ();
    instance._Create ();
    Add (7, 8);
  }

  public static Enemy Add(int xgrid, int ygrid) {
    foreach(var e in instance._pool) {
      if (e.enabled == false) {
        e.Create (1, eDir.Down);
        e.Warp (xgrid, ygrid, eDir.Down);
        e.enabled = true;
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
      _pool [i].enabled = false;
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
